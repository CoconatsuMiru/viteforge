namespace ReactGen.UI;

// A subtle bordered panel with a small cyan marker and a section title.
public class CardPanel : Panel
{
    private readonly string _title;

    public CardPanel(string title)
    {
        _title = title;

        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Theme.Card;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        using var borderPen = new Pen(Theme.CardBorder);
        e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);

        using var markerBrush = new SolidBrush(Theme.Accent);
        e.Graphics.FillRectangle(markerBrush, 16, 14, 3, 13);

        TextRenderer.DrawText(e.Graphics, _title, Theme.SectionFont, new Point(26, 11), Theme.Text);
    }
}