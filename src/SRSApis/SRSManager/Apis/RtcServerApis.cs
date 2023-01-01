using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using Common = SRSApis.Common;

namespace SrsApis.SrsManager.Apis
{
    public static class RtcServerApis
    {
        /// <summary>
        /// Delete the RTCServer segment
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool DeleteRtcServer(string deviceId, out ResponseStruct rs)
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
                if (ret.Srs != null)
                {
                    ret.Srs.Rtc_server = null;
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

        public static SrsRtcServerConfClass GetRtcServerTemplate(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            return new SrsRtcServerConfClass()
            {
                Enabled = true,
                Listen = 8000,
                Candidate = "*",
                SectionsName = "rtc_server",
                Protocol = "udp",
                Tcp = new Tcp
                {
                    Enabled = false,
                    Listen = 8000
                },
                UseAutoDetectNetworkIp = true,
                IpFamily = "ipv4",
                ApiAsCandidates= true,
                ResolveApiDomain= true,               
                KeepApiDomain= false,
                Black_hole = new BlackHole()
                {
                    Enabled = false,
                    Addr = "127.0.0.1:10000",
                    SectionsName = "black_hole",
                }
            };
        }

        /// <summary>
        /// Start or stop RtcServer
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="enabled"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool OnOrOffRtcServer(string deviceId, bool enabled, out ResponseStruct rs)
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
                if (ret.Srs != null && ret.Srs.Rtc_server != null)
                {
                    ret.Srs.Rtc_server.Enabled = enabled;
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
        /// Set the rtc_server in srs
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rtc"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool SetRtcServer(string deviceId, SrsRtcServerConfClass rtc, out ResponseStruct rs)
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
                if (ret.Srs != null)
                {
                    ret.Srs.Rtc_server = rtc;
                    return true;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
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
        /// Get RTCServer
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static SrsRtcServerConfClass GetRtcServer(string deviceId, out ResponseStruct rs)
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
                if (ret.Srs != null)
                {
                    return ret.Srs.Rtc_server!;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SrsObjectNotInit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
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
    }
}