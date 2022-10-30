using Entities.Entities;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace Services.Managers.FilesMergerManager;

public class FilesMergerManager : IFilesMergerManager
{
    private readonly IConfiguration configuration;

    public FilesMergerManager(IConfiguration configuration)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task MergeFiles(string inputFilesFolderPath, string? outputFilePath = null)
    {
        if (string.IsNullOrWhiteSpace(inputFilesFolderPath) || !Directory.Exists(inputFilesFolderPath))
            throw new ArgumentException($"'{nameof(inputFilesFolderPath)}' cannot be null or empty and directory must exist.", nameof(inputFilesFolderPath));

        string[] filePaths = Directory.GetFiles(inputFilesFolderPath);
        ConcurrentBag<FileRecord> bag = await ReadFilesAsync(filePaths);
        Console.WriteLine($"Processed {bag.Count()} files from {inputFilesFolderPath}.");

        string mergeFilesOrder = configuration["MergeFilesOrder"];
        string outputFileText = MergeTextFromFiles(bag, mergeFilesOrder);

        if (string.IsNullOrWhiteSpace(outputFilePath))
        {
            outputFilePath = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}fusionOutput.txt";
        }

        if (!Directory.Exists(outputFilePath))
        {
            string outputFolderPath = Path.GetDirectoryName(outputFilePath);
            Directory.CreateDirectory(outputFolderPath);
        }

        await File.WriteAllTextAsync(outputFilePath, outputFileText);
        Console.WriteLine($"Output file is {outputFilePath}.");
    }

    private async Task<ConcurrentBag<FileRecord>> ReadFilesAsync(string[] filePaths)
    {
        var bag = new ConcurrentBag<FileRecord>();
        await Parallel.ForEachAsync(filePaths, async (filePath, cancellationToken) =>
        {
            FileInfo fileInfo = new FileInfo(filePath);
            using StreamReader streamReader = fileInfo.OpenText();
            string text = await streamReader.ReadToEndAsync();
            bag.Add(new FileRecord(fileInfo, text));
            Console.WriteLine($"Processed: {filePath}.");
        });

        return bag;
    }

    private string MergeTextFromFiles(ConcurrentBag<FileRecord> bag, string mergeFilesOrder)
    {
        if (string.IsNullOrWhiteSpace(mergeFilesOrder))
        {
            mergeFilesOrder = "ByName";
        }

        Func<FileRecord, string> orderFunction = GetOrderFunction(mergeFilesOrder);

        return bag
            .OrderBy(orderFunction)
            .Select(record => record.Text)
            .Aggregate((prev, actual) =>
            {
                return $"{prev}{Environment.NewLine}{actual}";
            });
    }

    private Func<FileRecord, string> GetOrderFunction(string mergeFilesOrder)
    {
        switch (mergeFilesOrder)
        {
            case "ByName":
                return fileRecord => fileRecord.FileInfo.FullName;
            default:
                throw new ArgumentException("Order type not recognized.", nameof(mergeFilesOrder));
        }
    }

    public async Task RunAsync(string[] args)
    {
        string inputFilesFolderPath = args.Length < 2 || string.IsNullOrWhiteSpace(args[1])
            ? throw new ArgumentException("Input folder", "merge command requires at least an input folder param.")
            : args[1];

        string? outputFilePath = args.Length < 3 || string.IsNullOrWhiteSpace(args[2])
            ? string.Empty
            : args[2];

        bool isOutputFilePathValid = !string.IsNullOrWhiteSpace(outputFilePath);

        if (isOutputFilePathValid)
        {
            await MergeFiles(inputFilesFolderPath, outputFilePath);
            return;
        }

        await MergeFiles(inputFilesFolderPath);
    }
}
