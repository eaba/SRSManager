using System.Collections.Generic;
using System.Linq;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using Common = SRSApis.Common;

namespace SrsApis.SrsManager.Apis
{
    public static class VhostApis
    {
        /// <summary>
        /// Obtain the list of Instance names of the Vhost list
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<string> GetVhostsInstanceName(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null)
            {
                if (ret.Srs == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    return null!;
                }

                if (ret.Srs.Vhosts == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return null!;
                }

                return ret.Srs.Vhosts.Select(x => x.InstanceName).ToList()!;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return null!;
        }

        /// <summary>
        /// Get vhost by domain
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static SrsvHostConfClass GetVhostByDomain(string deviceId, string vhostDomain, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null)
            {
                if (ret.Srs == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    return null!;
                }

                if (ret.Srs.Vhosts == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return null!;
                }

                var retVhost = ret.Srs.Vhosts.FindLast(x =>
                    x.VhostDomain!.Trim().ToUpper().Equals(vhostDomain.Trim().ToUpper()))!;
                if (retVhost != null)
                {
                    return retVhost;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                };
                return null!;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return null!;
        }

        /// <summary>
        /// Get list of Vhosts
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<SrsvHostConfClass> GetVhostList(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null)
            {
                if (ret.Srs == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    return null!;
                }

                if (ret.Srs.Vhosts == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return null!;
                }

                return ret.Srs.Vhosts;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return null!;
        }


