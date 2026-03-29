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
using System.IO;
using System.Text.RegularExpressions;

namespace EduLogix
{
    public partial class StudentInfo : Form
    {
        private string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";
        private string currentStudentId = "";
        private string currentImagePath = "";
        private Color currentThemeColor = Color.FromArgb(48, 79, 99);
        private StudentInfoState loadedState;

        private sealed class StudentInfoState
        {
            public string Rfid;
            public string StudentId;
            public string FullName;
            public string StudentContact;
            public string Level;
            public string Grade;
            public string Section;
            public DateTime? BirthDate;
            public string GuardianName;
            public string GuardianContact;
            public string Address;
            public string ImagePath;
        }

        public StudentInfo()
        {
            InitializeComponent();
            InitializeUserOptionsPanel();
            ApplyUserIdentityLabels();
            InitializeSectionComboBox();
            InitializeBirthDatePicker();
            ConfigureInputConstraints();

            if (editBtn != null) editBtn.Click += editBtn_Click;
            if (saveBtn != null) saveBtn.Click += saveBtn_Click;
            if (archiveBtn != null)
            {
                archiveBtn.Click -= archiveBtn_Click_1;
                archiveBtn.Click += archiveBtn_Click_1;
            }

            var resetInfoButton = this.Controls.Find("resetinfo", true).FirstOrDefault();
            if (resetInfoButton != null)
            {
                resetInfoButton.Click -= resetinfo_Click;
                resetInfoButton.Click += resetinfo_Click;
            }

            SetEditMode(false);
        }

