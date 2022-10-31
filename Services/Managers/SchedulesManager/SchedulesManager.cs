using Entities.Interfaces;
using Microsoft.Extensions.Configuration;
using Services.Services.StepStrategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Managers.SchedulesManager
{
    public class SchedulesManager : ISchedulesManager
    {
        private readonly IConfiguration configuration;
        private readonly IStepStrategy stepStrategy;

        public SchedulesManager(IConfiguration configuration, IStepStrategy stepStrategy)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.stepStrategy = stepStrategy ?? throw new ArgumentNullException(nameof(stepStrategy));
        }

        public async Task RunSchedules()
        {
            var section = configuration.GetSection("Schedules");
            if (!section.Exists())
            {
                Console.WriteLine("Section Schedules do not exist.");
                return;
            }

            IEnumerable<string>? schedules = section.Get<IEnumerable<string>>();
            if (schedules is null || !schedules.Any())
            {
                Console.WriteLine("I did not find any schedule to run.");
                return;
            }

            foreach (string schedule in schedules)
            {
                Console.WriteLine($"Running the following schedule: {schedule}");

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
