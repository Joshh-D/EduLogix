using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace EduLogix
{
    public partial class SecurityAttendanceForm : Form
    {
        private readonly string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";
        private Color currentThemeColor = Color.FromArgb(48, 79, 99);
        private readonly string initialLevelFilter;
        private readonly string initialStatusFilter;

        public SecurityAttendanceForm() : this("All", "All")
        {
        }

        public SecurityAttendanceForm(string levelFilter, string statusFilter)
        {
            InitializeComponent();
            InitializeUserOptionsPanel();

            initialLevelFilter = string.IsNullOrWhiteSpace(levelFilter) ? "All" : levelFilter;
            initialStatusFilter = string.IsNullOrWhiteSpace(statusFilter) ? "All" : statusFilter;

            this.Load += AttendanceForm_Load;
        }

        private void InitializeUserOptionsPanel()
        {
            if (userOptions != null)
            {
                userOptions.Visible = false;
                PositionUserOptionsPanel();
                userOptions.BringToFront();
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

        private void PositionUserOptionsPanel()
        {
            if (userOptions == null || userProfile == null || userOptions.Parent == null) return;

            var parent = userOptions.Parent;
            int x = userProfile.Right + 8;
            int y = userProfile.Top + Math.Max(0, (userProfile.Height - userOptions.Height) / 2);

            if (x + userOptions.Width > parent.ClientSize.Width)
                x = Math.Max(0, userProfile.Left - userOptions.Width - 8);

            if (y + userOptions.Height > parent.ClientSize.Height)
                y = Math.Max(0, parent.ClientSize.Height - userOptions.Height - 8);

            userOptions.Location = new Point(Math.Max(0, x), Math.Max(0, y));
        }

        private void WireOutsideClickHandler(Control parent)
        {
            if (parent == null) return;

            bool isUserOptionsPanel = userOptions != null && parent == userOptions;
            bool isInsideUserOptionsPanel = IsInsideUserOptions(parent);

            if (!isUserOptionsPanel && !isInsideUserOptionsPanel)
            {
                parent.MouseDown -= OutsideUserOptions_MouseDown;
                parent.MouseDown += OutsideUserOptions_MouseDown;
            }

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

        private void OutsideUserOptions_MouseDown(object sender, MouseEventArgs e)
        {
            if (userOptions == null || !userOptions.Visible) return;

            Point clickPoint = System.Windows.Forms.Cursor.Position;
            bool clickedInsidePanel = userOptions.RectangleToScreen(userOptions.ClientRectangle).Contains(clickPoint);
            bool clickedUserProfile = userProfile != null && userProfile.RectangleToScreen(userProfile.ClientRectangle).Contains(clickPoint);

            if (!clickedInsidePanel && !clickedUserProfile)
                userOptions.Visible = false;
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

        private void UserOptionsLogout_Click(object sender, EventArgs e)
        {
            if (userOptions != null)
                userOptions.Visible = false;

            Logout_Click(sender, e);
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

        private void AttendanceForm_Load(object sender, EventArgs e)
        {
            guna2DataGridView1.ClearSelection();
            ApplyThemeToForm();
            BrandingHelper.ApplySchoolBranding(connectionString, schoolName, schoolLogo);
            MarkActiveNav();  // ADD THIS LINE
            InitializeFilters();
            ApplyInitialFilters();
            LoadAttendanceData(
                attendanceStudentSearch?.Text?.Trim() ?? "",
                attendanceCombobox1?.SelectedItem?.ToString() ?? "All",
                attendanceCombobox2?.SelectedItem?.ToString() ?? "All");
            ConfigureDataGridView();
        }

        private void ApplyInitialFilters()
        {
            if (attendanceCombobox1 != null && attendanceCombobox1.Items.Contains(initialLevelFilter))
            {
                attendanceCombobox1.SelectedItem = initialLevelFilter;
            }
            else if (attendanceCombobox1 != null)
            {
                attendanceCombobox1.SelectedIndex = 0;
            }

            if (attendanceCombobox2 != null && attendanceCombobox2.Items.Contains(initialStatusFilter))
            {
                attendanceCombobox2.SelectedItem = initialStatusFilter;
            }
            else if (attendanceCombobox2 != null)
            {
                attendanceCombobox2.SelectedIndex = 0;
            }
        }

        private void InitializeFilters()
        {
            // Wire search textbox
            if (attendanceStudentSearch != null)
            {
                attendanceStudentSearch.TextChanged += (s, args) => ApplyFilters();
            }

            // Initialize level filter combobox
            if (attendanceCombobox1 != null)
            {
                attendanceCombobox1.Items.Clear();
                attendanceCombobox1.Items.AddRange(new[] { "All", "Elementary", "Junior", "Senior" });
                attendanceCombobox1.SelectedIndex = 0;
                attendanceCombobox1.SelectedIndexChanged += (s, args) => ApplyFilters();
            }

            // Initialize status filter combobox
            if (attendanceCombobox2 != null)
            {
                attendanceCombobox2.Items.Clear();
                attendanceCombobox2.Items.AddRange(new[] { "All", "In Premises", "Departed" });
                attendanceCombobox2.SelectedIndex = 0;
                attendanceCombobox2.SelectedIndexChanged += (s, args) => ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            string searchText = attendanceStudentSearch?.Text?.Trim() ?? "";
            string levelFilter = attendanceCombobox1?.SelectedItem?.ToString() ?? "All";
            string statusFilter = attendanceCombobox2?.SelectedItem?.ToString() ?? "All";

            LoadAttendanceData(searchText, levelFilter, statusFilter);
        }

        private void ApplyThemeToForm()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT theme_red, theme_green, theme_blue FROM reg_theme WHERE id = 1";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int r = Convert.ToInt32(reader["theme_red"]);
                        int g = Convert.ToInt32(reader["theme_green"]);
                        int b = Convert.ToInt32(reader["theme_blue"]);
                        currentThemeColor = Color.FromArgb(r, g, b);

                        ApplyThemeColor(currentThemeColor);
                    }
                    reader.Close();
                }
            }
            catch
            {
                // Silently fail, use default
            }
        }

        private void ApplyThemeColor(Color themeColor)
        {
            // outer gradient background
            if (guna2GradientPanel1 != null)
            {
                var darker = GetMainGradientTopColor(themeColor);
                guna2GradientPanel1.FillColor = darker;
                guna2GradientPanel1.FillColor2 = themeColor;
            }

            // inner content panel
            if (guna2GradientPanel2 != null)
            {
                guna2GradientPanel2.FillColor = Color.White;
                guna2GradientPanel2.FillColor2 = LightenColor(themeColor, 0.9f);
            }

            // nav buttons
            ApplyNavTheme(Dashboard, themeColor);
            ApplyNavTheme(Attendance, themeColor);

            // control boxes
            if (guna2ControlBox4 != null)
            {
                guna2ControlBox4.FillColor = Color.Transparent;
                guna2ControlBox4.IconColor = Color.White;
            }
            if (guna2ControlBox5 != null)
            {
                guna2ControlBox5.FillColor = Color.Transparent;
                guna2ControlBox5.IconColor = Color.White;
            }
        }

        private void ApplyNavTheme(Guna.UI2.WinForms.Guna2Button btn, Color themeColor)
        {
            if (btn == null) return;
            Color normal = themeColor;
            Color checkedColor = LightenColor(themeColor, 0.2f);

            btn.FillColor = normal;
            btn.ForeColor = GetContrastColor(normal);
            btn.CheckedState.FillColor = checkedColor;
            btn.CheckedState.ForeColor = GetContrastColor(checkedColor);
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

        private void ConfigureDataGridView()
        {
            Color gridThemeColor = GetDataGridThemeColor();

            guna2DataGridView1.EnableHeadersVisualStyles = false;
            guna2DataGridView1.ColumnHeadersHeight = 40;

            // Header styling with theme color
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = gridThemeColor;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Inter", 10, FontStyle.Bold);

            // Data cell styling
            guna2DataGridView1.DefaultCellStyle.Font = new Font("Inter", 9, FontStyle.Regular);
            guna2DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            guna2DataGridView1.DefaultCellStyle.BackColor = Color.White;

            // Selection styling with THEME COLOR (not blue)
            guna2DataGridView1.DefaultCellStyle.SelectionBackColor = LightenColor(gridThemeColor, 0.3f);
            guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = DarkenColor(gridThemeColor, 0.15f);

            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            // Alternating row colors with pattern - using theme color (UPDATED)
            Color lightPatternColor = LightenColor(gridThemeColor, 0.7f);
            guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = lightPatternColor;
            guna2DataGridView1.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

            // Rename columns to friendly names
            RenameColumns();

            guna2DataGridView1.ClearSelection();
        }

        private void RenameColumns()
        {
            if (guna2DataGridView1.Columns.Contains("rfid_number"))
                guna2DataGridView1.Columns["rfid_number"].HeaderText = "RFID Number";

            if (guna2DataGridView1.Columns.Contains("student_num"))
                guna2DataGridView1.Columns["student_num"].HeaderText = "Student Number";

            if (guna2DataGridView1.Columns.Contains("student_name"))
                guna2DataGridView1.Columns["student_name"].HeaderText = "Student Name";

            if (guna2DataGridView1.Columns.Contains("grade_level"))
                guna2DataGridView1.Columns["grade_level"].HeaderText = "Grade Level";

            if (guna2DataGridView1.Columns.Contains("section"))
                guna2DataGridView1.Columns["section"].HeaderText = "Section";

            if (guna2DataGridView1.Columns.Contains("status"))
                guna2DataGridView1.Columns["status"].HeaderText = "Status";

            if (guna2DataGridView1.Columns.Contains("date_and_time"))
                guna2DataGridView1.Columns["date_and_time"].HeaderText = "Date & Time";

            if (guna2DataGridView1.Columns.Contains("education"))
                guna2DataGridView1.Columns["education"].HeaderText = "Education Level";
        }

        private Color LightenColor(Color color, float amount)
        {
            int r = Math.Min(255, (int)(color.R + (255 - color.R) * amount));
            int g = Math.Min(255, (int)(color.G + (255 - color.G) * amount));
            int b = Math.Min(255, (int)(color.B + (255 - color.B) * amount));
            return Color.FromArgb(color.A, r, g, b);
        }

        private Color DarkenColor(Color color, float amount)
        {
            int r = Math.Max(0, (int)(color.R * (1 - amount)));
            int g = Math.Max(0, (int)(color.G * (1 - amount)));
            int b = Math.Max(0, (int)(color.B * (1 - amount)));
            return Color.FromArgb(color.A, r, g, b);
        }

        private Color GetContrastColor(Color color)
        {
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return luminance > 0.5 ? Color.Black : Color.White;
        }

        private void LoadAttendanceData(string searchText = "", string levelFilter = "All", string statusFilter = "All")
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM reg_attendance_live WHERE 1=1";

                    if (levelFilter != "All")
                    {
                        string levelValue = ConvertLevelToDatabase(levelFilter);
                        query += " AND education = @level";
                    }

                    if (statusFilter != "All")
                    {
                        query += " AND status = @status";
                    }

                    if (!string.IsNullOrEmpty(searchText))
                    {
                        query += " AND (student_name LIKE @search OR student_num LIKE @search OR rfid_number LIKE @search)";
                    }

                    query += " ORDER BY student_name ASC";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    if (levelFilter != "All")
                        cmd.Parameters.AddWithValue("@level", ConvertLevelToDatabase(levelFilter));

                    if (statusFilter != "All")
                        cmd.Parameters.AddWithValue("@status", statusFilter);

                    if (!string.IsNullOrEmpty(searchText))
                        cmd.Parameters.AddWithValue("@search", "%" + searchText + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    guna2DataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading attendance data:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ConvertLevelToDatabase(string level)
        {
            if (level == "Elementary") return "elementary";
            if (level == "Junior") return "junior";
            if (level == "Senior") return "senior";
            return "";
        }

        private void Dashboard_Click(object sender, EventArgs e)
        {
            SecurityDashboardForm dashboard = new SecurityDashboardForm();
            dashboard.Show();
            this.Hide();
        }

        private void StudentsID_Click(object sender, EventArgs e)
        {
            StudentIDForm studentID = new StudentIDForm();
            studentID.Show();
            this.Hide();
        }

        private void Logs_Click(object sender, EventArgs e)
        {
            Logs logs = new Logs();
            logs.Show();
            this.Hide();
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                Login login = new Login();
                login.Show();
                this.Close();
            }
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Empty - DataGridView is read-only
        }

        // Add this method if it doesn't exist, or update MarkActiveNav if it does
        private void MarkActiveNav()
        {
            // Uncheck others and make them transparent
            if (Dashboard != null)
            {
                Dashboard.Checked = false;
                Dashboard.FillColor = Color.Transparent;
            }

            // Active button white
            if (Attendance != null)
            {
                Attendance.Checked = true;
                Attendance.FillColor = Color.White;
                Attendance.ForeColor = Color.Black;
                Attendance.CheckedState.FillColor = Color.White;
                Attendance.CheckedState.ForeColor = Color.Black;
            }
        }

        private void userProfile_Click(object sender, EventArgs e)
        {
            if (userOptions == null) return;

            PositionUserOptionsPanel();
            userOptions.Visible = !userOptions.Visible;
            if (userOptions.Visible)
                userOptions.BringToFront();
        }
    }
}