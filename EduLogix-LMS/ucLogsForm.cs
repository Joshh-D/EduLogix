using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using System.IO;


namespace EduLogix_LMS
{

    public partial class ucLogsForm : UserControl
    {
        dbhandler db = new dbhandler();

        public ucLogsForm()
        {
            InitializeComponent();
        }

        UserControl ucLogsFilterObj;
        bool isFilterDisplayed = false;


        private void ucLogsForm_Load(object sender, EventArgs e)
        {
            ucLogsFilterObj = new ucLogsFilter();
            ucLogsFilterObj.Location = new System.Drawing.Point(
                btnFilter.Location.X + 10,
                tableLayoutPanel1.Location.Y + tableLayoutPanel1.Size.Height + 5
            );
            ucLogsFilterObj.Visible = false;
            pnlBackground.Controls.Add(ucLogsFilterObj);

            LoadLogs(); // 🔥 THIS IS THE IMPORTANT PART
        }
        private void LoadLogs()
        {
            guna2DataGridView1.DataSource = db.GetAllLogs(); // ⚠️ CHANGE NAME if needed
        }
        private void btnFilter_Click(object sender, EventArgs e)
        {
            bool isVisible = ucLogsFilterObj.Visible;
            ucLogsFilterObj.Visible = !isVisible;

            if (ucLogsFilterObj.Visible)
                ucLogsFilterObj.BringToFront();

            isFilterDisplayed = ucLogsFilterObj.Visible;
        }

        private void guna2GradientButton2_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
            sfd.FileName = "Logs_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Logs");

                        // Headers
                        for (int i = 0; i < guna2DataGridView1.Columns.Count; i++)
                        {
                            ws.Cell(1, i + 1).Value = guna2DataGridView1.Columns[i].HeaderText;
                        }
                        ws.Row(1).Style.Font.Bold = true;
                        // Data
                        for (int i = 0; i < guna2DataGridView1.Rows.Count; i++)
                        {
                            for (int j = 0; j < guna2DataGridView1.Columns.Count; j++)
                            {
                                ws.Cell(i + 2, j + 1).Value =
                                    guna2DataGridView1.Rows[i].Cells[j].Value?.ToString();
                            }
                        }

                        ws.Columns().AdjustToContents();

                        wb.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("Export successful!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
