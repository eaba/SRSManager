using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SrsApis.SrsManager.Apis;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManageCommon.DBMoudle;
using SRSManageCommon.ManageStructs;

namespace SRSApis.SystemAutonomy
{
    public class MonitorStruct
    {
        private string? _deviceId;
        private string? _vhostDomain;
        private string? _app;
        private string? _stream;
        private string? _filename;
        private BlockingCollection<byte> _highFrequencyUpdateList = new BlockingCollection<byte>(100);
        private int _timeoutTimes = 0;

        public string? DeviceId
        {
            get => _deviceId;
            set => _deviceId = value;
        }

        public string? VhostDomain
        {
            get => _vhostDomain;
            set => _vhostDomain = value;
        }

        public string? App
        {
            get => _app;
            set => _app = value;
        }

        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }

        public BlockingCollection<byte> HighFrequencyUpdateList
        {
            get => _highFrequencyUpdateList;
            set => _highFrequencyUpdateList = value ?? throw new ArgumentNullException(nameof(value));
        }

        public int TimeOutTimes
        {
            get => _timeoutTimes;
            set => _timeoutTimes = value;
        }

        public string? Filename
        {
            get => _filename;
            set => _filename = value;
        }

        public MonitorStruct(string _deviceId, string _vhostDomain, string _app, string _stream, string _filename)
        {
            DeviceId = _deviceId;
            VhostDomain = _vhostDomain;
            App = _app;
            Stream = _stream;
            Filename = _filename;
            Task.Factory.StartNew(() =>
            {
                foreach (var value in HighFrequencyUpdateList.GetConsumingEnumerable())
                {
                    Thread.Sleep(10000); //Artificial delay of 100 milliseconds, delay if there is content, block if there is no content
                    TimeOutTimes = TimeOutTimes > 0 ? TimeOutTimes-- : 0;
                }
            });
        }
    }

    /// <summary>
    /// It is found that if ffmpeg exits abnormally or in other cases, srs will automatically monitor and restart the ffmpeg process
    /// Only when ffmpeg does not exit, but the stream is abnormal, srs cannot actively discover and handle such a situation
    /// Therefore, this method uses monitoring ffmpeg log writing to judge whether the streaming is normal.When ffmpeg streaming
    /// When an abnormal situation occurs, the log will be written crazily, and the log content is very small when the stream is pulled normally, so this class has done
    /// A system-level file change monitoring, when a file change occurs, write a status to the blocking queue, blocking
    /// The queue will process this state, waiting for 100 milliseconds each time it is processed, and the blocking queue has a number
    /// When the quantity goes online, when the quantity goes online, it means that the processing every 100 milliseconds can no longer keep up with the generation of logs.
    /// It can probably show that the ffmpeg log is being generated crazily, so it appears many times (the number determined here is 100 times)
    /// In this case, you need to restart the ingest streamer to solve this problem, which is how this class works.
    /// </summary>
    public class IngestMonitor
    {
        private string _ffmpegLogPath;
        private string _logExt;
        private string? _deviceId;
        private List<MonitorStruct> _monitorStructList = new List<MonitorStruct>();
        private FileSystemWatcher _watcher = new FileSystemWatcher();

        public string FfmpegLogPath
        {
            get => _ffmpegLogPath;
            set => _ffmpegLogPath = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string LogExt
        {
            get => _logExt;
            set => _logExt = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string? DeviceId
        {
            get => _deviceId;
            set => _deviceId = value;
        }

        public List<MonitorStruct> MonitorStructList
        {
            get => _monitorStructList;
            set => _monitorStructList = value ?? throw new ArgumentNullException(nameof(value));
        }

        public FileSystemWatcher Watcher
        {
            get => _watcher;
            set => _watcher = value;
        }

        private void RestartIngest(string deviceId, string vhostDomain, Ingest ingest)
        {
            LogWriter.WriteLog("Reboot Device ID:" + deviceId + "under" + vhostDomain + "under" + ingest.IngestName + " Ingest");

            lock (SrsManageCommon.Common.LockDbObjForOnlineClient)
            {
                OrmService.Db.Delete<OnlineClient>().Where(x => x.RtspUrl == ingest.Input!.Url).ExecuteAffrows();
            }

            var retInt = FoundProcess(ingest);
            if (retInt > -1)
            {
                try
                {
                    string cmd = "kill -9 " + retInt.ToString();
                    LinuxShell.Run(cmd, 1000);
                }
                catch (Exception ex)
                {
                    LogWriter.WriteLog(
                        "Reboot Device ID:" + deviceId + "under" + vhostDomain + "under" + ingest.IngestName + " fail Ingest",
                        ex.Message + "\r\n" + ex.StackTrace, ConsoleColor.Yellow);
                }
            }

            ResponseStruct rs = null!;
            VhostIngestApis.OnOrOffIngest(deviceId, vhostDomain, ingest.IngestName!, false, out rs);
            SystemApis.RefreshSrsObject(deviceId, out rs);
            Thread.Sleep(1000);
            VhostIngestApis.OnOrOffIngest(deviceId, vhostDomain, ingest.IngestName!, true, out rs);
            SystemApis.RefreshSrsObject(deviceId, out rs);
        }

        private int FoundProcess(Ingest ingest)
        {
            string url = ingest.Input!.Url!.Replace("&", @"\&");
            string cmd = "ps  -aux |grep " + url + "|grep -v grep |awk '{print $2}'";
            LinuxShell.Run(cmd, 1000, out string sdt, out string err);
            if (string.IsNullOrEmpty(sdt) && string.IsNullOrEmpty(err))
            {
                return -1;
            }

            if (int.TryParse(sdt, out int i))
            {
                return i;
            }

            if (int.TryParse(err, out int j))
            {
                return j;
            }

            return -1;
        }


        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var find = MonitorStructList.FindLast(x => x.Filename!.Equals(e.Name));
            if (find == null)
            {
                string[] strArr = e.Name.Split('-', StringSplitOptions.RemoveEmptyEntries);
                string app = "";
                string vhost = "";
                string stream = "";
                if (strArr.Length >= 5)
                {
                    stream = strArr[4].TrimEnd(LogExt.ToCharArray()).Trim();
                    app = strArr[3].Trim();
                    vhost = strArr[2].Trim();
                }

                MonitorStructList.Add(new MonitorStruct(DeviceId!, vhost, app, stream, e.Name));
            }
            else
            {
                if (!find.HighFrequencyUpdateList.TryAdd(1, 50)) //Tried to add, timed out 50ms
                {
                    find.TimeOutTimes++; //Timed out, timed out times +1
                }

                if (find.TimeOutTimes >= find.HighFrequencyUpdateList.BoundedCapacity) //If the number of timeouts is greater than the maximum number of queues, it is necessary to restart the ingest
                {
                    Watcher.EnableRaisingEvents = false;
                    string[] strArr = e.Name.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    string app = "";
                    string vhost = "";
                    string stream = "";
                    if (strArr.Length >= 5)
                    {
                        stream = strArr[4].TrimEnd(LogExt.ToCharArray()).Trim();
                        app = strArr[3].Trim();
                        vhost = strArr[2].Trim();
                    }
                    for (int i = 0; i <= MonitorStructList.Count - 1; i++)
                    {
                        if (MonitorStructList[i].Filename!.Equals(find.Filename))
                        {
                            MonitorStructList[i].HighFrequencyUpdateList.Dispose();
                            MonitorStructList[i] = null!;
                            break;
                        }
                    }
                    SrsManageCommon.Common.RemoveNull(MonitorStructList);
                    Ingest ingest =
                        VhostIngestApis.GetVhostIngest(DeviceId!, vhost, stream, out ResponseStruct rs);
                    if (ingest != null)
                    {
                        RestartIngest(DeviceId!, vhost, ingest!);

                        LogWriter.WriteLog("Monitoring found that there is an Ingest exception，perform restart...",
                            string.Format("DeviceId:{0},Vhost:{1},App:{2},Stream:{3}", DeviceId, vhost, app, stream),
                            ConsoleColor.Yellow);
                    }
                    Watcher.EnableRaisingEvents = true;
                }
            }
        }

        public IngestMonitor(string ffmpegLogPath, string logExt, string? deviceId)
        {
            _ffmpegLogPath = ffmpegLogPath;
            _logExt = logExt;
            _deviceId = deviceId;
            Watcher.Path = FfmpegLogPath;
            Watcher.IncludeSubdirectories = false;
            Watcher.Filter = "*" + LogExt;
            Watcher.Changed += OnChanged;
            Watcher.EnableRaisingEvents = true;
        }
    }
}