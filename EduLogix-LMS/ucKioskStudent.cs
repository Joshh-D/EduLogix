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

        // mark
         string connectionString = "server=localhost;user id=root;password=;database=edulogix-lms;";
        //string connectionString = "server = 192.168.0.105; database=edulogix-lms;uid=arduino_user;pwd=secret;";

        public ucKioskStudent()
        {
            InitializeComponent();
            this.Load += ucKioskStudent_Load;

            this.Disposed += (s, e) => StopCamera();
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
            // Validate ISBN before processing
            if (!ValidateISBN(isbn))
                return;

            guna2TextBox2.Text = isbn;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
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
                        //guna2TextBox4.Text = DateTime.Now.ToString("yyyy-MM-dd");

                        AddToScannedList(title, author, isbn);
                    }
                    else
                    {
                        MessageBox.Show("Book not found in database.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing ISBN: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (ValidateBorrowDays())
            {
                int days = 0;
                int.TryParse(guna2TextBox7.Text, out days);
                days++;
                guna2TextBox7.Text = days.ToString();
            }
        }


        private void guna2GradientButton3_Click(object sender, EventArgs e)
        {
            int days = 0;
            int.TryParse(guna2TextBox7.Text, out days);

            if (days > 0) days--;

            guna2TextBox7.Text = days.ToString();
        }

        /// <summary>
        /// Validates the borrow days input
        /// </summary>
        private bool ValidateBorrowDays()
        {
            if (!int.TryParse(guna2TextBox7.Text, out int days))
            {
                MessageBox.Show("Borrow days must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (days > 30)
            {
                MessageBox.Show("Borrow days cannot exceed 30 days.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates ISBN input before processing
        /// </summary>
        private bool ValidateISBN(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                MessageBox.Show("ISBN cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // ISBN should be 10 or 13 characters
            if (isbn.Length != 10 && isbn.Length != 13)
            {
                MessageBox.Show("ISBN must be 10 or 13 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // ISBN should contain only numbers (and hyphens which we remove)
            string cleanISBN = isbn.Replace("-", "");
            if (!System.Text.RegularExpressions.Regex.IsMatch(cleanISBN, @"^\d+$"))
            {
                MessageBox.Show("ISBN must contain only numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates checkout form before confirming
        /// </summary>
        public bool ValidateCheckoutForm()
        {
            // Check if any books are scanned
            if (pnlScannedBooks.Controls.Count == 0)
            {
                MessageBox.Show("Please scan at least one book.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate borrow days
            if (!ValidateBorrowDays())
                return false;

            return true;
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
        private void StopCamera()
        {
            try
            {
                
                if (pctbxLiveFeed.InvokeRequired)
                {
                    pctbxLiveFeed.Invoke(new Action(() =>
                    {
                        pctbxLiveFeed.Image?.Dispose();
                        pctbxLiveFeed.Image = null;
                    }));
                }
                else
                {
                    pctbxLiveFeed.Image?.Dispose();
                    pctbxLiveFeed.Image = null;
                }

                
                if (videoSource != null)
                {
                    if (videoSource.IsRunning)
                    {
                        videoSource.NewFrame -= VideoSource_NewFrame;
                        videoSource.SignalToStop();
                        videoSource.WaitForStop();
                    }

                    videoSource = null;
                }
            }
            catch
            {
                
            }
        }
        private void pctbxLiveFeed_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel10_Click(object sender, EventArgs e) { }
        private void guna2CirclePictureBox1_Click(object sender, EventArgs e) { }
        private void guna2Panel1_Paint(object sender, PaintEventArgs e) { }
        private void guna2HtmlLabel7_Click(object sender, EventArgs e) { }
        private void pnlScannedBooks_Paint(object sender, EventArgs e) { }
    }
}
