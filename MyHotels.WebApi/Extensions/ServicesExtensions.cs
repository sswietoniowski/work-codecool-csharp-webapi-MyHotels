using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MyHotels.WebApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHotels.WebApi.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
        }

        public static void ConfigureAuthenticationAndIdentityManagement(this IServiceCollection services)
        {

        }

        public static void UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        Log.Error($"Something went wrong in {contextFeature.Error}");
                        await context.Response.WriteAsync(new Error { StatusCode = context.Response.StatusCode, Message = "Internal server error, please try again later..." }.ToString());
                    }
                });
            });
        }

        public static void UseCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors("AllowAll");
        }
    }
}
