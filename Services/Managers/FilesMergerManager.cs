using Entities.Entities;
using Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Managers;

public class FilesMergerManager : IFilesMergerManager, IStepRunner
{
    public async Task MergeFiles(string inputFilesFolderPath, string? outputFilePath = null)
    {
        if (string.IsNullOrWhiteSpace(inputFilesFolderPath) || !Directory.Exists(inputFilesFolderPath))
            throw new ArgumentException($"'{nameof(inputFilesFolderPath)}' cannot be null or empty and directory must exist.", nameof(inputFilesFolderPath));
        
        string[] filePaths = Directory.GetFiles(inputFilesFolderPath);
        var bag = new ConcurrentBag<FileRecord>();
        await Parallel.ForEachAsync(filePaths, async (filePath, cancellationToken)  => {
            string text = await File.ReadAllTextAsync(filePath);
            bag.Add(new FileRecord(filePath, text));
            Console.WriteLine($"Processed: {filePath}.");
        });

        Console.WriteLine($"Processed {bag.Count()} files from {inputFilesFolderPath}.");

        var records = bag.OrderBy(record => record.Path);
        string outputFileText = records
            .Select(record => record.Text)
            .Aggregate((prev, actual) => 
            { 
                return $"{prev}{Environment.NewLine}{actual}"; 
            });

        if (!string.IsNullOrWhiteSpace(outputFilePath) && !Directory.Exists(outputFilePath))
        {
            string outputFolderPath = Path.GetDirectoryName(outputFilePath);
            Directory.CreateDirectory(outputFolderPath);
        }

        string outputFilePathName = !string.IsNullOrWhiteSpace(outputFilePath)
            ? $"{outputFilePath}"
            : $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}fusionOutput.txt";

        await File.WriteAllTextAsync(outputFilePathName, outputFileText);
        Console.WriteLine($"Output file is {outputFilePathName}.");
    }

    public async Task RunAsync(IDictionary<string, string> stepArgs)
    {
        if (stepArgs is null || stepArgs.Count == 0) throw new ArgumentNullException(nameof(stepArgs));

        bool isInputFilesFolderPathValid = stepArgs.TryGetValue("inputFilesFolderPath", out string? inputFilesFolderPath);
        if (!isInputFilesFolderPathValid)
        {
            throw new ArgumentException($"{nameof(inputFilesFolderPath)} it's not valid.", nameof(inputFilesFolderPath));
        }

        bool isOutputFilePathValid = stepArgs.TryGetValue("outputFilePath", out string? outputFilePath);

        if(isInputFilesFolderPathValid && isOutputFilePathValid)
        {
            await MergeFiles(inputFilesFolderPath, outputFilePath);
            return;
        }

        if (isInputFilesFolderPathValid)
        {
            await MergeFiles(inputFilesFolderPath: inputFilesFolderPath);
            return;
        }

        throw new ApplicationException($"{nameof(FilesMergerManager)} - Application was not able to run ${MergeFiles}");
    }
}