        private void resetinfo_Click(object sender, EventArgs e)
        {
            if (loadedState == null)
            {
                MessageBox.Show("No loaded student info to reset.", "Reset Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ApplyState(loadedState);
        }

        private void ConfigureInputConstraints()
        {
            if (rfidNum != null) rfidNum.MaxLength = 20;
            if (studentID != null) studentID.MaxLength = 10;
            if (studentName != null) studentName.MaxLength = 255;
            if (contactNum != null) contactNum.MaxLength = 14;
            if (level != null) level.MaxLength = 20;
            if (studentGrade != null) studentGrade.MaxLength = 2;
            if (guardianName != null) guardianName.MaxLength = 255;
            if (guardianNum != null) guardianNum.MaxLength = 14;
            if (address != null) address.MaxLength = 255;
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
                                        image_path,
                                        rfid_number,
                                        student_id,
                                        name,
                                        phone_number,                                 
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
                if (rfidNum != null && reader["rfid_number"] != DBNull.Value)
                    rfidNum.Text = reader["rfid_number"].ToString();

                // Student ID (guna2TextBox1)
                if (studentID != null && reader["student_id"] != DBNull.Value)
                    studentID.Text = reader["student_id"].ToString();

                // Full Name (guna2TextBox2)
                if (studentName != null && reader["name"] != DBNull.Value)
                    studentName.Text = reader["name"].ToString();

                // Grade (guna2TextBox3)
                if (studentGrade != null && reader["grade"] != DBNull.Value)
                    studentGrade.Text = reader["grade"].ToString();

                // Phone Number (guna2TextBox4)
                if (contactNum != null && reader["phone_number"] != DBNull.Value)
                    contactNum.Text = reader["phone_number"].ToString();

                // Guardian Name (guna2TextBox5)
                if (guardianName != null && reader["guardian_name"] != DBNull.Value)
                    guardianName.Text = reader["guardian_name"].ToString();

                // Guardian Phone Number (guna2TextBox6)
                if (guardianNum != null && reader["guardian_phone_number"] != DBNull.Value)
                    guardianNum.Text = reader["guardian_phone_number"].ToString();

                // Present Address (guna2TextBox7)
                if (address != null && reader["address"] != DBNull.Value)
                    address.Text = reader["address"].ToString();

                // Section
                if (sectionComboBox != null && reader["section"] != DBNull.Value)
                    sectionComboBox.Text = reader["section"].ToString();

                // Education/Level (guna2TextBox10)
                if (level != null && reader["level"] != DBNull.Value)
                    level.Text = reader["level"].ToString();

                // Date of Birth
                if (reader["date_of_birth"] != DBNull.Value)
                {
                    SetBirthDate(Convert.ToDateTime(reader["date_of_birth"]));
                }
                else
                {
                    SetBirthDate(null);
                }

                currentImagePath = reader["image_path"] == DBNull.Value
                    ? string.Empty
                    : reader["image_path"].ToString();

                LoadStudentPicture(currentImagePath);

                loadedState = CaptureCurrentState();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error populating student controls:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private StudentInfoState CaptureCurrentState()
        {
            return new StudentInfoState
            {
                Rfid = rfidNum != null ? rfidNum.Text : string.Empty,
                StudentId = studentID != null ? studentID.Text : string.Empty,
                FullName = studentName != null ? studentName.Text : string.Empty,
                StudentContact = contactNum != null ? contactNum.Text : string.Empty,
                Level = level != null ? level.Text : string.Empty,
                Grade = studentGrade != null ? studentGrade.Text : string.Empty,
                Section = sectionComboBox != null ? sectionComboBox.Text : string.Empty,
                BirthDate = birthDatePicker != null && birthDatePicker.CustomFormat != " " ? (DateTime?)birthDatePicker.Value.Date : null,
                GuardianName = guardianName != null ? guardianName.Text : string.Empty,
                GuardianContact = guardianNum != null ? guardianNum.Text : string.Empty,
                Address = address != null ? address.Text : string.Empty,
                ImagePath = currentImagePath
            };
        }

        private void ApplyState(StudentInfoState state)
        {
            if (state == null) return;

            if (rfidNum != null) rfidNum.Text = state.Rfid;
            if (studentID != null) studentID.Text = state.StudentId;
            if (studentName != null) studentName.Text = state.FullName;
            if (contactNum != null) contactNum.Text = state.StudentContact;
            if (level != null) level.Text = state.Level;
            if (studentGrade != null) studentGrade.Text = state.Grade;
            if (sectionComboBox != null) sectionComboBox.Text = state.Section;
            if (guardianName != null) guardianName.Text = state.GuardianName;
            if (guardianNum != null) guardianNum.Text = state.GuardianContact;
            if (address != null) address.Text = state.Address;

            SetBirthDate(state.BirthDate);

            currentImagePath = state.ImagePath;
            LoadStudentPicture(currentImagePath);
        }

        private void LoadStudentPicture(string imagePath)
        {
            try
            {
                if (studentPic == null) return;

                if (string.IsNullOrWhiteSpace(imagePath))
                {
                    studentPic.Image = Properties.Resources.student;
                    return;
                }

                string resolvedPath = imagePath;
                if (!Path.IsPathRooted(resolvedPath))
                {
                    resolvedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
                }

                if (!File.Exists(resolvedPath))
                {
                    studentPic.Image = Properties.Resources.student;
                    return;
                }

                using (var fs = new FileStream(resolvedPath, FileMode.Open, FileAccess.Read))
                {
                    studentPic.Image = Image.FromStream(fs);
                }
            }
            catch
            {
                if (studentPic != null)
                    studentPic.Image = Properties.Resources.student;
            }
        }

        private void SetEditMode(bool isEditMode)
        {
            if (rfidNum != null) rfidNum.ReadOnly = !isEditMode;
            if (studentID != null) studentID.ReadOnly = !isEditMode;
            if (studentName != null) studentName.ReadOnly = !isEditMode;
            if (contactNum != null) contactNum.ReadOnly = !isEditMode;
            if (level != null) level.ReadOnly = !isEditMode;
            if (studentGrade != null) studentGrade.ReadOnly = !isEditMode;
            if (sectionComboBox != null) sectionComboBox.Enabled = isEditMode;
            if (birthDatePicker != null) birthDatePicker.Enabled = isEditMode;
            if (guardianName != null) guardianName.ReadOnly = !isEditMode;
            if (guardianNum != null) guardianNum.ReadOnly = !isEditMode;
            if (address != null) address.ReadOnly = !isEditMode;

            if (saveBtn != null) saveBtn.Enabled = isEditMode;
            if (editBtn != null) editBtn.Enabled = !isEditMode;
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            SetEditMode(true);
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentStudentId)) return;

            string validationMessage;
            if (!ValidateStudentInfo(out validationMessage))
            {
                MessageBox.Show(validationMessage, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string previousStudentId = currentStudentId;
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string query = @"UPDATE reg_studentinfo
                                           SET rfid_number = @rfid,
                                               student_id = @studentId,
                                               name = @name,
                                               phone_number = @phone,
                                               level = @level,
                                               grade = @grade,
                                               section = @section,
                                               date_of_birth = @dateOfBirth,
                                               guardian_name = @guardianName,
                                               guardian_phone_number = @guardianPhone,
                                               address = @address
                                           WHERE student_id = @currentStudentId";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@rfid", rfidNum?.Text?.Trim() ?? string.Empty);
                        cmd.Parameters.AddWithValue("@studentId", studentID?.Text?.Trim() ?? string.Empty);
                        cmd.Parameters.AddWithValue("@name", studentName?.Text?.Trim() ?? string.Empty);
                        cmd.Parameters.AddWithValue("@phone", contactNum?.Text?.Trim() ?? string.Empty);
                        cmd.Parameters.AddWithValue("@level", level?.Text?.Trim() ?? string.Empty);
                        cmd.Parameters.AddWithValue("@grade", studentGrade?.Text?.Trim() ?? string.Empty);
                        cmd.Parameters.AddWithValue("@section", sectionComboBox?.Text?.Trim() ?? string.Empty);
                        cmd.Parameters.AddWithValue("@dateOfBirth", GetBirthDateValue());
                        cmd.Parameters.AddWithValue("@guardianName", guardianName?.Text?.Trim() ?? string.Empty);
                        cmd.Parameters.AddWithValue("@guardianPhone", guardianNum?.Text?.Trim() ?? string.Empty);
                        cmd.Parameters.AddWithValue("@address", address?.Text?.Trim() ?? string.Empty);
                        cmd.Parameters.AddWithValue("@currentStudentId", currentStudentId);

                        cmd.ExecuteNonQuery();
                    }
                }

                currentStudentId = studentID?.Text?.Trim() ?? currentStudentId;
                LogAction("StudentUpdated", $"Updated student info: {studentName?.Text?.Trim()} ({previousStudentId} -> {currentStudentId})");
                loadedState = CaptureCurrentState();
                SetEditMode(false);
                MessageBox.Show("Student information saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving student data:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LogAction(string actionType, string description)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string sql = @"INSERT INTO reg_logs (name, role, action, log_date)
                                         VALUES (@name, @role, @action, NOW())";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", "Registrar");
                        cmd.Parameters.AddWithValue("@role", "Registrar");
                        cmd.Parameters.AddWithValue("@action", string.Format("{0}: {1}", actionType ?? "", description ?? "").Trim(':', ' '));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
            }
        }

