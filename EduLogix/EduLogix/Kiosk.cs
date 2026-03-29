using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private readonly List<string> regIdleSlidePaths = new List<string>();
        private Timer regIdleSlideshowTimer;
        private int regIdleSlideIndex;
        private Timer kioskIdleTimer;
        private DateTime lastActivityTime;
        private int kioskIdleSeconds = 60;
        private bool isIdleMode;
        private readonly StringBuilder rfidBuffer = new StringBuilder();
        private DateTime lastRfidCharTime;

        public Kiosk()
        {
            InitializeComponent();
            if (timer1 != null)
            {
                timer1.Tick -= timer1_Tick;
                timer1.Tick += timer1_Tick;
            }

            KeyPreview = true;
            this.KeyPress += Kiosk_KeyPress;
            this.Load += Kiosk_Load;
        }
        public void Kiosk_Load(object sender, EventArgs e)
        {
            lblDateTime2.Text = DateTime.Now.ToString("MMMM dd, yyyy | hh:mm:ss:tt");
            timer1.Start();

            kioskIdleSeconds = GetKioskIdleSeconds();
            InitializeIdleDetection();

            EnsureRegIdleContainer();
            if (regIdlePanel != null)
                regIdlePanel.Visible = false;

            InitializeRegIdleSlideshow();
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

        private void InitializeRegIdleSlideshow()
        {
            try
            {
                regIdleSlidePaths.Clear();
                regIdleSlidePaths.AddRange(GetRegIdleSlideshowImages());

                if (regIdlePictureBox != null)
                {
                    regIdlePictureBox.Dock = DockStyle.Fill;
                    regIdlePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                }

                regIdleSlideIndex = 0;
                ShowRegIdleSlide(regIdleSlideIndex);

                if (regIdleSlideshowTimer != null)
                {
                    regIdleSlideshowTimer.Stop();
                    regIdleSlideshowTimer.Tick -= RegIdleSlideshowTimer_Tick;
                    regIdleSlideshowTimer.Dispose();
                }

                regIdleSlideshowTimer = new Timer();
                regIdleSlideshowTimer.Interval = 5000;
                regIdleSlideshowTimer.Tick += RegIdleSlideshowTimer_Tick;

                if (regIdleSlidePaths.Count > 1)
                    regIdleSlideshowTimer.Start();
            }
            catch
            {
                // keep kiosk stable if slideshow metadata is unavailable
            }
        }

        private void Kiosk_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                string scannedId = rfidBuffer.ToString().Trim();
                rfidBuffer.Clear();

                if (!string.IsNullOrWhiteSpace(scannedId))
                {
                    OnRfidScanned(scannedId);
                }

                e.Handled = true;
                return;
            }

            if (char.IsControl(e.KeyChar))
                return;

            if ((DateTime.Now - lastRfidCharTime).TotalMilliseconds > 700)
                rfidBuffer.Clear();

            rfidBuffer.Append(e.KeyChar);
            lastRfidCharTime = DateTime.Now;
        }

        private void OnRfidScanned(string scannedId)
        {
            RegisterActivity();
        }

        private void EnsureRegIdleContainer()
        {
            if (regIdlePanel == null) return;

            if (regIdlePanel.Parent == null && guna2GradientPanel2 != null)
            {
                guna2GradientPanel2.Controls.Add(regIdlePanel);
            }

            regIdlePanel.Dock = DockStyle.Fill;

            regIdlePanel.BringToFront();
            if (regIdlePictureBox != null)
                regIdlePictureBox.BringToFront();
        }

        private void InitializeIdleDetection()
        {
            lastActivityTime = DateTime.Now;
            isIdleMode = false;

            if (kioskIdleTimer != null)
            {
                kioskIdleTimer.Stop();
                kioskIdleTimer.Tick -= KioskIdleTimer_Tick;
                kioskIdleTimer.Dispose();
            }

            kioskIdleTimer = new Timer { Interval = 1000 };
            kioskIdleTimer.Tick += KioskIdleTimer_Tick;
            kioskIdleTimer.Start();
        }

        private void KioskIdleTimer_Tick(object sender, EventArgs e)
        {
            if (kioskIdleSeconds <= 0) return;

            var idleFor = DateTime.Now - lastActivityTime;
            if (!isIdleMode && idleFor.TotalSeconds >= kioskIdleSeconds)
            {
                isIdleMode = true;
                EnsureRegIdleContainer();
                if (regIdlePanel != null)
                    regIdlePanel.Visible = true;
            }
        }

        private int GetKioskIdleSeconds()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string query = "SELECT kiosk_idle_seconds FROM reg_settings WHERE id = 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        object value = cmd.ExecuteScalar();
                        int seconds = (value == null || value == DBNull.Value) ? 0 : Convert.ToInt32(value);
                        if (seconds > 0)
                            return seconds;

                        using (var fallbackCmd = new MySqlCommand("SELECT slideshow_duration FROM reg_settings WHERE id = 1", conn))
                        {
                            object fallback = fallbackCmd.ExecuteScalar();
                            int fallbackSeconds = (fallback == null || fallback == DBNull.Value) ? 0 : Convert.ToInt32(fallback);
                            return fallbackSeconds > 0 ? fallbackSeconds : 60;
                        }
                    }
                }
            }
            catch
            {
                return 60;
            }
        }

        private void RegisterActivity()
        {
            lastActivityTime = DateTime.Now;
            if (isIdleMode)
            {
                isIdleMode = false;
                if (regIdlePanel != null)
                    regIdlePanel.Visible = false;
            }
        }

        private IEnumerable<string> GetRegIdleSlideshowImages()
        {
            var paths = new List<string>();
            string resourcesPath = ResolveResourcesPath();
            string kioskFolder = Path.Combine(resourcesPath, "kiosk");

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string query = "SELECT kiosk_idle_slideshow FROM reg_settings WHERE id = 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        object value = cmd.ExecuteScalar();
                        if (value == null || value == DBNull.Value)
                            return paths;

                        var csv = value.ToString();
                        if (string.IsNullOrWhiteSpace(csv))
                            return paths;

                        foreach (var item in csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string trimmed = item.Trim();
                            if (string.IsNullOrWhiteSpace(trimmed))
                                continue;

                            string filename = Path.GetFileName(trimmed);
                            if (string.IsNullOrWhiteSpace(filename))
                                continue;

                            string fullPath = Path.Combine(kioskFolder, filename);
                            if (File.Exists(fullPath) && !paths.Contains(fullPath, StringComparer.OrdinalIgnoreCase))
                                paths.Add(fullPath);
                        }
                    }
                }
            }
            catch
            {
                // ignore and use fallback below
            }

            if (paths.Count == 0 && Directory.Exists(kioskFolder))
            {
                foreach (var file in Directory.GetFiles(kioskFolder))
                {
                    if (!IsSupportedImage(file)) continue;
                    if (!paths.Contains(file, StringComparer.OrdinalIgnoreCase))
                        paths.Add(file);
                }
            }

            return paths;
        }

        private bool IsSupportedImage(string path)
        {
            string ext = Path.GetExtension(path);
            if (string.IsNullOrWhiteSpace(ext)) return false;

            ext = ext.ToLowerInvariant();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif";
        }

        private string ResolveResourcesPath()
        {
            try
            {
                string projectResources = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\Resources"));
                if (Directory.Exists(projectResources))
                    return projectResources;
            }
            catch
            {
            }

            return Path.Combine(Application.StartupPath, "Resources");
        }

        private void RegIdleSlideshowTimer_Tick(object sender, EventArgs e)
        {
            if (regIdleSlidePaths.Count == 0) return;

            regIdleSlideIndex = (regIdleSlideIndex + 1) % regIdleSlidePaths.Count;
            ShowRegIdleSlide(regIdleSlideIndex);
        }

        private void ShowRegIdleSlide(int index)
        {
            if (regIdlePictureBox == null) return;

            if (regIdleSlidePaths.Count == 0)
            {
                if (regIdlePictureBox.Image != null)
                {
                    var old = regIdlePictureBox.Image;
                    regIdlePictureBox.Image = null;
                    old.Dispose();
                }
                return;
            }

            string path = regIdleSlidePaths[Math.Max(0, Math.Min(index, regIdleSlidePaths.Count - 1))];
            if (!File.Exists(path)) return;

            if (regIdlePictureBox.Image != null)
            {
                var old = regIdlePictureBox.Image;
                regIdlePictureBox.Image = null;
                old.Dispose();
            }

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var img = Image.FromStream(fs))
            {
                regIdlePictureBox.Image = new Bitmap(img);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (kioskIdleTimer != null)
            {
                kioskIdleTimer.Stop();
                kioskIdleTimer.Tick -= KioskIdleTimer_Tick;
                kioskIdleTimer.Dispose();
                kioskIdleTimer = null;
            }

            if (regIdleSlideshowTimer != null)
            {
                regIdleSlideshowTimer.Stop();
                regIdleSlideshowTimer.Tick -= RegIdleSlideshowTimer_Tick;
                regIdleSlideshowTimer.Dispose();
                regIdleSlideshowTimer = null;
            }

            if (regIdlePictureBox != null && regIdlePictureBox.Image != null)
            {
                var old = regIdlePictureBox.Image;
                regIdlePictureBox.Image = null;
                old.Dispose();
            }

            base.OnFormClosing(e);
        }
    }
}
