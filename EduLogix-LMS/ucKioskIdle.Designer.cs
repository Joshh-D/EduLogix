namespace EduLogix_LMS
{
    partial class ucKioskIdle
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pnlBackground = new Guna.UI2.WinForms.Guna2GradientPanel();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2PictureBox1 = new Guna.UI2.WinForms.Guna2PictureBox();
            pnlBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)guna2PictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pnlBackground
            // 
            pnlBackground.BorderRadius = 30;
            pnlBackground.Controls.Add(guna2PictureBox1);
            pnlBackground.Controls.Add(guna2HtmlLabel1);
            pnlBackground.CustomizableEdges = customizableEdges3;
            pnlBackground.Dock = DockStyle.Fill;
            pnlBackground.FillColor = Color.FromArgb(255, 254, 249);
            pnlBackground.FillColor2 = Color.FromArgb(216, 240, 150);
            pnlBackground.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            pnlBackground.Location = new Point(0, 0);
            pnlBackground.Margin = new Padding(5);
            pnlBackground.Name = "pnlBackground";
            pnlBackground.ShadowDecoration.CustomizableEdges = customizableEdges4;
            pnlBackground.Size = new Size(1244, 714);
            pnlBackground.TabIndex = 1;
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold);
            guna2HtmlLabel1.Location = new Point(1074, 174);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(77, 32);
            guna2HtmlLabel1.TabIndex = 2;
            guna2HtmlLabel1.Text = "Catalog";
            // 
            // guna2PictureBox1
            // 
            guna2PictureBox1.CustomizableEdges = customizableEdges1;
            guna2PictureBox1.ImageRotate = 0F;
            guna2PictureBox1.Location = new Point(21, 18);
            guna2PictureBox1.Name = "guna2PictureBox1";
            guna2PictureBox1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2PictureBox1.Size = new Size(928, 680);
            guna2PictureBox1.TabIndex = 3;
            guna2PictureBox1.TabStop = false;
            // 
            // ucKioskIdle
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Transparent;
            Controls.Add(pnlBackground);
            Name = "ucKioskIdle";
            Size = new Size(1244, 714);
            pnlBackground.ResumeLayout(false);
            pnlBackground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)guna2PictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2GradientPanel pnlBackground;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox1;
    }
}
