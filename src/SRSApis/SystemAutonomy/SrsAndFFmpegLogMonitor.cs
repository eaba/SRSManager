using System;
using System.IO;
using System.Threading;
using SrsManageCommon;

namespace SRSApis.SystemAutonomy
{
    public class SrsAndFFmpegLogMonitor
    {
        private int interval = SrsManageCommon.Common.SystemConfig.SrsAdnffmpegLogMonitorServiceinterval;

        private void ProcessSrsFileMove(string srsFilePath)
        {
            string fileName = Path.GetFileName(srsFilePath);
            string dirPath = Path.GetDirectoryName(srsFilePath)!;
            if (!Directory.Exists(dirPath + "/logbak"))
            {
                Directory.CreateDirectory(dirPath + "/logbak");
            }

            fileName = dirPath + "/logbak/srslogback_" + DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss") + fileName;
            File.Copy(srsFilePath, fileName);
            LinuxShell.Run("cat /dev/null >" + srsFilePath);
            LogWriter.WriteLog("Dump the srs log and clear the existing log", srsFilePath + "->" + fileName);
        }

        private void ProcessFFmpegFileMove(string ffmpegFilePath)
        {
            string fileName = Path.GetFileName(ffmpegFilePath);
            string dirPath = Path.GetDirectoryName(ffmpegFilePath)!;
            if (!Directory.Exists(dirPath + "/logbak"))
            {
                Directory.CreateDirectory(dirPath + "/logbak");
            }

            fileName = dirPath + "/logbak/ffmpeglogback_" + DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss") + fileName;

            File.Copy(ffmpegFilePath, fileName);
            LinuxShell.Run("cat /dev/null >" + ffmpegFilePath);
            LogWriter.WriteLog("Dump the ffmpeg log and clear the existing log", ffmpegFilePath + "->" + fileName);
        }

       
        private void Run()
        {
            while (true)
            {
                foreach (var srs in Common.SrsManagers)
                {
                    if (srs != null && srs.IsRunning)
                    {
                        string srsLogFile = srs.Srs.Srs_log_file!;
                        string ffmpegLogPath = srs.Srs.Ff_log_dir!;
                        FileInfo srsLog = new FileInfo(srsLogFile);
                        if (srsLog.Exists && srsLog.Length >= (1024 * 1000 * 10)) //10M
                        {
                            ProcessSrsFileMove(srsLogFile);
                        }

                        DirectoryInfo ffmpegDir = new DirectoryInfo(ffmpegLogPath);
                        if (ffmpegDir.Exists)
                        {
                            foreach (var file in ffmpegDir.GetFiles())
                            {
                                if (file.FullName.Contains("ffmpeglogback")) continue;

                                if (file.Exists && file.Length >= 1024 * 1000 * 10) //10M
                                {
                                    ProcessFFmpegFileMove(file.FullName);
                                }

                                Thread.Sleep(500);
                            }
                        }
                    }

                    Thread.Sleep(1000);
                }


                Thread.Sleep(interval);
            }
        }

        public SrsAndFFmpegLogMonitor()
        {
            new Thread(new ThreadStart(delegate

            {
                try
                {
                    LogWriter.WriteLog("Start the log dump service... (cycle interval：" + interval + "ms)");
                    Run();
                }
                catch (Exception ex)
                {
                    LogWriter.WriteLog("Failed to start the log dump service...", ex.Message + "\r\n" + ex.StackTrace, ConsoleColor.Yellow);
                }
            })).Start();
        }
    }
}