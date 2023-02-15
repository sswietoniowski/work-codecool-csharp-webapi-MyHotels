using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyHotels.WebApi.Data;
using MyHotels.WebApi.Domain;
using MyHotels.WebApi.Infrastructure;
using MyHotels.WebApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace MyHotels.WebApi.Extensions;

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

    public static void ConfigureSwaggerWithVersioning(this IServiceCollection services)
    {
        // based on this blog post: https://referbruv.com/blog/posts/integrating-aspnet-core-api-versions-with-swagger-ui

        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerOptions>();

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

    public static void UseSwaggerWithVersioning(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // handling swagger access while published to IIS or Azure
            string swaggerJsonBasePath = string.IsNullOrWhiteSpace(options.RoutePrefix) ? "." : "..";

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"{swaggerJsonBasePath}/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });
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

class ConfigureSwaggerOptions
    : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    public ConfigureSwaggerOptions(
        IApiVersionDescriptionProvider provider)
    {
        this.provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // add swagger document for every API version discovered
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                CreateVersionInfo(description));
        }
    }

    public void Configure(string name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    private OpenApiInfo CreateVersionInfo(
        ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "MyHotels.WebApi",
            Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}