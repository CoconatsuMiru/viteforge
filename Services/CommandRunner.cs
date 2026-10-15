using System.Diagnostics;
using System.Text;

namespace ReactGen.Services;

public record CommandResult(int ExitCode, string Output)
{
    public bool Success => ExitCode == 0;
}

public static class CommandRunner
{
    // Runs a command through cmd.exe and captures its output (nothing is shown).
    public static CommandResult RunCapture(
        string command,
        string arguments = "",
        string? workingDirectory = null)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {command} {arguments}".TrimEnd(),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        if (workingDirectory is not null)
        {
            startInfo.WorkingDirectory = workingDirectory;
        }

        var output = new StringBuilder();

        try
        {
            using var process = new Process { StartInfo = startInfo };

            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                {
                    output.AppendLine(e.Data);
                }
            };
            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                {
                    output.AppendLine(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            return new CommandResult(process.ExitCode, output.ToString().Trim());
        }
        catch (Exception ex)
        {
            return new CommandResult(-1, ex.Message);
        }
    }

    // Runs a command through cmd.exe and shows its output live in this window.
    // Returns the exit code (0 = success).
    public static int RunLive(string command, string? workingDirectory = null)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {command}",
            UseShellExecute = false
        };

        if (workingDirectory is not null)
        {
            startInfo.WorkingDirectory = workingDirectory;
        }

        try
        {
            using var process = Process.Start(startInfo);

            if (process is null)
            {
                return -1;
            }

            process.WaitForExit();
            return process.ExitCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return -1;
        }
    }
}