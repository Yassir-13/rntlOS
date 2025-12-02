namespace rntlOS.BackOffice
{
    public static class UserSession
    {
        public static int UserId { get; set; }
        public static string UserName { get; set; }
        public static string UserRole { get; set; }
        public static bool IsAuthenticated { get; set; }

        public static void Login(int userId, string userName, string userRole)
        {
            UserId = userId;
            UserName = userName;
            UserRole = userRole;
            IsAuthenticated = true;
        }

        public static void Logout()
        {
            UserId = 0;
            UserName = null;
            UserRole = null;
            IsAuthenticated = false;
        }
    }
}