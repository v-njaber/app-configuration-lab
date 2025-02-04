using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace configuration_lab
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            // This will pull configuration from all sources, so it will work with azure app service configuration.
            WorkingConfiguration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // used to inject controllers with IConfiguration object.
            services.AddSingleton<IConfiguration>(provider => WorkingConfiguration);
            // Enables attribute 
            services.AddControllers();
        }

        public IConfiguration WorkingConfiguration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/working", async context =>
                {
                    await sendConnectionStringResponse(context, WorkingConfiguration);
                }); 
                endpoints.MapGet("/not-working", async context =>
                {
                    // This will pull configuration from appsettings.json only, so it won't work with azure app service configuration.
                    var NotWorkingConfiguration = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                    
                    await sendConnectionStringResponse(context, NotWorkingConfiguration);
                });
            });


        }
        public Task sendConnectionStringResponse(HttpContext context, IConfiguration configuration) {
            string connection = configuration.GetConnectionString("MyDbConnection");
            if (connection == null || connection.Length == 0) connection = "Connnection string not found";
            return context.Response.WriteAsync(connection);
        }

    }
}
