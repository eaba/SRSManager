using System.Collections.Generic;
using SrsConfFile.SRSConfClass;
using SrsManageCommon;
using SRSManageCommon.DBMoudle;
using SRSManageCommon.ManageStructs;
using Common = SRSApis.Common;

namespace SrsApis.SrsManager.Apis
{
    public static class FastUsefulApis
    {
        
        
        /// <summary>
        /// Get all ingestBydeviceid
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<Ingest> GetAllIngestByDeviceId(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (Common.SrsManagers == null || Common.SrsManagers.Count == 0)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return null!;
            }

            var ret = Common.SrsManagers.FindLast(x =>
                x.SrsDeviceId.Trim().ToLower().Equals(deviceId.Trim().ToLower()));
            if (ret == null)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return null!;
            }

            var ingestList = new List<Ingest>();
            if (ret.Srs.Vhosts == null || ret.Srs.Vhosts.Count == 0) return null!;
            foreach (var vhost in ret.Srs.Vhosts)
            {
                if (vhost != null && vhost.Vingests != null)
                {
                    foreach (var ingest in vhost.Vingests)
                    {
                        if (ingest != null)
                        {
                            ingestList.Add(ingest);
                        }
                    }
                }
            }

            return ingestList;
        }

        /// <summary>
        /// Get all published cameras
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<OnlineClient> GetOnPublishMonitorListByDeviceId(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (string.IsNullOrEmpty(deviceId))
            {
                rs.Code = ErrorNumber.FunctionInputParamsError;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError];
                return null!;
            }

            lock (SrsManageCommon.Common.LockDbObjForOnlineClient)
            {
                var result = OrmService.Db.Select<OnlineClient>()
                    .Where(x => x.IsOnline == true && x.ClientType == ClientType.Monitor &&
                                x.Device_Id!.Equals(deviceId.Trim())).ToList();
                return result;
            }
        }

    }
}