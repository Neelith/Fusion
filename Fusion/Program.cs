﻿using Entities.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Managers.FilesMergerManager;
using Services.Managers.HelpManager;
using Services.Managers.HouseKeepingManager;
using Services.Services.StepStrategy;
using System.Diagnostics;

namespace Fusion;

internal class Program
{
    private static IHost Setup(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostingContext, configuration) =>
                    {
                        configuration.Sources.Clear();

                        //IHostEnvironment env = hostingContext.HostingEnvironment;

                        configuration
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", false, true);
                    })
                    .ConfigureServices(builder => {
                        builder
                        .AddTransient<IStepStrategy, StepStrategy>()
                        .AddTransient<IFilesMergerManager, FilesMergerManager>()
                        .AddTransient<IHelpManager, HelpManager>()
                        .AddTransient<IHouseKeepingManager, HouseKeepingManager>();
                    })
                    .Build();
        return host;
    }

    static async Task Main(string[] args)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        IHost host = Setup(args);

        try
        {
            await host.StartAsync();

            IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
            var appVersion = config.GetValue<string>("AppVersion");
            Console.WriteLine($"Fusion version {appVersion} is running...");

            string command = args.Length == 0 || string.IsNullOrWhiteSpace(args[0])
                ? "-h"
                : args[0];
            IStepStrategy stepStrategy = host.Services.GetRequiredService<IStepStrategy>();
            IStep step = stepStrategy.GetStep(command);
            await step.RunAsync(args);

            stopwatch.Stop();
            Console.WriteLine($"\nDone.\nTime elapsed: {stopwatch.Elapsed}");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"\nFailed.\nTime elapsed: {stopwatch.Elapsed}.\nException message: {ex.Message}");
        }
        finally
        {
            await host.StopAsync();
        }
    }
}