using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

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
            LoadStudents();
        }

        private void LoadStudents()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query =
                        "SELECT `student_num`, `name`, `grade_level`, `section`, `date_registered` FROM reg_regstudents";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    guna2DataGridView1.DataSource = table;

                    guna2DataGridView1.Columns["student_num"].HeaderText = "Student No.";
                    guna2DataGridView1.Columns["name"].HeaderText = "Name";
                    guna2DataGridView1.Columns["grade_level"].HeaderText = "Grade Level";
                    guna2DataGridView1.Columns["section"].HeaderText = "Section";
                    guna2DataGridView1.Columns["date_registered"].HeaderText = "Date";

                    guna2DataGridView1.EnableHeadersVisualStyles = false;
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor =
                        Color.FromArgb(181, 213, 167);
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor =
                        Color.FromArgb(181, 213, 167);
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor =
                        Color.White;
                    guna2DataGridView1.ColumnHeadersHeight = 40;
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font =
                        new Font("Segoe UI", 10, FontStyle.Bold);

                    guna2DataGridView1.DefaultCellStyle.Font =
                        new Font("Segoe UI", 9);
                    guna2DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                    guna2DataGridView1.DefaultCellStyle.SelectionBackColor =
                        Color.FromArgb(216, 235, 210);
                    guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

                    guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor =
                        Color.FromArgb(236, 246, 232);
                    guna2DataGridView1.AlternatingRowsDefaultCellStyle.ForeColor =
                        Color.Black;

                    guna2DataGridView1.GridColor = Color.LightGray;
                    guna2DataGridView1.BorderStyle = BorderStyle.None;
                    guna2DataGridView1.RowHeadersVisible = false;

                    guna2DataGridView1.AutoSizeColumnsMode =
                        DataGridViewAutoSizeColumnsMode.Fill;
                    guna2DataGridView1.ReadOnly = true;
                    guna2DataGridView1.SelectionMode =
                        DataGridViewSelectionMode.FullRowSelect;

                    foreach (DataGridViewColumn col in guna2DataGridView1.Columns)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                }
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
    }
}
