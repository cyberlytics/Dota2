using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data.Repositories;
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
            // setup database connection configuration
            services.Configure<MatchstoreDatabaseSettings>(
                Configuration.GetSection(nameof(MatchstoreDatabaseSettings)));
            services.AddSingleton<IMatchstoreDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<MatchstoreDatabaseSettings>>().Value);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Backend.API", Version = "v1"});
            });

            //Dependency Injections
            services.AddScoped<IMatchesRepository, MatchesRepository>();
            services.AddScoped<IMatchesService, MatchesService>();
            services.AddScoped<IOpenDotaService, OpenDotaService>();
            services.AddScoped<IMatchService, MatchService>();

            // TODO Start
            // Nach https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-5.0&tabs=visual-studio#add-a-crud-operations-service
            // soll die MongoDB als Singleton initialisiert werden.
            // Das führt aber bei mir zu einem Fehler ... vll. weiß ja jemand aus der Zukunft eine Lösung dazu.
            //
            //services.AddScoped<IMatchRepository, MatchRepository>();
            //services.AddSingleton<MatchRepository>();
            //services.AddSingleton<IMatchRepository>(MatchRepository);
            services.AddScoped<IMatchRepository, MatchRepository>();
            // TODO End
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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