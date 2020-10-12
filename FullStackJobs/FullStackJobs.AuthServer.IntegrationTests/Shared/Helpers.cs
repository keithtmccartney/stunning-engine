using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FullStackJobs.AuthServer.IntegrationTests.Shared
{
    public static class Helpers
    {
        public static IConfigurationRoot GetConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static string GetProjectPath(Assembly startupAssembly)
        {
            var projectName = startupAssembly.GetName().Name;

            var applicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;

            DirectoryInfo directoryInfo = new DirectoryInfo(applicationBasePath);

            do
            {
                var solutionFileInfo = directoryInfo.GetFiles("*.sln").FirstOrDefault();

                if (solutionFileInfo != null)
                {
                    return Path.GetFullPath(Path.Combine(directoryInfo.FullName, projectName));
                }

                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Solution root could not be located using application root {applicationBasePath}");
        }
    }
}
