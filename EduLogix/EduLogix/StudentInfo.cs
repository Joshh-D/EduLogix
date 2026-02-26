using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EduLogix
{
    public partial class StudentInfo : Form
    {
        public StudentInfo()
        {
            InitializeComponent();
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

        private void AttendancePanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void StudentInfo_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtbxRFIDNumber.Text))
            {
                lblPleaseScanTheRFID.Visible = true;
            }
            else
            {
                lblPleaseScanTheRFID.Visible = false;
            }
        }
    }
}
