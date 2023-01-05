using System;
using System.IO;

namespace SrsManageCommon
{
    public static class LogWriter
    {
        public static object lockLogFile = new object();

        public static void WriteLog(string message, string info = "", ConsoleColor color = ConsoleColor.Gray)
        {
            if (color != ConsoleColor.Gray)
            {
                Console.ForegroundColor = color;
            }

            var logPath = Common.LogPath;
            var now = DateTime.Now;
            var logpath = string.Format(logPath + @"streamnodelog_Y{0}M{1}D{2}.log", now.Year, now.Month, now.Day);
            var saveLogTxt = "[" + DateTime.Now.ToString() + "]\t" + message + "\t" + info + "\r\n";
            Console.WriteLine(saveLogTxt.Trim());
            if (color != ConsoleColor.Gray)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            lock (lockLogFile)
            {
                File.AppendAllText(logpath, saveLogTxt);
            }
        }
    }
}