using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EduLogix
{
    public partial class RegStudArchive : Form
    {
        private readonly string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";
        private Color currentThemeColor = Color.FromArgb(33, 150, 243);

        public RegStudArchive()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            this.Load += RegStudArchive_Load;

            if (guna2GradientButton2 != null)
            {
                guna2GradientButton2.Click -= guna2GradientButton2_Click;
                guna2GradientButton2.Click += guna2GradientButton2_Click;
            }

            if (deleteUser != null)
            {
                deleteUser.Click -= deleteUser_Click;
                deleteUser.Click += deleteUser_Click;
            }

            if (guna2TextBox1 != null)
            {
                guna2TextBox1.TextChanged -= guna2TextBox1_TextChanged;
                guna2TextBox1.TextChanged += guna2TextBox1_TextChanged;
            }
        }

        private void RegStudArchive_Load(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            LoadThemeFromDatabase();
            LoadArchivedStudents();
            ConfigureDataGridView();

            if (guna2DataGridView1 != null)
                guna2DataGridView1.ClearSelection();
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            LoadArchivedStudents(guna2TextBox1 != null ? guna2TextBox1.Text.Trim() : string.Empty);
            ConfigureDataGridView();
        }

        private void LoadArchivedStudents(string searchText = "")
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT
                                        student_id,
                                        name,
                                        phone_number,
                                        date_of_birth,
                                        grade,
                                        section,
                                        level
                                     FROM reg_studentarchive
                                     WHERE 1=1";

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        query += " AND (LOWER(name) LIKE @search OR LOWER(student_id) LIKE @search)";
                    }

                    query += " ORDER BY name ASC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        if (!string.IsNullOrWhiteSpace(searchText))
                            cmd.Parameters.AddWithValue("@search", "%" + searchText.ToLower() + "%");

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
                MessageBox.Show("Error loading archived students:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureDataGridView()
        {
            if (guna2DataGridView1 == null) return;

            Color gridThemeColor = GetDataGridThemeColor();
            Color headerTextColor = GetContrastColor(gridThemeColor);
            Color selectionBackColor = LightenColor(gridThemeColor, 0.3f);
            Color selectionTextColor = GetContrastColor(selectionBackColor);

            guna2DataGridView1.EnableHeadersVisualStyles = false;
            guna2DataGridView1.ColumnHeadersHeight = 40;
            guna2DataGridView1.RowTemplate.Height = 35;
            guna2DataGridView1.AllowUserToResizeRows = false;
            guna2DataGridView1.AllowUserToResizeColumns = false;
            guna2DataGridView1.AllowUserToDeleteRows = false;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.RowHeadersVisible = false;

            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = gridThemeColor;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = headerTextColor;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Inter", 10, FontStyle.Bold);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = DarkenColor(gridThemeColor, 0.15f);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = headerTextColor;

            guna2DataGridView1.DefaultCellStyle.Font = new Font("Inter", 9, FontStyle.Regular);
            guna2DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            guna2DataGridView1.DefaultCellStyle.BackColor = Color.White;
            guna2DataGridView1.DefaultCellStyle.SelectionBackColor = selectionBackColor;
            guna2DataGridView1.DefaultCellStyle.SelectionForeColor = selectionTextColor;

            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            Color lightPatternColor = LightenColor(gridThemeColor, 0.7f);
            guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = lightPatternColor;
            guna2DataGridView1.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

            RenameColumns();
            guna2DataGridView1.ClearSelection();
        }

        private void RenameColumns()
        {
            if (guna2DataGridView1.Columns.Contains("student_id"))
                guna2DataGridView1.Columns["student_id"].HeaderText = "Student ID";

            if (guna2DataGridView1.Columns.Contains("name"))
                guna2DataGridView1.Columns["name"].HeaderText = "Name";

            if (guna2DataGridView1.Columns.Contains("phone_number"))
                guna2DataGridView1.Columns["phone_number"].HeaderText = "Contact";

            if (guna2DataGridView1.Columns.Contains("date_of_birth"))
                guna2DataGridView1.Columns["date_of_birth"].HeaderText = "Date of Birth";

            if (guna2DataGridView1.Columns.Contains("grade"))
                guna2DataGridView1.Columns["grade"].HeaderText = "Grade";

            if (guna2DataGridView1.Columns.Contains("section"))
                guna2DataGridView1.Columns["section"].HeaderText = "Section";

            if (guna2DataGridView1.Columns.Contains("level"))
                guna2DataGridView1.Columns["level"].HeaderText = "Level";
        }

        private void guna2GradientButton2_Click(object sender, EventArgs e)
        {
            string studentId = GetSelectedStudentId();
            if (string.IsNullOrWhiteSpace(studentId))
            {
                MessageBox.Show("Select a row first.", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        const string insertBack = @"INSERT INTO reg_studentinfo
                            (image_path, rfid_number, student_id, name, phone_number, address, date_of_birth, grade, section, level, guardian_name, guardian_phone_number)
                            SELECT image_path, rfid_number, student_id, name, phone_number, address, date_of_birth, grade, section, level, guardian_name, guardian_phone_number
                            FROM reg_studentarchive
                            WHERE student_id = @studentId";

                        using (var cmdInsert = new MySqlCommand(insertBack, conn, tx))
                        {
                            cmdInsert.Parameters.AddWithValue("@studentId", studentId);
                            cmdInsert.ExecuteNonQuery();
                        }

                        using (var cmdDelete = new MySqlCommand("DELETE FROM reg_studentarchive WHERE student_id=@studentId", conn, tx))
                        {
                            cmdDelete.Parameters.AddWithValue("@studentId", studentId);
                            cmdDelete.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                }

                LoadArchivedStudents(guna2TextBox1 != null ? guna2TextBox1.Text.Trim() : string.Empty);
                ConfigureDataGridView();
                MessageBox.Show("Student restored.", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error restoring student:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void deleteUser_Click(object sender, EventArgs e)
        {
            string studentId = GetSelectedStudentId();
            if (string.IsNullOrWhiteSpace(studentId))
            {
                MessageBox.Show("Select a row first.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show("Delete selected archived student?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("DELETE FROM reg_studentarchive WHERE student_id=@studentId", conn))
                    {
                        cmd.Parameters.AddWithValue("@studentId", studentId);
                        cmd.ExecuteNonQuery();
                    }
                }

                LoadArchivedStudents(guna2TextBox1 != null ? guna2TextBox1.Text.Trim() : string.Empty);
                ConfigureDataGridView();
                MessageBox.Show("Archived student deleted.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting archived student:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetSelectedStudentId()
        {
            if (guna2DataGridView1 == null || !guna2DataGridView1.Columns.Contains("student_id"))
                return string.Empty;

            DataGridViewRow row = null;
            if (guna2DataGridView1.SelectedRows.Count > 0)
                row = guna2DataGridView1.SelectedRows[0];
            else if (guna2DataGridView1.CurrentRow != null)
                row = guna2DataGridView1.CurrentRow;

            return row == null ? string.Empty : Convert.ToString(row.Cells["student_id"].Value).Trim();
        }

        private void LoadThemeFromDatabase()
        {
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
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private Color GetDataGridThemeColor()
        {
            var designerMainColor = Color.FromArgb(208, 228, 150);
            if (currentThemeColor.ToArgb() == designerMainColor.ToArgb())
                return Color.FromArgb(48, 79, 99);

            return currentThemeColor;
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

        private void StudentsID_Click(object sender, EventArgs e)
        {
            StudentIDForm studentIDForm = new StudentIDForm();
            studentIDForm.Show();
            this.Hide();
        }

        private void Attendance_Click(object sender, EventArgs e)
        {
            AttendanceForm attendanceForm = new AttendanceForm();   
            attendanceForm.Show();
            this.Hide();
        }

        private void Dashboard_Click(object sender, EventArgs e)
        {
            DashboardForm dashboardForm = new DashboardForm();
            dashboardForm.Show();
            this.Hide();
        }
    }
}
