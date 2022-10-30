using Entities.Interfaces;

namespace Services.Managers.FilesMergerManager
{
    public interface IFilesMergerManager : IStep
    {
        Task MergeFiles(string inputFilesFolderPath, string? outputFilePath = null);
    }
}
