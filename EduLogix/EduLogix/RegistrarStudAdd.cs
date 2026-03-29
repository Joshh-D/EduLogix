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
    public partial class RegistrarStudAdd : Form
    {
        private readonly string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";

        public RegistrarStudAdd()
        {
            InitializeComponent();
            WirePrimaryActionButtons();
            InitializeBirthDatePicker();
        }

        private void WirePrimaryActionButtons()
        {
            if (addStudentBtn != null)
            {
                addStudentBtn.Click -= addStudentBtn_Click;
                addStudentBtn.Click += addStudentBtn_Click;
            }

            if (bindRFIDBtn != null)
            {
                bindRFIDBtn.Click -= bindRFIDBtn_Click;
                bindRFIDBtn.Click += bindRFIDBtn_Click;
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
            if (string.IsNullOrWhiteSpace(studentID.Text) || string.IsNullOrWhiteSpace(studentName.Text))
            {
                MessageBox.Show("Student ID and Student Name are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    const string query = @"INSERT INTO reg_studentinfo
                                           (rfid_number, student_id, name, phone_number, level, grade, section, guardian_name, guardian_phone_number, address, date_of_birth)
                                           VALUES
                                           (@rfid, @student_id, @name, @phone_number, @level, @grade, @section, @guardian_name, @guardian_phone_number, @address, @date_of_birth)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding student:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
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
        }

        private void RegistrarStudAdd_Load(object sender, EventArgs e)
        {
            InitializeSectionComboBox();
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
