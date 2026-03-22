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
        private readonly string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";
        private bool isLoading;
        private readonly Dictionary<string, object> settingsCache = new Dictionary<string, object>();
        private readonly string assetsPath = Path.Combine(Application.StartupPath, "assets");
        private readonly string logoPath = "";
        private readonly string kioskImagesPath = "";

        public Settings()
        {
            InitializeComponent();

            // Ensure assets directories exist
            Directory.CreateDirectory(assetsPath);
            Directory.CreateDirectory(Path.Combine(assetsPath, "idle_slideshow"));

            logoPath = Path.Combine(assetsPath, "logo.png");
            kioskImagesPath = Path.Combine(assetsPath, "idle_slideshow");

            if (!DesignMode)
            {
                InitializeSettings();
                LoadThemeFromDatabase();
                LoadSettingsFromDatabase();
                WireButtonEvents();
                MarkActiveNav();
            }
        }

        private void MarkActiveNav()
        {
            if (Dashboard != null) Dashboard.Checked = false;
            if (Attendance != null) Attendance.Checked = false;
            if (StudentsID != null) StudentsID.Checked = false;
        }

        #region Initialization

        private void InitializeSettings()
        {
            isLoading = true;

            // Auto logout seconds dropdown
            if (guna2ComboBox1 != null)
            {
                guna2ComboBox1.Items.Clear();
                guna2ComboBox1.Items.AddRange(new[]
                {
                    "30 seconds",
                    "1 minute",
                    "5 minutes",
                    "10 minutes"
                });
                guna2ComboBox1.SelectedIndex = 0;
            }

            // Slideshow duration dropdown (add if you have a second combo box)
            // If you have another dropdown (e.g., guna2ComboBox2), populate it here
            // for now, we'll assume it exists; adjust control name if needed
            if (guna2ComboBox1 != null) // Change this to your slideshow duration combo if different
            {
                // Can add slideshow durations here if you have a separate control
            }

            isLoading = false;
        }

        private void WireButtonEvents()
        {
            // Navigation handlers
            if (Dashboard != null) Dashboard.Click -= Dashboard_Click;
            if (Dashboard != null) Dashboard.Click += Dashboard_Click;

            if (Attendance != null) Attendance.Click -= Attendance_Click;
            if (Attendance != null) Attendance.Click += Attendance_Click;

            if (StudentsID != null) StudentsID.Click -= StudentsID_Click;
            if (StudentsID != null) StudentsID.Click += StudentsID_Click;

            if (guna2Button1 != null) guna2Button1.Click -= Logs_Click;
            if (guna2Button1 != null) guna2Button1.Click += Logs_Click;

            if (guna2GradientButton3 != null) guna2GradientButton3.Click -= guna2GradientButton3_Click;
            if (guna2GradientButton3 != null) guna2GradientButton3.Click += guna2GradientButton3_Click;

            if (guna2GradientButton4 != null) guna2GradientButton4.Click -= guna2GradientButton4_Click;
            if (guna2GradientButton4 != null) guna2GradientButton4.Click += guna2GradientButton4_Click;
        }

        #endregion

        #region Theme

        private void LoadThemeFromDatabase()
        {
            if (DesignMode) return;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string query = "SELECT theme_red, theme_green, theme_blue FROM reg_theme WHERE id = 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int r = Convert.ToInt32(reader["theme_red"]);
                            int g = Convert.ToInt32(reader["theme_green"]);
                            int b = Convert.ToInt32(reader["theme_blue"]);
                            var themeColor = Color.FromArgb(r, g, b);
                            ApplyThemeColor(themeColor);
                        }
                    }
                }
            }
            catch
            {
                // Swallow in designer/runtime to avoid breaking designer
            }
        }

        private void SaveThemeToDatabase(Color color)
        {
            if (DesignMode) return;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string query = @"UPDATE reg_theme 
                                           SET theme_red = @r, theme_green = @g, theme_blue = @b,
                                               updated_by = @user, updated_at = NOW()
                                           WHERE id = 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@r", color.R);
                        cmd.Parameters.AddWithValue("@g", color.G);
                        cmd.Parameters.AddWithValue("@b", color.B);
                        cmd.Parameters.AddWithValue("@user", "Registrar");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // ignore for now
            }
        }

        private void ApplyThemeColor(Color themeColor)
        {
            // Main background gradient: darker on top, picked color at bottom
            if (guna2GradientPanel1 != null)
            {
                var darker = DarkenColor(themeColor, 0.35f);
                guna2GradientPanel1.FillColor = darker;
                guna2GradientPanel1.FillColor2 = themeColor;
            }

            // Inner content gradient: white to picked color (very light)
            if (guna2GradientPanel2 != null)
            {
                guna2GradientPanel2.FillColor = Color.White;
                guna2GradientPanel2.FillColor2 = LightenColor(themeColor, 0.85f);
            }

            // Left nav buttons
            ApplyThemeToNavButton(Dashboard, themeColor);
            ApplyThemeToNavButton(Attendance, themeColor);
            ApplyThemeToNavButton(StudentsID, themeColor);
            ApplyThemeToNavButton(guna2Button1, themeColor);

            // Theme picker button
            if (guna2GradientButton1 != null)
            {
                var darker = DarkenColor(themeColor, 0.25f);
                guna2GradientButton1.FillColor = darker;
                guna2GradientButton1.FillColor2 = themeColor;
                guna2GradientButton1.ForeColor = GetContrastColor(themeColor);
            }

            // Save button
            if (guna2GradientButton3 != null)
            {
                var darker = DarkenColor(themeColor, 0.2f);
                guna2GradientButton3.FillColor = darker;
                guna2GradientButton3.FillColor2 = themeColor;
                guna2GradientButton3.ForeColor = GetContrastColor(themeColor);
            }

            // Reset stays red
            if (guna2GradientButton4 != null)
            {
                guna2GradientButton4.ForeColor = Color.White;
            }
            MarkActiveNav();
        }

        private void ApplyThemeToNavButton(Guna.UI2.WinForms.Guna2Button button, Color themeColor)
        {
            if (button == null) return;

            var normal = themeColor;
            var checkedColor = LightenColor(themeColor, 0.2f);

            button.FillColor = normal;
            button.ForeColor = GetContrastColor(normal);
            button.CheckedState.FillColor = checkedColor;
            button.CheckedState.ForeColor = GetContrastColor(checkedColor);
        }

        private Color LightenColor(Color color, float amount)
        {
            int r = Math.Min(255, (int)(color.R + (255 - color.R) * amount));
            int g = Math.Min(255, (int)(color.G + (255 - color.G) * amount));
            int b = Math.Min(255, (int)(color.B + (255 - color.B) * amount));
            return Color.FromArgb(color.A, r, g, b);
        }

        private Color DarkenColor(Color color, float amount)
        {
            int r = Math.Max(0, (int)(color.R * (1 - amount)));
            int g = Math.Max(0, (int)(color.G * (1 - amount)));
            int b = Math.Max(0, (int)(color.B * (1 - amount)));
            return Color.FromArgb(color.A, r, g, b);
        }

        private Color GetContrastColor(Color color)
        {
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return luminance > 0.5 ? Color.Black : Color.White;
        }

        #endregion

        #region Settings (reg_settings)

        private void LoadSettingsFromDatabase()
        {
            if (DesignMode) return;

            isLoading = true;
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string query = @"SELECT school_name, auto_logout_seconds, school_logo, slideshow_duration, kiosk_idle_seconds
                                           FROM reg_settings WHERE id = 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // School name
                            if (guna2TextBox1 != null && reader["school_name"] != DBNull.Value)
                                guna2TextBox1.Text = reader["school_name"].ToString();

                            // Auto logout seconds
                            if (guna2ComboBox1 != null && reader["auto_logout_seconds"] != DBNull.Value)
                            {
                                int seconds = Convert.ToInt32(reader["auto_logout_seconds"]);
                                guna2ComboBox1.SelectedItem = ConvertSecondsToDropdownText(seconds);
                            }

                            // School logo (load from assets)
                            if (File.Exists(logoPath))
                            {
                                if (guna2PictureBox1 != null)
                                {
                                    guna2PictureBox1.Image = Image.FromFile(logoPath);
                                    guna2PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                                }
                            }

                            // Slideshow duration (if you have a separate dropdown)
                            // if (guna2ComboBox2 != null && reader["slideshow_duration"] != DBNull.Value)
                            // {
                            //     int duration = Convert.ToInt32(reader["slideshow_duration"]);
                            //     guna2ComboBox2.SelectedItem = ConvertSecondsToDropdownText(duration);
                            // }

                            // Kiosk idle seconds (if you have another dropdown)
                            // Similar pattern as above
                        }
                    }
                }
            }
            catch
            {
                // ignore to keep designer safe
            }
            finally
            {
                isLoading = false;
            }
        }

        private void SaveSettingsToDatabase()
        {
            if (DesignMode) return;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string query = @"UPDATE reg_settings
                                           SET school_name = @name,
                                               auto_logout_seconds = @autoLogout,
                                               school_logo = @logo,
                                               slideshow_duration = @slideshowDuration,
                                               kiosk_idle_seconds = @kioskIdle,
                                               updated_at = NOW()
                                           WHERE id = 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        string schoolName = guna2TextBox1 != null ? guna2TextBox1.Text : "";
                        string autoLogoutText = (guna2ComboBox1 != null && guna2ComboBox1.SelectedItem != null)
                            ? guna2ComboBox1.SelectedItem.ToString()
                            : "30 seconds";

                        // Logo path (relative for git)
                        string logoPath_DB = File.Exists(logoPath) ? "assets/logo.png" : "";

                        // Slideshow duration (default to 30 seconds if not set)
                        int slideshowDuration = 30; // TODO: get from dropdown if you have one
                        int kioskIdle = 60; // TODO: get from dropdown if you have one

                        cmd.Parameters.AddWithValue("@name", schoolName);
                        cmd.Parameters.AddWithValue("@autoLogout", ConvertDropdownTextToSeconds(autoLogoutText));
                        cmd.Parameters.AddWithValue("@logo", logoPath_DB);
                        cmd.Parameters.AddWithValue("@slideshowDuration", slideshowDuration);
                        cmd.Parameters.AddWithValue("@kioskIdle", kioskIdle);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Settings saved successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving settings:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ConvertDropdownTextToSeconds(string dropdownText)
        {
            if (string.IsNullOrEmpty(dropdownText)) return 30;
            if (dropdownText.Contains("30")) return 30;
            if (dropdownText.Contains("1 minute")) return 60;
            if (dropdownText.Contains("5 minutes")) return 300;
            if (dropdownText.Contains("10 minutes")) return 600;
            return 30;
        }

        private string ConvertSecondsToDropdownText(int seconds)
        {
            switch (seconds)
            {
                case 60: return "1 minute";
                case 300: return "5 minutes";
                case 600: return "10 minutes";
                default: return "30 seconds";
            }
        }

        #endregion

        #region Logo Management

        private void guna2GradientButton2_Click(object sender, EventArgs e)
        {
            if (DesignMode) return;

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        // Copy logo to assets folder
                        File.Copy(ofd.FileName, logoPath, true);

                        // Display in preview
                        if (guna2PictureBox1 != null)
                        {
                            guna2PictureBox1.Image = Image.FromFile(logoPath);
                            guna2PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        }

                        MessageBox.Show("Logo saved to assets/logo.png",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LogHelper.Log("Registrar", "LogoChanged", $"Logo updated: {Path.GetFileName(ofd.FileName)}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading logo:\n" + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion

        #region Kiosk Slideshow Management

        private void guna2PictureBox9_Click(object sender, EventArgs e)
        {
            if (DesignMode) return;
            AddKioskSlideShowImage();
        }

        private void AddKioskSlideShowImage()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                ofd.Multiselect = true; // Allow multiple images

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        foreach (string filePath in ofd.FileNames)
                        {
                            // Generate unique filename with timestamp
                            string fileName = $"slide_{DateTime.Now:yyyyMMdd_HHmmss_fff}_{Path.GetFileNameWithoutExtension(filePath)}.png";
                            string destPath = Path.Combine(kioskImagesPath, fileName);

                            // Copy to kiosk images folder
                            File.Copy(filePath, destPath, true);

                            // Save metadata to DB (you may want to create a separate table for this)
                            SaveKioskImageMetadata(fileName);

                            LogHelper.Log("Registrar", "KioskImageAdded", $"Image added: {fileName}");
                        }

                        MessageBox.Show($"Added {ofd.FileNames.Length} image(s) to kiosk slideshow.",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error adding slideshow images:\n" + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SaveKioskImageMetadata(string imageName)
        {
            // For now, just store the relative path in reg_settings
            // If you want more control, create a separate table like reg_kiosk_images
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Example: append image path to a JSON array or comma-separated list
                    // This is simplified; you may want a dedicated table instead
                    const string query = @"UPDATE reg_settings
                                           SET kiosk_idle_slideshow = CONCAT(IFNULL(kiosk_idle_slideshow, ''), 
                                                                              IF(kiosk_idle_slideshow IS NULL OR kiosk_idle_slideshow = '', 
                                                                                 @image, 
                                                                                 CONCAT(',', @image)))
                                           WHERE id = 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@image", $"assets/idle_slideshow/{imageName}");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Log failure silently
            }
        }

        #endregion

        #region Event Handlers

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (isLoading || DesignMode) return;
            settingsCache["school_name"] = guna2TextBox1.Text;
        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading || DesignMode) return;
            settingsCache["auto_logout"] = guna2ComboBox1.SelectedItem;
        }

        private void guna2HtmlLabel6_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel5_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel3_Click(object sender, EventArgs e) { }

        private void kioskform_Click(object sender, EventArgs e)
        {
            if (DesignMode) return;
            new Kiosk().Show();
        }

        private void idleform_Click(object sender, EventArgs e)
        {
            if (DesignMode) return;
            new RegIdle().Show();
        }

        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {
            if (DesignMode) return;

            using (var dlg = new ColorDialog())
            {
                dlg.AllowFullOpen = true;
                dlg.FullOpen = true;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var color = dlg.Color;
                    ApplyThemeColor(color);
                    SaveThemeToDatabase(color);

                    LogHelper.Log("Registrar", "ThemeChanged",
                        $"New theme color: ({color.R}, {color.G}, {color.B})");
                }
            }
        }

        private void guna2GradientButton3_Click(object sender, EventArgs e)
        {
            SaveSettingsToDatabase();
        }

        private void guna2GradientButton4_Click(object sender, EventArgs e)
        {
            if (DesignMode) return;

            var result = MessageBox.Show(
                "Reset settings and theme to last saved values?",
                "Confirm Reset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                LoadSettingsFromDatabase();
                LoadThemeFromDatabase();
            }
        }

        private void Dashboard_Click(object sender, EventArgs e)
        {
            if (DesignMode) return;
            NavigateTo(new DashboardForm());
        }

        private void Attendance_Click(object sender, EventArgs e)
        {
            if (DesignMode) return;
            NavigateTo(new AttendanceForm());
        }

        private void StudentsID_Click(object sender, EventArgs e)
        {
            if (DesignMode) return;
            NavigateTo(new StudentIDForm());
        }

        private void Logs_Click(object sender, EventArgs e)
        {
            if (DesignMode) return;
            NavigateTo(new Logs());
        }

        private void NavigateTo(Form target)
        {
            target.StartPosition = FormStartPosition.Manual;
            target.Location = this.Location;
            target.FormClosed += (s, args) => this.Close();
            target.Show();
            this.Hide();
        }

        #endregion

        // Local logging helper (kept inside Settings, no extra files)
        private static class LogHelper
        {
            private static readonly string ConnectionString =
                "server=localhost;database=edulogix;uid=root;pwd=;";

            public static void Log(string userName, string actionType, string description)
            {
                try
                {
                    using (var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
                    {
                        conn.Open();
                        const string sql = @"INSERT INTO reg_logs
                                             (name, role, action, log_date)
                                             VALUES (@user, @type, @desc, NOW())";
                        using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@user", userName ?? "Unknown");
                            cmd.Parameters.AddWithValue("@type", actionType ?? "");
                            cmd.Parameters.AddWithValue("@desc", description ?? "");
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    // Silently ignore logging errors to not break UI
                }
            }
        }
    }
}