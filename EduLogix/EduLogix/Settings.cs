using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EduLogix
{
    public partial class Settings : Form
    {
        private string connectionString = "server=192.168.236.30;database=edulogix;uid=arduino_user;pwd=secret;";

        public Settings()
        {
            InitializeComponent();
            LoadThemeFromDatabase();
        }

        private void LoadThemeFromDatabase()
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

                        // Apply theme on load
                        ApplyThemeColor(themeColor);
                        guna2GradientButton1.FillColor = themeColor;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading theme:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveThemeToDatabase(Color color)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE reg_theme 
                                    SET theme_red = @r, theme_green = @g, theme_blue = @b, 
                                        updated_by = @user, updated_at = NOW() 
                                    WHERE id = 1";  
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@r", color.R);
                    cmd.Parameters.AddWithValue("@g", color.G);
                    cmd.Parameters.AddWithValue("@b", color.B);
                    cmd.Parameters.AddWithValue("@user", "Registrar");
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving theme:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Browse School Logo Button Click
       

        // Click on PictureBox 3 (First default logo in guna2Panel5)
        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            // This is the default logo, allow changing
            BrowseAndSetKioskImage(guna2PictureBox3);
        }

        // Click on PictureBox 9 (Plus icon 1 in guna2Panel6)
        private void guna2PictureBox9_Click(object sender, EventArgs e)
        {
            BrowseAndSetKioskImage(guna2PictureBox9);
        }

        private void BrowseAndSetKioskImage(Guna.UI2.WinForms.Guna2PictureBox pictureBox)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Kiosk Slideshow Image";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Load and display the image
                        pictureBox.Image = Image.FromFile(openFileDialog.FileName);
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

                        MessageBox.Show("Slideshow image added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Accounts_Click_1(object sender, EventArgs e)
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

        private void kioskform_Click(object sender, EventArgs e)
        {
            Kiosk kiosk = new Kiosk();
            kiosk.Show();          
        }

        private void idleform_Click(object sender, EventArgs e)
        {
            RegIdle regIdle = new RegIdle();
            regIdle.Show();
        }

        // Theme Picker Button Click Event
        

        private void ApplyThemeColor(Color themeColor)
        {
            // Apply to sidebar background only
            this.BackColor = themeColor;

            // Apply to all controls recursively
            ApplyThemeToControls(this.Controls, themeColor);
        }

        private void ApplyThemeToControls(Control.ControlCollection controls, Color themeColor)
        {
            foreach (Control control in controls)
            {
                // Apply to navigation buttons by exact name
                if (control is Guna.UI2.WinForms.Guna2Button btn)
                {
                    if (btn.Name == "Dashboard" || 
                        btn.Name == "Attendance" || 
                        btn.Name == "StudentsID" || 
                        btn.Name == "Accounts" ||
                        btn.Name == "Logs" ||
                        btn.Name == "guna2Button1" || // Settings button
                        btn.Name == "Logout")
                    {
                        btn.FillColor = themeColor;
                    }
                }

                // Apply to window control boxes (minimize, close)
                if (control is Guna.UI2.WinForms.Guna2ControlBox ctrlBox)
                {
                    ctrlBox.FillColor = themeColor;
                }

                // Keep main content panels WHITE, don't apply theme
                if (control is Guna.UI2.WinForms.Guna2Panel panel)
                {
                    // Keep AttendancePanel and inner panels white
                    if (panel.Name == "AttendancePanel" || 
                        panel.Name == "guna2Panel2" || 
                        panel.Name == "guna2Panel3" ||
                        panel.Name == "guna2Panel4" ||
                        panel.Name == "guna2Panel5" ||
                        panel.Name == "guna2Panel6" ||
                        panel.Name == "guna2Panel7" ||
                        panel.Name == "guna2Panel8" ||
                        panel.Name == "guna2Panel11" ||
                        panel.Name == "guna2Panel12" ||
                        panel.Name == "guna2Panel14")
                    {
                        panel.FillColor = Color.White;
                    }
                }

                // Apply to labels by exact control name
                if (control is Guna.UI2.WinForms.Guna2HtmlLabel htmlLabel)
                {
                    // Target UserName and role label by exact control names
                    if (htmlLabel.Name == "UserName" || htmlLabel.Name == "guna2HtmlLabel1")
                    {
                        htmlLabel.BackColor = themeColor;
                    }
                }

                // Recursively apply to child controls (important for nested controls)
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls, themeColor);
                }
            }
        }

        // Helper method to lighten a color (for better UI contrast)
        private Color LightenColor(Color color, float amount)
        {
            int r = Math.Min(255, (int)(color.R + (255 - color.R) * amount));
            int g = Math.Min(255, (int)(color.G + (255 - color.G) * amount));
            int b = Math.Min(255, (int)(color.B + (255 - color.B) * amount));
            return Color.FromArgb(color.A, r, g, b);
        }

        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.AllowFullOpen = true;
                colorDialog.FullOpen = true;
                colorDialog.ShowHelp = false;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    Color selectedColor = colorDialog.Color;

                    // Update the theme picker button color
                    guna2GradientButton1.FillColor = selectedColor;

                    // Apply theme to the form
                    ApplyThemeColor(selectedColor);

                    // Save theme to database
                    SaveThemeToDatabase(selectedColor);

                    // Show confirmation
                    MessageBox.Show(
                        $"Theme Color Saved!\nRGB: ({selectedColor.R}, {selectedColor.G}, {selectedColor.B})\n\nThis theme will apply to all registrar forms.",
                        "Theme Applied",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }

        private void guna2GradientButton2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select School Logo";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Load and display the image in guna2PictureBox2 (inside guna2Panel4)
                        guna2PictureBox2.Image = Image.FromFile(openFileDialog.FileName);
                        guna2PictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

                        MessageBox.Show("School logo updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {

        }
    }
}
