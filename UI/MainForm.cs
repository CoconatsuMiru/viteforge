using ReactGen.Models;
using ReactGen.Services;

namespace ReactGen.UI;

public class MainForm : Form
{
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

        var title = Theme.CreateLabel(
            "ReactGen - React + TypeScript + Vite project generator",
            Theme.TitleFont,
            Theme.Accent);
        title.Location = new Point(24, 20);

        var nameLabel = Theme.CreateLabel("Project name");
        nameLabel.Location = new Point(24, 70);

        _nameBox.Location = new Point(24, 94);
        _nameBox.Size = new Size(712, 28);
        _nameBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        var folderLabel = Theme.CreateLabel("Destination folder");
        folderLabel.Location = new Point(24, 140);

        _folderBox.Text = @"C:\Dev";
        _folderBox.Location = new Point(24, 164);
        _folderBox.Size = new Size(592, 28);
        _folderBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        _browseButton.Location = new Point(628, 162);
        _browseButton.Size = new Size(108, 32);
        _browseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _browseButton.Click += OnBrowse;

        var managerLabel = Theme.CreateLabel("Package manager");
        managerLabel.Location = new Point(24, 220);

        _managerBox.Location = new Point(24, 244);
        _managerBox.Size = new Size(200, 28);

        var integrationsLabel = Theme.CreateLabel("Optional integrations");
        integrationsLabel.Location = new Point(24, 300);

        _tailwindBox.Location = new Point(24, 328);
        _eslintBox.Location = new Point(24, 358);
        _prettierBox.Location = new Point(24, 388);
        _routerBox.Location = new Point(300, 328);
        _supabaseBox.Location = new Point(300, 358);

        _createButton.Location = new Point(24, 440);
        _createButton.Size = new Size(200, 36);
        _createButton.Click += OnReviewAndCreate;

        _status.AutoSize = false;
        _status.Location = new Point(24, 496);
        _status.Size = new Size(712, 56);
        _status.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        Controls.AddRange(new Control[]
        {
            title, nameLabel, _nameBox, folderLabel, _folderBox, _browseButton,
            managerLabel, _managerBox, integrationsLabel,
            _tailwindBox, _eslintBox, _prettierBox, _routerBox, _supabaseBox,
            _createButton, _status
        });
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

        using var summary = new SummaryDialog(options);

        if (summary.ShowDialog(this) != DialogResult.OK)
        {
            ShowStatus("[!] Cancelled. Nothing was created.", Theme.Warning);
            return;
        }

        ShowStatus("[OK] Confirmed. Project creation comes in the next step.", Theme.Success);
    }

    private void ShowStatus(string message, Color color)
    {
        _status.ForeColor = color;
        _status.Text = message;
    }
}