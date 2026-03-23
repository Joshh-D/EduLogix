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
        private Color currentThemeColor = Color.FromArgb(48, 79, 99);

        public Logs()
        {
            InitializeComponent();
            this.Load += Logs_Load;
        }

        private void Logs_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            LoadThemeFromDatabase();
            MarkActiveNav();
            InitializeDatePicker();
            LoadLogs();
        }

        private void InitializeDatePicker()
        {
            if (guna2DateTimePicker1 == null) return;

            guna2DateTimePicker1.ValueChanged -= Guna2DateTimePicker1_ValueChanged;
            guna2DateTimePicker1.Value = DateTime.Today;
            guna2DateTimePicker1.ValueChanged += Guna2DateTimePicker1_ValueChanged;
        }

        private void Guna2DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            LoadLogs();
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            LoadLogs();
        }

        private void LoadLogs()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    DateTime selectedDate = (guna2DateTimePicker1 != null) ? guna2DateTimePicker1.Value.Date : DateTime.Today;
                    string searchText = guna2TextBox1 != null ? guna2TextBox1.Text.Trim().ToLower() : "";

                    var dt = new DataTable();

                    string sql = @"
                SELECT `name`, `role`, `action`, `log_date`
                FROM `reg_logs`
                WHERE DATE(`log_date`) = @selectedDate";

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        sql += @"
                  AND (
                      LOWER(name) LIKE @search
                      OR LOWER(role) LIKE @search
                      OR LOWER(action) LIKE @search
                  )";
                    }

                    sql += " ORDER BY log_date DESC";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@selectedDate", selectedDate);

                        if (!string.IsNullOrWhiteSpace(searchText))
                            cmd.Parameters.AddWithValue("@search", "%" + searchText + "%");

                        using (var da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    // Fallback: no rows for selected date -> show all logs
                    if (dt.Rows.Count == 0 && string.IsNullOrWhiteSpace(searchText))
                    {
                        dt.Clear();
                        string allSql = @"
                    SELECT name, role, action, log_date
                    FROM reg_logs
                    ORDER BY log_date DESC";

                        using (var cmdAll = new MySqlCommand(allSql, conn))
                        using (var daAll = new MySqlDataAdapter(cmdAll))
                        {
                            daAll.Fill(dt);
                        }
                    }

                    // Force a clean rebind (helps when designer/grid state blocks display)
                    guna2DataGridView1.AutoGenerateColumns = true;
                    guna2DataGridView1.Columns.Clear();
                    guna2DataGridView1.DataSource = null;
                    guna2DataGridView1.DataSource = dt;
                    guna2DataGridView1.Refresh();

                    // Temporary debug indicator: remove after confirming logs display
                    this.Text = "Logs (" + dt.Rows.Count + " rows)";
                }

                ConfigureDataGridView();
                ApplyLogGridColumns();
                guna2DataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading logs:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyLogGridColumns()
        {
            if (guna2DataGridView1.Columns.Contains("name"))
            {
                guna2DataGridView1.Columns["name"].HeaderText = "Name";
                guna2DataGridView1.Columns["name"].FillWeight = 18;
            }

            if (guna2DataGridView1.Columns.Contains("role"))
            {
                guna2DataGridView1.Columns["role"].HeaderText = "Role";
                guna2DataGridView1.Columns["role"].FillWeight = 18;
            }

            if (guna2DataGridView1.Columns.Contains("action"))
            {
                guna2DataGridView1.Columns["action"].HeaderText = "Action";
                guna2DataGridView1.Columns["action"].FillWeight = 42;
            }

            if (guna2DataGridView1.Columns.Contains("log_date"))
            {
                guna2DataGridView1.Columns["log_date"].HeaderText = "Date & Time";
                guna2DataGridView1.Columns["log_date"].DefaultCellStyle.Format = "yyyy-MM-dd hh:mm:ss tt";
                guna2DataGridView1.Columns["log_date"].FillWeight = 22;
            }
        }

        private void ConfigureDataGridView()
        {
            Color gridThemeColor = GetDataGridThemeColor();

            guna2DataGridView1.EnableHeadersVisualStyles = false;
            guna2DataGridView1.ColumnHeadersHeight = 40;

            // Fixed sizing behavior
            guna2DataGridView1.AllowUserToResizeRows = false;
            guna2DataGridView1.AllowUserToResizeColumns = false;
            guna2DataGridView1.RowHeadersVisible = false;
            guna2DataGridView1.RowTemplate.Height = 35;

            // Header styling
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = gridThemeColor;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Inter", 10, FontStyle.Bold);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = DarkenColor(gridThemeColor, 0.15f);

            // Rows styling
            guna2DataGridView1.DefaultCellStyle.Font = new Font("Inter", 9, FontStyle.Regular);
            guna2DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            guna2DataGridView1.DefaultCellStyle.BackColor = Color.White;
            guna2DataGridView1.DefaultCellStyle.SelectionBackColor = LightenColor(gridThemeColor, 0.3f);
            guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            Color lightPatternColor = LightenColor(gridThemeColor, 0.7f);
            guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = lightPatternColor;
            guna2DataGridView1.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

            // Keep log_date visible and avoid action column eating all width
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

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

        private void ApplyThemeColor(Color themeColor)
        {
            // Same main gradient as Settings
            if (guna2GradientPanel1 != null)
            {
                var darker = GetMainGradientTopColor(themeColor);
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

        private Color GetMainGradientTopColor(Color themeColor)
        {
            var designerMainColor = Color.FromArgb(208, 228, 150);
            if (themeColor.ToArgb() == designerMainColor.ToArgb())
                return Color.FromArgb(48, 79, 99);

            return DarkenColor(themeColor, 0.35f);
        }

        private Color GetDataGridThemeColor()
        {
            var designerMainColor = Color.FromArgb(208, 228, 150);
            if (currentThemeColor.ToArgb() == designerMainColor.ToArgb())
                return Color.FromArgb(48, 79, 99);

            return currentThemeColor;
        }

        private void Dashboard_Click(object sender, EventArgs e)
        {
            DashboardForm dashboardForm = new DashboardForm();
            dashboardForm.Show();
            this.Close();
        }

        private void Attendance_Click(object sender, EventArgs e)
        {
            AttendanceForm attendanceForm = new AttendanceForm();
            attendanceForm.Show();
            this.Close();
        }

        private void StudentsID_Click(object sender, EventArgs e)
        {
            StudentIDForm studentIDForm = new StudentIDForm();
            studentIDForm.Show();
            this.Close();
        }
    }
}