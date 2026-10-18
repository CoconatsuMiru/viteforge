using ReactGen.Services;

namespace ReactGen.UI;

public class MainForm : Form
{
    private readonly TextBox _nameBox = Theme.CreateTextBox();
    private readonly TextBox _folderBox = Theme.CreateTextBox();
    private readonly Button _browseButton = Theme.CreateButton("Browse...");
    private readonly Button _checkButton = Theme.CreateButton("Check inputs");
    private readonly Label _status = Theme.CreateLabel("");

    public MainForm()
    {
        Text = "ReactGen";
        ClientSize = new Size(760, 380);
        MinimumSize = new Size(640, 380);
        StartPosition = FormStartPosition.CenterScreen;
        Theme.StyleForm(this);

        var title = Theme.CreateLabel(
            "ReactGen - React + TypeScript + Vite project generator",
            Theme.TitleFont,
            Theme.Accent);
        title.Location = new Point(24, 20);

        var nameLabel = Theme.CreateLabel("Project name");
        nameLabel.Location = new Point(24, 80);

        _nameBox.Location = new Point(24, 104);
        _nameBox.Size = new Size(712, 28);
        _nameBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        var folderLabel = Theme.CreateLabel("Destination folder");
        folderLabel.Location = new Point(24, 160);

        _folderBox.Text = @"C:\Dev";
        _folderBox.Location = new Point(24, 184);
        _folderBox.Size = new Size(592, 28);
        _folderBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        _browseButton.Location = new Point(628, 182);
        _browseButton.Size = new Size(108, 32);
        _browseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _browseButton.Click += OnBrowse;

        _checkButton.Location = new Point(24, 244);
        _checkButton.Size = new Size(140, 36);
        _checkButton.Click += OnCheck;

        _status.AutoSize = false;
        _status.Location = new Point(24, 300);
        _status.Size = new Size(712, 56);
        _status.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        Controls.AddRange(new Control[]
        {
            title, nameLabel, _nameBox, folderLabel, _folderBox,
            _browseButton, _checkButton, _status
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

    private void OnCheck(object? sender, EventArgs e)
    {
        var name = _nameBox.Text.Trim();
        var folder = _folderBox.Text;

        var error = InputValidator.ValidateProjectName(name)
            ?? InputValidator.ValidateDestinationFolder(folder)
            ?? InputValidator.ValidateProjectFolder(folder, name);

        if (error is not null)
        {
            _status.ForeColor = Theme.Error;
            _status.Text = $"[X] {error}";
            return;
        }

        var projectPath = Path.Combine(InputValidator.NormalizePath(folder), name);

        _status.ForeColor = Theme.Success;
        _status.Text = $"[OK] Will create: {projectPath}";
    }
}