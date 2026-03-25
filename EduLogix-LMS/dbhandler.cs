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

        public void StartDBConn()
        {
            using(MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // MessageBox.Show("Connection Success");
                }
                catch (MySqlException ex)
                {
                    // MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        public DataTable GetAllBooks()
        {
            conn.ConnectionString = connectionString;
            DataTable table = new DataTable();
            string query = "SELECT * FROM `edulogix-lms`.lms_book_catalogue ORDER BY title";

            MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
            adapter.Fill(table);

            return table;
        }
    }
}
