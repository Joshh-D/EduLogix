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
    public partial class ConfirmBorrow : Form
    {
        public ConfirmBorrow()
        {
            InitializeComponent();
        }

        private void ConfirmBorrow_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            LibIdle libIdle = new LibIdle();
            libIdle.Show();
            this.Hide();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
            "Do you want to borrow this book?",
            "Confirm Borrow",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

            if (confirm == DialogResult.Yes)
            {
                MessageBox.Show(
                    "Book borrowed successfully!\n\n" +
                    "Please remember to return the book on or before the due date.\n" +
                    "Late returns may result in penalties.",
                    "Borrowing Reminder",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );            
                LibIdle libIdle = new LibIdle();
                libIdle.Show();
                this.Close(); 
            }
        }
    }

}
