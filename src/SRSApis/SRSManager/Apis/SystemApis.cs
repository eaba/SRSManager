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