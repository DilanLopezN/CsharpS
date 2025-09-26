using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace Simjob.Framework.Services.Api
{
    public class Program
    {
        public static ApplicationStorage mainStorage = new ApplicationStorage();

        public static void Main(string[] args)
        {
            EnvironmentSettings.Flush();
            Console.WriteLine("Main storage directory: " + mainStorage.Prefix);
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11;
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseKestrel(options =>
                    {
                        options.Limits.MaxRequestBodySize = long.MaxValue;

                    });

                });
    }
}
