using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Auth4.WebApi
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
                            .AddJsonFile(Path.Combine(sharedFolder, "WebApi.settings.json"), true) // local run
                            .AddJsonFile("WebApi.settings.json", true); // docker run
                    }); });
        }
    }
}