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
        }

        private void KioskForm_Load(object sender, EventArgs e)
        {
            UserControl checkoutKiosk = new ucKioskStudent();
            checkoutKiosk.Dock = DockStyle.Fill;

            UserControl catalogKiosk = new ucBookCatalog();
            catalogKiosk.Dock = DockStyle.Fill;

            // hide catalog book options
            var addButton = catalogKiosk.Controls.Find("btnAddBook", true);
            var viewArchiveButton = catalogKiosk.Controls.Find("btnViewArchive", true);
            if (addButton != null) addButton[0].Visible = false;
            if (viewArchiveButton != null) viewArchiveButton[0].Visible = false;

            // catalog
            pnlMainContent.Controls.Add(checkoutKiosk);

            // checkout
            //pnlMainContent.Controls.Add(checkoutKiosk);
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            pnlMainContent.Controls.Clear();
            UserControl checkoutKiosk = new ucKioskStudent();
            checkoutKiosk.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(checkoutKiosk);
        }

        private void btnCatalog_Click(object sender, EventArgs e)
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
