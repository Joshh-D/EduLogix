using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Linq;

namespace EduLogix
{
    /// <summary>
    /// Student ID Form - Displays student list with filtering and navigation to StudentInfo
    /// </summary>
    public partial class StudentIDForm : Form
    {
        private readonly string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";
        private Color currentThemeColor = Color.FromArgb(33, 150, 243);

        public StudentIDForm()
        {
            InitializeComponent();

            // CRITICAL: Wire the Load event manually
            this.Load += StudentIDForm_Load;

            // CRITICAL: Override designer defaults IMMEDIATELY after InitializeComponent
            // This must happen BEFORE any other theme logic
            if (guna2GradientPanel1 != null)
            {
                // Force default colors (will be overridden by DB colors)
                guna2GradientPanel1.FillColor = Color.FromArgb(21, 97, 157);  // Dark blue
                guna2GradientPanel1.FillColor2 = Color.FromArgb(33, 150, 243); // Light blue
                guna2GradientPanel1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                
                System.Diagnostics.Debug.WriteLine("[Constructor] Overrode guna2GradientPanel1 designer defaults");
            }

            if (!DesignMode)
            {
                LoadThemeFromDatabase();
                MarkActiveNav();
            }
        }

        private void StudentIDForm_Load(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("========== StudentIDForm_Load START ==========");

                guna2DataGridView1.ClearSelection();
                LoadThemeFromDatabase();
                LoadStudentData();
                ConfigureDataGridView();
                InitializeFilters();

                // DEBUG: Check combobox state after initialization
                System.Diagnostics.Debug.WriteLine($"[Load] combobox1.Items.Count: {combobox1?.Items.Count}");
                System.Diagnostics.Debug.WriteLine($"[Load] combobox1.SelectedIndex: {combobox1?.SelectedIndex}");
                System.Diagnostics.Debug.WriteLine($"[Load] combobox1.SelectedItem: {combobox1?.SelectedItem}");
                System.Diagnostics.Debug.WriteLine($"[Load] combobox1.Text: '{combobox1?.Text}'");
                
                System.Diagnostics.Debug.WriteLine($"[Load] combobox2.Items.Count: {combobox2?.Items.Count}");
                System.Diagnostics.Debug.WriteLine($"[Load] combobox2.SelectedIndex: {combobox2?.SelectedIndex}");
                System.Diagnostics.Debug.WriteLine($"[Load] combobox2.SelectedItem: {combobox2?.SelectedItem}");
                System.Diagnostics.Debug.WriteLine($"[Load] combobox2.Text: '{combobox2?.Text}'");

                // Force text update after layout is complete
                this.BeginInvoke((MethodInvoker)delegate
                {
                    System.Diagnostics.Debug.WriteLine("[BeginInvoke] Setting text explicitly");
                    if (combobox1 != null && combobox1.Items.Count > 0)
                    {
                        combobox1.SelectedIndex = 0;
                        System.Diagnostics.Debug.WriteLine($"[BeginInvoke] combobox1.SelectedItem after reset: {combobox1.SelectedItem}");
                    }
                    if (combobox2 != null && combobox2.Items.Count > 0)
                    {
                        combobox2.SelectedIndex = 0;
                        System.Diagnostics.Debug.WriteLine($"[BeginInvoke] combobox2.SelectedItem after reset: {combobox2.SelectedItem}");
                    }
                });

                System.Diagnostics.Debug.WriteLine("========== StudentIDForm_Load END ==========");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Student ID form:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region ===== THEME MANAGEMENT =====

        private void LoadThemeFromDatabase()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[LoadThemeFromDatabase] Starting...");
                
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine("[LoadThemeFromDatabase] Connection opened");
                    
                    // FIXED: Load from ID = 1 (user's selected theme) NOT ID = 2
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
                            
                            System.Diagnostics.Debug.WriteLine($"[LoadThemeFromDatabase] Theme loaded from ID=1: R={r}, G={g}, B={b}");
                            System.Diagnostics.Debug.WriteLine($"[LoadThemeFromDatabase] Color: {currentThemeColor.Name}");
                            
                            ApplyThemeColor(currentThemeColor);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("[LoadThemeFromDatabase] No theme found in database (ID=1), using default");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadThemeFromDatabase] ERROR: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates theme to new colors (called from Settings form)
        /// </summary>
        public void UpdateThemeGradient(Color primaryColor, Color complementaryColor)
        {
            System.Diagnostics.Debug.WriteLine($"[UpdateThemeGradient] Called with R={primaryColor.R}, G={primaryColor.G}, B={primaryColor.B}");
            currentThemeColor = primaryColor;
            ApplyThemeColor(primaryColor);
        }

