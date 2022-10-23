using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Interfaces;
using Services.Managers;
using System.Configuration;
using System.Diagnostics;

namespace Fusion;

internal class Program
{
    static async Task Main(string[] args)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        IHost host = Setup(args);
        IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

        var appVersion = config.GetValue<string>("AppVersion");
        Console.WriteLine($"Fusion version {appVersion} is running...");


        
        string command = args.Length == 0 || string.IsNullOrWhiteSpace(args[0])
            ? "default" 
            : args[0];

        switch (command)
        {
            case "merge":
            case "m":
                string inputFilesFolderPath = args.Length < 2 || string.IsNullOrWhiteSpace(args[1])
                    ? throw new ArgumentException("Input folder", "merge command requires at least an input folder param.") 
                    : args[1];

                string? outputFilePath = args.Length < 3 || string.IsNullOrWhiteSpace(args[2])
                    ? string.Empty
                    : args[2];

                var stepArgs = new Dictionary<string, string>();
                stepArgs.Add("inputFilesFolderPath", inputFilesFolderPath);
                stepArgs.Add("outputFilePath", outputFilePath);

                IStepRunner filesMergerManager = new FilesMergerManager();
                await filesMergerManager.RunAsync(stepArgs);
                break;

            case "help":
            case "h":
            case "default":
                string helpText = config.GetValue<string>("HelpText");
                Console.WriteLine($"{helpText}");
                break;
            default:
                throw new ArgumentException(nameof(command), "Command not recognized.");
        }

        stopwatch.Stop();
        Console.WriteLine($"Done.\nTime elapsed: {stopwatch.Elapsed}");

        await host.RunAsync();
    }

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
                    .Build();
        return host;
    }
}