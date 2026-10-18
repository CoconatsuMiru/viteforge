namespace ReactGen.UI;

// Command-prompt look: black background, light gray text, Consolas font.
public static class Theme
{
    public static readonly Color Background = Color.FromArgb(12, 12, 12);
    public static readonly Color Panel = Color.FromArgb(30, 30, 30);
    public static readonly Color Border = Color.FromArgb(118, 118, 118);
    public static readonly Color Text = Color.FromArgb(204, 204, 204);
    public static readonly Color Accent = Color.FromArgb(97, 214, 214);
    public static readonly Color Success = Color.FromArgb(19, 161, 14);
    public static readonly Color Error = Color.FromArgb(231, 72, 86);
    public static readonly Color Warning = Color.FromArgb(249, 241, 165);

    public static readonly Font BodyFont = new("Consolas", 10f);
    public static readonly Font TitleFont = new("Consolas", 14f, FontStyle.Bold);

    public static void StyleForm(Form form)
    {
        form.AutoScaleDimensions = new SizeF(96F, 96F);
        form.AutoScaleMode = AutoScaleMode.Dpi;
        form.BackColor = Background;
        form.ForeColor = Text;
        form.Font = BodyFont;
    }

    public static Label CreateLabel(string text, Font? font = null, Color? color = null)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            Font = font ?? BodyFont,
            ForeColor = color ?? Text,
            BackColor = Color.Transparent
        };
    }

    public static TextBox CreateTextBox()
    {
        return new TextBox
        {
            Font = BodyFont,
            BackColor = Panel,
            ForeColor = Text,
            BorderStyle = BorderStyle.FixedSingle
        };
    }

    public static Button CreateButton(string text)
    {
        var button = new Button
        {
            Text = text,
            Font = BodyFont,
            BackColor = Background,
            ForeColor = Text,
            FlatStyle = FlatStyle.Flat,
            UseVisualStyleBackColor = false,
            Cursor = Cursors.Hand
        };

        button.FlatAppearance.BorderColor = Border;
        button.FlatAppearance.MouseOverBackColor = Panel;
        button.FlatAppearance.MouseDownBackColor = Border;

        return button;
    }
}