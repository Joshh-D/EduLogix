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

        public int ttl = 10;

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

        public void SetInformation(string numID, string name, string grade, string section, string status)
        {
            txtbxIDNum.Text = numID;
            txtbxName.Text = name;
            txtbxGrade.Text = grade;
            txtbxSection.Text = section;
            lblStatus.Text = status;
        }
    }
}
