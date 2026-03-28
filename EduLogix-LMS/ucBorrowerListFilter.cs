using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EduLogix_LMS
{
    public partial class ucBorrowerListFilter : UserControl
    {

        dbhandler db = new();
        List<string> grades = new List<string>();
        public List<string> appliedGradeFilters = new List<string>();
        public List<string> sortFilters = new List<string>();
        public bool applyFitler = false;
        private ucBorrowerList parentBorrowerList;

        public ucBorrowerListFilter(ucBorrowerList parent = null)
        {
            InitializeComponent();
            parentBorrowerList = parent;
            grades = db.GetDistinctGrades();
        }

        private void ucBorrowerListFilter_Load(object sender, EventArgs e)
        {
            HandleGradeTableCheckBoxes();
            btnApply.Click += (object sender, EventArgs e) =>
            {
                if (parentBorrowerList != null)
                {
                    parentBorrowerList.ApplyCurrentFilters(appliedGradeFilters);
                    this.Visible = false;
                }
            };
        }

        public List<string> GetAppliedGradeFilter()
        {
            return appliedGradeFilters;
        }



        private void HandleGradeTableCheckBoxes()
        {
            tblGradeLevel.AutoSize = true;
            tblGradeLevel.AutoSizeMode = AutoSizeMode.GrowOnly;
            tblGradeLevel.AutoScroll = false;
            tblGradeLevel.ColumnStyles.Clear();
            tblGradeLevel.RowStyles.Clear();

            tblGradeLevel.ColumnCount = 3;
            for (int i = 0; i < 3; i++)
                tblGradeLevel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            int currentCol = 0;
            int currentRow = 1;

            foreach (string grade in grades)
            {
                CheckBox chk = new CheckBox();
                chk.Text = "Grade " + grade;
                chk.AutoSize = true;
                chk.Margin = new Padding(5);
                chk.Dock = DockStyle.Top;
                chk.Tag = grade;

                chk.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (!appliedGradeFilters.Contains(chk.Tag.ToString()))
                        appliedGradeFilters.Add(chk.Tag.ToString());
                    else appliedGradeFilters.Remove(chk.Tag.ToString());
                    return;
                };

                if (tblGradeLevel.RowStyles.Count <= currentRow)
                    tblGradeLevel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                tblGradeLevel.Controls.Add(chk, currentCol, currentRow);

                currentCol++;
                if (currentCol >= 3)
                {
                    currentCol = 0;
                    currentRow++;
                }
            }
        }

        private void handleIsAsencding(object sender, EventArgs e)
        {
            if (!sortFilters.Contains("a-z")) sortFilters.Add("a-z");
            else sortFilters.Remove("a-z");
        }
    }
}
