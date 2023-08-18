#nullable disable

namespace PredictionModelSetup
{
    /// <summary>
    /// Represents the application configuration file
    /// </summary>
    internal class InstallConfig
    {
        public string PythonPath { get; init; }

        public string PredictionModelPath { get; init; }

        public string PredictionModelPort { get; init; }

        public bool InstallPython { get; init; }

        public bool ExtractPip { get; init; }

        public bool ExtractPredictionModel { get; init; }

        public bool StartPredictionModel { get; init; }
        public bool InstallService { get; init; }
        public string WindowsServiceName { get; init; }
        public string WindowsServicePath { get; init; }
    }
}
