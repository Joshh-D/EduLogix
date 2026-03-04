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
    public partial class ucSidebar : UserControl
    {
        public ucSidebar()
        {
            InitializeComponent();
        }

        private void OnHover(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void btnDashboard_Click_1(object sender, EventArgs e)
        {
            ChangeUserControl(1); // dahsboard
        }

        private void btnCatalog_Click(object sender, EventArgs e)
        {
            ChangeUserControl(3); // book catalog
        }



        private void ChangeUserControl(int formType)
        {
            var parentForm = this.FindForm();
            if (parentForm == null)
            {
                MessageBox.Show("No parent form found.");
                return;
            }

            var foundControls = parentForm.Controls.Find("pnlMainContent", true);
            if (foundControls.Length > 0 && foundControls[0] is Panel pnlMain)
            {
                UserControl uc;

                switch (formType)
                {
                    case 1:
                        uc = new ucDashboard();
                        break;
                    case 2:
                    case 3:
                        uc = new ucBookCatalog();
                        break;
                    case 4:
                    default:
                        MessageBox.Show("Invalid form type: " + formType);
                        return;
                }

                pnlMain.Controls.Clear();
                uc.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(uc);
                uc.BringToFront();
            }
            else MessageBox.Show("No pnlMainContent found");
        }
    }
}
