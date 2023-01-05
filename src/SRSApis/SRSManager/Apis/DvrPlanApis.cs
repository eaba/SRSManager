using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SrsManageCommon;
using SRSManageCommon.ControllerStructs.RequestModules;
using SRSManageCommon.ControllerStructs.ResponseModules;
using SRSManageCommon.DBMoudle;
using SRSManageCommon.ManageStructs;
using TaskStatus = SRSManageCommon.ManageStructs.TaskStatus;

namespace SrsApis.SrsManager.Apis
{
    public static class DvrPlanApis
    {

        /// <summary>
        /// Get Clipping Backlog List
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<CutMergeTaskStatusResponse> GetBacklogTaskList(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (CutMergeService.CutMergeTaskStatusList != null)
            {
                var retList= CutMergeService.CutMergeTaskStatusList.FindAll(x => x.TaskStatus == TaskStatus.Create).ToList();
                if (retList != null && retList.Count > 0)
                {
                    var resultList= new List<CutMergeTaskStatusResponse>();
                    foreach (var ret in retList!)
                    {
                        var res = new CutMergeTaskStatusResponse()
                        {
                            CallbakUrl = ret.CallbakUrl,
                            CreateTime = ret.CreateTime,
                            ProcessPercentage = ret.ProcessPercentage,
                            TaskId = ret.TaskId,
                            TaskStatus = ret.TaskStatus,
                        };
                        resultList.Add(res);
                    }

                    return resultList;
                }
            }

            return null!;
        }



        /// <summary>
        /// Get the list of files that need to be cropped and merged 
        /// </summary>
        /// <param name="rcmv"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private static List<CutMergeStruct> AnalysisVideoFile(ReqCutOrMergeVideoFile rcmv, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var startPos = -1;
            var endPos = -1;
            var _start = DateTime.Parse(rcmv.StartTime.ToString("yyyy-MM-dd HH:mm:ss")).AddSeconds(-20); //push forward 20 seconds
            var _end = DateTime.Parse(rcmv.EndTime.ToString("yyyy-MM-dd HH:mm:ss")).AddSeconds(20); //20 seconds backward delay
            var videoList = OrmService.Db.Select<DvrVideo>()
                .Where(x => x.StartTime > _start.AddMinutes(-60) && x.EndTime <= _end.AddMinutes(60))
                .WhereIf(!string.IsNullOrEmpty(rcmv.DeviceId),
                    x => x.Device_Id!.Trim().ToLower().Equals(rcmv.DeviceId!.Trim().ToLower()))
                .WhereIf(!string.IsNullOrEmpty(rcmv.VhostDomain),
                    x => x.Vhost!.Trim().ToLower().Equals(rcmv.VhostDomain!.Trim().ToLower()))
                .WhereIf(!string.IsNullOrEmpty(rcmv.App),
                    x => x.App!.Trim().ToLower().Equals(rcmv.App!.Trim().ToLower()))
                .WhereIf(!string.IsNullOrEmpty(rcmv.Stream),
                    x => x.Stream!.Trim().ToLower().Equals(rcmv.Stream!.Trim().ToLower()))
                .ToList(); //Get all the data within the first 60 minutes and the last 60 minutes of the condition range

