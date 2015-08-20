namespace DataAnalysisTool
{
    partial class GoldenSample
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goldenDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGoldenFilecsvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewGoldenDatadbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importGoldenDatacsvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toleranceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToleranceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToleranceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvList = new System.Windows.Forms.TreeView();
            this.gbInfo = new System.Windows.Forms.GroupBox();
            this.tbxInfo = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.goldenDataToolStripMenuItem,
            this.toleranceToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1016, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // goldenDataToolStripMenuItem
            // 
            this.goldenDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openGoldenFilecsvToolStripMenuItem,
            this.viewGoldenDatadbToolStripMenuItem,
            this.importGoldenDatacsvToolStripMenuItem});
            this.goldenDataToolStripMenuItem.Name = "goldenDataToolStripMenuItem";
            this.goldenDataToolStripMenuItem.Size = new System.Drawing.Size(84, 20);
            this.goldenDataToolStripMenuItem.Text = "Golden Data";
            // 
            // openGoldenFilecsvToolStripMenuItem
            // 
            this.openGoldenFilecsvToolStripMenuItem.Name = "openGoldenFilecsvToolStripMenuItem";
            this.openGoldenFilecsvToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.openGoldenFilecsvToolStripMenuItem.Text = "Open Golden File (.csv)";
            // 
            // viewGoldenDatadbToolStripMenuItem
            // 
            this.viewGoldenDatadbToolStripMenuItem.Name = "viewGoldenDatadbToolStripMenuItem";
            this.viewGoldenDatadbToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.viewGoldenDatadbToolStripMenuItem.Text = "View Golden Data (db)";
            // 
            // importGoldenDatacsvToolStripMenuItem
            // 
            this.importGoldenDatacsvToolStripMenuItem.Name = "importGoldenDatacsvToolStripMenuItem";
            this.importGoldenDatacsvToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.importGoldenDatacsvToolStripMenuItem.Text = "Import Golden Data (.csv)";
            this.importGoldenDatacsvToolStripMenuItem.Click += new System.EventHandler(this.importGoldenDatacsvToolStripMenuItem_Click);
            // 
            // toleranceToolStripMenuItem
            // 
            this.toleranceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToleranceToolStripMenuItem,
            this.addToleranceToolStripMenuItem});
            this.toleranceToolStripMenuItem.Name = "toleranceToolStripMenuItem";
            this.toleranceToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.toleranceToolStripMenuItem.Text = "Tolerance";
            // 
            // viewToleranceToolStripMenuItem
            // 
            this.viewToleranceToolStripMenuItem.Name = "viewToleranceToolStripMenuItem";
            this.viewToleranceToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.viewToleranceToolStripMenuItem.Text = "View Tolerance";
            this.viewToleranceToolStripMenuItem.Click += new System.EventHandler(this.viewToleranceToolStripMenuItem_Click);
            // 
            // addToleranceToolStripMenuItem
            // 
            this.addToleranceToolStripMenuItem.Name = "addToleranceToolStripMenuItem";
            this.addToleranceToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.addToleranceToolStripMenuItem.Text = "Add/Update Tolerance";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.topicToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // topicToolStripMenuItem
            // 
            this.topicToolStripMenuItem.Name = "topicToolStripMenuItem";
            this.topicToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.topicToolStripMenuItem.Text = "Topic";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbInfo);
            this.splitContainer1.Panel1.Controls.Add(this.tvList);
            this.splitContainer1.Size = new System.Drawing.Size(1016, 577);
            this.splitContainer1.SplitterDistance = 338;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 1;
            // 
            // tvList
            // 
            this.tvList.Dock = System.Windows.Forms.DockStyle.Top;
            this.tvList.Location = new System.Drawing.Point(0, 0);
            this.tvList.Name = "tvList";
            this.tvList.Size = new System.Drawing.Size(334, 277);
            this.tvList.TabIndex = 0;
            // 
            // gbInfo
            // 
            this.gbInfo.Controls.Add(this.tbxInfo);
            this.gbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbInfo.Location = new System.Drawing.Point(0, 277);
            this.gbInfo.Name = "gbInfo";
            this.gbInfo.Size = new System.Drawing.Size(334, 296);
            this.gbInfo.TabIndex = 1;
            this.gbInfo.TabStop = false;
            this.gbInfo.Text = "Information";
            // 
            // tbxInfo
            // 
            this.tbxInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxInfo.Location = new System.Drawing.Point(3, 16);
            this.tbxInfo.Multiline = true;
            this.tbxInfo.Name = "tbxInfo";
            this.tbxInfo.Size = new System.Drawing.Size(328, 277);
            this.tbxInfo.TabIndex = 0;
            // 
            // GoldenSample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 601);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "GoldenSample";
            this.Text = "Golden Sample";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gbInfo.ResumeLayout(false);
            this.gbInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goldenDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openGoldenFilecsvToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewGoldenDatadbToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importGoldenDatacsvToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toleranceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToleranceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToleranceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topicToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox gbInfo;
        private System.Windows.Forms.TextBox tbxInfo;
        private System.Windows.Forms.TreeView tvList;
    }
}