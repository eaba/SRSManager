using System.Collections.Generic;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using Common = SRSApis.Common;

namespace SrsApis.SrsManager.Apis
{
    public static class VhostIngestApis
    {
        public static bool OnOrOffIngest(string deviceId, string vhostDomain, string ingestName, bool enable,
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
                    retVhost.Vingests = new List<Ingest>();
                }

                var retVhostIngest = retVhost.Vingests.FindLast(x =>
                    x.IngestName!.Trim().ToUpper().Equals(ingestName!.Trim().ToUpper()));
                if (retVhostIngest != null)
                {
                    retVhostIngest.Enabled = enable;
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
        /// Get all or specified ingest instance names in vhost
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <param name="vhostDomain"></param>
        /// <returns></returns>
        public static List<VhostIngestNameModule> GetVhostIngestNameList(string deviceId, out ResponseStruct rs,
            string vhostDomain = "")
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

                var result = new List<VhostIngestNameModule>();
                if (string.IsNullOrEmpty(vhostDomain))
                {
                    foreach (var vhost in ret.Srs.Vhosts)
                    {
                        if (vhost.Vingests != null && vhost.Vingests.Count > 0)
                        {
                            foreach (var ingest in vhost.Vingests)
                            {
                                var vn = new VhostIngestNameModule();
                                vn.VhostDomain = vhost.VhostDomain;
                                vn.IngestInstanceName = ingest.IngestName;
                                result.Add(vn);
                            }
                        }
                    }

                    return result;
                }

                var retVhost = ret.Srs.Vhosts.FindLast(x =>
                    x.VhostDomain!.Trim().ToUpper().Equals(vhostDomain.Trim().ToUpper()));
                if (retVhost!.Vingests == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return null!;
                }

                foreach (var ingest in retVhost.Vingests)
                {
                    var vn = new VhostIngestNameModule();
                    vn.VhostDomain = retVhost.VhostDomain;
                    vn.IngestInstanceName = ingest.IngestName;
                    result.Add(vn);
                }

                return result;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SrsObjectNotInit,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit],
            };
            return null!;
        }

        
        /// <summary>
        /// Get an Ingest configuration
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="vhostDomain"></param>
        /// <param name="ingestName"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static Ingest GetVhostIngest(string deviceId, string vhostDomain, string ingestName,
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
                    x.VhostDomain!.Trim().ToUpper().Equals(vhostDomain.Trim().ToUpper()));

                if (retVhost == null || retVhost.Vingests == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return null!;
                }

                var retVhostIngest = retVhost.Vingests.FindLast(x =>
                    x.IngestName!.Trim().ToUpper().Equals(ingestName.Trim().ToUpper()));
                if (retVhostIngest == null)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SrsSubInstanceNotFound,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound],
                    };
                    return null!;
                }

                return retVhostIngest;
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