using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace MyHotels.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.File(
            path: @"logs\log-.txt",
            outputTemplate: "{Timestamp:yyyy-mm-dd HH:mm:ss.fff zzz} {Level:u3} {Message:lj}{NewLine}{Exception}",
            rollingInterval: RollingInterval.Day,
            restrictedToMinimumLevel: LogEventLevel.Information
        ).CreateLogger();

        try
        {
            Log.Information("Application is starting...");
            CreateHostBuilder(args).Build().Run();
            Log.Information("Application is closing...");
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Application failed to start!");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}