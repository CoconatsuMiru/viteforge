namespace ReactGen.UI;

// Single-line text input with a border that fades to the accent color while focused.
public class InputBox : Control
{
    private readonly TextBox _textBox = new();
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 15 };
    private float _amount;
    private bool _focused;

    public InputBox()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);

        TabStop = false;
        Height = 34;
        BackColor = Theme.InputFill;

        _textBox.BorderStyle = BorderStyle.None;
        _textBox.BackColor = Theme.InputFill;
        _textBox.ForeColor = Theme.Text;
        _textBox.Font = Theme.BodyFont;
        _textBox.GotFocus += (_, _) => SetFocused(true);
        _textBox.LostFocus += (_, _) => SetFocused(false);

        Controls.Add(_textBox);

        _timer.Tick += OnTimerTick;
    }

    public string Value
    {
        get => _textBox.Text;
        set => _textBox.Text = value;
    }

    public void FocusInput() => _textBox.Focus();

    private void SetFocused(bool focused)
    {
        _focused = focused;
        _timer.Start();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        _textBox.Width = Math.Max(10, Width - 20);
        _textBox.Location = new Point(10, Math.Max(0, (Height - _textBox.Height) / 2));
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _textBox.Focus();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        var target = _focused ? 1f : 0f;
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
        e.Graphics.Clear(Theme.InputFill);

        var border = Theme.Lerp(Theme.InputBorder, Theme.Accent, _amount);

        using var pen = new Pen(border);
        e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
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