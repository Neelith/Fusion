namespace Entities.Interfaces
{
    public interface IStep
    {
        Task RunAsync(string[] args);
    }
}
