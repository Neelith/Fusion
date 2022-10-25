namespace Services.Interfaces
{
    public interface IStepRunnerStrategy
    {
        IStepRunner GetStepRunner(string command);
    }
}
