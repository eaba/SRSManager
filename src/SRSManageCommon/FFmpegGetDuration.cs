using System;
using System.IO;

namespace SrsManageCommon
{
    public static class FFmpegGetDuration
    {
        private static bool ifNotMp4(string ffmpegBinPath, string videoFilePath,out string videoPath)
        {
            string ext = Path.GetExtension(videoFilePath);
            string newFileName = videoFilePath.Replace(ext, ".mp4");
            string ffmpegCmd = ffmpegBinPath + " -i " + videoFilePath + " -c copy -movflags faststart " +
                               newFileName;
            videoPath = newFileName;
            if (!string.IsNullOrEmpty(ext) && !ext.Trim().ToLower().Equals(".mp4"))
            {
               

                if (LinuxShell.Run(ffmpegCmd, 60 * 1000 * 5, out string std, out string err))
                {
                    if (!string.IsNullOrEmpty(std) || !string.IsNullOrEmpty(err))
                    {
                        if (File.Exists(newFileName))
                        {
                            FileInfo fi = new FileInfo(newFileName);
                            if (fi.Length > 100)
                            {
                                File.Delete(videoFilePath);
                                return true;
                            }
                            return false;
                        }
                        return false;
                    }

                    return false;
                }
                
            }
            return true;
        }

        /// <summary>
        /// The duration of the output video (milliseconds)
        /// </summary>
        /// <param name="ffmpegBinPath"></param>
        /// <param name="videoFilePath"></param>
        /// <param name="duartion"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool GetDuration(string ffmpegBinPath, string videoFilePath, out long duartion,out string path)
        {
            duartion = -1;
            if (File.Exists(ffmpegBinPath) && File.Exists(videoFilePath))
            {
                string newPath = "";
                var ret = ifNotMp4(ffmpegBinPath, videoFilePath, out newPath);
                if (ret)
                {
                    videoFilePath = newPath;
                }
                path = videoFilePath;
                string cmd = ffmpegBinPath + " -i " + videoFilePath;
                if (LinuxShell.Run(cmd, 1000, out string std, out string err))
                {
                    if (!string.IsNullOrEmpty(std) || !string.IsNullOrEmpty(err))
                    {
                        string tmp = "";
                        if (!string.IsNullOrEmpty(std))
                        {
                            tmp = Common.GetValue(std, "Duration:", ",");
                        }

                        if (string.IsNullOrEmpty(tmp))
                        {
                            tmp = Common.GetValue(err, "Duration:", ",");
                        }

                        if (!string.IsNullOrEmpty(tmp))
                        {
                            string[] tmpArr = tmp.Split(':', StringSplitOptions.RemoveEmptyEntries);
                            if (tmpArr.Length == 3)
                            {
                                int hour = int.Parse(tmpArr[0]);
                                int min = int.Parse(tmpArr[1]);
                                int sec = 0;
                                int msec = 0;
                                if (tmpArr[2].Contains('.'))
                                {
                                    string[] tmpArr2 = tmpArr[2].Split('.', StringSplitOptions.RemoveEmptyEntries);
                                    sec = int.Parse(tmpArr2[0]);
                                    msec = int.Parse(tmpArr2[1]);
                                }
                                else
                                {
                                    sec = int.Parse(tmpArr[2]);
                                }

                                hour = hour * 3600; //convert to seconds
                                min = min * 60;
                                sec = sec + hour + min; //total seconds
                                duartion = sec * 1000 + (msec * 10); //in milliseconds
                                LogWriter.WriteLog("Get video duration：" + duartion.ToString() + "millisecond", videoFilePath);
                                return true;
                            }
                        }
                    }
                }
            }

            path = videoFilePath;
            return false;
        }
    }
}