using MySqlConnector;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace EduLogix_LMS
{
    internal class dbhandler
    {

        //mark
        private string connectionString = "server=localhost;database=edulogix-lms;uid=root;pwd=root;";
        private string connectionStringRegistrar = "server=localhost;database=edulogix;uid=root;pwd=root;";

        //private string connectionString = "server=192.168.1.18;database=edulogix-lms;uid=arduino_user;pwd=secret;";
        MySqlConnection conn = new MySqlConnection();

        public void AddBorrower(KeyValuePair<string, string> borrowerInfo, KeyValuePair<string, string> bookInfo)
        {
            try {
                using (MySqlConnection loginConn = new MySqlConnection(connectionString))
                {

                    // just filter to get borrower books borrowed. continue creating the table for this query
                    string query = "INSERT INTO `edulogix-lms`.lms_borrower_list (student_name, grade, section, isbn, title) VALUES (@name, @grade, @section, @isbn, @title)";
                    MySqlCommand cmd = new MySqlCommand(query, loginConn);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Adding of new borrower Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void AddBook(string isbn, string title, string author, string genre, string copies)
        {
            try
            {
                using (MySqlConnection lmsConnection = new MySqlConnection(connectionString))
                {

                    string query = "INSERT INTO `edulogix-lms`.lms_book_catalogue (isbn, title, author, genre, copies, available) VALUES (@isbn, @title, @author, @genre, @copies, @available, @path)";
                    MySqlCommand cmd = new MySqlCommand(query, lmsConnection);

                    cmd.Parameters.AddWithValue("@isbn", isbn);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@author", author);
                    cmd.Parameters.AddWithValue("@genre", genre);
                    cmd.Parameters.AddWithValue("@copies", copies);
                    cmd.Parameters.AddWithValue("@available", copies);

                    string username = Environment.UserName;
                    string path = @"C:\Users\" + username + @"\Documents\EduLogix-LMS\book_covers\default.jpg";
                    cmd.Parameters.AddWithValue("@path", path);

                    var res = cmd.ExecuteNonQuery();
                    if (res > 0) MessageBox.Show("Book added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else MessageBox.Show("Failed to add book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            catch (MySqlException ex)
            {
                MessageBox.Show("Adding of new borrower Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void EditBook(string isbn, string title, string author, string genre, string copies)
        {
            try
            {
                using (MySqlConnection lmsConnection = new MySqlConnection(connectionString))
                {
                    string query = "UPDATE `edulogix-lms`.lms_book_catalogue SET title = @title, author = @author, genre = @genre, copies = @copies WHERE isbn = @isbn";
                    MySqlCommand cmd = new MySqlCommand(query, lmsConnection);

                    cmd.Parameters.AddWithValue("@isbn", isbn);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@author", author);
                    cmd.Parameters.AddWithValue("@genre", genre);
                    cmd.Parameters.AddWithValue("@copies", copies);

                    lmsConnection.Open();
                    var res = cmd.ExecuteNonQuery();
                    if (res > 0) MessageBox.Show("Book updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else MessageBox.Show("Failed to update book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            catch (MySqlException ex)
            {
                MessageBox.Show("Editing book Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ArchiveBook(string isbn)
        {
            try
            {
                using (MySqlConnection lmsConnection = new MySqlConnection(connectionString))
                {
                    string query = "DELETE FROM `edulogix-lms`.lms_book_catalogue WHERE isbn = @isbn";
                    MySqlCommand cmd = new MySqlCommand(query, lmsConnection);

                    cmd.Parameters.AddWithValue("@isbn", isbn);

                    lmsConnection.Open();
                    var res = cmd.ExecuteNonQuery();
                    if (res > 0) MessageBox.Show("Book archived successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else MessageBox.Show("Failed to archive book.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            catch (MySqlException ex)
            {
                MessageBox.Show("Archiving book Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public DataRow VerifyLogin(string username, string password)
        {
            string query = "SELECT username, role, rfid_number FROM edulogix.sys_users WHERE username = @user AND password = @pass LIMIT 1";

            DataTable resultTable = new DataTable();
            try
            {
                using (MySqlConnection loginConn = new MySqlConnection(connectionStringRegistrar))
                {
                    MySqlCommand cmd = new MySqlCommand(query, loginConn);
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(resultTable);
                }

                if (resultTable.Rows.Count > 0)
                {
                    return resultTable.Rows[0];
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Login Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        public DataTable SearchBar(string table, string filter)
        {
            conn.ConnectionString = connectionString;
            DataTable tableData = new DataTable();
            try
            {
                string query = "";
                if (table.Contains("lms_borrower_list"))
                    query = "SELECT * FROM `edulogix-lms`." + table + " WHERE student_name LIKE @filter OR grade LIKE @filter OR section LIKE @filter ORDER BY student_name";
                else if (table.Contains("lms_book_catalogue"))
                    query = "SELECT isbn as 'ISBN', title as 'Title', author as 'Author', genre as 'Genre', copies as 'Copies', available as 'Available', borrowed as 'Borrowed', overdue as 'Overdue', missing as 'Missing' FROM `edulogix-lms`." + table + " WHERE title LIKE @filter OR author LIKE @filter OR genre LIKE @filter ORDER BY title";


                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@filter", "%" + filter + "%");
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(tableData);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return tableData;
        }

        public DataTable GetAllBooks()
        {
            conn.ConnectionString = connectionString;
            DataTable table = new DataTable();
            try
            {
                string query = "SELECT isbn as 'ISBN', title as 'Title', author as 'Author', genre as 'Genre', copies as 'Copies', available as 'Available', borrowed as 'Borrowed', overdue as 'Overdue', missing as 'Missing' FROM `edulogix-lms`.lms_book_catalogue ORDER BY title";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                adapter.Fill(table);

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return table;
        }

        public List<string> GetDistinctGrades()
        {
            List<string> grades = new List<string>();
            conn.ConnectionString = connectionStringRegistrar;
            string query = "SELECT distinct grade from edulogix.reg_studentinfo order by grade asc;";

            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        grades.Add(reader["grade"].ToString());
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return grades;
        }

        // mark
        public DataTable GetBorrowerList(bool isGradeFilter = false, List<string>? gradeFilter = null, bool isAscending = true)
        {
            conn.ConnectionString = connectionString;
            DataTable table = new DataTable();
            
            try
            {
                string query = "SELECT * FROM `edulogix-lms`.lms_borrower_list";

                if (isGradeFilter && gradeFilter.Count > 0)
                {
                    query += " WHERE ";
                    int index = 1;
                    foreach (string grade in gradeFilter)
                    {
                        query += "grade = " + grade + (index < gradeFilter.Count ? " OR " : "") ;
                        index++;
                    }
                }

                if (isAscending) query += " ORDER BY student_name";

                //MessageBox.Show(query);
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

        public List<string> GetAllGenres()
        {
            List<string> genres = new List<string>();
            conn.ConnectionString = connectionString;
            string query = "SELECT DISTINCT genre FROM `edulogix-lms`.lms_book_catalogue ORDER BY genre ASC;";

            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        genres.Add(reader["genre"].ToString());
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return genres;
        }
    }
}
