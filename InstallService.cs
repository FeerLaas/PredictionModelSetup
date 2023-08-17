using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictionModelSetup
{
    internal class InstallService
    {
        private void StdOutHandler(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is null) return;

            Console.WriteLine(e.Data);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
            File.AppendAllLines(path, new[] { e.Data });
        }

        public void NewProcess(string FileName = "cmd", string? WorkingDirectory = null, params string[] line)
        {

            foreach (var item in line)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = FileName,
                        Arguments = item,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WorkingDirectory = WorkingDirectory != null ? WorkingDirectory : AppDomain.CurrentDomain.BaseDirectory,
                    }
                };
                process.OutputDataReceived += StdOutHandler;
                process.ErrorDataReceived += StdOutHandler;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }

        public void NewProcess( params string[] line)
        {

            foreach (var item in line)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = item,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    }
                };
                process.OutputDataReceived += StdOutHandler;
                process.ErrorDataReceived += StdOutHandler;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }
        private static int StartPredictionModel()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "python",
                    Arguments = "prediction_model.py",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = predictionPath
                }
            };

            process.Start();

            return process.Id;
        }


    }


}
