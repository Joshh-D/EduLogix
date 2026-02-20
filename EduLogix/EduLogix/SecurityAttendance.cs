using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EduLogix
{
    public partial class SecurityAttendance : Form
    {
        private string connectionString = "server=192.168.236.30;database=edulogix;uid=arduino_user;pwd=secret;";

        public SecurityAttendance()
        {
            InitializeComponent();
        }

        private void SecurityAttendance_Load(object sender, EventArgs e)
        {
            LoadAttendance();
            guna2DataGridView1.Focus();
        }

        private void LoadAttendance()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM reg_attendance";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    guna2DataGridView1.DataSource = table;
                    guna2DataGridView1.Refresh();
                    guna2DataGridView1.Invalidate();

                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(181, 213, 167);
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

                    guna2DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                    guna2DataGridView1.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

                    guna2DataGridView1.Columns["student_num"].HeaderText = "Student No.";
                    guna2DataGridView1.Columns["name"].HeaderText = "Name";
                    guna2DataGridView1.Columns["grade_level"].HeaderText = "Grade Level";
                    guna2DataGridView1.Columns["section"].HeaderText = "Section";
                    guna2DataGridView1.Columns["date_and_time"].HeaderText = "Date & Time";

                    guna2DataGridView1.EnableHeadersVisualStyles = false;
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(181, 213, 167);
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font =
                        new Font("Segoe UI", 10, FontStyle.Bold);
                    guna2DataGridView1.ColumnHeadersHeight = 40;

                    guna2DataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9);
                    guna2DataGridView1.DefaultCellStyle.SelectionBackColor =
                        Color.FromArgb(216, 235, 210);
                    guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

                    guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor =
                        Color.FromArgb(236, 246, 232);
                    guna2DataGridView1.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

                    guna2DataGridView1.GridColor = Color.LightGray;
                    guna2DataGridView1.BorderStyle = BorderStyle.None;
                    guna2DataGridView1.RowHeadersVisible = false;

                    guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    guna2DataGridView1.ReadOnly = true;
                    guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading attendance data:\n" + ex.Message);
            }
        }

        private void Dashboard_Click(object sender, EventArgs e)
        {
            Security security = new Security();
            security.Show();
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
    }
}
