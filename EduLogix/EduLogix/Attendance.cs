using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EduLogix
{
    public partial class AttendanceForm : Form
    {
        private string connectionString = "server=localhost;port=3306;database=edulogix;uid=root;";

        public AttendanceForm()
        {
            InitializeComponent();
        }

        private void AttendanceForm_Load(object sender, EventArgs e)
        {
            // Populate Level combobox with All + levels
            combobox1.Items.Clear();
            combobox1.Items.Add("All");
            combobox1.Items.Add("Elementary");
            combobox1.Items.Add("Junior");
            combobox1.Items.Add("Senior");
            combobox1.SelectedIndex = 0; // default to All

            // Grade combobox initially disabled
            combobox2.Enabled = false;
            combobox2.Items.Clear();

            LoadAttendance(); // initial load
            ApplyThemeToForm();
        }

        private void SearchData()
        {
            string keyword = guna2TextBox1.Text.Trim();
            string query = "SELECT * FROM reg_attendance WHERE " +
                           "(student_num LIKE @search OR " +
                           "name LIKE @search OR " +
                           "grade LIKE @search OR " +
                           "section LIKE @search OR " +
                           "level LIKE @search)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@search", "%" + keyword + "%");

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                guna2DataGridView1.DataSource = table;
                attendancestudcount.Text = guna2DataGridView1.Rows.Count.ToString();
            }
        }

        private void LoadAttendance(string query = "SELECT * FROM reg_attendance")
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    guna2DataGridView1.DataSource = table;

                    guna2DataGridView1.Columns["student_num"].HeaderText = "Student No.";
                    guna2DataGridView1.Columns["name"].HeaderText = "Name";
                    guna2DataGridView1.Columns["grade"].HeaderText = "Grade";
                    guna2DataGridView1.Columns["section"].HeaderText = "Section";
                    guna2DataGridView1.Columns["level"].HeaderText = "Level";
                    guna2DataGridView1.Columns["date_and_time"].HeaderText = "Date & Time";

                    guna2DataGridView1.EnableHeadersVisualStyles = false;
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font =
                        new Font("Segoe UI", 10, FontStyle.Bold);
                    guna2DataGridView1.ColumnHeadersHeight = 40;

                    guna2DataGridView1.DefaultCellStyle.Font =
                        new Font("Segoe UI", 9);
                    guna2DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                    guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

                    guna2DataGridView1.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

                    guna2DataGridView1.GridColor = Color.LightGray;
                    guna2DataGridView1.BorderStyle = BorderStyle.None;
                    guna2DataGridView1.RowHeadersVisible = false;

                    guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    guna2DataGridView1.ReadOnly = true;
                    guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                    attendancestudcount.Text = guna2DataGridView1.Rows.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading attendance data:\n" + ex.Message);
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
                        ApplyThemeToDataGridView(themeColor);
                    }
                }
            }
            catch { }
        }

        private void ApplyThemeToButtons(Color themeColor)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2Button btn)
                {
                    btn.FillColor = themeColor;
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
                    htmlLabel.BackColor = themeColor;
                }
            }
        }

        private void ApplyThemeToDataGridView(Color themeColor)
        {
            guna2DataGridView1.EnableHeadersVisualStyles = false;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = themeColor;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = themeColor;
        }

        private Color LightenColor(Color color, float amount)
        {
            int r = Math.Min(255, (int)(color.R + (255 - color.R) * amount));
            int g = Math.Min(255, (int)(color.G + (255 - color.G) * amount));
            int b = Math.Min(255, (int)(color.B + (255 - color.B) * amount));
            return Color.FromArgb(color.A, r, g, b);
        }
        private void FilterData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM reg_attendance";
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;

                    List<string> filters = new List<string>();

                    // Filter by Level
                    if (combobox1.SelectedItem != null && combobox1.SelectedItem.ToString() != "All")
                    {
                        filters.Add("level = @level");
                        cmd.Parameters.AddWithValue("@level", combobox1.SelectedItem.ToString());
                    }

                    // Filter by Grade
                    if (combobox2.Enabled && combobox2.SelectedItem != null && combobox2.SelectedItem.ToString() != "All")
                    {
                        string gradeNumber = combobox2.SelectedItem.ToString().Replace("Grade ", "");
                        filters.Add("grade = @grade");
                        cmd.Parameters.AddWithValue("@grade", gradeNumber);
                    }

                    // Filter by Date
                    DateTime selectedDate = guna2DateTimePicker1.Value.Date;
                    filters.Add("DATE(date_and_time) = @date");
                    cmd.Parameters.AddWithValue("@date", selectedDate);


                    // Apply WHERE if there are any filters
                    if (filters.Count > 0)
                        query += " WHERE " + string.Join(" AND ", filters);

                    cmd.CommandText = query;

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    guna2DataGridView1.DataSource = table;
                    attendancestudcount.Text = table.Rows.Count.ToString();

                    // Optional: fix headers
                    if (guna2DataGridView1.Columns.Count > 0)
                    {
                        guna2DataGridView1.Columns["student_num"].HeaderText = "Student No.";
                        guna2DataGridView1.Columns["name"].HeaderText = "Name";
                        guna2DataGridView1.Columns["grade"].HeaderText = "Grade";
                        guna2DataGridView1.Columns["section"].HeaderText = "Section";
                        guna2DataGridView1.Columns["level"].HeaderText = "Level";
                        guna2DataGridView1.Columns["date_and_time"].HeaderText = "Date & Time";

                        // 🔹 Move date_and_time to last column
                        int lastIndex = guna2DataGridView1.Columns.Count - 1;
                        guna2DataGridView1.Columns["date_and_time"].DisplayIndex = lastIndex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading attendance data:\n" + ex.Message);
            }

            



        }

        private void combobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset Grade combobox
            combobox2.Items.Clear();
            combobox2.Enabled = false;

            if (combobox1.SelectedItem == null) return;

            string selectedLevel = combobox1.SelectedItem.ToString();

            if (selectedLevel != "All")
            {
                combobox2.Enabled = true;
                combobox2.Items.Add("All"); // Add "All" first

                if (selectedLevel == "Elementary")
                {
                    for (int i = 1; i <= 6; i++)
                        combobox2.Items.Add("Grade " + i);
                }
                else if (selectedLevel == "Junior")
                {
                    for (int i = 7; i <= 10; i++)
                        combobox2.Items.Add("Grade " + i);
                }
                else if (selectedLevel == "Senior")
                {
                    combobox2.Items.Add("Grade 11");
                    combobox2.Items.Add("Grade 12");
                }

                combobox2.SelectedIndex = 0; // auto-select "All"
            }

            FilterData(); // reload grid based on new Level
        }

        private void combobox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void Dashboard_Click(object sender, EventArgs e)
        {
            DashboardForm dashboard = new DashboardForm();
            dashboard.Show();
            this.Hide();
        }

        private void StudentsID_Click(object sender, EventArgs e)
        {
            StudentIDForm studentsID = new StudentIDForm();
            studentsID.Show();
            this.Hide();
        }

        private void Accounts_Click(object sender, EventArgs e)
        {
            Users user = new Users();
            user.Show();
            this.Hide();
        }

        private void Logs_Click(object sender, EventArgs e)
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

        private void guna2DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            FilterData();
        }
    }
}