        private void InitializeBirthDatePicker()
        {
            if (birthDatePicker == null) return;

            birthDatePicker.Format = DateTimePickerFormat.Custom;
            birthDatePicker.CustomFormat = " ";
            birthDatePicker.MaxDate = new DateTime(2026, 12, 31);
            birthDatePicker.ValueChanged -= BirthDatePicker_ValueChanged;
            birthDatePicker.ValueChanged += BirthDatePicker_ValueChanged;
        }

        private void InitializeSectionComboBox()
        {
            if (sectionComboBox == null) return;

            sectionComboBox.Items.Clear();
            sectionComboBox.Items.AddRange(new[] { "A", "B", "C" });
            sectionComboBox.SelectedIndex = -1;
        }

        private void BirthDatePicker_ValueChanged(object sender, EventArgs e)
        {
            if (birthDatePicker == null) return;

            birthDatePicker.Format = DateTimePickerFormat.Custom;
            birthDatePicker.CustomFormat = "MMMM dd, yyyy";
        }

        private void SetBirthDate(DateTime? value)
        {
            if (birthDatePicker == null) return;

            birthDatePicker.Format = DateTimePickerFormat.Custom;

            if (value.HasValue)
            {
                var maxDate = new DateTime(2026, 12, 31);
                birthDatePicker.Value = value.Value > maxDate ? maxDate : value.Value;
                birthDatePicker.CustomFormat = "MMMM dd, yyyy";
            }
            else
            {
                birthDatePicker.CustomFormat = " ";
            }
        }

