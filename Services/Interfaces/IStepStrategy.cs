namespace Services.Interfaces
{
    public interface IStepStrategy
    {
        IStep GetStep(string command);
    }
}
