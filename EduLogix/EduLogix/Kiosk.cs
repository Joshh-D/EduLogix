using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EduLogix
{
    public partial class Kiosk : Form
    {
        public Kiosk()
        {
            InitializeComponent();
            this.Load += Kiosk_Load;
        }
        public void Kiosk_Load(object sender, EventArgs e)
        {
            lblDateTime2.Text = DateTime.Now.ToString("MMMM dd, yyyy | hh:mm:ss:tt");
            System.Windows.Forms.Timer kioskTimer = new System.Windows.Forms.Timer();
            kioskTimer.Interval = 1000;
            kioskTimer.Tick += new EventHandler(timerfunc);
            kioskTimer.Start();
            timer1.Start();
        }     

        public void timerfunc(object sender, EventArgs e)
        {
            lblDateTime2.Text = DateTime.Now.ToString("MMMM dd, yyyy | hh:mm:ss:tt");
        }

        private void guna2HtmlLabel6_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel10_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel6_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2Panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDateTime2.Text = DateTime.Now.ToString("MMMM dd, yyyy | hh:mm:tt:ss");
        }
    }
}
