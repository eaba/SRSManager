using System.Diagnostics;

namespace SrsManageCommon
{
    public static class LinuxShell
    {
        private const string processName = "/bin/bash";

        /// <summary>
        /// Execute CMD command
        /// </summary>
        /// <param name="command">command content</param>
        /// <returns>return execution result</returns>
        public static bool Run(string command) => Run(command, -1);

        /// <summary>
        /// Execute CMD command
        /// </summary>
        /// <param name="command">command content</param>
        /// <param name="milliseconds">Timeout (negative number means infinite wait)</param>
        /// <returns>return execution result</returns>
        public static bool Run(string command, int milliseconds)
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = processName;
                    process.StartInfo.UseShellExecute = false; //Do not use the shell to avoid operating system shell errors
                    process.StartInfo.CreateNoWindow = true; //do not show window

                    process.StartInfo.Arguments = $"-c \"{command.Replace("\"", "\\\"")}\"";
                    if (process.Start())
                    {
                        return process.WaitForExit(milliseconds);
                    }

                    return false;
                }
            }
            catch //Exception directly returns an error
            {
                //exception handling
                return false;
            }
        }

        /// <summary>
        /// Execute CMD command
        /// </summary>
        /// <param name="command">command content</param>
        /// <param name="milliseconds">Timeout (negative number means infinite wait)</param>
        /// <param name="stdOutput">result output</param>
        /// <returns></returns>
        public static bool Run(string command, int milliseconds, out string stdOutput)
        {
            stdOutput = null!;
            try
            {
                var escapedArgs = command.Replace("\"", "\\\"").Replace("$", "\\$");
                using (var process = new Process())
                {
                    process.StartInfo.FileName = processName;
                    process.StartInfo.UseShellExecute = false; //Do not use the shell to avoid operating system shell errors
                    process.StartInfo.CreateNoWindow = true; //do not show window
                    process.StartInfo.RedirectStandardOutput = true;

                    process.StartInfo.Arguments = $"-c \"{escapedArgs}\"";

                    var result = process.Start();
                    if (result)
                    {
                        result = process.WaitForExit(milliseconds);
                    }

                    if (result)
                    {
                        stdOutput = process.StandardOutput.ReadToEnd();
                    }

                    return result;
                }
            }
            catch //Exception directly returns an error
            {
                //exception handling
                return false;
            }
        }

        /// <summary>
        /// Execute CMD command
        /// </summary>
        /// <param name="command">command content</param>
        /// <param name="milliseconds">Timeout (negative number means infinite wait)</param>
        /// <param name="stdOutput">result output</param>
        /// <param name="stdError">error output</param>
        /// <returns></returns>
        public static bool Run(string command, int milliseconds, out string stdOutput, out string stdError)
        {
            stdOutput = null!;
            stdError = null!;
            try
            {
                var escapedArgs = command.Replace("\"", "\\\"");

                using (var process = new Process())
                {
                    process.StartInfo.FileName = processName;
                    process.StartInfo.UseShellExecute = false; //Do not use the shell to avoid operating system shell errors
                    process.StartInfo.CreateNoWindow = true; //do not show window
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.Arguments = $"-c \"{escapedArgs}\"";
                    var result = process.Start();
                    if (result)
                    {
                        result = process.WaitForExit(milliseconds);
                    }

                    if (result)
                    {
                        stdOutput = process.StandardOutput.ReadToEnd();
                        stdError = process.StandardError.ReadToEnd();
                    }

                    return result;
                }
            }
            catch //Exception directly returns an error
            {
                //exception handling
                return false;
            }
        }
    }
}