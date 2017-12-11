namespace DataAnalysisTool
{
    partial class frmTest
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbxProduct = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbxRev = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gbc = new System.Windows.Forms.GroupBox();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.lblinfo = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbxUL = new System.Windows.Forms.TextBox();
            this.tbxLL = new System.Windows.Forms.TextBox();
            this.tbxParameter = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbxParameter = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gbc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbxProduct);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 238);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Product";
            // 
            // lbxProduct
            // 
            this.lbxProduct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxProduct.FormattingEnabled = true;
            this.lbxProduct.Location = new System.Drawing.Point(3, 16);
            this.lbxProduct.Name = "lbxProduct";
            this.lbxProduct.Size = new System.Drawing.Size(194, 219);
            this.lbxProduct.TabIndex = 0;
            this.lbxProduct.SelectedIndexChanged += new System.EventHandler(this.lbxProduct_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbxRev);
            this.groupBox2.Location = new System.Drawing.Point(215, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 238);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Program Rev";
            // 
            // lbxRev
            // 
            this.lbxRev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxRev.FormattingEnabled = true;
            this.lbxRev.Location = new System.Drawing.Point(3, 16);
            this.lbxRev.Name = "lbxRev";
            this.lbxRev.Size = new System.Drawing.Size(194, 219);
            this.lbxRev.TabIndex = 0;
            this.lbxRev.SelectedIndexChanged += new System.EventHandler(this.lbxRev_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.gbc);
            this.panel1.Controls.Add(this.groupBox5);
            this.panel1.Controls.Add(this.lblinfo);
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(950, 603);
            this.panel1.TabIndex = 7;
            // 
            // gbc
            // 
            this.gbc.Controls.Add(this.chart);
            this.gbc.Location = new System.Drawing.Point(12, 294);
            this.gbc.Name = "gbc";
            this.gbc.Size = new System.Drawing.Size(908, 297);
            this.gbc.TabIndex = 6;
            this.gbc.TabStop = false;
            // 
            // chart
            // 
            chartArea1.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea1);
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart.Legends.Add(legend1);
            this.chart.Location = new System.Drawing.Point(3, 16);
            this.chart.Name = "chart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart.Series.Add(series1);
            this.chart.Size = new System.Drawing.Size(902, 278);
            this.chart.TabIndex = 0;
            this.chart.Text = "chart";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.listBox1);
            this.groupBox5.Location = new System.Drawing.Point(710, 317);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(200, 238);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Test Item";
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(3, 16);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(194, 219);
            this.listBox1.TabIndex = 0;
            // 
            // lblinfo
            // 
            this.lblinfo.Location = new System.Drawing.Point(15, 254);
            this.lblinfo.Name = "lblinfo";
            this.lblinfo.Size = new System.Drawing.Size(911, 26);
            this.lblinfo.TabIndex = 4;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tbxUL);
            this.groupBox4.Controls.Add(this.tbxLL);
            this.groupBox4.Controls.Add(this.tbxParameter);
            this.groupBox4.Controls.Add(this.checkBox1);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(672, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(254, 238);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Criteria";
            // 
            // tbxUL
            // 
            this.tbxUL.Location = new System.Drawing.Point(60, 138);
            this.tbxUL.Name = "tbxUL";
            this.tbxUL.Size = new System.Drawing.Size(89, 20);
            this.tbxUL.TabIndex = 7;
            // 
            // tbxLL
            // 
            this.tbxLL.Location = new System.Drawing.Point(60, 102);
            this.tbxLL.Name = "tbxLL";
            this.tbxLL.Size = new System.Drawing.Size(89, 20);
            this.tbxLL.TabIndex = 6;
            // 
            // tbxParameter
            // 
            this.tbxParameter.Location = new System.Drawing.Point(63, 41);
            this.tbxParameter.Name = "tbxParameter";
            this.tbxParameter.Size = new System.Drawing.Size(185, 20);
            this.tbxParameter.TabIndex = 5;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(63, 77);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "UL";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "LL";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Enable";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Test Item";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbxParameter);
            this.groupBox3.Location = new System.Drawing.Point(421, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 238);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Test Item";
            // 
            // lbxParameter
            // 
            this.lbxParameter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxParameter.FormattingEnabled = true;
            this.lbxParameter.Location = new System.Drawing.Point(3, 16);
            this.lbxParameter.Name = "lbxParameter";
            this.lbxParameter.Size = new System.Drawing.Size(194, 219);
            this.lbxParameter.TabIndex = 0;
            this.lbxParameter.SelectedIndexChanged += new System.EventHandler(this.lbxParameter_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(745, 256);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 603);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Name = "frmTest";
            this.Text = "Test";
            this.Load += new System.EventHandler(this.frmTest_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.gbc.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lbxProduct;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lbxRev;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tbxUL;
        private System.Windows.Forms.TextBox tbxLL;
        private System.Windows.Forms.TextBox tbxParameter;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox lbxParameter;
        private System.Windows.Forms.Label lblinfo;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox gbc;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.Button button1;
    }
}