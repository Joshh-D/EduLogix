using System;
using System.Drawing;
using System.IO;
using MySql.Data.MySqlClient;

namespace EduLogix
{
    /// <summary>
    /// Helper class for loading user profile images from database and file system
    /// Deprecated: user_profile column has been removed from sys_users table
    /// </summary>
    public static class UserProfileHelper
    {
        private readonly static string ConnectionString = "server=localhost;database=edulogix;uid=root;pwd=;";

        /// <summary>
        /// Returns default student image since user_profile column was removed
        /// </summary>
        public static Image LoadUserProfile(string username)
        {
            System.Diagnostics.Debug.WriteLine("[UserProfileHelper] user_profile column has been removed from database");
            System.Diagnostics.Debug.WriteLine("[UserProfileHelper] Returning default student image");

            try { return (Image)Properties.Resources.student?.Clone(); }
            catch { return null; }
        }

        /// <summary>
        /// Clears the cached profile image (call this on logout)
        /// </summary>
        public static void ClearCache()
        {
            System.Diagnostics.Debug.WriteLine("[UserProfileHelper] Cache cleared");
        }
    }
}
