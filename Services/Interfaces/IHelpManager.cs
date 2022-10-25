namespace Services.Interfaces
{
    public interface IHelpManager : IStepRunner
    {
        Task PrintHelpScreen(string text);
    }
}
