using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using StringSearch.Api.Di;
using StringSearch.Api.Mvc.ActionFilters;
using StringSearch.Api.Mvc.Middleware;
using StringSearch.Api.Mvc.ModelBinding;

namespace StringSearch.Api
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly ILoggerFactory loggerFactory;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.loggerFactory = loggerFactory;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .RegisterApiDependencies()
                .AddCors()
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PiSearch API v1", Version = "v1" });
                });

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ValidateModelsAttribute));
                options.RegisterAllCustomModelBinders(loggerFactory);
            }).AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseCors(builder => builder.AllowAnyOrigin());
            app.UseMiddleware<ExceptionRequestHandler>();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            ForwardedHeadersOptions forwardingOptions = new()
            {
                ForwardedHeaders = ForwardedHeaders.All
            };
            // Defaults to only allow localhost, as it's written for IIS
            //  As nginx is in a separate container it won't trust it, and will therefore ignore the X-Forwarded-* headers from it
            //  Clearing these means it acepts these headers from any source (but the docker network setup limits who can
            //  call it directly, so it's still protected without this)
            //  see: https://stackoverflow.com/a/44390593/5401981
            forwardingOptions.KnownNetworks.Clear();
            forwardingOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardingOptions);

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PiSearch API v1");
            });
        }
    }
}
