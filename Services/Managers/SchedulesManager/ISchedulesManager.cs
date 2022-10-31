using Entities.Interfaces;

namespace Services.Managers.SchedulesManager
{
    public interface ISchedulesManager : IStep
    {
        Task RunSchedules();
    }
}
