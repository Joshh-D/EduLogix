using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Windows.Forms.DataVisualization.Charting;

namespace EduLogix
{
    public partial class DashboardForm : Form
    {
        private string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";
        private Timer dateTimeTimer;

        public DashboardForm()
        {
            InitializeComponent();
            this.Load += DashboardForm_Load;
            InitializeDateTimeTimer();
        }

        private void InitializeDateTimeTimer()
        {
            dateTimeTimer = new Timer();
            dateTimeTimer.Interval = 1000;
            dateTimeTimer.Tick += DateTimeTimer_Tick;
            dateTimeTimer.Start();
        }

        private void DateTimeTimer_Tick(object sender, EventArgs e)
        {
            if (dashboardDateAndTime != null)
            {
                dashboardDateAndTime.Text = DateTime.Now.ToString("MMMM dd, yyyy hh:mm:ss tt");
            }
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            ApplyThemeToForm();
            if (dashboardDateAndTime != null)
            {
                dashboardDateAndTime.Text = DateTime.Now.ToString("MMMM dd, yyyy hh:mm:ss tt");
            }
            SetupAttendanceChart();
            UpdateDailyAttendanceCircle();
        }

        private void ApplyThemeToForm()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT theme_red, theme_green, theme_blue FROM reg_theme WHERE id = 2";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int r = Convert.ToInt32(reader["theme_red"]);
                        int g = Convert.ToInt32(reader["theme_green"]);
                        int b = Convert.ToInt32(reader["theme_blue"]);
                        Color themeColor = Color.FromArgb(r, g, b);
                        this.BackColor = themeColor;
                        ApplyThemeToButtons(themeColor);
                        ApplyThemeToControlBoxes(themeColor);
                        ApplyThemeToLabels(themeColor);
                    }
                }
            }
            catch (Exception) { }
        }

        private void ApplyThemeToButtons(Color themeColor)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2Button btn)
                {
                    if (btn.Name == "Dashboard" || btn.Name == "Attendance" ||
                        btn.Name == "StudentsID" || btn.Name == "Accounts" ||
                        btn.Name == "Logs" || btn.Name == "Settings" || btn.Name == "Logout")
                    {
                        btn.FillColor = themeColor;
                    }
                }
            }
        }

        private void ApplyThemeToControlBoxes(Color themeColor)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2ControlBox ctrlBox)
                {
                    ctrlBox.FillColor = themeColor;
                }
            }
        }

        private void ApplyThemeToLabels(Color themeColor)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2HtmlLabel htmlLabel)
                {
                    if (htmlLabel.Name == "UserName" || htmlLabel.Name == "guna2HtmlLabel1")
                    {
                        htmlLabel.BackColor = themeColor;
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (dateTimeTimer != null)
            {
                dateTimeTimer.Stop();
                dateTimeTimer.Dispose();
            }
            base.OnFormClosing(e);
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

        private void Logout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                new Login().Show();
                this.Hide();
            }
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e) { }

        private void guna2HtmlLabel4_Click(object sender, EventArgs e) { }

        private void guna2CustomGradientPanel1_Paint(object sender, PaintEventArgs e) { }

        private void SetupAttendanceChart()
        {
            // Clear previous series
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Maximum = 500;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Interval = 50;

            // Remove Y-axis title
            chart1.ChartAreas[0].AxisY.Title = "";

            // Create series for Elementary
            Series elementary = new Series("Elementary");
            elementary.ChartType = SeriesChartType.Column;
            elementary.Color = Color.FromArgb(233, 188, 119);
            elementary.IsValueShownAsLabel = true;

            // Create series for Junior
            Series junior = new Series("Junior");
            junior.ChartType = SeriesChartType.Column;
            junior.Color = Color.FromArgb(228, 185, 169);
            junior.IsValueShownAsLabel = true;

            // Create series for Senior
            Series senior = new Series("Senior");
            senior.ChartType = SeriesChartType.Column;
            senior.Color = Color.FromArgb(148, 191, 200);
            senior.IsValueShownAsLabel = true;

            // Add series to chart
            chart1.Series.Add(elementary);
            chart1.Series.Add(junior);
            chart1.Series.Add(senior);

            // Add points for each day
            string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            int[] elemData = { 320, 450, 330, 410, 440, 460 };   // Example numbers for Elementary
            int[] juniorData = { 420, 340, 420, 390, 410, 430 };   // Example numbers for Junior
            int[] seniorData = { 470, 460, 450, 380, 400, 430 };   // Example numbers for Senior

            for (int i = 0; i < days.Length; i++)
            {
                elementary.Points.AddXY(days[i], elemData[i]);
                junior.Points.AddXY(days[i], juniorData[i]);
                senior.Points.AddXY(days[i], seniorData[i]);
            }

            // Adjust column width
            foreach (Series s in chart1.Series)
            {
                s["PointWidth"] = "0.6";
            }
        }
        private void UpdateDailyAttendanceCircle()
        {
            int totalStudents = 500;
            int studentsPresentToday = 420; // replace with database value if needed

            double percentage = ((double)studentsPresentToday / totalStudents) * 100;
            double absentPercentage = 100 - percentage;

            guna2CircleProgressBar1.Value = 100;
            guna2CircleProgressBar1.FillColor = Color.LightGray;
            guna2CircleProgressBar1.ProgressColor = Color.Red;
            guna2CircleProgressBar1.ProgressThickness = 50;
            guna2CircleProgressBar1.InnerColor = Color.White;

            guna2CircleProgressBar1.Value = (int)percentage;
            guna2CircleProgressBar1.ProgressColor = Color.Green;
            guna2CircleProgressBar1.ProgressThickness = 50;
            guna2CircleProgressBar1.InnerColor = Color.White;
            guna2CircleProgressBar1.Text = $"{percentage:0}% Present";
            guna2CircleProgressBar1.Font = new Font("Inter", 16, FontStyle.Regular);
            guna2CircleProgressBar1.ForeColor = Color.FromArgb(48, 79, 99);
            guna2CircleProgressBar1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        }
    }
}