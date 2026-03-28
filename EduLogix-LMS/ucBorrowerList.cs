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

        private void ucBorrowerList_Load(object sender, EventArgs e)
        {
            ucBorrowerFilterListObj = new ucBorrowerListFilter(this);
            ucBorrowerFilterListObj.Location = new System.Drawing.Point(btnFilter.Location.X + 10, 5);
            ucBorrowerFilterListObj.Visible = false;
            isFilterDisplayed = false;
            pnlContainer.Controls.Add(ucBorrowerFilterListObj);

            guna2DataGridView1.DataSource = db.GetBorrowerList();
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
                guna2DataGridView1.DataSource = db.GetBorrowerList(true, gradeFilters, false);
            else guna2DataGridView1.DataSource = db.GetBorrowerList();
        }

        public void ApplyCurrentFilters(List<string> gradeFilter)
        {
            var gradeFilters = gradeFilter;
            //var sortFilters = ucBorrowerFilterListObj.sortFilters;
            if (gradeFilters != null && gradeFilters.Count > 0)
            {
                MessageBox.Show("Filtering from apply button");
                guna2DataGridView1.DataSource = db.GetBorrowerList(true, gradeFilters);

            }
            else guna2DataGridView1.DataSource = db.GetBorrowerList();
        }
    }
}
