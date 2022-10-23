using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Diagnostics;

namespace Fusion;

internal class Program
{
    static async Task Main(string[] args)
    {
        IHost host = Setup(args);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

        var appVersion = config.GetValue<string>("AppVersion");
        Console.WriteLine($"Fusion version {appVersion} is running...");

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

                        IHostEnvironment env = hostingContext.HostingEnvironment;

                        configuration
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", false, true);
                    })
                    .Build();
        return host;
    }
}