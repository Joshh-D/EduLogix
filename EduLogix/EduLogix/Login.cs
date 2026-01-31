using System;
using System.Drawing;
using System.Windows.Forms;

namespace EduLogix
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            LockTransparentBackColor(accountID);
            LockTransparentBackColor(accountpass);

            this.ActiveControl = guna2HtmlLabel9;

            accountID.Enter += accountID_Enter;
            accountpass.Enter += accountpass_Enter;
            loginbtn.Click += loginbtn_Click;
        }

        private void LockTransparentBackColor(Guna.UI2.WinForms.Guna2TextBox txt)
        {
            // BackColor can ONLY be set directly (no state)
            txt.BackColor = Color.Transparent;

            // Actual textbox background
            txt.FillColor = Color.White;

            // Prevent border popups
            txt.BorderThickness = 0;
            txt.BorderColor = Color.Transparent;
            txt.FocusedState.BorderColor = Color.Transparent;
            txt.HoverState.BorderColor = Color.Transparent;
            txt.DisabledState.BorderColor = Color.Transparent;
        }
    



        // --- Clear placeholder for Account ID ---
        private void accountID_Enter(object sender, EventArgs e)
        {
            accountID.BackColor = Color.Transparent;

            if (accountID.Text == "Account ID")
            {
                accountID.Text = "";
                accountID.ForeColor = Color.Black;
            }
        }

        private void accountpass_Enter(object sender, EventArgs e)
        {
            accountpass.BackColor = Color.Transparent;

            if (accountpass.Text == "Password")
            {
                accountpass.Text = "";
                accountpass.ForeColor = Color.Black;
                accountpass.UseSystemPasswordChar = true;
            }
        }

        private void loginbtn_Click(object sender, EventArgs e)
        {
            string correctIDreg = "Registrar";
            string correctPassreg = "RegAdmin2026";
            string correctIDLib = "Librarian";
            string correctPassLib = "LibAdmin2026";
            string correctIDsec = "Security";
            string correctPasssec = "Sec2026";


            if (accountID.Text == correctIDreg && accountpass.Text == correctPassreg)
            {
                DashboardForm dashboard = new DashboardForm();
                dashboard.Show();
                this.Hide();
                accountID.Clear();
                accountpass.Clear();
            }
            else if (accountID.Text == correctIDLib && accountpass.Text == correctPassLib)
            {
                LibraryDashboard dashboard = new LibraryDashboard();
                dashboard.Show();
                this.Hide();
                accountID.Clear();
                accountpass.Clear();
            }
            else if (accountID.Text == correctIDsec && accountpass.Text == correctPasssec)
            {
                Security security = new Security();
                security.Show();
                this.Hide();
                accountID.Clear();
                accountpass.Clear();
            }

            else
            {
                accountID.BorderColor = Color.Red;
                accountID.BorderThickness = 1;
                accountpass.BorderColor = Color.Red;
                accountpass.BorderThickness = 1;
                MessageBox.Show("Invalid Account ID or Password. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ResetColors()
        {
            accountID.BackColor = Color.White;
            accountpass.BackColor = Color.White;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        private void loginbtn_Click_1(object sender, EventArgs e)
        {

        }

       
    }
}
