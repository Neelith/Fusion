using Entities.Entities;
using Microsoft.Extensions.Configuration;

namespace Services.Managers.HouseKeepingManager
{
    public class HouseKeepingManager : IHouseKeepingManager
    {
        private readonly IConfiguration configuration;

        public HouseKeepingManager(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void DeleteAll(string inputFilesFolderPath, uint? retentionDays = null)
        {
            if (string.IsNullOrWhiteSpace(inputFilesFolderPath)) throw new ArgumentException($"'{nameof(inputFilesFolderPath)}' cannot be null or whitespace.", nameof(inputFilesFolderPath));
            
            string fileDateTimeCancellationCriteria = configuration["DateTimeCancellationCriteria"];
            if (string.IsNullOrWhiteSpace(fileDateTimeCancellationCriteria))
            {
                fileDateTimeCancellationCriteria = "ByLastWriteTime";
            }

            string[] filePaths = Directory.GetFiles(inputFilesFolderPath);
            Parallel.ForEach(filePaths, filePath =>
            {
                var file = new FileRecord(filePath);

                if (!retentionDays.HasValue)
                {
                    file.Delete();
                    Console.WriteLine($"Deleted: {filePath}.");
                    return;
                }

                DateTime lastTimeUsed = GetFileLastTimeUsed(file, fileDateTimeCancellationCriteria);
                double daysSinceLastTimeUsed = DateTime.UtcNow.Subtract(lastTimeUsed).TotalDays;
                if (daysSinceLastTimeUsed >= retentionDays)
                {
                    file.Delete();
                    Console.WriteLine($"Deleted: {filePath}.");
                }
            });
        }

        private DateTime GetFileLastTimeUsed(FileRecord file, string fileDateTimeCancellationCriteria)
        {
            switch (fileDateTimeCancellationCriteria)
            {
                case "ByLastAccessTime":
                    return file.FileInfo.LastAccessTimeUtc;
                case "ByLastWriteTime":
                    return file.FileInfo.LastWriteTimeUtc;
                case "ByCreationTime":
                    return file.FileInfo.CreationTimeUtc;
                default:
                    throw new ArgumentException($"{fileDateTimeCancellationCriteria} was not valid. Accepted values are: ByLastAccessTime, ByLastWriteTime, ByCreationTime.", nameof(fileDateTimeCancellationCriteria));
            }
        }

        public Task RunAsync(string[] args)
        {
            string inputFilesFolderPath = args.Length < 2 || string.IsNullOrWhiteSpace(args[1])
            ? throw new ArgumentException("Input folder", "delete command requires at least an input folder param.")
            : args[1];

            string? retentionDaysStr = args.Length < 3 || string.IsNullOrWhiteSpace(args[2])
                ? null
                : args[2];

            bool areRetentionDaysParsed = uint.TryParse(retentionDaysStr, out uint retentionDays);
            if (areRetentionDaysParsed)
            {
                DeleteAll(inputFilesFolderPath, retentionDays);
                return Task.CompletedTask;
            }

            DeleteAll(inputFilesFolderPath);
            return Task.CompletedTask;
        }
    }
}
