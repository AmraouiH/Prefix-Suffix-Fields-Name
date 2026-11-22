using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Prefix_Suffix_Fields_Name.Manager;
using Prefix_Suffix_Fields_Name.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace Prefix_Suffix_Fields_Name
{
    public partial class MyPluginControl : PluginControlBase
    {
        // Column names (also used as DataTable column names)
        private const string ColDisplayName = "Display Name";
        private const string ColSchemaName = "Schema Name";
        private const string ColType = "Type";
        private const string ColSearchable = "Searchable";
        private const string ColNewDisplayName = "New Display Name";
        private const string ColStatus = "Status";
        private const string ColMessage = "Message";

        private const string StatusUpdated = "Updated";
        private const string StatusSkipped = "Skipped";
        private const string StatusFailed = "Failed";
        private const string NoChange = "(no change)";
        private const string ColUsage = "Usage";
        private int hoverUsageRow = -1;

        private Settings mySettings;
        private DataTable dtEntities;
        private DataTable dtFields;
        private readonly DataTable dtSelected;
        private readonly Dictionary<string, Field> fieldsBySchema = new Dictionary<string, Field>();
        private string currentEntity;
        private string currentEntityDisplayName;
        private int languageCode;
        private bool hasResults;

        public MyPluginControl()
        {
            InitializeComponent();
            ApplyInterfaceTheme();

            dtSelected = CreateFieldsTable(withPreview: true);
            fieldsUpdateDataGridView.DataSource = dtSelected;
            ConfigureColumns(fieldsUpdateDataGridView);
            AddUsageColumn();
        }

        #region Interface Theme

        private void ApplyInterfaceTheme()
        {
            BackColor = Theme.Canvas;
            ForeColor = Theme.TextPrimary;
            rootLayout.BackColor = Theme.Canvas;
            rightLayout.BackColor = Theme.Canvas;

            #region Command bar
            toolStripMenu.Renderer = new FluentToolStripRenderer();
            toolStripMenu.BackColor = Theme.Surface;
            foreach (ToolStripItem item in toolStripMenu.Items)
            {
                item.Font = Theme.Body;
                item.ForeColor = Theme.TextPrimary;
            }

            tsbClose.Image = Theme.Icon(Theme.Glyph.Close, Theme.TextSecondary);
            loadEntitiesButton.Image = Theme.Icon(Theme.Glyph.List, Theme.Brand);
            loadFieldsButton.Image = Theme.Icon(Theme.Glyph.Refresh, Theme.Brand);
            exportButton.Image = Theme.Icon(Theme.Glyph.Export, Color.FromArgb(16, 124, 65));
            byButton.Image = Theme.Icon(Theme.Glyph.Contact, Theme.TextSecondary);
            helpButton.Image = Theme.Icon(Theme.Glyph.Help, Theme.TextSecondary);
            optionsButton.Image = Theme.Icon(Theme.Glyph.Personalize, Theme.TextSecondary);
            #endregion

            #region Search boxes
            entityTextSearch.Glyph = Theme.Glyph.Search;
            fieldsTextSearch.Glyph = Theme.Glyph.Search;
            affixTextBox.Glyph = Theme.Glyph.Tag;
            foreach (var box in new[] { entityTextSearch, fieldsTextSearch, affixTextBox })
                box.Font = Theme.Body;
            #endregion

            #region Rename card
            foreach (var label in new[] { positionLabel, actionLabel, textLabel })
            {
                label.Font = Theme.BodyStrong;
                label.ForeColor = Theme.TextSecondary;
                label.BackColor = Theme.Surface;
            }
            positionChips.SetChips("Prefix", "Suffix");
            actionChips.SetChips("Add", "Remove");
            positionChips.BackColor = actionChips.BackColor = Theme.Surface;
            cleanupToggle.Font = Theme.Body;
            cleanupToggle.BackColor = Theme.Surface;

            addFieldsButton.Glyph = Theme.Glyph.Add;
            addAllFilteredButton.Glyph = Theme.Glyph.Forward;
            removeFieldsButton.Glyph = Theme.Glyph.Remove;
            clearFieldsButton.Glyph = Theme.Glyph.Delete;
            applyButton.Glyph = Theme.Glyph.Rename;
            #endregion

            #region Grids
            GridStyler.Apply(entityDataGridView,
                () => dtEntities == null ? "Click “Load Tables” to list the tables of your environment." : "No table matches your search.",
                PaintEntityCell);
            entityDataGridView.RowTemplate.Height = Theme.Scale(46);
            entityDataGridView.ColumnHeadersVisible = false;
            entityDataGridView.DataBindingComplete += (s, e) => entityDataGridView.ClearSelection();

            GridStyler.Apply(fieldsDataGridView,
                () => dtFields == null ? "Click a table to list its renamable fields." : "No field matches your search.");
            fieldsDataGridView.DataBindingComplete += (s, e) => fieldsDataGridView.ClearSelection();

            GridStyler.Apply(fieldsUpdateDataGridView,
                () => dtFields == null ? "Selected fields and their new display name will appear here." : "Double-click fields, or select them and click “Add selected”.",
                PaintUsageButton);

            GridStyler.Apply(updateFieldsResultDataGridView,
                () => "The status of each field will appear here after “Apply”.",
                PaintStatusCell);
            #endregion

            UpdateHeader();
        }

        /// <summary>Tables grid: two line "display name / schema name" cell.</summary>
        private bool PaintEntityCell(DataGridViewCellPaintingEventArgs e)
        {
            var column = entityDataGridView.Columns[e.ColumnIndex];
            if (column.DataPropertyName != ColDisplayName)
                return false;

            var selected = (e.State & DataGridViewElementStates.Selected) != 0;
            var row = entityDataGridView.Rows[e.RowIndex];
            var schema = Convert.ToString(row.Cells[ColSchemaName].Value);
            var isCurrent = schema == currentEntity;
            var bounds = e.CellBounds;

            e.PaintBackground(bounds, selected);
            if (isCurrent)
            {
                using (var brush = new SolidBrush(Theme.BrandSelected))
                    e.Graphics.FillRectangle(brush, bounds.X, bounds.Y, bounds.Width, bounds.Height - 1);
                using (var brush = new SolidBrush(Theme.Brand))
                    e.Graphics.FillRectangle(brush, bounds.X, bounds.Y + Theme.Scale(8), Theme.Scale(3), bounds.Height - Theme.Scale(16));
            }

            var textBounds = new Rectangle(bounds.X + Theme.Scale(12), bounds.Y, bounds.Width - Theme.Scale(16), bounds.Height);
            var half = bounds.Height / 2;
            TextRenderer.DrawText(e.Graphics, Convert.ToString(e.Value), Theme.BodyStrong,
                new Rectangle(textBounds.X, textBounds.Y + Theme.Scale(4), textBounds.Width, half - Theme.Scale(3)), Theme.TextPrimary, Draw.LeftMiddle);
            TextRenderer.DrawText(e.Graphics, schema, Theme.Caption,
                new Rectangle(textBounds.X, textBounds.Y + half - Theme.Scale(1), textBounds.Width, half - Theme.Scale(4)), Theme.TextTertiary, Draw.LeftMiddle);
            e.Handled = true;
            return true;
        }

        /// <summary>Result grid: status drawn as a colored pill.</summary>
        private bool PaintStatusCell(DataGridViewCellPaintingEventArgs e)
        {
            var grid = updateFieldsResultDataGridView;
            if (grid.Columns[e.ColumnIndex].DataPropertyName != ColStatus || e.Value == null)
                return false;

            var selected = (e.State & DataGridViewElementStates.Selected) != 0;
            e.PaintBackground(e.CellBounds, selected);

            var status = e.Value.ToString();
            Color fore, back;
            char glyph;
            switch (status)
            {
                case StatusUpdated: fore = Theme.Success; back = Theme.SuccessSubtle; glyph = Theme.Glyph.Completed; break;
                case StatusSkipped: fore = Theme.TextSecondary; back = Theme.StrokeSubtle; glyph = Theme.Glyph.Info; break;
                default: fore = Theme.Danger; back = Theme.DangerSubtle; glyph = Theme.Glyph.Error; break;
            }

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var textSize = TextRenderer.MeasureText(g, status, Theme.Caption, Size.Empty, TextFormatFlags.NoPadding);
            var pill = new Rectangle(e.CellBounds.X + Theme.Scale(8), e.CellBounds.Y + (e.CellBounds.Height - Theme.Scale(22)) / 2,
                textSize.Width + Theme.Scale(36), Theme.Scale(22));
            Theme.FillRounded(g, back, pill, pill.Height / 2f);
            Theme.DrawGlyph(g, glyph, fore, new RectangleF(pill.X + Theme.Scale(6), pill.Y, Theme.Scale(14), pill.Height), Theme.Scale(11));
            TextRenderer.DrawText(g, status, Theme.Caption, new Rectangle(pill.X + Theme.Scale(24), pill.Y, pill.Width - Theme.Scale(26), pill.Height), fore, Draw.LeftMiddle);
            g.SmoothingMode = SmoothingMode.Default;
            e.Handled = true;
            return true;
        }

        private void UpdateHeader()
        {
            if (currentEntity == null)
            {
                contextHeader.Title = "No table selected";
                contextHeader.Subtitle = dtEntities == null ? "Load the tables, then click one to list its fields" : "Click a table on the left to list its fields";
                contextHeader.SetBadges();
                contextHeader.Hint = string.Empty;
                return;
            }

            contextHeader.Title = currentEntityDisplayName ?? currentEntity;
            contextHeader.Subtitle = currentEntity;
            contextHeader.SetBadges(
                dtFields != null ? $"{dtFields.Rows.Count} fields" : null,
                dtSelected.Rows.Count > 0 ? $"{dtSelected.Rows.Count} selected" : null);
            contextHeader.Hint = dtSelected.Rows.Count > 0 && !hasResults ? "Export a CSV backup before applying" : string.Empty;
        }

        #endregion

        #region XrmToolBox lifecycle

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();
                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }

            affixTextBox.Text = mySettings.LastText ?? string.Empty;
            positionChips.SelectedIndex = mySettings.LastIsSuffix ? 1 : 0;
            actionChips.SelectedIndex = mySettings.LastIsRemove ? 1 : 0;
            cleanupToggle.Checked = mySettings.CleanupEnabled;
            UpdateCleanupToggle();

            UpdateActionState();
        }

        public override void ClosingPlugin(PluginCloseInfo info)
        {
            SaveSettings();
            base.ClosingPlugin(info);
        }

        private void SaveSettings()
        {
            if (mySettings == null)
                return;
            mySettings.LastText = affixTextBox.Text;
            mySettings.LastIsSuffix = Position == AffixPosition.Suffix;
            mySettings.LastIsRemove = Action == AffixAction.Remove;
            mySettings.CleanupEnabled = cleanupToggle.Checked;
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }

            // Metadata of the previous organization is no longer valid.
            dtEntities = null;
            entityDataGridView.DataSource = null;
            entitiesCard.Subtitle = "load them to start";
            ResetAll();
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        #endregion

        #region Load tables

        private void loadEntitiesButton_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadEntities);
        }

        private void LoadEntities()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving tables...",
                Work = (worker, args) =>
                {
                    var lcid = Service.RetrieveMultiple(new QueryExpression("organization")
                    {
                        ColumnSet = new ColumnSet("languagecode")
                    }).Entities.First().GetAttributeValue<int>("languagecode");

                    var metadata = PrefixSuffixFieldsNameManager.GetEntitiesMetadata(Service);

                    var dt = new DataTable();
                    dt.Columns.Add(ColDisplayName, typeof(string));
                    dt.Columns.Add(ColSchemaName, typeof(string));
                    foreach (var item in metadata)
                    {
                        dt.Rows.Add(PrefixSuffixFieldsNameManager.GetLabel(item.DisplayName) ?? item.LogicalName, item.LogicalName);
                    }

                    args.Result = new Tuple<int, DataTable>(lcid, dt);
                },
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        ShowError("Unable to retrieve tables", args.Error);
                        return;
                    }

                    var result = (Tuple<int, DataTable>)args.Result;
                    ResetAll();
                    languageCode = result.Item1;
                    dtEntities = result.Item2;
                    dtEntities.DefaultView.Sort = $"[{ColDisplayName}] ASC";
                    entityDataGridView.DataSource = dtEntities.DefaultView;
                    entityDataGridView.Columns[ColSchemaName].Visible = false;
                    entityDataGridView.Columns[ColDisplayName].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    entityTextSearch.Enabled = true;
                    UpdateEntitiesSubtitle();
                    UpdateHeader();
                    UpdateActionState();
                    entityTextSearch.Focus();
                }
            });
        }

        private void UpdateEntitiesSubtitle()
        {
            if (dtEntities == null)
                return;
            var shown = dtEntities.DefaultView.Count;
            entitiesCard.Subtitle = shown == dtEntities.Rows.Count
                ? $"{shown} · click to list the fields"
                : $"{shown} of {dtEntities.Rows.Count}";
        }

        #endregion

        #region Load fields

        private void entityDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                LoadFieldsForRow(entityDataGridView.Rows[e.RowIndex], force: false);
        }

        private void entityDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && entityDataGridView.CurrentRow != null)
            {
                e.Handled = true;
                LoadFieldsForRow(entityDataGridView.CurrentRow, force: false);
            }
        }

        private void loadFieldsButton_Click(object sender, EventArgs e)
        {
            if (currentEntity != null)
                ExecuteMethod(LoadFields);
            else if (entityDataGridView.CurrentRow != null)
                LoadFieldsForRow(entityDataGridView.CurrentRow, force: true);
            else
                MessageBox.Show("Please select a table first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadFieldsForRow(DataGridViewRow row, bool force)
        {
            var schema = row.Cells[ColSchemaName].Value as string;
            if (string.IsNullOrEmpty(schema) || (!force && schema == currentEntity))
                return;

            if (dtSelected.Rows.Count > 0 && schema != currentEntity)
            {
                var answer = MessageBox.Show(
                    $"You have {dtSelected.Rows.Count} selected field(s) on '{currentEntity}'. Switching table will clear this selection. Continue?",
                    "Change table", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer != DialogResult.Yes)
                    return;
                dtSelected.Rows.Clear();
            }

            currentEntity = schema;
            currentEntityDisplayName = row.Cells[ColDisplayName].Value as string;
            hasResults = false;
            updateFieldsResultDataGridView.DataSource = null;
            resultCard.Subtitle = "status of the last update";
            entityDataGridView.Invalidate();
            ExecuteMethod(LoadFields);
        }

        private void LoadFields()
        {
            var entity = currentEntity;
            var keepSelection = dtSelected.Rows.Cast<DataRow>().Select(r => (string)r[ColSchemaName]).ToList();

            WorkAsync(new WorkAsyncInfo
            {
                Message = $"Retrieving fields of {entity}...",
                Work = (worker, args) =>
                {
                    args.Result = PrefixSuffixFieldsNameManager.GetEntityFields(Service, entity, worker);
                },
                ProgressChanged = e => SetWorkingMessage(e.UserState.ToString()),
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        ShowError($"Unable to retrieve the fields of '{entity}'", args.Error);
                        return;
                    }

                    var fields = (List<Field>)args.Result;
                    fieldsBySchema.Clear();
                    foreach (var f in fields)
                        fieldsBySchema[f.SchemaName] = f;

                    dtFields = CreateFieldsTable(withPreview: false);
                    foreach (var f in fields)
                        dtFields.Rows.Add(f.DisplayName, f.SchemaName, f.Type, f.IsSearchable);
                    dtFields.DefaultView.Sort = $"[{ColDisplayName}] ASC";

                    fieldsDataGridView.DataSource = dtFields.DefaultView;
                    ConfigureColumns(fieldsDataGridView);
                    fieldsTextSearch.Text = string.Empty;
                    fieldsTextSearch.Enabled = true;
                    UpdateFieldsSubtitle();

                    // Rebuild the selection with fresh metadata (same table only).
                    dtSelected.Rows.Clear();
                    foreach (var schema in keepSelection.Where(fieldsBySchema.ContainsKey))
                        AddToSelection(fieldsBySchema[schema]);

                    UpdateActionState();
                }
            });
        }

        private void UpdateFieldsSubtitle()
        {
            if (dtFields == null)
            {
                fieldsCard.Subtitle = "click a table";
                return;
            }
            var shown = dtFields.DefaultView.Count;
            fieldsCard.Subtitle = (shown == dtFields.Rows.Count ? $"{shown} renamable" : $"{shown} of {dtFields.Rows.Count}")
                + " · double-click to select";
        }

        #endregion

        #region Selection

        private void fieldsDataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
                AddRowsToSelection(new[] { fieldsDataGridView.Rows[e.RowIndex] });
        }

        private void fieldsDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                AddRowsToSelection(fieldsDataGridView.SelectedRows.Cast<DataGridViewRow>());
            }
        }

        private void addFieldsButton_Click(object sender, EventArgs e)
        {
            var rows = fieldsDataGridView.SelectedRows.Cast<DataGridViewRow>().ToList();
            if (rows.Count == 0)
            {
                MessageBox.Show("Select one or more fields in the list first (Ctrl/Shift + click for multiple).", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            AddRowsToSelection(rows);
        }

        private void addAllFilteredButton_Click(object sender, EventArgs e)
        {
            AddRowsToSelection(fieldsDataGridView.Rows.Cast<DataGridViewRow>());
        }

        private void AddRowsToSelection(IEnumerable<DataGridViewRow> rows)
        {
            foreach (var row in rows)
            {
                var schema = row.Cells[ColSchemaName].Value as string;
                if (schema != null && fieldsBySchema.TryGetValue(schema, out var field))
                    AddToSelection(field);
            }
            UpdateActionState();
        }

        private void AddToSelection(Field field)
        {
            if (dtSelected.Rows.Cast<DataRow>().Any(r => (string)r[ColSchemaName] == field.SchemaName))
                return;
            dtSelected.Rows.Add(field.DisplayName, field.SchemaName, field.Type, field.IsSearchable, string.Empty);
        }

        private void fieldsUpdateDataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && fieldsUpdateDataGridView.Columns[e.ColumnIndex].Name != ColUsage)
                RemoveFromSelection(new[] { fieldsUpdateDataGridView.Rows[e.RowIndex] });
        }

        private void fieldsUpdateDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
                RemoveFromSelection(fieldsUpdateDataGridView.SelectedRows.Cast<DataGridViewRow>());
            }
        }

        private void removeFieldsButton_Click(object sender, EventArgs e)
        {
            RemoveFromSelection(fieldsUpdateDataGridView.SelectedRows.Cast<DataGridViewRow>());
        }

        private void clearFieldsButton_Click(object sender, EventArgs e)
        {
            dtSelected.Rows.Clear();
            UpdateActionState();
        }

        private void RemoveFromSelection(IEnumerable<DataGridViewRow> rows)
        {
            foreach (var view in rows.Select(r => r.DataBoundItem).OfType<DataRowView>().ToList())
                view.Row.Delete();
            dtSelected.AcceptChanges();
            UpdateActionState();
        }

        #endregion

        #region Options

        private CleanupOptions ConfiguredCleanup => mySettings?.Options ?? CleanupOptions.None;

        /// <summary>
        /// Options of the next run: clean up actions only when the toggle is on; safety and analysis settings always.
        /// </summary>
        private CleanupOptions ActiveCleanup => ConfiguredCleanup.ForRun(cleanupToggle.Checked);

        private void optionsButton_Click(object sender, EventArgs e)
        {
            if (Service == null)
            {
                ShowOptions(new List<KeyValuePair<string, string>>());
                return;
            }

            // List the unmanaged solutions first, for the "Add to a solution" option.
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading solutions...",
                Work = (worker, args) =>
                {
                    var query = new QueryExpression("solution")
                    {
                        ColumnSet = new ColumnSet("uniquename", "friendlyname"),
                        Criteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression("ismanaged", ConditionOperator.Equal, false),
                                new ConditionExpression("isvisible", ConditionOperator.Equal, true),
                                new ConditionExpression("uniquename", ConditionOperator.NotEqual, "Default")
                            }
                        },
                        Orders = { new OrderExpression("friendlyname", OrderType.Ascending) }
                    };
                    args.Result = Service.RetrieveMultiple(query).Entities
                        .Select(s => new KeyValuePair<string, string>(s.GetAttributeValue<string>("uniquename"), s.GetAttributeValue<string>("friendlyname")))
                        .ToList();
                },
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                        LogWarning("Unable to list solutions: {0}", args.Error.Message);
                    ShowOptions(args.Error == null ? (List<KeyValuePair<string, string>>)args.Result : new List<KeyValuePair<string, string>>());
                }
            });
        }

        private void ShowOptions(List<KeyValuePair<string, string>> solutions)
        {
            using (var dialog = new OptionsDialog(ConfiguredCleanup, solutions))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                mySettings.Options = dialog.Options;
                if (mySettings.Options.Any && !cleanupToggle.Checked)
                    cleanupToggle.Checked = true;
                SaveSettings();
            }
            UpdateCleanupToggle();
            UpdateActionState();
        }

        private void UpdateCleanupToggle()
        {
            var options = ConfiguredCleanup;
            if (!options.Any)
            {
                cleanupToggle.Checked = false;
                cleanupToggle.Enabled = false;
                cleanupToggle.Text = "Options (none enabled, click Options in the toolbar)";
            }
            else
            {
                cleanupToggle.Enabled = true;
                cleanupToggle.Text = "Options";
            }
            cleanupToggle.Width = cleanupToggle.GetPreferredSize(Size.Empty).Width;
        }

        #endregion

        #region Usage of one field

        /// <summary>"Usage" button in front of each selected field.</summary>
        private void AddUsageColumn()
        {
            var column = new DataGridViewTextBoxColumn
            {
                Name = ColUsage,
                HeaderText = string.Empty,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = Theme.Scale(96),
                ToolTipText = "Show where this field is used"
            };
            fieldsUpdateDataGridView.Columns.Insert(0, column);
            column.DisplayIndex = 0;

            fieldsUpdateDataGridView.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && fieldsUpdateDataGridView.Columns[e.ColumnIndex].Name == ColUsage)
                    ShowUsage((string)fieldsUpdateDataGridView.Rows[e.RowIndex].Cells[ColSchemaName].Value);
            };
            fieldsUpdateDataGridView.CellMouseEnter += (s, e) =>
            {
                var onButton = e.RowIndex >= 0 && e.ColumnIndex >= 0 && fieldsUpdateDataGridView.Columns[e.ColumnIndex].Name == ColUsage;
                fieldsUpdateDataGridView.Cursor = onButton ? Cursors.Hand : Cursors.Default;
                var row = onButton ? e.RowIndex : -1;
                if (row != hoverUsageRow) { hoverUsageRow = row; fieldsUpdateDataGridView.Invalidate(); }
            };
            fieldsUpdateDataGridView.MouseLeave += (s, e) =>
            {
                fieldsUpdateDataGridView.Cursor = Cursors.Default;
                if (hoverUsageRow != -1) { hoverUsageRow = -1; fieldsUpdateDataGridView.Invalidate(); }
            };
        }

        private bool PaintUsageButton(DataGridViewCellPaintingEventArgs e)
        {
            if (fieldsUpdateDataGridView.Columns[e.ColumnIndex].Name != ColUsage)
                return false;

            e.PaintBackground(e.CellBounds, (e.State & DataGridViewElementStates.Selected) != 0);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var hover = e.RowIndex == hoverUsageRow;
            var r = new Rectangle(e.CellBounds.X + Theme.Scale(6), e.CellBounds.Y + (e.CellBounds.Height - Theme.Scale(24)) / 2, e.CellBounds.Width - Theme.Scale(12), Theme.Scale(24));
            Theme.FillRounded(g, hover ? Theme.Brand : Theme.BrandSubtle, r, Theme.Scale(4));
            var color = hover ? Theme.TextOnBrand : Theme.Brand;
            Theme.DrawGlyph(g, Theme.Glyph.Search, color, new RectangleF(r.X + Theme.Scale(6), r.Y, Theme.Scale(14), r.Height), Theme.Scale(11));
            TextRenderer.DrawText(g, "Usage", Theme.Caption, new Rectangle(r.X + Theme.Scale(24), r.Y, r.Width - Theme.Scale(26), r.Height), color, Draw.LeftMiddle);
            g.SmoothingMode = SmoothingMode.Default;
            e.Handled = true;
            return true;
        }

        /// <summary>Analyses one field and shows every place it is used (read only).</summary>
        private void ShowUsage(string schema)
        {
            if (schema == null || !fieldsBySchema.TryGetValue(schema, out var field))
                return;

            var entity = currentEntity;
            var text = affixTextBox.Text;
            var position = Position;
            var action = Action;
            var cleanup = ActiveCleanup;
            var lcid = languageCode;
            var fields = new List<Field> { field };

            ExecuteMethod(() => WorkAsync(new WorkAsyncInfo
            {
                Message = $"Analysing where {schema} is used...",
                Work = (worker, args) =>
                {
                    var names = PrefixSuffixFieldsNameManager.PreviewDisplayNames(fields, text, position, action, lcid);
                    args.Result = UsageManager.Analyze(Service, entity, fields, names, cleanup, worker);
                },
                ProgressChanged = e => SetWorkingMessage(e.UserState.ToString()),
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        ShowError($"Unable to analyse {schema}", args.Error);
                        return;
                    }
                    using (var dialog = new ImpactDialog(currentEntityDisplayName ?? entity, 1, (List<FieldUsage>)args.Result, readOnly: true, field: $"{field.DisplayName} ({schema})"))
                        dialog.ShowDialog(this);
                }
            }));
        }

        #endregion

        #region Preview & apply

        private void RenameOptions_Changed(object sender, EventArgs e)
        {
            UpdateActionState();
        }

        private AffixPosition Position => positionChips.SelectedIndex == 1 ? AffixPosition.Suffix : AffixPosition.Prefix;
        private AffixAction Action => actionChips.SelectedIndex == 1 ? AffixAction.Remove : AffixAction.Add;

        /// <summary>
        /// Refreshes the preview column, counters, header and buttons state.
        /// </summary>
        private void UpdateActionState()
        {
            if (dtSelected == null)
                return;

            int changes = 0;
            foreach (DataRow row in dtSelected.Rows)
            {
                var newLabel = PrefixSuffixFieldsNameManager.ComputeNewLabel((string)row[ColDisplayName], affixTextBox.Text, Position, Action);
                row[ColNewDisplayName] = newLabel ?? NoChange;
                if (newLabel != null || ActiveCleanup.Any)
                    changes++;
            }

            selectedCard.Subtitle = dtSelected.Rows.Count == 0
                ? "preview of the new display names"
                : $"{dtSelected.Rows.Count} · {changes} will change";
            applyButton.Enabled = currentEntity != null && changes > 0;
            var verb = ConfiguredCleanup.DryRun ? "Dry run" : "Apply to";
            applyButton.Text = changes > 0 ? $"{verb} {changes}" : (ConfiguredCleanup.DryRun ? "Dry run" : "Apply");
            addFieldsButton.Enabled = addAllFilteredButton.Enabled = dtFields != null;
            removeFieldsButton.Enabled = clearFieldsButton.Enabled = dtSelected.Rows.Count > 0;
            exportButton.Enabled = dtFields != null;
            loadFieldsButton.Enabled = dtEntities != null;
            fieldsUpdateDataGridView.Invalidate();
            UpdateHeader();
        }

        private void fieldsUpdateDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var grid = (DataGridView)sender;
            if (grid.Columns[e.ColumnIndex].Name != ColNewDisplayName)
                return;

            if (e.Value as string == NoChange)
            {
                e.CellStyle.ForeColor = Theme.TextTertiary;
                e.CellStyle.SelectionForeColor = Theme.TextTertiary;
            }
            else
            {
                e.CellStyle.ForeColor = Theme.Brand;
                e.CellStyle.SelectionForeColor = Theme.Brand;
                e.CellStyle.Font = Theme.BodyStrong;
            }
        }

        private void ProceedButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(affixTextBox.Text) && !ActiveCleanup.Any)
            {
                MessageBox.Show("Please enter the text to add or remove, or switch on the clean up.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            SaveSettings();
            ExecuteMethod(AnalyzeThenApply);
        }

        /// <summary>
        /// Analyses where the fields are used, shows every impact and applies only after the user confirms.
        /// </summary>
        private void AnalyzeThenApply()
        {
            var entity = currentEntity;
            var text = affixTextBox.Text;
            var position = Position;
            var action = Action;
            var cleanup = ActiveCleanup;
            var lcid = languageCode;
            var fields = dtSelected.Rows.Cast<DataRow>()
                .Select(r => fieldsBySchema[(string)r[ColSchemaName]])
                .ToList();

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Analysing where the fields are used...",
                Work = (worker, args) =>
                {
                    var names = PrefixSuffixFieldsNameManager.PreviewDisplayNames(fields, text, position, action, lcid);
                    args.Result = UsageManager.Analyze(Service, entity, fields, names, cleanup, worker);
                },
                ProgressChanged = e => SetWorkingMessage(e.UserState.ToString()),
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        ShowError("The impact analysis failed", args.Error);
                        return;
                    }

                    var usages = (List<FieldUsage>)args.Result;
                    using (var dialog = new ImpactDialog(currentEntityDisplayName ?? entity, fields.Count, usages, dryRun: cleanup.DryRun))
                    {
                        if (dialog.ShowDialog(this) != DialogResult.OK || cleanup.DryRun)
                            return;
                    }

                    ApplyChanges(entity, fields, text, position, action, cleanup, lcid);
                }
            });
        }

        private void ApplyChanges(string entity, List<Field> fields, string text, AffixPosition position, AffixAction action, CleanupOptions cleanup, int lcid)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Updating fields...",
                Work = (worker, args) =>
                {
                    args.Result = PrefixSuffixFieldsNameManager.UpdateAttributes(
                        Service, entity, fields, text, position, action, cleanup, lcid, worker);
                },
                ProgressChanged = e => SetWorkingMessage(e.UserState.ToString()),
                PostWorkCallBack = args =>
                {
                    if (args.Error != null)
                    {
                        ShowError("The update failed", args.Error);
                        return;
                    }

                    var results = (List<UpdateResult>)args.Result;
                    ShowResults(results);

                    var ok = results.Count(r => r.Status == ChangeStatus.Updated);
                    var failed = results.Count(r => r.Status == ChangeStatus.Failed);
                    var skipped = results.Count(r => r.Status == ChangeStatus.Skipped);
                    LogInfo("Update on {0}: {1} updated, {2} skipped, {3} failed", entity, ok, skipped, failed);

                    // Keep failed fields selected so the user can retry, drop the others.
                    var failedSchemas = new HashSet<string>(results.Where(r => r.Status == ChangeStatus.Failed).Select(r => r.SchemaName));
                    foreach (var row in dtSelected.Rows.Cast<DataRow>().Where(r => !failedSchemas.Contains((string)r[ColSchemaName])).ToList())
                        row.Delete();
                    dtSelected.AcceptChanges();

                    ExecuteMethod(LoadFields);

                    MessageBox.Show(
                        $"{ok} change(s) applied and published\n{skipped} skipped (nothing to change)\n{failed} failed" +
                        (failed > 0 ? "\n\nSee the 'Message' column for details." : string.Empty),
                        "Update finished", MessageBoxButtons.OK, failed > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
                }
            });
        }

        private void ShowResults(List<UpdateResult> results)
        {
            var dt = new DataTable();
            dt.Columns.Add(ColStatus, typeof(string));
            dt.Columns.Add("Field", typeof(string));
            dt.Columns.Add("Operation", typeof(string));
            dt.Columns.Add(ColMessage, typeof(string));

            foreach (var r in results.OrderBy(r => r.SchemaName).ThenBy(r => r.Operation))
                dt.Rows.Add(r.Status.ToString(), r.SchemaName, r.Operation, r.Message);

            hasResults = true;
            updateFieldsResultDataGridView.DataSource = dt;
            updateFieldsResultDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            updateFieldsResultDataGridView.Columns[ColStatus].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            updateFieldsResultDataGridView.Columns[ColStatus].Width = Theme.Scale(110);
            updateFieldsResultDataGridView.Columns["Operation"].FillWeight = 120;
            updateFieldsResultDataGridView.Columns[ColMessage].FillWeight = 180;
            updateFieldsResultDataGridView.ClearSelection();
            resultCard.Subtitle = $"{results.Count(r => r.Status == ChangeStatus.Updated)} applied · {results.Count(r => r.Status == ChangeStatus.Skipped)} skipped · {results.Count(r => r.Status == ChangeStatus.Failed)} failed";
            UpdateHeader();
        }

        #endregion

        #region Search

        private void entityTextSearch_TextChanged(object sender, EventArgs e)
        {
            if (dtEntities == null)
                return;
            dtEntities.DefaultView.RowFilter = BuildFilter(entityTextSearch.Text);
            UpdateEntitiesSubtitle();
        }

        private void fieldsTextSearch_TextChanged(object sender, EventArgs e)
        {
            if (dtFields == null)
                return;
            dtFields.DefaultView.RowFilter = BuildFilter(fieldsTextSearch.Text);
            UpdateFieldsSubtitle();
        }

        private static string BuildFilter(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return string.Empty;
            var value = EscapeLike(search.Trim());
            return $"[{ColDisplayName}] LIKE '%{value}%' OR [{ColSchemaName}] LIKE '%{value}%'";
        }

        /// <summary>
        /// Escapes a value for a DataView LIKE expression.
        /// </summary>
        private static string EscapeLike(string value)
        {
            var sb = new StringBuilder(value.Length);
            foreach (var c in value)
            {
                switch (c)
                {
                    case '*':
                    case '%':
                    case '[':
                    case ']':
                        sb.Append('[').Append(c).Append(']');
                        break;
                    case '\'':
                        sb.Append("''");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }

        #endregion

        #region Export

        private void exportButton_Click(object sender, EventArgs e)
        {
            if (dtFields == null)
                return;

            using (var dialog = new SaveFileDialog
            {
                Filter = "CSV file (*.csv)|*.csv",
                FileName = $"{currentEntity}_labels_{DateTime.Now:yyyyMMdd_HHmm}.csv"
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Table;Schema Name;Type;Searchable;Language;Display Name");
                    foreach (var field in fieldsBySchema.Values.OrderBy(f => f.SchemaName))
                    {
                        var labels = field.DisplayNameLabel?.LocalizedLabels;
                        if (labels == null || labels.Count == 0)
                        {
                            sb.AppendLine(string.Join(";", Csv(currentEntity), Csv(field.SchemaName), Csv(field.Type), field.IsSearchable, "", ""));
                            continue;
                        }
                        foreach (var l in labels)
                            sb.AppendLine(string.Join(";", Csv(currentEntity), Csv(field.SchemaName), Csv(field.Type), field.IsSearchable, l.LanguageCode, Csv(l.Label)));
                    }
                    File.WriteAllText(dialog.FileName, sb.ToString(), new UTF8Encoding(true));
                    LogInfo("Labels exported to {0}", dialog.FileName);
                    MessageBox.Show($"{fieldsBySchema.Count} field(s) exported.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    ShowError("Export failed", ex);
                }
            }
        }

        private static string Csv(string value)
        {
            value = value ?? string.Empty;
            return value.IndexOfAny(new[] { ';', '"', '\n', '\r' }) >= 0 ? "\"" + value.Replace("\"", "\"\"") + "\"" : value;
        }

        #endregion

        #region Helpers

        private void ResetAll()
        {
            currentEntity = null;
            currentEntityDisplayName = null;
            hasResults = false;
            fieldsBySchema.Clear();
            dtFields = null;
            fieldsDataGridView.DataSource = null;
            updateFieldsResultDataGridView.DataSource = null;
            dtSelected?.Rows.Clear();
            fieldsTextSearch.Text = string.Empty;
            fieldsTextSearch.Enabled = false;
            entityTextSearch.Text = string.Empty;
            UpdateFieldsSubtitle();
            resultCard.Subtitle = "status of the last update";
            UpdateActionState();
            UpdateHeader();
        }

        private static DataTable CreateFieldsTable(bool withPreview)
        {
            var dt = new DataTable();
            dt.Columns.Add(ColDisplayName, typeof(string));
            dt.Columns.Add(ColSchemaName, typeof(string));
            dt.Columns.Add(ColType, typeof(string));
            dt.Columns.Add(ColSearchable, typeof(bool));
            if (withPreview)
                dt.Columns.Add(ColNewDisplayName, typeof(string));
            return dt;
        }

        private static void ConfigureColumns(DataGridView grid)
        {
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            if (grid.Columns.Contains(ColType))
                grid.Columns[ColType].FillWeight = 60;
            if (grid.Columns.Contains(ColSearchable))
            {
                grid.Columns[ColSearchable].FillWeight = 45;
                grid.Columns[ColSearchable].HeaderText = "Search";
            }
            if (grid.Columns.Contains(ColNewDisplayName))
                grid.Columns[ColNewDisplayName].FillWeight = 120;
        }

        private void ShowError(string title, Exception ex)
        {
            LogError("{0}: {1}", title, ex.ToString());
            MessageBox.Show($"{title}.\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            var message = new StringBuilder()
                .AppendLine("1. Click 'Load Tables' to load all tables of your environment.")
                .AppendLine()
                .AppendLine("2. Click a table to list its renamable fields.")
                .AppendLine()
                .AppendLine("3. Select fields (Ctrl/Shift + click for multiple) and click 'Add selected', press Enter or double-click. 'Add all listed' adds every field matching the search.")
                .AppendLine()
                .AppendLine("4. Remove fields from the selection with 'Remove', the Delete key or a double-click. Click 'Usage' in front of a field to see everywhere it is used.")
                .AppendLine()
                .AppendLine("5. Choose Prefix/Suffix and Add/Remove, type the text and check the 'New Display Name' preview.")
                .AppendLine()
                .AppendLine("6. Optionally switch on 'Clean up the fields'. Click 'Options' to choose what it does: remove from forms, views, plug-in steps, make optional, remove from Advanced Find.")
                .AppendLine()
                .AppendLine("7. Click 'Export to CSV' to back up the current labels, then 'Apply'. A review window lists every form, view, chart, process or plug-in using the fields and what will be done. Nothing changes until you confirm. The table is then published and the Result card shows each operation.")
                .AppendLine()
                .AppendLine("All translations of the display name are updated. Comparison is case-insensitive and a prefix already present is not added twice.")
                .AppendLine()
                .AppendLine("Click Yes to open the full documentation.");

            if (MessageBox.Show(message.ToString(), "Help", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Process.Start("https://dyn365forlife.wixsite.com/my-site/post/tools-bulk-prefix-suffix-attributes-name");
            }
        }

        private void byButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.linkedin.com/in/hamza-amraoui/");
        }

        #endregion
    }
}
