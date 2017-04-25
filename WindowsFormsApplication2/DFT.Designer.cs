namespace WindowsFormsApplication2
{
    partial class DFT
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DFT));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.grid = new System.Windows.Forms.ToolStripButton();
            this.lg_x = new System.Windows.Forms.ToolStripButton();
            this.lg_y = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.interv = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.удалитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.локальныйМасштабToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.глобальныйМасштабToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.grid,
            this.lg_x,
            this.lg_y,
            this.toolStripSeparator1,
            this.interv,
            this.toolStripButton2,
            this.toolStripButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(451, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // grid
            // 
            this.grid.Checked = true;
            this.grid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.grid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.grid.Image = ((System.Drawing.Image)(resources.GetObject("grid.Image")));
            this.grid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(24, 24);
            this.grid.Text = "Решетка";
            this.grid.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // lg_x
            // 
            this.lg_x.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lg_x.Enabled = false;
            this.lg_x.Image = ((System.Drawing.Image)(resources.GetObject("lg_x.Image")));
            this.lg_x.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.lg_x.Name = "lg_x";
            this.lg_x.Size = new System.Drawing.Size(31, 24);
            this.lg_x.Text = "LgX";
            this.lg_x.Click += new System.EventHandler(this.lgx_Click);
            // 
            // lg_y
            // 
            this.lg_y.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lg_y.Image = ((System.Drawing.Image)(resources.GetObject("lg_y.Image")));
            this.lg_y.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.lg_y.Name = "lg_y";
            this.lg_y.Size = new System.Drawing.Size(31, 24);
            this.lg_y.Text = "LgY";
            this.lg_y.Click += new System.EventHandler(this.lgy_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // interv
            // 
            this.interv.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.interv.Image = ((System.Drawing.Image)(resources.GetObject("interv.Image")));
            this.interv.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.interv.Name = "interv";
            this.interv.Size = new System.Drawing.Size(24, 24);
            this.interv.Text = "Интервал";
            this.interv.Click += new System.EventHandler(this.interv_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Enabled = false;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton2.Text = "Увеличить";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Enabled = false;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(24, 24);
            this.toolStripButton3.Text = "Уменьшить";
            // 
            // DFT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(451, 206);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DFT";
            this.Text = "Дискретное преобразование Фурье";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.close);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.position2);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.position1);
            this.Resize += new System.EventHandler(this.resize);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton interv;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton grid;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem удалитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem локальныйМасштабToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem глобальныйМасштабToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton lg_x;
        private System.Windows.Forms.ToolStripButton lg_y;
    }
}