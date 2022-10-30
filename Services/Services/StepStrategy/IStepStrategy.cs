using Entities.Interfaces;

namespace Services.Services.StepStrategy
{
    public interface IStepStrategy
    {
        IStep GetStep(string command);
    }
}
