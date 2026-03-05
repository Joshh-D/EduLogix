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
    public partial class ucLogsForm : UserControl
    {
        public ucLogsForm()
        {
            InitializeComponent();
        }

        UserControl ucLogsFilterObj;
        bool isFilterDisplayed = false;

        private void ucLogsForm_Load(object sender, EventArgs e)
        {
            ucLogsFilterObj = new ucLogsFilter();
            ucLogsFilterObj.Location = new System.Drawing.Point(btnFilter.Location.X + 10, tableLayoutPanel1.Location.Y + tableLayoutPanel1.Size.Height + 5);
            ucLogsFilterObj.Visible = false;
            isFilterDisplayed = false;
            pnlBackground.Controls.Add(ucLogsFilterObj);
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            bool isVisible = ucLogsFilterObj.Visible;
            ucLogsFilterObj.Visible = !isVisible;

            if (ucLogsFilterObj.Visible)
                ucLogsFilterObj.BringToFront();

            isFilterDisplayed = ucLogsFilterObj.Visible;
        }
    }
}
