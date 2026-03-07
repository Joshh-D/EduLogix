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
            UserControl defForm = new ucKioskStudent();
            //var addButton = defForm.Controls.Find("btnAddBook", true);
            //var viewArchiveButton = defForm.Controls.Find("btnViewArchive", true);

            //if (addButton != null) addButton[0].Visible = false;
            //if (viewArchiveButton != null) viewArchiveButton[0].Visible = false;

            defForm.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(defForm);
        }
    }
}