        private void ApplyThemeColor(Color themeColor)
        {
            System.Diagnostics.Debug.WriteLine($"[ApplyThemeColor] Applying color: R={themeColor.R}, G={themeColor.G}, B={themeColor.B}");
            
            // Main gradient background
            if (guna2GradientPanel1 != null)
            {
                var darker = DarkenColor(themeColor, 0.35f);
                
                // Set colors directly - no ThemeStyle property exists
                guna2GradientPanel1.FillColor = darker;
                guna2GradientPanel1.FillColor2 = themeColor;
                guna2GradientPanel1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                
                // Force immediate repaint
                guna2GradientPanel1.Invalidate();
                guna2GradientPanel1.Refresh();
                
                System.Diagnostics.Debug.WriteLine($"[ApplyThemeColor] Gradient panel - FillColor (darker): R={darker.R}, G={darker.G}, B={darker.B}");
                System.Diagnostics.Debug.WriteLine($"[ApplyThemeColor] Gradient panel - FillColor2: R={themeColor.R}, G={themeColor.G}, B={themeColor.B}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[ApplyThemeColor] ERROR: guna2GradientPanel1 is NULL");
            }

            // Navigation buttons - only inactive ones get theme color
            ApplyThemeToNavButton(Dashboard, themeColor);
            ApplyThemeToNavButton(Attendance, themeColor);
            ApplyThemeToNavButton(StudentsID, themeColor);
            ApplyThemeToNavButton(Logs, themeColor);

            // Re-configure DataGridView with new theme
            if (guna2DataGridView1 != null && guna2DataGridView1.Rows.Count > 0)
            {
                ConfigureDataGridView();
            }

            if (guna2GradientPanel2 != null)
            {
                guna2GradientPanel2.FillColor = Color.White;
                guna2GradientPanel2.FillColor2 = LightenColor(themeColor, 0.9f);
            }

            // CRITICAL: Re-apply active nav styling after theme is applied
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

        #endregion

        #region ===== FILTERS =====

        private void InitializeFilters()
        {
            System.Diagnostics.Debug.WriteLine("[InitializeFilters] Starting");

            // Wire search textbox
            if (guna2TextBox1 != null)
            {
                guna2TextBox1.TextChanged += (s, args) => ApplyFilters();
                System.Diagnostics.Debug.WriteLine("[InitializeFilters] Wired guna2TextBox1");
            }

            // Initialize level filter combobox
            if (combobox1 != null)
            {
                System.Diagnostics.Debug.WriteLine("[InitializeFilters] combobox1 is not null");
                combobox1.Items.Clear();
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Cleared combobox1. Items.Count: {combobox1.Items.Count}");
                
                combobox1.Items.AddRange(new[] { "All", "Elementary", "Junior", "Senior" });
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Added items to combobox1. Items.Count: {combobox1.Items.Count}");
                
                combobox1.SelectedIndex = 0;
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Set SelectedIndex=0. SelectedItem: {combobox1.SelectedItem}");
                
                combobox1.SelectedIndexChanged += (s, args) => ApplyFilters();
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Wired SelectedIndexChanged");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[InitializeFilters] ERROR: combobox1 is NULL!");
            }

            // Initialize section filter combobox
            if (combobox2 != null)
            {
                System.Diagnostics.Debug.WriteLine("[InitializeFilters] combobox2 is not null");
                combobox2.Items.Clear();
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Cleared combobox2. Items.Count: {combobox2.Items.Count}");
                
                combobox2.Items.AddRange(new[] { "All", "A", "B", "C", "D", "E" });
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Added items to combobox2. Items.Count: {combobox2.Items.Count}");
                
                combobox2.SelectedIndex = 0;
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Set SelectedIndex=0. SelectedItem: {combobox2.SelectedItem}");
                
                combobox2.SelectedIndexChanged += (s, args) => ApplyFilters();
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Wired SelectedIndexChanged");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[InitializeFilters] ERROR: combobox2 is NULL!");
            }
            
            System.Diagnostics.Debug.WriteLine("[InitializeFilters] Complete");
        }

        private void ApplyFilters()
        {
            string searchText = guna2TextBox1?.Text?.Trim() ?? "";
            string levelFilter = combobox1?.SelectedItem?.ToString() ?? "All";
            string sectionFilter = combobox2?.SelectedItem?.ToString() ?? "All";

            LoadStudentData(searchText, levelFilter, sectionFilter);
        }

        #endregion

        #region ===== DATA LOADING =====

        private void LoadStudentData(string searchText = "", string levelFilter = "All", string sectionFilter = "All")
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT 
                                        student_id,
                                        name,
                                        phone,
                                        email,
                                        grade,
                                        section,
                                        level
                                    FROM reg_studentinfo
                                    WHERE 1=1";

                    if (levelFilter != "All")
                    {
                        query += " AND LOWER(level) = @level";
                    }

                    if (sectionFilter != "All")
                    {
                        query += " AND LOWER(section) = @section";
                    }

                    if (!string.IsNullOrEmpty(searchText))
                    {
                        query += " AND (LOWER(name) LIKE @search OR LOWER(student_id) LIKE @search)";
                    }

                    query += " ORDER BY name ASC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        if (levelFilter != "All")
                            cmd.Parameters.AddWithValue("@level", levelFilter.ToLower());

                        if (sectionFilter != "All")
                            cmd.Parameters.AddWithValue("@section", sectionFilter.ToLower());

