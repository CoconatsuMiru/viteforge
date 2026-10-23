namespace ReactGen.UI;

// A thin bar: a block sweeps back and forth while work runs,
// then the bar fills with a final color (green or red).
public class ActivityBar : Control
{
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 20 };
    private float _position;
    private float _direction = 1f;
    private bool _running;
    private bool _finished;
    private Color _color = Theme.Accent;

    public ActivityBar()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);

        Height = 6;
        _timer.Tick += OnTimerTick;
    }

    public void Start()
    {
        _color = Theme.Accent;
        _position = 0f;
        _direction = 1f;
        _finished = false;
        _running = true;
        _timer.Start();
    }

    public void Finish(Color color)
    {
        _timer.Stop();
        _running = false;
        _finished = true;
        _color = color;
        Invalidate();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _position += 0.015f * _direction;

        if (_position >= 1f)
        {
            _position = 1f;
            _direction = -1f;
        }
        else if (_position <= 0f)
        {
            _position = 0f;
            _direction = 1f;
        }

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(Theme.Panel);

        using var brush = new SolidBrush(_color);

        if (_running)
        {
            var blockWidth = Math.Max(40, Width / 4);
            var x = (int)((Width - blockWidth) * _position);
            e.Graphics.FillRectangle(brush, x, 0, blockWidth, Height);
        }
        else if (_finished)
        {
            e.Graphics.FillRectangle(brush, 0, 0, Width, Height);
        }
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