using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullStackJobs.AuthServer.Infrastructure.Data.Identity
{
    public class AppUser : IdentityUser
    {
        // Add additional profile data for application users
        // by adding properties to this class
        public string FullName { get; set; }
    }
}
