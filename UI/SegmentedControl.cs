namespace ReactGen.UI;

// A row of options where exactly one is selected (like radio buttons).
// Segments fade smoothly on hover and selection.
public class SegmentedControl : Control
{
    private const int Gap = 8;

    private readonly string[] _items;
    private readonly float[] _selectAmount;
    private readonly float[] _hoverAmount;
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 15 };
    private int _selectedIndex;
    private int _hoverIndex = -1;

    public SegmentedControl(params string[] items)
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);

        _items = items;
        _selectAmount = new float[items.Length];
        _hoverAmount = new float[items.Length];
        _selectAmount[0] = 1f;

        BackColor = Theme.Card;
        Font = Theme.BodyFont;
        Cursor = Cursors.Hand;
        Height = 34;

        _timer.Tick += OnTimerTick;
    }

    public event EventHandler? SelectedIndexChanged;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (value < 0 || value >= _items.Length || value == _selectedIndex)
            {
                return;
            }

            _selectedIndex = value;
            _timer.Start();
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private Rectangle GetSegment(int index)
    {
        var width = (Width - Gap * (_items.Length - 1)) / _items.Length;
        return new Rectangle(index * (width + Gap), 0, width, Height);
    }

    private int GetIndexAt(Point point)
    {
        for (var i = 0; i < _items.Length; i++)
        {
            if (GetSegment(i).Contains(point))
            {
                return i;
            }
        }

        return -1;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        var index = GetIndexAt(e.Location);

        if (index != _hoverIndex)
        {
            _hoverIndex = index;
            _timer.Start();
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);

        _hoverIndex = -1;
        _timer.Start();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        var index = GetIndexAt(e.Location);

        if (index >= 0)
        {
            SelectedIndex = index;
        }
    }

    protected override bool IsInputKey(Keys keyData) =>
        keyData is Keys.Left or Keys.Right || base.IsInputKey(keyData);

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.KeyCode == Keys.Left)
        {
            SelectedIndex = Math.Max(0, _selectedIndex - 1);
        }
        else if (e.KeyCode == Keys.Right)
        {
            SelectedIndex = Math.Min(_items.Length - 1, _selectedIndex + 1);
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        var moving = false;

        for (var i = 0; i < _items.Length; i++)
        {
            moving |= Approach(ref _selectAmount[i], i == _selectedIndex ? 1f : 0f);
            moving |= Approach(ref _hoverAmount[i], i == _hoverIndex ? 1f : 0f);
        }

        if (!moving)
        {
            _timer.Stop();
        }

        Invalidate();
    }

    // Moves value toward target. Returns true while it is still moving.
    private static bool Approach(ref float value, float target)
    {
        const float step = 0.18f;

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
        e.Graphics.Clear(BackColor);

        for (var i = 0; i < _items.Length; i++)
        {
            var rect = GetSegment(i);
            var select = _selectAmount[i];
            var hover = _hoverAmount[i];

            var fill = Theme.Lerp(Theme.Lerp(Theme.Card, Theme.Panel, hover), Theme.AccentDim, select);
            var border = Theme.Lerp(Theme.Lerp(Theme.InputBorder, Theme.Border, hover), Theme.Accent, select);
            var text = Theme.Lerp(Theme.Lerp(Theme.Muted, Theme.Text, hover), Theme.Accent, select);

            using var brush = new SolidBrush(fill);
            e.Graphics.FillRectangle(brush, rect);

            using var pen = new Pen(border);
            e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

            TextRenderer.DrawText(
                e.Graphics,
                _items[i],
                Font,
                rect,
                text,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
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