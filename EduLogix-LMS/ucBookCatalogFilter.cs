//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace EduLogix_LMS
{
    public partial class ucBookCatalogFilter : UserControl
    {
        dbhandler db;
        private ucBookCatalog ucBC;
        Guna2CheckBox ChkBx_Borrowed;
        List<string> genres = new List<string>();
        List<string> genreFilter = new List<string>();
        List<string> statusFilter = new List<string>();

        public ucBookCatalogFilter(ucBookCatalog parent = null)
        {
            InitializeComponent();
            db = new dbhandler();
            ucBC = parent;
        }

        private void ucBookCatalogFilter_Load(object sender, EventArgs e)
        {
            GetAllBookGenres();
        }

        // Generates a dynamic checkbox for every genre in the database
        public void GetAllBookGenres()
        {
            genres = db.GetAllGenres();

            for (int i = 0; i < genres.Count; i++)
            {
                ChkBx = new Guna2CheckBox
                {
                    Text = genres[i],
                    Name = "ChkBx_Genre" + i,
                    Padding = new Padding(0),
                    Margin = new Padding(0, 0, 5, 0),
                    Size = new System.Drawing.Size(115, 50)
                };
                Pnl_GenreFilters.Controls.Add(ChkBx);
            }
        }

        // Unchecks all checkboxes and clears the filter query
        private void Btn_ClearFilter_Click(object sender, EventArgs e)
        {
            if (Btn_ClearFilter.Text == "Confirm?")
            {
                Btn_ClearFilter.Text = "Clear";
                Btn_ClearFilter.FillColor = Color.FromArgb(192, 165, 123);
                Btn_ClearFilter.FillColor2 = Color.FromArgb(255, 201, 118);
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
            else
            {
                Btn_ClearFilter.Text = "Confirm?";
                Btn_ClearFilter.FillColor = Color.FromArgb(183, 128, 128);
                Btn_ClearFilter.FillColor2 = Color.FromArgb(237, 128, 128);
            }

            ucBC.ApplyTableFilters(genreFilter, statusFilter);
        }

        // Applies all checked checkboxes and passes the generated List<> to be parsed into a query
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
                        genreFilter.Add("'" + ChkBx.Text + "'");
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

            // MessageBox.Show("Selected Genres:\n\n" + string.Join(", ", genreFilter) + "\n\n" + "Selected Status:\n\n" + string.Join(", ", statusFilter));
            ucBC.ApplyTableFilters(genreFilter, statusFilter);
        }

        // Makes the filter list scroll less jittery (somewhat..)
        private void Pnl_GenreFilters_ScrollHandler(object sender, ScrollEventArgs e)
        {
            Panel p = (Panel)sender;
            p.VerticalScroll.Value = e.NewValue;
        }
    }
}
