using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Mozilla;

namespace EduLogix
{
    public partial class Kiosk : Form
    {
        private string connectionString = "server=192.168.236.30;database=edulogix;uid=arduino_user;pwd=secret;";
        int currentCount = 0;
        DataTable attendanceTable = null;
        ucID recentID = null;
        List<ucID> ids = new List<ucID>();

        public Kiosk()
        {
            InitializeComponent();
            this.Load += Kiosk_Load;

        }
        public void Kiosk_Load(object sender, EventArgs e)
        {
            lblDateTime2.Text = DateTime.Now.ToString("MMMM dd, yyyy | hh:mm:ss:tt");
            System.Windows.Forms.Timer kioskTimer = new System.Windows.Forms.Timer();
            kioskTimer.Interval = 1000;
            kioskTimer.Tick += new EventHandler(timerfunc);
            kioskTimer.Tick += new EventHandler(kioskCheck);
            kioskTimer.Start();
            timer1.Start();


            getAttendanceList();
        }

        public void getAttendanceList()
        {
            try
            {
                using (MySqlConnection conn1 = new MySqlConnection(connectionString))
                {
                    string getStudentAttendanceListQuery = "SELECT * FROM reg_attendance";
                    conn1.Open();
                    
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(getStudentAttendanceListQuery, conn1))
                    {
                        guna2DataGridView1.DataSource = null;
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        guna2DataGridView1.DataSource = table;
                    }

                    conn1.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void kioskCheck(object sender, EventArgs e)
        {
            if (ids.Count > 0)
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    ucID id = ids.ElementAt(i);
                    if (id.ttl > 0) id.ttl -= 1;
                    if (id.ttl < 1)
                    {
                        ids.Remove(id);
                        pnlLatest.Refresh();
                        flowLayoutPanel1.Refresh();
                    }
                }
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(student_name) FROM reg_attendance_live";
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        int newCount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (newCount > currentCount)
                        {
                            currentCount = newCount;
                            
                            getAttendanceList();
                            int rowCount = guna2DataGridView1.RowCount;
                            string value = guna2DataGridView1.Rows[rowCount - 2].Cells["student_name"].Value?.ToString();

                            pnlLatest.Controls.Clear();

                            if (recentID != null)
                            {
                                ucID recentIDTemp = new ucID();
                                recentIDTemp = recentID;
                                recentIDTemp.Dock = DockStyle.Top;

                                flowLayoutPanel1.SuspendLayout();
                                flowLayoutPanel1.Controls.Add(recentIDTemp);
                                flowLayoutPanel1.Controls.SetChildIndex(recentIDTemp, 0);
                                flowLayoutPanel1.ResumeLayout();
                                //ids.Add(recentIDTemp);
                            }

                            ucID latestID = new ucID();
                            latestID.SetName(value);
                            latestID.Dock = DockStyle.Fill;

                            ids.Add(latestID);
                            pnlLatest.Controls.Add(latestID);

                            //int index = ids.Count;
                            //pnlLatest.Controls.Add(ids.ElementAt(index));

                            // set thes latestID to recen
                            recentID = latestID;
                        }

                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        public void timerfunc(object sender, EventArgs e)
        {
            lblDateTime2.Text = DateTime.Now.ToString("MMMM dd, yyyy | hh:mm:ss:tt");
        }

        private void guna2HtmlLabel6_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel10_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel6_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2Panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDateTime2.Text = DateTime.Now.ToString("MMMM dd, yyyy | hh:mm:tt:ss");
        }
    }
}
