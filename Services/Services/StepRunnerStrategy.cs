using Services.Interfaces;

namespace Services.Services
{
    public class StepRunnerStrategy : IStepRunnerStrategy
    {
        private readonly IFilesMergerManager filesMergerManager;
        private readonly IHelpManager helpManager;

        public StepRunnerStrategy(IFilesMergerManager filesMergerManager, IHelpManager helpManager)
        {
            this.filesMergerManager = filesMergerManager ?? throw new ArgumentNullException(nameof(filesMergerManager));
            this.helpManager = helpManager ?? throw new ArgumentNullException(nameof(helpManager));
        }

        public IStepRunner GetStepRunner(string command)
        {
            switch (command)
            {
                case "merge":
                case "m":
                    return filesMergerManager;

                case "help":
                case "h":
                case "default":
                    return helpManager;
                default:
                    throw new ArgumentException(nameof(command), "Command not recognized.");
            }
        }
    }
}