        private bool ValidateStudentInfo(out string message)
        {
            message = string.Empty;

            string rfid = rfidNum != null ? rfidNum.Text.Trim() : string.Empty;
            string studentIdValue = studentID != null ? studentID.Text.Trim() : string.Empty;
            string fullName = studentName != null ? studentName.Text.Trim() : string.Empty;
            string studentContact = contactNum != null ? contactNum.Text.Trim() : string.Empty;
            string levelValue = level != null ? level.Text.Trim() : string.Empty;
            string gradeValue = studentGrade != null ? studentGrade.Text.Trim() : string.Empty;
            string sectionValue = sectionComboBox != null ? sectionComboBox.Text.Trim() : string.Empty;
            string guardianNameValue = guardianName != null ? guardianName.Text.Trim() : string.Empty;
            string guardianContact = guardianNum != null ? guardianNum.Text.Trim() : string.Empty;
            string addressValue = address != null ? address.Text.Trim() : string.Empty;

            if (rfid.Length > 20)
            {
                message = "RFID must be at most 20 characters.";
                return false;
            }

            if (!Regex.IsMatch(studentIdValue, @"^\d{8}-[CNS]$", RegexOptions.IgnoreCase))
            {
                message = "Student ID must be 8 digits followed by '-' and end with C, N, or S (example: 20240357-C).";
                return false;
            }

            if (fullName.Length == 0 || fullName.Length > 255)
            {
                message = "Full name is required and must be at most 255 characters.";
                return false;
            }

            if (!IsValidPhoneNumber(studentContact))
            {
                message = "Student contact must start with '09' (11 digits) or '+63 9' (14 chars including plus and space).";
                return false;
            }

            if (!(string.Equals(levelValue, "elementary", StringComparison.OrdinalIgnoreCase)
                || string.Equals(levelValue, "junior", StringComparison.OrdinalIgnoreCase)
                || string.Equals(levelValue, "senior", StringComparison.OrdinalIgnoreCase)))
            {
                message = "Level must be only: elementary, junior, or senior.";
                return false;
            }

            int gradeNumber;
            if (!int.TryParse(gradeValue, out gradeNumber) || gradeNumber < 1 || gradeNumber > 12)
            {
                message = "Grade must be a number from 1 to 12.";
                return false;
            }

            if (!(string.Equals(sectionValue, "A", StringComparison.OrdinalIgnoreCase)
                || string.Equals(sectionValue, "B", StringComparison.OrdinalIgnoreCase)
                || string.Equals(sectionValue, "C", StringComparison.OrdinalIgnoreCase)))
            {
                message = "Section must be only A, B, or C.";
                return false;
            }

            if (birthDatePicker != null && birthDatePicker.CustomFormat != " " && birthDatePicker.Value.Year > 2026)
            {
                message = "Date of birth year must not exceed 2026.";
                return false;
            }

            if (guardianNameValue.Length > 255)
            {
                message = "Guardian name must be at most 255 characters.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(guardianContact) && !IsValidPhoneNumber(guardianContact))
            {
                message = "Guardian contact must start with '09' (11 digits) or '+63 9' (14 chars including plus and space).";
                return false;
            }

            if (addressValue.Length > 255)
            {
                message = "Address must be at most 255 characters.";
                return false;
            }

            return true;
        }

        private bool IsValidPhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;

            if (Regex.IsMatch(value, @"^09\d{9}$"))
                return true;

            if (Regex.IsMatch(value, @"^\+63 9\d{9}$") && value.Length == 14)
                return true;

            return false;
        }

        private object GetBirthDateValue()
        {
            if (birthDatePicker == null || birthDatePicker.CustomFormat == " ")
                return DBNull.Value;

            return birthDatePicker.Value.Date;
        }

        private void archiveBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Archive function is linked to this button. Add your archive table/column logic if needed.", "Archive", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void ApplyUserIdentityLabels()
        {
            string userName = string.IsNullOrWhiteSpace(UserSession.UserName) ? "Username" : UserSession.UserName;
            string roleText = string.IsNullOrWhiteSpace(UserSession.Role) ? "Role" : UserSession.Role;

            if (this.username != null)
            {
                this.username.Text = userName;
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
                if (nameLabel != null) nameLabel.Text = userName;
            }

            if (this.role != null)
            {
                this.role.Text = roleText;
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
                if (roleLabel != null) roleLabel.Text = roleText;
            }
        }

        private void guna2HtmlLabel12_Click(object sender, EventArgs e)
        {

        }