        /// <summary>
        /// Get various templates of Vhost
        /// </summary>
        /// <param name="vtype"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static SrsvHostConfClass GetVhostTemplate(VhostIngestInputType vtype, out ResponseStruct rs)
        {
            var vhost = new SrsvHostConfClass();
            vhost.Vingests = new List<Ingest>();
            vhost.InstanceName = "your.domain.com"; //change it 
            vhost.VhostDomain = vhost.InstanceName;
            vhost.SectionsName = "vhost";
            vhost.Enabled = false;
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None] + "\r\n" + JsonHelper.ToJson(vtype),
            };
            switch (vtype)
            {
                case VhostIngestInputType.WebCast://add for live
                    var dvr= new Dvr();
                    dvr.Dvr_apply = "all";
                    dvr.Enabled = true;
                    dvr.Dvr_plan = "segment";
                    dvr.Dvr_path = "请替换";
                    dvr.Dvr_duration = 120;
                    dvr.Dvr_wait_keyframe = true;
                    dvr.Time_Jitter = PlayTimeJitter.full;
                    var httpHooks = new HttpHooks();
                    httpHooks.Enabled = true;
                    //httpHooks.On_connect = "http://127.0.0.1:5800/SrsHooks/OnConnect";
                    httpHooks.On_publish = "http://127.0.0.1:5800/SrsHooks/OnPublish";
                    //httpHooks.On_close = "http://127.0.0.1:5800/SrsHooks/OnClose";
                    httpHooks.On_play = "http://127.0.0.1:5800/SrsHooks/OnPlay";
                    httpHooks.On_unpublish = "http://127.0.0.1:5800/SrsHooks/OnUnPublish";
                    httpHooks.On_stop = "http://127.0.0.1:5800/SrsHooks/OnStop";
                    httpHooks.On_dvr = "http://127.0.0.1:5800/SrsHooks/OnDvr";
                    httpHooks.On_hls = "http://127.0.0.1:5800/SrsHooks/Test";
                    httpHooks.On_hls_notify = "http://127.0.0.1:5800/SrsHooks/Test";
                    var httpRemux= new HttpRemux();
                    httpRemux.Enabled = true;
                    httpRemux.Fast_cache = 30;
                    httpRemux.Mount = "[vhost]/[app]/[stream].flv";
                    vhost.Vdvr = dvr;
                    vhost.Vhttp_hooks = httpHooks;
                    vhost.Vhttp_remux = httpRemux;
                    return vhost;
                case VhostIngestInputType.Stream: //RTSP or other Stream
                    var ing = new Ingest();
                    ing.Enabled = true;
                    ing.InstanceName = "ingest_template_rtsp";
                    ing.IngestName = ing.InstanceName;
                    ing.SectionsName = "ingest";
                    var inginput = new IngestInput();
                    var ingeng = new IngestTranscodeEngine();
                    ing.Engines = new List<IngestTranscodeEngine>();
                    ingeng.Perfile = new IngestEnginePerfile();
                    ingeng.Perfile.SectionsName = "perfile";
                    ingeng.Enabled = true;
                    inginput.SectionsName = "input";
                    inginput.Type = IngestInputType.stream;
                    inginput.Url =
                        "rtsp://admin:12345678@192.168.2.21:554/Streaming/Channels/501?transportmode=unicast";
                    ing.Input = inginput;
                    ing.Ffmpeg = SrsManageCommon.Common.FFmpegBinPath;
                    ingeng.SectionsName = "engine";
                    ingeng.Enabled = true;
                    ingeng.Vcodec = "copy";
                    ingeng.Acodec = "copy";
                    ingeng.Perfile.Rtsp_transport = "tcp";
                    ingeng.Perfile.SectionsName = "perfile";
                    ingeng.Output = "rtmp://127.0.0.1/[vhost]/[live]/[livestream]";
                    ing.Engines.Add(ingeng);
                    ing.Input = inginput;
                    vhost.Vingests.Add(ing);
                    return vhost!;
                case VhostIngestInputType.File: //From File
                    var inginput1 = new IngestInput();
                    var ingeng1 = new IngestTranscodeEngine();
                    var ing1 = new Ingest();
                    ing1.Enabled = true;
                    ing1.Engines = new List<IngestTranscodeEngine>();
                    ing1.InstanceName = "ingest_template_file";
                    ing1.IngestName = ing1.InstanceName;
                    ing1.SectionsName = "ingest";
                    ingeng1.Enabled = true;
                    inginput1.SectionsName = "input";
                    inginput1.Type = IngestInputType.file;
                    inginput1.Url =
                        "./demo.mp4";
                    ing1.Input = inginput1;
                    ing1.Ffmpeg = SrsManageCommon.Common.FFmpegBinPath;
                    ingeng1.SectionsName = "engine";
                    ingeng1.Enabled = true;
                    ingeng1.Output = "rtmp://127.0.0.1/[vhost]/[live]/[livestream]";
                    ing1.Engines.Add(ingeng1);
                    ing1.Input = inginput1;
                    vhost.Vingests.Add(ing1);
                    return vhost!;
                case VhostIngestInputType.Device: //From Device
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsConfigFunctionUnsupported,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsConfigFunctionUnsupported] + "\r\n" +
                                  JsonHelper.ToJson(vtype),
                    };
                    return null!;
                default:
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.FunctionInputParamsError,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError] + "\r\n" +
                                  JsonHelper.ToJson(vtype),
                    };
                    return null!;
            }
        }

        /// <summary>
        /// Delete a vhost, use the domain name
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool DeleteVhostByDomain(string deviceId, string vhostDomain, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null)
            {
                if (ret.Srs == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    return false;
                }

                if (ret.Srs.Vhosts == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return false;
                }

                var retVhost = ret.Srs.Vhosts.FindLast(x =>
                    x.VhostDomain!.Trim().ToUpper().Equals(vhostDomain.Trim().ToUpper()))!;
                if (retVhost != null)
                {
                    return ret.Srs.Vhosts.Remove(retVhost);
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                };
                return false;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Modify the domain name of the vhost
        /// </summary>
        /// <param name="vhostDomain"></param>
        /// <param name="newVhostDomain"></param>
        /// <param name="rs"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static bool ChangeVhostDomain(string deviceId, string vhostDomain, string newVhostDomain,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null)
            {
                if (ret.Srs == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    return false;
                }

                if (ret.Srs.Vhosts == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return false;
                }

                var retVhost = ret.Srs.Vhosts.FindLast(x =>
                    x.VhostDomain!.Trim().ToUpper().Equals(vhostDomain.Trim().ToUpper()))!;

                var retVhostNew = ret.Srs.Vhosts.FindLast(x =>
                    x.VhostDomain!.Trim().ToUpper().Equals(newVhostDomain.Trim().ToUpper()))!;
                if (retVhost != null)
                {
                    if (retVhostNew != null)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.SrsSubInstanceAlreadyExists,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceAlreadyExists],
                        };
                        return false;
                    }

                    retVhost.VhostDomain = newVhostDomain;
                    return true;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                };
                return false;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Close or start an ingest in a Vhost in the Vhost list
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain">vhost的域名</param>
        /// <param name="ingestInstanceName">ingest的instancename</param>
        /// <param name="enabled">关闭或开启</param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool OnOrOffVhostIngest(string deviceId, string vhostDomain, string ingestInstanceName,
            bool enabled,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null)
            {
                if (ret.Srs == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    return false;
                }

                if (ret.Srs.Vhosts == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return false;
                }

                var retVhost = ret.Srs.Vhosts.FindLast(x =>
                    x.VhostDomain!.Trim().ToUpper().Equals(vhostDomain.Trim().ToUpper()));
                if (retVhost == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return false;
                }

                if (retVhost.Vingests == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return false;
                }

                var retVhostVingest = retVhost.Vingests.FindLast(x =>
                    x.InstanceName!.Trim().ToUpper().Equals(ingestInstanceName.Trim().ToUpper()));
                if (retVhostVingest == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return false;
                }

                retVhostVingest.Enabled = enabled;
                return true;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }

        /// <summary>
        /// Set the parameters of a Vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhost"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool SetVhost(string deviceId, SrsvHostConfClass vhost, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToUpper().Equals(deviceId.Trim().ToUpper()));
            if (ret != null)
            {
                if (ret.Srs == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsObjectNotInit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
                    };
                    return false;
                }

                if (ret.Srs.Vhosts == null)
                {
                    ret.Srs.Vhosts = new List<SrsvHostConfClass>();
                    ret.Srs.Vhosts.Add(vhost);
                    return true;
                }

                var retVhost = ret.Srs.Vhosts.FindLast(x =>
                    x.VhostDomain!.Trim().ToUpper().Equals(vhost.VhostDomain!.Trim().ToUpper()));


                if (retVhost == null)
                {
                    ret.Srs.Vhosts.Add(vhost);
                    return true;
                }

                ret.Srs.Vhosts[ret.Srs.Vhosts.IndexOf(retVhost)] = vhost;
                return true;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return false;
        }
    }
}