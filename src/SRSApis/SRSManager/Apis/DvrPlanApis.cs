using System;
using System.Collections.Generic;
using SrsManageCommon;
using SRSManageCommon.ControllerStructs.RequestModules;
using SRSManageCommon.ControllerStructs.ResponseModules;
using SRSManageCommon.DBMoudle;
using SRSManageCommon.ManageStructs;

namespace SrsApis.SrsManager.Apis
{
    public static class DvrPlanApis
    {

        

        /// <summary>
        /// Get video file list
        /// </summary>
        /// <param name="rgdv"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static DvrVideoResponseList GetDvrVideoList(ReqGetDvrVideo rgdv, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var idFound = !string.IsNullOrEmpty(rgdv.DeviceId);
            var vhostFound = !string.IsNullOrEmpty(rgdv.VhostDomain);
            var streamFound = !string.IsNullOrEmpty(rgdv.Stream);
            var appFound = !string.IsNullOrEmpty(rgdv.App);
            var isPageQuery = (rgdv.PageIndex != null && rgdv.PageIndex >= 1);
            var haveOrderBy = rgdv.OrderBy != null;
            if (isPageQuery)
            {
                if (rgdv.PageSize > 10000)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SystemDataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseLimited],
                    };
                    return null!;
                }

                if (rgdv.PageIndex <= 0)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SystemDataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseLimited],
                    };
                    return null!;
                }
            }

            var orderBy = "";
            if (haveOrderBy)
            {
                foreach (var order in rgdv.OrderBy!)
                {
                    if (order != null)
                    {
                        orderBy += order.FieldName + " " + Enum.GetName(typeof(OrderByDir), order.OrderByDir!) + ",";
                    }
                }

                orderBy = orderBy.TrimEnd(',');
            }

            long total = -1;
            List<DvrVideo> retList = null!;

            if (!isPageQuery)
            {
                lock (Common.LockDbObjForDvrVideo)
                {
                    retList = OrmService.Db.Select<DvrVideo>().Where("1=1")
                        .WhereIf(idFound, x => x.Device_Id!.Trim().ToLower().Equals(rgdv.DeviceId!.Trim().ToLower()))
                        .WhereIf(vhostFound, x => x.Vhost!.Trim().ToLower().Equals(rgdv.VhostDomain!.Trim().ToLower()))
                        .WhereIf(streamFound, x => x.Stream!.Trim().ToLower().Equals(rgdv.Stream!.Trim().ToLower()))
                        .WhereIf(rgdv.StartTime != null, x => x.StartTime >= rgdv.StartTime)
                        .WhereIf(rgdv.EndTime != null, x => x.EndTime <= rgdv.EndTime)
                        .WhereIf(appFound, x => x.App!.Trim().ToLower().Equals(rgdv.App!.Trim().ToLower()))
                        .WhereIf(!(bool) rgdv.IncludeDeleted!, x => x.Deleted == false)
                        .OrderBy(orderBy)
                        .ToList();
                }
            }
            else
            {
                lock (Common.LockDbObjForDvrVideo)
                {
                    retList = OrmService.Db.Select<DvrVideo>().Where("1=1")
                        .WhereIf(idFound, x => x.Device_Id!.Trim().ToLower().Equals(rgdv.DeviceId!.Trim().ToLower()))
                        .WhereIf(vhostFound, x => x.Vhost!.Trim().ToLower().Equals(rgdv.VhostDomain!.Trim().ToLower()))
                        .WhereIf(streamFound, x => x.Stream!.Trim().ToLower().Equals(rgdv.Stream!.Trim().ToLower()))
                        .WhereIf(rgdv.StartTime != null, x => x.StartTime >= rgdv.StartTime)
                        .WhereIf(rgdv.EndTime != null, x => x.EndTime <= rgdv.EndTime)
                        .WhereIf(appFound, x => x.App!.Trim().ToLower().Equals(rgdv.App!.Trim().ToLower()))
                        .WhereIf(!(bool) rgdv.IncludeDeleted!, x => x.Deleted == false).OrderBy(orderBy)
                        .Count(out total)
                        .Page((int) rgdv.PageIndex!, (int) rgdv.PageSize!)
                        .ToList();
                }
            }

            var result = new DvrVideoResponseList();
            result.DvrVideoList = retList;
            if (!isPageQuery)
            {
                if (retList != null)
                {
                    total = retList.Count;
                }
                else
                {
                    total = 0;
                }
            }

            result.Total = total;
            result.Request = rgdv;
            return result;
        }

        public static List<StreamDvrPlan> GetDvrPlanList(ReqGetDvrPlan rgdp, out ResponseStruct rs)
        {
            var idFound = !string.IsNullOrEmpty(rgdp.DeviceId);
            var vhostFound = !string.IsNullOrEmpty(rgdp.VhostDomain);
            var streamFound = !string.IsNullOrEmpty(rgdp.Stream);
            var appFound = !string.IsNullOrEmpty(rgdp.App);
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            lock (Common.LockDbObjForStreamDvrPlan)
            {
                /*together with subclasses*/
                return OrmService.Db.Select<StreamDvrPlan>().IncludeMany(a => a.TimeRangeList)
                    .WhereIf(idFound == true, x => x.DeviceId.Trim().ToLower().Equals(rgdp.DeviceId!.Trim().ToLower()))
                    .WhereIf(vhostFound == true,
                        x => x.VhostDomain.Trim().ToLower().Equals(rgdp.VhostDomain!.Trim().ToLower()))
                    .WhereIf(appFound == true, x => x.App.Trim().ToLower().Equals(rgdp.App!.Trim().ToLower()))
                    .WhereIf(streamFound == true, x => x.Stream.Trim().ToLower().Equals(rgdp.Stream!.Trim().ToLower()))
                    .ToList();
                /*together with subclasses*/
            }
        }

        
    }
}