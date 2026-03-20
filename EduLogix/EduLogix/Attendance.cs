using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EduLogix
{
    public partial class AttendanceForm : Form
    {
        private string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";

        private static readonly string[] ElementaryGrades = { "Grade 1", "Grade 2", "Grade 3", "Grade 4", "Grade 5", "Grade 6" };
        private static readonly string[] JuniorHighGrades = { "Grade 7", "Grade 8", "Grade 9", "Grade 10" };
        private static readonly string[] SeniorHighGrades = { "Grade 11", "Grade 12" };

        public AttendanceForm()
        {
            InitializeComponent();
        }

        private void AttendanceForm_Load(object sender, EventArgs e)
        {
            attendanceCombobox1.SelectedIndexChanged += attendanceSecondCondCombobox_SelectedIndexChanged;
            attendanceCombobox2.SelectedIndexChanged += secondComboBox_SelectedIndexChanged;
            attendanceStudentSearch.TextChanged += attendanceStudentSearch_TextChanged;

            PopulateMainComboBox();
            LoadAttendance();
            ApplyThemeToForm();
        }

        // ===== COMBOBOX POPULATION =====

        private void PopulateMainComboBox()
        {
            // Temporarily detach event to avoid firing during population
            attendanceCombobox1.SelectedIndexChanged -= attendanceSecondCondCombobox_SelectedIndexChanged;

            attendanceCombobox1.Items.Clear();
            attendanceCombobox1.Items.Add("All");
            attendanceCombobox1.Items.Add("Elementary");
            attendanceCombobox1.Items.Add("Junior High");
            attendanceCombobox1.Items.Add("Senior High");
            attendanceCombobox1.SelectedIndex = 0;

            attendanceCombobox2.Items.Clear();
            attendanceCombobox2.Items.Add("All Grades");
            attendanceCombobox2.SelectedIndex = 0;
            attendanceCombobox2.Enabled = false;

            // Reattach event
            attendanceCombobox1.SelectedIndexChanged += attendanceSecondCondCombobox_SelectedIndexChanged;
        }

        private void PopulateSecondComboBox(string[] grades)
        {
            // Temporarily detach event to avoid firing during population
            attendanceCombobox2.SelectedIndexChanged -= secondComboBox_SelectedIndexChanged;

            attendanceCombobox2.Items.Clear();
            attendanceCombobox2.Items.Add("All Grades");

            foreach (string grade in grades)
                attendanceCombobox2.Items.Add(grade);

            attendanceCombobox2.SelectedIndex = 0;
            attendanceCombobox2.Enabled = true;

            // Reattach event
            attendanceCombobox2.SelectedIndexChanged += secondComboBox_SelectedIndexChanged;
        }

        // ===== COMBOBOX EVENTS =====

        private void attendanceSecondCondCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = attendanceCombobox1.SelectedItem?.ToString() ?? "";

            switch (selected)
            {
                case "Elementary":
                    PopulateSecondComboBox(ElementaryGrades);
                    LoadAttendance(attendanceStudentSearch.Text.Trim(), education: "elementary");
                    break;
                case "Junior High":
                    PopulateSecondComboBox(JuniorHighGrades);
                    LoadAttendance(attendanceStudentSearch.Text.Trim(), education: "junior");
                    break;
                case "Senior High":
                    PopulateSecondComboBox(SeniorHighGrades);
                    LoadAttendance(attendanceStudentSearch.Text.Trim(), education: "senior");
                    break;
                default: // "All"
                    attendanceCombobox2.SelectedIndexChanged -= secondComboBox_SelectedIndexChanged;
                    attendanceCombobox2.Items.Clear();
                    attendanceCombobox2.Items.Add("All Grades");
                    attendanceCombobox2.SelectedIndex = 0;
                    attendanceCombobox2.Enabled = false;
                    attendanceCombobox2.SelectedIndexChanged += secondComboBox_SelectedIndexChanged;
                    LoadAttendance(attendanceStudentSearch.Text.Trim());
                    break;
            }
        }

        private void secondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string grade = attendanceCombobox2.SelectedItem?.ToString() ?? "";
            if (grade == "All Grades") grade = "";

            // Strip "Grade " prefix to match database values (e.g. "Grade 11" → "11")
            if (grade.StartsWith("Grade "))
                grade = grade.Replace("Grade ", "").Trim();

            string education = GetSelectedEducation();
            LoadAttendance(attendanceStudentSearch.Text.Trim(), gradeLevel: grade, education: education);
        }

        private string GetSelectedEducation()
        {
            string selected = attendanceCombobox1.SelectedItem?.ToString() ?? "";
            switch (selected)
            {
                case "Elementary": return "elementary";
                case "Junior High": return "junior";
                case "Senior High": return "senior";
                default: return "";
            }
        }


        // ===== THEME =====

        private void ApplyThemeToForm()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT theme_red, theme_green, theme_blue FROM reg_theme WHERE id = 2";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int r = Convert.ToInt32(reader["theme_red"]);
                        int g = Convert.ToInt32(reader["theme_green"]);
                        int b = Convert.ToInt32(reader["theme_blue"]);
                        Color themeColor = Color.FromArgb(r, g, b);

                        this.BackColor = themeColor;
                        ApplyThemeToButtons(themeColor);
                        ApplyThemeToControlBoxes(themeColor);
                        ApplyThemeToLabels(themeColor);
                        ApplyThemeToDataGridView(themeColor);
                    }
                }
            }
            catch (Exception) { }
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
                    }
                }
            }
        }

        private void ApplyThemeToControlBoxes(Color themeColor)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2ControlBox ctrlBox)
                    ctrlBox.FillColor = themeColor;
            }
        }

        private void ApplyThemeToLabels(Color themeColor)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2HtmlLabel htmlLabel)
                {
                    if (htmlLabel.Name == "UserName" || htmlLabel.Name == "guna2HtmlLabel1")
                        htmlLabel.BackColor = themeColor;
                }
            }
        }

        private void ApplyThemeToDataGridView(Color themeColor)
        {
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = themeColor;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = themeColor;
            guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = LightenColor(themeColor, 0.7f);
            guna2DataGridView1.DefaultCellStyle.SelectionBackColor = LightenColor(themeColor, 0.5f);
        }

        private Color LightenColor(Color color, float amount)
        {
            int r = Math.Min(255, (int)(color.R + (255 - color.R) * amount));
            int g = Math.Min(255, (int)(color.G + (255 - color.G) * amount));
            int b = Math.Min(255, (int)(color.B + (255 - color.B) * amount));
            return Color.FromArgb(color.A, r, g, b);
        }

        // ===== DATA LOADING =====

        private void LoadAttendance(string filter = "", string gradeLevel = "", string status = "", string education = "")
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM reg_attendance WHERE 1=1";

                    if (!string.IsNullOrWhiteSpace(education))
                        query += " AND education = @education";

                    if (!string.IsNullOrWhiteSpace(filter))
                        query += " AND (student_name LIKE @nameFilter OR student_num LIKE @numFilter OR date_and_time LIKE @dateFilter)";

                    if (!string.IsNullOrWhiteSpace(gradeLevel))
                        query += " AND grade_level = @gradeLevel";

                    if (!string.IsNullOrWhiteSpace(status))
                        query += " AND status = @status";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        cmd.Parameters.AddWithValue("@nameFilter", "%" + filter + "%");
                        cmd.Parameters.AddWithValue("@numFilter", filter + "%");
                        cmd.Parameters.AddWithValue("@dateFilter", "%" + filter + "%");
                    }

                    if (!string.IsNullOrWhiteSpace(education))
                        cmd.Parameters.AddWithValue("@education", education);

                    if (!string.IsNullOrWhiteSpace(gradeLevel))
                        cmd.Parameters.AddWithValue("@gradeLevel", gradeLevel);

                    if (!string.IsNullOrWhiteSpace(status))
                        cmd.Parameters.AddWithValue("@status", status);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    guna2DataGridView1.DataSource = table;

                    if (guna2DataGridView1.Columns.Contains("rfid_number"))
                        guna2DataGridView1.Columns["rfid_number"].Visible = false;

                    guna2DataGridView1.Columns["student_num"].HeaderText = "Student No.";
                    guna2DataGridView1.Columns["student_name"].HeaderText = "Name";
                    guna2DataGridView1.Columns["grade_level"].HeaderText = "Grade Level";
                    guna2DataGridView1.Columns["education"].HeaderText = "Education";
                    guna2DataGridView1.Columns["section"].HeaderText = "Section";
                    guna2DataGridView1.Columns["status"].HeaderText = "Status";
                    guna2DataGridView1.Columns["date_and_time"].HeaderText = "Date & Time";

                    guna2DataGridView1.EnableHeadersVisualStyles = false;

                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Ivory;
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.Ivory;
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Inter", 10, FontStyle.Bold);
                    guna2DataGridView1.ColumnHeadersHeight = 40;

                    guna2DataGridView1.DefaultCellStyle.Font = new Font("Inter", 9, FontStyle.Regular);
                    guna2DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                    guna2DataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
                    guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

                    guna2DataGridView1.RowsDefaultCellStyle.BackColor = Color.White;
                    guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.Ivory;

                    guna2DataGridView1.GridColor = Color.LightGray;
                    guna2DataGridView1.BorderStyle = BorderStyle.None;
                    guna2DataGridView1.RowHeadersVisible = false;
                    guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    guna2DataGridView1.ReadOnly = true;
                    guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    guna2DataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;

                    attendancetotal.Text = table.Rows.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading attendance data:\n" + ex.Message);
            }
        }

        private void SetupDataGridViewSelectionBehavior()
        {
            guna2DataGridView1.ClearSelection();

            guna2DataGridView1.MouseDown += (s, e) =>
            {
                var hit = guna2DataGridView1.HitTest(e.X, e.Y);
                if (hit.Type == DataGridViewHitTestType.None)
                    guna2DataGridView1.ClearSelection();
            };
        }
        private void attendanceStudentSearch_TextChanged(object sender, EventArgs e)
        {
            string filter = attendanceStudentSearch.Text.Trim();
            string grade = GetSelectedGrade();
            string education = GetSelectedEducation();
            string status = GetSelectedStatus();

            LoadAttendance(filter, gradeLevel: grade, status: status, education: education);
        }

        private string GetSelectedGrade()
        {
            if (!attendanceCombobox2.Enabled || attendanceCombobox2.SelectedItem == null)
                return "";

            string selected = attendanceCombobox2.SelectedItem.ToString();
            if (selected == "All Grades") return "";

            if (selected.StartsWith("Grade "))
                selected = selected.Replace("Grade ", "").Trim();

            return selected;
        }

        private string GetSelectedStatus()
        {
            string selected = attendanceCombobox1.SelectedItem?.ToString() ?? "";
            return (selected == "In Premises" || selected == "Departed") ? selected : "";
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        

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
                new Login().Show();
                this.Close();
            }
        }

        private void Dashboard_Click(object sender, EventArgs e)
        {
            new DashboardForm().Show(); this.Hide();
        }

        private void StudentsID_Click(object sender, EventArgs e)
        {
            new StudentIDForm().Show(); this.Hide();
        }
    }
}