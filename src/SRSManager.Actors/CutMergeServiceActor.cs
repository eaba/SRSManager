using System.Collections.Concurrent;

using Akka.Actor;
using SrsManageCommon.SrsManageCommon;
using SrsManageCommon;
using SRSManageCommon.ManageStructs;
using System.Diagnostics;
using Akka.Event;
using TaskStatus = SRSManageCommon.ManageStructs.TaskStatus;
using SRSManager.Messages;

namespace SRSManager.Actors
{
    internal class CutMergeServiceActor : ReceiveActor
    {
        private  BlockingCollection<CutMergeTask> _cutMergeTaskList = new BlockingCollection<CutMergeTask>(10);
        private List<CutMergeTask> _cutMergeTaskStatusList = new List<CutMergeTask>();

        private readonly ILoggingAdapter _log;
        public CutMergeServiceActor() 
        {
            _log = Context.GetLogger();
            Receive<CutMergeTaskStatusListAdd>(c => _cutMergeTaskStatusList.Add(c.Task));
            Receive<CutMergeTaskListAdd>(c => _cutMergeTaskList.Add(c.Task));
            ReceiveAsync<CutMerge>(async c =>
            {
                var cut = await CutMerge(c.Task);
                Sender.Tell(cut);
            });//CutMergeTaskResponse
            Receive<CutMergeTaskStatusList>(c => Sender.Tell(_cutMergeTaskStatusList));
            Receive<CutMergeTaskList>(c => Sender.Tell(_cutMergeTaskList.GetConsumingEnumerable()));
            Akka.Dispatch.ActorTaskScheduler.RunTask(async() =>
            {
                foreach (var value in _cutMergeTaskList.GetConsumingEnumerable())
                {
                    //  CutMergeTaskStatusList.Add(value);
                    var taskReturn = await CutMerge(value);
                    if (taskReturn != null)
                    {
                        taskReturn.Uri = ":" + Common.SystemConfig.HttpPort +
                                         taskReturn.FilePath!.Replace(Common.WorkPath + "CutMergeFile", "");
                        var postDate = JsonHelper.ToJson(taskReturn);
                        var ret = NetHelperNew.HttpPostRequest(taskReturn.Task.CallbakUrl!, null!, postDate);
                    }
                }

            });
        }
        private async ValueTask<CutMergeTaskResponse> CutMerge(CutMergeTask task)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start(); //  Start monitoring code runtime
            _log.Info($"Merge request task started(CutMerge)...{task.TaskId!}");
            var taskPath = "";
            if (task != null && task.CutMergeFileList != null && task.CutMergeFileList.Count > 0)
            {
                taskPath = Common.WorkPath + "CutMergeDir/" + task.TaskId;
                if (!Directory.Exists(taskPath))
                {
                    Directory.CreateDirectory(taskPath);
                }

                try
                {
                    task = await PackageToTsStreamFile(task); //transfer ts file
                    task.TaskStatus = TaskStatus.Cutting;

                    var cutFileList = task.CutMergeFileList!
                        .FindAll(x => x.CutEndPos != null && x.CutStartPos != null).ToList();
                    for (var i = 0; i <= task.CutMergeFileList!.Count - 1; i++)
                    {
                        if (task.CutMergeFileList[i].CutStartPos != null && task.CutMergeFileList[i].CutEndPos != null)
                        {
                            task.ProcessPercentage += ((double)1 / (double)cutFileList.Count * 100f) * 0.15f;

                            //do cut
                            task.CutMergeFileList[i] = CutProcess(task.CutMergeFileList[i]);
                            await Task.Delay(20);
                        }
                    }

                    var filePath = MergeProcess(task);
                    task.ProcessPercentage = 100f;
                    task.TaskStatus = TaskStatus.Closed;
                    stopwatch.Stop(); //  stop monitoring
                    var timespan = stopwatch.Elapsed;
                    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    {
                        long duration = -1;
                        var newPath = "";
                        FFmpegGetDuration.GetDuration(Common.FFmpegBinPath, filePath, out duration, out newPath);
                        var ret = _cutMergeTaskStatusList.FindLast(x => x.TaskId == task.TaskId);
                        if (ret != null)
                        {

                        }
                        return new CutMergeTaskResponse
                        {
                            FilePath = newPath,
                            FileSize = new FileInfo(filePath).Length,
                            Duration = duration,
                            Status = CutMergeRequestStatus.Succeed,
                            Task = task,
                            TimeConsuming = timespan.TotalMilliseconds,
                        };
                    }

                    return new CutMergeTaskResponse
                    {
                        FilePath = "",
                        Status = CutMergeRequestStatus.Failed,
                        FileSize = -1,
                        Duration = -1,
                        Task = task,
                        TimeConsuming = timespan.TotalMilliseconds,
                    };
                }
                catch (Exception ex)
                {
                    _log.Info($"An exception occurs when cropping and merging video files...", ex.Message + "\r\n" + ex.StackTrace, ConsoleColor.Yellow);
                    return null!;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(taskPath) && Directory.Exists(taskPath)) //清理战场
                    {
                        Directory.Delete(taskPath, true);
                    }

                    if (File.Exists(Common.WorkPath + "CutMergeDir/" + task!.TaskId + "files.txt")
                    ) //clear the battlefield
                    {
                        File.Delete(Common.WorkPath + "CutMergeDir/" + task!.TaskId + "files.txt");
                    }
                }
            }