        private void archiveBtn_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentStudentId))
            {
                MessageBox.Show("No student selected to archive.", "Archive", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to archive {studentName.Text}?",
                "Confirm Archive",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            try
            {
                string imagePath = currentImagePath;
                string rfid = rfidNum != null ? rfidNum.Text.Trim() : string.Empty;
                string studentId = studentID != null ? studentID.Text.Trim() : string.Empty;
                string fullName = studentName != null ? studentName.Text.Trim() : string.Empty;
                string phone = contactNum != null ? contactNum.Text.Trim() : string.Empty;
                string guardian = guardianName != null ? guardianName.Text.Trim() : string.Empty;
                string guardianPhone = guardianNum != null ? guardianNum.Text.Trim() : string.Empty;
                string addressValue = address != null ? address.Text.Trim() : string.Empty;
                string gradeValue = studentGrade != null ? studentGrade.Text.Trim() : string.Empty;
                string sectionValue = sectionComboBox != null ? sectionComboBox.Text.Trim() : string.Empty;
                string levelValue = level != null ? level.Text.Trim() : string.Empty;
                DateTime? dob = (birthDatePicker == null || birthDatePicker.CustomFormat == " ")
                    ? (DateTime?)null
                    : birthDatePicker.Value.Date;

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        const string insertQuery = @"INSERT INTO reg_studentarchive
                            (image_path, rfid_number, student_id, name, phone_number, guardian_name, guardian_phone_number, address, grade, section, `level`, date_of_birth)
                            VALUES
                            (@image, @rfid, @studentId, @name, @phone, @guardian, @guardianPhone, @address, @grade, @section, @level, @dob)";

                        using (var insertCmd = new MySqlCommand(insertQuery, conn, transaction))
                        {
                            insertCmd.Parameters.AddWithValue("@image", string.IsNullOrWhiteSpace(imagePath) ? (object)DBNull.Value : imagePath);
                            insertCmd.Parameters.AddWithValue("@rfid", string.IsNullOrWhiteSpace(rfid) ? (object)DBNull.Value : rfid);
                            insertCmd.Parameters.AddWithValue("@studentId", studentId);
                            insertCmd.Parameters.AddWithValue("@name", string.IsNullOrWhiteSpace(fullName) ? (object)DBNull.Value : fullName);
                            insertCmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone);
                            insertCmd.Parameters.AddWithValue("@guardian", string.IsNullOrWhiteSpace(guardian) ? (object)DBNull.Value : guardian);
                            insertCmd.Parameters.AddWithValue("@guardianPhone", string.IsNullOrWhiteSpace(guardianPhone) ? (object)DBNull.Value : guardianPhone);
                            insertCmd.Parameters.AddWithValue("@address", string.IsNullOrWhiteSpace(addressValue) ? (object)DBNull.Value : addressValue);
                            insertCmd.Parameters.AddWithValue("@grade", string.IsNullOrWhiteSpace(gradeValue) ? (object)DBNull.Value : gradeValue);
                            insertCmd.Parameters.AddWithValue("@section", string.IsNullOrWhiteSpace(sectionValue) ? (object)DBNull.Value : sectionValue);
                            insertCmd.Parameters.AddWithValue("@level", string.IsNullOrWhiteSpace(levelValue) ? (object)DBNull.Value : levelValue);
                            insertCmd.Parameters.AddWithValue("@dob", dob.HasValue ? (object)dob.Value : DBNull.Value);
                            insertCmd.ExecuteNonQuery();
                        }

                        using (var deleteCmd = new MySqlCommand("DELETE FROM reg_studentinfo WHERE student_id = @studentId", conn, transaction))
                        {
                            deleteCmd.Parameters.AddWithValue("@studentId", currentStudentId);
                            deleteCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }

                LogAction("StudentArchived", $"Archived and transferred student: {fullName} ({currentStudentId})");

                MessageBox.Show($"{fullName} was successfully archived.",
                    "Archive Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ReturnToStudentIdForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to archive student:\n" + ex.Message,
                    "Archive Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReturnToStudentIdForm()
        {
            var studentIdForm = Application.OpenForms
                .OfType<StudentIDForm>()
                .FirstOrDefault();

            if (studentIdForm == null)
                studentIdForm = new StudentIDForm();

            studentIdForm.StartPosition = FormStartPosition.Manual;
            studentIdForm.Location = this.Location;
            studentIdForm.Show();
            studentIdForm.BringToFront();
            studentIdForm.Activate();

            this.Close();
        }

        private void guna2HtmlLabel16_Click(object sender, EventArgs e)
        {

        }
    }
}
