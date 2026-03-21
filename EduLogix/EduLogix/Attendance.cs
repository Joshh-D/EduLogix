using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace EduLogix
{
    public partial class AttendanceForm : Form
    {
        // Assuming your DataGridView is bound to a DataTable
        private DataTable attendanceData;

        public AttendanceForm()
        {
            InitializeComponent();
            
            // Wire up event handlers
            this.Load += AttendanceForm_Load;
            attendanceStudentSearch.TextChanged += FilterAttendance;
            attendanceDateTimePicker.ValueChanged += AttendanceDateTimePicker_ValueChanged;
            attendanceCombobox1.SelectedIndexChanged += AttendanceCombobox1_SelectedIndexChanged;
            attendanceCombobox2.SelectedIndexChanged += FilterAttendance;
        }

        private void AttendanceForm_Load(object sender, EventArgs e)
        {
            // Set initial state for cascading comboboxes
            attendanceCombobox2.Enabled = false;
            
            // TODO: Load your data into attendanceData here
            
            // Clear DGV default selection
            guna2DataGridView1.ClearSelection();
            guna2DataGridView1.CurrentCell = null;
            
            UpdateTotalLabel();
        }

        private void AttendanceDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            EvaluateCombobox2State();
            FilterAttendance(sender, e);
        }

        private void AttendanceCombobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            EvaluateCombobox2State();
            FilterAttendance(sender, e);
        }

        private void EvaluateCombobox2State()
        {
            bool hasLevelSelection = attendanceCombobox1.SelectedIndex > 0; // Assuming 0 is "All" or unselected
            bool isPastDate = attendanceDateTimePicker.Value.Date < DateTime.Now.Date;

            // Combobox 2 rules: disabled if past date, enabled only if ComboBox 1 has selection
            if (isPastDate)
            {
                attendanceCombobox2.Enabled = false;
                attendanceCombobox2.SelectedIndex = -1; // Reset selection
            }
            else
            {
                attendanceCombobox2.Enabled = hasLevelSelection;
            }
        }

        private void FilterAttendance(object sender, EventArgs e)
        {
            if (attendanceData == null) return;

            string searchText = attendanceStudentSearch.Text.Trim().Replace("'", "''");
            DateTime selectedDate = attendanceDateTimePicker.Value.Date;
            
            string levelFilter = attendanceCombobox1.SelectedItem?.ToString();
            string statusFilter = attendanceCombobox2.SelectedItem?.ToString();

            // Build RowFilter string
            string filter = $"AttendanceDate = '{selectedDate:yyyy-MM-dd}'";

            if (!string.IsNullOrEmpty(searchText))
            {
                filter += $" AND (StudentName LIKE '%{searchText}%' OR StudentID LIKE '%{searchText}%')";
            }

            if (!string.IsNullOrEmpty(levelFilter) && levelFilter != "All")
            {
                filter += $" AND Level = '{levelFilter}'";
            }

            if (attendanceCombobox2.Enabled && !string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                filter += $" AND Status = '{statusFilter}'";
            }

            // Apply filter if you are using a DataView
            DataView dv = attendanceData.DefaultView;
            dv.RowFilter = filter;
            guna2DataGridView1.DataSource = dv;

            UpdateTotalLabel();
        }

        private void UpdateTotalLabel()
        {
            int totalVisible = guna2DataGridView1.Rows.Count;
            // Subtract new row placeholder if AllowUserToAddRows is true
            if (guna2DataGridView1.AllowUserToAddRows) totalVisible--;
            
            attendancetotal.Text = totalVisible.ToString();
        }

        private void Dashboard_Click(object sender, EventArgs e)
        {
            DashboardForm dashboard = new DashboardForm();
            dashboard.Show();
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

        private void Settings_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
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
    }
}
