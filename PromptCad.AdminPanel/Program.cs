namespace PromptCad.AdminPanel
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // If in the access_token.txt have a valid token, then skip the login form
            var isTokenValid = Task.Run(() => Utils.CheckAccessToken.IsTokenValidAsync()).Result;
            if (isTokenValid)
            {
                // If the token is valid, you can proceed to the main application form
                // For example, you might want to show a main dashboard form here
                Application.Run(new DashboardForm());
                return; // Exit the application if you don't want to show the login form
            }
            Application.Run(new LoginForm());
        }
    }
}