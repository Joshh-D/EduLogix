using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EduLogix
{
    public partial class Logs : Form
    {
        private readonly string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";

        public Logs()
        {
            InitializeComponent();

            if (!DesignMode)
            {
                LoadThemeFromDatabase();
                MarkActiveNav();
                InitializeDatePicker();
                LoadLogs();  // Load AFTER theme
                ConfigureDataGridView();  // Configure AFTER loading
            }
        }

        private void InitializeDatePicker()
        {
            if (guna2DateTimePicker1 != null)
            {
                // Set default date to today
                guna2DateTimePicker1.Value = DateTime.Now.Date;
                
                // Wire the date change event
                guna2DateTimePicker1.ValueChanged += (s, args) => LoadLogs();
            }
        }

        private void ConfigureDataGridView()
        {
            guna2DataGridView1.EnableHeadersVisualStyles = false;
            guna2DataGridView1.ColumnHeadersHeight = 40;
            
            // Header styling with theme color
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = currentThemeColor;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Inter", 10, FontStyle.Bold);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = DarkenColor(currentThemeColor, 0.15f);
            
            // Data cell styling
            guna2DataGridView1.DefaultCellStyle.Font = new Font("Inter", 9, FontStyle.Regular);
            guna2DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            guna2DataGridView1.DefaultCellStyle.BackColor = Color.White;
            
            // Selection styling with THEME COLOR
            guna2DataGridView1.DefaultCellStyle.SelectionBackColor = LightenColor(currentThemeColor, 0.3f);
            guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            
            // Make read-only
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            // Alternating row colors with pattern - using theme color
            Color lightPatternColor = LightenColor(currentThemeColor, 0.7f);
            guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = lightPatternColor;
            guna2DataGridView1.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            
            guna2DataGridView1.ClearSelection();
        }

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
                            currentThemeColor = Color.FromArgb(r, g, b);
                            ApplyThemeColor(currentThemeColor);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        { 
        }

        private void Dashboard_Click_1(object sender, EventArgs e)
        {
            var form = new DashboardForm();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = this.Location;
            form.FormClosed += (s, args) => this.Close();
            form.Show();
            this.Hide();
        }

        private void Attendance_Click_1(object sender, EventArgs e)
        {
            var form = new AttendanceForm();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = this.Location;
            form.FormClosed += (s, args) => this.Close();
            form.Show();
            this.Hide();
        }

        private void StudentsID_Click_1(object sender, EventArgs e)
        {
            var form = new StudentIDForm();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = this.Location;
            form.FormClosed += (s, args) => this.Close();
            form.Show();
            this.Hide();
        }

        private void ApplyThemeColor(Color themeColor)
        {
            // Same main gradient as Settings
            if (guna2GradientPanel1 != null)
            {
                var darker = DarkenColor(themeColor, 0.35f);
                guna2GradientPanel1.FillColor = darker;
                guna2GradientPanel1.FillColor2 = themeColor;
            }

            if (guna2GradientPanel2 != null)
            {
                guna2GradientPanel2.FillColor = Color.White;
                guna2GradientPanel2.FillColor2 = LightenColor(themeColor, 0.9f);
            }

            // Left nav buttons – same styling as Settings
            ApplyThemeToNavButton(Dashboard, themeColor);
            ApplyThemeToNavButton(Attendance, themeColor);
            ApplyThemeToNavButton(StudentsID, themeColor);
            ApplyThemeToNavButton(guna2Button1, themeColor); // Logs button
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

        private void MarkActiveNav()
        {
            // Uncheck others and make them transparent
            if (Dashboard != null)
            {
                Dashboard.Checked = false;
                Dashboard.FillColor = Color.Transparent;
            }
            if (Attendance != null)
            {
                Attendance.Checked = false;
                Attendance.FillColor = Color.Transparent;
            }
            if (StudentsID != null)
            {
                StudentsID.Checked = false;
                StudentsID.FillColor = Color.Transparent;
            }

            // Active button white
            if (guna2Button1 != null)
            {
                guna2Button1.Checked = true;
                guna2Button1.FillColor = Color.White;
                guna2Button1.ForeColor = Color.Black;
                guna2Button1.CheckedState.FillColor = Color.White;
                guna2Button1.CheckedState.ForeColor = Color.Black;
            }
        }

        private Color currentThemeColor = Color.FromArgb(48, 79, 99);

        private Color DarkenColor(Color color, float amount)
        {
            int r = Math.Max(0, (int)(color.R * (1 - amount)));
            int g = Math.Max(0, (int)(color.G * (1 - amount)));
            int b = Math.Max(0, (int)(color.B * (1 - amount)));
            return Color.FromArgb(color.A, r, g, b);
        }

        private Color LightenColor(Color color, float amount)
        {
            int r = Math.Min(255, (int)(color.R + (255 - color.R) * amount));
            int g = Math.Min(255, (int)(color.G + (255 - color.G) * amount));
            int b = Math.Min(255, (int)(color.B + (255 - color.B) * amount));
            return Color.FromArgb(color.A, r, g, b);
        }

        private Color GetContrastColor(Color color)
        {
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return luminance > 0.5 ? Color.Black : Color.White;
        }

        private void LoadLogs()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    
                    // Get selected date from date picker
                    DateTime selectedDate = guna2DateTimePicker1 != null 
                        ? guna2DateTimePicker1.Value.Date 
                        : DateTime.Now.Date;
                    
                    // Query logs for the selected date only
                    const string sql = @"SELECT name, role, action, log_date
                                         FROM reg_logs
                                         WHERE DATE(log_date) = @selectedDate
                                         ORDER BY log_date DESC";
                    
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@selectedDate", selectedDate);
                        using (var da = new MySqlDataAdapter(cmd))
                        {
                            var dt = new DataTable();
                            da.Fill(dt);
                            guna2DataGridView1.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading logs:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}