using FullStackJobs.AuthServer.Infrastructure.Data;
using FullStackJobs.Tests;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FullStackJobs.AuthServer.IntegrationTests.Fixtures
{
    public class AuthServerWebApplicationFactory<TTestStartup, TDbContext> : WebApplicationFactory<Startup> where TTestStartup : class where TDbContext : DbContext
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(null)
                .UseStartup<TTestStartup>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureServices(services =>
            {
                services.AddScoped<IUserRepository, MockUserRepository>();
                services.AddInMemoryDataAccessServices<TDbContext>();
            });
        }
    }
}