                        if (!string.IsNullOrEmpty(searchText))
                            cmd.Parameters.AddWithValue("@search", "%" + searchText.ToLower() + "%");

                        using (var da = new MySqlDataAdapter(cmd))
                        {
                            var dt = new DataTable();
                            da.Fill(dt);
                            guna2DataGridView1.DataSource = dt;

                            // Update total count
                            if (attendancetotal != null)
                                attendancetotal.Text = "Total: " + dt.Rows.Count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading student data:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region ===== DATAGRIDVIEW CONFIGURATION =====

        private void ConfigureDataGridView()
        {
            System.Diagnostics.Debug.WriteLine($"[ConfigureDataGridView] Starting with currentThemeColor: R={currentThemeColor.R}, G={currentThemeColor.G}, B={currentThemeColor.B}");
            
            guna2DataGridView1.EnableHeadersVisualStyles = false;
            guna2DataGridView1.ColumnHeadersHeight = 40;
            
            // FIXED SIZE - Disable all resizing
            guna2DataGridView1.RowTemplate.Height = 35;
            guna2DataGridView1.AllowUserToResizeRows = false;
            guna2DataGridView1.AllowUserToResizeColumns = false;
            guna2DataGridView1.AllowUserToDeleteRows = false;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.RowHeadersVisible = false;

            // Header styling with theme color
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = currentThemeColor;
            System.Diagnostics.Debug.WriteLine($"[ConfigureDataGridView] Set ColumnHeadersDefaultCellStyle.BackColor to: R={currentThemeColor.R}, G={currentThemeColor.G}, B={currentThemeColor.B}");
            
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

            // Rename columns
            RenameColumns();

            // Clear selection
            guna2DataGridView1.ClearSelection();
            
            System.Diagnostics.Debug.WriteLine("[ConfigureDataGridView] Complete");
        }

        private void RenameColumns()
        {
            if (guna2DataGridView1.Columns.Contains("student_id"))
                guna2DataGridView1.Columns["student_id"].HeaderText = "Student ID";

            if (guna2DataGridView1.Columns.Contains("name"))
                guna2DataGridView1.Columns["name"].HeaderText = "Name";

            if (guna2DataGridView1.Columns.Contains("phone"))
                guna2DataGridView1.Columns["phone"].HeaderText = "Phone";

            if (guna2DataGridView1.Columns.Contains("email"))
                guna2DataGridView1.Columns["email"].HeaderText = "Email";

            if (guna2DataGridView1.Columns.Contains("grade"))
                guna2DataGridView1.Columns["grade"].HeaderText = "Grade";

            if (guna2DataGridView1.Columns.Contains("section"))
                guna2DataGridView1.Columns["section"].HeaderText = "Section";

            if (guna2DataGridView1.Columns.Contains("level"))
                guna2DataGridView1.Columns["level"].HeaderText = "Level";
        }

        #endregion

        #region ===== EVENT HANDLERS =====

        private void guna2DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                System.Diagnostics.Debug.WriteLine($"[guna2DataGridView1_CellDoubleClick] Row index: {e.RowIndex}, Column index: {e.ColumnIndex}");
                
                DataGridViewRow row = guna2DataGridView1.Rows[e.RowIndex];
                string studentId = row.Cells["student_id"]?.Value?.ToString();

                System.Diagnostics.Debug.WriteLine($"[guna2DataGridView1_CellDoubleClick] Student ID: {studentId}");

                if (!string.IsNullOrEmpty(studentId))
                {
                    StudentInfo studentInfoForm = new StudentInfo();
                    studentInfoForm.LoadStudentInfo(studentId);
                    studentInfoForm.Show();
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening student information:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"[guna2DataGridView1_CellDoubleClick] ERROR: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Empty - DataGridView is read-only
        }

        #endregion

        #region ===== NAVIGATION =====

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
            if (Logs != null)
            {
                Logs.Checked = false;
                Logs.FillColor = Color.Transparent;
            }

            // Active button white
            if (StudentsID != null)
            {
                StudentsID.Checked = true;
                StudentsID.FillColor = Color.White;
                StudentsID.ForeColor = Color.Black;
                StudentsID.CheckedState.FillColor = Color.White;
                StudentsID.CheckedState.ForeColor = Color.Black;
            }
        }

        private void Dashboard_Click(object sender, EventArgs e)
        {
            var form = new DashboardForm();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = this.Location;
            form.FormClosed += (s, args) => this.Close();
            form.Show();
            this.Hide();
        }

        private void Attendance_Click(object sender, EventArgs e)
        {
            var form = new AttendanceForm();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = this.Location;
            form.FormClosed += (s, args) => this.Close();
            form.Show();
            this.Hide();
        }

        private void Logs_Click_1(object sender, EventArgs e)
        {
            var form = new Logs();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = this.Location;
            form.FormClosed += (s, args) => this.Close();
            form.Show();
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

        #endregion

        #region ===== FILTER EVENT HANDLERS =====

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void combobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void combobox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        #endregion

        #region ===== COLOR HELPER METHODS =====

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
    }
}