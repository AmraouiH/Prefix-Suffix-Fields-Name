using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Prefix_Suffix_Fields_Name.UI
{
    /// <summary>
    /// Option card: icon, title, short description and a toggle. The whole card is clickable;
    /// when on, it is highlighted with the brand color.
    /// </summary>
    [DefaultEvent("CheckedChanged")]
    public class OptionTile : Control
    {
        private bool _checked;
        private bool _hover;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private char _glyph = Theme.Glyph.Info;
        private readonly ToolTip _tooltip = new ToolTip();

        public OptionTile()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            Cursor = Cursors.Hand;
            TabStop = true;
            Margin = new Padding(Theme.Scale(5));
            Height = Theme.Scale(84);
        }

        public event EventHandler CheckedChanged;

        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked == value) return;
                _checked = value;
                Invalidate();
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string Title { get { return _title; } set { _title = value ?? string.Empty; Invalidate(); } }
        public char Glyph { get { return _glyph; } set { _glyph = value; Invalidate(); } }

        /// <summary>Short description (two lines max); the long one goes in <see cref="Help"/>.</summary>
        public string Description { get { return _description; } set { _description = value ?? string.Empty; Invalidate(); } }

        /// <summary>Full explanation shown as tooltip.</summary>
        public string Help { set { _tooltip.SetToolTip(this, value); } }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover = true; Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover = false; Invalidate(); }
        protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }
        protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }
        protected override void OnEnabledChanged(EventArgs e) { base.OnEnabledChanged(e); Invalidate(); }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (!Enabled) return;
            Focus();
            Checked = !Checked;
        }

        protected override bool IsInputKey(Keys keyData) => keyData == Keys.Space || base.IsInputKey(keyData);

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Space && Enabled)
                Checked = !Checked;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _tooltip.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Draw.ParentBack(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var on = _checked && Enabled;
            var bounds = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f);
            var radius = Theme.Scale(8);
            Theme.FillRounded(g, !Enabled ? Theme.SurfaceDisabled : on ? Theme.BrandSubtle : _hover ? Theme.SurfaceHover : Theme.Surface, bounds, radius);
            Theme.DrawRounded(g, !Enabled ? Theme.Stroke : on ? Theme.Brand : _hover ? Theme.StrokeStrong : Theme.Stroke, bounds, radius, on ? 1.5f : 1f);

            var pad = Theme.Scale(12);

            // Icon
            var icon = Theme.Scale(34);
            var iconRect = new Rectangle(pad, pad, icon, icon);
            Theme.FillRounded(g, on ? Theme.Brand : Theme.Blend(Theme.Surface, Theme.Brand, 0.10f), iconRect, Theme.Scale(8));
            Theme.DrawGlyph(g, _glyph, !Enabled ? Theme.TextDisabled : on ? Theme.TextOnBrand : Theme.Brand, iconRect, Theme.Scale(16));

            // Toggle (top right)
            var trackW = Theme.Scale(36);
            var trackH = Theme.Scale(18);
            var track = new RectangleF(Width - pad - trackW, pad + Theme.Scale(2), trackW, trackH);
            var knob = Theme.Scale(10);
            if (on)
            {
                Theme.FillRounded(g, Theme.Brand, track, trackH / 2f);
                using (var b = new SolidBrush(Theme.Surface))
                    g.FillEllipse(b, track.Right - knob - Theme.Scale(4), track.Y + (trackH - knob) / 2f, knob, knob);
            }
            else
            {
                Theme.FillRounded(g, Theme.Surface, track, trackH / 2f);
                Theme.DrawRounded(g, Enabled ? Theme.TextSecondary : Theme.TextDisabled, track, trackH / 2f);
                using (var b = new SolidBrush(Enabled ? Theme.TextSecondary : Theme.TextDisabled))
                    g.FillEllipse(b, track.X + Theme.Scale(4), track.Y + (trackH - knob) / 2f, knob, knob);
            }

            // Texts
            var x = iconRect.Right + Theme.Scale(12);
            var textWidth = (int)track.X - x - Theme.Scale(8);
            TextRenderer.DrawText(g, _title, Theme.BodyStrong, new Rectangle(x, pad - Theme.Scale(1), textWidth, Theme.Scale(20)),
                Enabled ? Theme.TextPrimary : Theme.TextDisabled, Draw.LeftMiddle);
            TextRenderer.DrawText(g, _description, Theme.Caption, new Rectangle(x, pad + Theme.Scale(21), Width - x - pad, Height - pad * 2 - Theme.Scale(20)),
                Enabled ? Theme.TextSecondary : Theme.TextDisabled, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            if (Focused && ShowFocusCues)
                Theme.DrawRounded(g, Theme.TextPrimary, new RectangleF(2.5f, 2.5f, Width - 5.5f, Height - 5.5f), radius - 2);
        }
    }
}
