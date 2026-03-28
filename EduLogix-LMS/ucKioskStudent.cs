using Guna.UI2.WinForms;
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
    public partial class ucKioskStudent : UserControl
    {
        public ucKioskStudent()
        {
            InitializeComponent();
        }

        private void btnCatalog_Click(object sender, EventArgs e)
        {
            KioskForm parentForm = this.ParentForm as KioskForm;
            parentForm.btnCatalog_Click(true);
        }

        private void guna2HtmlLabel10_Click(object sender, EventArgs e)
        {

        }

        private void guna2GradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2GradientPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void btnRemoveBook_Click(object sender, EventArgs e)
        {

        }
    }
}
