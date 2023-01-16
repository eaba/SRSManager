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
using DvrVideoResponseList = SRSManager.Messages.DvrVideoResponseList;
using SrsConfFile.SRSConfClass;
using SrsApis.SrsManager.Apis;
using MySqlX.XDevAPI.Relational;
using Akka.Util.Internal;
using SharpPulsar.Extension;
using Akka.Dispatch.SysMsg;

namespace SRSManager.Actors
{
    internal class DvrPlanActor : ReceiveActor
    {
        private PulsarSystem _pulsarSystem;
        private AvroSchema<DvrVideo> _dvrVideo;
        private AvroSchema<StreamDvrPlan> _streamDvr;
        private PulsarSrsConfig _pulsarSrsConfig;
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
                _pulsarSrsConfig= f;
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
                await UndoSoftDelete(vh.DvrVideoId, Sender);
            });
            ReceiveAsync<DvrPlan>(vhIf => vhIf.Method == "HardDeleteDvrVideoById", async vh =>
            {
                await HardDeleteDvrVideoById(vh.DvrVideoId, Sender);
            });
            ReceiveAsync<DvrPlan>(vhIf => vhIf.Method == "SoftDeleteDvrVideoById", async vh =>
            {
                await SoftDeleteDvrVideoById(vh.DvrVideoId, Sender);
            });
            ReceiveAsync<DvrPlan>(vhIf => vhIf.Method == "GetDvrVideoList", async vh =>
            {
                await GetDvrVideoList(vh.Rgdv!, Sender);
            });
            ReceiveAsync<DvrPlan>(vhIf => vhIf.Method == "DeleteDvrPlanById", async vh =>
            {
                await DeleteDvrPlanById(vh.DvrVideoId, Sender);
            });
            ReceiveAsync<DvrPlan>(vhIf => vhIf.Method == "OnOrOffDvrPlanById", async vh =>
            {
                await OnOrOffDvrPlanById(vh.DvrVideoId, vh.Enable!.Value, Sender);
            });
            ReceiveAsync<DvrPlan>(vhIf => vhIf.Method == "SetDvrPlanById", async vh =>
            {
                await SetDvrPlanById(vh.DvrVideoId, vh.Sdp!, Sender);
            });
            ReceiveAsync<DvrPlan>(vhIf => vhIf.Method == "CreateDvrPlan", async vh =>
            {
                var c = await CreateDvrPlan(vh.Sdp!);
                Sender.Tell(new ApisResult(c.b, c.rs));
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
        private async ValueTask<List<DvrVideo>> DvrVideosSql(ReqCutOrMergeVideoFile rcmv)
        {
            var _start = DateTime.Parse(rcmv.StartTime.ToString("yyyy-MM-dd HH:mm:ss")).AddSeconds(-20); //push forward 20 seconds
            var _end = DateTime.Parse(rcmv.EndTime.ToString("yyyy-MM-dd HH:mm:ss")).AddSeconds(20); //20 seconds backward delay

            var topic = _producerConfigDvr.Topic;
            var option = new ClientOptions { Server = _pulsarSrsConfig.TrinoUrl, Execute = 
                @$"select * from ""{topic}"" 
                WHERE Device_Id = '{rcmv.DeviceId}' 
                CAST({rcmv.StartTime.ToString("yyyy-MM-dd HH:mm:ss")} AS timestamp) 
                BETWEEN timestamp '{_start.AddMinutes(-60).ToString("yyyy-MM-dd HH:mm:ss")}' 
                AND timestamp '{_end.AddMinutes(60).ToString("yyyy-MM-dd HH:mm:ss")}' 
                AND Vhost = '{rcmv.VhostDomain}' 
                AND App = '{rcmv.App}' 
                AND Stream = '{rcmv.Stream}' 
                AND FileDelete = true
                ORDER BY __publish_time__ ASC",
                Catalog = "pulsar", Schema = $"{_pulsarSrsConfig.Tenant}/{_pulsarSrsConfig.NameSpace}" };
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
        private async ValueTask<List<DvrVideo>> DvrVideosSql(long dvrVideoId)
        {
            var topic = _producerConfigDvr.Topic;
            var option = new ClientOptions
            {
                Server = _pulsarSrsConfig.TrinoUrl,
                Execute =
                @$"select * from ""{topic}"" 
                WHERE DvrVideoId = '{dvrVideoId}' 
                AND FileDelete = true
                ORDER BY __publish_time__ ASC",
                Catalog = "pulsar",
                Schema = $"{_pulsarSrsConfig.Tenant}/{_pulsarSrsConfig.NameSpace}"
            };
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
        private async ValueTask<DvrVideo> DvrVideoSql(long dvrVideoId)
        {
            var topic = _producerConfigDvr.Topic;
            var option = new ClientOptions
            {
                Server = _pulsarSrsConfig.TrinoUrl,
                Execute =
                @$"select * from ""{topic}"" 
                WHERE DvrVideoId = '{dvrVideoId}' 
                AND FileDelete = true
                ORDER BY __publish_time__ ASC LIMIT 1",
                Catalog = "pulsar",
                Schema = $"{_pulsarSrsConfig.Tenant}/{_pulsarSrsConfig.NameSpace}"
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
                            var tmpStruct = new CutMergeStruct
                            {
                                DbId = cutMegerList[i].DvrVideoId,
                                Duration = cutMegerList[i].Duration,
                                EndTime = cutMegerList[i].EndTime,
                                FilePath = cutMegerList[i].VideoPath,
                                FileSize = cutMegerList[i].FileSize,
                                StartTime = cutMegerList[i].StartTime
                            };

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
                                DbId = cutMegerList[i].DvrVideoId,
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
                            tmpStruct.DbId = cutMegerList[i].DvrVideoId;
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
                                DbId = cutMegerList[i].DvrVideoId,
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
                            DbId = cutMegerList[i].DvrVideoId,
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
            var retSelect =  await DvrVideoSql(dvrVideoId);
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
            var rets = await DvrVideosSql(dvrVideoId);
            
            foreach (var dvrVideo in rets)
            {
                var dvr = dvrVideo with 
                { DvrVideoId = dvrVideo.DvrVideoId, Deleted = false, UpdateTime = DateTime.Now, Undo = false };
                if (_producerDvr == null)
                {
                    _producerDvr = await _client.NewProducerAsync(_dvrVideo, _producerConfigDvr);
                }
                await _producerDvr.NewMessage().Value(dvr).SendAsync();
            }

            if (rets.Count > 0)
            {
                sender.Tell(new ApisResult(true, rs));
                return;
            }

            sender.Tell(new ApisResult(false, rs));
        }

        /// <summary>
        /// Delete a video file (soft delete, only mark but not delete the file, the file will be deleted after 24 hours)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private async ValueTask SoftDeleteDvrVideoById(long dvrVideoId, IActorRef sender)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var rets = await DvrVideosSql(dvrVideoId);

            foreach (var dvrVideo in rets)
            {
                var dvr = dvrVideo with
                { DvrVideoId = dvrVideo.DvrVideoId, Deleted = true, UpdateTime = DateTime.Now, Undo = true };
                
                if (_producerDvr == null)
                {
                    _producerDvr = await _client.NewProducerAsync(_dvrVideo, _producerConfigDvr);
                }
                await _producerDvr.NewMessage().Value(dvr).SendAsync();
            }

            if (rets.Count > 0)
            {
                sender.Tell(new ApisResult(true, rs));
                return;
            }

            sender.Tell(new ApisResult(false, rs));
        }
        /// <summary>
        /// Delete a video file (hard delete, delete the file immediately, set the database to Delete)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private async ValueTask HardDeleteDvrVideoById(long dvrVideoId, IActorRef sender)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var retSelect = await DvrVideosSql(dvrVideoId);
            var retUpdate = -1;
            foreach (var dvrVideo in retSelect)
            {
                var dvr = dvrVideo with
                { DvrVideoId = dvrVideo.DvrVideoId, Deleted = true, UpdateTime = DateTime.Now, Undo = false, FileDelete = false };
                
                if (_producerDvr == null)
                {
                    _producerDvr = await _client.NewProducerAsync(_dvrVideo, _producerConfigDvr);
                }
                var id = await _producerDvr.NewMessage().Value(dvr).SendAsync();
                File.Delete(dvrVideo.VideoPath!);
            }
            
            if (retUpdate > 0)
            {
                sender.Tell(new ApisResult(true, rs));
                return;
            }

            sender.Tell(new ApisResult(false, rs));
        }

        /// <summary>
        /// Get video file list
        /// </summary>
        /// <param name="rgdv"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private async ValueTask GetDvrVideoList(ReqGetDvrVideo rgdv, IActorRef sender)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var isPageQuery = (rgdv.PageIndex != null && rgdv.PageIndex >= 1);
            
            if (isPageQuery)
            {
                if (rgdv.PageSize > 10000)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SystemDataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseLimited],
                    };
                    sender.Tell(new ApisResult(null!, rs));
                    return;
                }

                if (rgdv.PageIndex <= 0)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SystemDataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseLimited],
                    };
                    sender.Tell(new ApisResult(null!, rs));
                    return;
                }
            }


            long total = -1;
            var retList = await DvrVideosSql(rgdv);

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
            sender.Tell(new ApisResult(result, rs));
        }
        private async ValueTask<List<DvrVideo>> DvrVideosSql(ReqGetDvrVideo rgdv)
        {
            var idFound = !string.IsNullOrEmpty(rgdv.DeviceId);
            var vhostFound = !string.IsNullOrEmpty(rgdv.VhostDomain);
            var streamFound = !string.IsNullOrEmpty(rgdv.Stream);
            var appFound = !string.IsNullOrEmpty(rgdv.App);
            var isPageQuery = (rgdv.PageIndex != null && rgdv.PageIndex >= 1);
            var haveOrderBy = rgdv.OrderBy != null;
            var topic = _producerConfigDvr.Topic;
            var select = @$"select * from ""{topic}"" WHERE 1=1 ";

            if (!string.IsNullOrEmpty(rgdv.DeviceId))
                select += $"AND Device_Id = {rgdv.DeviceId} ";
             
            if(!string.IsNullOrEmpty(rgdv.VhostDomain))
                select += $"AND Vhost = {rgdv.VhostDomain} ";

            if (!string.IsNullOrEmpty(rgdv.Stream))
                select += $"AND Stream = {rgdv.Stream} ";

            if (!string.IsNullOrEmpty(rgdv.App))
                select += $"AND App = {rgdv.App} ";

            if (!rgdv.IncludeDeleted!.Value)
                select += $"AND Deleted = false ";

            if (rgdv.StartTime != null)
                select += $"AND StartTime >= {rgdv.StartTime} ";

            if (rgdv.EndTime != null)
                select += $"AND EndTime <= {rgdv.EndTime} ";

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

            if (!isPageQuery)
            {
                select += $"Order By {orderBy},  __publish_time__ ASC ";
            }
            else
            {
                var rows = (int)rgdv.PageSize! * ((int)rgdv.PageIndex! - 1);
                select += $"OFFSET {rows} ROWS ";
                select += $" FETCH NEXT {(int)rgdv.PageSize!} ROWS ONLY ";
            }
            var option = new ClientOptions
            {
                Server = _pulsarSrsConfig.TrinoUrl,
                Execute = select,
                Catalog = "pulsar",
                Schema = $"{_pulsarSrsConfig.Tenant}/{_pulsarSrsConfig.NameSpace}"
            };
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
        /// <summary>
        /// Delete a recording plan by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private async ValueTask DeleteDvrPlanById(long id, IActorRef sender)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };


            if (id <= 0)
            {
                rs.Code = ErrorNumber.FunctionInputParamsError;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError];
                sender.Tell(new ApisResult(false, rs));
                return;
            }

            var retSelect = await DvrStreamSql(id);
            var retDelete = -1;
            foreach( var s in retSelect ) 
            {
                var sel = s with { StreamDvrPlanId = id, delete = true };
                await _producerStream.NewMessage().Value(sel).SendAsync();
            }
            
            if (retDelete > 0)
            {
                sender.Tell(new ApisResult(true, rs));
                return;
            }


            rs.Code = ErrorNumber.SrsDvrPlanNotExists;
            rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanNotExists];
            sender.Tell(new ApisResult(false, rs));
            return;
        }
        /// <summary>
        /// Enable or stop a recording schedule
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enable"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private async ValueTask OnOrOffDvrPlanById(long id, bool enable, IActorRef sender)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };


            if (id <= 0)
            {
                rs.Code = ErrorNumber.FunctionInputParamsError;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError];
                sender.Tell(new ApisResult(false, rs));
                return;
            }
            var retSelect = await DvrStreamSql(id);
            var retDelete = -1;
            foreach (var s in retSelect)
            {
                var sel = s with { StreamDvrPlanId = id, Enable = enable };
                if (_producerStream == null)
                {
                    _producerStream = await _client.NewProducerAsync(_streamDvr, _producerConfigStream);
                }
                await _producerStream.NewMessage().Value(sel).SendAsync();
            }

            if (retDelete > 0)
            {
                sender.Tell(new ApisResult(true, rs));
                return;
            }
           
            rs.Code = ErrorNumber.SrsDvrPlanNotExists;
            rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanNotExists];
            sender.Tell(new ApisResult(false, rs));
        }
        private async ValueTask<List<StreamDvrPlan>> DvrStreamSql(long id)
        {
           
            var topic =  _producerConfigStream.Topic;
            var select = @$"select * from ""{topic}"" WHERE StreamDvrPlanId = {id} ";
            
            var option = new ClientOptions
            {
                Server = _pulsarSrsConfig.TrinoUrl,
                Execute = select,
                Catalog = "pulsar",
                Schema = $"{_pulsarSrsConfig.Tenant}/{_pulsarSrsConfig.NameSpace}"
            };
            var sql = new SqlInstance(_pulsarSystem.System, option);
            var data = await sql.ExecuteAsync();
            var stream = new List<StreamDvrPlan>();
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
                        stream.Add(JsonSerializer.Deserialize<StreamDvrPlan>(json)!);
                    }
                    _log.Info(JsonSerializer.Serialize(dt.StatementStats, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case ErrorResponse er:
                    _log.Info(JsonSerializer.Serialize(er, new JsonSerializerOptions { WriteIndented = true }));
                    break;
            }
            return stream;
        }

       
        /// <summary>
        /// modify dvrplan
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sdp"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private async ValueTask SetDvrPlanById(long id, ReqStreamDvrPlan sdp, IActorRef sender)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
           
            var retSrs = await Context.Parent.Ask<SrsManager>(new Messages.System(sdp.DeviceId, method: "GetSrsManagerInstanceByDeviceId")); 
            if (retSrs == null)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                sender.Tell(new ApisResult(false, rs));
                return;
            }
            var retVhost = await Context.Parent.Ask<SrsvHostConfClass>(new Vhost(sdp.DeviceId, sdp.VhostDomain, method: "GetVhostByDomain"));
            if (retVhost == null)
            {
                rs.Code = ErrorNumber.SrsSubInstanceNotFound;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound];
                sender.Tell(new ApisResult(false, rs));
                return;
            }

            if (sdp.TimeRangeList != null && sdp.TimeRangeList.Count > 0)
            {
                foreach (var timeRange in sdp.TimeRangeList)
                {
                    if (timeRange.StartTime >= timeRange.EndTime)
                    {
                        rs.Code = ErrorNumber.FunctionInputParamsError;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError];
                        sender.Tell(new ApisResult(false, rs));
                        return;
                    }

                    if ((timeRange.EndTime - timeRange.StartTime).TotalSeconds <= 120)
                    {
                        rs.Code = ErrorNumber.SrsDvrPlanTimeLimitExcept;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanTimeLimitExcept];

                        sender.Tell(new ApisResult(false, rs));
                        return;
                    }
                }
            }

            try
            {
                StreamDvrPlan retSelect = await DvrStream1Sql(id);
                if (retSelect == null)
                {
                    rs.Code = ErrorNumber.SrsDvrPlanNotExists;
                    rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanNotExists];
                    sender.Tell(new ApisResult(false, rs));
                    return;
                }
                if (_producerStream == null)
                {
                    _producerStream = await _client.NewProducerAsync(_streamDvr, _producerConfigStream);
                }
                var sel = retSelect with { StreamDvrPlanId = id, delete = true };
                await _producerStream.NewMessage().Value(sel).SendAsync();
                var retDelete = -1;
                var retCreate = await CreateDvrPlan(sdp); //create new dvr
                if (retCreate.b)
                {
                    sender.Tell(new ApisResult(true, retCreate.rs));
                }

                sender.Tell(new ApisResult(false, retCreate.rs));
            }
            catch (Exception ex)
            {
                rs.Code = ErrorNumber.SystemDataBaseExcept;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseExcept] + "\r\n" + ex.Message;

                sender.Tell(new ApisResult(false, rs));
            }
        }
        /// <summary>
        /// Create a recording schedule
        /// </summary>
        /// <param name="sdp"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private async ValueTask<(bool b, ResponseStruct rs)> CreateDvrPlan(ReqStreamDvrPlan sdp)
        {
            var rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var srs = await Context.Parent.Ask<ManagerSrs>(GetManagerSrs.Instance);
            if (srs.SRSs.Count == 0)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return (false, rs);
            }
            var retSrs = await Context.Parent.Ask<SrsManager>(new Messages.System(sdp.DeviceId, method: "GetSrsManagerInstanceByDeviceId"));
            
            if (retSrs == null)
            {
                rs.Code = ErrorNumber.SrsObjectNotInit;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsObjectNotInit];
                return (false, rs);
            }

            var retVhost = await Context.Parent.Ask<SrsvHostConfClass>(new Vhost(sdp.DeviceId, sdp.VhostDomain, method: "GetVhostByDomain"));
            if (retVhost == null)
            {
                rs.Code = ErrorNumber.SrsSubInstanceNotFound;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsSubInstanceNotFound];
                return (false, rs);
            }

            if (sdp.TimeRangeList != null && sdp.TimeRangeList.Count > 0)
            {
                foreach (var timeRange in sdp.TimeRangeList)
                {
                    if (timeRange.StartTime >= timeRange.EndTime)
                    {
                        rs.Code = ErrorNumber.FunctionInputParamsError;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError];
                        return (false, rs);
                    }

                    if ((timeRange.EndTime - timeRange.StartTime).TotalSeconds <= 120)
                    {
                        rs.Code = ErrorNumber.SrsDvrPlanTimeLimitExcept;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanTimeLimitExcept];

                        return (false, rs);
                    }
                }
            }

            StreamDvrPlan? retSelect = await DvrStream1Sql(sdp);
            
            if (retSelect != null)
            {
                rs.Code = ErrorNumber.SrsDvrPlanAlreadyExists;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanAlreadyExists];

                return (false, rs);
            }

            try
            {
                var tmpStream = new StreamDvrPlan
                {
                    App = sdp.App,
                    Enable = sdp.Enable,
                    Stream = sdp.Stream,
                    DeviceId = sdp.DeviceId,
                    LimitDays = sdp.LimitDays,
                    LimitSpace = sdp.LimitSpace,
                    VhostDomain = sdp.VhostDomain,
                    OverStepPlan = sdp.OverStepPlan,
                    TimeRangeList = new List<DvrDayTimeRange>()
                };
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

                if (_producerStream == null)
                {
                    _producerStream = await _client.NewProducerAsync(_streamDvr, _producerConfigStream);
                }
                
                await _producerStream.NewMessage().Value(tmpStream).SendAsync();
                return (true, rs);
            }
            catch (Exception ex)
            {
                rs.Code = ErrorNumber.SystemDataBaseExcept;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseExcept] + "\r\n" + ex.Message;

                return (false, rs);
            }
        }
        private async ValueTask<StreamDvrPlan> DvrStream1Sql(ReqStreamDvrPlan sdp)
        {

            var topic = _producerConfigStream.Topic;
            var select = @$"select * from ""{topic}"" 
            WHERE DeviceId = '{sdp.DeviceId}' 
            AND Vhost = '{sdp.VhostDomain}'
            AND Stream = '{sdp.Stream}' 
            AND App = '{sdp.App}' Order By __publish_time__ ASC  LIMIT 1";
            var option = new ClientOptions
            {
                Server = _pulsarSrsConfig.TrinoUrl,
                Execute = select,
                Catalog = "pulsar",
                Schema = $"{_pulsarSrsConfig.Tenant}/{_pulsarSrsConfig.NameSpace}"
            };
            var sql = new SqlInstance(_pulsarSystem.System, option);
            var data = await sql.ExecuteAsync();
            var stream = new StreamDvrPlan();
            switch (data.Response)
            {
                case StatsResponse stats:
                    _log.Info(JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case DataResponse dt:
                    var d = dt.Data.FirstOrDefault();
                    var json = JsonSerializer.Serialize(d, new JsonSerializerOptions { WriteIndented = true });
                    stream = JsonSerializer.Deserialize<StreamDvrPlan>(json)!;
                    _log.Info(JsonSerializer.Serialize(dt.StatementStats, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case ErrorResponse er:
                    _log.Info(JsonSerializer.Serialize(er, new JsonSerializerOptions { WriteIndented = true }));
                    break;
            }
            return stream;
        }

        private async ValueTask<StreamDvrPlan> DvrStream1Sql(long id)
        {

            var topic = _producerConfigStream.Topic;
            var select = @$"select * from ""{topic}"" 
            WHERE StreamDvrPlanId = '{id}'
            Order By __publish_time__ ASC  LIMIT 1";
            var option = new ClientOptions
            {
                Server = _pulsarSrsConfig.TrinoUrl,
                Execute = select,
                Catalog = "pulsar",
                Schema = $"{_pulsarSrsConfig.Tenant}/{_pulsarSrsConfig.NameSpace}"
            };
            var sql = new SqlInstance(_pulsarSystem.System, option);
            var data = await sql.ExecuteAsync();
            var stream = new StreamDvrPlan();
            switch (data.Response)
            {
                case StatsResponse stats:
                    _log.Info(JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case DataResponse dt:
                    var d = dt.Data.FirstOrDefault();
                    var json = JsonSerializer.Serialize(d, new JsonSerializerOptions { WriteIndented = true });
                    stream = JsonSerializer.Deserialize<StreamDvrPlan>(json)!;
                    _log.Info(JsonSerializer.Serialize(dt.StatementStats, new JsonSerializerOptions { WriteIndented = true }));
                    break;
                case ErrorResponse er:
                    _log.Info(JsonSerializer.Serialize(er, new JsonSerializerOptions { WriteIndented = true }));
                    break;
            }
            return stream;
        }
        public static Props Prop(PulsarSystem pulsarSystem, IActorRef cutMergeService)
        {
            return Props.Create(() => new DvrPlanActor(pulsarSystem, cutMergeService));
        }
       
    }
    
}
