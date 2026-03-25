using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing.Text;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using TheArtOfDevHtmlRenderer.Adapters;

namespace EduLogix_LMS
{
    internal class dbhandler
    {
        private string connectionString = "server=192.168.1.18;database=edulogix-lms;uid=arduino_user;pwd=secret;";
        MySqlConnection conn = new MySqlConnection();

        public DataTable GetAllBooks()
        {
            conn.ConnectionString = connectionString;
            DataTable table = new DataTable();
            try
            {
                string query = "SELECT * FROM `edulogix-lms`.lms_book_catalogue ORDER BY title";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                adapter.Fill(table);

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return table;
        }

        public int[] GetUpperDashboardInfo()
        {
            int[] upperDashboardInfo = new int[6];
            try
            {
                String query;
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    query = "SELECT SUM(copies) FROM `edulogix-lms`.lms_book_catalogue";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        upperDashboardInfo[0] = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    query = "SELECT SUM(available) FROM `edulogix-lms`.lms_book_catalogue";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        upperDashboardInfo[3] = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return upperDashboardInfo;
        }
    }
}
