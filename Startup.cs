
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MovieApi.Models;
using Microsoft.AspNetCore.SignalR;

namespace MovieApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                       .AllowAnyMethod()
                       .SetIsOriginAllowed(_ => true)
                       .AllowAnyHeader()
                       .AllowCredentials();
            }));
            services.AddMvc();
            services.AddControllers();
            services.AddControllers().AddXmlSerializerFormatters();
            services.AddSingleton<MovieContext>((container) =>
            {
                var logger = container.GetRequiredService<ILogger<MovieContext>>();
                var hubContext = container.GetRequiredService<IHubContext<Hubs.MovieHub>>();
                return new MovieContext(Configuration.GetConnectionString("DefaultConnection"),logger,hubContext);
            });
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthorization();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<Hubs.MovieHub>("/movieHub");
            });
            loggerFactory.AddProvider(new Models.fileLogProvider());
 
        }
    }
}
