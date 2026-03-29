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


        // []==========[ Dashboard Functions ]==========[]
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

                    query = "SELECT SUM(missing) FROM `edulogix-lms`.lms_book_catalogue";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        upperDashboardInfo[1] = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    query = "SELECT SUM(available) FROM `edulogix-lms`.lms_book_catalogue";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        upperDashboardInfo[2] = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    query = "SELECT SUM(borrowed) FROM `edulogix-lms`.lms_book_catalogue";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        upperDashboardInfo[3] = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    query = "SELECT SUM(overdue) FROM `edulogix-lms`.lms_book_catalogue";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        upperDashboardInfo[4] = Convert.ToInt32(cmd.ExecuteScalar());
                    }


                }

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return upperDashboardInfo;
        }

        // []==========[ Book Catalogue Functions ]==========[]
        public DataTable GetAllBooks(string query)
        {
            conn.ConnectionString = connectionString;
            DataTable table = new DataTable();
            try
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                adapter.Fill(table);

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return table;
        }

        public List<string> GetAllGenres()
        {
            conn.ConnectionString = connectionString;
            List<string> listStore = new List<string>();
            try
            {
                string query = "SELECT DISTINCT genre FROM `edulogix-lms`.lms_book_catalogue ORDER BY genre";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listStore.Add(reader.GetString(0));
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return listStore;
        }

        public void FilterBookCatalogTable(List<string> genres, List<string> status)
        {

            // string query = "SELECT * FROM `edulogix-lms`.lms_book_catalogue "

        }
    }
}
