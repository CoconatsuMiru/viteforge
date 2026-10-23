using ReactGen.Models;
using ReactGen.Services;

namespace ReactGen.UI;

public class MainForm : Form
{
    private const string TitleText = "> ReactGen";

    private readonly Label _title = Theme.CreateLabel("_", Theme.TitleFont, Theme.Accent);
    private readonly System.Windows.Forms.Timer _titleTimer = new() { Interval = 90 };
    private int _typedCount;
    private int _blinkTicks;
    private bool _cursorVisible = true;

    private readonly InputBox _nameBox = new();
    private readonly InputBox _folderBox = new();
    private readonly Button _browseButton = Theme.CreateButton("Browse...");
    private readonly SegmentedControl _managerBox = new("npm", "pnpm", "yarn");
    private readonly CheckToggle _tailwindBox = new("Tailwind CSS", true);
    private readonly CheckToggle _eslintBox = new("ESLint", true);
    private readonly CheckToggle _prettierBox = new("Prettier", true);
    private readonly CheckToggle _routerBox = new("React Router");
    private readonly CheckToggle _supabaseBox = new("Supabase");
    private readonly Button _createButton = Theme.CreateButton("Review and create", primary: true);
    private readonly Label _status = Theme.CreateLabel("");

    public MainForm()
    {
        Text = "ReactGen";
        ClientSize = new Size(760, 620);
        MinimumSize = new Size(700, 660);
        StartPosition = FormStartPosition.CenterScreen;
        Theme.StyleForm(this);

        // Header
        _title.Location = new Point(24, 16);
        _titleTimer.Tick += OnTitleTick;

        var subtitle = Theme.CreateLabel(
            "Set up a React + TypeScript + Vite project in a few clicks.",
            Theme.SmallFont,
            Theme.Muted);
        subtitle.Location = new Point(26, 46);

        // Card: Project
        var projectCard = new CardPanel("Project")
        {
            Location = new Point(24, 76),
            Size = new Size(712, 172),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        var nameLabel = Theme.CreateLabel("Project name", Theme.SmallFont, Theme.Muted);
        nameLabel.Location = new Point(16, 40);

        _nameBox.Location = new Point(16, 60);
        _nameBox.Size = new Size(680, 34);
        _nameBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        var folderLabel = Theme.CreateLabel("Destination folder", Theme.SmallFont, Theme.Muted);
        folderLabel.Location = new Point(16, 104);

        _folderBox.Value = @"C:\Dev";
        _folderBox.Location = new Point(16, 124);
        _folderBox.Size = new Size(560, 34);
        _folderBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        _browseButton.Location = new Point(588, 124);
        _browseButton.Size = new Size(108, 34);
        _browseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _browseButton.Click += OnBrowse;

        projectCard.Controls.AddRange(new Control[]
        {
            nameLabel, _nameBox, folderLabel, _folderBox, _browseButton
        });

        // Card: Package manager
        var managerCard = new CardPanel("Package manager")
        {
            Location = new Point(24, 260),
            Size = new Size(712, 88),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        _managerBox.Location = new Point(16, 40);
        _managerBox.Size = new Size(346, 34);

        managerCard.Controls.Add(_managerBox);

        // Card: Integrations
        var integrationsCard = new CardPanel("Optional integrations")
        {
            Location = new Point(24, 360),
            Size = new Size(712, 140),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        _tailwindBox.Location = new Point(16, 40);
        _eslintBox.Location = new Point(16, 70);
        _prettierBox.Location = new Point(16, 100);
        _routerBox.Location = new Point(360, 40);
        _supabaseBox.Location = new Point(360, 70);

        integrationsCard.Controls.AddRange(new Control[]
        {
            _tailwindBox, _eslintBox, _prettierBox, _routerBox, _supabaseBox
        });

        // Action row
        _createButton.Location = new Point(24, 516);
        _createButton.Size = new Size(220, 38);
        _createButton.Click += OnReviewAndCreate;
        AcceptButton = (IButtonControl)_createButton;

        _status.AutoSize = false;
        _status.Location = new Point(24, 566);
        _status.Size = new Size(712, 44);
        _status.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        Controls.AddRange(new Control[]
        {
            _title, subtitle, projectCard, managerCard, integrationsCard, _createButton, _status
        });
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        _titleTimer.Start();
        _nameBox.FocusInput();
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

        if (_blinkTicks % 6 == 0)
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

        var current = InputValidator.NormalizePath(_folderBox.Value);

        if (Directory.Exists(current))
        {
            dialog.InitialDirectory = current;
        }

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _folderBox.Value = dialog.SelectedPath;
        }
    }

    private void OnReviewAndCreate(object? sender, EventArgs e)
    {
        var name = _nameBox.Value.Trim();
        var folder = _folderBox.Value;

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