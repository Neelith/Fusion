namespace Services.Interfaces
{
    public interface IStep
    {
        Task RunAsync(string[] args);
    }
}