            List<DvrVideo> cutMegerList = null!;
            if (videoList != null && videoList.Count > 0)
            {
                for (var i = 0; i <= videoList.Count - 1; i++)
                {
                    if (!File.Exists(videoList[i].VideoPath))
                    {
                        continue; //file does not exist, skip
                    }

                    var startInDb =
                        DateTime.Parse(((DateTime) videoList[i].StartTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                    var endInDb =
                        DateTime.Parse(((DateTime) videoList[i].EndTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                    if (startInDb <= _start && endInDb > _start) //Find a video that meets your requirements
                    {
                        startPos = i;
                    }

                    if (startInDb < _end && endInDb >= _end) //Find the ending video that meets the requirements
                    {
                        endPos = i;
                    }
                }

                if (startPos >= 0 && endPos >= 0) //If both the beginning and the end are found, take the video in this range
                {
                    cutMegerList = videoList.GetRange(startPos, endPos - startPos + 1);
                }

                if (startPos < 0 && endPos >= 0) //If the start is not found and the end is found
                {
                    var tmpStartList = new List<KeyValuePair<int, double>>();
                    for (var i = 0; i <= videoList.Count - 1; i++)
                    {
                        tmpStartList.Add(new KeyValuePair<int, double>(i,
                            Math.Abs(((DateTime) videoList[i]!.StartTime!).Subtract(_start)
                                .TotalMilliseconds))); //The start time required to subtract the start time of all videos, take the absolute value
                    }

                    tmpStartList.Sort((left, right) => //Sort by absolute value after subtraction
                    {
                        if (left.Value > right.Value)
                            return 1;
                        else if ((int) left.Value == (int) right.Value)
                            return 0;
                        else
                            return -1;
                    });

                    cutMegerList =
                        videoList.GetRange(tmpStartList[0].Key, endPos - tmpStartList[0].Key + 1); //Take the video closest to the requested time as the start video
                    for (var i = cutMegerList.Count - 1; i >= 0; i--)
                    {
                        if (cutMegerList[i].StartTime > _end && cutMegerList[i].EndTime > _end
                        ) //If the start time of the video is greater than the required end time and it is not the last video, filter out the video
                        {
                            if (i > 0)
                            {
                                cutMegerList[i] = null!;
                            }
                        }
                    }

                    Common.RemoveNull(cutMegerList);
                }

                if (startPos >= 0 && endPos < 0) //The start video was found, but the end video was not found
                {
                    var tmpEndList = new List<KeyValuePair<int, double>>();

                    for (var i = 0; i <= videoList.Count - 1; i++)
                    {
                        tmpEndList.Add(new KeyValuePair<int, double>(i,
                            Math.Abs(((DateTime) videoList[i]!.EndTime!).Subtract(_end)
                                .TotalMilliseconds))); //Same as above, take the absolute value
                    }

                    tmpEndList.Sort((left, right) => //to sort
                    {
                        if (left.Value > right.Value)
                            return 1;
                        else if ((int) left.Value == (int) right.Value)
                            return 0;
                        else
                            return -1;
                    });
                    cutMegerList = videoList.GetRange(startPos, tmpEndList[0].Key - startPos + 1);
                    for (var i = cutMegerList.Count - 1; i >= 0; i--)
                    {
                        if (cutMegerList[i].StartTime > _end && cutMegerList[i].EndTime > _end) //filter
                        {
                            if (i > 0)
                            {
                                cutMegerList[i] = null!;
                            }
                        }
                    }

                    Common.RemoveNull(cutMegerList);
                }

                if (startPos < 0 && endPos < 0) //If the beginning is not found, and the end is not found, then report an error
                {
                }
            }

            if (cutMegerList != null && cutMegerList.Count > 0) //Get the list of files to merge
            {
                var cutMergeStructList = new List<CutMergeStruct>();
                for (var i = 0; i <= cutMegerList.Count - 1; i++)
                {
                    var tmpCutMeger = cutMegerList[i];
                    if (tmpCutMeger != null && i == 0) //See if the first file needs to be cropped
                    {
                        var tmpCutMegerStartTime =
                            DateTime.Parse(((DateTime) tmpCutMeger.StartTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                        var tmpCutMegerEndTime =
                            DateTime.Parse(((DateTime) tmpCutMeger.EndTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                        if (tmpCutMegerStartTime < _start && tmpCutMegerEndTime > _start
                        ) //If the video start time is greater than the desired start time and the video end time is greater than the desired start time
                        {
                            var ts = -tmpCutMegerStartTime.Subtract(_start); //The start time of the video minus the required start time, and then negate
                            var ts2 = tmpCutMegerEndTime.Subtract(_start) + ts; //The end time of the video minus the desired start time, plus the previous value
                            var tmpStruct = new CutMergeStruct();
                            tmpStruct.DbId = cutMegerList[i].Id;
                            tmpStruct.Duration = cutMegerList[i].Duration;
                            tmpStruct.EndTime = cutMegerList[i].EndTime;
                            tmpStruct.FilePath = cutMegerList[i].VideoPath;
                            tmpStruct.FileSize = cutMegerList[i].FileSize;
                            tmpStruct.StartTime = cutMegerList[i].StartTime;

                            if (ts2.Hours <= 0 && ts2.Minutes <= 0 && ts2.Seconds <= 0) //If the items of time ts2 are less than 0, it means that no cropping is required
                            {
                                tmpStruct.CutEndPos = "";
                                tmpStruct.CutStartPos = "";
                            }
                            else //Otherwise, set the clipping parameters
                            {
                                tmpStruct.CutEndPos = ts2.Hours.ToString().PadLeft(2, '0') + ":" +
                                                      ts2.Minutes.ToString().PadLeft(2, '0') + ":" +
                                                      ts2.Seconds.ToString().PadLeft(2, '0');
                                tmpStruct.CutStartPos = ts.Hours.ToString().PadLeft(2, '0') + ":" +
                                                        ts.Minutes.ToString().PadLeft(2, '0') + ":" +
                                                        ts.Seconds.ToString().PadLeft(2, '0');
                            }

                            cutMergeStructList.Add(tmpStruct); //Add to processing list
                        }
                        else //If the video time is greater than or equal to the required start time or greater than or equal to the required end time, the time is just right, so add it directly
                        {
                            var tmpStruct = new CutMergeStruct()
                            {
                                DbId = cutMegerList[i].Id,
                                CutEndPos = null,
                                CutStartPos = null,
                                Duration = cutMegerList[i].Duration,
                                EndTime = cutMegerList[i].EndTime,
                                FilePath = cutMegerList[i].VideoPath,
                                FileSize = cutMegerList[i].FileSize,
                                StartTime = cutMegerList[i].StartTime,
                            };
                            cutMergeStructList.Add(tmpStruct);
                        }
                    }
                    else if (tmpCutMeger != null && i == cutMegerList.Count - 1) //Process the last video to see if it needs to be cropped, the follow-up operation is the same as above
                    {
                        var tmpCutMegerStartTime =
                            DateTime.Parse(((DateTime) tmpCutMeger.StartTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                        var tmpCutMegerEndTime =
                            DateTime.Parse(((DateTime) tmpCutMeger.EndTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                        if (tmpCutMegerEndTime > _end)
                        {
                            var ts = tmpCutMegerEndTime.Subtract(_end);
                            ts = (tmpCutMegerEndTime - tmpCutMegerStartTime).Subtract(ts);
                            var tmpStruct = new CutMergeStruct();
                            tmpStruct.DbId = cutMegerList[i].Id;
                            tmpStruct.Duration = cutMegerList[i].Duration;
                            tmpStruct.EndTime = cutMegerList[i].EndTime;
                            tmpStruct.FilePath = cutMegerList[i].VideoPath;
                            tmpStruct.FileSize = cutMegerList[i].FileSize;
                            tmpStruct.StartTime = cutMegerList[i].StartTime;
                            if (ts.Hours <= 0 && ts.Minutes <= 0 && ts.Seconds <= 0)
                            {
                                tmpStruct.CutEndPos = "";
                                tmpStruct.CutStartPos = "";
                            }
                            else
                            {
                                tmpStruct.CutEndPos = ts.Hours.ToString().PadLeft(2, '0') + ":" +
                                                      ts.Minutes.ToString().PadLeft(2, '0') + ":" +
                                                      ts.Seconds.ToString().PadLeft(2, '0');
                                tmpStruct.CutStartPos = "00:00:00";
                            }


                            cutMergeStructList.Add(tmpStruct);
                        }
                        else if (tmpCutMegerEndTime <= _end)
                        {
                            var tmpStruct = new CutMergeStruct()
                            {
                                DbId = cutMegerList[i].Id,
                                CutEndPos = null,
                                CutStartPos = null,
                                Duration = cutMegerList[i].Duration,
                                EndTime = cutMegerList[i].EndTime,
                                FilePath = cutMegerList[i].VideoPath,
                                FileSize = cutMegerList[i].FileSize,
                                StartTime = cutMegerList[i].StartTime,
                            };
                            cutMergeStructList.Add(tmpStruct);
                        }
                    }
                    else //If it is not the first or the last, it is the middle part, directly added to the list
                    {
                        var tmpStruct = new CutMergeStruct()
                        {
                            DbId = cutMegerList[i].Id,
                            CutEndPos = null,
                            CutStartPos = null,
                            Duration = cutMegerList[i].Duration,
                            EndTime = cutMegerList[i].EndTime,
                            FilePath = cutMegerList[i].VideoPath,
                            FileSize = cutMegerList[i].FileSize,
                            StartTime = cutMegerList[i].StartTime,
                        };
                        cutMergeStructList.Add(tmpStruct);
                    }
                }

                return cutMergeStructList;
            }

            rs = new ResponseStruct() //Error, video resource not found
            {
                Code = ErrorNumber.DvrCutMergeFileNotFound,
                Message = ErrorMessage.ErrorDic![ErrorNumber.DvrCutMergeFileNotFound],
            };
            return null!;
        }


        /// <summary>
        /// Get the situation of merging and cutting tasks, excluding synchronization tasks
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static CutMergeTaskStatusResponse GetMergeTaskStatus(string taskId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var ret = CutMergeService.CutMergeTaskStatusList.FindLast(x => x.TaskId == taskId);
            
            if (ret == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.DvrCutMergeTaskNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.DvrCutMergeTaskNotExists],
                };
                return null!;
            }

            
            var result = new CutMergeTaskStatusResponse()
            {
                CallbakUrl = ret.CallbakUrl,
                CreateTime = ret.CreateTime,
                ProcessPercentage = ret.ProcessPercentage,
                TaskId = ret.TaskId,
                TaskStatus = ret.TaskStatus,
                
            };
           
            return result;
        }

        public static CutMergeTaskResponse CutOrMergeVideoFile(ReqCutOrMergeVideoFile rcmv, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (rcmv.StartTime >= rcmv.EndTime)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return null!;
            }

            if ((rcmv.EndTime - rcmv.StartTime).Minutes > 120) //Execution of tasks is not allowed for more than 120 minutes
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.DvrCutMergeTimeLimit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.DvrCutMergeTimeLimit],
                };

                return null!;
                
            }

            if (string.IsNullOrEmpty(rcmv.CallbackUrl) || !Common.IsUrl(rcmv.CallbackUrl!))
            {
                //return synchronously
                if ((rcmv.EndTime - rcmv.StartTime).Minutes > 10)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.DvrCutMergeTimeLimit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.DvrCutMergeTimeLimit],
                    };

                    return null!;
                }

                var mergeList = AnalysisVideoFile(rcmv, out rs);

                if (mergeList != null && mergeList.Count > 0)
                {
                    var task = new CutMergeTask()
                    {
                        CutMergeFileList = mergeList,
                        CallbakUrl = null,
                        CreateTime = DateTime.Now,
                        TaskId = Common.CreateUuid()!,
                        TaskStatus = TaskStatus.Create,
                        ProcessPercentage = 0,
                    };
                    var taskReturn = Task.Factory.StartNew(() => CutMergeService.CutMerge(task)); //thread throwing
                    taskReturn.Wait();
                    taskReturn.Result.Request = rcmv;
                    taskReturn.Result.Uri = ":" + SrsManageCommon.Common.SystemConfig.HttpPort +
                                            taskReturn.Result.FilePath!.Replace(Common.WorkPath + "CutMergeFile", "");
                    return taskReturn.Result;
                }

                return null!;
            }
            else
            {
                //asynchronous callback
                var mergeList = AnalysisVideoFile(rcmv, out rs);
                if (mergeList != null && mergeList.Count > 0)
                {
                    var task = new CutMergeTask()
                    {
                        CutMergeFileList = mergeList,
                        CallbakUrl = rcmv.CallbackUrl,
                        CreateTime = DateTime.Now,
                        TaskId = Common.CreateUuid()!,
                        TaskStatus = TaskStatus.Create,
                        ProcessPercentage = 0,
                    };
                    try
                    {
                        CutMergeService.CutMergeTaskList.Add(task);
                        CutMergeService.CutMergeTaskStatusList.Add(task);

                        return new CutMergeTaskResponse()
                        {
                            Duration = -1,
                            FilePath = "",
                            FileSize = -1,
                            Status = CutMergeRequestStatus.WaitForCallBack,
                            Task = task,
                            Request = rcmv,
                        };
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct() //An error is reported, the queue is larger than the maximum value
                        {
                            Code = ErrorNumber.DvrCutProcessQueueLimit,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.DvrCutProcessQueueLimit] + "\r\n" +
                                      ex.Message + "\r\n" + ex.StackTrace,
                        };
                        return null!;
                    }
                }

                return null!;
            }
        }

        /// <summary>
        /// Recover soft-deleted recordings
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool UndoSoftDelete(long id, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            DvrVideo retSelect = null!;
            lock (Common.LockDbObjForDvrVideo)
            {
                retSelect = OrmService.Db.Select<DvrVideo>().Where(x => x.Id == id).First();
                if (retSelect == null)
                {
                    rs.Code = ErrorNumber.SystemDataBaseRecordNotExists;
                    rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseRecordNotExists];
                    return false;
                }
            }

            if (!File.Exists(retSelect.VideoPath))
            {
                rs.Code = ErrorNumber.DvrVideoFileNotExists;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.DvrVideoFileNotExists];
                return false;
            }

