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
    public partial class RegistrarStudAdd : Form
    {
        private readonly string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";
        private bool hasUnsavedChanges;
        private bool suppressDirtyTracking;
        private string selectedImageSourcePath;

        public RegistrarStudAdd()
        {
            InitializeComponent();
            InitializeUserOptionsPanel();
            WirePrimaryActionButtons();
            InitializeBirthDatePicker();
            InitializeSectionComboBox();
            ConfigureInputConstraints();
            WireDirtyTracking();
            WireNavigationButtons();
            ApplyUserIdentityLabels();

            if (guna2PictureBox2 != null)
            {
                guna2PictureBox2.DoubleClick -= guna2PictureBox2_DoubleClick;
                guna2PictureBox2.DoubleClick += guna2PictureBox2_DoubleClick;
            }

            this.FormClosing -= RegistrarStudAdd_FormClosing;
            this.FormClosing += RegistrarStudAdd_FormClosing;
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

            var panelKioskButton = userOptions != null ? userOptions.Controls["kiosk"] as Button : null;
            if (panelKioskButton != null)
            {
                panelKioskButton.Click -= UserOptionskiosk_Click;
                panelKioskButton.Click += UserOptionskiosk_Click;
            }

            if (userProfile != null)
            {
                userProfile.Click -= userProfile_Click;
                userProfile.Click += userProfile_Click;
            }
        }

        private void WireNavigationButtons()
        {
            if (Dashboard != null)
            {
                Dashboard.Click -= Dashboard_Click;
                Dashboard.Click += Dashboard_Click;
            }

            if (Attendance != null)
            {
                Attendance.Click -= Attendance_Click;
                Attendance.Click += Attendance_Click;
            }

            if (StudentsID != null)
            {
                StudentsID.Click -= StudentsID_Click;
                StudentsID.Click += StudentsID_Click;
            }

            if (Logs != null)
            {
                Logs.Click -= Logs_Click;
                Logs.Click += Logs_Click;
            }
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

        private void WireDirtyTracking()
        {
            WireTextDirty(rfidNum);
            WireTextDirty(studentID);
            WireTextDirty(studentName);
            WireTextDirty(contactNum);
            WireTextDirty(level);
            WireTextDirty(studentGrade);
            WireTextDirty(guardianName);
            WireTextDirty(guardianNum);
            WireTextDirty(address);

            if (sectionComboBox != null)
            {
                sectionComboBox.SelectedIndexChanged -= sectionComboBox_SelectedIndexChanged;
                sectionComboBox.SelectedIndexChanged += sectionComboBox_SelectedIndexChanged;
            }
        }

        private void WireTextDirty(Guna.UI2.WinForms.Guna2TextBox textBox)
        {
            if (textBox == null) return;
            textBox.TextChanged -= InputControl_Changed;
            textBox.TextChanged += InputControl_Changed;
        }

        private void InputControl_Changed(object sender, EventArgs e)
        {
            if (suppressDirtyTracking) return;
            hasUnsavedChanges = true;
        }

        private void sectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suppressDirtyTracking) return;
            hasUnsavedChanges = true;
        }

        private void RegistrarStudAdd_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!hasUnsavedChanges) return;

            var result = MessageBox.Show(
                "Unsaved student info will be discarded. Continue?",
                "Discard Changes",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                e.Cancel = true;
        }

        private bool ConfirmDiscardIfNeeded()
        {
            if (!hasUnsavedChanges) return true;

            var result = MessageBox.Show(
                "Unsaved student info will be discarded. Continue?",
                "Discard Changes",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            return result == DialogResult.Yes;
        }

        private void WirePrimaryActionButtons()
        {
            if (addStudentBtn != null)
            {
                addStudentBtn.Click -= addStudentBtn_Click;
                addStudentBtn.Click += addStudentBtn_Click;
            }

        }

        private void bindRFIDBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(studentID.Text))
            {
                MessageBox.Show("Enter Student ID before binding RFID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                studentID.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(rfidNum.Text))
            {
                MessageBox.Show("Enter RFID number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rfidNum.Focus();
                return;
            }

            MessageBox.Show("RFID is ready to be saved with this student.", "RFID Bound", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void addStudentBtn_Click(object sender, EventArgs e)
        {
            string validationMessage;
            if (!ValidateStudentInfo(out validationMessage))
            {
                MessageBox.Show(validationMessage, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string imagePathDb = SaveSelectedImageToResources();

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    const string query = @"INSERT INTO reg_studentinfo
                                           (image_path, rfid_number, student_id, name, phone_number, level, grade, section, guardian_name, guardian_phone_number, address, date_of_birth)
                                           VALUES
                                           (@image_path, @rfid, @student_id, @name, @phone_number, @level, @grade, @section, @guardian_name, @guardian_phone_number, @address, @date_of_birth)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@image_path", string.IsNullOrWhiteSpace(imagePathDb) ? (object)DBNull.Value : imagePathDb);
                        cmd.Parameters.AddWithValue("@rfid", rfidNum.Text.Trim());
                        cmd.Parameters.AddWithValue("@student_id", studentID.Text.Trim());
                        cmd.Parameters.AddWithValue("@name", studentName.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone_number", contactNum.Text.Trim());
                        cmd.Parameters.AddWithValue("@level", level.Text.Trim());
                        cmd.Parameters.AddWithValue("@grade", studentGrade.Text.Trim());
                        cmd.Parameters.AddWithValue("@section", GetSectionValue());
                        cmd.Parameters.AddWithValue("@guardian_name", guardianName.Text.Trim());
                        cmd.Parameters.AddWithValue("@guardian_phone_number", guardianNum.Text.Trim());
                        cmd.Parameters.AddWithValue("@address", address.Text.Trim());
                        cmd.Parameters.AddWithValue("@date_of_birth", GetBirthDateValue());

                        cmd.ExecuteNonQuery();
                    }
                }

                LogAction("StudentAdded", $"Added new student: {studentName.Text.Trim()} ({studentID.Text.Trim()})");
                MessageBox.Show("Student added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                hasUnsavedChanges = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding student:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            string sectionValue = GetSectionValue();
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
                message = "Student ID must be 8 digits followed by '-' and end with C, N, or S.";
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

            int grade;
            if (!int.TryParse(gradeValue, out grade) || grade < 1 || grade > 12)
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

            var dob = GetBirthDateValue();
            if (dob == DBNull.Value)
            {
                message = "Date of birth is required.";
                return false;
            }

            DateTime dobValue = Convert.ToDateTime(dob);
            if (dobValue.Year > 2026)
            {
                message = "Date of birth year must not exceed 2026.";
                return false;
            }

            if (dobValue > DateTime.Today.AddYears(-1))
            {
                message = "Student must be at least 1 year old to be enrolled.";
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

        private string SaveSelectedImageToResources()
        {
            if (string.IsNullOrWhiteSpace(selectedImageSourcePath) || !File.Exists(selectedImageSourcePath))
                return string.Empty;

            string extension = Path.GetExtension(selectedImageSourcePath);
            if (!string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            string resourcesRoot = Path.Combine(Application.StartupPath, "Resources", "students");
            Directory.CreateDirectory(resourcesRoot);

            string fileName = "student_" + Guid.NewGuid().ToString("N") + extension;
            string destinationPath = Path.Combine(resourcesRoot, fileName);

            File.Copy(selectedImageSourcePath, destinationPath, true);
            return Path.Combine("Resources", "students", fileName).Replace("\\", "/");
        }

        private void ClearFields()
        {
            suppressDirtyTracking = true;

            rfidNum.Text = string.Empty;
            studentID.Text = string.Empty;
            studentName.Text = string.Empty;
            contactNum.Text = string.Empty;
            level.Text = string.Empty;
            studentGrade.Text = string.Empty;
            var legacySectionTextBox = this.Controls.Find("studentSection", true)
                .OfType<Guna.UI2.WinForms.Guna2TextBox>()
                .FirstOrDefault();
            if (legacySectionTextBox != null)
                legacySectionTextBox.Text = string.Empty;
            guardianName.Text = string.Empty;
            guardianNum.Text = string.Empty;
            address.Text = string.Empty;

            var sectionCombo = this.Controls.Find("sectionComboBox", true)
                .OfType<Guna.UI2.WinForms.Guna2ComboBox>()
                .FirstOrDefault();
            if (sectionCombo != null)
                sectionCombo.SelectedIndex = -1;

            ResetBirthDatePicker();
            selectedImageSourcePath = null;
            if (guna2PictureBox2 != null)
                guna2PictureBox2.Image = Properties.Resources.student;

            suppressDirtyTracking = false;
        }

        private void RegistrarStudAdd_Load(object sender, EventArgs e)
        {
            InitializeSectionComboBox();
            LoadUserProfileImage();
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

        private string GetSectionValue()
        {
            var sectionCombo = this.Controls.Find("sectionComboBox", true)
                .OfType<Guna.UI2.WinForms.Guna2ComboBox>()
                .FirstOrDefault();

            if (sectionCombo != null)
                return (sectionCombo.Text ?? string.Empty).Trim();

            var legacySectionTextBox = this.Controls.Find("studentSection", true)
                .OfType<Guna.UI2.WinForms.Guna2TextBox>()
                .FirstOrDefault();

            return legacySectionTextBox != null ? legacySectionTextBox.Text.Trim() : string.Empty;
        }

        private object GetBirthDateValue()
        {
            var gunaDatePicker = this.Controls.Find("birthDatePicker", true)
                .OfType<Guna.UI2.WinForms.Guna2DateTimePicker>()
                .FirstOrDefault();
            if (gunaDatePicker != null)
            {
                if (gunaDatePicker.CustomFormat == " ")
                    return DBNull.Value;

                return gunaDatePicker.Value.Date;
            }

            var winDatePicker = this.Controls.Find("birthDatePicker", true)
                .OfType<DateTimePicker>()
                .FirstOrDefault();
            if (winDatePicker != null)
                return winDatePicker.Value.Date;

            return DBNull.Value;
        }

        private void InitializeSectionComboBox()
        {
            var sectionCombo = this.Controls.Find("sectionComboBox", true)
                .OfType<Guna.UI2.WinForms.Guna2ComboBox>()
                .FirstOrDefault();

            if (sectionCombo == null) return;

            sectionCombo.Items.Clear();
            sectionCombo.Items.AddRange(new[] { "A", "B", "C" });
            sectionCombo.SelectedIndex = -1;
        }

        private void InitializeBirthDatePicker()
        {
            var gunaDatePicker = this.Controls.Find("birthDatePicker", true)
                .OfType<Guna.UI2.WinForms.Guna2DateTimePicker>()
                .FirstOrDefault();

            if (gunaDatePicker != null)
            {
                gunaDatePicker.Format = DateTimePickerFormat.Custom;
                gunaDatePicker.CustomFormat = " ";
                gunaDatePicker.MaxDate = new DateTime(2026, 12, 31);
                gunaDatePicker.ValueChanged -= BirthDatePicker_ValueChanged;
                gunaDatePicker.ValueChanged += BirthDatePicker_ValueChanged;
                return;
            }

            var winDatePicker = this.Controls.Find("birthDatePicker", true)
                .OfType<DateTimePicker>()
                .FirstOrDefault();

            if (winDatePicker == null) return;

            winDatePicker.Format = DateTimePickerFormat.Custom;
            winDatePicker.CustomFormat = " ";
            winDatePicker.MaxDate = new DateTime(2026, 12, 31);
            winDatePicker.ValueChanged -= BirthDatePicker_ValueChanged;
            winDatePicker.ValueChanged += BirthDatePicker_ValueChanged;
        }

        private void ResetBirthDatePicker()
        {
            var gunaDatePicker = this.Controls.Find("birthDatePicker", true)
                .OfType<Guna.UI2.WinForms.Guna2DateTimePicker>()
                .FirstOrDefault();

            if (gunaDatePicker != null)
            {
                gunaDatePicker.Format = DateTimePickerFormat.Custom;
                gunaDatePicker.CustomFormat = " ";
                return;
            }

            var winDatePicker = this.Controls.Find("birthDatePicker", true)
                .OfType<DateTimePicker>()
                .FirstOrDefault();

            if (winDatePicker == null) return;

            winDatePicker.Format = DateTimePickerFormat.Custom;
            winDatePicker.CustomFormat = " ";
        }

        private void BirthDatePicker_ValueChanged(object sender, EventArgs e)
        {
            var gunaDatePicker = sender as Guna.UI2.WinForms.Guna2DateTimePicker;
            if (gunaDatePicker != null)
            {
                gunaDatePicker.Format = DateTimePickerFormat.Custom;
                gunaDatePicker.CustomFormat = "MMMM dd, yyyy";
                return;
            }

            var winDatePicker = sender as DateTimePicker;
            if (winDatePicker != null)
            {
                winDatePicker.Format = DateTimePickerFormat.Custom;
                winDatePicker.CustomFormat = "MMMM dd, yyyy";
            }
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox2_DoubleClick(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                ofd.Title = "Select Student Photo";

                if (ofd.ShowDialog(this) != DialogResult.OK)
                    return;

                string extension = Path.GetExtension(ofd.FileName);
                if (!string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Only JPEG and PNG files are allowed.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                selectedImageSourcePath = ofd.FileName;

                if (guna2PictureBox2 != null)
                {
                    using (var fs = new FileStream(selectedImageSourcePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var img = Image.FromStream(fs))
                    {
                        guna2PictureBox2.Image = new Bitmap(img);
                    }
                }

                hasUnsavedChanges = true;
            }
        }

        private void Dashboard_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfNeeded()) return;
            var form = new DashboardForm();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = this.Location;
            form.Show();
            this.Close();
        }

        private void Attendance_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfNeeded()) return;
            var form = new AttendanceForm();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = this.Location;
            form.Show();
            this.Close();
        }

        private void StudentsID_Click(object sender, EventArgs e)
        {
            // 1. Check if we should discard changes
            if (!ConfirmDiscardIfNeeded()) return;

            // 2. Create the target form
            var students = new StudentIDForm();

            // 3. Keep the window in the same position on the screen
            students.StartPosition = FormStartPosition.Manual;
            students.Location = this.Location;

            // 4. Show the target form FIRST
            students.Show();

            // 5. Close the current form SECOND
            this.Close();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            StudentsID_Click(sender, e);
        }

        private void Logs_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardIfNeeded()) return;

            var logs = new Logs();
            logs.StartPosition = FormStartPosition.Manual;
            logs.Location = this.Location;
            logs.Show();
            this.Close();
        }

        private void UserOptionsSettings_Click(object sender, EventArgs e)
        {
            if (userOptions != null)
                userOptions.Visible = false;

            if (!ConfirmDiscardIfNeeded()) return;

            var settingsForm = new Settings();
            settingsForm.StartPosition = FormStartPosition.Manual;
            settingsForm.Location = this.Location;
            settingsForm.Show();
            this.Close();
        }

        private void UserOptionsLogout_Click(object sender, EventArgs e)
        {
            if (userOptions != null)
                userOptions.Visible = false;

            if (!ConfirmDiscardIfNeeded()) return;

            var result = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            var login = new Login();
            login.Show();
            this.Close();
        }

        private void UserOptionskiosk_Click(object sender, EventArgs e)
        {
            if (userOptions != null)
                userOptions.Visible = false;

            if (!ConfirmDiscardIfNeeded()) return;

            var kioskForm = new Kiosk();
            kioskForm.StartPosition = FormStartPosition.Manual;
            kioskForm.Location = this.Location;
            kioskForm.Show();
            this.Close();
        }

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

        private void guna2HtmlLabel5_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel8_Click(object sender, EventArgs e)
        {

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
    }
}
