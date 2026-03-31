using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Linq;
using System.IO;
using System.Data;
using ExcelDataReader;

namespace EduLogix
{
    /// <summary>
    /// Student ID Form - Displays student list with filtering and navigation to StudentInfo
    /// </summary>
    public partial class ManageAccounts : Form
    {
        private readonly string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";
        private Color currentThemeColor = Color.FromArgb(33, 150, 243);
        private const string ResetPasswordColumnName = "colResetPassword";
        private const string DefaultResetPasswordValue = "EduLogix";

        private sealed class AccountRowEditState
        {
            public bool IsNew;
            public bool IsEditing;
            public string OriginalUsername;
        }

        public ManageAccounts()
        {
            InitializeComponent();
            InitializeUserOptionsPanel();

            if (manageAccountsDataGrid != null)
            {
                manageAccountsDataGrid.CellContentClick -= guna2DataGridView1_CellContentClick;
                manageAccountsDataGrid.CellContentClick += guna2DataGridView1_CellContentClick;
                manageAccountsDataGrid.SelectionChanged -= manageAccountsDataGrid_SelectionChanged;
                manageAccountsDataGrid.SelectionChanged += manageAccountsDataGrid_SelectionChanged;
            }

            if (editUser != null)
            {
                editUser.Click -= editUser_Click;
                editUser.Click += editUser_Click;
            }

            if (deleteUser != null)
            {
                deleteUser.Click -= deleteUser_Click;
                deleteUser.Click += deleteUser_Click;
            }

            if (saveChanges != null)
            {
                saveChanges.Click -= saveChanges_Click;
                saveChanges.Click += saveChanges_Click;
            }

            // CRITICAL: Wire the Load event manually
            this.Load += ManageAccounts_Load;

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

        }

