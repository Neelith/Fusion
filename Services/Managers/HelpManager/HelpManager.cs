using Microsoft.Extensions.Configuration;

namespace Services.Managers.HelpManager
{
    public class HelpManager : IHelpManager
    {
        private readonly IConfiguration configuration;

        public HelpManager(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Task PrintHelpScreen(string text)
        {
            Console.WriteLine(text);
            return Task.CompletedTask;
        }

        public Task RunAsync(string[] args)
        {
            string helpText = configuration["HelpText"];
            PrintHelpScreen(helpText);
            return Task.CompletedTask;
        }
    }
}
