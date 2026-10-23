namespace ReactGen.UI;

// Flat command-prompt style button that fades smoothly between normal and hover colors.
public class AnimatedButton : Button
{
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 15 };
    private float _amount;
    private bool _hovering;
    private bool _pressed;

    public AnimatedButton()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);

        Font = Theme.BodyFont;
        Cursor = Cursors.Hand;

        _timer.Tick += OnTimerTick;
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
        _pressed = false;
        _timer.Start();
        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        _pressed = true;
        Invalidate();
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        _pressed = false;
        Invalidate();
        base.OnMouseUp(e);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Invalidate();
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
        var target = _hovering ? 1f : 0f;
        const float step = 0.18f;

        if (_amount < target)
        {
            _amount = Math.Min(target, _amount + step);
        }
        else if (_amount > target)
        {
            _amount = Math.Max(target, _amount - step);
        }

        if (_amount == target)
        {
            _timer.Stop();
        }

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var amount = Enabled ? _amount : 0f;

        var fill = Theme.Lerp(Theme.Background, Theme.Panel, amount);
        var border = Theme.Lerp(Theme.Border, Theme.Accent, amount);
        var text = Theme.Lerp(Theme.Text, Theme.Accent, amount);

        if (!Enabled)
        {
            text = Theme.Border;
        }
        else
        {
            if (_pressed)
            {
                fill = Color.FromArgb(48, 48, 48);
            }

            if (Focused)
            {
                border = Theme.Accent;
            }
        }

        e.Graphics.Clear(fill);

        using var pen = new Pen(border);
        e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

        TextRenderer.DrawText(
            e.Graphics,
            Text,
            Font,
            ClientRectangle,
            text,
            TextFormatFlags.HorizontalCenter |
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