            return null!;
        }
        /// <summary>
        /// Convert mp4 to ts format package, it may need to catch exception here, timeout is 30 minutes
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private async ValueTask<CutMergeTask> PackageToTsStreamFile(CutMergeTask task)
        {
            task.TaskStatus = TaskStatus.Packaging;
            var tsPath = Common.WorkPath + "CutMergeDir/" + task.TaskId + "/ts";
            if (!Directory.Exists(tsPath))
            {
                Directory.CreateDirectory(tsPath);
            }

            for (var i = 0; i <= task.CutMergeFileList!.Count - 1; i++)
            {
                var videoFileNameWithOutExt = Path.GetFileNameWithoutExtension(task.CutMergeFileList[i]!.FilePath!);
                var videoTsFileName = videoFileNameWithOutExt + ".ts";
                var videoTsFilePath = tsPath + "/" + videoTsFileName;
                var ffmpegCmd = Common.FFmpegBinPath + " -i " + task.CutMergeFileList[i]!.FilePath! +
                                   " -vcodec copy -acodec copy -vbsf h264_mp4toannexb " + videoTsFilePath + " -y";
                var retRun = LinuxShell.Run(ffmpegCmd, 1000 * 60 * 30, out var std, out var err);

                if (retRun && (!string.IsNullOrEmpty(std) || !string.IsNullOrEmpty(err)) &&
                    File.Exists(videoTsFilePath))
                {
                    long find = -1;
                    if (!string.IsNullOrEmpty(std))
                    {
                        var str = Common.GetValue(std, "video:", "audio:");
                        if (!string.IsNullOrEmpty(str))
                        {
                            str = str.ToLower();
                            str = str.Replace("kb", "");
                            long.TryParse(str, out find);
                        }
                    }
                    else if (!string.IsNullOrEmpty(err))
                    {
                        var str = Common.GetValue(err, "video:", "audio:");
                        str = str.ToLower();
                        str = str.Replace("kb", "");
                        long.TryParse(str, out find);
                    }


                    if (find > 0)
                    {
                        task.CutMergeFileList[i].FilePath = videoTsFilePath;
                        LogWriter.WriteLog("Merge request conversion TS task succeeded(packageToTsStreamFile)...",
                            task.TaskId! + "->" + videoTsFilePath);
                    }
                    else
                    {
                        LogWriter.WriteLog("Merge request conversion TS task fails(packageToTsStreamFile)...",
                           task.TaskId! + "->" + videoTsFilePath + " ***\r\n" + err, ConsoleColor.Yellow);
                    }
                }
                task.ProcessPercentage += ((double)1 / (double)task.CutMergeFileList!.Count * 100f) * 0.4f;
                await Task.Delay(20);
            }

            return task;
        }

        /// <summary>
        /// Generate merged files, merge ts files, and output mp4 files at the same time. The -movflags faststart flag allows mp4 to be loaded and played quickly on the web, with a timeout of 30 minutes
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private string MergeProcess(CutMergeTask task)
        {
            task.TaskStatus = TaskStatus.Mergeing;
            var mergePath = Common.WorkPath + "CutMergeDir/" + task.TaskId;
            var outPutPath = Common.WorkPath + "CutMergeFile/" +
                                DateTime.Now.Date.ToString("yyyy-MM-dd");
            if (!Directory.Exists(outPutPath))
            {
                Directory.CreateDirectory(outPutPath);
            }

            var mergeStringList = new List<string>();
            for (var i = 0; i <= task.CutMergeFileList!.Count - 1; i++)
            {
                mergeStringList.Add("file '" + task.CutMergeFileList[i].FilePath + "'");
            }

            File.WriteAllLines(mergePath + "files.txt", mergeStringList);
            var newFilePath = outPutPath + "/" + task.TaskId + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") +
                                 ".mp4";
            var ffmpegCmd = Common.FFmpegBinPath + " -threads " + Common.FFmpegThreadCount.ToString() +
                               " -f concat -safe 0 -i " + mergePath +
                               "files.txt" + " -c copy  -movflags faststart " + newFilePath;
            var retRun = LinuxShell.Run(ffmpegCmd, 1000 * 60 * 30, out var std, out var err);
            task.ProcessPercentage += 40f;
            if (retRun && (!string.IsNullOrEmpty(std) || !string.IsNullOrEmpty(err)) &&
                File.Exists(newFilePath))
            {
                long find = -1;
                if (!string.IsNullOrEmpty(std))
                {
                    var str = Common.GetValue(std, "video:", "audio:");
                    if (!string.IsNullOrEmpty(str))
                    {
                        str = str.ToLower();
                        str = str.Replace("kb", "");
                        long.TryParse(str, out find);
                    }
                }
                else if (!string.IsNullOrEmpty(err))
                {
                    var str = Common.GetValue(err, "video:", "audio:");
                    str = str.ToLower();
                    str = str.Replace("kb", "");
                    long.TryParse(str, out find);
                }

                if (find > 0)
                {
                    LogWriter.WriteLog("Merge request task succeeded(mergeProcess)...", task.TaskId! + "->" + newFilePath);
                    return newFilePath;
                }
            }

            LogWriter.WriteLog("Merge request task failed(mergeProcess失败)...", task.TaskId! + "\r\n" + err, ConsoleColor.Yellow);
            return null!;
        }

        /// <summary>
        /// Trim the video that needs to be trimmed, the timeout is 30 minutes
        /// </summary>
        /// <param name="cms"></param>
        /// <returns></returns>
        private CutMergeStruct CutProcess(CutMergeStruct cms)
        {
            var tsPath = Path.GetDirectoryName(cms.FilePath!)!;
            var fileName = Path.GetFileName(cms.FilePath!)!;
            var newTsName = tsPath + "/cut_" + fileName;


            var ffmpegCmd = Common.FFmpegBinPath + " -i " + cms.FilePath +
                               " -vcodec copy -acodec copy -ss " + cms.CutStartPos + " -to " + cms.CutEndPos + " " +
                               newTsName + " -y";
            var retRun = LinuxShell.Run(ffmpegCmd, 1000 * 60 * 30, out var std, out var err);
            if (retRun && (!string.IsNullOrEmpty(std) || !string.IsNullOrEmpty(err)) &&
                File.Exists(newTsName))
            {
                long find = -1;
                if (!string.IsNullOrEmpty(std))
                {
                    var str = Common.GetValue(std, "video:", "audio:");
                    if (!string.IsNullOrEmpty(str))
                    {
                        str = str.ToLower();
                        str = str.Replace("kb", "");
                        long.TryParse(str, out find);
                    }
                }
                else if (!string.IsNullOrEmpty(err))
                {
                    var str = Common.GetValue(err, "video:", "audio:");
                    str = str.ToLower();
                    str = str.Replace("kb", "");
                    long.TryParse(str, out find);
                }

                if (find > 0)
                {
                    LogWriter.WriteLog("Merge request task trimmed successfully(cutProcess)...", newTsName);
                    cms.FilePath = newTsName;
                }
                else
                {
                    LogWriter.WriteLog("Merge request task pruning failed(cutProcess)...", ffmpegCmd + "\r\n" + err, ConsoleColor.Yellow);
                }
            }

            return cms;
        }

        public static Props Prop()
        {
            return Props.Create(() => new CutMergeServiceActor());
        }
    }
}
