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
        public ucBorrowerList()
        {
            InitializeComponent();
        }

        UserControl ucBorrowerFilterListObj;
        bool isFilterDisplayed = false;

        private void ucBorrowerList_Load(object sender, EventArgs e)
        {
            ucBorrowerFilterListObj = new ucBorrowerListFilter();
            ucBorrowerFilterListObj.Location = new System.Drawing.Point(btnFilter.Location.X + 10, tableLayoutPanel1.Location.Y + tableLayoutPanel1.Size.Height + 5);
            ucBorrowerFilterListObj.Visible = false;
            isFilterDisplayed = false;
            pnlBackground.Controls.Add(ucBorrowerFilterListObj);
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            bool isVisible = ucBorrowerFilterListObj.Visible;
            ucBorrowerFilterListObj.Visible = !isVisible;

            if (ucBorrowerFilterListObj.Visible)
                ucBorrowerFilterListObj.BringToFront();

            isFilterDisplayed = ucBorrowerFilterListObj.Visible;
        }
    }
}
