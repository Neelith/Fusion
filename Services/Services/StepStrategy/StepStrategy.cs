using Entities.Interfaces;
using Services.Managers.FilesMergerManager;
using Services.Managers.HelpManager;
using Services.Managers.HouseKeepingManager;

namespace Services.Services.StepStrategy
{
    public class StepStrategy : IStepStrategy
    {
        private readonly IFilesMergerManager filesMergerManager;
        private readonly IHelpManager helpManager;
        private readonly IHouseKeepingManager houseKeepingManager;

        public StepStrategy(
            IFilesMergerManager filesMergerManager, 
            IHelpManager helpManager, 
            IHouseKeepingManager houseKeepingManager)
        {
            this.filesMergerManager = filesMergerManager ?? throw new ArgumentNullException(nameof(filesMergerManager));
            this.helpManager = helpManager ?? throw new ArgumentNullException(nameof(helpManager));
            this.houseKeepingManager = houseKeepingManager ?? throw new ArgumentNullException(nameof(houseKeepingManager));
        }

        public IStep GetStep(string command)
        {
            switch (command)
            {
                case "-merge":
                case "-m":
                    return filesMergerManager;
                case "-delete":
                case "-d":
                    return houseKeepingManager;
                case "-help":
                case "-h":
                    return helpManager;
                default:
                    throw new ArgumentException(nameof(command), "Command not recognized.");
            }
        }
    }
}