            lock (Common.LockDbObjForDvrVideo)
            {
                var retUpdate = OrmService.Db.Update<DvrVideo>().Set(x => x.Deleted, false)
                    .Set(x => x.UpdateTime, DateTime.Now)
                    .Set(x => x.Undo, false).Where(x => x.Id == (long) id).ExecuteAffrows();
                if (retUpdate > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Delete a video file (hard delete, delete the file immediately, set the database to Delete)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool HardDeleteDvrVideoById(long id, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            List<DvrVideo> retSelect = null!;
            var retUpdate = -1;
            lock (Common.LockDbObjForDvrVideo)
            {
                retSelect = OrmService.Db.Select<DvrVideo>().Where(x => x.Id == id).ToList();
                retUpdate = OrmService.Db.Update<DvrVideo>().Set(x => x.Deleted, true)
                    .Set(x => x.Undo, false)
                    .Set(x => x.UpdateTime, DateTime.Now).Where(x => x.Id == (long) id).ExecuteAffrows();
            }

            if (retUpdate > 0)
            {
                foreach (var select in retSelect)
                {
                    try
                    {
                        File.Delete(select.VideoPath);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete a video file (soft delete, only mark but not delete the file, the file will be deleted after 24 hours)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool SoftDeleteDvrVideoById(long id, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            lock (Common.LockDbObjForDvrVideo)
            {
                var retUpdate = OrmService.Db.Update<DvrVideo>().Set(x => x.Deleted, true)
                    .Set(x => x.UpdateTime, DateTime.Now)
                    .Set(x => x.Undo, true).Where(x => x.Id == (long) id).ExecuteAffrows();
                if (retUpdate > 0)
                {
                    return true;
                }
            }

            return false;
        }

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


        /// <summary>
        /// Delete a recording plan by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool DeleteDvrPlanById(long id, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };


            if (id <= 0)
            {
                rs.Code = ErrorNumber.FunctionInputParamsError;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError];
                return false;
            }

            List<StreamDvrPlan> retSelect = null!;
            var retDelete = -1;
            lock (Common.LockDbObjForStreamDvrPlan)
            {
                retSelect = OrmService.Db.Select<StreamDvrPlan>().Where(x => x.Id == id).ToList();
                retDelete = OrmService.Db.Delete<StreamDvrPlan>().Where(x => x.Id == id).ExecuteAffrows();
            }

            if (retDelete > 0)
            {
                lock (Common.LockDbObjForStreamDvrPlan)
                {
                    foreach (var select in retSelect)
                    {
                        OrmService.Db.Delete<DvrDayTimeRange>().Where(x => x.StreamDvrPlanId == select.Id)
                            .ExecuteAffrows();
                    }
                }

                return true;
            }


            rs.Code = ErrorNumber.SrsDvrPlanNotExists;
            rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanNotExists];
            return false;
        }

        /// <summary>
        /// Enable or stop a recording schedule
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enable"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool OnOrOffDvrPlanById(long id, bool enable, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };


            if (id <= 0)
            {
                rs.Code = ErrorNumber.FunctionInputParamsError;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError];
                return false;
            }

            lock (Common.LockDbObjForStreamDvrPlan)
            {
                var retUpdate = OrmService.Db.Update<StreamDvrPlan>().Set(x => x.Enable, enable)
                    .Where(x => x.Id == id)
                    .ExecuteAffrows();
                if (retUpdate > 0)
                    return true;
            }

            rs.Code = ErrorNumber.SrsDvrPlanNotExists;
            rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanNotExists];
            return false;
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


        /// <summary>
        /// modify dvrplan
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sdp"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool SetDvrPlanById(int id, ReqStreamDvrPlan sdp, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (SRSApis.Common.SrsManagers == null || SRSApis.Common.SrsManagers.Count == 0)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];

                return false;
            }

