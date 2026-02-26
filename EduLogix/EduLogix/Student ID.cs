using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EduLogix
{
    public partial class StudentIDForm : Form
    {
        private string connectionString = "server=localhost;database=edulogix;uid=root;";

        public StudentIDForm()
        {
            InitializeComponent();
            this.Load += StudentIDForm_Load;

        }

        private void StudentIDForm_Load(object sender, EventArgs e)
        {
            combobox1.SelectedIndex = 0;
            combobox2.Enabled = false;
            combobox2.Items.Clear();

            LoadStudents();
            ApplyThemeToForm();
        }

        private void SearchData()
        {
            string keyword = guna2TextBox1.Text.Trim();

            try
            {
                DataTable table = DatabaseFunctions.SearchStudents(connectionString, keyword);
                guna2DataGridView1.DataSource = table;
                attendancestudcount.Text = guna2DataGridView1.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FilterData()
        {
            try
            {
                string levelFilter = null;
                int? gradeFilter = null;

                if (combobox1.SelectedItem != null && combobox1.SelectedItem.ToString() != "All")
                    levelFilter = combobox1.SelectedItem.ToString().ToLower();

                if (combobox2.Enabled && combobox2.SelectedItem != null && combobox2.SelectedItem.ToString() != "All")
                    gradeFilter = int.Parse(combobox2.SelectedItem.ToString().Replace("Grade ", ""));

                DataTable table = DatabaseFunctions.GetStudents(connectionString, levelFilter, gradeFilter);
                guna2DataGridView1.DataSource = table;
                attendancestudcount.Text = guna2DataGridView1.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ApplyThemeToForm()
        {
            try
            {
                Color? themeColorNullable = DatabaseFunctions.GetThemeColor(connectionString);

                studentidcount.Text = (guna2DataGridView1.Rows.Count).ToString();

                if (themeColorNullable.HasValue)
                {
                    Color themeColor = themeColorNullable.Value;

                    // Apply theme to sidebar background
                    this.BackColor = themeColor;

                    // Apply theme to navigation buttons
                    ApplyThemeToButtons(themeColor);

                    // Apply theme to control boxes
                    ApplyThemeToControlBoxes(themeColor);

                    // Apply theme to labels
                    ApplyThemeToLabels(themeColor);

                    // Apply theme to DataGridView
                    ApplyThemeToDataGridView(themeColor);
                }
            }
            catch (Exception ex)
            {
                // Silently fail and use default colors
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
                    }
                }
            }
        }

        private void ApplyThemeToControlBoxes(Color themeColor)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2ControlBox ctrlBox)
                {
                    ctrlBox.FillColor = themeColor;
                }
            }
        }

        private void ApplyThemeToLabels(Color themeColor)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2HtmlLabel htmlLabel)
                {
                    if (htmlLabel.Name == "UserName" || htmlLabel.Name == "guna2HtmlLabel1")
                    {
                        htmlLabel.BackColor = themeColor;
                    }
                }
            }
        }

        private void ApplyThemeToDataGridView(Color themeColor)
        {
            // Calculate lighter tint for alternating rows
            Color lightTint = LightenColor(themeColor, 0.7f);

            // Apply theme color to table header
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = themeColor;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = themeColor;

            // Apply light tint to alternating rows
            guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = lightTint;

            // Keep selection colors themed
            guna2DataGridView1.DefaultCellStyle.SelectionBackColor = LightenColor(themeColor, 0.5f);
        }

        private Color LightenColor(Color color, float amount)
        {
            int r = Math.Min(255, (int)(color.R + (255 - color.R) * amount));
            int g = Math.Min(255, (int)(color.G + (255 - color.G) * amount));
            int b = Math.Min(255, (int)(color.B + (255 - color.B) * amount));
            return Color.FromArgb(color.A, r, g, b);
        }

        private void LoadStudents()
        {
            try
            {
                string levelFilter = null;
                int? gradeFilter = null;

                if (combobox1.SelectedItem != null && combobox1.SelectedItem.ToString() != "All")
                    levelFilter = combobox1.SelectedItem.ToString();

                if (combobox2.Enabled && combobox2.SelectedItem != null && combobox2.SelectedItem.ToString() != "All")
                    gradeFilter = int.Parse(combobox2.SelectedItem.ToString().Replace("Grade ", ""));

                DataTable table = DatabaseFunctions.GetStudents(connectionString, levelFilter, gradeFilter);

                // Bind to DataGridView
                guna2DataGridView1.DataSource = table;
                studentidcount.Text = table.Rows.Count.ToString();

                // Customize headers
                if (guna2DataGridView1.Columns.Count > 0)
                {
                    guna2DataGridView1.Columns["student_id"].HeaderText = "Student No.";
                    guna2DataGridView1.Columns["name"].HeaderText = "Name";
                    guna2DataGridView1.Columns["grade"].HeaderText = "Grade";
                    guna2DataGridView1.Columns["section"].HeaderText = "Section";
                    guna2DataGridView1.Columns["level"].HeaderText = "Level";

                    foreach (DataGridViewColumn col in guna2DataGridView1.Columns)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                }

                // Styling (optional, keeps your original style)
                guna2DataGridView1.EnableHeadersVisualStyles = false;
                guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                guna2DataGridView1.ColumnHeadersHeight = 40;
                guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font =
                    new Font("Segoe UI", 10, FontStyle.Bold);

                guna2DataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9);
                guna2DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
                guna2DataGridView1.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
                guna2DataGridView1.GridColor = Color.LightGray;
                guna2DataGridView1.BorderStyle = BorderStyle.None;
                guna2DataGridView1.RowHeadersVisible = false;
                guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                guna2DataGridView1.ReadOnly = true;
                guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Dashboard_Click_1(object sender, EventArgs e)
        {
            DashboardForm dashboard = new DashboardForm();
            dashboard.Show();
            this.Hide();
        }

        private void Attendance_Click_1(object sender, EventArgs e)
        {
            AttendanceForm attendance = new AttendanceForm();
            attendance.Show();
            this.Hide();
        }

        private void Accounts_Click(object sender, EventArgs e)
        {
            Users user = new Users();
            user.Show();
            this.Hide();
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            Logs log = new Logs();
            log.Show();
            this.Hide();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
            this.Hide();
        }

        private void ViewStudentInfo_Click(object sender, EventArgs e)
        {
            StudentInfo studentInfo = new StudentInfo();
            studentInfo.Show();
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

        private void viewstudinfo_Click_1(object sender, EventArgs e)
        {
            StudentInfo studentInfo = new StudentInfo();
            studentInfo.Show();
            this.Hide();
        }

        private void combobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            combobox2.Items.Clear();
            combobox2.Enabled = false;

            if (combobox1.SelectedItem == null) return;

            string selectedLevel = combobox1.SelectedItem.ToString();

            if (selectedLevel == "Elementary")
            {
                combobox2.Enabled = true;
                combobox2.Items.Add("All");
                for (int i = 1; i <= 6; i++)
                    combobox2.Items.Add("Grade " + i);
            }
            else if (selectedLevel == "Junior")
            {
                combobox2.Enabled = true;
                combobox2.Items.Add("All");
                for (int i = 7; i <= 10; i++)
                    combobox2.Items.Add("Grade " + i);
            }
            else if (selectedLevel == "Senior")
            {
                combobox2.Enabled = true;
                combobox2.Items.Add("All");
                combobox2.Items.Add("Grade 11");
                combobox2.Items.Add("Grade 12");
            }

            if (combobox2.Items.Count > 0)
                combobox2.SelectedIndex = 0; // auto-select "All"

            LoadStudents(); // reload DataGridView
        }


        private void combobox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStudents();
        }

        
        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(guna2TextBox1.Text))
            {
                FilterData();
            }
            else
            {
                SearchData();
            }
        }

        private void btnAddNewStudent_Event(object sender, EventArgs e)
        {
            StudentInfo stdinfo = new StudentInfo();
            stdinfo.Show();
            this.Hide();
        }
    }
}


