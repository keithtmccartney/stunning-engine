using Microsoft.AspNetCore.Server.IISIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullStackJobs.AuthServer.Models
{
    public class AccountOptions
    {
        public static bool AllowLocalLogin = true;
        public static bool AllowRememberLogin = true;
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
        public static bool ShowLogoutPrompt = true;
        public static bool AutomaticRedirectAfterSignOut = true;
        public static readonly string WindowsAuthenticationSchemeName = IISDefaults.AuthenticationScheme;
        public static bool IncludeWindowsGroups = false;
        public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}
