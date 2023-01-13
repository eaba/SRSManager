using System.Text.Json;
using SharpPulsar.Trino.Message;
using Akka.Actor;
using SharpPulsar;
using SRSManageCommon.ManageStructs;
using SrsManageCommon;
using Akka.Event;
using SRSManager.Messages;
using SrsApis.SrsManager;
using SRSManageCommon.ControllerStructs.ResponseModules;
using SRSManageCommon.ControllerStructs.RequestModules;
using TaskStatus = SRSManageCommon.ManageStructs.TaskStatus;
using SharpPulsar.Trino;
using SharpPulsar.Utils;
using SharpPulsar.Schemas;
using Newtonsoft.Json.Schema;
using SharpPulsar.Builder;
using SharpPulsar.User;
using SharpPulsar.Interfaces;
using SrsConfFile.SRSConfClass;
using System.Security.Policy;
using Ubiety.Dns.Core.Records;
using System;

namespace SRSManager.Actors
{
    internal class DvrPlanActor : ReceiveActor
    {
        private PulsarSystem _pulsarSystem;
        private AvroSchema<DvrVideo> _dvrVideo;
        private AvroSchema<StreamDvrPlan> _streamDvr;
        private IActorRef _cutMergeService;
        private readonly ILoggingAdapter _log;
        private Producer<DvrVideo> _producerDvr;
        private Producer<StreamDvrPlan> _producerStream;
        private ProducerConfigBuilder<DvrVideo> _producerConfigDvr;
        private ProducerConfigBuilder<StreamDvrPlan> _producerConfigStream;
        private PulsarClient _client;
        private string _workPath = Environment.CurrentDirectory + "/";
        private PulsarClientConfigBuilder _pulsarConfig;
        private SystemConfig _systemConfig = null!;
        public DvrPlanActor(PulsarSystem pulsarSystem, IActorRef cutMergeService)
        {
            _dvrVideo = AvroSchema<DvrVideo>.Of(typeof(DvrVideo));
            _streamDvr = AvroSchema<StreamDvrPlan>.Of(typeof(StreamDvrPlan));

            _pulsarSystem = pulsarSystem;
            _log = Context.GetLogger();
            _cutMergeService = cutMergeService;
            ReceiveAsync<DvrPlan>(vhIf => vhIf.Method == "PulsarSrsConfig", async vh =>
            {
                var f = vh.Client!.Value;
                _pulsarConfig = new PulsarClientConfigBuilder()
                .ServiceUrl(f.BrokerUrl);
                _client = await pulsarSystem.NewClient(_pulsarConfig);

                _producerConfigDvr = new ProducerConfigBuilder<DvrVideo>()
                .ProducerName("dvr_video")
                .Schema(_dvrVideo)
                .Topic($"{f.Topic}");

                _producerConfigStream = new ProducerConfigBuilder<StreamDvrPlan>()
                .ProducerName("stream_dvr_plan")
                .Schema(_streamDvr)
                .Topic($"{f.Topic}_stream");
            });
            ReceiveAsync<DvrPlan>(vhIf => vhIf.Method == "GetBacklogTaskList", async vh => 
            {
                await GetBacklogTaskList(Sender);
            });
            ReceiveAsync<DvrPlan>(vhIf => vhIf.Method == "GetMergeTaskStatus", async vh =>
            {
                await GetMergeTaskStatus(vh.TaskId!, Sender);
            });
            ReceiveAsync<DvrPlan>(vhIf => vhIf.Method == "CutOrMergeVideoFile", async vh =>
            {
                await CutOrMergeVideoFile(vh.Rcmv!, Sender);
            });
            ReceiveAsync<DvrPlan>(vhIf => vhIf.Method == "UndoSoftDelete", async vh =>
            {
                await UndoSoftDelete(vh.DvrVideoId!.Value, Sender);
            });
            //Sender.Tell(new ApisResult(false, rs));
        }
        /// <summary>
        /// Get Clipping Backlog List
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        private async ValueTask GetBacklogTaskList(IActorRef sender)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var cutMerge = await _cutMergeService.Ask<List<CutMergeTask>>(CutMergeTaskStatusList.Instance);
            if (cutMerge != null)
            {
                var retList = cutMerge.FindAll(x => x.TaskStatus == SRSManageCommon.ManageStructs.TaskStatus.Create).ToList();
                if (retList != null && retList.Count > 0)
                {
                    var resultList = new List<CutMergeTaskStatusResponse>();
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
                    sender.Tell(new ApisResult(resultList, rs));
                    return;
                }
            }

