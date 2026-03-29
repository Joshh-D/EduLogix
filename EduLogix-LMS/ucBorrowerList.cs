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
    public partial class ucBorrowerList : UserControl
    {
        dbhandler db = new();

        public ucBorrowerList()
        {
            InitializeComponent();
        }

        ucBorrowerListFilter ucBorrowerFilterListObj;
        bool isFilterDisplayed = false;

        public Guna2DataGridView GetBorrowerListDGV()
        {
            return dgvBorrowerList;
        }

        private void ucBorrowerList_Load(object sender, EventArgs e)
        {
            ucBorrowerFilterListObj = new ucBorrowerListFilter(this);
            ucBorrowerFilterListObj.Location = new System.Drawing.Point(btnFilter.Location.X + 10, 5);
            ucBorrowerFilterListObj.Visible = false;
            isFilterDisplayed = false;
            pnlContainer.Controls.Add(ucBorrowerFilterListObj);

            dgvBorrowerList.DataSource = db.GetBorrowerList();
        }

        public void btnFilter_Click(object sender, EventArgs e)
        {
            ucBorrowerFilterListObj.Visible = !ucBorrowerFilterListObj.Visible;

            if (ucBorrowerFilterListObj.Visible) ucBorrowerFilterListObj.BringToFront();
            else ApplyCurrentFilters();

            isFilterDisplayed = ucBorrowerFilterListObj.Visible;
        }

        public void ApplyCurrentFilters()
        {
            var gradeFilters = ucBorrowerFilterListObj.GetAppliedGradeFilter();
            var sortFilters = ucBorrowerFilterListObj.sortFilters;
            if (gradeFilters != null && gradeFilters.Count > 0 && !sortFilters.Contains("a-z"))
                dgvBorrowerList.DataSource = db.GetBorrowerList(true, gradeFilters, false);
            else dgvBorrowerList.DataSource = db.GetBorrowerList();
        }

        public void ApplyCurrentFilters(List<string> gradeFilter)
        {
            var gradeFilters = gradeFilter;
            //var sortFilters = ucBorrowerFilterListObj.sortFilters;
            if (gradeFilters != null && gradeFilters.Count > 0)
            {
                MessageBox.Show("Filtering from apply button");
                dgvBorrowerList.DataSource = db.GetBorrowerList(true, gradeFilters);

            }
            else dgvBorrowerList.DataSource = db.GetBorrowerList();
        }

        private void txtbxSearchBar_TextChanged(object sender, EventArgs e)
        {
            dgvBorrowerList.DataSource = db.SearchBar("lms_borrower_list", txtbxSearchBar.Text);
        }
    }
}
