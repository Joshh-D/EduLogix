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
    public partial class ucBookInfoIcon : UserControl
    {
        public ucBookInfoIcon()
        {
            InitializeComponent();
        }

        public void SetBookCoverImage(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    pctbxBookCover.Image = null;
                    return;
                }

                if (!System.IO.File.Exists(path))
                {
                    MessageBox.Show($"Image file not found: {path}", "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    pctbxBookCover.Image = null;
                    return;
                }

                pctbxBookCover.Image = Image.FromFile(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Image Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                pctbxBookCover.Image = null;
            }
        }
    }
}
