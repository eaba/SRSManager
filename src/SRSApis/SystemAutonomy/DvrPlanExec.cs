using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using SrsApis.SrsManager.Apis;
using SrsManageCommon;
using SRSManageCommon.ControllerStructs.RequestModules;
using SRSManageCommon.DBMoudle;

namespace SRSApis.SystemAutonomy
{
    public class DvrPlanExec
    {
        private int interval = SrsManageCommon.Common.SystemConfig.DvrPlanExecServiceinterval;

        private List<string> GetDvrPlanFileDataList(StreamDvrPlan plan)
        {
            List<string?> ret = null!;
            lock (SrsManageCommon.Common.LockDbObjForDvrVideo)
            {
                ret = OrmService.Db.Select<DvrVideo>()
                    .WhereIf(!string.IsNullOrEmpty(plan.DeviceId), x =>
                        x.App!.Trim().ToLower().Equals(plan.App!.Trim().ToLower()))
                    .WhereIf(!string.IsNullOrEmpty(plan.VhostDomain), x =>
                        x.Vhost!.Trim().ToLower().Equals(plan.VhostDomain!.Trim().ToLower()))
                    .WhereIf(!string.IsNullOrEmpty(plan.DeviceId), x =>
                        x.Device_Id!.Trim().ToLower().Equals(plan.DeviceId!.Trim().ToLower()))
                    .WhereIf(!string.IsNullOrEmpty(plan.Stream), x =>
                        x.Stream!.Trim().ToLower().Equals(plan.Stream!.Trim().ToLower()))
                    .Where(x => x.Deleted == false)
                    .GroupBy(x => x.RecordDate)
                    .OrderBy(x => x.Value.RecordDate)
                    .ToList(a => a.Value.RecordDate);
                
            }

            if (ret != null && ret.Count > 0)
            {
                return ret!;
            }

            return null!;
        }

