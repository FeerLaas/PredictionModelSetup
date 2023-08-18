using System.Diagnostics;

namespace PredictionModelSetup.Utility
{
    /// <summary>
    /// Utility class for starting background processes
    /// </summary>
    public static class Utils
    {
        private static readonly string BaseDir =
            AppDomain.CurrentDomain.BaseDirectory;

        private static readonly string LogPath =
            Path.Combine(BaseDir, "log.txt");


        /// <summary>
        /// Logs the given message to file and to the standard output
        /// </summary>
        public static void Log(string? message)
        {
            if (message is null)
                return;

            Console.WriteLine(message);
            File.AppendAllLines(LogPath, new[] { message });
        }

        /// <summary>
        /// Handles the messages received from the standard output of the newly created proccesses
        /// </summary>
        private static void StdOutHandler(object sender, DataReceivedEventArgs e) =>
            Log(e.Data);

        /// <summary>
        /// Starts new process
        /// </summary>
        public static Task<int> StartProcess(
            string filename,
            string arguments,
            string? workingDir = null,
            bool window = true,
            bool stdOut = false,
            bool wait = false
        )
        {
            return Task.Run(() =>
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = filename,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WorkingDirectory = workingDir ?? BaseDir
                    }
                };

                if (stdOut)
                {
                    process.StartInfo.RedirectStandardOutput = true;
                    process.OutputDataReceived += StdOutHandler;
                    process.StartInfo.RedirectStandardError = true;
                    process.ErrorDataReceived += StdOutHandler;
                }

                if (!window)
                    process.StartInfo.CreateNoWindow = true;

                process.Start();

                if (stdOut)
                {
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }

                if (wait)
                    process.WaitForExit();

                return process.Id;
            });
        }
    }
}
