
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Linq;

namespace EduLogix
{
    public partial class Logs : Form
    {
        private readonly string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";
        private Color currentThemeColor = Color.FromArgb(48, 79, 99);

        public Logs()
        {
            InitializeComponent();
            InitializeUserOptionsPanel();
            this.Load += Logs_Load;
        }

        private void InitializeUserOptionsPanel()
        {
            if (userOptions != null)
            {
                userOptions.Visible = false;
                userOptions.BringToFront();
            }

            if (settings != null)
            {
                settings.Click -= UserOptionsSettings_Click;
                settings.Click += UserOptionsSettings_Click;
            }

            var panelLogoutButton = userOptions != null ? userOptions.Controls["logout"] as Button : null;
            if (panelLogoutButton != null)
            {
                panelLogoutButton.Click -= UserOptionsLogout_Click;
                panelLogoutButton.Click += UserOptionsLogout_Click;
            }

            var panelkioskButton = userOptions != null ? userOptions.Controls["kiosk"] as Button : null;
            if (panelkioskButton != null)
            {
                panelkioskButton.Click -= UserOptionskiosk_Click;
                panelkioskButton.Click += UserOptionskiosk_Click;
            }

            WireOutsideClickHandler(this);
        }

        private void WireOutsideClickHandler(Control parent)
        {
            if (parent == null) return;

            bool isUserOptionsPanel = userOptions != null && parent == userOptions;
            bool isInsideUserOptionsPanel = IsInsideUserOptions(parent);

            foreach (Control child in parent.Controls)
            {
                WireOutsideClickHandler(child);
            }
        }

        private bool IsInsideUserOptions(Control control)
        {
            if (control == null || userOptions == null) return false;

            var current = control.Parent;
            while (current != null)
            {
                if (current == userOptions) return true;
                current = current.Parent;
            }

            return false;
        }

        private void UserOptionsSettings_Click(object sender, EventArgs e)
        {
            if (userOptions != null)
                userOptions.Visible = false;

            var form = new Settings();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = this.Location;
            form.Show();
            this.Hide();
        }

        private void UserOptionskiosk_Click(object sender, EventArgs e)
        {
            if (userOptions != null)
                userOptions.Visible = false;

            var form = new Kiosk();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = this.Location;
            form.Show();
        }

        private void UserOptionsLogout_Click(object sender, EventArgs e)
        {
            if (userOptions != null)
                userOptions.Visible = false;

            DialogResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var login = new Login();
                login.Show();
                this.Hide();
            }
        }

        private void Logs_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            LoadThemeFromDatabase();
            BrandingHelper.ApplySchoolBranding(connectionString, schoolName, schoolLogo);
            ApplyUserIdentityLabels();
            MarkActiveNav();
            InitializeDatePicker();
            LoadLogs();

