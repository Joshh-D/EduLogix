using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace EduLogix
{
    public partial class StudentIDForm : Form
    {
        private string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";

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
            combobox2.Text = "All Grades";

            LoadStudents();
            ApplyThemeToForm();
        }

        private void SearchData()
        {
            string keyword = guna2TextBox1.Text.Trim();
            string query = "SELECT student_id, name, grade, section, level FROM reg_studentinfo WHERE " +
               "(student_id LIKE @search OR name LIKE @search OR grade LIKE @search OR section LIKE @search OR level LIKE @search)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@search", "%" + keyword + "%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                guna2DataGridView1.DataSource = table;
                ApplyGridStyle(table);
            }
        }

        private void FilterData()
        {
            string query = "SELECT student_id, name, grade, section, level FROM reg_studentinfo";
            string levelFilter = "";
            string gradeFilter = "";

            if (combobox1.SelectedItem != null && combobox1.SelectedItem.ToString() != "All")
                levelFilter = "level = @level";

            if (combobox2.Enabled && combobox2.Text != "All Grades")
                gradeFilter = "grade = @grade";

            if (levelFilter != "" && gradeFilter != "")
                query += " WHERE " + levelFilter + " AND " + gradeFilter;
            else if (levelFilter != "")
                query += " WHERE " + levelFilter;
            else if (gradeFilter != "")
                query += " WHERE " + gradeFilter;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                if (levelFilter != "")
                    cmd.Parameters.AddWithValue("@level", combobox1.SelectedItem.ToString().ToLower());

                if (gradeFilter != "")
                    cmd.Parameters.AddWithValue("@grade", combobox2.Text.Replace("Grade ", ""));

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                guna2DataGridView1.DataSource = table;
                ApplyGridStyle(table);
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
                        Color themeColor = Color.FromArgb(r, g, b);

                        this.BackColor = themeColor;
                        ApplyThemeToButtons(themeColor);
                        ApplyThemeToControlBoxes(themeColor);
                        ApplyThemeToLabels(themeColor);
                        ApplyThemeToDataGridView();
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

        private void ApplyThemeToDataGridView()
        {
            guna2DataGridView1.ThemeStyle.RowsStyle.SelectionBackColor = Color.White;
            guna2DataGridView1.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
        }

        private void LoadStudents()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT student_id, name, grade, section, level FROM reg_studentinfo";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                guna2DataGridView1.DataSource = table;
                ApplyGridStyle(table);
            }
        }

        private void ApplyGridStyle(DataTable table)
        {
            guna2DataGridView1.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            guna2DataGridView1.EnableHeadersVisualStyles = false;

            guna2DataGridView1.Columns["student_id"].HeaderText = "Student ID";
            guna2DataGridView1.Columns["name"].HeaderText = "Name";
            guna2DataGridView1.Columns["grade"].HeaderText = "Grade";
            guna2DataGridView1.Columns["section"].HeaderText = "Section";
            guna2DataGridView1.Columns["level"].HeaderText = "Level";

            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Ivory;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.Ivory;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Inter", 10, FontStyle.Bold);
            guna2DataGridView1.ColumnHeadersHeight = 40;

            guna2DataGridView1.DefaultCellStyle.Font = new Font("Inter", 9, FontStyle.Regular);
            guna2DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            guna2DataGridView1.DefaultCellStyle.BackColor = Color.White;
            guna2DataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            guna2DataGridView1.RowsDefaultCellStyle.BackColor = Color.White;
            guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.Ivory;

            guna2DataGridView1.ThemeStyle.RowsStyle.BackColor = Color.White;
            guna2DataGridView1.ThemeStyle.AlternatingRowsStyle.BackColor = Color.Ivory;

            guna2DataGridView1.ThemeStyle.RowsStyle.SelectionBackColor = Color.White;
            guna2DataGridView1.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;

            guna2DataGridView1.GridColor = Color.LightGray;
            guna2DataGridView1.BorderStyle = BorderStyle.None;
            guna2DataGridView1.RowHeadersVisible = false;
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            attendancetotal.Text = table.Rows.Count.ToString();
        }

        private string GetSelectedGrade()
        {
            if (!combobox2.Enabled || combobox2.SelectedItem == null)
                return "";

            string selected = combobox2.SelectedItem.ToString();
            if (selected == "All Grades") return "";

            if (selected.StartsWith("Grade "))
                selected = selected.Replace("Grade ", "").Trim();

            return selected;
        }

        private void combobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            combobox2.Items.Clear();
            combobox2.Enabled = false;
            combobox2.Text = "All Grades";

            if (combobox1.SelectedItem == null) return;

            string selectedLevel = combobox1.SelectedItem.ToString();

            if (selectedLevel == "Elementary")
            {
                combobox2.Enabled = true;
                for (int i = 1; i <= 6; i++)
                    combobox2.Items.Add("Grade " + i);
            }
            else if (selectedLevel == "Junior")
            {
                combobox2.Enabled = true;
                for (int i = 7; i <= 10; i++)
                    combobox2.Items.Add("Grade " + i);
            }
            else if (selectedLevel == "Senior")
            {
                combobox2.Enabled = true;
                combobox2.Items.Add("Grade 11");
                combobox2.Items.Add("Grade 12");
            }

            LoadStudents();
        }

        private void combobox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStudents();
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(guna2TextBox1.Text))
                FilterData();
            else
                SearchData();
        }

        private void Dashboard_Click(object sender, EventArgs e)
        {
            new DashboardForm().Show();
            this.Hide();
        }

        private void Attendance_Click(object sender, EventArgs e)
        {
            new AttendanceForm().Show();
            this.Hide();
        }

        private void Accounts_Click(object sender, EventArgs e)
        {
            new Users().Show();
            this.Hide();
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            new Logs().Show();
            this.Hide();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            new Settings().Show();
            this.Hide();
        }

        private void ViewStudentInfo_Click(object sender, EventArgs e)
        {
            new StudentInfo().Show();
            this.Hide();
        }

        private void viewstudinfo_Click_1(object sender, EventArgs e)
        {
            new StudentInfo().Show();
            this.Hide();
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                new Login().Show();
                this.Close();
            }
        }
    }
}