using Entities.Interfaces;

namespace Services.Managers.HelpManager
{
    public interface IHelpManager : IStep
    {
        Task PrintHelpScreen(string text);
    }
}
