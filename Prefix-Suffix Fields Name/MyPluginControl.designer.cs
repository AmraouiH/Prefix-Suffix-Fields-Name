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
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSample = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.entitiesGroupBox = new System.Windows.Forms.GroupBox();
            this.fieldsGroupBox = new System.Windows.Forms.GroupBox();
            this.entityDataGridView = new System.Windows.Forms.DataGridView();
            this.fieldsDataGridView = new System.Windows.Forms.DataGridView();
            this.propretiesGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.prefixGroupBox = new System.Windows.Forms.GroupBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.addRemoveGroupBox = new System.Windows.Forms.GroupBox();
            this.nameGroupBox = new System.Windows.Forms.GroupBox();
            this.logGroupBox = new System.Windows.Forms.GroupBox();
            this.toolStripMenu.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.entitiesGroupBox.SuspendLayout();
            this.fieldsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entityDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fieldsDataGridView)).BeginInit();
            this.propretiesGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1,
            this.tsbSample});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1047, 25);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(86, 22);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbSample
            // 
            this.tsbSample.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbSample.Name = "tsbSample";
            this.tsbSample.Size = new System.Drawing.Size(46, 22);
            this.tsbSample.Text = "Try me";
            this.tsbSample.Click += new System.EventHandler(this.tsbSample_Click);
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1047, 549);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // entitiesGroupBox
            // 
            this.entitiesGroupBox.Controls.Add(this.entityDataGridView);
            this.entitiesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entitiesGroupBox.Location = new System.Drawing.Point(3, 3);
            this.entitiesGroupBox.Name = "entitiesGroupBox";
            this.entitiesGroupBox.Size = new System.Drawing.Size(255, 543);
            this.entitiesGroupBox.TabIndex = 0;
            this.entitiesGroupBox.TabStop = false;
            this.entitiesGroupBox.Text = "Entities";
            // 
            // fieldsGroupBox
            // 
            this.fieldsGroupBox.Controls.Add(this.fieldsDataGridView);
            this.fieldsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsGroupBox.Location = new System.Drawing.Point(264, 3);
            this.fieldsGroupBox.Name = "fieldsGroupBox";
            this.fieldsGroupBox.Size = new System.Drawing.Size(255, 543);
            this.fieldsGroupBox.TabIndex = 1;
            this.fieldsGroupBox.TabStop = false;
            this.fieldsGroupBox.Text = "Entity Fields";
            // 
            // entityDataGridView
            // 
            this.entityDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.entityDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityDataGridView.Location = new System.Drawing.Point(3, 16);
            this.entityDataGridView.Name = "entityDataGridView";
            this.entityDataGridView.Size = new System.Drawing.Size(249, 524);
            this.entityDataGridView.TabIndex = 0;
            // 
            // fieldsDataGridView
            // 
            this.fieldsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fieldsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsDataGridView.Location = new System.Drawing.Point(3, 16);
            this.fieldsDataGridView.Name = "fieldsDataGridView";
            this.fieldsDataGridView.Size = new System.Drawing.Size(249, 524);
            this.fieldsDataGridView.TabIndex = 0;
            // 
            // propretiesGroupBox
            // 
            this.propretiesGroupBox.Controls.Add(this.tableLayoutPanel2);
            this.propretiesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propretiesGroupBox.Location = new System.Drawing.Point(525, 3);
            this.propretiesGroupBox.Name = "propretiesGroupBox";
            this.propretiesGroupBox.Size = new System.Drawing.Size(519, 543);
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
            this.tableLayoutPanel2.Size = new System.Drawing.Size(513, 524);
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
            this.tableLayoutPanel3.Size = new System.Drawing.Size(507, 72);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // prefixGroupBox
            // 
            this.prefixGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prefixGroupBox.Location = new System.Drawing.Point(3, 3);
            this.prefixGroupBox.Name = "prefixGroupBox";
            this.prefixGroupBox.Size = new System.Drawing.Size(247, 66);
            this.prefixGroupBox.TabIndex = 0;
            this.prefixGroupBox.TabStop = false;
            this.prefixGroupBox.Text = "Suffix/Preffix";
            // 
            // addRemoveGroupBox
            // 
            this.addRemoveGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addRemoveGroupBox.Location = new System.Drawing.Point(256, 3);
            this.addRemoveGroupBox.Name = "addRemoveGroupBox";
            this.addRemoveGroupBox.Size = new System.Drawing.Size(248, 66);
            this.addRemoveGroupBox.TabIndex = 1;
            this.addRemoveGroupBox.TabStop = false;
            this.addRemoveGroupBox.Text = "Add/Remove";
            // 
            // nameGroupBox
            // 
            this.nameGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nameGroupBox.Location = new System.Drawing.Point(3, 81);
            this.nameGroupBox.Name = "nameGroupBox";
            this.nameGroupBox.Size = new System.Drawing.Size(507, 72);
            this.nameGroupBox.TabIndex = 1;
            this.nameGroupBox.TabStop = false;
            this.nameGroupBox.Text = "Preffix/Suffix Text";
            // 
            // logGroupBox
            // 
            this.logGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logGroupBox.Location = new System.Drawing.Point(3, 159);
            this.logGroupBox.Name = "logGroupBox";
            this.logGroupBox.Size = new System.Drawing.Size(507, 362);
            this.logGroupBox.TabIndex = 2;
            this.logGroupBox.TabStop = false;
            this.logGroupBox.Text = "Log";
            // 
            // MyPluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(1047, 574);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.entitiesGroupBox.ResumeLayout(false);
            this.fieldsGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.entityDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fieldsDataGridView)).EndInit();
            this.propretiesGroupBox.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
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
    }
}
