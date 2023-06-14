using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Backend.Domain.Models;
using Backend.Domain.Repositories;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Backend.API
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
            // Initialisiert die Konfiguration zur Verbindung zur Datenbank
            services.Configure<MatchstoreDatabaseSettings>(
                Configuration.GetSection(nameof(MatchstoreDatabaseSettings)));
            services.AddSingleton<IMatchstoreDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<MatchstoreDatabaseSettings>>().Value);

            // Aktivierung von Cross-Origin Requests
            services.AddCors();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend.API", Version = "v1" });
                
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // Dependency Injections
            services.AddScoped<IJupyterService, JupyterService>();
            services.AddScoped<IOpenDotaService, OpenDotaService>();
            services.AddScoped<IOpenDotaCallerService, OpenDotaCallerService>();

            services.AddSingleton<IMatchRepository, MatchRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Konfiguration der Cross-Origin Requests
            // Sehr einfache Konfiguration! - Es wird einfach alles erlaubt.
            app.UseCors(builder => builder
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .SetIsOriginAllowed((host) => true)
                     .AllowCredentials()
                 );

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}