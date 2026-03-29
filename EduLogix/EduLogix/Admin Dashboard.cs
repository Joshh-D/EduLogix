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
        private int lastCountRefreshSecond = -1;

        public DashboardForm()
        {
            InitializeComponent();
            InitializeUserOptionsPanel();
            WireDashboardCardClicks();
            this.Load += DashboardForm_Load;
            InitializeDateTimeTimer();
        }

        private void WireDashboardCardClicks()
        {
            WireCardAndChildrenClick(elementaryCount, ElementaryCount_Click);
            WireCardAndChildrenClick(juniorCount, JuniorCount_Click);
            WireCardAndChildrenClick(seniorCount, SeniorCount_Click);
            WireCardAndChildrenClick(inPremisesCount, InPremisesCount_Click);
            WireCardAndChildrenClick(arrivalsCount, ArrivalsCount_Click);
            WireCardAndChildrenClick(departedCount, DepartedCount_Click);
        }

        private void WireCardAndChildrenClick(Control control, EventHandler handler)
        {
            if (control == null || handler == null) return;

            control.Click -= handler;
            control.Click += handler;

            foreach (Control child in control.Controls)
            {
                WireCardAndChildrenClick(child, handler);
            }
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

            var panelLogoutButton = userOptions != null ? userOptions.Controls["logout"] as Button : null;
            if (panelLogoutButton != null)
            {
                panelLogoutButton.Click -= UserOptionsLogout_Click;
                panelLogoutButton.Click += UserOptionsLogout_Click;
            }

            var panelkioskButton = userOptions != null ? userOptions.Controls["kiosk"] as Button : null;
            if (panelkioskButton != null)
            {
                panelkioskButton.Click -= UserOptionskiosk_Click;
                panelkioskButton.Click += UserOptionskiosk_Click;
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

            OpenSettingsForm();
        }

        private void UserOptionsLogout_Click(object sender, EventArgs e)
        {
            if (userOptions != null)
                userOptions.Visible = false;

            Logout_Click(sender, e);
        }

        private void UserOptionskiosk_Click(object sender, EventArgs e)
        {
            if (userOptions != null)
                userOptions.Visible = false;

            var form = new Kiosk();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = this.Location;
            form.Show();
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

            int currentSecond = DateTime.Now.Second;
            if (currentSecond % 10 == 0 && currentSecond != lastCountRefreshSecond)
            {
                lastCountRefreshSecond = currentSecond;
                RefreshDashboardCounts();
            }
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            ApplyThemeToForm();
            BrandingHelper.ApplySchoolBranding(connectionString, schoolName, schoolLogo);
            ApplyUserIdentityLabels();
            MarkActiveNav();
            if (dashboardDateAndTime != null)
            {
                dashboardDateAndTime.Text = DateTime.Now.ToString("MMMM dd, yyyy hh:mm:ss tt");
            }
            RefreshDashboardCounts();
            SetupAttendanceChart();
        }

        private void RefreshDashboardCounts()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string query = @"
                        SELECT
                            SUM(CASE WHEN LOWER(education) = 'elementary' THEN 1 ELSE 0 END) AS elementary_count,
                            SUM(CASE WHEN LOWER(education) = 'junior' THEN 1 ELSE 0 END) AS junior_count,
                            SUM(CASE WHEN LOWER(education) = 'senior' THEN 1 ELSE 0 END) AS senior_count,
                            SUM(CASE WHEN status = 'In Premises' THEN 1 ELSE 0 END) AS in_premises_count,
                            SUM(CASE WHEN status = 'Departed' THEN 1 ELSE 0 END) AS departed_count,
                            COUNT(*) AS arrivals_count,
                            (SELECT COUNT(*) FROM reg_studentinfo) AS total_students
                        FROM reg_attendance_live";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return;

                        int elementary = ToInt(reader["elementary_count"]);
                        int junior = ToInt(reader["junior_count"]);
                        int senior = ToInt(reader["senior_count"]);
                        int inPremises = ToInt(reader["in_premises_count"]);
                        int departed = ToInt(reader["departed_count"]);
                        int arrivals = ToInt(reader["arrivals_count"]);
                        int totalStudents = ToInt(reader["total_students"]);

                        if (elemNum != null) elemNum.Text = elementary.ToString();
                        if (juniorNum != null) juniorNum.Text = junior.ToString();
                        if (seniorNum != null) seniorNum.Text = senior.ToString();
                        if (inPremisesNum != null) inPremisesNum.Text = inPremises.ToString();
                        if (arrivalsNum != null) arrivalsNum.Text = arrivals.ToString();
                        if (departedNum != null) departedNum.Text = departed.ToString();

                        UpdateDailyAttendanceCircle(totalStudents, arrivals);
                    }
                }
            }
            catch
            {
                // Keep dashboard responsive if DB is temporarily unavailable.
            }
        }

        private int ToInt(object value)
        {
            if (value == null || value == DBNull.Value) return 0;
            return Convert.ToInt32(value);
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
            if (role != null)
                role.ForeColor = Color.LightGray;
            if (username != null)
                username.ForeColor = Color.White;

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

       

        private void Logs_Click(object sender, EventArgs e)
        {
            Logs log = new Logs();
            log.Show();
            this.Hide();
        }

        private void settings_Click(object sender, EventArgs e)
        {
            OpenSettingsForm();
        }

        private void OpenSettingsForm()
        {
            try
            {
                var form = Application.OpenForms.OfType<Settings>().FirstOrDefault();
                if (form == null)
                    form = new Settings();

                form.StartPosition = FormStartPosition.Manual;
                form.Location = this.Location;
                form.Show();
                form.BringToFront();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open settings:\n" + ex.Message, "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyUserIdentityLabels()
        {
            string userName = string.IsNullOrWhiteSpace(UserSession.UserName) ? "Username" : UserSession.UserName;
            string role = string.IsNullOrWhiteSpace(UserSession.Role) ? "Role" : UserSession.Role;

            if (username != null)
                username.Text = userName;

            if (this.role != null)
                this.role.Text = role;
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
            attendanceChartWeekly.Series.Clear();
            attendanceChartWeekly.ChartAreas[0].AxisY.Maximum = 500;
            attendanceChartWeekly.ChartAreas[0].AxisY.Minimum = 0;
            attendanceChartWeekly.ChartAreas[0].AxisY.Interval = 50;
            attendanceChartWeekly.ChartAreas[0].AxisY.Title = "";

            Series elementary = new Series("Elementary");
            elementary.ChartType = SeriesChartType.Column;
            elementary.Color = Color.FromArgb(233, 188, 119);
            elementary.IsValueShownAsLabel = true;

            Series junior = new Series("Junior");
            junior.ChartType = SeriesChartType.Column;
            junior.Color = Color.FromArgb(228, 185, 169);
            junior.IsValueShownAsLabel = true;

            Series senior = new Series("Senior");
            senior.ChartType = SeriesChartType.Column;
            senior.Color = Color.FromArgb(148, 191, 200);
            senior.IsValueShownAsLabel = true;

            attendanceChartWeekly.Series.Add(elementary);
            attendanceChartWeekly.Series.Add(junior);
            attendanceChartWeekly.Series.Add(senior);

            string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            int[] elemData = new int[days.Length];
            int[] juniorData = new int[days.Length];
            int[] seniorData = new int[days.Length];

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    const string query = @"
                        SELECT DAYOFWEEK(date_and_time) AS day_of_week,
                               LOWER(education) AS education,
                               COUNT(*) AS total_count
                        FROM reg_attendance_weekly
                        WHERE date_and_time IS NOT NULL
                        GROUP BY DAYOFWEEK(date_and_time), LOWER(education)";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int dayOfWeek = Convert.ToInt32(reader["day_of_week"]);
                            string education = reader["education"] == DBNull.Value
                                ? string.Empty
                                : reader["education"].ToString();
                            int totalCount = reader["total_count"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(reader["total_count"]);

                            int dayIndex = dayOfWeek - 2; // Monday=2 -> 0, Saturday=7 -> 5
                            if (dayIndex < 0 || dayIndex >= days.Length) continue;

                            if (education == "elementary") elemData[dayIndex] = totalCount;
                            else if (education == "junior") juniorData[dayIndex] = totalCount;
                            else if (education == "senior") seniorData[dayIndex] = totalCount;
                        }
                    }
                }
            }
            catch
            {
                // Keep chart alive even if DB is temporarily unavailable.
            }

            for (int i = 0; i < days.Length; i++)
            {
                elementary.Points.AddXY(days[i], elemData[i]);
                junior.Points.AddXY(days[i], juniorData[i]);
                senior.Points.AddXY(days[i], seniorData[i]);
            }

            int maxValue = Math.Max(
                elemData.Concat(juniorData).Concat(seniorData).DefaultIfEmpty(0).Max(),
                10);
            int roundedMax = ((maxValue + 9) / 10) * 10;
            attendanceChartWeekly.ChartAreas[0].AxisY.Maximum = roundedMax;
            attendanceChartWeekly.ChartAreas[0].AxisY.Interval = Math.Max(1, roundedMax / 10);

            foreach (Series s in attendanceChartWeekly.Series)
            {
                s["PointWidth"] = "0.6";
            }
        }
        private void UpdateDailyAttendanceCircle(int totalStudents, int studentsPresentToday)
        {
            if (totalStudents <= 0)
            {
                if (attendanceChartDaily != null)
                {
                    attendanceChartDaily.Value = 0;
                    attendanceChartDaily.Text = "0% Present";
                }
                return;
            }

            double percentage = ((double)studentsPresentToday / totalStudents) * 100;

            attendanceChartDaily.Value = 100;
            attendanceChartDaily.FillColor = Color.LightGray;
            attendanceChartDaily.ProgressColor = Color.Red;
            attendanceChartDaily.ProgressThickness = 50;
            attendanceChartDaily.InnerColor = Color.White;

            attendanceChartDaily.Value = Math.Max(0, Math.Min(100, (int)Math.Round(percentage)));
            attendanceChartDaily.ProgressColor = Color.Green;
            attendanceChartDaily.ProgressThickness = 50;
            attendanceChartDaily.InnerColor = Color.White;
            attendanceChartDaily.Text = $"{percentage:0}% Present";
            attendanceChartDaily.Font = new Font("Inter", 16, FontStyle.Regular);
            attendanceChartDaily.ForeColor = Color.FromArgb(48, 79, 99);
            attendanceChartDaily.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        }

        private void OpenAttendanceWithFilters(string levelFilter, string statusFilter)
        {
            var attendance = new AttendanceForm(levelFilter, statusFilter);
            attendance.StartPosition = FormStartPosition.Manual;
            attendance.Location = this.Location;
            attendance.Show();
            this.Hide();
        }

        private void ElementaryCount_Click(object sender, EventArgs e)
        {
            OpenAttendanceWithFilters("Elementary", "All");
        }

        private void JuniorCount_Click(object sender, EventArgs e)
        {
            OpenAttendanceWithFilters("Junior", "All");
        }

        private void SeniorCount_Click(object sender, EventArgs e)
        {
            OpenAttendanceWithFilters("Senior", "All");
        }

        private void InPremisesCount_Click(object sender, EventArgs e)
        {
            OpenAttendanceWithFilters("All", "In Premises");
        }

        private void ArrivalsCount_Click(object sender, EventArgs e)
        {
            OpenAttendanceWithFilters("All", "All");
        }

        private void DepartedCount_Click(object sender, EventArgs e)
        {
            OpenAttendanceWithFilters("All", "Departed");
        }

        private void userProfile_Click(object sender, EventArgs e)
        {
            if (userOptions == null) return;

            PositionUserOptionsPanel();
            userOptions.Visible = !userOptions.Visible;
            if (userOptions.Visible)
                userOptions.BringToFront();
        }

        private void elementaryCount_Paint(object sender, PaintEventArgs e)
        {

        }

        private void juniorCount_Paint(object sender, PaintEventArgs e)
        {

        }

        private void seniorCount_Paint(object sender, PaintEventArgs e)
        {

        }

        private void inPremisesCount_Paint(object sender, PaintEventArgs e)
        {

        }

        private void arrivalsCount_Paint(object sender, PaintEventArgs e)
        {

        }

        private void departedCount_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DashboardForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}