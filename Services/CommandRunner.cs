using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using ReactGen.UI;

namespace ReactGen.Services;

public record CommandResult(int ExitCode, string Output)
{
    public bool Success => ExitCode == 0;
}

public static class CommandRunner
{
    // Matches color/cursor codes that some tools print.
    private static readonly Regex AnsiPattern =
        new("\u001B\\[[0-9;?]*[ -/]*[@-~]", RegexOptions.Compiled);

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

    // Runs a command through cmd.exe and sends its output line by line to ConsoleUI.
    // Returns the exit code (0 = success).
    public static int RunLive(string command, string? workingDirectory = null)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {command}",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        if (workingDirectory is not null)
        {
            startInfo.WorkingDirectory = workingDirectory;
        }

        try
        {
            using var process = new Process { StartInfo = startInfo };

            process.OutputDataReceived += (_, e) => ShowLine(e.Data);
            process.ErrorDataReceived += (_, e) => ShowLine(e.Data);

            process.Start();

            // Nothing will be typed into the command, so close its input right away.
            process.StandardInput.Close();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            return process.ExitCode;
        }
        catch (Exception ex)
        {
            ConsoleUI.Error(ex.Message);
            return -1;
        }
    }

    private static void ShowLine(string? line)
    {
        if (line is null)
        {
            return;
        }

        var clean = AnsiPattern.Replace(line, "").TrimEnd();

        if (clean.Length > 0)
        {
            ConsoleUI.Info(clean);
        }
    }
}