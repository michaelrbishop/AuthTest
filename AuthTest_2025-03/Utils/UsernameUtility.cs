namespace AuthTest_2025_03.Utils
{
    public static class UsernameUtility
    {
        public static string ExtractUsernameFromEmail(string email)
        {
            return email.Split("@")[0];
        }

        public static string GenerateUniqueUsername(string userName)
        {
            return $"{userName}#{DateTime.Now.Ticks}";
        }
    }
}
