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
using Org.BouncyCastle.Tls;

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
            string query = "SELECT * FROM `edulogix-lms`.lms_book_catalogue ORDER BY title";
            ucBookCatalogFilterObj = new ucBookCatalogFilter(this);
            ucBookCatalogFilterObj.Location = new System.Drawing.Point(btnFilter.Location.X + 10, tableLayoutPanel1.Location.Y + tableLayoutPanel1.Size.Height + 5);
            ucBookCatalogFilterObj.Visible = false;
            isFilterDisplayed = false;
            pnlBackground.Controls.Add(ucBookCatalogFilterObj);
            UpdateBookCatalog(query);


        }

        // Updates book catalog to reflect any search and filter queries
        private void UpdateBookCatalog(string query)
        {
            Tbl_Book_Catalog.DataSource = db.GetAllBooks(query);
        }

        // Opens book catalog filter selection
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

        // Is called whenever the Apply or Clear filter buttons are pressed
        // Takes the provided List<> parameters, and parses them as a MySQL query
        public void ApplyTableFilters(List<string> genre, List<string> status)
        {
            // MessageBox.Show("Selected Genres:\n\n" + string.Join(", ", genre) + "\n\n" + "Selected Status:\n\n" + string.Join(", ", status));
            string query = "SELECT * FROM `edulogix-lms`.lms_book_catalogue";
            if ((genre.Count > 0) || (status.Count > 0))
            {
                query += " WHERE ";
                if ((genre.Count > 0))
                {
                    query += "genre IN(" + string.Join(", ", genre) + ")";

                    if ((status.Count > 0))
                    {
                        query += " AND " + string.Join(" AND ", status);

                    }
                }
                else
                {
                    query += string.Join(" AND ", status);
                }
            }

            query += " ORDER BY title";
            // MessageBox.Show(query);
            UpdateBookCatalog(query);
        }

        private void SearchAndFilter(object sender, EventArgs e)
        {
            Tbl_Book_Catalog.DataSource = db.SearchBar("lms_book_catalogue", TxtBx_SearchBar.Text);
        }
    }
}
