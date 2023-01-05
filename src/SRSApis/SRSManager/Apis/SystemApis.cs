using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using SrsManageCommon.SrsManageCommon;
using Common = SRSApis.Common;

namespace SrsApis.SrsManager.Apis
{
    public static class SystemApis
    {
        /// <summary>
        /// Delete a SrsInstance
        /// </summary>
        /// <param name="devid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool DelSrsInstanceByDeviceId(string devid, out ResponseStruct rs)
        {
            if (Common.SrsManagers == null) Common.SrsManagers = new List<SrsManager>();
            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(devid.Trim().ToUpper()));
            if (ret != null)
            {
                Common.SrsManagers.Remove(ret);
                if (ret.Srs != null && (ret.IsRunning || ret.IsInit))
                {
                    //Stop the srs process
                    while (ret.IsRunning)
                    {
                        ret.Stop(out rs);
                        Thread.Sleep(100);
                    }
                }

                File.Delete(ret.SrsConfigPath);
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.None,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                };
                return true;
            }
            else
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                };
                return false;
            }
        }

        /// <summary>
        /// Check whether the various ports of the newly created SRS instance have conflicts
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private static bool CheckNewSrsInstanceListenRight(SrsManager sm, out ResponseStruct rs)
        {
            if (sm == null || sm.Srs == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return false;
            }

            var port = sm.Srs.Listen;
            if (port == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return false;
            }

            var ret = Common.SrsManagers.FindLast(x => x.Srs.Listen == port);
            if (ret != null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsInstanceListenExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsInstanceListenExists],
                };
                return false;
            }

            if (sm.Srs.Http_api != null && sm.Srs.Http_api.Listen != null)
            {
                port = sm.Srs.Http_api.Listen;
                ret = Common.SrsManagers.FindLast(x => x.Srs.Http_api!.Listen == port);
                if (ret != null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsInstanceHttpApiListenExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsInstanceHttpApiListenExists],
                    };
                    return false;
                }
            }

            if (sm.Srs.Http_server != null && sm.Srs.Http_server.Listen != null)
            {
                port = sm.Srs.Http_server.Listen;
                ret = Common.SrsManagers.FindLast(x => x.Srs.Http_server!.Listen == port);
                if (ret != null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsInstanceHttpServerListenExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsInstanceHttpServerListenExists],
                    };
                    return false;
                }
            }

            if (sm.Srs.Rtc_server != null && sm.Srs.Rtc_server.Listen != null)
            {
                port = sm.Srs.Rtc_server.Listen;
                ret = Common.SrsManagers.FindLast(x => x.Srs.Rtc_server!.Listen == port);
                if (ret != null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsInstanceRtcServerListenExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsInstanceRtcServerListenExists],
                    };
                    return false;
                }
            }

            if (sm.Srs.Srt_server != null && sm.Srs.Srt_server.Listen != null)
            {
                port = sm.Srs.Srt_server.Listen;
                ret = Common.SrsManagers.FindLast(x => x.Srs.Srt_server!.Listen == port);
                if (ret != null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsInstanceSrtServerListenExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsInstanceSrtServerListenExists],
                    };
                    return false;
                }
            }

            if (sm.Srs.Stream_casters != null && sm.Srs.Stream_casters.Count > 0)
            {
                foreach (var caster in sm.Srs.Stream_casters)
                {
                    if (caster != null && caster.Listen != null)
                    {
                        foreach (var srs in Common.SrsManagers)
                        {
                            foreach (var sc in srs.Srs.Stream_casters!)
                            {
                                if (sc != null)
                                {
                                    if (sc.Listen == caster.Listen)
                                    {
                                        rs = new ResponseStruct()
                                        {
                                            Code = ErrorNumber.SrsInstanceStreamCasterListenExists,
                                            Message = ErrorMessage.ErrorDic![
                                                ErrorNumber.SrsInstanceStreamCasterListenExists],
                                        };
                                        return false;
                                    }

                                    if (caster.sip != null && sc.sip != null && caster.sip.Listen != null &&
                                        sc.sip.Listen != null)
                                    {
                                        if (caster.sip.Listen == sc.sip.Listen)
                                        {
                                            rs = new ResponseStruct()
                                            {
                                                Code = ErrorNumber.SrsInstanceStreamCasterSipListenExists,
                                                Message = ErrorMessage.ErrorDic![
                                                    ErrorNumber.SrsInstanceStreamCasterSipListenExists],
                                            };
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            return true;
        }

        /// <summary>
        /// Check whether the various paths of the newly created srs process instance are normal
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private static bool CheckNewSrsInstancePathRight(SrsManager sm, out ResponseStruct rs)
        {
            if (sm == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return false;
            }

            var devId = sm.SrsDeviceId;
            var confPath = sm.SrsConfigPath;
            if (string.IsNullOrEmpty(devId) || string.IsNullOrEmpty(confPath))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return false;
            }

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(devId.Trim().ToUpper()));
            if (ret != null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsInstanceExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsInstanceExists],
                };
                return false;
            }

            ret = Common.SrsManagers.FindLast(x =>
                x.SrsConfigPath.Trim().ToUpper().Equals(confPath.Trim().ToUpper()));
            if (ret != null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsInstanceConfigPathExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsInstanceConfigPathExists],
                };
                return false;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            return true;
        }

        /// <summary>
        /// Get the srs instance template
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static SrsManager GetSrsInstanceTemplate(out ResponseStruct rs)
        {
            var srsManager = new SrsManager();
            srsManager.Srs = new SrsSystemConfClass();

            srsManager.Srs = new SrsSystemConfClass();
            srsManager.Srs.Listen = 1935;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                srsManager.Srs.Max_connections = 1000;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                srsManager.Srs.Max_connections = 128;
            }
            else
            {
                srsManager.Srs.Max_connections = 512;
            }

            srsManager.SrsDeviceId = SrsManageCommon.Common.CreateUuid()?.Trim()!;
            srsManager.SrsWorkPath = Common.WorkPath;
            srsManager.Srs.Srs_log_file = srsManager.SrsWorkPath + srsManager.SrsDeviceId + "/srs.log";
            srsManager.Srs.Srs_log_level = "verbose"; //Observer initially
            srsManager.Srs.Pid = srsManager.SrsWorkPath + srsManager.SrsDeviceId + "/srs.pid";
            srsManager.Srs.Chunk_size = 6000;
            srsManager.Srs.Ff_log_dir = srsManager.SrsWorkPath + srsManager.SrsDeviceId + "/ffmpegLog/";
            srsManager.Srs.Ff_log_level = "warning";
            srsManager.Srs.Daemon = true;
            srsManager.Srs.Utc_time = false;
            srsManager.Srs.Work_dir = srsManager.SrsWorkPath;
            srsManager.Srs.Asprocess = false; //If the parent process is closed, if false, srs will not be closed
            srsManager.Srs.Inotify_auto_reload = false; //Configuration file modification does not automatically reload
            srsManager.Srs.Srs_log_tank = "file";
            srsManager.Srs.Grace_start_wait = 2300;
            srsManager.Srs.Grace_final_wait = 3200;
            srsManager.Srs.Force_grace_quit = false;
            srsManager.Srs.Http_api = new SrsHttpApiConfClass();
            srsManager.Srs.Http_api.Crossdomain = true;
            srsManager.Srs.Http_api.Enabled = true;
            srsManager.Srs.Http_api.Listen = 8000;
            srsManager.Srs.Http_api.InstanceName = "";
            srsManager.Srs.Http_api.SectionsName = "http_api";
            /*srsManager.Srs.Http_api.Raw_Api = new RawApi();
            srsManager.Srs.Http_api.Raw_Api.Allow_query = true;
            srsManager.Srs.Http_api.Raw_Api.Allow_reload = true;
            srsManager.Srs.Http_api.Raw_Api.Allow_update = true;
            srsManager.Srs.Http_api.Raw_Api.SectionsName = "raw_api";
            srsManager.Srs.Http_api.Raw_Api.Enabled = true;*/
            srsManager.Srs.Heartbeat = new SrsHeartbeatConfClass();
            srsManager.Srs.Heartbeat.Device_id = SrsManageCommon.Common.AddDoubleQuotation(srsManager.SrsDeviceId !);
            srsManager.Srs.Heartbeat.Enabled = true;
            srsManager.Srs.Heartbeat.SectionsName = "heartbeat";
            srsManager.Srs.Heartbeat.Interval = 5; //in seconds
            srsManager.Srs.Heartbeat.Summaries = true;
            srsManager.Srs.Heartbeat.Url = "http://127.0.0.1:5000/api/v1/heartbeat";
            srsManager.Srs.Http_server = new SrsHttpServerConfClass();
            srsManager.Srs.Http_server.Enabled = true;
            srsManager.Srs.Http_server.Dir = srsManager.SrsWorkPath + srsManager.SrsDeviceId + "/wwwroot";
            srsManager.Srs.Http_server.Listen = 8001;
            srsManager.Srs.Http_server.SectionsName = "http_server";
            srsManager.Srs.Http_server.Crossdomain = true;
            srsManager.Srs.Vhosts = new List<SrsvHostConfClass>();
            var vhost = new SrsvHostConfClass();
            vhost.SectionsName = "vhost";
            vhost.VhostDomain = "__defaultVhost__";
            srsManager.Srs.Vhosts.Add(vhost);
            rs = new ResponseStruct();
            rs.Code = ErrorNumber.None;
            rs.Message = ErrorMessage.ErrorDic![ErrorNumber.None];
            return srsManager;
        }

        /// <summary>
        /// Create a new SRS instance
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static SrsManager CreateNewSrsInstance(SrsManager sm, out ResponseStruct rs)
        {
            if (Common.SrsManagers == null) Common.SrsManagers = new List<SrsManager>();
            if (!CheckNewSrsInstancePathRight(sm, out rs)) //Check if the path is normal
            {
                return null!;
            }

            if (!CheckNewSrsInstanceListenRight(sm, out rs)) //Check whether the listening port is normal
            {
                return null!;
            }

            if (!sm.CreateSrsManagerSelf(out rs))
            {
                return null!;
            }

            return sm;
        }

        /// <summary>
        /// Refresh srs configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool RefreshSrsObject(string deviceId, out ResponseStruct rs)
        {
            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null)
            {
                return Common.RefreshSrsObject(ret, out rs);
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Obtain the list of SRS instance device IDs in the system
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllSrsManagerDeviceId()
        {
            List<string> list = null!;
            if (Common.SrsManagers.Count > 0)
            {
                list = new List<string>();
            }
            else
            {
                return null!;
            }

            foreach (var srs in Common.SrsManagers)
            {
                if (srs != null)
                {
                    list.Add(srs.SrsDeviceId);
                }
            }

            return list;
        }

        /// <summary>
        /// Get an instance of SRSManage
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static SrsManager GetSrsManagerInstanceByDeviceId(string deviceId)
        {
            foreach (var srs in Common.SrsManagers)
            {
                if (srs != null)
                {
                    if (srs.SrsDeviceId.Equals(deviceId)) return srs;
                }
            }

            return null!;
        }


        /// <summary>
        /// Get disk information in the system
        /// </summary>
        /// <returns></returns>
        public static List<DriveDiskInfo> GetDriveDisksInfo()
        {
            var disks = new List<DriveDiskInfo>();
            var drives = DriveInfo.GetDrives();
            foreach (var d in drives)
            {
                var ddi = new DriveDiskInfo()
                {
                    Format = d.DriveFormat,
                    VolumeLabel = d.VolumeLabel,
                    Free = (ulong) d.AvailableFreeSpace / 1000 / 1000,
                    Size = (ulong) d.TotalSize / 1000 / 1000,
                    Path = d.Name,
                    RootDirectory = d.RootDirectory.FullName,
                };
                if (ddi.Size > 0 && ddi.Free > 0)
                {
                    disks.Add(ddi);
                }
            }

            return disks;
        }

        /// <summary>
        /// Get system platform information
        /// </summary>
        /// <returns></returns>
        public static SystemInfoModule GetSystemInfo()
        {
            var sim = new SystemInfoModule();
            sim.NetworkInterfaceList = GetNetworkAdapterList();
            sim.Architecture = RuntimeInformation.OSArchitecture.ToString();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                sim.Platform = "linux";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                sim.Platform = "windows";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                sim.Platform = "Mac/OSX";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                sim.Platform = "freebsd";
            sim.DisksInfo = GetDriveDisksInfo();
            sim.Version = RuntimeInformation.OSDescription + " " + Environment.OSVersion;
            sim.X64 = Environment.Is64BitOperatingSystem;
            sim.HostName = Environment.MachineName;
            sim.CpuCoreSize = (ushort) Environment.ProcessorCount;

            if (Common.SrsManagers != null && Common.SrsManagers.Count > 0)
            {
                foreach (var sm in Common.SrsManagers)
                {
                    if (sm.IsRunning && sm.Srs.Http_api != null && sm.Srs.Http_api.Enabled == true)
                    {
                        var reqUrl = "http://127.0.0.1:" + sm!.Srs.Http_api!.Listen + "/api/v1/summaries";
                        try
                        {
                            var tmpStr = NetHelperNew.HttpGetRequest(reqUrl, null!);
                            var retReq = JsonHelper.FromJson<SrsSystemInfo>(tmpStr);
                            if (retReq != null && retReq.Data != null && retReq.Data.Self != null)
                            {
                                if (sim.SrsList == null) sim.SrsList = new List<Self_Srs>();
                                var filename = Path.GetFileName(retReq.Data.Self.Argv)!;
                                var ext = Path.GetExtension(filename);
                                retReq.Data.Self.Srs_DeviceId = filename.Replace(ext, "");
                                sim.SrsList.Add(retReq.Data.Self);
                                if (sim.System == null)
                                {
                                    sim.System = retReq.Data.System;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                    Thread.Sleep(50);
                }
            }

            return sim;
        }

        /// <summary>
        /// Get system network information
        /// </summary>
        /// <returns></returns>
        public static List<NetworkInterfaceModule> GetNetworkAdapterList()
        {
            var listofnetwork = new List<NetworkInterfaceModule>();
            var adapters = NetworkInterface.GetAllNetworkInterfaces();
            var ipaddr = "";
            ushort index = 0;
            if (adapters.Length > 0)
            {
                foreach (var adapter in adapters)
                {
                    if (adapter.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
                    ipaddr = "";
                    var adapterProperties = adapter.GetIPProperties();
                    var ipCollection = adapterProperties.UnicastAddresses;
                    foreach (var ipadd in ipCollection)
                    {
                        if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipaddr = ipadd.Address.ToString(); //get ip
                        }
                        else if (ipadd.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            //Local IPV6 address, do not take IPV6 address
                        }
                    }

                    var tmp_adapter = new NetworkInterfaceModule()
                    {
                        Index = index,
                        Name = adapter.Name,
                        //  Speed = (adapter.Speed / 1000 / 1000).ToString() + "MB", //linux not Supported
                        Mac = adapter.GetPhysicalAddress().ToString(),
                        Type = adapter.NetworkInterfaceType.ToString(),
                        Ipaddr = ipaddr,
                    };
                    index++;
                    if (!string.IsNullOrEmpty(tmp_adapter.Ipaddr))
                    {
                        var loop = 1;
                        if (!tmp_adapter.Mac.Contains('-'))
                        {
                            for (var i = 1; i < 10; i += 2)
                            {
                                tmp_adapter.Mac = tmp_adapter.Mac.Insert(i + loop, "-");
                                loop += 1;
                            }
                        }

                        listofnetwork.Add(tmp_adapter);
                    }
                }

                return listofnetwork;
            }

            return null!;
        }
    }
}