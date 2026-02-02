using System.Diagnostics;

string appId = "";
string pName = "";
string pressureVesselCmd = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".steam/debian-installation/steamapps/common/SteamLinuxRuntime_sniper/pressure-vessel/bin/steam-runtime-launch-client");
string gdbPath = "";
string port = "1234";

foreach (string s in args)
{
    if (s.StartsWith("--appId"))
        appId = s.Split('=')[1].Trim();
    else if (s.StartsWith("--processName"))
        pName = s.Split('=')[1].Trim(' ', '"');
    else if (s.StartsWith("--pressureVesselCmd"))
        pressureVesselCmd = s.Split('=')[1].Trim(' ', '"');
    else if (s.StartsWith("--gdb"))
        gdbPath = s.Split('=')[1].Trim(' ', '"');
    else if (s.StartsWith("--port"))
        port = s.Split('=')[1].Trim();
}

if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(pName))
{
    Console.WriteLine("appId or process name is empty");
    return;
}

if (!File.Exists(Path.GetFullPath(gdbPath)))
{
    Console.WriteLine("gdb not found");
    return;
}

if (!File.Exists(pressureVesselCmd))
{
    Console.WriteLine("pressure-vessel not found");
    return;
}

Environment.SetEnvironmentVariable("WINEDEBUG", "-all");
Directory.SetCurrentDirectory("/");
Process process = new Process();
ProcessStartInfo startInfo = new ProcessStartInfo(Path.GetFullPath(pressureVesselCmd));
startInfo.Arguments = $"    --bus-name=com.steampowered.App{appId}  --      bash";
startInfo.UseShellExecute = false;
startInfo.CreateNoWindow = true;
startInfo.RedirectStandardInput = true;
startInfo.RedirectStandardOutput = true;
startInfo.RedirectStandardError = true;
startInfo.EnvironmentVariables["WINEDEBUG"] = "-all";
process.StartInfo = startInfo;
process.Start();

StreamWriter writer = process.StandardInput;
StreamReader reader = process.StandardOutput;
StreamReader error = process.StandardError;
writer.WriteLine("wine cmd");
await reader.ReadLineAsync();
writer.WriteLine($"tasklist | findstr \"{pName}\"\n");

string? taskInfo = await reader.ReadLineAsync();
taskInfo = await reader.ReadLineAsync();

if (string.IsNullOrEmpty(taskInfo))
    return;

string pid = taskInfo.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1].Trim();
Process gdb = new Process();
ProcessStartInfo gdbInfo = new ProcessStartInfo(Path.GetFullPath(pressureVesselCmd));
gdbInfo.Arguments = $"    --bus-name=com.steampowered.App{appId}  --      bash";
gdbInfo.UseShellExecute = false;
gdbInfo.CreateNoWindow = true;
gdbInfo.RedirectStandardInput = true;
gdbInfo.RedirectStandardOutput = true;
gdbInfo.RedirectStandardError = true;
gdbInfo.EnvironmentVariables["WINEDEBUG"] = "-all";
gdb.StartInfo = gdbInfo;
gdb.Start();

StreamWriter gdbWriter = gdb.StandardInput;
gdbWriter.WriteLine($"cd \"{Path.GetDirectoryName(Path.GetFullPath(gdbPath))}\"");
gdbWriter.WriteLine($"wine {Path.GetFileName(gdbPath)} --once localhost:{port} --attach {pid}");

process.Kill();