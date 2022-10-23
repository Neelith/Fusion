using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IFilesMergerManager
    {
        Task MergeFiles(string inputFilesFolderPath, string? outputFilePath = null);
    }
}