        private decimal GetDvrPlanFileSize(StreamDvrPlan sdp)
        {
            try
            {
                lock (SrsManageCommon.Common.LockDbObjForDvrVideo)
                {
                    return OrmService.Db.Select<DvrVideo>()
                        .WhereIf(!string.IsNullOrEmpty(sdp.DeviceId), x =>
                            x.App!.Trim().ToLower().Equals(sdp.App!.Trim().ToLower()))
                        .WhereIf(!string.IsNullOrEmpty(sdp.VhostDomain), x =>
                            x.Vhost!.Trim().ToLower().Equals(sdp.VhostDomain!.Trim().ToLower()))
                        .WhereIf(!string.IsNullOrEmpty(sdp.DeviceId), x =>
                            x.Device_Id!.Trim().ToLower().Equals(sdp.DeviceId!.Trim().ToLower()))
                        .WhereIf(!string.IsNullOrEmpty(sdp.Stream), x =>
                            x.Stream!.Trim().ToLower().Equals(sdp.Stream!.Trim().ToLower()))
                        .Where(x => x.Deleted == false)
                        .Sum(x => x.FileSize);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }


        /// <summary>
        /// Clear files that were soft deleted 24 hours ago
        /// </summary>
        /// <param name="sdp"></param>
        private void DoDeleteFor24HourAgo(StreamDvrPlan sdp)
        {
            List<DvrVideo> retList = null!;
            lock (SrsManageCommon.Common.LockDbObjForDvrVideo)
            {
                retList = OrmService.Db.Select<DvrVideo>()
                    .WhereIf(!string.IsNullOrEmpty(sdp.DeviceId), x =>
                        x.App!.Trim().ToLower().Equals(sdp.App!.Trim().ToLower()))
                    .WhereIf(!string.IsNullOrEmpty(sdp.VhostDomain), x =>
                        x.Vhost!.Trim().ToLower().Equals(sdp.VhostDomain!.Trim().ToLower()))
                    .WhereIf(!string.IsNullOrEmpty(sdp.DeviceId), x =>
                        x.Device_Id!.Trim().ToLower().Equals(sdp.DeviceId!.Trim().ToLower()))
                    .WhereIf(!string.IsNullOrEmpty(sdp.Stream), x =>
                        x.Stream!.Trim().ToLower().Equals(sdp.Stream!.Trim().ToLower()))
                    .Where(x => x.Deleted == true)
                    .Where(x => ((DateTime) x.UpdateTime!).AddHours(24) <= DateTime.Now)
                    .ToList();
            }

            if (retList != null && retList.Count > 0)
            {
                lock (SrsManageCommon.Common.LockDbObjForDvrVideo)
                {
                    OrmService.Db.Transaction(() =>
                    {
                        foreach (var ret in retList)
                        {
                            if (ret != null)
                            {
                                if (ret.UpdateTime >= DateTime.Now)
                                {
                                    if (File.Exists(ret.VideoPath))
                                    {
                                        File.Delete(ret.VideoPath);
                                        LogWriter.WriteLog("Delete soft-deleted recordings (24 hours) ", ret.VideoPath!);
                                        Thread.Sleep(20);
                                    }

                                    OrmService.Db.Update<DvrVideo>().Set(x => x.UpdateTime, DateTime.Now)
                                        .Set(x => x.Undo, false)
                                        .Where(x => x.Id == ret!.Id).ExecuteAffrows();
                                }
                            }
                        }
                    });
                }
            }
        }

        private bool GetDvrOnorOff(StreamDvrPlan sdp)
        {
            var dvr = VhostDvrApis.GetVhostDvr(sdp.DeviceId, sdp.VhostDomain, out var rs);
            if (dvr == null) return false;
            var dvrApply = dvr.Dvr_apply!;
            dvrApply = dvrApply.Replace(";", "");
            var dvrStreams = new List<string>();
            if (dvrApply.Trim().ToLower().Equals("all"))
            {
                dvrApply = "";
            }
            if (!string.IsNullOrEmpty(dvrApply))
            {
                dvrStreams = Regex.Split(dvrApply, @"[\s]+").ToList();
                if (!dvrStreams.Contains((sdp.App + "/" + sdp.Stream).Trim()))
                {
                    return false;
                }

                return true;
            }

           
            return false;
        }

        private void SetDvrOnorOff(StreamDvrPlan sdp, bool eanble)
        {
            var dvr = VhostDvrApis.GetVhostDvr(sdp.DeviceId, sdp.VhostDomain, out var rs);
            if (dvr != null)
            {
                var dvrApply = dvr.Dvr_apply!;
                var dvrStreams = new List<string>();
                if (!string.IsNullOrEmpty(dvrApply))
                {
                    dvrStreams = Regex.Split(dvrApply, @"[\s]+").ToList();
                }


                if (dvrStreams.Count == 0)
                {
                    dvrStreams.Add("");
                }

                for (var i = 0; i <= dvrStreams.Count - 1; i++)
                {
                    dvrStreams[i] = dvrStreams[i].TrimEnd(';').Trim();
                }

                var needWrite = false;
                switch (eanble)
                {
                    case true:
                        if (!dvrStreams.Contains((sdp.App + "/" + sdp.Stream).Trim()))
                        {
                            dvrStreams.Add((sdp.App + "/" + sdp.Stream).Trim());
                            needWrite = true;
                        }

                        break;
                    case false:
                        if (dvrStreams.Contains((sdp.App + "/" + sdp.Stream).Trim()))
                        {
                            dvrStreams.Remove((sdp.App + "/" + sdp.Stream).Trim());
                            needWrite = true;
                        }

                        break;
                }

                if (needWrite)
                {
                    dvr.Dvr_apply = "";
                    foreach (var str in dvrStreams)
                    {
                        dvr.Dvr_apply += str + "\t";
                    }

                    if (dvr.Dvr_apply.Trim().Equals(";"))
                    {
                        dvr.Dvr_apply = "N;";
                    }

                    dvr.Dvr_apply = dvr.Dvr_apply.TrimEnd('\t');
                    VhostDvrApis.SetVhostDvr(sdp.DeviceId, sdp.VhostDomain, dvr, out rs);
                    SystemApis.RefreshSrsObject(sdp.DeviceId, out rs);
                }
            }
        }


        private void DeleteFileOneByOne(decimal videoSize, StreamDvrPlan sdp)
        {
            long deleteSize = 0;
            var orderBy = new List<OrderByStruct>();
            orderBy.Add(new OrderByStruct()
            {
                FieldName = "starttime",
                OrderByDir = OrderByDir.ASC,
            });
            var rgdv = new ReqGetDvrVideo()
            {
                App = sdp.App,
                DeviceId = sdp.DeviceId,
                EndTime = null,
                IncludeDeleted = false,
                OrderBy = orderBy,
                PageIndex = 1,
                PageSize = 10,
                StartTime = null,
                Stream = sdp.Stream,
                VhostDomain = sdp.VhostDomain,
            };
            while (videoSize - deleteSize > sdp.LimitSpace)
            {
                var videoList = DvrPlanApis.GetDvrVideoList(rgdv, out var rs);
                if (videoList != null && videoList.DvrVideoList != null && videoList.DvrVideoList.Count > 0)
                {
                    lock (SrsManageCommon.Common.LockDbObjForDvrVideo)
                    {
                        OrmService.Db.Transaction(() =>
                        {
                            foreach (var ret in videoList.DvrVideoList)
                            {
                                if (ret != null)
                                {
                                    if (File.Exists(ret.VideoPath))
                                    {
                                        File.Delete(ret.VideoPath); 
                                        deleteSize += (long) ret.FileSize!;
                                        LogWriter.WriteLog("删除录制文件", ret.VideoPath!);
                                        
                                        Thread.Sleep(20);
                                    }

                                    OrmService.Db.Update<DvrVideo>().Set(x => x.UpdateTime, DateTime.Now)
                                        .Set(x => x.Deleted, true)
                                        .Where(x => x.Id == ret!.Id).ExecuteAffrows();
                                }
                                if ((videoSize - deleteSize) < sdp.LimitSpace)
                                {
                                    break;
                                }
                            }
                        });
                    }
                }
            }
        }

        private void DeleteFileByDay(List<string> days)
        {
            foreach (var day in days)
            {
                List<DvrVideo> deleteList = null!;
                lock (SrsManageCommon.Common.LockDbObjForDvrVideo)
                {
                    deleteList = OrmService.Db.Select<DvrVideo>().Where(x => x.RecordDate == day).ToList();
                    OrmService.Db.Update<DvrVideo>().Set(x => x.UpdateTime, DateTime.Now)
                        .Set(x => x.Deleted, true)
                        .Where(x => x.RecordDate == day).ExecuteAffrows();
                    LogWriter.WriteLog("To delete files except one day, the database is marked for deletion", day!);
                }

                if (deleteList != null && deleteList.Count > 0)
                {
                    foreach (var del in deleteList)
                    {
                        if (del != null)
                        {
                            if (File.Exists(del.VideoPath))
                            {
                                File.Delete(del.VideoPath);
                                LogWriter.WriteLog("delete recording", del.VideoPath!);
                                Thread.Sleep(20);
                            }
                        }
                    }
                }
            }
        }


        private void ExecOnOrOff(StreamDvrPlan sdp)
        {
            var isEnable = true;
            var dateCount = 0;
            decimal videoSize = 0;
            List<string?> dateList = null!;
            videoSize = GetDvrPlanFileSize(sdp)!;
            dateList = GetDvrPlanFileDataList(sdp)!;
            if (sdp.OverStepPlan == OverStepPlan.StopDvr)
            {
              
                if (sdp.LimitDays > 0) //Handle the case with a limit of days
                {
                    if (dateList != null)
                    {
                        dateCount = dateList.Count;
                    }
                    else
                    {
                        dateCount = 0;
                    }

                    if (dateList != null && sdp.LimitDays < dateList.Count)
                    {

                        //stop
                        isEnable = false;
                    }
                }

                if (sdp.LimitSpace > 0) //Handle the case with a limit of days
                {
                    
                    if (videoSize > sdp.LimitSpace)
                    {
                        //stop
                        isEnable = false;
                    }
                }
            }
            var isTime = CheckTimeRange(sdp);
            isEnable = isEnable && sdp.Enable; //To handle the status of planned outages


            if (isTime && isEnable)
            {
                if (!GetDvrOnorOff(sdp))
                {
                    LogWriter.WriteLog("The recording plan is about to start recording, because the video stream has not reached the limit condition, it has entered the time specified in the plan and the recording program is closed", sdp.DeviceId + "->" +
                                                                                           sdp.VhostDomain + "->" +
                                                                                           sdp.App + "->" + sdp.Stream +
                                                                                           "\t" + "space limitation：" +
                                                                                           sdp.LimitSpace.ToString() +
                                                                                           "byte::actual space occupied：" +
                                                                                           videoSize.ToString() +
                                                                                           "byte \t time limit：" +
                                                                                           sdp.LimitDays.ToString() +
                                                                                           "sky::Actual recording days：" +
                                                                                           dateCount.ToString() +
                                                                                           "\tRecording Schedule Enabled Status:" +
                                                                                           sdp.Enable.ToString());
                    SetDvrOnorOff(sdp, true);
                }
               
            }
            else
            {
                if (GetDvrOnorOff(sdp))
                {
                    LogWriter.WriteLog("The recording plan is about to close the recording, because the video stream may have reached the limit condition or has left the scheduled time and the recording program is in the active state", sdp.DeviceId + "->" +
                                                                                            sdp.VhostDomain + "->" +
                                                                                            sdp.App + "->" +
                                                                                            sdp.Stream +
                                                                                            "\t" + "space limitation：" +
                                                                                            sdp.LimitSpace.ToString() +
                                                                                            "byte::actual space occupied：" +
                                                                                            videoSize.ToString() +
                                                                                            "byte \ttime limit：" +
                                                                                            sdp.LimitDays.ToString() +
                                                                                            "sky::Actual recording days：" +
                                                                                            dateCount.ToString() +
                                                                                            "\tRecording Schedule Enabled Status:" +
                                                                                            sdp.Enable.ToString());
                    SetDvrOnorOff(sdp, false);
                }
              
            }
        }

        private void ExecDelete(StreamDvrPlan sdp)
        {
            DoDeleteFor24HourAgo(sdp); //Files to be deleted after 24 hours of processing
            if (sdp.OverStepPlan == OverStepPlan.DeleteFile)
            {
                if (sdp.LimitDays > 0) //processing time-limited
                {
                    List<string?> dateList = null!;
                    dateList = GetDvrPlanFileDataList(sdp)!;
                    if (dateList != null)
                    {
                        SrsManageCommon.Common.RemoveNull(dateList);
                    }

                    if (dateList != null && dateList.Count > sdp.LimitDays)
                    {
                        //Perform day-by-day deletion
                        var loopCount = dateList.Count - sdp.LimitDays;

                        var willDeleteDays = new List<string>();
                        for (var i = 0; i < loopCount; i++)
                        {
                            willDeleteDays.Add(dateList[i]!);
                        }

                        DeleteFileByDay(willDeleteDays);
                    }
                }

                if (sdp.LimitSpace > 0) //Handle capacity constraints
                {
                    var videoSize = GetDvrPlanFileSize(sdp);

                    if (videoSize > sdp.LimitSpace)
                    {
                        //Delete files one by one
                        DeleteFileOneByOne(videoSize, sdp);
                    }
                }
            }
        }
     
        

        protected bool GetTimeSpan(string timeStr)
        {
            //Determine whether the current time is within the working hours
            var _strWorkingDayAM = "08:30";//working hours in the morning 08:30
            var _strWorkingDayPM = "17:30";
            var dspWorkingDayAM = DateTime.Parse(_strWorkingDayAM).TimeOfDay;
            var dspWorkingDayPM = DateTime.Parse(_strWorkingDayPM).TimeOfDay;

            //string time1 = "2017-2-17 8:10:00";
            var t1 = Convert.ToDateTime(timeStr);

            var dspNow = t1.TimeOfDay;
            if (dspNow > dspWorkingDayAM && dspNow < dspWorkingDayPM)
            {
                return true;
            }
            return false;
        }
        
        private bool IsTimeRange(DvrDayTimeRange d)
        {
            var nowDt = DateTime.Now.TimeOfDay;
            var start = d.StartTime.ToString("HH:mm:ss");
            var end = d.EndTime.ToString("HH:mm:ss");
            var workStartDT = DateTime.Parse(start).TimeOfDay;
            var workEndDT = DateTime.Parse(end).TimeOfDay;
            if (nowDt > workStartDT && nowDt < workEndDT)
            {
                return true;
            }

            return false;
          
        }
        
        

        private bool CheckTimeRange(StreamDvrPlan sdp)
        {
            if (sdp.TimeRangeList != null && sdp.TimeRangeList.Count > 0)
            {
                var t = sdp.TimeRangeList.FindLast(x => x.WeekDay == DateTime.Now.DayOfWeek);
                if (t != null && IsTimeRange(t))//Have a plan for the day and return true within the time query
                {
                    
                    return true;
                }

                if (t != null && !IsTimeRange(t))//Return false only if the plan for the day is set and it is not within the plan time of the day
                {
                    return false;
                }

                return true;//Returns true if no plan for the day is set
            }

            return true; //If it is empty, just return the runnable
        }


        //delete all empty directories for dvr directory
        private void ClearNofileDir(string deviceId)
        {
            var srs = SystemApis.GetSrsManagerInstanceByDeviceId(deviceId);
            if (srs != null)
            {
                var dvrPath = srs.SrsWorkPath + srs.SrsDeviceId + "/wwwroot/dvr";
                if (Directory.Exists(dvrPath))
                {
                    var dir = new DirectoryInfo(dvrPath);
                    var subdirs = dir.GetDirectories("*.*", SearchOption.AllDirectories);
                    foreach (var subdir in subdirs)
                    {
                        var subFiles = subdir.GetFileSystemInfos();
                        if (subFiles.Length == 0)
                        {
                            LogWriter.WriteLog("Monitoring found that there is an empty directory that needs to be deleted...", subdir.FullName);
                            subdir.Delete();
                        }
                    }
                }   
            }
        }
        private void Run()
        {
            while (true)
            {
                var srsDeviceIdList = SystemApis.GetAllSrsManagerDeviceId();
                if (srsDeviceIdList == null || srsDeviceIdList.Count == 0)
                {
                    Thread.Sleep(interval);
                    continue;
                }

                foreach (var deviceId in srsDeviceIdList)
                {
                    ClearNofileDir(deviceId);//清除空的目录
                    var rgdp = new ReqGetDvrPlan();
                    rgdp.DeviceId = deviceId;

                    var dvrPlanList = DvrPlanApis.GetDvrPlanList(rgdp, out var rs);

                    if (dvrPlanList == null || dvrPlanList.Count == 0) continue;
                    foreach (var dvrPlan in dvrPlanList)
                    {
                        if (dvrPlan == null)
                        {
                            continue;
                        }
                        
                        ExecDelete(dvrPlan);
                        ExecOnOrOff(dvrPlan);
                        Thread.Sleep(2000);
                    }
                }

                Thread.Sleep(interval);
            }
        }

        public DvrPlanExec()
        {
            new Thread(new ThreadStart(delegate

            {
                try
                {
                    LogWriter.WriteLog("Start the automatic recording schedule service...(cycle interval：" + interval + "ms)");
                    Run();
                }
                catch (Exception ex)
                {
                    LogWriter.WriteLog("Failed to start the automatic recording schedule service...", ex.Message + "\r\n" + ex.StackTrace, ConsoleColor.Yellow);
                }
            })).Start();
        }
    }
}