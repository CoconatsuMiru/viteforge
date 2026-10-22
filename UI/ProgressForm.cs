using ReactGen.Models;
using ReactGen.Services;
using ReactGen.Services.Integrations;

namespace ReactGen.UI;

// Runs the generator, shows its output live, then offers follow-up actions.
public class ProgressForm : Form
{
    private readonly ProjectOptions _options;
    private readonly RichTextBox _log = new();
    private readonly Label _result = Theme.CreateLabel("Working... please wait.", color: Theme.Warning);
    private readonly Button _openButton = Theme.CreateButton("Open in VS Code");
    private readonly Button _copyButton = Theme.CreateButton("Copy start commands");
    private readonly Button _closeButton = Theme.CreateButton("Close");
    private bool _running;

    public bool Succeeded { get; private set; }

    public ProgressForm(ProjectOptions options)
    {
        _options = options;

        Text = "Creating project";
        ClientSize = new Size(820, 520);
        MinimumSize = new Size(640, 400);
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        Theme.StyleForm(this);

        _log.Location = new Point(24, 24);
        _log.Size = new Size(772, 370);
        _log.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _log.ReadOnly = true;
        _log.BackColor = Theme.Background;
        _log.ForeColor = Theme.Text;
        _log.Font = Theme.BodyFont;
        _log.BorderStyle = BorderStyle.FixedSingle;
        _log.DetectUrls = false;

        _result.AutoSize = false;
        _result.Location = new Point(24, 404);
        _result.Size = new Size(772, 40);
        _result.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        _openButton.Location = new Point(24, 456);
        _openButton.Size = new Size(190, 36);
        _openButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        _openButton.Visible = false;
        _openButton.Click += OnOpenVsCode;

        _copyButton.Location = new Point(230, 456);
        _copyButton.Size = new Size(240, 36);
        _copyButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        _copyButton.Visible = false;
        _copyButton.Click += OnCopyCommands;

        _closeButton.Location = new Point(676, 456);
        _closeButton.Size = new Size(120, 36);
        _closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _closeButton.DialogResult = DialogResult.OK;
        _closeButton.Enabled = false;

        Controls.AddRange(new Control[] { _log, _result, _openButton, _copyButton, _closeButton });
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);

        _running = true;
        ConsoleUI.OutputHandler = AppendLog;

        var success = await Task.Run(Generate);

        ConsoleUI.OutputHandler = null;
        _running = false;
        Succeeded = success;

        if (success)
        {
            _result.ForeColor = Theme.Success;
            _result.Text = "[OK] Done.";

            ShowStartCommands();

            _openButton.Visible = true;
            _copyButton.Visible = true;
        }
        else
        {
            _result.ForeColor = Theme.Error;
            _result.Text = "[X] Failed. Check the log above for details.";
        }

        _closeButton.Enabled = true;
        _closeButton.Focus();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // Don't allow closing while commands are still running.
        if (_running)
        {
            e.Cancel = true;
            return;
        }

        base.OnFormClosing(e);
    }

    private bool Generate()
    {
        try
        {
            if (!ToolChecker.EnsureAvailable(_options.PackageManager))
            {
                ConsoleUI.Error("Fix the problems above and try again.");
                return false;
            }

            if (!ProjectScaffolder.Create(_options))
            {
                ConsoleUI.Error("Project creation failed.");
                return false;
            }

            if (!IntegrationRunner.Run(_options))
            {
                ConsoleUI.Error("Integration setup failed.");
                return false;
            }

            ConsoleUI.Info("");
            ConsoleUI.Success($"Project created at {_options.ProjectPath}");
            return true;
        }
        catch (Exception ex)
        {
            ConsoleUI.Error($"Unexpected error: {ex.Message}");
            return false;
        }
    }

    private void ShowStartCommands()
    {
        AppendLog("", ConsoleColor.Gray);
        AppendLog("To start the dev server, run:", ConsoleColor.Cyan);
        AppendLog($"  cd \"{_options.ProjectPath}\"", ConsoleColor.Gray);
        AppendLog($"  {_options.PackageManager.ToDevCommand()}", ConsoleColor.Gray);

        if (_options.UseSupabase)
        {
            AppendLog("", ConsoleColor.Gray);
            AppendLog("[!] Fill in your Supabase URL and key in .env before using Supabase.", ConsoleColor.Yellow);
        }
    }

    private async void OnOpenVsCode(object? sender, EventArgs e)
    {
        _openButton.Enabled = false;

        var (success, message) = await Task.Run(OpenInVsCode);

        _result.ForeColor = success ? Theme.Success : Theme.Error;
        _result.Text = message;
        _openButton.Enabled = true;
    }

    private (bool Success, string Message) OpenInVsCode()
    {
        if (!CommandRunner.RunCapture("code", "--version").Success)
        {
            return (false, "[X] The 'code' command was not found. In VS Code, run: Shell Command: Install 'code' command in PATH");
        }

        var result = CommandRunner.RunCapture("code", $"\"{_options.ProjectPath}\"");

        return result.Success
            ? (true, "[OK] Opened in VS Code.")
            : (false, "[X] Could not open VS Code.");
    }

    private void OnCopyCommands(object? sender, EventArgs e)
    {
        var text = $"cd \"{_options.ProjectPath}\"{Environment.NewLine}{_options.PackageManager.ToDevCommand()}";

        try
        {
            Clipboard.SetText(text);
            _result.ForeColor = Theme.Success;
            _result.Text = "[OK] Start commands copied to the clipboard.";
        }
        catch (Exception)
        {
            _result.ForeColor = Theme.Error;
            _result.Text = "[X] Could not access the clipboard.";
        }
    }

    private void AppendLog(string message, ConsoleColor color)
    {
        try
        {
            BeginInvoke(new Action(() =>
            {
                _log.SelectionStart = _log.TextLength;
                _log.SelectionLength = 0;
                _log.SelectionColor = ToColor(color);
                _log.SelectedText = message + Environment.NewLine;
                _log.ScrollToCaret();
            }));
        }
        catch (InvalidOperationException)
        {
            // Thrown if the window is already closed or disposed. Safe to ignore.
        }
    }

    private static Color ToColor(ConsoleColor color) => color switch
    {
        ConsoleColor.Cyan => Theme.Accent,
        ConsoleColor.Green => Theme.Success,
        ConsoleColor.Yellow => Theme.Warning,
        ConsoleColor.Red => Theme.Error,
        _ => Theme.Text
    };
}