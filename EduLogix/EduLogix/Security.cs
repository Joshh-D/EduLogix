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
    public partial class Security : Form
    {
        public Security()
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

        private void SecurityAttendance_Click(object sender, EventArgs e)
        {
            SecurityAttendance security = new SecurityAttendance();
            security.Show();
            this.Hide();
        }
    }
}
