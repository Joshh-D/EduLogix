using Org.BouncyCastle.Asn1.Mozilla;
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

namespace EduLogix
{
    public partial class Kiosk : Form
    {
        private readonly string connectionString = "server=localhost;database=edulogix;uid=root;pwd=;";
        private Timer idleTimer;
        private int idleTimeoutSeconds = 30;
        private bool isOpeningIdleForm;

        public Kiosk()
        {
            InitializeComponent();
            this.Load += Kiosk_Load;
        }
        public void Kiosk_Load(object sender, EventArgs e)
        {
            lblDateTime2.Text = DateTime.Now.ToString("MMMM dd, yyyy | hh:mm:ss:tt");
            timer1.Start();

            LoadIdleTimeoutFromSettings();
            InitializeIdleTimer();
            WireActivityHandlers(this);
            ResetIdleTimer();
        }     

        private void LoadIdleTimeoutFromSettings()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string query = "SELECT auto_logout_seconds FROM reg_settings WHERE id = 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            int seconds;
                            if (int.TryParse(result.ToString(), out seconds) && seconds > 0)
                            {
                                idleTimeoutSeconds = seconds;
                            }
                        }
                    }
                }
            }
            catch
            {
                idleTimeoutSeconds = 30;
            }
        }

        private void InitializeIdleTimer()
        {
            if (idleTimer != null)
            {
                idleTimer.Stop();
                idleTimer.Dispose();
            }

            idleTimer = new Timer();
            idleTimer.Interval = Math.Max(1, idleTimeoutSeconds) * 1000;
            idleTimer.Tick += IdleTimer_Tick;
        }

        private void WireActivityHandlers(Control parent)
        {
            if (parent == null) return;

            parent.MouseMove -= ActivityDetected;
            parent.MouseMove += ActivityDetected;
            parent.MouseDown -= ActivityDetected;
            parent.MouseDown += ActivityDetected;
            parent.KeyDown -= ActivityDetected;
            parent.KeyDown += ActivityDetected;
            parent.KeyPress -= ActivityDetected;
            parent.KeyPress += ActivityDetected;

            foreach (Control child in parent.Controls)
            {
                WireActivityHandlers(child);
            }
        }

        private void ActivityDetected(object sender, EventArgs e)
        {
            ResetIdleTimer();
        }

        private void ResetIdleTimer()
        {
            if (idleTimer == null || isOpeningIdleForm) return;

            idleTimer.Stop();
            idleTimer.Start();
        }

        private void IdleTimer_Tick(object sender, EventArgs e)
        {
            if (isOpeningIdleForm) return;

            isOpeningIdleForm = true;
            idleTimer.Stop();

            var idleForm = new RegIdle();
            idleForm.StartPosition = FormStartPosition.Manual;
            idleForm.Location = this.Location;
            idleForm.Show();
            this.Hide();
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
            lblDateTime2.Text = DateTime.Now.ToString("MMMM dd, yyyy | hh:mm:ss tt");
        }

        private void guna2HtmlLabel16_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel12_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel8_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel11_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel9_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel10_Click_1(object sender, EventArgs e)
        {

        }
    }
}