        private void InitializeUserOptionsPanel()
        {
            if (userOptions != null)
            {
                userOptions.Visible = false;
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

        private void WireOutsideClickHandler(Control parent)
        {
            if (parent == null) return;

            bool isUserOptionsPanel = userOptions != null && parent == userOptions;
            bool isInsideUserOptionsPanel = IsInsideUserOptions(parent);

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

        private void UserOptionsSettings_Click(object sender, EventArgs e)
        {
            if (userOptions != null)
                userOptions.Visible = false;

            var form = new Settings();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = this.Location;
            form.FormClosed += (s, args) => this.Close();
            form.Show();
            this.Hide();
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

        private void ManageAccounts_Load(object sender, EventArgs e)
        {
            if (!UserSession.IsSuperAdmin)
            {
                MessageBox.Show("Only the Super Admin can access Manage Accounts.",
                    "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            try
            {
                // 1. Load UI Themes and Branding First
                LoadThemeFromDatabase();
                BrandingHelper.ApplySchoolBranding(connectionString, schoolName, schoolLogo);

                // 2. Load the Data and Configure the Grid
                LoadStudentData();
                ConfigureDataGridView();

                // Clear selection AFTER the data is actually loaded into the grid
                if (manageAccountsDataGrid != null)
                {
                    manageAccountsDataGrid.ClearSelection();
                }

                // 3. Setup the Dropdowns
                InitializeFilters();

                // 4. Force ComboBox selection update after the form layout is complete.
                // This prevents the "blank text" bug common with custom UI ComboBoxes.
                this.BeginInvoke((MethodInvoker)delegate
                {
                    var roleCombo = GetRoleFilterCombo();
                    if (roleCombo != null && roleCombo.Items.Count > 0)
                    {
                        roleCombo.SelectedIndex = 0;
                    }
                    var sortCombo = GetSortFilterCombo();
                    if (sortCombo != null && sortCombo.Items.Count > 0)
                    {
                        sortCombo.SelectedIndex = 0;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Manage Accounts form:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                var darker = GetMainGradientTopColor(themeColor);
                
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
            if (manageAccountsDataGrid != null && manageAccountsDataGrid.Rows.Count > 0)
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

        private Color GetMainGradientTopColor(Color themeColor)
        {
            var designerMainColor = Color.FromArgb(208, 228, 150);
            if (themeColor.ToArgb() == designerMainColor.ToArgb())
                return Color.FromArgb(48, 79, 99);

            return DarkenColor(themeColor, 0.35f);
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
            var roleCombo = GetRoleFilterCombo();
            if (roleCombo != null)
            {
                System.Diagnostics.Debug.WriteLine("[InitializeFilters] combobox1 is not null");
                roleCombo.Items.Clear();
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Cleared combobox1. Items.Count: {roleCombo.Items.Count}");
                
                roleCombo.Items.AddRange(new[] { "All", "registrar", "librarian", "security", "admin" });
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Added items to combobox1. Items.Count: {roleCombo.Items.Count}");
                
                roleCombo.SelectedIndex = 0;
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Set SelectedIndex=0. SelectedItem: {roleCombo.SelectedItem}");
                
                roleCombo.SelectedIndexChanged += (s, args) => ApplyFilters();
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Wired SelectedIndexChanged");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[InitializeFilters] ERROR: combobox1 is NULL!");
            }

            // Initialize section filter combobox
            var sortCombo = GetSortFilterCombo();
            if (sortCombo != null)
            {
                System.Diagnostics.Debug.WriteLine("[InitializeFilters] combobox2 is not null");
                sortCombo.Items.Clear();
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Cleared combobox2. Items.Count: {sortCombo.Items.Count}");
                
                sortCombo.Items.AddRange(new[] { "Newest", "Oldest" });
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Added items to combobox2. Items.Count: {sortCombo.Items.Count}");
                
                sortCombo.SelectedIndex = 0;
                System.Diagnostics.Debug.WriteLine($"[InitializeFilters] Set SelectedIndex=0. SelectedItem: {sortCombo.SelectedItem}");
                
                sortCombo.SelectedIndexChanged += (s, args) => ApplyFilters();
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
            string levelFilter = GetCurrentRoleFilter();
            string sectionFilter = GetCurrentSortFilter();

            LoadStudentData(searchText, levelFilter, sectionFilter);
        }

        #endregion

        #region ===== DATA LOADING =====

        private void LoadStudentData(string searchText = "", string levelFilter = "All", string sectionFilter = "Newest")
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT 
                                        rfid_number,
                                        username,
                                        password,
                                        role,
                                        created_at
                                    FROM sys_users
                                    WHERE 1=1";

                    if (levelFilter != "All")
                    {
                        query += " AND LOWER(role) = @level";
                    }

                    if (!string.IsNullOrEmpty(searchText))
                    {
                        query += " AND (LOWER(username) LIKE @search OR LOWER(rfid_number) LIKE @search OR LOWER(role) LIKE @search)";
                    }

                    query += sectionFilter == "Oldest"
                        ? " ORDER BY created_at ASC"
                        : " ORDER BY created_at DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        if (levelFilter != "All")
                            cmd.Parameters.AddWithValue("@level", levelFilter.ToLower());

                        if (!string.IsNullOrEmpty(searchText))
                            cmd.Parameters.AddWithValue("@search", "%" + searchText.ToLower() + "%");

                        using (var da = new MySqlDataAdapter(cmd))
                        {
                            var dt = new DataTable();
                            da.Fill(dt);
                            manageAccountsDataGrid.DataSource = dt;
                            EnsureActionColumns();
                            SetGridDefaultReadOnlyState();

                            // Update total count
                            if (attendancetotal != null)
                                attendancetotal.Text = "" + dt.Rows.Count;
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
            Color gridThemeColor = GetDataGridThemeColor();
            Color headerTextColor = GetContrastColor(gridThemeColor);
            Color selectionBackColor = LightenColor(gridThemeColor, 0.3f);
            Color selectionTextColor = GetContrastColor(selectionBackColor);
            
            manageAccountsDataGrid.EnableHeadersVisualStyles = false;
            manageAccountsDataGrid.ColumnHeadersHeight = 40;
            
            // FIXED SIZE - Disable all resizing
            manageAccountsDataGrid.RowTemplate.Height = 35;
            manageAccountsDataGrid.AllowUserToResizeRows = false;
            manageAccountsDataGrid.AllowUserToResizeColumns = false;
            manageAccountsDataGrid.AllowUserToDeleteRows = false;
            manageAccountsDataGrid.AllowUserToAddRows = false;
            manageAccountsDataGrid.RowHeadersVisible = false;

            // Header styling with theme color
            manageAccountsDataGrid.ColumnHeadersDefaultCellStyle.BackColor = gridThemeColor;
            System.Diagnostics.Debug.WriteLine($"[ConfigureDataGridView] Set ColumnHeadersDefaultCellStyle.BackColor to: R={gridThemeColor.R}, G={gridThemeColor.G}, B={gridThemeColor.B}");
            
            manageAccountsDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = headerTextColor;
            manageAccountsDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Inter", 10, FontStyle.Bold);
            manageAccountsDataGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = DarkenColor(gridThemeColor, 0.15f);
            manageAccountsDataGrid.ColumnHeadersDefaultCellStyle.SelectionForeColor = headerTextColor;

            // Data cell styling
            manageAccountsDataGrid.DefaultCellStyle.Font = new Font("Inter", 9, FontStyle.Regular);
            manageAccountsDataGrid.DefaultCellStyle.ForeColor = Color.Black;
            manageAccountsDataGrid.DefaultCellStyle.BackColor = Color.White;

            // Selection styling with THEME COLOR
            manageAccountsDataGrid.DefaultCellStyle.SelectionBackColor = selectionBackColor;
            manageAccountsDataGrid.DefaultCellStyle.SelectionForeColor = selectionTextColor;

            // Keep grid editable at row level only (Edit/Save flow controls cells)
            manageAccountsDataGrid.ReadOnly = false;
            manageAccountsDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            // Alternating row colors with pattern - using theme color
            Color lightPatternColor = LightenColor(gridThemeColor, 0.7f);
            manageAccountsDataGrid.AlternatingRowsDefaultCellStyle.BackColor = lightPatternColor;
            manageAccountsDataGrid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

            // Rename columns
            RenameColumns();

            // Clear selection
            manageAccountsDataGrid.ClearSelection();
            
            System.Diagnostics.Debug.WriteLine("[ConfigureDataGridView] Complete");
        }

        private void RenameColumns()
        {
            if (manageAccountsDataGrid.Columns.Contains("rfid_number"))
                manageAccountsDataGrid.Columns["rfid_number"].HeaderText = "RFID Number";

            if (manageAccountsDataGrid.Columns.Contains("username"))
                manageAccountsDataGrid.Columns["username"].HeaderText = "Username";

            if (manageAccountsDataGrid.Columns.Contains("password"))
                manageAccountsDataGrid.Columns["password"].HeaderText = "Password";

            if (manageAccountsDataGrid.Columns.Contains("role"))
                manageAccountsDataGrid.Columns["role"].HeaderText = "Role";

            if (manageAccountsDataGrid.Columns.Contains("created_at"))
            {
                manageAccountsDataGrid.Columns["created_at"].HeaderText = "Created At";
                manageAccountsDataGrid.Columns["created_at"].DefaultCellStyle.Format = "yyyy-MM-dd hh:mm:ss tt";
            }

            if (manageAccountsDataGrid.Columns.Contains(ResetPasswordColumnName))
                manageAccountsDataGrid.Columns[ResetPasswordColumnName].HeaderText = string.Empty;
        }

        private void EnsureActionColumns()
        {
            if (manageAccountsDataGrid == null) return;

            if (!manageAccountsDataGrid.Columns.Contains(ResetPasswordColumnName))
            {
                var resetPasswordColumn = new DataGridViewButtonColumn
                {
                    Name = ResetPasswordColumnName,
                    HeaderText = string.Empty,
                    UseColumnTextForButtonValue = false
                };
                manageAccountsDataGrid.Columns.Add(resetPasswordColumn);
            }
        }

        private void SetGridDefaultReadOnlyState()
        {
            if (manageAccountsDataGrid == null) return;

            foreach (DataGridViewColumn column in manageAccountsDataGrid.Columns)
            {
                column.ReadOnly = true;
            }

            foreach (DataGridViewRow row in manageAccountsDataGrid.Rows)
            {
                SetRowEditable(row, false);
            }

            UpdateResetPasswordButtons();
        }

        private void manageAccountsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            UpdateResetPasswordButtons();
        }

        private void UpdateResetPasswordButtons()
        {
            if (manageAccountsDataGrid == null || !manageAccountsDataGrid.Columns.Contains(ResetPasswordColumnName))
                return;

            int selectedRowIndex = -1;
            if (manageAccountsDataGrid.SelectedRows.Count > 0)
                selectedRowIndex = manageAccountsDataGrid.SelectedRows[0].Index;

            foreach (DataGridViewRow row in manageAccountsDataGrid.Rows)
            {
                if (row.IsNewRow) continue;
                row.Cells[ResetPasswordColumnName].Value = row.Index == selectedRowIndex ? "Reset" : string.Empty;
            }
        }

        private void SetRowEditable(DataGridViewRow row, bool isEditable)
        {
            if (row == null) return;

            foreach (DataGridViewCell cell in row.Cells)
            {
                string col = manageAccountsDataGrid.Columns[cell.ColumnIndex].Name;
                bool isActionCol = col == ResetPasswordColumnName;
                bool isCreatedAt = col == "created_at";

                cell.ReadOnly = !isEditable || isActionCol || isCreatedAt;
            }
        }

        private AccountRowEditState GetRowState(DataGridViewRow row, bool createIfMissing)
        {
            if (row == null) return null;

            var state = row.Tag as AccountRowEditState;
            if (state == null && createIfMissing)
            {
                state = new AccountRowEditState
                {
                    IsNew = false,
                    IsEditing = false,
                    OriginalUsername = Convert.ToString(row.Cells["username"].Value)
                };
                row.Tag = state;
            }

            return state;
        }

        #endregion

        #region ===== EVENT HANDLERS =====

        private void guna2DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Disabled for Manage Accounts.
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || manageAccountsDataGrid == null) return;
            if (e.ColumnIndex < 0) return;

            string columnName = manageAccountsDataGrid.Columns[e.ColumnIndex].Name;
            if (columnName == ResetPasswordColumnName)
            {
                ResetPassword(e.RowIndex);
                return;
            }
        }

        private void editUser_Click(object sender, EventArgs e)
        {
            int rowIndex = GetSelectedRowIndex();
            if (rowIndex < 0)
            {
                MessageBox.Show("Select a user row first.", "Edit User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            BeginEditAccountRow(rowIndex);
        }

        private void saveChanges_Click(object sender, EventArgs e)
        {
            int rowIndex = GetSelectedRowIndex();
            if (rowIndex < 0)
            {
                MessageBox.Show("Select a user row first.", "Save Changes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveAccountRow(rowIndex);
        }

        private void deleteUser_Click(object sender, EventArgs e)
        {
            int rowIndex = GetSelectedRowIndex();
            if (rowIndex < 0)
            {
                MessageBox.Show("Select a user row first.", "Delete User", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DeleteAccount(rowIndex);
        }

        private int GetSelectedRowIndex()
        {
            if (manageAccountsDataGrid == null) return -1;

            if (manageAccountsDataGrid.SelectedRows.Count > 0)
                return manageAccountsDataGrid.SelectedRows[0].Index;

            if (manageAccountsDataGrid.CurrentRow != null)
                return manageAccountsDataGrid.CurrentRow.Index;

            return -1;
        }

        private void ResetPassword(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= manageAccountsDataGrid.Rows.Count) return;

            var row = manageAccountsDataGrid.Rows[rowIndex];
            var state = GetRowState(row, false);
            string username = Convert.ToString(row.Cells["username"].Value);

            if (state != null && state.IsNew)
            {
                row.Cells["password"].Value = DefaultResetPasswordValue;
                return;
            }

            if (string.IsNullOrWhiteSpace(username)) return;

            var result = MessageBox.Show(
                $"Reset password for '{username}' to '{DefaultResetPasswordValue}'?",
                "Confirm Reset Password",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string sql = "UPDATE sys_users SET password=@password WHERE username=@username";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@password", DefaultResetPasswordValue);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.ExecuteNonQuery();
                    }
                }

                row.Cells["password"].Value = DefaultResetPasswordValue;
                LogAction("PasswordReset", $"Password reset for account: {username}");
                MessageBox.Show("Password has been reset.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // If a profile image is present in an OpenFileDialog or in the row, save it to disk and update DB
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error resetting password:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BeginEditAccountRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= manageAccountsDataGrid.Rows.Count) return;

            var row = manageAccountsDataGrid.Rows[rowIndex];
            var state = GetRowState(row, true);
            state.IsEditing = true;

            if (string.IsNullOrWhiteSpace(state.OriginalUsername))
                state.OriginalUsername = Convert.ToString(row.Cells["username"].Value);

            SetRowEditable(row, true);
            manageAccountsDataGrid.CurrentCell = row.Cells["rfid_number"];
            manageAccountsDataGrid.BeginEdit(true);
        }

        private void SaveAccountRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= manageAccountsDataGrid.Rows.Count) return;

            var row = manageAccountsDataGrid.Rows[rowIndex];
            var state = GetRowState(row, false);
            if (state == null || !state.IsEditing) return;

            string rfid = Convert.ToString(row.Cells["rfid_number"].Value).Trim();
            string username = Convert.ToString(row.Cells["username"].Value).Trim();
            string password = Convert.ToString(row.Cells["password"].Value).Trim();
            string role = Convert.ToString(row.Cells["role"].Value).Trim().ToLower();

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Username, Password, and Role are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    if (state.IsNew)
                    {
                        const string insertSql = "INSERT INTO sys_users (rfid_number, username, password, role, created_at) VALUES (@rfid, @username, @password, @role, NOW())";
                        using (var cmd = new MySqlCommand(insertSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@rfid", string.IsNullOrWhiteSpace(rfid) ? (object)DBNull.Value : rfid);
                            cmd.Parameters.AddWithValue("@username", username);
                            cmd.Parameters.AddWithValue("@password", password);
                            cmd.Parameters.AddWithValue("@role", role);
                            cmd.ExecuteNonQuery();
                        }

                        LogAction("AccountAdded", $"Added account: {username}");
                    }
                    else
                    {
                        const string updateSql = "UPDATE sys_users SET rfid_number=@rfid, username=@username, password=@password, role=@role WHERE username=@currentUsername";
                        using (var cmd = new MySqlCommand(updateSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@rfid", string.IsNullOrWhiteSpace(rfid) ? (object)DBNull.Value : rfid);
                            cmd.Parameters.AddWithValue("@username", username);
                            cmd.Parameters.AddWithValue("@password", password);
                            cmd.Parameters.AddWithValue("@role", role);
                            cmd.Parameters.AddWithValue("@currentUsername", state.OriginalUsername ?? username);
                            cmd.ExecuteNonQuery();
                        }

                        LogAction("AccountEdited", $"Edited account: {state.OriginalUsername} -> {username}");
                    }
                }

                LoadStudentData(guna2TextBox1?.Text?.Trim() ?? string.Empty,
                    GetCurrentRoleFilter(),
                    GetCurrentSortFilter());
                ConfigureDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving account:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteAccount(int rowIndex)
        {
            var row = manageAccountsDataGrid.Rows[rowIndex];
            var state = GetRowState(row, false);
            string username = Convert.ToString(row.Cells["username"].Value);

            if (state != null && state.IsNew)
            {
                if (manageAccountsDataGrid.DataSource is DataTable dt)
                {
                    dt.Rows.RemoveAt(rowIndex);
                    if (attendancetotal != null)
                        attendancetotal.Text = dt.Rows.Count.ToString();
                }
                return;
            }

            if (string.IsNullOrWhiteSpace(username)) return;

            var result = MessageBox.Show($"Delete account '{username}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string sql = "DELETE FROM sys_users WHERE username=@username";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.ExecuteNonQuery();
                    }
                }

                LogAction("AccountDeleted", $"Deleted account: {username}");
                LoadStudentData(guna2TextBox1?.Text?.Trim() ?? string.Empty,
                    GetCurrentRoleFilter(),
                    GetCurrentSortFilter());
                ConfigureDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting account:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

            }
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

        private Color GetDataGridThemeColor()
        {
            var designerMainColor = Color.FromArgb(208, 228, 150);
            if (currentThemeColor.ToArgb() == designerMainColor.ToArgb())
                return Color.FromArgb(48, 79, 99);

            return currentThemeColor;
        }

        #endregion

        private void userProfile_Click(object sender, EventArgs e)
        {
            if (userOptions == null) return;

            userOptions.Visible = !userOptions.Visible;
            if (userOptions.Visible)
                userOptions.BringToFront();
        }

        private void addNewStudent_Click(object sender, EventArgs e)
        {
            AddNewEditableRow();
        }

        private void AddNewEditableRow()
        {
            if (!(manageAccountsDataGrid.DataSource is DataTable dt)) return;

            var newRow = dt.NewRow();
            newRow["rfid_number"] = string.Empty;
            newRow["username"] = string.Empty;
            newRow["password"] = string.Empty;
            newRow["role"] = string.Empty;
            newRow["created_at"] = DateTime.Now;
            dt.Rows.Add(newRow);

            int rowIndex = manageAccountsDataGrid.Rows.Count - 1;
            if (rowIndex < 0) return;

            var gridRow = manageAccountsDataGrid.Rows[rowIndex];
            gridRow.Tag = new AccountRowEditState
            {
                IsNew = true,
                IsEditing = true,
                OriginalUsername = string.Empty
            };

            SetRowEditable(gridRow, true);
            manageAccountsDataGrid.CurrentCell = gridRow.Cells["rfid_number"];
            manageAccountsDataGrid.BeginEdit(true);

            if (attendancetotal != null)
                attendancetotal.Text = dt.Rows.Count.ToString();
        }

        private Guna.UI2.WinForms.Guna2ComboBox GetRoleFilterCombo()
        {
            return this.Controls.Find("combobox1", true)
                .OfType<Guna.UI2.WinForms.Guna2ComboBox>()
                .FirstOrDefault();
        }

        private Guna.UI2.WinForms.Guna2ComboBox GetSortFilterCombo()
        {
            return this.Controls.Find("combobox2", true)
                .OfType<Guna.UI2.WinForms.Guna2ComboBox>()
                .FirstOrDefault();
        }

        private string GetCurrentRoleFilter()
        {
            var roleCombo = GetRoleFilterCombo();
            return roleCombo != null && roleCombo.SelectedItem != null
                ? roleCombo.SelectedItem.ToString()
                : "All";
        }

        private string GetCurrentSortFilter()
        {
            var sortCombo = GetSortFilterCombo();
            return sortCombo != null && sortCombo.SelectedItem != null
                ? sortCombo.SelectedItem.ToString()
                : "Newest";
        }

        private void LogAction(string actionType, string description)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string sql = @"INSERT INTO reg_logs (name, role, action, log_date)
                                         VALUES (@name, @role, @action, NOW())";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", "Registrar");
                        cmd.Parameters.AddWithValue("@role", "Registrar");
                        cmd.Parameters.AddWithValue("@action", string.Format("{0}: {1}", actionType ?? "", description ?? "").Trim(':', ' '));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
            }
        }
    }
}