using System.Drawing.Drawing2D;

namespace ReactGen.UI;

// Checkbox with a square box; the fill and tick fade in smoothly when toggled.
public class CheckToggle : Control
{
    private const int BoxSize = 18;

    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 15 };
    private float _checkAmount;
    private float _hoverAmount;
    private bool _checked;
    private bool _hovering;

    public CheckToggle(string text, bool isChecked = false)
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);

        Text = text;
        Font = Theme.BodyFont;
        Cursor = Cursors.Hand;
        BackColor = Theme.Card;
        Size = new Size(240, 26);

        _checked = isChecked;
        _checkAmount = isChecked ? 1f : 0f;

        _timer.Tick += OnTimerTick;
    }

    public event EventHandler? CheckedChanged;

    public bool Checked
    {
        get => _checked;
        set
        {
            if (_checked == value)
            {
                return;
            }

            _checked = value;
            _timer.Start();
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _hovering = true;
        _timer.Start();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _hovering = false;
        _timer.Start();
        base.OnMouseLeave(e);
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        Checked = !Checked;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.KeyCode == Keys.Space)
        {
            Checked = !Checked;
            e.Handled = true;
        }
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        Invalidate();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        var moving = Approach(ref _checkAmount, _checked ? 1f : 0f);
        moving |= Approach(ref _hoverAmount, _hovering ? 1f : 0f);

        if (!moving)
        {
            _timer.Stop();
        }

        Invalidate();
    }

    // Moves value toward target. Returns true while it is still moving.
    private static bool Approach(ref float value, float target)
    {
        const float step = 0.2f;

        if (value < target)
        {
            value = Math.Min(target, value + step);
        }
        else if (value > target)
        {
            value = Math.Max(target, value - step);
        }

        return value != target;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(BackColor);

        var box = new Rectangle(0, (Height - BoxSize) / 2, BoxSize, BoxSize);

        var fill = Theme.Lerp(Theme.InputFill, Theme.Accent, _checkAmount);
        var border = Theme.Lerp(
            Theme.Lerp(Theme.InputBorder, Theme.Border, _hoverAmount),
            Theme.Accent,
            _checkAmount);

        if (Focused)
        {
            border = Theme.Accent;
        }

        using (var brush = new SolidBrush(fill))
        {
            g.FillRectangle(brush, box);
        }

        using (var pen = new Pen(border))
        {
            g.DrawRectangle(pen, box.X, box.Y, box.Width - 1, box.Height - 1);
        }

        if (_checkAmount > 0.05f)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using var tick = new Pen(Theme.Lerp(fill, Theme.Background, _checkAmount), 2f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };

            g.DrawLines(tick, new[]
            {
                new PointF(box.X + 4.5f, box.Y + 9.5f),
                new PointF(box.X + 8f, box.Y + 13f),
                new PointF(box.X + 13.5f, box.Y + 5.5f)
            });
        }

        var textColor = Theme.Lerp(
            Theme.Lerp(Theme.Muted, Theme.Text, _hoverAmount),
            Theme.Text,
            _checkAmount);

        var textRect = new Rectangle(BoxSize + 10, 0, Math.Max(0, Width - BoxSize - 10), Height);

        TextRenderer.DrawText(
            g,
            Text,
            Font,
            textRect,
            textColor,
            TextFormatFlags.Left |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.SingleLine |
            TextFormatFlags.EndEllipsis);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer.Dispose();
        }

        base.Dispose(disposing);
    }
}