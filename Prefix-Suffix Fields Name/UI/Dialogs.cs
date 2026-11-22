using Prefix_Suffix_Fields_Name.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Prefix_Suffix_Fields_Name.UI
{
    /// <summary>
    /// Base Fluent dialog: canvas background, title + subtitle header, footer with secondary / primary buttons.
    /// </summary>
    public class FluentDialog : Form
    {
        protected readonly Panel Body;
        protected readonly FlowLayoutPanel Footer;
        protected readonly FluentButton PrimaryButton;
        protected readonly FluentButton SecondaryButton;
        private readonly string _title;
        private readonly string _subtitle;
        private readonly char _glyph;

        public FluentDialog(string title, string subtitle, char glyph, string primaryText, string secondaryText = "Cancel")
        {
            _title = title;
            _subtitle = subtitle;
            _glyph = glyph;

            Text = title;
            Font = Theme.Body;
            BackColor = Theme.Canvas;
            ForeColor = Theme.TextPrimary;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            AutoScaleMode = AutoScaleMode.Dpi;

            var header = new Panel { Dock = DockStyle.Top, Height = Theme.Scale(72), BackColor = Theme.Canvas };
            header.Paint += PaintHeader;
            header.Resize += (s, e) => header.Invalidate();

            Footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = Theme.Scale(60),
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(Theme.Scale(16), Theme.Scale(12), Theme.Scale(16), Theme.Scale(12)),
                BackColor = Theme.Surface
            };
            Footer.Paint += (s, e) =>
            {
                using (var pen = new Pen(Theme.Stroke))
                    e.Graphics.DrawLine(pen, 0, 0, Footer.Width, 0);
            };

            PrimaryButton = new FluentButton { Text = primaryText, Size = new Size(Theme.Scale(150), Theme.Scale(34)), Margin = new Padding(Theme.Scale(8), 0, 0, 0), DialogResult = DialogResult.OK };
            SecondaryButton = new FluentButton { Text = secondaryText, Primary = false, Size = new Size(Theme.Scale(110), Theme.Scale(34)), Margin = new Padding(0), DialogResult = DialogResult.Cancel };
            Footer.Controls.Add(PrimaryButton);
            Footer.Controls.Add(SecondaryButton);
            AcceptButton = PrimaryButton;
            CancelButton = SecondaryButton;

            Body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(Theme.Scale(10), 0, Theme.Scale(10), Theme.Scale(10)), BackColor = Theme.Canvas };

            Controls.Add(Body);
            Controls.Add(Footer);
            Controls.Add(header);
        }

        private void PaintHeader(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            var panel = (Control)sender;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var icon = Theme.Scale(40);
            var iconRect = new Rectangle(Theme.Scale(18), (panel.Height - icon) / 2, icon, icon);
            Theme.FillRounded(g, Theme.Brand, iconRect, Theme.Scale(8));
            Theme.DrawGlyph(g, _glyph, Theme.TextOnBrand, iconRect, Theme.Scale(18));

            var x = iconRect.Right + Theme.Scale(14);
            using (var titleFont = new Font("Segoe UI Semibold", 14F))
                TextRenderer.DrawText(g, _title, titleFont, new Rectangle(x, Theme.Scale(12), panel.Width - x - Theme.Scale(16), Theme.Scale(28)), Theme.TextPrimary, Draw.LeftMiddle);
            TextRenderer.DrawText(g, _subtitle, Theme.Body, new Rectangle(x, Theme.Scale(40), panel.Width - x - Theme.Scale(16), Theme.Scale(20)), Theme.TextSecondary, Draw.LeftMiddle);
        }
    }

    /// <summary>
    /// Options: three tabs (Clean up / Safety / Analysis) of option tiles laid out in a grid, no scrolling.
    /// </summary>
    public class OptionsDialog : FluentDialog
    {
        private const string NoSolution = "(none)";

        private readonly PivotTabs _tabs;
        private readonly List<Control> _pages = new List<Control>();
        private readonly List<OptionTile> _cleanupTiles = new List<OptionTile>();
        private readonly List<OptionTile> _safetyTiles = new List<OptionTile>();
        private readonly OptionTile _forms, _hide, _views, _charts, _steps, _mappings, _duplicates, _security, _required, _advancedFind, _audit, _note;
        private readonly OptionTile _backup, _skipManaged, _publish, _dryRun, _dataUsage;
        private readonly FluentTextBox _noteText, _backupFolder;
        private readonly FluentComboBox _solution;
        private readonly List<KeyValuePair<string, string>> _solutions;
        private Control _noteRow, _folderRow;

        /// <param name="solutions">Unmanaged solutions: unique name / display name.</param>
        public OptionsDialog(CleanupOptions current, IList<KeyValuePair<string, string>> solutions)
            : base("Options", "Choose what Apply does. Hover a tile for details. A confirmation listing every impact is always shown first.", Theme.Glyph.Personalize, "Save")
        {
            Size = new Size(Theme.Scale(1120), Theme.Scale(740));
            MinimumSize = new Size(Theme.Scale(960), Theme.Scale(640));
            _solutions = solutions?.ToList() ?? new List<KeyValuePair<string, string>>();

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Theme.Canvas };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, Theme.Scale(46)));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Body.Controls.Add(root);

            _tabs = new PivotTabs { Dock = DockStyle.Fill, Margin = new Padding(Theme.Scale(6), 0, Theme.Scale(6), Theme.Scale(4)) };
            _tabs.AddRange("Clean up", "Safety", "Analysis");
            root.Controls.Add(_tabs, 0, 0);

            var host = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Canvas, Margin = new Padding(0) };
            root.Controls.Add(host, 0, 1);

            #region Clean up
            var cleanupGrid = NewGrid(4);
            _forms = Tile(cleanupGrid, _cleanupTiles, Theme.Glyph.Form, "Remove from forms", "Main, Quick Create, Quick View, card. Scripts are kept.",
                "Removes the field from every form of the table: main, Quick Create, Quick View (also shown on other tables), card and header. OnChange scripts are kept. The primary name field is never removed.", current.RemoveFromForms);
            _hide = Tile(cleanupGrid, _cleanupTiles, Theme.Glyph.Hide, "Hide on forms instead", "Keeps the field hidden on the forms: scripts keep working.",
                "Keeps the field on the forms but hidden, so scripts using it keep working. Exclusive with “Remove from forms”.", current.HideOnForms);
            _views = Tile(cleanupGrid, _cleanupTiles, Theme.Glyph.Views, "Remove from views", "Column, sort and Quick Find search. Filters are kept.",
                "Removes the column, the sort and the Quick Find search column from every system view. Filters are kept so views keep returning the same records.", current.RemoveFromViews);
            _charts = Tile(cleanupGrid, _cleanupTiles, Theme.Glyph.Chart, "Remove from charts", "Kept when the field is an axis, a series or a filter.",
                "Removes the field from system charts. A field used as axis, series or filter is kept: the chart would break.", current.RemoveFromCharts);
            _steps = Tile(cleanupGrid, _cleanupTiles, Theme.Glyph.Code, "Remove from plug-in steps", "Filtering attributes and images. Kept if it is the only one.",
                "Removes the field from the filtering attributes and the Pre/Post images of the plug-in steps. Kept when it is the only attribute (an empty list means all attributes).", current.RemoveFromSteps);
            _mappings = Tile(cleanupGrid, _cleanupTiles, Theme.Glyph.Link, "Remove from mappings", "Deletes the relationship field mappings.",
                "Deletes the relationship mappings of the field (values copied from the parent record). System mappings cannot be deleted.", current.RemoveFromMappings);
            _duplicates = Tile(cleanupGrid, _cleanupTiles, Theme.Glyph.Copy, "Remove from duplicate rules", "Deletes the conditions, publishes the rule again.",
                "Deletes the duplicate detection conditions on the field and publishes the rule again. A rule whose only condition is the field is unpublished instead.", current.RemoveFromDuplicateRules);
            _security = Tile(cleanupGrid, _cleanupTiles, Theme.Glyph.Lock, "Remove from field security", "Deletes the field permissions of the profiles.",
                "Deletes the field permissions of every field security profile.", current.RemoveFromFieldSecurity);
            _required = Tile(cleanupGrid, _cleanupTiles, Theme.Glyph.Warning, "Make the field optional", "Requirement level set to Optional.",
                "Sets the requirement level to Optional (business required / recommended → optional). System required fields cannot be changed.", current.MakeNotRequired);
            _advancedFind = Tile(cleanupGrid, _cleanupTiles, Theme.Glyph.Search, "Remove from Advanced Find", "The field can no longer be searched or filtered.",
                "The field can no longer be used to search or filter in Advanced Find.", current.RemoveFromAdvancedFind);
            _audit = Tile(cleanupGrid, _cleanupTiles, Theme.Glyph.History, "Disable auditing", "Stops auditing the field. History is kept.",
                "Stops auditing the field. The existing audit history is kept.", current.DisableAuditing);
            _note = Tile(cleanupGrid, _cleanupTiles, Theme.Glyph.Comment, "Add a deprecation note", "Added before the description, every language.",
                "Adds the note below before the current description, in every language. {date} is replaced by today's date.", current.AddDeprecationNote);

            _noteText = new FluentTextBox { Text = current.DeprecationNote ?? string.Empty, Placeholder = "Deprecated on {date} - do not use.", Glyph = Theme.Glyph.Comment, Font = Theme.Body, Dock = DockStyle.Fill, Margin = new Padding(0) };
            _noteRow = SettingRow("Deprecation note", "{date} = today", _noteText);

            var cleanupToolbar = Presets(
                Preset("Recommended", Theme.Glyph.CheckMark, () => SetCleanup(_forms, _views, _advancedFind, _required, _audit, _note)),
                Preset("All", Theme.Glyph.Add, () => SetCleanup(_cleanupTiles.Where(t => t != _hide).ToArray())),
                Preset("None", Theme.Glyph.Close, () => SetCleanup()));
            AddPage(host, Page("Clean up actions", "applied to the selected fields when the “Options” toggle is on", cleanupToolbar, cleanupGrid, _noteRow));
            #endregion

            #region Safety
            var safetyGrid = NewGrid(2);
            _backup = Tile(safetyGrid, _safetyTiles, Theme.Glyph.Save, "Automatic backup", "Saves forms, views, charts, steps and labels first.",
                "Before any change, saves the field labels and properties, the forms, views, charts, plug-in steps and images of the table in a timestamped folder. If the backup fails, nothing is changed.", current.AutoBackup);
            _skipManaged = Tile(safetyGrid, _safetyTiles, Theme.Glyph.Shield, "Skip managed components", "Never changes components of managed solutions.",
                "Never changes forms, views, charts or plug-in steps coming from a managed solution (avoids unmanaged layers).", current.SkipManaged);
            _publish = Tile(safetyGrid, _safetyTiles, Theme.Glyph.Upload, "Publish automatically", "Publishes the table at the end.",
                "Publishes the table at the end. Off: publish it yourself later.", current.Publish);
            _dryRun = Tile(safetyGrid, _safetyTiles, Theme.Glyph.Play, "Dry run", "Only shows the impact report. Nothing changes.",
                "Only shows the impact report (exportable to CSV). Nothing is ever changed.", current.DryRun);
            // Two tiles on the last row: keep the grid balanced
            safetyGrid.Controls.Add(new Panel { BackColor = Theme.Surface, Dock = DockStyle.Fill });
            safetyGrid.Controls.Add(new Panel { BackColor = Theme.Surface, Dock = DockStyle.Fill });

            _backupFolder = new FluentTextBox { Text = current.EffectiveBackupFolder(), Glyph = Theme.Glyph.Folder, Font = Theme.Body, Dock = DockStyle.Fill, Margin = new Padding(0) };
            var browse = new FluentButton { Text = "Browse", Primary = false, Glyph = Theme.Glyph.OpenInNew, Dock = DockStyle.Fill, Margin = new Padding(Theme.Scale(8), 0, 0, 0) };
            browse.Click += (s, e) =>
            {
                using (var dialog = new FolderBrowserDialog { SelectedPath = _backupFolder.Text, Description = "Backup folder" })
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                        _backupFolder.Text = dialog.SelectedPath;
            };
            _folderRow = SettingRow("Backup folder", "one sub-folder per run", _backupFolder, browse);

            _solution = new FluentComboBox { Dock = DockStyle.Fill, Font = Theme.Body, Margin = new Padding(0) };
            _solution.Items.Add(_solutions.Count == 0 ? "(none) · connect to list the unmanaged solutions" : NoSolution);
            foreach (var s in _solutions)
                _solution.Items.Add($"{s.Value} ({s.Key})");
            var index = _solutions.FindIndex(s => string.Equals(s.Key, current.SolutionUniqueName, StringComparison.OrdinalIgnoreCase));
            if (index < 0 && !string.IsNullOrWhiteSpace(current.SolutionUniqueName))
            {
                _solutions.Add(new KeyValuePair<string, string>(current.SolutionUniqueName, current.SolutionUniqueName));
                _solution.Items.Add($"{current.SolutionUniqueName} ({current.SolutionUniqueName})");
                index = _solutions.Count - 1;
            }
            _solution.SelectedIndex = index + 1;
            var solutionRow = SettingRow("Add to solution", "changed components (ALM)", _solution);

            AddPage(host, Page("Safety", "always applied, even when the “Options” toggle is off", safetyGrid, _folderRow, solutionRow));
            #endregion

            #region Analysis
            var analysisGrid = NewGrid(1);
            var analysisTiles = new List<OptionTile>();
            _dataUsage = Tile(analysisGrid, analysisTiles, Theme.Glyph.Database, "Show data usage", "Records with a value and last modification date.",
                "Counts the records holding a value and shows when the most recent one was modified. Shown in the header of the review window. More than 50,000 records: not counted.", current.ShowDataUsage);
            analysisGrid.Controls.Add(new Panel { BackColor = Theme.Surface, Dock = DockStyle.Fill });
            analysisGrid.Controls.Add(new Panel { BackColor = Theme.Surface, Dock = DockStyle.Fill });
            AddPage(host, Page("Analysis", "what the review window shows", analysisGrid));
            #endregion

            // Behaviour
            _forms.CheckedChanged += (s, e) => { if (_forms.Checked) _hide.Checked = false; };
            _hide.CheckedChanged += (s, e) => { if (_hide.Checked) _forms.Checked = false; };
            _note.CheckedChanged += (s, e) => _noteRow.Enabled = _note.Checked;
            _backup.CheckedChanged += (s, e) => _folderRow.Enabled = _backup.Checked;
            _noteRow.Enabled = _note.Checked;
            _folderRow.Enabled = _backup.Checked;
            foreach (var t in _cleanupTiles.Concat(_safetyTiles).Concat(analysisTiles))
                t.CheckedChanged += (s, e) => UpdateBadges();

            _tabs.SelectedIndexChanged += (s, e) =>
            {
                for (var i = 0; i < _pages.Count; i++)
                    _pages[i].Visible = i == _tabs.SelectedIndex;
            };
            _tabs.SelectedIndex = 0;
            UpdateBadges();
        }

        #region Builders

        private static TableLayoutPanel NewGrid(int rows)
        {
            var grid = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 3, RowCount = rows, BackColor = Theme.Surface, Margin = new Padding(0), Height = rows * Theme.Scale(86) };
            for (var c = 0; c < 3; c++)
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            for (var r = 0; r < rows; r++)
                grid.RowStyles.Add(new RowStyle(SizeType.Absolute, Theme.Scale(86)));
            return grid;
        }

        private static OptionTile Tile(TableLayoutPanel grid, List<OptionTile> list, char glyph, string title, string description, string help, bool value)
        {
            var tile = new OptionTile { Glyph = glyph, Title = title, Description = description, Help = help, Checked = value, Dock = DockStyle.Fill };
            grid.Controls.Add(tile);
            list.Add(tile);
            return tile;
        }

        /// <summary>Preset buttons, right aligned.</summary>
        private static Control Presets(params FluentButton[] buttons)
        {
            var bar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = Theme.Scale(38), FlowDirection = FlowDirection.RightToLeft, WrapContents = false, BackColor = Theme.Surface, Margin = new Padding(0), Padding = new Padding(0, 0, Theme.Scale(2), 0) };
            var label = new Label { Text = "Presets", AutoSize = true, Font = Theme.Caption, ForeColor = Theme.TextTertiary, BackColor = Theme.Surface, Margin = new Padding(0, Theme.Scale(8), Theme.Scale(4), 0) };
            foreach (var b in buttons.Reverse()) bar.Controls.Add(b);
            bar.Controls.Add(label);
            return bar;
        }

        private static FluentButton Preset(string text, char glyph, Action action)
        {
            var button = new FluentButton { Text = text, Primary = false, Glyph = glyph, Size = new Size(Theme.Scale(text.Length > 6 ? 136 : 84), Theme.Scale(30)), Margin = new Padding(Theme.Scale(6), 0, 0, 0) };
            button.Click += (s, e) => action();
            return button;
        }

        /// <summary>Label + input on one line, inside the white card.</summary>
        private static Control SettingRow(string title, string hint, Control input, Control extra = null)
        {
            var row = new TableLayoutPanel { Dock = DockStyle.Top, Height = Theme.Scale(46), ColumnCount = extra == null ? 2 : 3, RowCount = 1, BackColor = Theme.Surface, Margin = new Padding(0), Padding = new Padding(Theme.Scale(6), Theme.Scale(6), Theme.Scale(6), Theme.Scale(6)) };
            row.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Theme.Scale(250)));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            if (extra != null)
            {
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, Theme.Scale(118)));
                row.Controls.Add(extra, 2, 0);
            }
            var label = new Label { Dock = DockStyle.Fill, BackColor = Theme.Surface, Margin = new Padding(0) };
            label.Paint += (s, e) =>
            {
                e.Graphics.Clear(Theme.Surface);
                var w = TextRenderer.MeasureText(e.Graphics, title, Theme.BodyStrong, Size.Empty, TextFormatFlags.NoPadding).Width;
                var color = row.Enabled ? Theme.TextPrimary : Theme.TextDisabled;
                TextRenderer.DrawText(e.Graphics, title, Theme.BodyStrong, new Rectangle(0, 0, w + 2, label.Height), color, Draw.LeftMiddle);
                TextRenderer.DrawText(e.Graphics, hint, Theme.Caption, new Rectangle(w + Theme.Scale(8), 0, label.Width - w - Theme.Scale(8), label.Height), Theme.TextTertiary, Draw.LeftMiddle);
            };
            row.EnabledChanged += (s, e) => label.Invalidate();
            row.Controls.Add(label, 0, 0);
            row.Controls.Add(input, 1, 0);
            return row;
        }

        /// <summary>A white card stacking the given blocks from top to bottom.</summary>
        private static Control Page(string title, string subtitle, params Control[] blocks)
        {
            var card = new CardBox { Dock = DockStyle.Fill, Text = title, Subtitle = subtitle, Padding = new Padding(Theme.Scale(10), 0, Theme.Scale(10), Theme.Scale(10)) };
            var stack = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Surface, AutoScroll = true };
            // Docked Top controls stack in reverse order of addition
            for (var i = blocks.Length - 1; i >= 0; i--)
            {
                stack.Controls.Add(blocks[i]);
                if (i > 0)
                    stack.Controls.Add(new Panel { Dock = DockStyle.Top, Height = Theme.Scale(6), BackColor = Theme.Surface });
            }
            card.Controls.Add(stack);
            return card;
        }

        private void AddPage(Control host, Control page)
        {
            page.Visible = false;
            host.Controls.Add(page);
            _pages.Add(page);
        }

        private void SetCleanup(params OptionTile[] on)
        {
            foreach (var t in _cleanupTiles)
                t.Checked = on.Contains(t);
        }

        private void UpdateBadges()
        {
            _tabs.SetBadge(0, $"{_cleanupTiles.Count(t => t.Checked)}/{_cleanupTiles.Count}");
            _tabs.SetBadge(1, $"{_safetyTiles.Count(t => t.Checked)}/{_safetyTiles.Count}");
            _tabs.SetBadge(2, _dataUsage.Checked ? "on" : "off");
        }

        #endregion

        public CleanupOptions Options => new CleanupOptions
        {
            RemoveFromForms = _forms.Checked,
            HideOnForms = _hide.Checked && !_forms.Checked,
            RemoveFromViews = _views.Checked,
            RemoveFromCharts = _charts.Checked,
            RemoveFromSteps = _steps.Checked,
            RemoveFromMappings = _mappings.Checked,
            RemoveFromDuplicateRules = _duplicates.Checked,
            RemoveFromFieldSecurity = _security.Checked,
            MakeNotRequired = _required.Checked,
            RemoveFromAdvancedFind = _advancedFind.Checked,
            DisableAuditing = _audit.Checked,
            AddDeprecationNote = _note.Checked,
            DeprecationNote = string.IsNullOrWhiteSpace(_noteText.Text) ? "Deprecated on {date} - do not use." : _noteText.Text,
            AutoBackup = _backup.Checked,
            BackupFolder = _backupFolder.Text,
            SkipManaged = _skipManaged.Checked,
            Publish = _publish.Checked,
            DryRun = _dryRun.Checked,
            SolutionUniqueName = _solution.SelectedIndex > 0 ? _solutions[_solution.SelectedIndex - 1].Key : null,
            ShowDataUsage = _dataUsage.Checked
        };
    }

    /// <summary>
    /// Lists every change and every place the fields are used, sorted by priority, before anything is modified.
    /// Data usage is shown in the header. Also used read only ("Usage" button) and for dry runs.
    /// </summary>
    public class ImpactDialog : FluentDialog
    {
        private const string ColPriority = "Priority";
        private const string ColAction = "Action";
        private readonly IList<FieldUsage> _usages;
        private readonly string _entity;
        private readonly ToolTip _tooltip = new ToolTip { AutoPopDelay = 30000 };

        public ImpactDialog(string entity, int fieldCount, IList<FieldUsage> usages, bool readOnly = false, string field = null, bool dryRun = false)
            : base(readOnly ? $"Where {field} is used" : dryRun ? $"Dry run on {entity}" : $"Review the changes on {entity}",
                   readOnly ? $"Table {entity} · read only, nothing is modified. “Will change” shows what Apply would do with the current options."
                   : dryRun ? "Option “Dry run” is on: this is the report of what would be done. Nothing will be changed."
                   : "Nothing has been modified yet. Check every impact below, sorted by priority, then confirm.",
                   readOnly ? Theme.Glyph.Search : Theme.Glyph.Warning,
                   readOnly || dryRun ? "Close" : "Apply changes")
        {
            _usages = usages;
            _entity = entity;
            Size = new Size(Theme.Scale(1200), Theme.Scale(760));
            MinimumSize = new Size(Theme.Scale(860), Theme.Scale(520));

            var data = usages.Where(u => u.Category == UsageManager.DataUsageCategory).ToList();
            var impacts = usages.Where(u => u.Category != UsageManager.DataUsageCategory).ToList();
            var willChange = impacts.Count(u => u.Action == UsageAction.WillChange);
            var kept = impacts.Count(u => u.Action == UsageAction.Kept);
            var high = impacts.Count(u => u.Action != UsageAction.Info && UsageManager.PriorityRank(u) <= 2);
            var places = impacts.Count(u => u.Action != UsageAction.Info);

            var columns = data.Count > 0 ? 4 : 3;
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = columns, RowCount = 2, BackColor = Theme.Canvas };
            for (var c = 0; c < columns; c++)
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / columns));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, Theme.Scale(88)));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Body.Controls.Add(layout);

            layout.Controls.Add(readOnly
                ? new KpiTile { Title = "Places", Value = places.ToString(), Caption = "where the field is used", Glyph = Theme.Glyph.List, Dock = DockStyle.Fill }
                : new KpiTile { Title = "Fields", Value = fieldCount.ToString(), Caption = "selected", Glyph = Theme.Glyph.List, Dock = DockStyle.Fill }, 0, 0);
            layout.Controls.Add(new KpiTile { Title = "Changes", Value = willChange.ToString(), Caption = dryRun ? "would be applied" : "will be applied", Glyph = Theme.Glyph.Rename, Dock = DockStyle.Fill }, 1, 0);
            layout.Controls.Add(new KpiTile
            {
                Title = "To review",
                Value = kept.ToString(),
                Caption = high > 0 ? $"{high} high priority" : "kept, review manually",
                Glyph = Theme.Glyph.Warning,
                Accent = high > 0 ? Theme.Danger : kept > 0 ? Theme.Warning : Theme.Success,
                Dock = DockStyle.Fill
            }, 2, 0);
            if (data.Count > 0)
                layout.Controls.Add(DataTile(data), 3, 0);

            var card = new CardBox { Text = "Impacts", Subtitle = $"{impacts.Count} · sorted by priority: what blocks the deletion or can break first", Dock = DockStyle.Fill };
            layout.Controls.Add(card, 0, 1);
            layout.SetColumnSpan(card, columns);

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            card.Controls.Add(grid);
            GridStyler.Apply(grid, () => "The selected fields are not used anywhere else.", e => PaintPills(grid, e));

            var dt = new DataTable();
            dt.Columns.Add(ColPriority, typeof(string));
            dt.Columns.Add(ColAction, typeof(string));
            dt.Columns.Add("Field", typeof(string));
            dt.Columns.Add("Where", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Details", typeof(string));
            foreach (var u in UsageManager.SortByPriority(impacts))
                dt.Rows.Add(u.Action == UsageAction.Info ? "Info" : UsageManager.PriorityLabel(UsageManager.PriorityRank(u)), u.Action.ToString(), u.SchemaName, u.Category, u.Name, u.Detail);

            grid.DataSource = dt;
            grid.DataBindingComplete += (s, e) =>
            {
                foreach (var name in new[] { ColPriority, ColAction })
                {
                    grid.Columns[name].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    grid.Columns[name].Width = Theme.Scale(name == ColPriority ? 96 : 130);
                }
                grid.Columns["Name"].FillWeight = 150;
                grid.Columns["Details"].FillWeight = 170;
                grid.ClearSelection();
            };

            var export = new FluentButton { Text = "Export report", Primary = false, Glyph = Theme.Glyph.Export, Size = new Size(Theme.Scale(150), Theme.Scale(34)), Margin = new Padding(0, 0, Theme.Scale(8), 0) };
            export.Click += (s, e) => ExportReport();
            Footer.Controls.Add(export);

            if (readOnly || dryRun)
            {
                SecondaryButton.Visible = false;
                PrimaryButton.DialogResult = DialogResult.Cancel;
                CancelButton = PrimaryButton;
                return;
            }

            PrimaryButton.Enabled = willChange > 0;
            PrimaryButton.Text = willChange > 0 ? $"Apply {willChange} change(s)" : "Nothing to apply";
            PrimaryButton.Width = Theme.Scale(190);
        }

        /// <summary>Header tile: data usage of one field (percentage + bar) or of several (fields without data).</summary>
        private KpiTile DataTile(List<FieldUsage> data)
        {
            var tile = new KpiTile { Title = "Data usage", Glyph = Theme.Glyph.Database, Dock = DockStyle.Fill };
            if (data.Count == 1)
            {
                var d = data[0];
                if (d.Filled.HasValue && d.Total.HasValue && d.Total.Value > 0)
                {
                    var ratio = (float)d.Filled.Value / d.Total.Value;
                    tile.Value = $"{ratio * 100:0.#} %";
                    tile.Caption = $"{d.Filled.Value:N0} of {d.Total.Value:N0}";
                    tile.Progress = ratio;
                    tile.Accent = d.Filled.Value == 0 ? Theme.Success : ratio < 0.05f ? Theme.Warning : Theme.Brand;
                }
                else if (d.Filled.HasValue)
                {
                    tile.Value = d.Filled.Value.ToString("N0");
                    tile.Caption = "records with a value";
                    tile.Accent = d.Filled.Value == 0 ? Theme.Success : Theme.Brand;
                }
                else
                {
                    tile.Value = "—";
                    tile.Caption = d.Detail;
                }
                if (d.LastModified.HasValue)
                    tile.Title = $"Data usage · last value {d.LastModified:yyyy-MM-dd}";
                else if (d.Filled == 0)
                    tile.Title = "Data usage · never filled";
            }
            else
            {
                var counted = data.Where(d => d.Filled.HasValue).ToList();
                var empty = counted.Count(d => d.Filled.Value == 0);
                tile.Value = $"{empty}/{data.Count}";
                tile.Caption = "fields never filled";
                tile.Accent = empty == data.Count ? Theme.Success : Theme.Brand;
            }

            _tooltip.SetToolTip(tile, string.Join("\n", data.Select(d => $"{d.SchemaName}: {d.Name}" + (string.IsNullOrEmpty(d.Detail) ? string.Empty : $" · {d.Detail}"))));
            return tile;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _tooltip.Dispose();
            base.Dispose(disposing);
        }

        private void ExportReport()
        {
            using (var dialog = new SaveFileDialog { Filter = "CSV file (*.csv)|*.csv", FileName = $"{_entity}_impacts_{DateTime.Now:yyyyMMdd_HHmm}.csv" })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;
                try
                {
                    var sb = new StringBuilder("Priority;Action;Field;Where;Name;Details\r\n");
                    foreach (var u in UsageManager.SortByPriority(_usages))
                    {
                        var priority = u.Action == UsageAction.Info ? "Info" : UsageManager.PriorityLabel(UsageManager.PriorityRank(u));
                        sb.Append(string.Join(";", priority, u.Action, Csv(u.SchemaName), Csv(u.Category), Csv(u.Name), Csv(u.Detail))).Append("\r\n");
                    }
                    File.WriteAllText(dialog.FileName, sb.ToString(), new UTF8Encoding(true));
                    MessageBox.Show(this, $"{_usages.Count} line(s) exported.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Export failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static string Csv(string value)
        {
            value = value ?? string.Empty;
            return value.IndexOfAny(new[] { ';', '"', '\n', '\r' }) >= 0 ? "\"" + value.Replace("\"", "\"\"") + "\"" : value;
        }

        private static bool PaintPills(DataGridView grid, DataGridViewCellPaintingEventArgs e)
        {
            var column = grid.Columns[e.ColumnIndex].DataPropertyName;
            if ((column != ColAction && column != ColPriority) || e.Value == null)
                return false;

            e.PaintBackground(e.CellBounds, (e.State & DataGridViewElementStates.Selected) != 0);
            string text;
            Color fore, back;
            char glyph;
            switch (e.Value.ToString())
            {
                case nameof(UsageAction.WillChange): text = "Will change"; fore = Theme.Brand; back = Theme.BrandSubtle; glyph = Theme.Glyph.Rename; break;
                case nameof(UsageAction.Kept): text = "Kept"; fore = Theme.Warning; back = Theme.WarningSubtle; glyph = Theme.Glyph.Warning; break;
                case "High": text = "High"; fore = Theme.Danger; back = Theme.DangerSubtle; glyph = Theme.Glyph.Flag; break;
                case "Medium": text = "Medium"; fore = Theme.Warning; back = Theme.WarningSubtle; glyph = Theme.Glyph.Flag; break;
                case "Low": text = "Low"; fore = Theme.TextSecondary; back = Theme.StrokeSubtle; glyph = Theme.Glyph.Flag; break;
                default: text = "Info"; fore = Theme.TextSecondary; back = Theme.StrokeSubtle; glyph = Theme.Glyph.Info; break;
            }

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var size = TextRenderer.MeasureText(g, text, Theme.Caption, Size.Empty, TextFormatFlags.NoPadding);
            var pill = new Rectangle(e.CellBounds.X + Theme.Scale(8), e.CellBounds.Y + (e.CellBounds.Height - Theme.Scale(22)) / 2, size.Width + Theme.Scale(36), Theme.Scale(22));
            Theme.FillRounded(g, back, pill, pill.Height / 2f);
            Theme.DrawGlyph(g, glyph, fore, new RectangleF(pill.X + Theme.Scale(6), pill.Y, Theme.Scale(14), pill.Height), Theme.Scale(11));
            TextRenderer.DrawText(g, text, Theme.Caption, new Rectangle(pill.X + Theme.Scale(24), pill.Y, pill.Width - Theme.Scale(26), pill.Height), fore, Draw.LeftMiddle);
            g.SmoothingMode = SmoothingMode.Default;
            e.Handled = true;
            return true;
        }
    }
}
