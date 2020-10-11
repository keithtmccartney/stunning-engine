using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FullStackJobs.AuthServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (webBuilder.GetSetting("environment") == "Development")
                    {
                        webBuilder.UseKestrel((host, options) =>
                        {
                            options.Listen(IPEndPoint.Parse(host.Configuration.GetValue("AppSettings:Address", "")));
                        });
                    }

                    webBuilder.UseStartup<Startup>();
                });
    }
}
