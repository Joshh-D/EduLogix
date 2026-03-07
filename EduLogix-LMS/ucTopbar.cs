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
    public partial class ucTopbar : UserControl
    {
        public ucTopbar()
        {
            InitializeComponent();
        }

        UserControl ucAccountMenuObj;
        bool isFilterDisplayed = false;

        private void ucTopbar_Load(object sender, EventArgs e)
        {
        }
    }
}
