using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Auth4.AuthDev
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>()
                    .ConfigureAppConfiguration((context, builder) =>
                    {
                        var env = context.HostingEnvironment;
                        var sharedFolder = Path.Combine(env.ContentRootPath, "../Docker.local", ".settings");
                        builder
                            .AddJsonFile("appsettings.json", true)
                            .AddJsonFile(Path.Combine(sharedFolder, "AuthDev.settings.json"), true) // local run
                            .AddJsonFile("AuthDev.settings.json", true); // docker run
                    }); });
        }
    }
}