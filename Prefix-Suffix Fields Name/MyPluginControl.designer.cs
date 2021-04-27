namespace Prefix_Suffix_Fields_Name
{
    partial class MyPluginControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyPluginControl));
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSample = new System.Windows.Forms.ToolStripButton();
            this.loadEntitiesButto = new System.Windows.Forms.ToolStripButton();
            this.loadFieldsButton = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.entitiesGroupBox = new System.Windows.Forms.GroupBox();
            this.entityDataGridView = new System.Windows.Forms.DataGridView();
            this.fieldsGroupBox = new System.Windows.Forms.GroupBox();
            this.fieldsDataGridView = new System.Windows.Forms.DataGridView();
            this.propretiesGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.prefixGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.preffixButton = new System.Windows.Forms.RadioButton();
            this.SuffixButton = new System.Windows.Forms.RadioButton();
            this.addRemoveGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.AddButton = new System.Windows.Forms.RadioButton();
            this.RemoveButton = new System.Windows.Forms.RadioButton();
            this.nameGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.PSTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.ProceedButton = new System.Windows.Forms.Button();
            this.logGroupBox = new System.Windows.Forms.GroupBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenu.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.entitiesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entityDataGridView)).BeginInit();
            this.fieldsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fieldsDataGridView)).BeginInit();
            this.propretiesGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.prefixGroupBox.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.addRemoveGroupBox.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.nameGroupBox.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1,
            this.tsbSample,
            this.loadEntitiesButto,
            this.loadFieldsButton});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1347, 31);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(86, 28);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // tsbSample
            // 
            this.tsbSample.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbSample.Name = "tsbSample";
            this.tsbSample.Size = new System.Drawing.Size(46, 28);
            this.tsbSample.Text = "Try me";
            this.tsbSample.Click += new System.EventHandler(this.tsbSample_Click);
            // 
            // loadEntitiesButto
            // 
            this.loadEntitiesButto.Image = ((System.Drawing.Image)(resources.GetObject("loadEntitiesButto.Image")));
            this.loadEntitiesButto.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loadEntitiesButto.Name = "loadEntitiesButto";
            this.loadEntitiesButto.Size = new System.Drawing.Size(102, 28);
            this.loadEntitiesButto.Text = "Load Entities";
            this.loadEntitiesButto.ToolTipText = "Load Entities";
            this.loadEntitiesButto.Click += new System.EventHandler(this.loadEntitiesButto_Click);
            // 
            // loadFieldsButton
            // 
            this.loadFieldsButton.Image = ((System.Drawing.Image)(resources.GetObject("loadFieldsButton.Image")));
            this.loadFieldsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loadFieldsButton.Name = "loadFieldsButton";
            this.loadFieldsButton.Size = new System.Drawing.Size(94, 28);
            this.loadFieldsButton.Text = "Load Fields";
            this.loadFieldsButton.Click += new System.EventHandler(this.loadFieldsButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.entitiesGroupBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.fieldsGroupBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.propretiesGroupBox, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 31);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1347, 565);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // entitiesGroupBox
            // 
            this.entitiesGroupBox.Controls.Add(this.entityDataGridView);
            this.entitiesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entitiesGroupBox.Location = new System.Drawing.Point(3, 3);
            this.entitiesGroupBox.Name = "entitiesGroupBox";
            this.entitiesGroupBox.Size = new System.Drawing.Size(330, 559);
            this.entitiesGroupBox.TabIndex = 0;
            this.entitiesGroupBox.TabStop = false;
            this.entitiesGroupBox.Text = "Entities";
            // 
            // entityDataGridView
            // 
            this.entityDataGridView.AllowUserToAddRows = false;
            this.entityDataGridView.AllowUserToDeleteRows = false;
            this.entityDataGridView.AllowUserToResizeRows = false;
            this.entityDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.entityDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.entityDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityDataGridView.Location = new System.Drawing.Point(3, 16);
            this.entityDataGridView.Name = "entityDataGridView";
            this.entityDataGridView.ReadOnly = true;
            this.entityDataGridView.Size = new System.Drawing.Size(324, 540);
            this.entityDataGridView.TabIndex = 0;
            this.entityDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.entityDataGridView_CellContentClick);
            // 
            // fieldsGroupBox
            // 
            this.fieldsGroupBox.Controls.Add(this.fieldsDataGridView);
            this.fieldsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsGroupBox.Location = new System.Drawing.Point(339, 3);
            this.fieldsGroupBox.Name = "fieldsGroupBox";
            this.fieldsGroupBox.Size = new System.Drawing.Size(330, 559);
            this.fieldsGroupBox.TabIndex = 1;
            this.fieldsGroupBox.TabStop = false;
            this.fieldsGroupBox.Text = "Entity Fields";
            // 
            // fieldsDataGridView
            // 
            this.fieldsDataGridView.AllowUserToAddRows = false;
            this.fieldsDataGridView.AllowUserToDeleteRows = false;
            this.fieldsDataGridView.AllowUserToResizeRows = false;
            this.fieldsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
            this.fieldsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fieldsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsDataGridView.Location = new System.Drawing.Point(3, 16);
            this.fieldsDataGridView.Name = "fieldsDataGridView";
            this.fieldsDataGridView.Size = new System.Drawing.Size(324, 540);
            this.fieldsDataGridView.TabIndex = 0;
            // 
            // propretiesGroupBox
            // 
            this.propretiesGroupBox.Controls.Add(this.tableLayoutPanel2);
            this.propretiesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propretiesGroupBox.Location = new System.Drawing.Point(675, 3);
            this.propretiesGroupBox.Name = "propretiesGroupBox";
            this.propretiesGroupBox.Size = new System.Drawing.Size(669, 559);
            this.propretiesGroupBox.TabIndex = 2;
            this.propretiesGroupBox.TabStop = false;
            this.propretiesGroupBox.Text = "Propreties";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.nameGroupBox, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.logGroupBox, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(663, 540);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.prefixGroupBox, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.addRemoveGroupBox, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(657, 75);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // prefixGroupBox
            // 
            this.prefixGroupBox.Controls.Add(this.tableLayoutPanel4);
            this.prefixGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prefixGroupBox.Location = new System.Drawing.Point(3, 3);
            this.prefixGroupBox.Name = "prefixGroupBox";
            this.prefixGroupBox.Size = new System.Drawing.Size(322, 69);
            this.prefixGroupBox.TabIndex = 0;
            this.prefixGroupBox.TabStop = false;
            this.prefixGroupBox.Text = "Suffix/Preffix";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.preffixButton, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.SuffixButton, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(316, 50);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // preffixButton
            // 
            this.preffixButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.preffixButton.AutoSize = true;
            this.preffixButton.Location = new System.Drawing.Point(3, 3);
            this.preffixButton.Name = "preffixButton";
            this.preffixButton.Size = new System.Drawing.Size(152, 44);
            this.preffixButton.TabIndex = 0;
            this.preffixButton.TabStop = true;
            this.preffixButton.Text = "Preffix Name";
            this.preffixButton.UseVisualStyleBackColor = true;
            // 
            // SuffixButton
            // 
            this.SuffixButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SuffixButton.AutoSize = true;
            this.SuffixButton.Location = new System.Drawing.Point(161, 3);
            this.SuffixButton.Name = "SuffixButton";
            this.SuffixButton.Size = new System.Drawing.Size(152, 44);
            this.SuffixButton.TabIndex = 1;
            this.SuffixButton.TabStop = true;
            this.SuffixButton.Text = "Suffix Name";
            this.SuffixButton.UseVisualStyleBackColor = true;
            // 
            // addRemoveGroupBox
            // 
            this.addRemoveGroupBox.Controls.Add(this.tableLayoutPanel5);
            this.addRemoveGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addRemoveGroupBox.Location = new System.Drawing.Point(331, 3);
            this.addRemoveGroupBox.Name = "addRemoveGroupBox";
            this.addRemoveGroupBox.Size = new System.Drawing.Size(323, 69);
            this.addRemoveGroupBox.TabIndex = 1;
            this.addRemoveGroupBox.TabStop = false;
            this.addRemoveGroupBox.Text = "Add/Remove";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.AddButton, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.RemoveButton, 1, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(317, 50);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // AddButton
            // 
            this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AddButton.AutoSize = true;
            this.AddButton.Location = new System.Drawing.Point(3, 3);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(152, 44);
            this.AddButton.TabIndex = 0;
            this.AddButton.TabStop = true;
            this.AddButton.Text = "Add Text";
            this.AddButton.UseVisualStyleBackColor = true;
            // 
            // RemoveButton
            // 
            this.RemoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveButton.AutoSize = true;
            this.RemoveButton.Location = new System.Drawing.Point(161, 3);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(153, 44);
            this.RemoveButton.TabIndex = 1;
            this.RemoveButton.TabStop = true;
            this.RemoveButton.Text = "Remove Text";
            this.RemoveButton.UseVisualStyleBackColor = true;
            // 
            // nameGroupBox
            // 
            this.nameGroupBox.Controls.Add(this.tableLayoutPanel6);
            this.nameGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nameGroupBox.Location = new System.Drawing.Point(3, 84);
            this.nameGroupBox.Name = "nameGroupBox";
            this.nameGroupBox.Size = new System.Drawing.Size(657, 75);
            this.nameGroupBox.TabIndex = 1;
            this.nameGroupBox.TabStop = false;
            this.nameGroupBox.Text = "Preffix/Suffix Text";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel8, 1, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(651, 56);
            this.tableLayoutPanel6.TabIndex = 0;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 1;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel7.Controls.Add(this.PSTextBox, 0, 1);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 3;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(449, 50);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // PSTextBox
            // 
            this.PSTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PSTextBox.Location = new System.Drawing.Point(0, 10);
            this.PSTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.PSTextBox.MaxLength = 50;
            this.PSTextBox.Name = "PSTextBox";
            this.PSTextBox.Size = new System.Drawing.Size(449, 20);
            this.PSTextBox.TabIndex = 0;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 3;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel8.Controls.Add(this.ProceedButton, 1, 1);
            this.tableLayoutPanel8.Location = new System.Drawing.Point(458, 3);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 3;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(190, 50);
            this.tableLayoutPanel8.TabIndex = 1;
            // 
            // ProceedButton
            // 
            this.ProceedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProceedButton.Location = new System.Drawing.Point(41, 11);
            this.ProceedButton.Name = "ProceedButton";
            this.ProceedButton.Size = new System.Drawing.Size(108, 27);
            this.ProceedButton.TabIndex = 1;
            this.ProceedButton.Text = "Proceed";
            this.ProceedButton.UseVisualStyleBackColor = true;
            this.ProceedButton.Click += new System.EventHandler(this.ProceedButton_Click);
            // 
            // logGroupBox
            // 
            this.logGroupBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.logGroupBox.Location = new System.Drawing.Point(3, 165);
            this.logGroupBox.Name = "logGroupBox";
            this.logGroupBox.Size = new System.Drawing.Size(657, 372);
            this.logGroupBox.TabIndex = 2;
            this.logGroupBox.TabStop = false;
            this.logGroupBox.Text = "Log";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // MyPluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(1347, 596);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.entitiesGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.entityDataGridView)).EndInit();
            this.fieldsGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fieldsDataGridView)).EndInit();
            this.propretiesGroupBox.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.prefixGroupBox.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.addRemoveGroupBox.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.nameGroupBox.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripButton tsbSample;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox entitiesGroupBox;
        private System.Windows.Forms.GroupBox fieldsGroupBox;
        private System.Windows.Forms.DataGridView entityDataGridView;
        private System.Windows.Forms.DataGridView fieldsDataGridView;
        private System.Windows.Forms.GroupBox propretiesGroupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox prefixGroupBox;
        private System.Windows.Forms.GroupBox addRemoveGroupBox;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.GroupBox nameGroupBox;
        private System.Windows.Forms.GroupBox logGroupBox;
        private System.Windows.Forms.ToolStripButton loadEntitiesButto;
        private System.Windows.Forms.ToolStripButton loadFieldsButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.RadioButton preffixButton;
        private System.Windows.Forms.RadioButton SuffixButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.RadioButton AddButton;
        private System.Windows.Forms.RadioButton RemoveButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.TextBox PSTextBox;
        private System.Windows.Forms.Button ProceedButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
    }
}
