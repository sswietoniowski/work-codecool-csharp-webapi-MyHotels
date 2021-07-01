using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MyHotels.WebApi.Data;
using MyHotels.WebApi.Extensions;
using MyHotels.WebApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyHotels.WebApi
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
            // DbContext
            services.AddDbContext<MyHotelsDbContext>(options => 
            {
                //options.UseSqlServer(Configuration.GetConnectionString("MssqlConnection"));
                options.UseSqlite(Configuration.GetConnectionString("SqliteConnection"));
            });

            // UnitOfWork
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            // Automapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // CORS - create policy
            services.ConfigureCorsPolicy();

            // Authentication & Identity Management
            services.ConfigureAuthenticationAndIdentityManagement();

            // JWT
            services.ConfigureJwt(Configuration);

            // Versioning
            services.ConfigureVersioning();

            // Throttling
            services.ConfigureRateLimiting();

            // Caching
            services.ConfigureCaching();

            services.AddControllers(config => 
            {
                // Defines named caching profile.
                config.CacheProfiles.Add("120SecondsDuration", new CacheProfile { Duration = 120 });
            })
            // Solves problem with cyclical dependency between countries and hotels.
            .AddNewtonsoftJson(options => {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.ConfigureSwaggerWithVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, MyHotelsDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // special configuration for Swagger if we're using versioning
                app.UseSwaggerWithVersioning(provider);

                // Just as an example
                context.Database.EnsureCreated();
            }

            // Our own exception handler

            app.UseCustomExceptionHandler();

            app.UseHttpsRedirection();

            // CORS
            app.UseCorsPolicy();

            // Caching
            app.UseCaching();

            // Throttling
            app.UseRateLimiting();

            app.UseRouting();

            // Authentication & Authorization
            app.UseAuthenticationAndAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
