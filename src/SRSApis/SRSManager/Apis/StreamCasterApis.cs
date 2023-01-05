using System.Collections.Generic;
using System.Linq;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using Common = SRSApis.Common;

namespace SrsApis.SrsManager.Apis
{
    public static class StreamCasterApis
    {
        /// <summary>
        /// Get the list of StreamCasterInstenceName
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<string> GetStreamCastersInstanceName(string deviceId, out ResponseStruct rs)
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

                if (ret.Srs.Stream_casters == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return null!;
                }

                var slist = ret.Srs.Stream_casters.Select(i => i.InstanceName).ToList()!;
                return slist!;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return null!;
        }

        /// <summary>
        /// Get the list of StreamCasters in the SrsManager instance
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<SrsStreamCasterConfClass> GetStreamCasterList(string deviceId, out ResponseStruct rs)
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

                if (ret.Srs.Stream_casters == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return null!;
                }

                return ret.Srs.Stream_casters!;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return null!;
        }

        /// <summary>
        /// Create a StreamCaster in the current SrsManager instance
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="streamCaster"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool CreateStreamCaster(string deviceId, SrsStreamCasterConfClass streamCaster,
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
                    return false!;
                }

                if (ret.Srs.Stream_casters == null)
                {
                    ret.Srs.Stream_casters = new List<SrsStreamCasterConfClass>();
                    ret.Srs.Stream_casters.Add(streamCaster);
                    return true;
                }

                var retStreamCaster = ret.Srs.Stream_casters.FindLast(x =>
                    x.InstanceName!.Trim().ToUpper().Equals(streamCaster.InstanceName!.Trim().ToUpper()));
                if (retStreamCaster == null)
                {
                    ret.Srs.Stream_casters.Add(streamCaster);
                    return true;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsSubInstanceAlreadyExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceAlreadyExists],
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
        /// Get various types of templates for StreamCaster
        /// </summary>
        /// <param name="casterType"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static SrsStreamCasterConfClass GetStreamCasterTemplate(CasterEnum casterType, out ResponseStruct rs)
        {
            var result = new SrsStreamCasterConfClass();
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None] + "\r\n" + JsonHelper.ToJson(casterType),
            };
            switch (casterType)
            {
                case CasterEnum.flv:
                    result.InstanceName = "streamcaster-flv-template";
                    result.SectionsName = "stream_caster";
                    result.sip = null;
                    result.Enabled = true;
                    result.Caster = CasterEnum.flv;
                    result.Listen = 8936;
                    result.Output = "rtmp://127.0.0.1/[vhost]/[app]/[stream]";
                    return result;
                case CasterEnum.gb28181:
                    result.InstanceName = "streamcaster-gb28181-template";
                    result.SectionsName = "stream_caster";
                    result.sip = new Sip();
                    result.sip.SectionsName = "sip";
                    result.sip.Enabled = true;
                    result.sip.Listen = 5060;
                    result.sip.Serial = "34020000002000000001"; //server-id
                    result.sip.Realm = "3402000000"; //server domain
                    result.sip.Ack_timeout = 30;
                    result.sip.Keepalive_timeout = 120;
                    result.sip.Auto_play = true;
                    result.sip.Invite_port_fixed = true;
                    result.sip.Query_catalog_interval = 60;
                    result.Enabled = true;
                    result.Caster = CasterEnum.gb28181;
                    result.Output = "rtmp://127.0.0.1/[vhost]/[app]/[stream]";
                    result.Listen = 9000;
                    result.Rtp_port_max = 58300;
                    result.Rtp_port_min = 58200;
                    result.Wait_keyframe = false;
                    result.Rtp_idle_timeout = 30;
                    result.Audio_enable = true; //Only supports acc format audio stream
                    result.Host = "*";
                    result.Auto_create_channel = false;
                    return result;
                case CasterEnum.mpegts_over_udp:
                    result.InstanceName = "streamcaster-mpegts_over_udp-template";
                    result.SectionsName = "stream_caster";
                    result.sip = null;
                    result.Enabled = true;
                    result.Caster = CasterEnum.mpegts_over_udp;
                    result.Listen = 1935;
                    result.Output = "rtmp://127.0.0.1/[vhost]/[app]/[stream]";
                    return result;
                case CasterEnum.rtsp:
                    result.InstanceName = "streamcaster-rtsp-template";
                    result.SectionsName = "stream_caster";
                    result.sip = null;
                    result.Enabled = true;
                    result.Caster = CasterEnum.rtsp;
                    result.Listen = 554;
                    result.Output = "rtmp://127.0.0.1/[vhost]/[app]/[stream]";
                    result.Rtp_port_min = 57200;
                    result.Rtp_port_max = 57300;
                    return result;
                default:
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.FunctionInputParamsError,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError] + "\r\n" +
                                  JsonHelper.ToJson(casterType),
                    };
                    return null!;
            }
        }

        /// <summary>
        /// Use InstanceName to delete StreamCaster
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="instanceName"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool DeleteStreamCasterByInstanceName(string deviceId, string instanceName, out ResponseStruct rs)
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

                if (ret.Srs.Stream_casters == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return false;
                }

                var retStreamCaster = ret.Srs.Stream_casters.FindLast(x =>
                    x.InstanceName!.Trim().ToUpper().Equals(instanceName!.Trim().ToUpper()));
                if (retStreamCaster == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return false;
                }

                ret.Srs.Stream_casters.Remove(retStreamCaster);
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
        /// Modify the instance name of StreamCaster
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="instanceName"></param>
        /// <param name="newInstanceName"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool ChangeStreamCasterInstanceName(string deviceId, string instanceName, string newInstanceName,
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

                if (ret.Srs.Stream_casters == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return false;
                }

                var retStreamCaster = ret.Srs.Stream_casters.FindLast(x =>
                    x.InstanceName!.Trim().ToUpper().Equals(instanceName!.Trim().ToUpper()));
                if (retStreamCaster == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return false;
                }

                var retStreamCasterNew = ret.Srs.Stream_casters.FindLast(x =>
                    x.InstanceName!.Trim().ToUpper().Equals(newInstanceName!.Trim().ToUpper()));
                if (retStreamCasterNew != null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceAlreadyExists,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceAlreadyExists],
                    };
                    return false;
                }

                retStreamCaster.InstanceName = newInstanceName;
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
        /// Modifying the start of the streamcaster will stop
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="instanceName"></param>
        /// <param name="enabled"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool OnOrOffStreamCaster(string deviceId, string instanceName, bool enabled,
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
                if (ret.Srs != null && ret.Srs.Stream_casters != null)
                {
                    var retStreamCaster = ret.Srs.Stream_casters.FindLast(x =>
                        x.InstanceName!.Trim().ToUpper().Equals(instanceName.Trim().ToUpper()));
                    if (retStreamCaster != null)
                    {
                        retStreamCaster.Enabled = enabled;
                        if (retStreamCaster.sip != null)
                        {
                            retStreamCaster.sip.Enabled = enabled;
                        }

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
        /// Configure the parameters of a StreamCaster in the SrsManager instance
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="streamCaster"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool SetStreamCaster(string deviceId, SrsStreamCasterConfClass streamCaster,
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
                if (ret.Srs != null && ret.Srs.Stream_casters != null)
                {
                    var retStreamCaster = ret.Srs.Stream_casters.FindLast(x =>
                        x.InstanceName!.Trim().ToUpper().Equals(streamCaster.InstanceName!.Trim().ToUpper()));
                    if (retStreamCaster != null) //Revise
                    {
                        ret.Srs.Stream_casters[ret.Srs.Stream_casters.IndexOf(retStreamCaster)] = streamCaster;
                        return true;
                    }

                    ret.Srs.Stream_casters.Add(streamCaster);
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
    }
}