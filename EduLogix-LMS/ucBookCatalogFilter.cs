using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace EduLogix_LMS
{
    public partial class ucBookCatalogFilter : UserControl
    {
        dbhandler db;
        ucBookCatalog ucBC;
        Guna2CheckBox ChkBx_Borrowed;
        List<string> genres = new List<string>();
        List<string> genreFilter = new List<string>();
        List<string> statusFilter = new List<string>();

        public ucBookCatalogFilter()
        {
            InitializeComponent();
            db = new dbhandler();
            ucBC = new ucBookCatalog();
        }

        private void ucBookCatalogFilter_Load(object sender, EventArgs e)
        {
            GetAllBookGenres();
        }

        public void GetAllBookGenres()
        {
            genres = db.GetAllGenres();

            for (int i = 0; i < genres.Count; i++)
            {
                // int j = 0;
                ChkBx = new Guna2CheckBox
                {
                    Text = genres[i],
                    Name = "ChkBx_Genre" + i,
                    Padding = new Padding(0),
                    Margin = new Padding(0, 0, 5, 0),
                    Size = new System.Drawing.Size(115, 50)
                };
                Pnl_GenreFilters.Controls.Add(ChkBx);
                // j = (j == 2) ? 0 : j + 1;
            }

            //tblGenreFilters.AutoScroll = false;
            //tblGenreFilters.HorizontalScroll.Enabled = false;
            //tblGenreFilters.HorizontalScroll.Visible = false;
            //tblGenreFilters.AutoScroll = true;
        }

        private void Btn_ClearFilter_Click(object sender, EventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show
                (
                    "Clear Filter?",
                    "Warning",
                    (MessageBoxButtons)MessageBoxButton.OKCancel,
                    (MessageBoxIcon)MessageBoxImage.Warning,
                    MessageBoxDefaultButton.Button2
                );

            if (result == DialogResult.OK)
            {
                foreach (Control ctrl in Pnl_GenreFilters.Controls)
                {
                    if (ctrl is CheckBox ChkBx)
                    {
                        ChkBx.Checked = false;
                    }
                }

                foreach (Control ctrl in Pnl_StatusFilters.Controls)
                {
                    if (ctrl is CheckBox ChkBx)
                    {
                        ChkBx.Checked = false;
                    }
                }

                genreFilter.Clear();
                statusFilter.Clear();
            }
            
        }

        private void Btn_ApplyFilter_Click(object sender, EventArgs e)
        {
            genreFilter.Clear();
            statusFilter.Clear();
            foreach (Control ctrl in Pnl_GenreFilters.Controls)
            {
                if (ctrl is CheckBox ChkBx)
                {
                    if (ChkBx.Checked == true)
                    {
                        genreFilter.Add(ChkBx.Text);
                    }
                }
            }

            foreach (Control ctrl in Pnl_StatusFilters.Controls)
            {
                if (ctrl is CheckBox ChkBx)
                {
                    if (ChkBx.Checked == true)
                    {
                        statusFilter.Add(ChkBx.Text + " > 0 ");
                    }
                }
            }

            // System.Windows.MessageBox.Show("Selected Genres:\n\n" + string.Join(", ", genreFilter) + "\n\n" + "Selected Status:\n\n" + string.Join(", ", statusFilter));
            ucBC.ApplyTableFilters(genreFilter, statusFilter);
        }

        private void Pnl_GenreFilters_ScrollHandler(object sender, ScrollEventArgs e)
        {
            Panel p = (Panel)sender;
            p.VerticalScroll.Value = e.NewValue;
        }
    }
}
