using System.Runtime.InteropServices;

namespace ReactGen.UI;

// Command-prompt look: near-black surfaces, soft gray text, one cyan accent, Consolas font.
public static class Theme
{
    public static readonly Color Background = Color.FromArgb(12, 12, 12);
    public static readonly Color Card = Color.FromArgb(22, 22, 22);
    public static readonly Color CardBorder = Color.FromArgb(48, 48, 48);
    public static readonly Color Panel = Color.FromArgb(30, 30, 30);
    public static readonly Color InputFill = Color.FromArgb(14, 14, 14);
    public static readonly Color InputBorder = Color.FromArgb(64, 64, 64);
    public static readonly Color Border = Color.FromArgb(118, 118, 118);
    public static readonly Color Text = Color.FromArgb(204, 204, 204);
    public static readonly Color Muted = Color.FromArgb(140, 140, 140);
    public static readonly Color Accent = Color.FromArgb(97, 214, 214);
    public static readonly Color AccentDim = Color.FromArgb(16, 46, 48);
    public static readonly Color Success = Color.FromArgb(19, 161, 14);
    public static readonly Color Error = Color.FromArgb(231, 72, 86);
    public static readonly Color Warning = Color.FromArgb(249, 241, 165);

    public static readonly Font BodyFont = new("Consolas", 10f);
    public static readonly Font SmallFont = new("Consolas", 9f);
    public static readonly Font SectionFont = new("Consolas", 10f, FontStyle.Bold);
    public static readonly Font TitleFont = new("Consolas", 14f, FontStyle.Bold);

    private static readonly Lazy<Icon?> AppIcon = new(LoadAppIcon);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

    public static void StyleForm(Form form)
    {
        form.AutoScaleDimensions = new SizeF(96F, 96F);
        form.AutoScaleMode = AutoScaleMode.Dpi;
        form.BackColor = Background;
        form.ForeColor = Text;
        form.Font = BodyFont;

        if (AppIcon.Value is { } icon)
        {
            form.Icon = icon;
        }

        form.HandleCreated += (_, _) => ApplyDarkTitleBar(form);

        FadeIn(form);
    }

    // Loads the icon embedded in the .exe. Returns null if it can't be read.
    private static Icon? LoadAppIcon()
    {
        try
        {
            using var stream = typeof(Theme).Assembly.GetManifestResourceStream("app.ico");

            return stream is null ? null : new Icon(stream);
        }
        catch (Exception)
        {
            return null;
        }
    }

    // Dark title bar. Attributes 34-36 (border/caption/text color) only work on Windows 11;
    // on other versions the calls fail quietly and the standard dark title bar is used.
    private static void ApplyDarkTitleBar(Form form)
    {
        try
        {
            var dark = 1;

            if (DwmSetWindowAttribute(form.Handle, 20, ref dark, sizeof(int)) != 0)
            {
                DwmSetWindowAttribute(form.Handle, 19, ref dark, sizeof(int));
            }

            var caption = ToColorRef(Background);
            DwmSetWindowAttribute(form.Handle, 35, ref caption, sizeof(int));

            var text = ToColorRef(Text);
            DwmSetWindowAttribute(form.Handle, 36, ref text, sizeof(int));

            var border = ToColorRef(CardBorder);
            DwmSetWindowAttribute(form.Handle, 34, ref border, sizeof(int));
        }
        catch (Exception)
        {
            // Cosmetic only. Ignore if the system doesn't support it.
        }
    }

    private static int ToColorRef(Color color) => color.R | (color.G << 8) | (color.B << 16);

    // Blends two colors. amount 0 = from, 1 = to.
    public static Color Lerp(Color from, Color to, float amount)
    {
        int Mix(int a, int b) => (int)Math.Round(a + (b - a) * amount);

        return Color.FromArgb(Mix(from.R, to.R), Mix(from.G, to.G), Mix(from.B, to.B));
    }

    // Makes the window fade in when it is first shown.
    public static void FadeIn(Form form)
    {
        form.Opacity = 0;

        var timer = new System.Windows.Forms.Timer { Interval = 15 };

        timer.Tick += (_, _) =>
        {
            form.Opacity = Math.Min(1.0, form.Opacity + 0.08);

            if (form.Opacity >= 1.0)
            {
                timer.Stop();
            }
        };

        form.Shown += (_, _) => timer.Start();
        form.FormClosed += (_, _) => timer.Dispose();
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

    public static Button CreateButton(string text, bool primary = false)
    {
        return new AnimatedButton { Text = text, IsPrimary = primary };
    }
}