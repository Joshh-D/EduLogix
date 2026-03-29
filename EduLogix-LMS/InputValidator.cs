using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EduLogix_LMS
{
    /// <summary>
    /// Utility class for input validation in textboxes
    /// </summary>
    public static class InputValidator
    {
        /// <summary>
        /// Validates if a textbox is not empty
        /// </summary>
        public static bool IsNotEmpty(Guna.UI2.WinForms.Guna2TextBox textbox, string fieldName = "This field")
        {
            if (string.IsNullOrWhiteSpace(textbox.Text))
            {
                MessageBox.Show($"{fieldName} cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textbox.Focus();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates if a textbox meets minimum length requirement
        /// </summary>
        public static bool HasMinimumLength(Guna.UI2.WinForms.Guna2TextBox textbox, int minLength, string fieldName = "This field")
        {
            if (textbox.Text.Length < minLength)
            {
                MessageBox.Show($"{fieldName} must be at least {minLength} characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textbox.Focus();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates if a textbox meets maximum length requirement
        /// </summary>
        public static bool HasMaximumLength(Guna.UI2.WinForms.Guna2TextBox textbox, int maxLength, string fieldName = "This field")
        {
            if (textbox.Text.Length > maxLength)
            {
                MessageBox.Show($"{fieldName} cannot exceed {maxLength} characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textbox.Focus();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates if a textbox contains only numeric characters
        /// </summary>
        public static bool IsNumericOnly(Guna.UI2.WinForms.Guna2TextBox textbox, string fieldName = "This field")
        {
            if (!Regex.IsMatch(textbox.Text, @"^\d+$"))
            {
                MessageBox.Show($"{fieldName} must contain only numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textbox.Focus();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates if a textbox contains only alphabetic characters
        /// </summary>
        public static bool IsAlphabeticOnly(Guna.UI2.WinForms.Guna2TextBox textbox, string fieldName = "This field")
        {
            if (!Regex.IsMatch(textbox.Text, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show($"{fieldName} must contain only letters and spaces.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textbox.Focus();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates if a textbox contains a valid email format
        /// </summary>
        public static bool IsValidEmail(Guna.UI2.WinForms.Guna2TextBox textbox, string fieldName = "Email")
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(textbox.Text, emailPattern))
            {
                MessageBox.Show($"{fieldName} is not in a valid format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textbox.Focus();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates if textbox contains alphanumeric characters only
        /// </summary>
        public static bool IsAlphanumericOnly(Guna.UI2.WinForms.Guna2TextBox textbox, string fieldName = "This field")
        {
            if (!Regex.IsMatch(textbox.Text, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show($"{fieldName} must contain only letters and numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textbox.Focus();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates password strength (min 8 chars, at least one uppercase, one lowercase, one number)
        /// </summary>
        public static bool IsStrongPassword(Guna.UI2.WinForms.Guna2TextBox textbox, string fieldName = "Password")
        {
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d@$!%*?&]{8,}$";
            if (!Regex.IsMatch(textbox.Text, passwordPattern))
            {
                MessageBox.Show($"{fieldName} must be at least 8 characters long and contain uppercase, lowercase, and numbers.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textbox.Focus();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Clears all textboxes in a container control
        /// </summary>
        public static void ClearTextboxes(Control container)
        {
            foreach (Control control in container.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2TextBox textbox)
                {
                    textbox.Text = string.Empty;
                }
                else if (control.HasChildren)
                {
                    ClearTextboxes(control);
                }
            }
        }
    }
}
