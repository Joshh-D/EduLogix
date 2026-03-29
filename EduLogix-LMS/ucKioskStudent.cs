using AForge.Video;
using AForge.Video.DirectShow;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZXing;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace EduLogix_LMS
{
    public partial class ucKioskStudent : UserControl
    {
        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoSource;

        private string lastScannedISBN = "";
        private bool isProcessing = false;

        string connectionString = "server = 192.168.1.18; database=edulogix-lms;uid=arduino_user;pwd=secret;";

        public ucKioskStudent()
        {
            InitializeComponent();
            this.Load += ucKioskStudent_Load;
        }
        private void ucKioskStudent_Load(object sender, EventArgs e)
        {
            StartCamera();
        }

        
        private void StartCamera()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            MessageBox.Show("Cameras found: " + videoDevices.Count); 

            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();
            }
            else
            {
                MessageBox.Show("No camera found.");
            }
        }

        
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap frame = (Bitmap)eventArgs.Frame.Clone();

                
                Bitmap scanBitmap = (Bitmap)frame.Clone();

               
                pctbxLiveFeed.Invoke(new Action(() =>
                {
                    pctbxLiveFeed.Image?.Dispose(); 
                    pctbxLiveFeed.Image = (Bitmap)frame.Clone();
                }));

                if (isProcessing) return;

                var reader = new ZXing.Windows.Compatibility.BarcodeReader();
                var result = reader.Decode(scanBitmap);

                if (result != null)
                {
                    string isbn = result.Text;

                    if (isbn == lastScannedISBN) return;

                    lastScannedISBN = isbn;
                    isProcessing = true;

                    this.Invoke(new Action(() =>
                    {
                        HandleScannedISBN(isbn);
                    }));

                    System.Threading.Tasks.Task.Delay(1500).ContinueWith(t =>
                    {
                        isProcessing = false;
                    });
                }

                scanBitmap.Dispose();
                frame.Dispose();
            }
            catch
            {
                
            }
        }

        
        private void HandleScannedISBN(string isbn)
        {
            guna2TextBox2.Text = isbn;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT title, author FROM lms_book_catalogue WHERE isbn = @isbn LIMIT 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@isbn", isbn);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string title = reader["title"].ToString();
                    string author = reader["author"].ToString();

                    guna2TextBox1.Text = title;
                    guna2TextBox3.Text = author;
                    guna2TextBox4.Text = DateTime.Now.ToString("yyyy-MM-dd");

                    AddToScannedList(title, author, isbn);
                }
                else
                {
                    MessageBox.Show("Book not found in database.");
                }
            }
        }

        
        private void AddToScannedList(string title, string author, string isbn)
        {
            
            foreach (Control ctrl in pnlScannedBooks.Controls)
            {
                if (ctrl is Label lbl && lbl.Tag != null && lbl.Tag.ToString() == isbn)
                {
                    return;
                }
            }

            Label bookLabel = new Label();
            bookLabel.Text = $"{title} - {author}";
            bookLabel.Tag = isbn;
            bookLabel.AutoSize = true;
            bookLabel.Padding = new Padding(5);
            bookLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            pnlScannedBooks.Controls.Add(bookLabel);
        }

       
        private void guna2GradientButton2_Click(object sender, EventArgs e)
        {
            int days = 0;
            int.TryParse(guna2TextBox7.Text, out days);
            days++;
            guna2TextBox7.Text = days.ToString();
        }

        
        private void guna2GradientButton3_Click(object sender, EventArgs e)
        {
            int days = 0;
            int.TryParse(guna2TextBox7.Text, out days);

            if (days > 0) days--;

            guna2TextBox7.Text = days.ToString();
        }

        
        private void btnRemoveBook_Click(object sender, EventArgs e)
        {
            if (pnlScannedBooks.Controls.Count > 0)
            {
                pnlScannedBooks.Controls.RemoveAt(pnlScannedBooks.Controls.Count - 1);
            }
        }

        
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Books borrowed successfully!");
        }

        
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }

            base.OnHandleDestroyed(e);
        }

        private void pctbxLiveFeed_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel10_Click(object sender, EventArgs e) { }
        private void guna2CirclePictureBox1_Click(object sender, EventArgs e) { }
        private void guna2Panel1_Paint(object sender, PaintEventArgs e) { }
        private void guna2HtmlLabel7_Click(object sender, EventArgs e) { }
        private void pnlScannedBooks_Paint(object sender, EventArgs e) { }
    }
}