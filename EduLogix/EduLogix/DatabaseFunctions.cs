using System;
using System.Data;
using System.Drawing;
using MySql.Data.MySqlClient;

namespace EduLogix
{
    public static class DatabaseFunctions
    {
        public static readonly string DefaultConnectionString = "server=localhost;database=edulogix;uid=root;";

        // Logging helpers (existing)
        public static void Log(MySqlConnection conn, string username, string action, string role)
        {
            if (conn == null) throw new ArgumentNullException(nameof(conn));

            bool openedHere = false;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
                openedHere = true;
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO reg_logs (name, role, action, date_and_time) VALUES (@username, @role, @action, @dt)";
                cmd.Parameters.AddWithValue("@dt", DateTime.Now);
                cmd.Parameters.AddWithValue("@username", (object)username ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@action", (object)action ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@role", (object)role ?? DBNull.Value);


                cmd.ExecuteNonQuery();
            }

            if (openedHere)
                conn.Close();
        }

        public static void Log(string connectionString, string username, string action, string role)
        {
            using (var conn = new MySqlConnection(connectionString ?? DefaultConnectionString))
            {
                conn.Open();
                Log(conn, username, action, role);
            }
        }

        // Returns DataTable of students with optional filters
        public static DataTable GetStudents(MySqlConnection conn, string level = null, int? grade = null)
        {
            if (conn == null) throw new ArgumentNullException(nameof(conn));

            bool openedHere = false;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
                openedHere = true;
            }

            try
            {
                string query = "SELECT student_id, name, grade, section, level FROM reg_studentinfo";
                using (var cmd = conn.CreateCommand())
                {
                    var filters = new System.Collections.Generic.List<string>();

                    if (!string.IsNullOrWhiteSpace(level) && level != "All")
                    {
                        filters.Add("level = @level");
                        cmd.Parameters.AddWithValue("@level", level);
                    }

                    if (grade.HasValue)
                    {
                        filters.Add("grade = @grade");
                        cmd.Parameters.AddWithValue("@grade", grade.Value);
                    }

                    if (filters.Count > 0)
                        query += " WHERE " + string.Join(" AND ", filters);

                    cmd.CommandText = query;

                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
            finally
            {
                if (openedHere)
                    conn.Close();
            }
        }

        public static DataTable GetStudents(string connectionString, string level = null, int? grade = null)
        {
            using (var conn = new MySqlConnection(connectionString ?? DefaultConnectionString))
            {
                return GetStudents(conn, level, grade);
            }
        }

        // Search students by keyword (matches multiple columns)
        public static DataTable SearchStudents(MySqlConnection conn, string keyword)
        {
            if (conn == null) throw new ArgumentNullException(nameof(conn));

            bool openedHere = false;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
                openedHere = true;
            }

            try
            {
                string query = "SELECT student_id, name, grade, section, level FROM reg_studentinfo WHERE " +
                               "(student_id LIKE @search OR name LIKE @search OR grade LIKE @search OR section LIKE @search OR level LIKE @search)";

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@search", "%" + (keyword ?? string.Empty) + "%");

                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
            finally
            {
                if (openedHere)
                    conn.Close();
            }
        }

        public static DataTable SearchStudents(string connectionString, string keyword)
        {
            using (var conn = new MySqlConnection(connectionString ?? DefaultConnectionString))
            {
                return SearchStudents(conn, keyword);
            }
        }

        // Get theme color from reg_theme id=1
        public static Color? GetThemeColor(MySqlConnection conn)
        {
            if (conn == null) throw new ArgumentNullException(nameof(conn));

            bool openedHere = false;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
                openedHere = true;
            }

            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT theme_red, theme_green, theme_blue FROM reg_theme WHERE id = 1";
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int r = Convert.ToInt32(reader["theme_red"]);
                            int g = Convert.ToInt32(reader["theme_green"]);
                            int b = Convert.ToInt32(reader["theme_blue"]);
                            return Color.FromArgb(r, g, b);
                        }
                    }
                }
                return null;
            }
            finally
            {
                if (openedHere)
                    conn.Close();
            }
        }

        public static Color? GetThemeColor(string connectionString)
        {
            using (var conn = new MySqlConnection(connectionString ?? DefaultConnectionString))
            {
                return GetThemeColor(conn);
            }
        }
    }
}