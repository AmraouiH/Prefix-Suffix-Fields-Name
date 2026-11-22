using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Prefix_Suffix_Fields_Name.UI
{
    internal static class GridStyler
    {
        public const string PercentageColumn = "Percentage Of Use";

        /// <summary>
        /// Fluent table look: roomy rows, horizontal hairlines, quiet header, check glyphs for booleans,
        /// in-cell bars for the percentage of use and an empty state message.
        /// </summary>
        public static void Apply(DataGridView grid, Func<string> emptyText, Func<DataGridViewCellPaintingEventArgs, bool> customPainter = null)
        {
            grid.BackgroundColor = Theme.Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Theme.StrokeSubtle;
            grid.RowHeadersVisible = false;
            grid.AllowUserToResizeRows = false;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = Theme.Scale(36);
            grid.RowTemplate.Height = Theme.Scale(34);

            var header = grid.ColumnHeadersDefaultCellStyle;
            header.BackColor = Theme.Surface;
            header.ForeColor = Theme.TextSecondary;
            header.SelectionBackColor = Theme.Surface;
            header.SelectionForeColor = Theme.TextSecondary;
            header.Font = Theme.BodyStrong;
            header.Padding = new Padding(Theme.Scale(6), 0, Theme.Scale(6), 0);
            header.WrapMode = DataGridViewTriState.False;

            var cell = grid.DefaultCellStyle;
            cell.BackColor = Theme.Surface;
            cell.ForeColor = Theme.TextPrimary;
            cell.SelectionBackColor = Theme.BrandSubtle;
            cell.SelectionForeColor = Theme.TextPrimary;
            cell.Font = Theme.Body;
            cell.Padding = new Padding(Theme.Scale(6), 0, Theme.Scale(6), 0);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Theme.Surface;

            grid.CellPainting += (sender, e) =>
            {
                if (e.ColumnIndex < 0)
                    return;

                if (e.RowIndex == -1)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                    using (var pen = new Pen(Theme.Stroke))
                        e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
                    e.Handled = true;
                    return;
                }

                if (customPainter != null && customPainter(e))
                    return;

                var column = grid.Columns[e.ColumnIndex];
                if (column.DataPropertyName == PercentageColumn && e.Value is double)
                {
                    PaintPercentage(e, (double)e.Value);
                }
                else if (column is DataGridViewCheckBoxColumn)
                {
                    PaintBoolean(e);
                }
            };

            grid.Paint += (sender, e) =>
            {
                var message = emptyText?.Invoke();
                if (grid.Rows.Count > 0 || string.IsNullOrEmpty(message))
                    return;

                var top = grid.ColumnHeadersVisible && grid.Columns.Count > 0 ? grid.ColumnHeadersHeight : 0;
                var area = new Rectangle(0, top, grid.Width, grid.Height - top);
                var iconRect = new RectangleF(0, area.Y + area.Height / 2f - Theme.Scale(34), area.Width, Theme.Scale(28));
                Theme.DrawGlyph(e.Graphics, Theme.Glyph.Info, Theme.TextDisabled, iconRect, Theme.Scale(22));
                TextRenderer.DrawText(e.Graphics, message, Theme.Body,
                    new Rectangle(Theme.Scale(16), area.Y + area.Height / 2 - Theme.Scale(2), area.Width - Theme.Scale(32), Theme.Scale(40)),
                    Theme.TextTertiary, TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);
            };
            grid.Resize += (sender, e) => { if (grid.Rows.Count == 0) grid.Invalidate(); };
        }

        private static bool IsSelected(DataGridViewCellPaintingEventArgs e) => (e.State & DataGridViewElementStates.Selected) != 0;

        private static void PaintBoolean(DataGridViewCellPaintingEventArgs e)
        {
            e.PaintBackground(e.CellBounds, IsSelected(e));
            if (e.Value is bool && (bool)e.Value)
            {
                Theme.DrawGlyph(e.Graphics, Theme.Glyph.CheckMark, Theme.Brand, e.CellBounds, Theme.Scale(12));
            }
            else
            {
                var w = Theme.Scale(8);
                using (var pen = new Pen(Theme.TextDisabled))
                    e.Graphics.DrawLine(pen, e.CellBounds.X + (e.CellBounds.Width - w) / 2, e.CellBounds.Y + e.CellBounds.Height / 2, e.CellBounds.X + (e.CellBounds.Width + w) / 2, e.CellBounds.Y + e.CellBounds.Height / 2);
            }
            e.Handled = true;
        }

        private static void PaintPercentage(DataGridViewCellPaintingEventArgs e, double value)
        {
            e.PaintBackground(e.CellBounds, IsSelected(e));
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = e.CellBounds;
            var pad = Theme.Scale(10);
            var text = value.ToString("0.##") + " %";
            var textSize = TextRenderer.MeasureText(g, text, Theme.Body, Size.Empty, TextFormatFlags.NoPadding);
            var barWidth = Math.Max(Theme.Scale(30), Math.Min(Theme.Scale(96), bounds.Width - textSize.Width - pad * 3));
            var bar = new RectangleF(bounds.X + pad, bounds.Y + (bounds.Height - Theme.Scale(6)) / 2f, barWidth, Theme.Scale(6));

            Theme.FillRounded(g, Theme.StrokeSubtle, bar, bar.Height / 2);
            var fillWidth = (float)(bar.Width * Math.Min(100, Math.Max(0, value)) / 100.0);
            if (fillWidth >= 1)
                Theme.FillRounded(g, value < 5 ? Theme.Warning : Theme.Brand, new RectangleF(bar.X, bar.Y, Math.Max(bar.Height, fillWidth), bar.Height), bar.Height / 2);

            TextRenderer.DrawText(g, text, Theme.Body,
                new Rectangle((int)bar.Right + pad, bounds.Y, bounds.Right - (int)bar.Right - pad, bounds.Height),
                value <= 0 ? Theme.TextTertiary : Theme.TextPrimary, Draw.LeftMiddle);
            g.SmoothingMode = SmoothingMode.Default;
            e.Handled = true;
        }
    }
}
