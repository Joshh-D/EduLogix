namespace EduLogix_LMS
{
    partial class Login
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            backgroundColor = new Guna.UI2.WinForms.Guna2GradientPanel();
            SuspendLayout();
            // 
            // backgroundColor
            // 
            backgroundColor.BackColor = Color.Transparent;
            backgroundColor.CustomizableEdges = customizableEdges1;
            backgroundColor.Dock = DockStyle.Fill;
            backgroundColor.FillColor = Color.FromArgb(48, 79, 99);
            backgroundColor.FillColor2 = Color.FromArgb(208, 228, 150);
            backgroundColor.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            backgroundColor.Location = new Point(0, 0);
            backgroundColor.Name = "backgroundColor";
            backgroundColor.ShadowDecoration.CustomizableEdges = customizableEdges2;
            backgroundColor.Size = new Size(588, 649);
            backgroundColor.TabIndex = 1;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(588, 649);
            Controls.Add(backgroundColor);
            Name = "Login";
            Text = "Login";
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2GradientPanel backgroundColor;
    }
}