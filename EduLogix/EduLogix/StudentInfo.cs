using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EduLogix
{
    public partial class StudentInfo : Form
    {
        private string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";
        private string currentStudentId = "";
        private Color currentThemeColor = Color.FromArgb(48, 79, 99);

        public StudentInfo()
        {
            InitializeComponent();
            InitializeUserOptionsPanel();
        }

        private void InitializeUserOptionsPanel()
        {
            if (userOptions != null)
            {
                userOptions.Visible = false;
                PositionUserOptionsPanel();
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

        private void StudentInfo_Load(object sender, EventArgs e)
        {
            if (!DesignMode && string.IsNullOrWhiteSpace(currentStudentId))
            {
                ApplyThemeToForm();
            }
        }

        public void LoadStudentInfo(string studentId)
        {
            currentStudentId = studentId;
            ApplyThemeToForm();
            LoadStudentData(studentId);
        }

        private void LoadStudentData(string studentId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT 
                                        rfid_number,
                                        student_id,
                                        name,
                                        phone,
                                        email,
                                        address,
                                        date_of_birth,
                                        grade,
                                        section,
                                        level,
                                        guardian_name,
                                        guardian_phone_number
                                    FROM reg_studentinfo
                                    WHERE student_id = @studentId";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        PopulateStudentControls(reader);
                    }
                    else
                    {
                        MessageBox.Show("Student record not found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading student data:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateStudentControls(MySqlDataReader reader)
        {
            try
            {
                // RFID Number (guna2TextBox8)
                if (guna2TextBox8 != null && reader["rfid_number"] != DBNull.Value)
                    guna2TextBox8.Text = reader["rfid_number"].ToString();

                // Student ID (guna2TextBox1)
                if (guna2TextBox1 != null && reader["student_id"] != DBNull.Value)
                    guna2TextBox1.Text = reader["student_id"].ToString();

                // Full Name (guna2TextBox2)
                if (guna2TextBox2 != null && reader["name"] != DBNull.Value)
                    guna2TextBox2.Text = reader["name"].ToString();

                // Grade (guna2TextBox3)
                if (guna2TextBox3 != null && reader["grade"] != DBNull.Value)
                    guna2TextBox3.Text = reader["grade"].ToString();

                // Phone Number (guna2TextBox4)
                if (guna2TextBox4 != null && reader["phone"] != DBNull.Value)
                    guna2TextBox4.Text = reader["phone"].ToString();

                // Guardian Name (guna2TextBox5)
                if (guna2TextBox5 != null && reader["guardian_name"] != DBNull.Value)
                    guna2TextBox5.Text = reader["guardian_name"].ToString();

                // Guardian Phone Number (guna2TextBox6)
                if (guna2TextBox6 != null && reader["guardian_phone_number"] != DBNull.Value)
                    guna2TextBox6.Text = reader["guardian_phone_number"].ToString();

                // Present Address (guna2TextBox7)
                if (guna2TextBox7 != null && reader["address"] != DBNull.Value)
                    guna2TextBox7.Text = reader["address"].ToString();

                // Section (guna2TextBox9)
                if (guna2TextBox9 != null && reader["section"] != DBNull.Value)
                    guna2TextBox9.Text = reader["section"].ToString();

                // Education/Level (guna2TextBox10)
                if (guna2TextBox10 != null && reader["level"] != DBNull.Value)
                    guna2TextBox10.Text = reader["level"].ToString();

                // Email (add appropriate control name if it exists)
                if (reader["email"] != DBNull.Value)
                {
                    // Assign to email control if available (e.g., guna2TextBox11 or similar)
                    // guna2TextBoxEmail.Text = reader["email"].ToString();
                }

                // Date of Birth (add appropriate control name if it exists)
                if (reader["date_of_birth"] != DBNull.Value)
                {
                    // Assign to date of birth control if available (e.g., guna2TextBox12 or similar)
                    // guna2TextBoxDateOfBirth.Text = ((DateTime)reader["date_of_birth"]).ToString("yyyy-MM-dd");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error populating student controls:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                        ApplyThemeToPanel(currentThemeColor);
                        ApplyThemeToButtons(currentThemeColor);
                    }
                    reader.Close();
                }

                BrandingHelper.ApplySchoolBranding(connectionString, schoolName, schoolLogo);
            }
            catch
            {
                // Silently fail, use default
            }
        }

        private void ApplyThemeToPanel(Color themeColor)
        {
            // Apply gradient to main panel if it exists
            foreach (Control control in this.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2GradientPanel gradientPanel)
                {
                    var darker = GetMainGradientTopColor(themeColor);
                    gradientPanel.FillColor = darker;
                    gradientPanel.FillColor2 = themeColor;
                }
            }
        }

        private void ApplyThemeToButtons(Color themeColor)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2Button btn)
                {
                    if (btn.Name == "Dashboard" || btn.Name == "Attendance" ||
                        btn.Name == "StudentsID" || btn.Name == "Accounts" ||
                        btn.Name == "Logs" || btn.Name == "Settings" || btn.Name == "Logout")
                    {
                        btn.FillColor = themeColor;
                        btn.ForeColor = GetContrastColor(themeColor);
                        
                        // Apply checked state
                        btn.CheckedState.FillColor = LightenColor(themeColor, 0.2f);
                        btn.CheckedState.ForeColor = GetContrastColor(LightenColor(themeColor, 0.2f));
                    }
                }
            }
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
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

        private void Dashboard_Click(object sender, EventArgs e)
        {
            DashboardForm dashboard = new DashboardForm();
            dashboard.Show();
            this.Hide();
        }

        private void Attendance_Click(object sender, EventArgs e)
        {
            AttendanceForm attendance = new AttendanceForm();
            attendance.Show();
            this.Hide();
        }

        private void StudentsID_Click(object sender, EventArgs e)
        {
            StudentIDForm studentID = new StudentIDForm();
            studentID.Show();
            this.Hide();
        }

        private void Accounts_Click(object sender, EventArgs e)
        {
            Users users = new Users();
            users.Show();
            this.Hide();
        }

        private void Logs_Click(object sender, EventArgs e)
        {
            Logs logs = new Logs();
            logs.Show();
            this.Hide();
        }

        #region ===== COLOR HELPER METHODS =====

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

        private Color GetMainGradientTopColor(Color themeColor)
        {
            var designerMainColor = Color.FromArgb(208, 228, 150);
            if (themeColor.ToArgb() == designerMainColor.ToArgb())
                return Color.FromArgb(48, 79, 99);

            return DarkenColor(themeColor, 0.35f);
        }

        #endregion

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
