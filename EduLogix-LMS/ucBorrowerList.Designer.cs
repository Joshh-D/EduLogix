namespace EduLogix_LMS
{
    partial class ucBorrowerList
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pnlBackground = new Guna.UI2.WinForms.Guna2GradientPanel();
            pnlContainer = new Guna.UI2.WinForms.Guna2Panel();
            dgvBorrowerList = new Guna.UI2.WinForms.Guna2DataGridView();
            tableLayoutPanel1 = new TableLayoutPanel();
            btnFilter = new Guna.UI2.WinForms.Guna2GradientButton();
            txtbxSearchBar = new Guna.UI2.WinForms.Guna2TextBox();
            pnlBackground.SuspendLayout();
            pnlContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBorrowerList).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold);
            guna2HtmlLabel1.Location = new Point(33, 16);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(126, 32);
            guna2HtmlLabel1.TabIndex = 2;
            guna2HtmlLabel1.Text = "Borrower list";
            // 
            // pnlBackground
            // 
            pnlBackground.BorderRadius = 30;
            pnlBackground.Controls.Add(pnlContainer);
            pnlBackground.Controls.Add(tableLayoutPanel1);
            pnlBackground.Controls.Add(guna2HtmlLabel1);
            pnlBackground.CustomizableEdges = customizableEdges7;
            pnlBackground.Dock = DockStyle.Fill;
            pnlBackground.FillColor = Color.FromArgb(255, 254, 249);
            pnlBackground.FillColor2 = Color.FromArgb(216, 240, 150);
            pnlBackground.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            pnlBackground.Location = new Point(0, 0);
            pnlBackground.Margin = new Padding(5);
            pnlBackground.Name = "pnlBackground";
            pnlBackground.ShadowDecoration.CustomizableEdges = customizableEdges8;
            pnlBackground.Size = new Size(1244, 714);
            pnlBackground.TabIndex = 1;
            // 
            // pnlContainer
            // 
            pnlContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlContainer.BackColor = Color.Transparent;
            pnlContainer.BorderRadius = 30;
            pnlContainer.Controls.Add(dgvBorrowerList);
            pnlContainer.CustomizableEdges = customizableEdges1;
            pnlContainer.FillColor = Color.White;
            pnlContainer.Location = new Point(30, 130);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.ShadowDecoration.BorderRadius = 30;
            pnlContainer.ShadowDecoration.Color = Color.DimGray;
            pnlContainer.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlContainer.ShadowDecoration.Depth = 20;
            pnlContainer.ShadowDecoration.Enabled = true;
            pnlContainer.ShadowDecoration.Shadow = new Padding(1, 1, 5, 5);
            pnlContainer.Size = new Size(1191, 520);
            pnlContainer.TabIndex = 4;
            // 
            // dgvBorrowerList
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            dgvBorrowerList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvBorrowerList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvBorrowerList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvBorrowerList.ColumnHeadersHeight = 30;
            dgvBorrowerList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvBorrowerList.DefaultCellStyle = dataGridViewCellStyle3;
            dgvBorrowerList.GridColor = Color.FromArgb(231, 229, 255);
            dgvBorrowerList.Location = new Point(25, 25);
            dgvBorrowerList.Margin = new Padding(10);
            dgvBorrowerList.Name = "dgvBorrowerList";
            dgvBorrowerList.ReadOnly = true;
            dgvBorrowerList.RowHeadersVisible = false;
            dgvBorrowerList.RowHeadersWidth = 51;
            dgvBorrowerList.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgvBorrowerList.Size = new Size(1136, 485);
            dgvBorrowerList.TabIndex = 3;
            dgvBorrowerList.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            dgvBorrowerList.ThemeStyle.AlternatingRowsStyle.Font = null;
            dgvBorrowerList.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            dgvBorrowerList.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            dgvBorrowerList.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            dgvBorrowerList.ThemeStyle.BackColor = Color.White;
            dgvBorrowerList.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            dgvBorrowerList.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            dgvBorrowerList.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvBorrowerList.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            dgvBorrowerList.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvBorrowerList.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvBorrowerList.ThemeStyle.HeaderStyle.Height = 30;
            dgvBorrowerList.ThemeStyle.ReadOnly = true;
            dgvBorrowerList.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvBorrowerList.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvBorrowerList.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            dgvBorrowerList.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            dgvBorrowerList.ThemeStyle.RowsStyle.Height = 25;
            dgvBorrowerList.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dgvBorrowerList.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.BackColor = Color.Transparent;
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32.95154F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13.8325987F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 23.9647579F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15.3303967F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.1850224F));
            tableLayoutPanel1.Controls.Add(btnFilter, 1, 0);
            tableLayoutPanel1.Controls.Add(txtbxSearchBar, 0, 0);
            tableLayoutPanel1.Location = new Point(30, 73);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1191, 51);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // btnFilter
            // 
            btnFilter.Anchor = AnchorStyles.None;
            btnFilter.BackColor = Color.Transparent;
            btnFilter.BorderRadius = 10;
            btnFilter.CustomizableEdges = customizableEdges3;
            btnFilter.DisabledState.BorderColor = Color.DarkGray;
            btnFilter.DisabledState.CustomBorderColor = Color.DarkGray;
            btnFilter.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnFilter.DisabledState.FillColor2 = Color.FromArgb(169, 169, 169);
            btnFilter.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnFilter.FillColor = Color.FromArgb(182, 159, 150);
            btnFilter.FillColor2 = Color.FromArgb(236, 189, 171);
            btnFilter.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnFilter.ForeColor = Color.White;
            btnFilter.Image = Properties.Resources.icon_funnel;
            btnFilter.ImageOffset = new Point(-3, 0);
            btnFilter.Location = new Point(394, 8);
            btnFilter.Name = "btnFilter";
            btnFilter.ShadowDecoration.BorderRadius = 10;
            btnFilter.ShadowDecoration.Color = Color.DimGray;
            btnFilter.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnFilter.ShadowDecoration.Depth = 20;
            btnFilter.ShadowDecoration.Enabled = true;
            btnFilter.ShadowDecoration.Shadow = new Padding(1, 1, 5, 5);
            btnFilter.Size = new Size(158, 34);
            btnFilter.TabIndex = 3;
            btnFilter.Text = "Filter";
            btnFilter.Click += btnFilter_Click;
            // 
            // txtbxSearchBar
            // 
            txtbxSearchBar.BackColor = Color.Transparent;
            txtbxSearchBar.BorderRadius = 10;
            txtbxSearchBar.CustomizableEdges = customizableEdges5;
            txtbxSearchBar.DefaultText = "";
            txtbxSearchBar.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtbxSearchBar.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtbxSearchBar.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtbxSearchBar.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtbxSearchBar.Dock = DockStyle.Fill;
            txtbxSearchBar.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtbxSearchBar.Font = new Font("Segoe UI", 9F);
            txtbxSearchBar.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtbxSearchBar.Location = new Point(3, 4);
            txtbxSearchBar.Margin = new Padding(3, 4, 3, 4);
            txtbxSearchBar.Name = "txtbxSearchBar";
            txtbxSearchBar.PasswordChar = '\0';
            txtbxSearchBar.PlaceholderText = "🔎 Search";
            txtbxSearchBar.SelectedText = "";
            txtbxSearchBar.ShadowDecoration.BorderRadius = 10;
            txtbxSearchBar.ShadowDecoration.Color = Color.DimGray;
            txtbxSearchBar.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtbxSearchBar.ShadowDecoration.Depth = 20;
            txtbxSearchBar.ShadowDecoration.Enabled = true;
            txtbxSearchBar.ShadowDecoration.Shadow = new Padding(1, 1, 5, 5);
            txtbxSearchBar.Size = new Size(385, 43);
            txtbxSearchBar.TabIndex = 0;
            txtbxSearchBar.TextChanged += txtbxSearchBar_TextChanged;
            // 
            // ucBorrowerList
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlBackground);
            Name = "ucBorrowerList";
            Size = new Size(1244, 714);
            Load += ucBorrowerList_Load;
            pnlBackground.ResumeLayout(false);
            pnlBackground.PerformLayout();
            pnlContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvBorrowerList).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2GradientPanel pnlBackground;
        private TableLayoutPanel tableLayoutPanel1;
        private Guna.UI2.WinForms.Guna2GradientButton btnFilter;
        private Guna.UI2.WinForms.Guna2TextBox txtbxSearchBar;
        private Guna.UI2.WinForms.Guna2Panel pnlContainer;
        private Guna.UI2.WinForms.Guna2DataGridView dgvBorrowerList;
    }
}
