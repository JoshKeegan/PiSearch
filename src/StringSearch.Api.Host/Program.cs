using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using StringSearch.Api.Infrastructure.Di;

namespace StringSearch.Api.Host
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            createWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder createWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    services.RegisterInfrastructureDependencies();
                });
    }
}
