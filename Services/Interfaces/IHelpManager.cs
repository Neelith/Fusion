namespace Services.Interfaces
{
    public interface IHelpManager : IStep
    {
        Task PrintHelpScreen(string text);
    }
}
