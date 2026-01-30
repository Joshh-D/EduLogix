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
    public partial class LibraryDashboard : Form
    {
        public LibraryDashboard()
        {
            InitializeComponent();
        }

        private void Borrowings_Click(object sender, EventArgs e)
        {
            Borrowers borrowings = new Borrowers();
            borrowings.Show();
            this.Hide();
        }

        private void BookCatalog_Click(object sender, EventArgs e)
        {
            BookCatalog bookCatalog = new BookCatalog();
            bookCatalog.Show();
            this.Hide();
        }

        private void LibLogs_Click(object sender, EventArgs e)
        {
            LibraryLogs libLogs = new LibraryLogs();
            libLogs.Show();
            this.Hide();
        }

        private void Settings_Click(object sender, EventArgs e)
        {

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

        private void viewstudinfo_Click(object sender, EventArgs e)
        {
            LibIdle libIdle = new LibIdle();
            libIdle.Show();
        }
    }
}
