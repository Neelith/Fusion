using Entities.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Services.Managers.FilesMergerManager;
using Services.Managers.HelpManager;
using Services.Managers.HouseKeepingManager;
using Services.Managers.SchedulesManager;
using Services.Services.StepStrategy;
using System.Diagnostics;

namespace Fusion;

internal class Program
{
    private static IHost Setup(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .WriteTo.Console()
                            .WriteTo.File(@$"logs{Path.DirectorySeparatorChar}log-.txt", rollingInterval: RollingInterval.Minute)
                            .CreateBootstrapLogger();

        IHost host = Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostingContext, configuration) =>
                    {
                        configuration.Sources.Clear();
                        configuration
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddJsonFile("schedules.json", optional: true, reloadOnChange: true);
                    })
                    .ConfigureServices(builder => {
                        builder
                        .AddTransient<IStepStrategy, StepStrategy>()
                        .AddTransient<IFilesMergerManager, FilesMergerManager>()
                        .AddTransient<IHelpManager, HelpManager>()
                        .AddTransient<IHouseKeepingManager, HouseKeepingManager>()
                        .AddTransient<ISchedulesManager, SchedulesManager>();
                    })
                    .UseSerilog()
                    .Build();
        return host;
    }

    static async Task Main(string[] args)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        IHost host = Setup(args);
        await host.StartAsync();
        ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();

        try
        {
            IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
            var appVersion = config.GetValue<string>("AppVersion");
            logger.LogInformation($"Fusion version {appVersion} is running...");

            string command = args.Length == 0 || string.IsNullOrWhiteSpace(args[0])
                ? "-h"
                : args[0];
            if (command == "-s" || command == "-schedules")
            {
                var schedulesManager = host.Services.GetRequiredService<ISchedulesManager>();
                await schedulesManager.RunAsync(args);
            }
            else
            {
                IStepStrategy stepStrategy = host.Services.GetRequiredService<IStepStrategy>();
                IStep step = stepStrategy.GetStep(command);
                await step.RunAsync(args);
            }
            
            stopwatch.Stop();
            logger.LogInformation($"Done.\nTime elapsed: {stopwatch.Elapsed}");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogInformation($"Failed.\nTime elapsed: {stopwatch.Elapsed}.\nException message: {ex.Message}");
        }
        finally
        {
            await host.StopAsync();
        }
    }
}