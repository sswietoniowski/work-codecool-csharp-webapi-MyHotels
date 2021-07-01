using AspNetCoreRateLimit;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MyHotels.WebApi.Data;
using MyHotels.WebApi.Domain;
using MyHotels.WebApi.Infrastructure;
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
            services.AddAuthentication();
            var builder = services.AddIdentityCore<ApiUser>(q => q.User.RequireUniqueEmail = true);
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddEntityFrameworkStores<MyHotelsDbContext>().AddDefaultTokenProviders();
        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Environment.GetEnvironmentVariable("MY_HOTELS_KEY");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                    ValidAudience = jwtSettings.GetSection("Audience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key))
                };
            });

            services.AddScoped<IAuthenticationManager, AuthenticationManager>();
        }

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 1);
                //options.ApiVersionReader = new QueryStringApiVersionReader("v", "ver", "version", "api-version");
                //options.ApiVersionReader = new HeaderApiVersionReader("X-Version");
                //options.ApiVersionReader = new UrlSegmentApiVersionReader();
                // not recommended to use all at the same time, just for demonstration purposes
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Version"),
                    new QueryStringApiVersionReader("v", "ver", "version", "api-version")); 
            });
        }

        private static ApiVersion ApiVersion(int v1, int v2)
        {
            throw new NotImplementedException();
        }

        public static void ConfigureRateLimiting(this IServiceCollection services)
        {
            services.AddMemoryCache();

            var rateLimitRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*",
                    Limit = 1,
                    Period = "5s"
                }
            };

            services.Configure<IpRateLimitOptions>(options =>
            {
                options.GeneralRules = rateLimitRules;
            });

            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddHttpContextAccessor();
        }

        public static void ConfigureCaching(this IServiceCollection services)
        {
            services.AddResponseCaching();
            //services.AddHttpCacheHeaders(
            //    (expirationOptions) =>
            //    {
            //        expirationOptions.MaxAge = 120;
            //        expirationOptions.CacheLocation = CacheLocation.Private;
            //    },
            //    (validationOptions) =>
            //    {
            //        validationOptions.MustRevalidate = true;
            //    });
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

        public static void UseAuthenticationAndAuthorization(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        public static void UseRateLimiting(this IApplicationBuilder app)
        {
            app.UseIpRateLimiting();
        }

        public static void UseCaching(this IApplicationBuilder app)
        {
            app.UseResponseCaching();
            //app.UseHttpCacheHeaders();
        }
    }
}
