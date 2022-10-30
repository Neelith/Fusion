using Entities.Interfaces;

namespace Services.Managers.HouseKeepingManager
{
    public interface IHouseKeepingManager : IStep
    {
        void DeleteAll(string inputFilesFolderPath, uint? retentionDays = null);
    }
}
