using ReactGen.Models;

namespace ReactGen.UI;

// Shows the chosen options. ShowDialog returns OK if the user confirms.
public class SummaryDialog : Form
{
    public SummaryDialog(ProjectOptions options)
    {
        Text = "Confirm";
        ClientSize = new Size(560, 330);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Theme.StyleForm(this);

        var title = Theme.CreateLabel("Summary", Theme.TitleFont, Theme.Accent);
        title.Location = new Point(24, 20);

        var details = Theme.CreateLabel(BuildText(options));
        details.Location = new Point(24, 64);

        var createButton = Theme.CreateButton("Create project");
        createButton.Location = new Point(24, 270);
        createButton.Size = new Size(160, 36);
        createButton.DialogResult = DialogResult.OK;

        var cancelButton = Theme.CreateButton("Cancel");
        cancelButton.Location = new Point(200, 270);
        cancelButton.Size = new Size(120, 36);
        cancelButton.DialogResult = DialogResult.Cancel;

        AcceptButton = createButton;
        CancelButton = cancelButton;

        Controls.AddRange(new Control[] { title, details, createButton, cancelButton });
    }

    private static string BuildText(ProjectOptions options)
    {
        var lines = new[]
        {
            Line("Project name:", options.ProjectName),
            Line("Location:", options.ProjectPath),
            Line("Package manager:", options.PackageManager.ToCommand()),
            Line("Template:", "React + TypeScript + Vite"),
            Line("Tailwind CSS:", YesNo(options.UseTailwind)),
            Line("ESLint:", YesNo(options.UseEslint)),
            Line("Prettier:", YesNo(options.UsePrettier)),
            Line("React Router:", YesNo(options.UseReactRouter)),
            Line("Supabase:", YesNo(options.UseSupabase))
        };

        return string.Join(Environment.NewLine, lines);
    }

    private static string Line(string label, string value) => $"{label,-18}{value}";

    private static string YesNo(bool value) => value ? "Yes" : "No";
}