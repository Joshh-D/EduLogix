using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EduLogix
{
    internal static class BrandingHelper
    {
        public static void ApplySchoolBranding(string connectionString, Control schoolNameControl, PictureBox schoolLogoControl)
        {
            if (schoolNameControl == null && schoolLogoControl == null) return;

            try
            {
                string schoolName = null;
                string schoolLogoFromDb = null;

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    const string query = "SELECT school_name, school_logo FROM reg_settings WHERE id = 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            schoolName = reader["school_name"] != DBNull.Value
                                ? reader["school_name"].ToString()
                                : null;

                            schoolLogoFromDb = reader["school_logo"] != DBNull.Value
                                ? reader["school_logo"].ToString()
                                : null;
                        }
                    }
                }

                if (schoolNameControl != null && !string.IsNullOrWhiteSpace(schoolName))
                {
                    schoolNameControl.Text = schoolName;
                }

                if (schoolLogoControl != null)
                {
                    string logoPath = ResolveLogoPath(schoolLogoFromDb);
                    if (!string.IsNullOrWhiteSpace(logoPath) && File.Exists(logoPath))
                    {
                        SetPictureBoxImageNoLock(schoolLogoControl, logoPath);
                    }
                }
            }
            catch
            {
                // keep silent to avoid breaking form load
            }
        }

        private static string ResolveLogoPath(string schoolLogoFromDb)
        {
            if (!string.IsNullOrWhiteSpace(schoolLogoFromDb))
            {
                if (Path.IsPathRooted(schoolLogoFromDb) && File.Exists(schoolLogoFromDb))
                    return schoolLogoFromDb;

                string normalizedRelative = schoolLogoFromDb.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);

                string startupRelative = Path.Combine(Application.StartupPath, normalizedRelative);
                if (File.Exists(startupRelative))
                    return startupRelative;

                string projectRelative = Path.GetFullPath(Path.Combine(Application.StartupPath, "..", "..", normalizedRelative));
                if (File.Exists(projectRelative))
                    return projectRelative;
            }

            string startupDefault = Path.Combine(Application.StartupPath, "Resources", "logo", "logo.png");
            if (File.Exists(startupDefault))
                return startupDefault;

            string projectDefault = Path.GetFullPath(Path.Combine(Application.StartupPath, "..", "..", "Resources", "logo", "logo.png"));
            if (File.Exists(projectDefault))
                return projectDefault;

            return null;
        }

        private static void SetPictureBoxImageNoLock(PictureBox pictureBox, string filePath)
        {
            if (pictureBox == null || string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return;

            if (pictureBox.Image != null)
            {
                var oldImage = pictureBox.Image;
                pictureBox.Image = null;
                oldImage.Dispose();
            }

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var img = Image.FromStream(fs))
            {
                pictureBox.Image = new Bitmap(img);
            }

            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        }
    }
}
