using System;
using System.Globalization;
using System.IO;

namespace SrsManageCommon
{
    /// <summary>
    /// webapi日志记录类
    /// </summary>
    public static class LogWebApiWriter
    {
        /// <summary>
        /// lock object
        /// </summary>
        private static object _lockLogFile = new object();

        /// <summary>
        /// log write method
        /// </summary>
        /// <param name="message"></param>
        /// <param name="info"></param>
        /// <param name="color"></param>
        public static void WriteWebApiLog(string message, string info = "", ConsoleColor color = ConsoleColor.Gray)
        {
            if (color != ConsoleColor.Gray)
            {
                Console.ForegroundColor = color;
            }

            var now = DateTime.Now;
            var logPath = Path.Combine(Environment.CurrentDirectory, "logs",
                $@"webapilog_Y{now.Year}M{now.Month}D{now.Day}.log");
            var saveLogTxt = "[" + DateTime.Now.ToString(CultureInfo.CurrentCulture) + "]\t" + message + "\t" +
                                info + "\r\n";
            Console.WriteLine(saveLogTxt.Trim());
            if (color != ConsoleColor.Gray)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            lock (_lockLogFile)
            {
                File.AppendAllText(logPath, saveLogTxt);
            }
        }
    }
}