using Entities.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Services.StepStrategy;

namespace Services.Managers.SchedulesManager
{
    public class SchedulesManager : ISchedulesManager
    {
        private readonly IConfiguration configuration;
        private readonly IStepStrategy stepStrategy;
        private readonly ILogger<SchedulesManager> logger;

        public SchedulesManager(
            IConfiguration configuration, 
            IStepStrategy stepStrategy, 
            ILogger<SchedulesManager> logger)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.stepStrategy = stepStrategy ?? throw new ArgumentNullException(nameof(stepStrategy));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RunSchedules()
        {
            var section = configuration.GetSection("Schedules");
            if (!section.Exists())
            {
                logger.LogInformation("Section Schedules do not exist.");
                return;
            }

            IEnumerable<string>? schedules = section.Get<IEnumerable<string>>();
            if (schedules is null || !schedules.Any())
            {
                logger.LogInformation("I did not find any schedule to run.");
                return;
            }

            foreach (string schedule in schedules)
            {
                logger.LogInformation($"Running the following schedule: {schedule}");

                string[] args = schedule.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                string command = args.Length == 0 || string.IsNullOrWhiteSpace(args[0])
                ? throw new ArgumentException("Command was not found.", nameof(command))
                : args[0];

                IStep step = stepStrategy.GetStep(command);
                await step.RunAsync(args);
            }
        }

        public async Task RunAsync(string[] args)
        {
            await RunSchedules();
        }
    }
}
