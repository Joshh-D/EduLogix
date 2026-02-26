using System;
using System.Data;
using System.Drawing;
using MySql.Data.MySqlClient;

namespace EduLogix
{
    public static class DatabaseFunctions
    {
        public static readonly string DefaultConnectionString = "server=localhost;database=edulogix;uid=root;";

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

        public static bool InsertNewStudent(
            MySqlConnection conn,
            string rfidNumber,
            string studentId,
            string name,
            string guardianName,
            string guardianPhoneNumber,
            string address,
            int grade,
            string section,
            string level,
            string imagePath = "uploads/default.png")
        {
            if (conn == null) throw new ArgumentNullException(nameof(conn));
            if (string.IsNullOrWhiteSpace(studentId)) throw new ArgumentException("studentId is required", nameof(studentId));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is required", nameof(name));

            if (!string.IsNullOrWhiteSpace(guardianPhoneNumber))
            {
                string digitsOnly = string.Empty;
                foreach (char c in guardianPhoneNumber)
                {
                    if (char.IsDigit(c)) digitsOnly += c;
                }
                if (digitsOnly.Length > 11)
                    throw new ArgumentException("guardianPhoneNumber must be at most 11 digits", nameof(guardianPhoneNumber));
                guardianPhoneNumber = digitsOnly;
            }

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
                    cmd.CommandText =
                       "INSERT INTO `edulogix`.`reg_studentinfo` " +
                       "(`image_path`, `rfid_number`, `student_id`, `name`, `guardian_name`, `guardian_phone_number`, `address`, `grade`, `section`, `level`) " +
                       "VALUES (@image_path, @rfid_number, @student_id, @name, @guardian_name, @guardian_phone_number, @address, @grade, @section, @level) ";

                    cmd.Parameters.AddWithValue("@image_path", (object)imagePath ?? "uploads/default.png");
                    cmd.Parameters.AddWithValue("@rfid_number", (object)rfidNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@student_id", studentId);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@guardian_name", (object)guardianName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@guardian_phone_number", string.IsNullOrWhiteSpace(guardianPhoneNumber) ? (object)DBNull.Value : (object)guardianPhoneNumber);
                    cmd.Parameters.AddWithValue("@address", (object)address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@grade", grade);
                    cmd.Parameters.AddWithValue("@section", (object)section ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@level", (object)level ?? DBNull.Value);

                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            finally
            {
                if (openedHere)
                    conn.Close();
            }
        }

        public static bool InsertNewStudent(
            string connectionString,
            string rfidNumber,
            string studentId,
            string name,
            string guardianName,
            string guardianPhoneNumber,
            string address,
            int grade,
            string section,
            string level,
            string imagePath = "uploads/default.png")
        {
            using (var conn = new MySqlConnection(connectionString ?? DefaultConnectionString))
            {
                conn.Open();
                return InsertNewStudent(conn, rfidNumber, studentId, name, guardianName, guardianPhoneNumber, address, grade, section, level, imagePath);
            }
        }

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