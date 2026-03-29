using System;
using System.Data;
using System.Windows.Forms;

namespace EduLogix_LMS
{
    public partial class Login : Form
    {
        private dbhandler db = new dbhandler();

        public Login()
        {
            InitializeComponent();
            // Link the click event if not done via Designer
            btnLogin.Click += btnLogin_Click;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            txtbxAccountID.TextChanged += TxtbxAccountID_TextChanged;
            txtbxPassword.TextChanged += TxtbxPassword_TextChanged;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (ValidateLoginInputs())
            {
                string username = txtbxAccountID.Text.Trim();
                string password = txtbxPassword.Text;

                DataRow userRow = db.VerifyLogin(username, password);

                if (userRow != null)
                {
                    if (userRow != null)
                    {
                        this.DialogResult = DialogResult.OK; // This hides the form and returns OK to Program.cs
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Username or Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtbxPassword.Clear();
                    txtbxPassword.Focus();
                }
            }
        }

        private void BtnSeePassword_Click(object sender, EventArgs e)
        {
            txtbxPassword.UseSystemPasswordChar = !txtbxPassword.UseSystemPasswordChar;
            if (txtbxPassword.UseSystemPasswordChar)
            {
                txtbxPassword.PasswordChar = '●';
                btnSeePassword.IconChar = FontAwesome.Sharp.IconChar.EyeLowVision;
            }
            else
            {
                txtbxPassword.PasswordChar = '\0';
                btnSeePassword.IconChar = FontAwesome.Sharp.IconChar.Eye;
            }
        }

        private void BtnKioskMode_Click(object sender, EventArgs e)
        {
            try
            {
                KioskForm kioskForm = new KioskForm();
                this.Hide();
                kioskForm.ShowDialog();
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Kiosk Mode: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtbxAccountID_TextChanged(object sender, EventArgs e)
        { txtbxAccountID.Text = txtbxAccountID.Text.Trim(); }

        private void TxtbxPassword_TextChanged(object sender, EventArgs e) { }

        public bool ValidateLoginInputs()
        {
            if (!InputValidator.IsNotEmpty(txtbxAccountID, "Account ID")) return false;
            if (!InputValidator.IsNotEmpty(txtbxPassword, "Password")) return false;
            return true;
        }

        public void ClearInputs()
        {
            InputValidator.ClearTextboxes(this);
        }
    }
}