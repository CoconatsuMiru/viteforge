using ReactGen.Models;
using ReactGen.Services;

namespace ReactGen.UI;

public class MainForm : Form
{
    private const string TitleText = "ReactGen - React + TypeScript + Vite project generator";

    private readonly Label _title = Theme.CreateLabel("_", Theme.TitleFont, Theme.Accent);
    private readonly System.Windows.Forms.Timer _titleTimer = new() { Interval = 30 };
    private int _typedCount;
    private int _blinkTicks;
    private bool _cursorVisible = true;

    private readonly TextBox _nameBox = Theme.CreateTextBox();
    private readonly TextBox _folderBox = Theme.CreateTextBox();
    private readonly Button _browseButton = Theme.CreateButton("Browse...");
    private readonly ComboBox _managerBox = Theme.CreateComboBox("npm", "pnpm", "yarn");
    private readonly CheckBox _tailwindBox = Theme.CreateCheckBox("Tailwind CSS", true);
    private readonly CheckBox _eslintBox = Theme.CreateCheckBox("ESLint", true);
    private readonly CheckBox _prettierBox = Theme.CreateCheckBox("Prettier", true);
    private readonly CheckBox _routerBox = Theme.CreateCheckBox("React Router");
    private readonly CheckBox _supabaseBox = Theme.CreateCheckBox("Supabase");
    private readonly Button _createButton = Theme.CreateButton("Review and create");
    private readonly Label _status = Theme.CreateLabel("");

    public MainForm()
    {
        Text = "ReactGen";
        ClientSize = new Size(760, 570);
        MinimumSize = new Size(640, 570);
        StartPosition = FormStartPosition.CenterScreen;
        Theme.StyleForm(this);

        _title.Location = new Point(24, 20);
        _titleTimer.Tick += OnTitleTick;

        var divider = new Panel
        {
            BackColor = Theme.Border,
            Location = new Point(24, 58),
            Size = new Size(712, 1),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        var nameLabel = Theme.CreateLabel("Project name");
        nameLabel.Location = new Point(24, 76);

        _nameBox.Location = new Point(24, 100);
        _nameBox.Size = new Size(712, 28);
        _nameBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        var folderLabel = Theme.CreateLabel("Destination folder");
        folderLabel.Location = new Point(24, 146);

        _folderBox.Text = @"C:\Dev";
        _folderBox.Location = new Point(24, 170);
        _folderBox.Size = new Size(592, 28);
        _folderBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        _browseButton.Location = new Point(628, 168);
        _browseButton.Size = new Size(108, 32);
        _browseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _browseButton.Click += OnBrowse;

        var managerLabel = Theme.CreateLabel("Package manager");
        managerLabel.Location = new Point(24, 224);

        _managerBox.Location = new Point(24, 248);
        _managerBox.Size = new Size(200, 28);

        var integrationsLabel = Theme.CreateLabel("Optional integrations");
        integrationsLabel.Location = new Point(24, 302);

        _tailwindBox.Location = new Point(24, 330);
        _eslintBox.Location = new Point(24, 360);
        _prettierBox.Location = new Point(24, 390);
        _routerBox.Location = new Point(300, 330);
        _supabaseBox.Location = new Point(300, 360);

        _createButton.Location = new Point(24, 442);
        _createButton.Size = new Size(200, 36);
        _createButton.Click += OnReviewAndCreate;

        _status.AutoSize = false;
        _status.Location = new Point(24, 498);
        _status.Size = new Size(712, 56);
        _status.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        Controls.AddRange(new Control[]
        {
            _title, divider, nameLabel, _nameBox, folderLabel, _folderBox, _browseButton,
            managerLabel, _managerBox, integrationsLabel,
            _tailwindBox, _eslintBox, _prettierBox, _routerBox, _supabaseBox,
            _createButton, _status
        });
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _titleTimer.Start();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _titleTimer.Stop();
        _titleTimer.Dispose();
        base.OnFormClosed(e);
    }

    // Types the title one character at a time, then blinks the cursor.
    private void OnTitleTick(object? sender, EventArgs e)
    {
        if (_typedCount < TitleText.Length)
        {
            _typedCount++;
            _title.Text = TitleText[.._typedCount] + "_";
            return;
        }

        _blinkTicks++;

        if (_blinkTicks % 16 == 0)
        {
            _cursorVisible = !_cursorVisible;
            _title.Text = TitleText + (_cursorVisible ? "_" : " ");
        }
    }

    private void OnBrowse(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Choose where the project folder will be created",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true
        };

        var current = InputValidator.NormalizePath(_folderBox.Text);

        if (Directory.Exists(current))
        {
            dialog.InitialDirectory = current;
        }

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _folderBox.Text = dialog.SelectedPath;
        }
    }

    private void OnReviewAndCreate(object? sender, EventArgs e)
    {
        var name = _nameBox.Text.Trim();
        var folder = _folderBox.Text;

        var error = InputValidator.ValidateProjectName(name)
            ?? InputValidator.ValidateDestinationFolder(folder)
            ?? InputValidator.ValidateProjectFolder(folder, name);

        if (error is not null)
        {
            ShowStatus($"[X] {error}", Theme.Error);
            return;
        }

        var options = new ProjectOptions
        {
            ProjectName = name,
            DestinationFolder = InputValidator.NormalizePath(folder),
            PackageManager = (PackageManager)_managerBox.SelectedIndex,
            UseTailwind = _tailwindBox.Checked,
            UseEslint = _eslintBox.Checked,
            UsePrettier = _prettierBox.Checked,
            UseReactRouter = _routerBox.Checked,
            UseSupabase = _supabaseBox.Checked
        };

        using (var summary = new SummaryDialog(options))
        {
            if (summary.ShowDialog(this) != DialogResult.OK)
            {
                ShowStatus("[!] Cancelled. Nothing was created.", Theme.Warning);
                return;
            }
        }

        using var progress = new ProgressForm(options);
        progress.ShowDialog(this);

        if (progress.Succeeded)
        {
            ShowStatus($"[OK] Project created at {options.ProjectPath}", Theme.Success);
        }
        else
        {
            ShowStatus("[X] Project creation failed. See the log for details.", Theme.Error);
        }
    }

    private void ShowStatus(string message, Color color)
    {
        _status.ForeColor = color;
        _status.Text = message;
    }
}