            sender.Tell(new ApisResult(null!, rs));
            return;
        }

        private async ValueTask GetMergeTaskStatus(string taskId, IActorRef sender)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            
            var cutMerge = await _cutMergeService.Ask<List<CutMergeTask>>(CutMergeTaskStatusList.Instance);
            var ret = cutMerge.FindLast(x => x.TaskId == taskId);
            if (ret == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.DvrCutMergeTaskNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.DvrCutMergeTaskNotExists],
                };
                sender.Tell(new ApisResult(null!, rs));
                return;
            }


            var result = new CutMergeTaskStatusResponse()
            {
                CallbakUrl = ret.CallbakUrl,
                CreateTime = ret.CreateTime,
                ProcessPercentage = ret.ProcessPercentage,
                TaskId = ret.TaskId,
                TaskStatus = ret.TaskStatus,

            };

            sender.Tell(new ApisResult(result, rs));
        }

        private async ValueTask CutOrMergeVideoFile(ReqCutOrMergeVideoFile rcmv, IActorRef sender)
        {
            var rs = new ResponseStruct()
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
                sender.Tell(new ApisResult(null!, rs));
                return ;
            }

            if ((rcmv.EndTime - rcmv.StartTime).Minutes > 120) //Execution of tasks is not allowed for more than 120 minutes
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.DvrCutMergeTimeLimit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.DvrCutMergeTimeLimit],
                };

                sender.Tell(new ApisResult(null!, rs));
                return;

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

                    sender.Tell(new ApisResult(null!, rs));
                    return;
                }

                var mergeList = await AnalysisVideoFile(rcmv);

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
                    var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    Akka.Dispatch.ActorTaskScheduler.RunTask(async () =>
                    {
                        var r =  await _cutMergeService.Ask<CutMergeTaskResponse>(new CutMerge(task));
                        r.Request = rcmv;
                        r.Uri = ":" + Common.SystemConfig.HttpPort + r.FilePath!.Replace(Common.WorkPath + "CutMergeFile", "");
                      
                        sender.Tell(new ApisResult(r, rs));
                    });
                    await tcs.Task;
                    return;
                }

                sender.Tell(new ApisResult(null!, rs));
                return;
            }
            else
            {
                //asynchronous callback
                var mergeList = await AnalysisVideoFile(rcmv);
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
                        _cutMergeService.Tell(new CutMergeTaskListAdd(task));
                        _cutMergeService.Tell(new CutMergeTaskStatusListAdd(task));
                        var s = new CutMergeTaskResponse()
                        {
                            Duration = -1,
                            FilePath = "",
                            FileSize = -1,
                            Status = CutMergeRequestStatus.WaitForCallBack,
                            Task = task,
                            Request = rcmv,
                        };
                        sender.Tell(new ApisResult(s, rs));
                        return;
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct() //An error is reported, the queue is larger than the maximum value
                        {
                            Code = ErrorNumber.DvrCutProcessQueueLimit,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.DvrCutProcessQueueLimit] + "\r\n" +
                                      ex.Message + "\r\n" + ex.StackTrace,
                        };
                        sender.Tell(new ApisResult(null!, rs));
                        return;
                    }
                }

                sender.Tell(new ApisResult(null!, rs));
                return;
            }
        }
        private async ValueTask<List<DvrVideo>> DvrVideosSql(ReqCutOrMergeVideoFile rcmv, string tenant = "public", string nameSpace = "default" )
        {
            var _start = DateTime.Parse(rcmv.StartTime.ToString("yyyy-MM-dd HH:mm:ss")).AddSeconds(-20); //push forward 20 seconds
            var _end = DateTime.Parse(rcmv.EndTime.ToString("yyyy-MM-dd HH:mm:ss")).AddSeconds(20); //20 seconds backward delay

            var topic = _producerConfigDvr.Topic;
            var option = new ClientOptions { Server = "http://127.0.0.1:8081", Execute = 
                @$"select * from ""{topic}"" 
                WHERE Device_Id = '{rcmv.DeviceId}' 
                CAST({rcmv.StartTime.ToString("yyyy-MM-dd HH:mm:ss")} AS timestamp) 
                BETWEEN timestamp '{_start.AddMinutes(-60).ToString("yyyy-MM-dd HH:mm:ss")}' 
                AND timestamp '{_end.AddMinutes(60).ToString("yyyy-MM-dd HH:mm:ss")}' 
                AND Vhost = '{rcmv.VhostDomain}' 
                AND App = '{rcmv.App}' 
                AND Stream = '{rcmv.Stream}' 
                ORDER BY __publish_time__ ASC",
                Catalog = "pulsar", Schema = $"{tenant}/{nameSpace}" };
            var sql = new SqlInstance(_pulsarSystem.System, option);
            var data = await sql.ExecuteAsync();
            var dvr = new List<DvrVideo>(); 
            switch (data.Response)
            {
                case StatsResponse stats:
                    _log.Info(JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case DataResponse dt:
                    for (var i = 0; i < dt.Data.Count; i++)
                    {
                        var ob = dt.Data.ElementAt(i);
                        var json = JsonSerializer.Serialize(ob, new JsonSerializerOptions { WriteIndented = true });
                        dvr.Add(JsonSerializer.Deserialize<DvrVideo>(json)!);
                    }
                    _log.Info(JsonSerializer.Serialize(dt.StatementStats, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case ErrorResponse er:
                    _log.Info(JsonSerializer.Serialize(er, new JsonSerializerOptions { WriteIndented = true }));
                    break;
            }
            return dvr;
        }
        private async ValueTask<DvrVideo> DvrVideoSql(long id, string tenant = "public", string nameSpace = "default")
        {
            var topic = _producerConfigDvr.Topic;
            var option = new ClientOptions
            {
                Server = "http://127.0.0.1:8081",
                Execute =
                @$"select * from ""{topic}"" 
                WHERE Id = '{id}'                  
                ORDER BY __publish_time__ ASC LIMIT 1",
                Catalog = "pulsar",
                Schema = $"{tenant}/{nameSpace}"
            };
            var sql = new SqlInstance(_pulsarSystem.System, option);
            var data = await sql.ExecuteAsync();
            var dvr = new DvrVideo();
            switch (data.Response)
            {
                case StatsResponse stats:
                    _log.Info(JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case DataResponse dt:
                    var d = dt.Data.FirstOrDefault();
                    var json = JsonSerializer.Serialize(d, new JsonSerializerOptions { WriteIndented = true });
                    dvr = JsonSerializer.Deserialize<DvrVideo>(json)!;
                    _log.Info(JsonSerializer.Serialize(dt.StatementStats, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case ErrorResponse er:
                    _log.Info(JsonSerializer.Serialize(er, new JsonSerializerOptions { WriteIndented = true }));
                    break;
            }
            return dvr;
        }

        /// <summary>
        /// Get the list of files that need to be cropped and merged 
        /// </summary>
        /// <param name="rcmv"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private async ValueTask<List<CutMergeStruct>> AnalysisVideoFile(ReqCutOrMergeVideoFile rcmv)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var startPos = -1;
            var endPos = -1;
            var _start = DateTime.Parse(rcmv.StartTime.ToString("yyyy-MM-dd HH:mm:ss")).AddSeconds(-20); //push forward 20 seconds
            var _end = DateTime.Parse(rcmv.EndTime.ToString("yyyy-MM-dd HH:mm:ss")).AddSeconds(20); //20 seconds backward delay
            var videoList = await DvrVideosSql(rcmv);

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
                        DateTime.Parse(((DateTime)videoList[i].StartTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                    var endInDb =
                        DateTime.Parse(((DateTime)videoList[i].EndTime!).ToString("yyyy-MM-dd HH:mm:ss"));
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
                            Math.Abs(((DateTime)videoList[i]!.StartTime!).Subtract(_start)
                                .TotalMilliseconds))); //The start time required to subtract the start time of all videos, take the absolute value
                    }

                    tmpStartList.Sort((left, right) => //Sort by absolute value after subtraction
                    {
                        if (left.Value > right.Value)
                            return 1;
                        else if ((int)left.Value == (int)right.Value)
                            return 0;
                        else
                            return -1;
                    });

                    cutMegerList = videoList.GetRange(tmpStartList[0].Key, endPos - tmpStartList[0].Key + 1); //Take the video closest to the requested time as the start video
                    for (var i = cutMegerList.Count - 1; i >= 0; i--)
                    {
                        if (cutMegerList[i].StartTime > _end && cutMegerList[i].EndTime > _end
                        ) //If the start time of the video is greater than the required end time and it is not the last video, filter out the video
                        {
                            if (i > 0)
                            {                               
                                cutMegerList.Remove(cutMegerList[i]);
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
                            Math.Abs(((DateTime)videoList[i]!.EndTime!).Subtract(_end)
                                .TotalMilliseconds))); //Same as above, take the absolute value
                    }

                    tmpEndList.Sort((left, right) => //to sort
                    {
                        if (left.Value > right.Value)
                            return 1;
                        else if ((int)left.Value == (int)right.Value)
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
                                cutMegerList.Remove(cutMegerList[i]);
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
                            DateTime.Parse(((DateTime)tmpCutMeger.StartTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                        var tmpCutMegerEndTime =
                            DateTime.Parse(((DateTime)tmpCutMeger.EndTime!).ToString("yyyy-MM-dd HH:mm:ss"));
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
                            DateTime.Parse(((DateTime)tmpCutMeger.StartTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                        var tmpCutMegerEndTime =
                            DateTime.Parse(((DateTime)tmpCutMeger.EndTime!).ToString("yyyy-MM-dd HH:mm:ss"));
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
        /// Recover soft-deleted recordings
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private async ValueTask UndoSoftDelete(long dvrVideoId, IActorRef sender)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            DvrVideo retSelect =  await DvrVideoSql(dvrVideoId);
            if (retSelect == null)
            {
                rs.Code = ErrorNumber.SystemDataBaseRecordNotExists;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseRecordNotExists];
                sender.Tell(new ApisResult(false, rs));
                return;
            }

            if (!File.Exists(retSelect.VideoPath))
            {
                rs.Code = ErrorNumber.DvrVideoFileNotExists;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.DvrVideoFileNotExists];
                sender.Tell(new ApisResult(false, rs));
                return;
            }
            var dvr = new DvrVideo 
            {
                Id = dvrVideoId,    
                Deleted = false,
                UpdateTime = DateTime.Now,  
                Undo = false,
                Device_Id = retSelect.Device_Id,
                Client_Id = retSelect.Client_Id,    
                ClientIp = retSelect.ClientIp,  
                ClientType = retSelect.ClientType,  
                MonitorType = retSelect.MonitorType,    
                VideoPath = retSelect.VideoPath,    
                FileSize = retSelect.FileSize,  
                Vhost = retSelect.Vhost,    
                Dir = retSelect.Dir,    
                Stream = retSelect.Stream,
                App = retSelect.App,
                Duration = retSelect.Duration,  
                StartTime = retSelect.StartTime,    
                EndTime = retSelect.EndTime,
                Param = retSelect.Param,    
                RecordDate = retSelect.RecordDate,  
                Url = retSelect.Url    

            };
            if(_producerDvr == null)
            {
                _producerDvr = await _client.NewProducerAsync(_dvrVideo, _producerConfigDvr);
            }
            await _producerDvr.NewMessage().Value(dvr).SendAsync();
            sender.Tell(new ApisResult(true, rs));
        }

        /// <summary>
        /// Delete a video file (hard delete, delete the file immediately, set the database to Delete)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private async ValueTask HardDeleteDvrVideoById(long id)
        {
            var rs = new ResponseStruct()
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
                    .Set(x => x.UpdateTime, DateTime.Now).Where(x => x.Id == (long)id).ExecuteAffrows();
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
        public static Props Prop(PulsarSystem pulsarSystem, IActorRef cutMergeService)
        {
            return Props.Create(() => new DvrPlanActor(pulsarSystem, cutMergeService));
        }
       
    }
    
}
