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
    public partial class ucDashboard : UserControl
    {
        dbhandler db;
        public ucDashboard()
        {
            InitializeComponent();
            db = new dbhandler();
        }

        private void ucDashboard_Load(object sender, EventArgs e)
        {
            // ensure controls/handles exist and layout is calculated
            //this.CreateControl();                     // ensures handle created
            //pnlBackground?.PerformLayout();           // if you have a background panel
            //this.PerformLayout();
            //Application.DoEvents();                   // optional, forces pending layout/paint

            //// run animation after layout/paint completes
            //this.BeginInvoke((Action)(() =>
            //{
            //    tblWidgets.Visible = false;
            //    tblGraphs.Visible = false;
            //    guna2Transition1.ShowSync(tblWidgets);
            //    guna2Transition1.ShowSync(tblGraphs);
            //}));



            RefreshDashboardInfo();
            timer1.Interval = 1000; // 1 second
            timer1.Start();         // start the timer
        }

        private void RefreshDashboardInfo()
        {
            int[] upperDashboardInfo = db.GetUpperDashboardInfo();
            lblTotalBooks.Text = upperDashboardInfo[0].ToString();
            lblMissingBooks.Text = upperDashboardInfo[1].ToString();
            lblAvailableBooks.Text = upperDashboardInfo[2].ToString();
            lblBorrowedBooks.Text = upperDashboardInfo[3].ToString();
            lblOverdue.Text = upperDashboardInfo[4].ToString();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");
        }

        private void lblOverdue_Click(object sender, EventArgs e)
        {

        }

        private void guna2GradientPanel6_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}