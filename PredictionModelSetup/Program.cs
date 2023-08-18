using Newtonsoft.Json;
using static PredictionModelSetup.Utility.Utils;

namespace PredictionModelSetup;

/// <summary>
/// ================================== PredictionModel Installer ==================================
/// Ferenc Csardas (ferenc.csardas@moanasoftware.com),
/// Peter Forro (peter.forro@moanasoftware.com)
/// </summary>
public class Program
{
    private static readonly string ConfigPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InstallConfig.json");

    private static InstallConfig _config;

    /// <summary>
    /// Loads the InstallConfig.json file
    /// </summary>
    private static async Task<InstallConfig> LoadConfig()
    {
        var json = await File.ReadAllTextAsync(ConfigPath);

        return JsonConvert.DeserializeObject<InstallConfig>(json) ??
            throw new Exception("Cannot load InstallConfig.json");
    }

    /// <summary>
    /// Configures the necessary environment variables
    /// </summary>
    private static void SetEnvironmentVariables()
    {
        var path = Environment.GetEnvironmentVariable(
            variable: "PATH",
            target: EnvironmentVariableTarget.Machine
        );

        path ??= string.Empty;

        if (path.Contains(_config.PythonPath))
            return;

        path =
            $"{_config.PythonPath};" +
            $"{Path.Combine(_config.PythonPath, "Scripts")};" +
            $"{path}";

        Environment.SetEnvironmentVariable(
            variable: "PATH",
            value: path,
            target: EnvironmentVariableTarget.Machine
        );

        Log("Environment variables have been set");
    }

    /// <summary>
    /// Installs the Python interpreter (v3.10.11)
    /// </summary>
    private static async Task InstallPython()
    {
        Log("Installing Python v3.10.11");

        await StartProcess(
            filename: "python.exe",
            arguments: $@"/quiet TargetDir=""{_config.PythonPath}"" InstallAllUsers=1 PrependPath=1",
            stdOut: true,
            wait: true
        );

        Log("Python has been installed");
    }

    
    /// <summary>
    /// Extracts the Python (PIP) packages as dependencies
    /// </summary>
    private static async Task ExtractPipPackages()
    {
        Log("Extracting the necessary Python packages");

        await StartProcess(
            filename: "powershell.exe",
            arguments: $"expand-archive -path ./Python310.zip -DestinationPath {_config.PythonPath} -Force",
            stdOut: true,
            wait: true
        );

        Log("Python packages have been extracted");
    }
    
    /// <summary>
    /// Extracts the PredictionModel to the installation folder
    /// </summary>
    private static async Task ExtractPredictionModel()
    {
        Log("Extracting PredictionModel");

        await StartProcess(
            filename: "powershell.exe",
            arguments: $"expand-archive -path ./bin.zip -DestinationPath {_config.PredictionModelPath} -Force",
            stdOut: true,
            wait: true
        );

        Log("PredictionModel has been extracted");
    }

    private static async Task InstallService()
    {
        Log("Install WindowsService");

        await StartProcess(
            filename: "powershell.exe",
            arguments: $"{_config.WindowsServicePath} && net start {_config.WindowsServiceName}",
            stdOut: true,
            wait: true
        );

        Log("WindowsService has been installed");
    }


    /// <summary>
    /// Starts the prediction model api 
    /// </summary>
    private static async Task StartPredictionModel()
    {
        var pid = await StartProcess(
            filename: "python.exe",
            arguments: $"prediction_model.py {_config.PredictionModelPort}",
            workingDir: _config.PredictionModelPath,
            window: false
        );

        Log($"PredictionModel has been started on port: {_config.PredictionModelPort}, PID={pid}");
    }

    /// <summary>
    /// Main Entry point
    /// </summary>
    public static async Task Main(string[] args)
    {
        try
        {
            _config = await LoadConfig();

            SetEnvironmentVariables();

            if (_config.InstallPython)
                await InstallPython();

            if (_config.ExtractPip)
                await ExtractPipPackages();

            if (_config.ExtractPredictionModel)
                await ExtractPredictionModel();
            if (_config.InstallService) await InstallService();
            if (_config.StartPredictionModel)
                await StartPredictionModel();
        }
        catch (Exception ex)
        {
            Log($"FATAL: {ex}");
        }

        Log("=== Finished ===");
        Console.ReadLine();
    }
}
