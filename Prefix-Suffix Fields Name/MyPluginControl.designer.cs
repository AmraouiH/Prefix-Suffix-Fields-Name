namespace Prefix_Suffix_Fields_Name
{
    partial class MyPluginControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadEntitiesButton = new System.Windows.Forms.ToolStripButton();
            this.loadFieldsButton = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exportButton = new System.Windows.Forms.ToolStripButton();
            this.byButton = new System.Windows.Forms.ToolStripButton();
            this.helpButton = new System.Windows.Forms.ToolStripButton();
            this.optionsButton = new System.Windows.Forms.ToolStripButton();
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.entitiesCard = new Prefix_Suffix_Fields_Name.UI.CardBox();
            this.entitiesLayout = new System.Windows.Forms.TableLayoutPanel();
            this.entityTextSearch = new Prefix_Suffix_Fields_Name.UI.FluentTextBox();
            this.entityDataGridView = new System.Windows.Forms.DataGridView();
            this.fieldsCard = new Prefix_Suffix_Fields_Name.UI.CardBox();
            this.fieldsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.fieldsTextSearch = new Prefix_Suffix_Fields_Name.UI.FluentTextBox();
            this.fieldsDataGridView = new System.Windows.Forms.DataGridView();
            this.fieldsButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.addFieldsButton = new Prefix_Suffix_Fields_Name.UI.FluentButton();
            this.addAllFilteredButton = new Prefix_Suffix_Fields_Name.UI.FluentButton();
            this.rightLayout = new System.Windows.Forms.TableLayoutPanel();
            this.contextHeader = new Prefix_Suffix_Fields_Name.UI.ContextHeader();
            this.renameCard = new Prefix_Suffix_Fields_Name.UI.CardBox();
            this.renameLayout = new System.Windows.Forms.TableLayoutPanel();
            this.positionLabel = new System.Windows.Forms.Label();
            this.positionChips = new Prefix_Suffix_Fields_Name.UI.ChipBar();
            this.actionLabel = new System.Windows.Forms.Label();
            this.actionChips = new Prefix_Suffix_Fields_Name.UI.ChipBar();
            this.textLabel = new System.Windows.Forms.Label();
            this.affixTextBox = new Prefix_Suffix_Fields_Name.UI.FluentTextBox();
            this.cleanupToggle = new Prefix_Suffix_Fields_Name.UI.ToggleSwitch();
            this.applyButton = new Prefix_Suffix_Fields_Name.UI.FluentButton();
            this.selectedCard = new Prefix_Suffix_Fields_Name.UI.CardBox();
            this.selectedLayout = new System.Windows.Forms.TableLayoutPanel();
            this.fieldsUpdateDataGridView = new System.Windows.Forms.DataGridView();
            this.selectedButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.removeFieldsButton = new Prefix_Suffix_Fields_Name.UI.FluentButton();
            this.clearFieldsButton = new Prefix_Suffix_Fields_Name.UI.FluentButton();
            this.resultCard = new Prefix_Suffix_Fields_Name.UI.CardBox();
            this.updateFieldsResultDataGridView = new System.Windows.Forms.DataGridView();
            this.toolStripMenu.SuspendLayout();
            this.rootLayout.SuspendLayout();
            this.entitiesCard.SuspendLayout();
            this.entitiesLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entityDataGridView)).BeginInit();
            this.fieldsCard.SuspendLayout();
            this.fieldsLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fieldsDataGridView)).BeginInit();
            this.fieldsButtonsPanel.SuspendLayout();
            this.rightLayout.SuspendLayout();
            this.renameCard.SuspendLayout();
            this.renameLayout.SuspendLayout();
            this.selectedCard.SuspendLayout();
            this.selectedLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fieldsUpdateDataGridView)).BeginInit();
            this.selectedButtonsPanel.SuspendLayout();
            this.resultCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updateFieldsResultDataGridView)).BeginInit();
            this.SuspendLayout();
            //
            // toolStripMenu
            //
            this.toolStripMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(16, 16);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1,
            this.loadEntitiesButton,
            this.loadFieldsButton,
            this.tssSeparator2,
            this.exportButton,
            this.byButton,
            this.helpButton,
            this.optionsButton});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.toolStripMenu.Size = new System.Drawing.Size(1400, 44);
            this.toolStripMenu.TabIndex = 0;
            //
            // tsbClose
            //
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.tsbClose.ToolTipText = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            //
            // tssSeparator1
            //
            this.tssSeparator1.Name = "tssSeparator1";
            //
            // loadEntitiesButton
            //
            this.loadEntitiesButton.Name = "loadEntitiesButton";
            this.loadEntitiesButton.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.loadEntitiesButton.Text = "Load Tables";
            this.loadEntitiesButton.ToolTipText = "Load all tables of the environment";
            this.loadEntitiesButton.Click += new System.EventHandler(this.loadEntitiesButton_Click);
            //
            // loadFieldsButton
            //
            this.loadFieldsButton.Enabled = false;
            this.loadFieldsButton.Name = "loadFieldsButton";
            this.loadFieldsButton.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.loadFieldsButton.Text = "Reload Fields";
            this.loadFieldsButton.ToolTipText = "Reload the fields of the selected table";
            this.loadFieldsButton.Click += new System.EventHandler(this.loadFieldsButton_Click);
            //
            // tssSeparator2
            //
            this.tssSeparator2.Name = "tssSeparator2";
            //
            // exportButton
            //
            this.exportButton.Enabled = false;
            this.exportButton.Name = "exportButton";
            this.exportButton.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.exportButton.Text = "Export to CSV";
            this.exportButton.ToolTipText = "Back up the current labels (all languages) of the table fields";
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            //
            // byButton
            //
            this.byButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.byButton.Name = "byButton";
            this.byButton.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.byButton.Tag = "link";
            this.byButton.Text = "Hamza AMRAOUI";
            this.byButton.Click += new System.EventHandler(this.byButton_Click);
            //
            // helpButton
            //
            this.helpButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.helpButton.Name = "helpButton";
            this.helpButton.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.helpButton.Tag = "link";
            this.helpButton.Text = "Help";
            this.helpButton.Click += new System.EventHandler(this.HelpButton_Click);
            //
            // optionsButton
            //
            this.optionsButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.optionsButton.Name = "optionsButton";
            this.optionsButton.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.optionsButton.Tag = "link";
            this.optionsButton.Text = "Options";
            this.optionsButton.ToolTipText = "Choose what \u201cClean up the fields\u201d does";
            this.optionsButton.Click += new System.EventHandler(this.optionsButton_Click);
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 3;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.rootLayout.Controls.Add(this.entitiesCard, 0, 0);
            this.rootLayout.Controls.Add(this.fieldsCard, 1, 0);
            this.rootLayout.Controls.Add(this.rightLayout, 2, 0);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(8);
            this.rootLayout.RowCount = 1;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.TabIndex = 1;
            //
            // entitiesCard
            //
            this.entitiesCard.Controls.Add(this.entitiesLayout);
            this.entitiesCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entitiesCard.Name = "entitiesCard";
            this.entitiesCard.Subtitle = "load them to start";
            this.entitiesCard.TabIndex = 0;
            this.entitiesCard.TabStop = false;
            this.entitiesCard.Text = "Tables";
            //
            // entitiesLayout
            //
            this.entitiesLayout.BackColor = System.Drawing.Color.White;
            this.entitiesLayout.ColumnCount = 1;
            this.entitiesLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.entitiesLayout.Controls.Add(this.entityTextSearch, 0, 0);
            this.entitiesLayout.Controls.Add(this.entityDataGridView, 0, 1);
            this.entitiesLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entitiesLayout.Name = "entitiesLayout";
            this.entitiesLayout.RowCount = 2;
            this.entitiesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.entitiesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.entitiesLayout.TabIndex = 0;
            //
            // entityTextSearch
            //
            this.entityTextSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.entityTextSearch.Enabled = false;
            this.entityTextSearch.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.entityTextSearch.Name = "entityTextSearch";
            this.entityTextSearch.Placeholder = "Search by display or schema name";
            this.entityTextSearch.Size = new System.Drawing.Size(200, 32);
            this.entityTextSearch.TabIndex = 0;
            this.entityTextSearch.TextChanged += new System.EventHandler(this.entityTextSearch_TextChanged);
            //
            // entityDataGridView
            //
            this.entityDataGridView.AllowUserToAddRows = false;
            this.entityDataGridView.AllowUserToDeleteRows = false;
            this.entityDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityDataGridView.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.entityDataGridView.MultiSelect = false;
            this.entityDataGridView.Name = "entityDataGridView";
            this.entityDataGridView.ReadOnly = true;
            this.entityDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.entityDataGridView.TabIndex = 1;
            this.entityDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.entityDataGridView_CellClick);
            this.entityDataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.entityDataGridView_KeyDown);
            //
            // fieldsCard
            //
            this.fieldsCard.Controls.Add(this.fieldsLayout);
            this.fieldsCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsCard.Name = "fieldsCard";
            this.fieldsCard.Subtitle = "click a table";
            this.fieldsCard.TabIndex = 1;
            this.fieldsCard.TabStop = false;
            this.fieldsCard.Text = "Fields";
            //
            // fieldsLayout
            //
            this.fieldsLayout.BackColor = System.Drawing.Color.White;
            this.fieldsLayout.ColumnCount = 1;
            this.fieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fieldsLayout.Controls.Add(this.fieldsTextSearch, 0, 0);
            this.fieldsLayout.Controls.Add(this.fieldsDataGridView, 0, 1);
            this.fieldsLayout.Controls.Add(this.fieldsButtonsPanel, 0, 2);
            this.fieldsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsLayout.Name = "fieldsLayout";
            this.fieldsLayout.RowCount = 3;
            this.fieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.fieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.fieldsLayout.TabIndex = 0;
            //
            // fieldsTextSearch
            //
            this.fieldsTextSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.fieldsTextSearch.Enabled = false;
            this.fieldsTextSearch.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.fieldsTextSearch.Name = "fieldsTextSearch";
            this.fieldsTextSearch.Placeholder = "Search by display or schema name";
            this.fieldsTextSearch.Size = new System.Drawing.Size(200, 32);
            this.fieldsTextSearch.TabIndex = 0;
            this.fieldsTextSearch.TextChanged += new System.EventHandler(this.fieldsTextSearch_TextChanged);
            //
            // fieldsDataGridView
            //
            this.fieldsDataGridView.AllowUserToAddRows = false;
            this.fieldsDataGridView.AllowUserToDeleteRows = false;
            this.fieldsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsDataGridView.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.fieldsDataGridView.Name = "fieldsDataGridView";
            this.fieldsDataGridView.ReadOnly = true;
            this.fieldsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.fieldsDataGridView.TabIndex = 1;
            this.fieldsDataGridView.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.fieldsDataGridView_CellMouseDoubleClick);
            this.fieldsDataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fieldsDataGridView_KeyDown);
            //
            // fieldsButtonsPanel
            //
            this.fieldsButtonsPanel.BackColor = System.Drawing.Color.White;
            this.fieldsButtonsPanel.Controls.Add(this.addFieldsButton);
            this.fieldsButtonsPanel.Controls.Add(this.addAllFilteredButton);
            this.fieldsButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsButtonsPanel.Name = "fieldsButtonsPanel";
            this.fieldsButtonsPanel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.fieldsButtonsPanel.TabIndex = 2;
            //
            // addFieldsButton
            //
            this.addFieldsButton.Enabled = false;
            this.addFieldsButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.addFieldsButton.Name = "addFieldsButton";
            this.addFieldsButton.Size = new System.Drawing.Size(140, 32);
            this.addFieldsButton.TabIndex = 0;
            this.addFieldsButton.Text = "Add selected";
            this.addFieldsButton.Click += new System.EventHandler(this.addFieldsButton_Click);
            //
            // addAllFilteredButton
            //
            this.addAllFilteredButton.Enabled = false;
            this.addAllFilteredButton.Margin = new System.Windows.Forms.Padding(0);
            this.addAllFilteredButton.Name = "addAllFilteredButton";
            this.addAllFilteredButton.Primary = false;
            this.addAllFilteredButton.Size = new System.Drawing.Size(140, 32);
            this.addAllFilteredButton.TabIndex = 1;
            this.addAllFilteredButton.Text = "Add all listed";
            this.addAllFilteredButton.Click += new System.EventHandler(this.addAllFilteredButton_Click);
            //
            // rightLayout
            //
            this.rightLayout.ColumnCount = 1;
            this.rightLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightLayout.Controls.Add(this.contextHeader, 0, 0);
            this.rightLayout.Controls.Add(this.renameCard, 0, 1);
            this.rightLayout.Controls.Add(this.selectedCard, 0, 2);
            this.rightLayout.Controls.Add(this.resultCard, 0, 3);
            this.rightLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightLayout.Margin = new System.Windows.Forms.Padding(0);
            this.rightLayout.Name = "rightLayout";
            this.rightLayout.RowCount = 4;
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 196F));
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.rightLayout.TabIndex = 2;
            //
            // contextHeader
            //
            this.contextHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contextHeader.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.contextHeader.Name = "contextHeader";
            this.contextHeader.TabIndex = 0;
            //
            // renameCard
            //
            this.renameCard.Controls.Add(this.renameLayout);
            this.renameCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renameCard.Name = "renameCard";
            this.renameCard.Subtitle = "choose what to apply to the selected fields";
            this.renameCard.TabIndex = 1;
            this.renameCard.TabStop = false;
            this.renameCard.Text = "Rename";
            //
            // renameLayout
            //
            this.renameLayout.BackColor = System.Drawing.Color.White;
            this.renameLayout.ColumnCount = 4;
            this.renameLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.renameLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.renameLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.renameLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.renameLayout.Controls.Add(this.positionLabel, 0, 0);
            this.renameLayout.Controls.Add(this.positionChips, 1, 0);
            this.renameLayout.Controls.Add(this.actionLabel, 2, 0);
            this.renameLayout.Controls.Add(this.actionChips, 3, 0);
            this.renameLayout.Controls.Add(this.textLabel, 0, 1);
            this.renameLayout.Controls.Add(this.affixTextBox, 1, 1);
            this.renameLayout.Controls.Add(this.cleanupToggle, 0, 2);
            this.renameLayout.Controls.Add(this.applyButton, 3, 2);
            this.renameLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renameLayout.Name = "renameLayout";
            this.renameLayout.RowCount = 3;
            this.renameLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.renameLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.renameLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.renameLayout.SetColumnSpan(this.affixTextBox, 3);
            this.renameLayout.SetColumnSpan(this.cleanupToggle, 3);
            this.renameLayout.TabIndex = 0;
            //
            // positionLabel
            //
            this.positionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.positionLabel.AutoSize = true;
            this.positionLabel.Name = "positionLabel";
            this.positionLabel.TabIndex = 0;
            this.positionLabel.Text = "Position";
            //
            // positionChips
            //
            this.positionChips.Dock = System.Windows.Forms.DockStyle.Fill;
            this.positionChips.Name = "positionChips";
            this.positionChips.TabIndex = 1;
            this.positionChips.SelectedIndexChanged += new System.EventHandler(this.RenameOptions_Changed);
            //
            // actionLabel
            //
            this.actionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.actionLabel.AutoSize = true;
            this.actionLabel.Name = "actionLabel";
            this.actionLabel.TabIndex = 2;
            this.actionLabel.Text = "Action";
            //
            // actionChips
            //
            this.actionChips.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionChips.Name = "actionChips";
            this.actionChips.TabIndex = 3;
            this.actionChips.SelectedIndexChanged += new System.EventHandler(this.RenameOptions_Changed);
            //
            // textLabel
            //
            this.textLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textLabel.AutoSize = true;
            this.textLabel.Name = "textLabel";
            this.textLabel.TabIndex = 4;
            this.textLabel.Text = "Text";
            //
            // affixTextBox
            //
            this.affixTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.affixTextBox.MaxLength = 50;
            this.affixTextBox.Name = "affixTextBox";
            this.affixTextBox.Placeholder = "e.g. ZZ_ or [Deprecated] ";
            this.affixTextBox.Size = new System.Drawing.Size(200, 32);
            this.affixTextBox.TabIndex = 5;
            this.affixTextBox.TextChanged += new System.EventHandler(this.RenameOptions_Changed);
            //
            // cleanupToggle
            //
            this.cleanupToggle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cleanupToggle.Name = "cleanupToggle";
            this.cleanupToggle.TabIndex = 6;
            this.cleanupToggle.Text = "Options";
            this.cleanupToggle.CheckedChanged += new System.EventHandler(this.RenameOptions_Changed);
            //
            // applyButton
            //
            this.applyButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.applyButton.Enabled = false;
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(150, 34);
            this.applyButton.TabIndex = 7;
            this.applyButton.Text = "Apply";
            this.applyButton.Click += new System.EventHandler(this.ProceedButton_Click);
            //
            // selectedCard
            //
            this.selectedCard.Controls.Add(this.selectedLayout);
            this.selectedCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectedCard.Name = "selectedCard";
            this.selectedCard.Subtitle = "preview of the new display names";
            this.selectedCard.TabIndex = 2;
            this.selectedCard.TabStop = false;
            this.selectedCard.Text = "Selected fields";
            //
            // selectedLayout
            //
            this.selectedLayout.BackColor = System.Drawing.Color.White;
            this.selectedLayout.ColumnCount = 1;
            this.selectedLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.selectedLayout.Controls.Add(this.fieldsUpdateDataGridView, 0, 0);
            this.selectedLayout.Controls.Add(this.selectedButtonsPanel, 0, 1);
            this.selectedLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectedLayout.Name = "selectedLayout";
            this.selectedLayout.RowCount = 2;
            this.selectedLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.selectedLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.selectedLayout.TabIndex = 0;
            //
            // fieldsUpdateDataGridView
            //
            this.fieldsUpdateDataGridView.AllowUserToAddRows = false;
            this.fieldsUpdateDataGridView.AllowUserToDeleteRows = false;
            this.fieldsUpdateDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsUpdateDataGridView.Margin = new System.Windows.Forms.Padding(0);
            this.fieldsUpdateDataGridView.Name = "fieldsUpdateDataGridView";
            this.fieldsUpdateDataGridView.ReadOnly = true;
            this.fieldsUpdateDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.fieldsUpdateDataGridView.TabIndex = 0;
            this.fieldsUpdateDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.fieldsUpdateDataGridView_CellFormatting);
            this.fieldsUpdateDataGridView.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.fieldsUpdateDataGridView_CellMouseDoubleClick);
            this.fieldsUpdateDataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fieldsUpdateDataGridView_KeyDown);
            //
            // selectedButtonsPanel
            //
            this.selectedButtonsPanel.BackColor = System.Drawing.Color.White;
            this.selectedButtonsPanel.Controls.Add(this.removeFieldsButton);
            this.selectedButtonsPanel.Controls.Add(this.clearFieldsButton);
            this.selectedButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectedButtonsPanel.Name = "selectedButtonsPanel";
            this.selectedButtonsPanel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.selectedButtonsPanel.TabIndex = 1;
            //
            // removeFieldsButton
            //
            this.removeFieldsButton.Enabled = false;
            this.removeFieldsButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.removeFieldsButton.Name = "removeFieldsButton";
            this.removeFieldsButton.Primary = false;
            this.removeFieldsButton.Size = new System.Drawing.Size(120, 32);
            this.removeFieldsButton.TabIndex = 0;
            this.removeFieldsButton.Text = "Remove";
            this.removeFieldsButton.Click += new System.EventHandler(this.removeFieldsButton_Click);
            //
            // clearFieldsButton
            //
            this.clearFieldsButton.Enabled = false;
            this.clearFieldsButton.Margin = new System.Windows.Forms.Padding(0);
            this.clearFieldsButton.Name = "clearFieldsButton";
            this.clearFieldsButton.Primary = false;
            this.clearFieldsButton.Size = new System.Drawing.Size(120, 32);
            this.clearFieldsButton.TabIndex = 1;
            this.clearFieldsButton.Text = "Clear";
            this.clearFieldsButton.Click += new System.EventHandler(this.clearFieldsButton_Click);
            //
            // resultCard
            //
            this.resultCard.Controls.Add(this.updateFieldsResultDataGridView);
            this.resultCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultCard.Name = "resultCard";
            this.resultCard.Subtitle = "status of the last update";
            this.resultCard.TabIndex = 3;
            this.resultCard.TabStop = false;
            this.resultCard.Text = "Result";
            //
            // updateFieldsResultDataGridView
            //
            this.updateFieldsResultDataGridView.AllowUserToAddRows = false;
            this.updateFieldsResultDataGridView.AllowUserToDeleteRows = false;
            this.updateFieldsResultDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.updateFieldsResultDataGridView.Name = "updateFieldsResultDataGridView";
            this.updateFieldsResultDataGridView.ReadOnly = true;
            this.updateFieldsResultDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.updateFieldsResultDataGridView.TabIndex = 0;
            //
            // MyPluginControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.rootLayout);
            this.Controls.Add(this.toolStripMenu);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(1400, 800);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.rootLayout.ResumeLayout(false);
            this.entitiesCard.ResumeLayout(false);
            this.entitiesLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.entityDataGridView)).EndInit();
            this.fieldsCard.ResumeLayout(false);
            this.fieldsLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fieldsDataGridView)).EndInit();
            this.fieldsButtonsPanel.ResumeLayout(false);
            this.rightLayout.ResumeLayout(false);
            this.renameCard.ResumeLayout(false);
            this.renameLayout.ResumeLayout(false);
            this.renameLayout.PerformLayout();
            this.selectedCard.ResumeLayout(false);
            this.selectedLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fieldsUpdateDataGridView)).EndInit();
            this.selectedButtonsPanel.ResumeLayout(false);
            this.resultCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.updateFieldsResultDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.ToolStripButton loadEntitiesButton;
        private System.Windows.Forms.ToolStripButton loadFieldsButton;
        private System.Windows.Forms.ToolStripSeparator tssSeparator2;
        private System.Windows.Forms.ToolStripButton exportButton;
        private System.Windows.Forms.ToolStripButton byButton;
        private System.Windows.Forms.ToolStripButton helpButton;
        private System.Windows.Forms.ToolStripButton optionsButton;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private UI.CardBox entitiesCard;
        private System.Windows.Forms.TableLayoutPanel entitiesLayout;
        private UI.FluentTextBox entityTextSearch;
        private System.Windows.Forms.DataGridView entityDataGridView;
        private UI.CardBox fieldsCard;
        private System.Windows.Forms.TableLayoutPanel fieldsLayout;
        private UI.FluentTextBox fieldsTextSearch;
        private System.Windows.Forms.DataGridView fieldsDataGridView;
        private System.Windows.Forms.FlowLayoutPanel fieldsButtonsPanel;
        private UI.FluentButton addFieldsButton;
        private UI.FluentButton addAllFilteredButton;
        private System.Windows.Forms.TableLayoutPanel rightLayout;
        private UI.ContextHeader contextHeader;
        private UI.CardBox renameCard;
        private System.Windows.Forms.TableLayoutPanel renameLayout;
        private System.Windows.Forms.Label positionLabel;
        private UI.ChipBar positionChips;
        private System.Windows.Forms.Label actionLabel;
        private UI.ChipBar actionChips;
        private System.Windows.Forms.Label textLabel;
        private UI.FluentTextBox affixTextBox;
        private UI.ToggleSwitch cleanupToggle;
        private UI.FluentButton applyButton;
        private UI.CardBox selectedCard;
        private System.Windows.Forms.TableLayoutPanel selectedLayout;
        private System.Windows.Forms.DataGridView fieldsUpdateDataGridView;
        private System.Windows.Forms.FlowLayoutPanel selectedButtonsPanel;
        private UI.FluentButton removeFieldsButton;
        private UI.FluentButton clearFieldsButton;
        private UI.CardBox resultCard;
        private System.Windows.Forms.DataGridView updateFieldsResultDataGridView;
    }
}
