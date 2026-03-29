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

        private List<ucBookInfoIcon> ucBookInfoIcons = new List<ucBookInfoIcon>();
        List<string> booksScanned = new List<string>();
        ucBookInfoIcon focusedBook;

        // Inactivity timeout tracking (in milliseconds)
        private int inactivityCounter = 0;
        private const int INACTIVITY_TIMEOUT = 20000; // 30 second idle mode
        private const int WARNING_THRESHOLD = 15000; // 20 second show warning
        private bool warningShown = false;
        int timerValue = 0;

        // mark
        string connectionString = "server=localhost;user id=root;password=root;database=edulogix-lms;";

        // todo: need to pass the session handler info here to get the student info (name, rfid, grade, section) for the borrowing transaction
        // also need to implement the catalog button in the main form to show the book catalog in a new form (so that user can switch to catalog without logging out)
        public ucKioskStudent()
        {
            InitializeComponent();
            this.Load += ucKioskStudent_Load;
        }

        private void ucKioskStudent_Load(object sender, EventArgs e)
        {
            StartCamera();
            // Initialize timer
            timer1.Interval = 1000; // Check every 1 second
            timer1.Start();
            inactivityCounter = 0;
            warningShown = false;
            RegisterActivityEvents(this);
        }

        private void RegisterActivityEvents(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                ctrl.MouseMove += ActivityDetected;
                ctrl.Click += ActivityDetected;
                ctrl.KeyPress += ActivityDetected;

                // Recursive (IMPORTANT)
                if (ctrl.HasChildren)
                {
                    RegisterActivityEvents(ctrl);
                }
            }

            // Also include main control
            parent.MouseMove += ActivityDetected;
            parent.Click += ActivityDetected;
            parent.KeyPress += ActivityDetected;
        }

        private void ActivityDetected(object sender, EventArgs e)
        {
            ResetInactivityTimer();
        }
        private void StartCamera()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            //MessageBox.Show("Cameras found: " + videoDevices.Count);

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
                    ResetInactivityTimer();
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
            // Reset inactivity timer on user activity
            ResetInactivityTimer();

            // Validate ISBN before processing
            if (!ValidateISBN(isbn))
                return;

            txtbxISBN.Text = isbn;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT title, author, image_path FROM lms_book_catalogue WHERE isbn = @isbn LIMIT 1";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@isbn", isbn);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string title = reader["title"].ToString();
                        string author = reader["author"].ToString();
                        //MessageBox.Show(reader["image_path"].ToString());

                        txtbxBookTitle.Text = title;
                        txtbxAuthor.Text = author;
                        //guna2TextBox4.Text = DateTime.Now.ToString("yyyy-MM-dd");

                        AddToScannedList(title, author, isbn, reader["image_path"].ToString());
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


        private void AddToScannedList(string title, string author, string isbn, string image_path)
        {
            if (booksScanned.Contains(isbn)) return;

            ucBookInfoIcon templ = new ucBookInfoIcon();
            templ.lblBookTitle.Text = title;

            //templ.SetBookCoverImage(image_path);
            templ.pctbxBookCover.Image = Image.FromFile(image_path);
            templ.pctbxBookCover.SizeMode = PictureBoxSizeMode.StretchImage;

            templ.pctbxBookCover.Click += (object sender, EventArgs e) =>
            {
                MessageBox.Show($"Title: {title}\nISBN: {isbn}", "Book Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                focusedBook.pnlContainer.FillColor = Color.White;
                focusedBook = templ;
                focusedBook.pnlContainer.FillColor = Color.LightBlue;

                txtbxBookTitle.Text = title;
                txtbxAuthor.Text = author;
                txtbxISBN.Text = isbn;
            };


            if (focusedBook != null)
                focusedBook.pnlContainer.FillColor = Color.White;

            templ.pnlContainer.FillColor = Color.LightBlue;
            focusedBook = templ;

            booksScanned.Add(isbn);
            ucBookInfoIcons.Add(templ);
            flpnlBooksContainer.Controls.Add(templ);

            //flpnlBooksContainer.ScrollControlIntoView(templ);
        }


        private void guna2GradientButton2_Click(object sender, EventArgs e)
        {
            ResetInactivityTimer();
            if (ValidateBorrowDays())
            {
                int days = 0;
                int.TryParse(txtbxDays.Text, out days);
                days++;
                txtbxDays.Text = days.ToString();
            }
        }


        private void guna2GradientButton3_Click(object sender, EventArgs e)
        {
            ResetInactivityTimer();
            int days = 0;
            int.TryParse(txtbxDays.Text, out days);

            if (days > 0) days--;

            txtbxDays.Text = days.ToString();
        }

        private bool ValidateBorrowDays()
        {
            if (!int.TryParse(txtbxDays.Text, out int days))
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
            ResetInactivityTimer();
            if (focusedBook != null)
            {
                DialogResult res = MessageBox.Show($"Are you sure you to remove '{focusedBook.lblBookTitle.Text}' from the list?", "Remove Book", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (res != DialogResult.Yes) return;

                booksScanned.Remove(focusedBook.lblBookTitle.Text);
                flpnlBooksContainer.Controls.Remove(focusedBook);
                ucBookInfoIcons.Remove(focusedBook);
                focusedBook = null;
                txtbxBookTitle.Text = "";
                txtbxAuthor.Text = "";
                txtbxISBN.Text = "";
            }

            else MessageBox.Show("Please select a book to remove.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void btnConfirm_Click(object sender, EventArgs e)
        {
            ResetInactivityTimer();
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

        private void btnIncreaseDays_Click(object sender, EventArgs e)
        {
            txtbxDays.Text = (int.Parse(txtbxDays.Text) + 1).ToString();

        }

        private void btnDecreaseDays_Click(object sender, EventArgs e)
        {
            int days = txtbxDays.Text != "" ? int.Parse(txtbxDays.Text) : 1;

            if (days > 0 && (days - 1) > 0)
                txtbxDays.Text = (int.Parse(txtbxDays.Text) - 1).ToString();
            else if ((days - 1) <= 0)
            {
                btnRemoveBook_Click(sender, e);
                txtbxDays.Text = "0";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            inactivityCounter += timer1.Interval;

            // Show warning at 80% of timeout
            if (inactivityCounter >= WARNING_THRESHOLD && !warningShown)
            {
                warningShown = true;
                int secondsRemaining = (INACTIVITY_TIMEOUT - inactivityCounter) / 1000;
                // Optional: Could display a label or notification instead of messagebox
                System.Diagnostics.Debug.WriteLine($"⏰ [INACTIVITY WARNING] Session will expire in {secondsRemaining} seconds due to inactivity.");
            }

            // Session timeout - expire and show idle screen
            if (inactivityCounter >= INACTIVITY_TIMEOUT)
            {
                ExpireSession();
            }
        }

        /// <summary>
        /// Expires the current session and transitions to idle screen
        /// </summary>
        private void ExpireSession()
        {
            try
            {
                timer1.Stop();
                System.Diagnostics.Debug.WriteLine("⏰ [SESSION EXPIRED] Transitioning to idle screen...");

                // Show notification
                MessageBox.Show("Session expired due to inactivity.\n\nPlease scan your ID to start a new session.", 
                              "Session Expired", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Information);

                // Clear current session state
                ClearSessionState();

                // Stop camera
                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource.WaitForStop();
                }

                // Load idle screen
                LoadIdleScreen();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ [SESSION EXPIRY ERROR] {ex.Message}");
                MessageBox.Show($"Error during session expiry: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearSessionState()
        {
            booksScanned.Clear();
            ucBookInfoIcons.Clear();
            focusedBook = null;

            // Clear UI elements
            flpnlBooksContainer.Controls.Clear();
            txtbxISBN.Text = "";
            txtbxBookTitle.Text = "";
            txtbxAuthor.Text = "";
            txtbxDays.Text = "0";

            // Reset video display
            pctbxLiveFeed.Image?.Dispose();
            pctbxLiveFeed.Image = null;
        }

        private void LoadIdleScreen()
        {
            KioskForm form = this.FindForm() as KioskForm;
            if (form != null)
            {
                form.ShowIdleScreen(); // ✅ CLEAN SWITCH
            }
        }

        private void ResetInactivityTimer()
        {
            inactivityCounter = 0;
            warningShown = false;
            System.Diagnostics.Debug.WriteLine("🔄 [ACTIVITY DETECTED] Inactivity timer reset");
        }
    }
}
