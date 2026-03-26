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
            InitializeUserOptionsPanel();
            this.Load += DashboardForm_Load;
            InitializeDateTimeTimer();
        }

        private void InitializeUserOptionsPanel()
        {
            if (userOptions != null)
            {
                userOptions.Visible = false;
                PositionUserOptionsPanel();
                userOptions.BringToFront();
            }

            if (settings != null)
            {
                settings.Click -= UserOptionsSettings_Click;
                settings.Click += UserOptionsSettings_Click;
            }

            WireOutsideClickHandler(this);
        }

        private void PositionUserOptionsPanel()
        {
            if (userOptions == null || userProfile == null || userOptions.Parent == null) return;

            var parent = userOptions.Parent;
            int x = userProfile.Right + 8;
            int y = userProfile.Top + Math.Max(0, (userProfile.Height - userOptions.Height) / 2);

            if (x + userOptions.Width > parent.ClientSize.Width)
                x = Math.Max(0, userProfile.Left - userOptions.Width - 8);

            if (y + userOptions.Height > parent.ClientSize.Height)
                y = Math.Max(0, parent.ClientSize.Height - userOptions.Height - 8);

            userOptions.Location = new Point(Math.Max(0, x), Math.Max(0, y));
        }

        private void WireOutsideClickHandler(Control parent)
        {
            if (parent == null) return;

            bool isUserOptionsPanel = userOptions != null && parent == userOptions;
            bool isInsideUserOptionsPanel = IsInsideUserOptions(parent);

            if (!isUserOptionsPanel && !isInsideUserOptionsPanel)
            {
                parent.MouseDown -= OutsideUserOptions_MouseDown;
                parent.MouseDown += OutsideUserOptions_MouseDown;
            }

            foreach (Control child in parent.Controls)
            {
                WireOutsideClickHandler(child);
            }
        }

        private bool IsInsideUserOptions(Control control)
        {
            if (control == null || userOptions == null) return false;

            var current = control.Parent;
            while (current != null)
            {
                if (current == userOptions) return true;
                current = current.Parent;
            }

            return false;
        }

        private void OutsideUserOptions_MouseDown(object sender, MouseEventArgs e)
        {
            if (userOptions == null || !userOptions.Visible) return;

            Point clickPoint = System.Windows.Forms.Cursor.Position;
            bool clickedInsidePanel = userOptions.RectangleToScreen(userOptions.ClientRectangle).Contains(clickPoint);
            bool clickedUserProfile = userProfile != null && userProfile.RectangleToScreen(userProfile.ClientRectangle).Contains(clickPoint);

            if (!clickedInsidePanel && !clickedUserProfile)
                userOptions.Visible = false;
        }

        private void UserOptionsSettings_Click(object sender, EventArgs e)
        {
            if (userOptions != null)
                userOptions.Visible = false;

            var form = new Settings();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = this.Location;
            form.Show();
            this.Hide();
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
            BrandingHelper.ApplySchoolBranding(connectionString, schoolName, schoolLogo);
            MarkActiveNav();
            if (dashboardDateAndTime != null)
            {
                dashboardDateAndTime.Text = DateTime.Now.ToString("MMMM dd, yyyy hh:mm:ss tt");
            }
            SetupAttendanceChart();
            UpdateDailyAttendanceCircle();
        }

        private void MarkActiveNav()
        {
            // Uncheck others and make them transparent
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
            if (Logs != null)
            {
                Logs.Checked = false;
                Logs.FillColor = Color.Transparent;
            }

            // Active button white
            if (Dashboard != null)
            {
                Dashboard.Checked = true;
                Dashboard.FillColor = Color.White;
                Dashboard.ForeColor = Color.Black;
                Dashboard.CheckedState.FillColor = Color.White;
                Dashboard.CheckedState.ForeColor = Color.Black;
            }
        }

        private void ApplyThemeToForm()
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

                        ApplyThemeColor(themeColor);
                    }
                }
            }
            catch (Exception)
            {
                // keep silent to avoid breaking designer
            }
        }

        private void ApplyThemeColor(Color themeColor)
        {
            // main background gradient panel
            if (guna2GradientPanel7 != null)
            {
                if (themeColor.ToArgb() == Color.FromArgb(208, 228, 150).ToArgb())
                {
                    guna2GradientPanel7.FillColor = Color.FromArgb(48, 79, 99);
                    guna2GradientPanel7.FillColor2 = Color.FromArgb(208, 228, 150);
                }
                else
                {
                    guna2GradientPanel7.FillColor = themeColor;
                    guna2GradientPanel7.FillColor2 = LightenColor(themeColor, 0.4f);
                }
            }

            // inner content gradient panel
            if (guna2GradientPanel8 != null)
            {
                guna2GradientPanel8.FillColor = Color.White;
                guna2GradientPanel8.FillColor2 = LightenColor(themeColor, 0.9f);
            }

            // nav buttons on left sidebar
            ApplyNavTheme(Dashboard);
            ApplyNavTheme(Attendance);
            ApplyNavTheme(StudentsID);
            ApplyNavTheme(Logs);

            // top-right control boxes
            if (guna2ControlBox4 != null)
            {
                guna2ControlBox4.FillColor = Color.Transparent;
                guna2ControlBox4.IconColor = Color.White;
            }

            if (guna2ControlBox5 != null)
            {
                guna2ControlBox5.FillColor = Color.Transparent;
                guna2ControlBox5.IconColor = Color.White;
            }

            // labels over sidebar
            if (guna2HtmlLabel1 != null)
                guna2HtmlLabel1.ForeColor = Color.LightGray;
            if (guna2HtmlLabel17 != null)
                guna2HtmlLabel17.ForeColor = Color.White;

            void ApplyNavTheme(Guna.UI2.WinForms.Guna2Button btn)
            {
                if (btn == null) return;
                Color normal = themeColor;
                Color checkedColor = LightenColor(themeColor, 0.2f);

                btn.FillColor = normal;
                btn.ForeColor = GetContrastColor(normal);
                btn.CheckedState.FillColor = checkedColor;
                btn.CheckedState.ForeColor = GetContrastColor(checkedColor);
            }
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

        private void userProfile_Click(object sender, EventArgs e)
        {
            if (userOptions == null) return;

            PositionUserOptionsPanel();
            userOptions.Visible = !userOptions.Visible;
            if (userOptions.Visible)
                userOptions.BringToFront();
        }
    }
}