namespace EduLogix_LMS
{
    partial class LMSDashboard
    {
        private System.ComponentModel.IContainer components = null;

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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            backgroundColor = new Guna.UI2.WinForms.Guna2GradientPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            pnlMainContent = new Guna.UI2.WinForms.Guna2Panel();
            pnlSidebarContainer = new Guna.UI2.WinForms.Guna2GradientPanel();
            pnlTemplate = new Guna.UI2.WinForms.Guna2GradientPanel();
            backgroundColor.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            pnlMainContent.SuspendLayout();
            SuspendLayout();
            // 
            // backgroundColor
            // 
            backgroundColor.BackColor = Color.Transparent;
            backgroundColor.Controls.Add(tableLayoutPanel1);
            backgroundColor.CustomizableEdges = customizableEdges7;
            backgroundColor.Dock = DockStyle.Fill;
            backgroundColor.FillColor = Color.FromArgb(48, 79, 99);
            backgroundColor.FillColor2 = Color.FromArgb(208, 228, 150);
            backgroundColor.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            backgroundColor.Location = new Point(0, 0);
            backgroundColor.Margin = new Padding(3, 4, 3, 4);
            backgroundColor.Name = "backgroundColor";
            backgroundColor.ShadowDecoration.CustomizableEdges = customizableEdges8;
            backgroundColor.Size = new Size(1402, 861);
            backgroundColor.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.Transparent;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 289F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(pnlMainContent, 1, 0);
            tableLayoutPanel1.Controls.Add(pnlSidebarContainer, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(3, 4, 3, 4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1402, 861);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // pnlMainContent
            // 
            pnlMainContent.BackColor = Color.Transparent;
            pnlMainContent.Controls.Add(pnlTemplate);
            pnlMainContent.CustomizableEdges = customizableEdges3;
            pnlMainContent.Dock = DockStyle.Fill;
            pnlMainContent.FillColor = Color.Transparent;
            pnlMainContent.Location = new Point(291, 10);
            pnlMainContent.Margin = new Padding(2, 10, 10, 10);
            pnlMainContent.Name = "pnlMainContent";
            pnlMainContent.ShadowDecoration.CustomizableEdges = customizableEdges4;
            pnlMainContent.Size = new Size(1101, 841);
            pnlMainContent.TabIndex = 6;
            // 
            // pnlSidebarContainer
            // 
            pnlSidebarContainer.CustomizableEdges = customizableEdges5;
            pnlSidebarContainer.Dock = DockStyle.Fill;
            pnlSidebarContainer.FillColor = Color.Transparent;
            pnlSidebarContainer.FillColor2 = Color.Transparent;
            pnlSidebarContainer.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal;
            pnlSidebarContainer.Location = new Point(0, 0);
            pnlSidebarContainer.Margin = new Padding(0);
            pnlSidebarContainer.Name = "pnlSidebarContainer";
            pnlSidebarContainer.ShadowDecoration.CustomizableEdges = customizableEdges6;
            pnlSidebarContainer.Size = new Size(289, 861);
            pnlSidebarContainer.TabIndex = 5;
            // 
            // pnlTemplate
            // 
            pnlTemplate.BorderRadius = 30;
            pnlTemplate.CustomizableEdges = customizableEdges1;
            pnlTemplate.Dock = DockStyle.Fill;
            pnlTemplate.FillColor = Color.FromArgb(255, 254, 249);
            pnlTemplate.FillColor2 = Color.FromArgb(216, 240, 150);
            pnlTemplate.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            pnlTemplate.Location = new Point(0, 0);
            pnlTemplate.Name = "pnlTemplate";
            pnlTemplate.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlTemplate.Size = new Size(1101, 841);
            pnlTemplate.TabIndex = 0;
            // 
            // LMSDashboard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Wheat;
            ClientSize = new Size(1402, 861);
            Controls.Add(backgroundColor);
            Margin = new Padding(3, 4, 3, 4);
            Name = "LMSDashboard";
            Text = "EduLogix-LMS";
            Load += LMSDashboard_Load;
            backgroundColor.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            pnlMainContent.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2GradientPanel backgroundColor;
        private Guna.UI2.WinForms.Guna2Panel pnlContentParent;
        private TableLayoutPanel tableLayoutPanel1;
        private Guna.UI2.WinForms.Guna2GradientPanel pnlSidebarContainer;
        private Guna.UI2.WinForms.Guna2Panel pnlMainContent;
        private Guna.UI2.WinForms.Guna2GradientPanel pnlTemplate;
    }
}
