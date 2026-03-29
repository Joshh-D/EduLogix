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
    public partial class KioskForm : Form
    {
        public KioskForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void KioskForm_Load(object sender, EventArgs e)
        {
            UserControl checkoutKiosk = new ucKioskStudent();
            checkoutKiosk.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(checkoutKiosk);
        }

        public void ShowIdleScreen()
        {
            pnlMainContent.Controls.Clear();
            ucKioskIdle idleForm = new ucKioskIdle();
            idleForm.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(idleForm);
            idleForm.BringToFront();
        }

        public void btnCheckout_Click(bool isLoggedIn = false)
        {
            pnlMainContent.Controls.Clear();
            UserControl checkoutKiosk = new ucKioskStudent();
            checkoutKiosk.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(checkoutKiosk);
        }

        public void btnCatalog_Click(bool isLoggedIn = false)
        {
            pnlMainContent.Controls.Clear();
            UserControl catalogKiosk = new ucBookCatalog();
            var addButton = catalogKiosk.Controls.Find("btnAddBook", true);
            var viewArchiveButton = catalogKiosk.Controls.Find("btnViewArchive", true);
            if (addButton != null) addButton[0].Visible = false;
            if (viewArchiveButton != null) viewArchiveButton[0].Visible = false;

            catalogKiosk.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(catalogKiosk);
        }
    }
}
