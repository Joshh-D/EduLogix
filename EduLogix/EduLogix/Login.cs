using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EduLogix
{
    public partial class Login : Form
    {
        private string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";

        public Login()
        {
            InitializeComponent();
            LockTransparentBackColor(accountID);
            LockTransparentBackColor(accountpass);

            BrandingHelper.ApplySchoolBranding(connectionString, schoolName, schoolLogo);

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
            // validation on textboxes
            if (string.IsNullOrWhiteSpace(accountID.Text) || accountID.Text == "Account ID")
            {
                MessageBox.Show("Please enter your username.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(accountpass.Text) || accountpass.Text == "Password")
            {
                MessageBox.Show("Please enter your password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT username, password, role FROM sys_users WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", accountID.Text.Trim());

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string dbUsername = reader["username"].ToString();
                        string dbPassword = reader["password"].ToString();
                        string dbRole = reader["role"].ToString();

                        // check if password matches
                        if (accountpass.Text == dbPassword)
                        {
                            reader.Close();
                            conn.Close();

                            // clear input fields
                            accountID.Clear();
                            accountpass.Clear();

                            // navigate based on role
                            if (dbRole.Equals("registrar", StringComparison.OrdinalIgnoreCase))
                            {
                                DashboardForm dashboard = new DashboardForm();
                                dashboard.Show();
                                this.Hide();
                            }
                            else if (dbRole.Equals("librarian", StringComparison.OrdinalIgnoreCase))
                            {
                                LibraryDashboard dashboard = new LibraryDashboard();
                                dashboard.Show();
                                this.Hide();
                            }
                            else if (dbRole.Equals("security", StringComparison.OrdinalIgnoreCase))
                            {
                                Security security = new Security();
                                security.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Unknown role. Please contact administrator.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            // incorrect password
                            reader.Close();
                            ShowLoginError();
                        }
                    }
                    else
                    {
                        // username not found
                        reader.Close();
                        ShowLoginError();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowLoginError()
        {
            accountID.BorderColor = Color.Red;
            accountID.BorderThickness = 1;
            accountpass.BorderColor = Color.Red;
            accountpass.BorderThickness = 1;
            MessageBox.Show("Invalid Account ID or Password. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
