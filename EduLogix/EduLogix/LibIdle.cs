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
    public partial class LibIdle : Form
    {
        public LibIdle()
        {
            InitializeComponent();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            ConfirmBorrow confirmBorrow = new ConfirmBorrow();
            confirmBorrow.Show();
            this.Hide();
        }
    }
}
