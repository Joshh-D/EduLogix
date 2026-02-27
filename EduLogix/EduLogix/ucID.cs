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
    public partial class ucID : UserControl
    {
        public ucID()
        {
            InitializeComponent();
        }

        public int ttl = 5;

        private void ucID_Load(object sender, EventArgs e)
        {

        }

        public void DeleteSelf()
        {
            if (ttl < 1)
            {
                this.Dispose();
            }
        }

        public void SetName(string name)
        {
            txtbxName.Text = name;
        }
    }
}
