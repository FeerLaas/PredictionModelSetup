namespace PredictionModelSetup;

public class Program
{

    private static readonly InstallService _helper =
        new InstallService();

    const string PYTHON_PATH = @"C:\Python310";
    const string PREDICTION_PATH = @"C:\PredictionModel";
    const string APPCMD = @"/windows/system32/inetsrv/appcmd.exe";


    /// <summary>
    /// Configures the necessary environment variables
    /// </summary>
    private static void SetEnvVars()
    {
        var scope = EnvironmentVariableTarget.Machine;
        string path = Environment.GetEnvironmentVariable("PATH", scope) ?? "";
        if (!path.Contains(PYTHON_PATH))
        {
            path = PYTHON_PATH + ";" + Path.Combine(PYTHON_PATH, "Scripts") + ";" + path;
            Environment.SetEnvironmentVariable("PATH", path, scope);
            Console.WriteLine("Set PYTHON_PATH");
        }
    }

    /// <summary>
    /// Installs the Python interpreter (v3.10.11)
    /// </summary>
    private static async Task InstallPython()
    {
        Console.WriteLine("Starting Install Python");
        Console.WriteLine("please wait ...");

        await Task.Run(() =>
        {
            _helper.NewProcess("python.exe", null, "/quiet TargetDir=\"" + PYTHON_PATH + "\" InstallAllUsers=1 PrependPath=1");
        });

        Console.WriteLine("Success install Python");
    }

    /// <summary>
    /// Extracts the PIP packages
    /// </summary>
    private static async Task ExtractDependencies()
    {
        await Task.Run(() =>
        {
            _helper.NewProcess("powershell", null, "expand-archive -path ./bin.zip -DestinationPath " + PREDICTION_PATH + " -Force");
            _helper.NewProcess("powershell", null, "expand-archive -path ./Python310.zip -DestinationPath " + PYTHON_PATH + " -Force");
        });

    }

    /// <summary>
    /// Starts the prediction model api 
    /// </summary>
    private static void StartPredictionModel()
    {
        _helper
    }

    /// <summary>
    /// Main Entry point
    /// </summary>
    public static async void Main(string[] args)
    {
        SetEnvVars();
        await InstallPython();
        await ExtractDependencies();
    }
}
