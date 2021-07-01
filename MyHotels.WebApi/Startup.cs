using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
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
                options.UseSqlServer(Configuration.GetConnectionString("MssqlConnection"));
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

            services.AddControllers()
                // Solves problem with cyclical dependency between countries and hotels.
                .AddNewtonsoftJson(options => {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyHotels.WebApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyHotels.WebApi v1"));
            }

            // Our own exception handler

            app.UseCustomExceptionHandler();

            app.UseHttpsRedirection();

            // CORS
            app.UseCorsPolicy();

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
