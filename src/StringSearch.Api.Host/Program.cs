using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using StringSearch.Di;
using StringSearch.Infrastructure.Di;

namespace StringSearch.Api.Host
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            createWebHostBuilder(args).Build().Run();
        }

        private static IHostBuilder createWebHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => 
                    webBuilder
                        .UseStartup<Startup>()
                        .ConfigureServices(services =>
                            services
                                .RegisterInfrastructureDependencies()
                                .AddStringSearch()));
    }
}
