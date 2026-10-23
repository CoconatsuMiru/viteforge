using ReactGen.Models;

namespace ReactGen.UI;

// Shows the chosen options. ShowDialog returns OK if the user confirms.
public class SummaryDialog : Form
{
    public SummaryDialog(ProjectOptions options)
    {
        Text = "Confirm";
        ClientSize = new Size(560, 350);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Theme.StyleForm(this);

        var card = new CardPanel("Summary")
        {
            Location = new Point(24, 24),
            Size = new Size(512, 262)
        };

        var y = 42;

        AddRow(card, ref y, "Project name", options.ProjectName);
        AddRow(card, ref y, "Location", options.ProjectPath);
        AddRow(card, ref y, "Package manager", options.PackageManager.ToCommand());
        AddRow(card, ref y, "Template", "React + TypeScript + Vite");
        AddToggleRow(card, ref y, "Tailwind CSS", options.UseTailwind);
        AddToggleRow(card, ref y, "ESLint", options.UseEslint);
        AddToggleRow(card, ref y, "Prettier", options.UsePrettier);
        AddToggleRow(card, ref y, "React Router", options.UseReactRouter);
        AddToggleRow(card, ref y, "Supabase", options.UseSupabase);

        var createButton = Theme.CreateButton("Create project", primary: true);
        createButton.Location = new Point(24, 302);
        createButton.Size = new Size(170, 38);
        createButton.DialogResult = DialogResult.OK;

        var cancelButton = Theme.CreateButton("Cancel");
        cancelButton.Location = new Point(210, 302);
        cancelButton.Size = new Size(120, 38);
        cancelButton.DialogResult = DialogResult.Cancel;

        AcceptButton = createButton;
        CancelButton = cancelButton;

        Controls.AddRange(new Control[] { card, createButton, cancelButton });
    }

    private static void AddRow(CardPanel card, ref int y, string label, string value, Color? valueColor = null)
    {
        var name = Theme.CreateLabel(label, Theme.SmallFont, Theme.Muted);
        name.Location = new Point(16, y + 2);

        var text = Theme.CreateLabel(value, Theme.BodyFont, valueColor);
        text.AutoSize = false;
        text.AutoEllipsis = true;
        text.Location = new Point(190, y);
        text.Size = new Size(306, 20);

        card.Controls.Add(name);
        card.Controls.Add(text);

        y += 24;
    }

    private static void AddToggleRow(CardPanel card, ref int y, string label, bool value)
    {
        AddRow(card, ref y, label, value ? "Yes" : "No", value ? Theme.Accent : Theme.Muted);
    }
}