            var retSrs = SystemApis.GetSrsManagerInstanceByDeviceId(sdp.DeviceId!);
            if (retSrs == null)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return false;
            }

            var retVhost = VhostApis.GetVhostByDomain(sdp.DeviceId!, sdp.VhostDomain!, out rs);
            if (retVhost == null)
            {
                rs.Code = ErrorNumber.SrsSubInstanceNotFound;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound];
                return false;
            }

            if (sdp.TimeRangeList != null && sdp.TimeRangeList.Count > 0)
            {
                foreach (var timeRange in sdp.TimeRangeList)
                {
                    if (timeRange.StartTime >= timeRange.EndTime)
                    {
                        rs.Code = ErrorNumber.FunctionInputParamsError;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError];
                        return false;
                    }

                    if ((timeRange.EndTime - timeRange.StartTime).TotalSeconds <= 120)
                    {
                        rs.Code = ErrorNumber.SrsDvrPlanTimeLimitExcept;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanTimeLimitExcept];

                        return false;
                    }
                }
            }

            try
            {
                StreamDvrPlan retSelect = null!;
                var retDelete = -1;
                lock (Common.LockDbObjForStreamDvrPlan)
                {
                    retSelect = OrmService.Db.Select<StreamDvrPlan>().Where(x => x.Id == id).First();
                    retDelete = OrmService.Db.Delete<StreamDvrPlan>().Where(x => x.Id == id).ExecuteAffrows();
                }


                if (retDelete > 0)
                {
                    lock (Common.LockDbObjForStreamDvrPlan)
                    {
                        OrmService.Db.Delete<DvrDayTimeRange>()
                            .Where(x => x.StreamDvrPlanId == retSelect.Id).ExecuteAffrows();
                    }

                    var retCreate = CreateDvrPlan(sdp, out rs); //create new dvr
                    if (retCreate)
                    {
                        return true;
                    }

                    return false;
                }

                rs.Code = ErrorNumber.SrsDvrPlanNotExists;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanNotExists];
                return false;
            }
            catch (Exception ex)
            {
                rs.Code = ErrorNumber.SystemDataBaseExcept;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseExcept] + "\r\n" + ex.Message;

                return false;
            }
        }


        /// <summary>
        /// Create a recording schedule
        /// </summary>
        /// <param name="sdp"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool CreateDvrPlan(ReqStreamDvrPlan sdp, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (SRSApis.Common.SrsManagers == null || SRSApis.Common.SrsManagers.Count == 0)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return false;
            }

            var retSrs = SystemApis.GetSrsManagerInstanceByDeviceId(sdp.DeviceId!);
            if (retSrs == null)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return false;
            }

            var retVhost = VhostApis.GetVhostByDomain(sdp.DeviceId!, sdp.VhostDomain!, out rs);
            if (retVhost == null)
            {
                rs.Code = ErrorNumber.SrsSubInstanceNotFound;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound];
                return false;
            }

            if (sdp.TimeRangeList != null && sdp.TimeRangeList.Count > 0)
            {
                foreach (var timeRange in sdp.TimeRangeList)
                {
                    if (timeRange.StartTime >= timeRange.EndTime)
                    {
                        rs.Code = ErrorNumber.FunctionInputParamsError;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError];
                        return false;
                    }

                    if ((timeRange.EndTime - timeRange.StartTime).TotalSeconds <= 120)
                    {
                        rs.Code = ErrorNumber.SrsDvrPlanTimeLimitExcept;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanTimeLimitExcept];

                        return false;
                    }
                }
            }

            StreamDvrPlan retSelect = null!;
            lock (Common.LockDbObjForStreamDvrPlan)
            {
                retSelect = OrmService.Db.Select<StreamDvrPlan>().Where(x =>
                    x.DeviceId!.Trim().ToLower().Equals(sdp.DeviceId!.Trim().ToLower())
                    && x.VhostDomain!.Trim().ToLower().Equals(sdp.VhostDomain!.Trim().ToLower())
                    && x.App!.Trim().ToLower().Equals(sdp.App!.Trim().ToLower())
                    && x.Stream!.Trim().ToLower().Equals(sdp.Stream!.Trim().ToLower())).First();
            }

            if (retSelect != null)
            {
                rs.Code = ErrorNumber.SrsDvrPlanAlreadyExists;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanAlreadyExists];

                return false;
            }

            try
            {
                lock (Common.LockDbObjForStreamDvrPlan)
                {
                    var tmpStream = new StreamDvrPlan();
                    tmpStream.App = sdp.App;
                    tmpStream.Enable = sdp.Enable;
                    tmpStream.Stream = sdp.Stream;
                    tmpStream.DeviceId = sdp.DeviceId;
                    tmpStream.LimitDays = sdp.LimitDays;
                    tmpStream.LimitSpace = sdp.LimitSpace;
                    tmpStream.VhostDomain = sdp.VhostDomain;
                    tmpStream.OverStepPlan = sdp.OverStepPlan;
                    tmpStream.TimeRangeList = new List<DvrDayTimeRange>();
                    if (sdp.TimeRangeList != null && sdp.TimeRangeList.Count > 0)
                    {
                        foreach (var tmp in sdp.TimeRangeList)
                        {
                            tmpStream.TimeRangeList.Add(new DvrDayTimeRange()
                            {
                                EndTime = tmp.EndTime,
                                StartTime = tmp.StartTime,
                                WeekDay = tmp.WeekDay,
                            });
                        }
                    }

                    /*Insert with subclasses*/
                    var repo = OrmService.Db.GetRepository<StreamDvrPlan>();
                    repo.DbContextOptions.EnableAddOrUpdateNavigateList = true; //Need to open manually
                    var ret = repo.Insert(tmpStream);
                    /*Insert with subclasses*/
                    if (ret != null)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                rs.Code = ErrorNumber.SystemDataBaseExcept;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseExcept] + "\r\n" + ex.Message;

                return false;
            }
        }
    }
}