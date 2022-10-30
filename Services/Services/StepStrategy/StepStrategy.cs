using Entities.Interfaces;
using Services.Managers.FilesMergerManager;
using Services.Managers.HelpManager;

namespace Services.Services.StepStrategy
{
    public class StepStrategy : IStepStrategy
    {
        private readonly IFilesMergerManager filesMergerManager;
        private readonly IHelpManager helpManager;

        public StepStrategy(IFilesMergerManager filesMergerManager, IHelpManager helpManager)
        {
            this.filesMergerManager = filesMergerManager ?? throw new ArgumentNullException(nameof(filesMergerManager));
            this.helpManager = helpManager ?? throw new ArgumentNullException(nameof(helpManager));
        }

        public IStep GetStep(string command)
        {
            switch (command)
            {
                case "-merge":
                case "-m":
                    return filesMergerManager;
                case "-help":
                case "-h":
                    return helpManager;
                default:
                    throw new ArgumentException(nameof(command), "Command not recognized.");
            }
        }
    }
}