            enabledatefilter.Checked = false;
            guna2DateTimePicker1.Enabled = false;
        }

        private void LoadUserProfileImage()
        {
            if (userProfile != null && !string.IsNullOrWhiteSpace(UserSession.UserName))
            {
                try
                {
                    userProfile.Image = UserProfileHelper.LoadUserProfile(UserSession.UserName);
                }
                catch
                {
                    // Silently fail; PictureBox will display default or nothing
                }
            }
        }

        private void InitializeDatePicker()
        {
            if (guna2DateTimePicker1 == null) return;

            guna2DateTimePicker1.ValueChanged -= Guna2DateTimePicker1_ValueChanged;
            guna2DateTimePicker1.Value = DateTime.Today;
            guna2DateTimePicker1.ValueChanged += Guna2DateTimePicker1_ValueChanged;
        }

        private void Guna2DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (enabledatefilter.Checked)
            {
                LoadLogs();
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            LoadLogs();
        }

        private void LoadLogs()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    DateTime selectedDate = (guna2DateTimePicker1 != null) ? guna2DateTimePicker1.Value.Date : DateTime.Today;
                    string searchText = guna2TextBox1 != null ? guna2TextBox1.Text.Trim().ToLower() : "";

                    var dt = new DataTable();

                    string sql = @"
                    SELECT `name`, `role`, `action`, `log_date`
                    FROM `reg_logs`
                    WHERE 1=1";

                    if (enabledatefilter.Checked)
                    {
                        sql += " AND DATE(`log_date`) = @selectedDate";
                    }

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        sql += @"
                  AND (
                      LOWER(name) LIKE @search
                      OR LOWER(role) LIKE @search
                      OR LOWER(action) LIKE @search
                  )";
                    }

                    sql += " ORDER BY log_date DESC";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        if (enabledatefilter.Checked)
                        {
                            cmd.Parameters.AddWithValue("@selectedDate", selectedDate);
                        }

                        if (!string.IsNullOrWhiteSpace(searchText))
                        {
                            cmd.Parameters.AddWithValue("@search", "%" + searchText + "%");
                        }

                        using (var da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    NormalizeLogRows(dt);

     

                    guna2DataGridView1.AutoGenerateColumns = true;
                    guna2DataGridView1.Columns.Clear();
                    guna2DataGridView1.DataSource = null;
                    guna2DataGridView1.DataSource = dt;
                    guna2DataGridView1.Refresh();
                }

                ConfigureDataGridView();
                ApplyLogGridColumns();
                ApplyStudentIdLikeGridSpacing();
                guna2DataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading logs:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NormalizeLogRows(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;

            foreach (DataRow row in dt.Rows)
            {
                string name = (row["name"] == DBNull.Value ? string.Empty : row["name"].ToString()).Trim();
                string role = (row["role"] == DBNull.Value ? string.Empty : row["role"].ToString()).Trim();
                string action = (row["action"] == DBNull.Value ? string.Empty : row["action"].ToString()).Trim();

                // Handle legacy rows where action type was written into role column.
                if (ShouldTreatRoleAsActionToken(role))
                {
                    row["role"] = string.IsNullOrWhiteSpace(name) ? "Registrar" : name;
                    row["action"] = string.IsNullOrWhiteSpace(action)
                        ? role
                        : string.Format("{0}: {1}", role, action);
                    continue;
                }

                // Fill missing action type labels for older rows.
                if (!action.Contains(":"))
                {
                    if (action.StartsWith("Image added", StringComparison.OrdinalIgnoreCase))
                        row["action"] = "KioskImageAdded: " + action;
                    else if (action.Equals("Logo updated", StringComparison.OrdinalIgnoreCase))
                        row["action"] = "LogoChanged: " + action;
                    else if (action.StartsWith("New theme color", StringComparison.OrdinalIgnoreCase))
                        row["action"] = "ThemeChanged: " + action;
                    else if (action.IndexOf("reset to designer defaults", StringComparison.OrdinalIgnoreCase) >= 0)
                        row["action"] = "SettingsReset: " + action;
                }
            }
        }

        private bool ShouldTreatRoleAsActionToken(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return false;

            // Known actual roles.
            if (role.Equals("Registrar", StringComparison.OrdinalIgnoreCase) ||
                role.Equals("Registrar Staff", StringComparison.OrdinalIgnoreCase) ||
                role.Equals("System", StringComparison.OrdinalIgnoreCase) ||
                role.Equals("Administrator", StringComparison.OrdinalIgnoreCase) ||
                role.Equals("Admin User", StringComparison.OrdinalIgnoreCase))
                return false;

            // Action tokens are usually PascalCase (e.g., ThemeChanged, KioskImageAdded).
            return !role.Contains(" ") && role.IndexOf("Changed", StringComparison.OrdinalIgnoreCase) >= 0
                   || role.IndexOf("Added", StringComparison.OrdinalIgnoreCase) >= 0
                   || role.IndexOf("Reset", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void ApplyLogGridColumns()
        {
            if (guna2DataGridView1.Columns.Contains("name"))
            {
                guna2DataGridView1.Columns["name"].HeaderText = "Name";
            }

            if (guna2DataGridView1.Columns.Contains("role"))
            {
                guna2DataGridView1.Columns["role"].HeaderText = "Role";
            }

            if (guna2DataGridView1.Columns.Contains("action"))
            {
                guna2DataGridView1.Columns["action"].HeaderText = "Action";
            }

            if (guna2DataGridView1.Columns.Contains("log_date"))
            {
                guna2DataGridView1.Columns["log_date"].HeaderText = "Date & Time";
                guna2DataGridView1.Columns["log_date"].DefaultCellStyle.Format = "yyyy-MM-dd hh:mm:ss tt";
            }
        }

        private void ConfigureDataGridView()
        {
            Color gridThemeColor = GetDataGridThemeColor();
            Color headerTextColor = GetContrastColor(gridThemeColor);
            Color selectionBackColor = LightenColor(gridThemeColor, 0.3f);
            Color selectionTextColor = GetContrastColor(selectionBackColor);

            guna2DataGridView1.EnableHeadersVisualStyles = false;
            guna2DataGridView1.ColumnHeadersHeight = 40;
            guna2DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;

            guna2DataGridView1.AllowUserToResizeRows = false;
            guna2DataGridView1.AllowUserToResizeColumns = false;
            guna2DataGridView1.AllowUserToDeleteRows = false;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.RowHeadersVisible = false;
            guna2DataGridView1.RowTemplate.Height = 35;

            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = gridThemeColor;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = headerTextColor;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Inter", 10, FontStyle.Bold);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = DarkenColor(gridThemeColor, 0.15f);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = headerTextColor;

            guna2DataGridView1.DefaultCellStyle.Font = new Font("Inter", 9, FontStyle.Regular);
            guna2DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            guna2DataGridView1.DefaultCellStyle.BackColor = Color.White;
            guna2DataGridView1.DefaultCellStyle.Padding = new Padding(2, 4, 2, 4);
            guna2DataGridView1.DefaultCellStyle.SelectionBackColor = selectionBackColor;
            guna2DataGridView1.DefaultCellStyle.SelectionForeColor = selectionTextColor;

            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            Color lightPatternColor = LightenColor(gridThemeColor, 0.7f);
            guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = lightPatternColor;
            guna2DataGridView1.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

            guna2DataGridView1.ThemeStyle.HeaderStyle.Height = 40;
            guna2DataGridView1.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            guna2DataGridView1.ThemeStyle.HeaderStyle.Font = new Font("Inter", 10, FontStyle.Bold);
            guna2DataGridView1.ThemeStyle.HeaderStyle.BackColor = gridThemeColor;
            guna2DataGridView1.ThemeStyle.HeaderStyle.ForeColor = headerTextColor;
            guna2DataGridView1.ThemeStyle.RowsStyle.Height = 35;
            guna2DataGridView1.ThemeStyle.RowsStyle.Font = new Font("Inter", 9, FontStyle.Regular);
            guna2DataGridView1.ThemeStyle.RowsStyle.SelectionBackColor = selectionBackColor;
            guna2DataGridView1.ThemeStyle.RowsStyle.SelectionForeColor = selectionTextColor;
            guna2DataGridView1.ThemeStyle.RowsStyle.BackColor = Color.White;
            guna2DataGridView1.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            guna2DataGridView1.ThemeStyle.ReadOnly = true;

            guna2DataGridView1.ClearSelection();
        }

        private void ApplyStudentIdLikeGridSpacing()
        {
            if (guna2DataGridView1 == null) return;

            guna2DataGridView1.ColumnHeadersHeight = 40;

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                row.Height = 35;
            }
        }

        private void LoadThemeFromDatabase()
        {
            if (DesignMode) return;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string query = "SELECT theme_red, theme_green, theme_blue FROM reg_theme WHERE id = 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int r = Convert.ToInt32(reader["theme_red"]);
                            int g = Convert.ToInt32(reader["theme_green"]);
                            int b = Convert.ToInt32(reader["theme_blue"]);
                            currentThemeColor = Color.FromArgb(r, g, b);
                            ApplyThemeColor(currentThemeColor);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void ApplyThemeColor(Color themeColor)
        {
            if (guna2GradientPanel1 != null)
            {
                var darker = GetMainGradientTopColor(themeColor);
                guna2GradientPanel1.FillColor = darker;
                guna2GradientPanel1.FillColor2 = themeColor;
            }

            if (guna2GradientPanel2 != null)
            {
                guna2GradientPanel2.FillColor = Color.White;
                guna2GradientPanel2.FillColor2 = LightenColor(themeColor, 0.9f);
            }

            ApplyThemeToNavButton(Dashboard, themeColor);
            ApplyThemeToNavButton(Attendance, themeColor);
            ApplyThemeToNavButton(StudentsID, themeColor);
            ApplyThemeToNavButton(guna2Button1, themeColor);
        }

        private void ApplyThemeToNavButton(Guna.UI2.WinForms.Guna2Button button, Color themeColor)
        {
            if (button == null) return;

            var normal = themeColor;
            var checkedColor = LightenColor(themeColor, 0.2f);

            button.FillColor = normal;
            button.ForeColor = GetContrastColor(normal);
            button.CheckedState.FillColor = checkedColor;
            button.CheckedState.ForeColor = GetContrastColor(checkedColor);
        }

        private void MarkActiveNav()
        {
            if (Dashboard != null)
            {
                Dashboard.Checked = false;
                Dashboard.FillColor = Color.Transparent;
            }
            if (Attendance != null)
            {
                Attendance.Checked = false;
                Attendance.FillColor = Color.Transparent;
            }
            if (StudentsID != null)
            {
                StudentsID.Checked = false;
                StudentsID.FillColor = Color.Transparent;
            }

            if (guna2Button1 != null)
            {
                guna2Button1.Checked = true;
                guna2Button1.FillColor = Color.White;
                guna2Button1.ForeColor = Color.Black;
                guna2Button1.CheckedState.FillColor = Color.White;
                guna2Button1.CheckedState.ForeColor = Color.Black;
            }
        }

        private Color DarkenColor(Color color, float amount)
        {
            int r = Math.Max(0, (int)(color.R * (1 - amount)));
            int g = Math.Max(0, (int)(color.G * (1 - amount)));
            int b = Math.Max(0, (int)(color.B * (1 - amount)));
            return Color.FromArgb(color.A, r, g, b);
        }

        private Color LightenColor(Color color, float amount)
        {
            int r = Math.Min(255, (int)(color.R + (255 - color.R) * amount));
            int g = Math.Min(255, (int)(color.G + (255 - color.G) * amount));
            int b = Math.Min(255, (int)(color.B + (255 - color.B) * amount));
            return Color.FromArgb(color.A, r, g, b);
        }

        private Color GetContrastColor(Color color)
        {
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return luminance > 0.5 ? Color.Black : Color.White;
        }

        private Color GetMainGradientTopColor(Color themeColor)
        {
            var designerMainColor = Color.FromArgb(208, 228, 150);
            if (themeColor.ToArgb() == designerMainColor.ToArgb())
                return Color.FromArgb(48, 79, 99);

            return DarkenColor(themeColor, 0.35f);
        }

        private Color GetDataGridThemeColor()
        {
            var designerMainColor = Color.FromArgb(208, 228, 150);
            if (currentThemeColor.ToArgb() == designerMainColor.ToArgb())
                return Color.FromArgb(48, 79, 99);

            return currentThemeColor;
        }

        private void Dashboard_Click(object sender, EventArgs e)
        {
            DashboardForm dashboardForm = new DashboardForm();
            dashboardForm.Show();
            this.Hide();
        }

        private void Attendance_Click(object sender, EventArgs e)
        {
            AttendanceForm attendanceForm = new AttendanceForm();
            attendanceForm.Show();
            this.Hide();
        }

        private void StudentsID_Click(object sender, EventArgs e)
        {
            StudentIDForm studentIDForm = new StudentIDForm();
            studentIDForm.Show();
            this.Hide();
        }

        private void userProfile_Click(object sender, EventArgs e)
        {
            if (userOptions == null) return;

            userOptions.Visible = !userOptions.Visible;
            if (userOptions.Visible)
                userOptions.BringToFront();
        }

        private void ApplyUserIdentityLabels()
        {
            string userNameValue = string.IsNullOrWhiteSpace(UserSession.UserName) ? "Username" : UserSession.UserName;
            string roleValue = string.IsNullOrWhiteSpace(UserSession.Role) ? "Role" : UserSession.Role;

            if (this.username != null)
            {
                this.username.Text = userNameValue;
            }
            else
            {
                var nameLabel = this.Controls.Find("username", true)
                    .OfType<Guna.UI2.WinForms.Guna2HtmlLabel>()
                    .FirstOrDefault();
                if (nameLabel == null)
                {
                    nameLabel = this.Controls.Find("guna2HtmlLabel17", true)
                        .OfType<Guna.UI2.WinForms.Guna2HtmlLabel>()
                        .FirstOrDefault();
                }
                if (nameLabel != null) nameLabel.Text = userNameValue;
            }

            if (this.role != null)
            {
                this.role.Text = roleValue;
            }
            else
            {
                var roleLabel = this.Controls.Find("role", true)
                    .OfType<Guna.UI2.WinForms.Guna2HtmlLabel>()
                    .FirstOrDefault();
                if (roleLabel == null)
                {
                    roleLabel = this.Controls.Find("guna2HtmlLabel13", true)
                        .OfType<Guna.UI2.WinForms.Guna2HtmlLabel>()
                        .FirstOrDefault();
                }
                if (roleLabel != null) roleLabel.Text = roleValue;
            }
        }

        private void enabledatefilter_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2DateTimePicker1 != null)
            {
                guna2DateTimePicker1.Enabled = enabledatefilter.Checked;
            }

            LoadLogs();
        }
    }
}