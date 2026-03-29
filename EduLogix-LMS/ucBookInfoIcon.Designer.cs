namespace EduLogix_LMS
{
    partial class ucBookInfoIcon
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pnlContainer = new Guna.UI2.WinForms.Guna2Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            lblBookTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblAction = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pctbxBookCover = new PictureBox();
            pnlContainer.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pctbxBookCover).BeginInit();
            SuspendLayout();
            // 
            // pnlContainer
            // 
            pnlContainer.BackColor = Color.Transparent;
            pnlContainer.BorderRadius = 30;
            pnlContainer.Controls.Add(tableLayoutPanel1);
            pnlContainer.CustomizableEdges = customizableEdges1;
            pnlContainer.FillColor = Color.White;
            pnlContainer.Location = new Point(3, 0);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlContainer.Size = new Size(191, 280);
            pnlContainer.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(lblBookTitle, 0, 1);
            tableLayoutPanel1.Controls.Add(lblAction, 0, 2);
            tableLayoutPanel1.Controls.Add(pctbxBookCover, 0, 0);
            tableLayoutPanel1.Location = new Point(13, 13);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 71.6F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.4F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14F));
            tableLayoutPanel1.Size = new Size(167, 250);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // lblBookTitle
            // 
            lblBookTitle.Anchor = AnchorStyles.Top;
            lblBookTitle.BackColor = Color.Transparent;
            lblBookTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblBookTitle.Location = new Point(27, 182);
            lblBookTitle.Name = "lblBookTitle";
            lblBookTitle.Size = new Size(112, 19);
            lblBookTitle.TabIndex = 8;
            lblBookTitle.Text = "guna2HtmlLabel2";
            // 
            // lblAction
            // 
            lblAction.Anchor = AnchorStyles.Top;
            lblAction.BackColor = Color.Transparent;
            lblAction.Location = new Point(35, 218);
            lblAction.Name = "lblAction";
            lblAction.Size = new Size(97, 17);
            lblAction.TabIndex = 7;
            lblAction.Text = "guna2HtmlLabel1";
            // 
            // pctbxBookCover
            // 
            pctbxBookCover.Dock = DockStyle.Fill;
            pctbxBookCover.Image = Properties.Resources._default;
            pctbxBookCover.Location = new Point(3, 3);
            pctbxBookCover.Name = "pctbxBookCover";
            pctbxBookCover.Size = new Size(161, 173);
            pctbxBookCover.SizeMode = PictureBoxSizeMode.StretchImage;
            pctbxBookCover.TabIndex = 9;
            pctbxBookCover.TabStop = false;
            // 
            // ucBookInfoIcon
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlContainer);
            Name = "ucBookInfoIcon";
            Size = new Size(197, 283);
            pnlContainer.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pctbxBookCover).EndInit();
            ResumeLayout(false);
        }

        #endregion

        public Guna.UI2.WinForms.Guna2Panel pnlContainer;
        public TableLayoutPanel tableLayoutPanel1;
        public Guna.UI2.WinForms.Guna2HtmlLabel lblBookTitle;
        public Guna.UI2.WinForms.Guna2HtmlLabel lblAction;
        public PictureBox pctbxBookCover;
    }
}
