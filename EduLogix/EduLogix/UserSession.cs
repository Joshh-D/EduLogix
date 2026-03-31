namespace EduLogix
{
    internal static class UserSession
    {
        public static string UserName { get; set; }
        public static string Role { get; set; }

        public static bool IsSuperAdmin
        {
            get
            {
                return string.Equals(Role, "SuperAdmin", System.StringComparison.OrdinalIgnoreCase);
            }
        }

        public static void Clear()
        {
            UserName = null;
            Role = null;
            UserProfileHelper.ClearCache();
        }
    }
}
