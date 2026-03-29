using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace EduLogix_LMS
{
    public partial class ucBookCatalog : UserControl
    {
        dbhandler db;

        public ucBookCatalog()
        {
            InitializeComponent();
            db = new dbhandler();
        }

        UserControl ucBookCatalogFilterObj;
        bool isFilterDisplayed = false;

        private void ucBookCatalog_Load(object sender, EventArgs e)
        {
            ucBookCatalogFilterObj = new ucBookCatalogFilter();
            ucBookCatalogFilterObj.Location = new System.Drawing.Point(btnFilter.Location.X + 10, tableLayoutPanel1.Location.Y + tableLayoutPanel1.Size.Height + 5);
            ucBookCatalogFilterObj.Visible = false;
            isFilterDisplayed = false;
            pnlBackground.Controls.Add(ucBookCatalogFilterObj);
            Tbl_Book_Catalog.DataSource = db.GetAllBooks();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            bool isVisible = ucBookCatalogFilterObj.Visible;
            ucBookCatalogFilterObj.Visible = !isVisible;

            if (ucBookCatalogFilterObj.Visible)
            {
                ucBookCatalogFilterObj.BringToFront();
            }

            isFilterDisplayed = ucBookCatalogFilterObj.Visible;
        }

        
    }
}
