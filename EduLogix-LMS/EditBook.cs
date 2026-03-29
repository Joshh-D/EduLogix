using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EduLogix_LMS
{
    public partial class EditBook : Form
    {
        dbhandler db;

        public EditBook()
        {
            InitializeComponent();
            db = new dbhandler();
        }

        private void EditBook_Load(object sender, EventArgs e)
        {
            // Register textbox event handlers for real-time validation
            txtbxISBN.TextChanged += TxtbxISBN_TextChanged;
            txtbxTitle.TextChanged += TxtbxTitle_TextChanged;
            txtbxAuthor.TextChanged += TxtbxAuthor_TextChanged;
            txtbxGenre.TextChanged += TxtbxGenre_TextChanged;
            txtbxCopies.TextChanged += TxtbxCopies_TextChanged;
        }

        /// <summary>
        /// Validates ISBN input - must be 10 or 13 digits
        /// </summary>
        private void TxtbxISBN_TextChanged(object sender, EventArgs e)
        {
            txtbxISBN.Text = txtbxISBN.Text.Trim();
        }

        /// <summary>
        /// Validates Title input - trims whitespace
        /// </summary>
        private void TxtbxTitle_TextChanged(object sender, EventArgs e)
        {
            txtbxTitle.Text = txtbxTitle.Text.Trim();
        }

        /// <summary>
        /// Validates Author input - trims whitespace
        /// </summary>
        private void TxtbxAuthor_TextChanged(object sender, EventArgs e)
        {
            txtbxAuthor.Text = txtbxAuthor.Text.Trim();
        }

        /// <summary>
        /// Validates Genre input - trims whitespace
        /// </summary>
        private void TxtbxGenre_TextChanged(object sender, EventArgs e)
        {
            txtbxGenre.Text = txtbxGenre.Text.Trim();
        }

        /// <summary>
        /// Validates Copies input - only allows numeric values
        /// </summary>
        private void TxtbxCopies_TextChanged(object sender, EventArgs e)
        {
            // Remove non-numeric characters
            string text = txtbxCopies.Text;
            txtbxCopies.Text = System.Text.RegularExpressions.Regex.Replace(text, @"[^0-9]", "");
        }

        /// <summary>
        /// Validates all book details before saving
        /// </summary>
        public bool ValidateBookDetails()
        {
            // Validate ISBN
            if (!ValidateISBN())
                return false;

            // Validate Title
            if (!ValidateTitle())
                return false;

            // Validate Author
            if (!ValidateAuthor())
                return false;

            // Validate Genre
            if (!ValidateGenre())
                return false;

            // Validate Copies
            if (!ValidateCopies())
                return false;

            return true;
        }

        /// <summary>
        /// Validates ISBN field
        /// </summary>
        private bool ValidateISBN()
        {
            if (!InputValidator.IsNotEmpty(txtbxISBN, "ISBN"))
                return false;

            string isbn = txtbxISBN.Text.Replace("-", "");

            if (!System.Text.RegularExpressions.Regex.IsMatch(isbn, @"^\d+$"))
            {
                MessageBox.Show("ISBN must contain only numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtbxISBN.Focus();
                return false;
            }

            if (isbn.Length != 10 && isbn.Length != 13)
            {
                MessageBox.Show("ISBN must be 10 or 13 digits long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtbxISBN.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates Title field
        /// </summary>
        private bool ValidateTitle()
        {
            if (!InputValidator.IsNotEmpty(txtbxTitle, "Book Title"))
                return false;

            if (!InputValidator.HasMinimumLength(txtbxTitle, 2, "Book Title"))
                return false;

            if (!InputValidator.HasMaximumLength(txtbxTitle, 200, "Book Title"))
                return false;

            return true;
        }

        /// <summary>
        /// Validates Author field
        /// </summary>
        private bool ValidateAuthor()
        {
            if (!InputValidator.IsNotEmpty(txtbxAuthor, "Author"))
                return false;

            if (!InputValidator.HasMinimumLength(txtbxAuthor, 2, "Author"))
                return false;

            if (!InputValidator.HasMaximumLength(txtbxAuthor, 100, "Author"))
                return false;

            // Allow letters, spaces, and common punctuation
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtbxAuthor.Text, @"^[a-zA-Z\s\-\.,']+$"))
            {
                MessageBox.Show("Author name contains invalid characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtbxAuthor.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates Genre field
        /// </summary>
        private bool ValidateGenre()
        {
            if (!InputValidator.IsNotEmpty(txtbxGenre, "Genre"))
                return false;

            if (!InputValidator.HasMinimumLength(txtbxGenre, 2, "Genre"))
                return false;

            if (!InputValidator.HasMaximumLength(txtbxGenre, 50, "Genre"))
                return false;

            return true;
        }

        /// <summary>
        /// Validates Copies field
        /// </summary>
        private bool ValidateCopies()
        {
            if (!InputValidator.IsNotEmpty(txtbxCopies, "Number of Copies"))
                return false;

            if (!int.TryParse(txtbxCopies.Text, out int copies))
            {
                MessageBox.Show("Number of copies must be a valid number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtbxCopies.Focus();
                return false;
            }

            if (copies <= 0)
            {
                MessageBox.Show("Number of copies must be greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtbxCopies.Focus();
                return false;
            }

            if (copies > 9999)
            {
                MessageBox.Show("Number of copies cannot exceed 9999.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtbxCopies.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clears all input fields
        /// </summary>
        public void ClearAllFields()
        {
            txtbxISBN.Text = "";
            txtbxTitle.Text = "";
            txtbxAuthor.Text = "";
            txtbxGenre.Text = "";
            txtbxCopies.Text = "1";
            pctbxBookCover.Image = null;
            txtbxISBN.Focus();
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate all fields before saving
            if (ValidateBookDetails())
            {
                MessageBox.Show("Book details are valid! Ready to save.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // TODO: Add database save logic here
            }
            // If validation fails, error message is already shown by validation methods
        }
    }
}
