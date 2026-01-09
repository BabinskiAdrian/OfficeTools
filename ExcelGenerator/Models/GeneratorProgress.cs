using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeTools.ExcelGenerator.Core.Models;

public class GeneratorProgress
{
    public int CurrentFileIndex { get; set; }
    public int TotalFiles { get; set; }
    public string Message { get; set; } = string.Empty;

    // Oblicza procent dla paska postępu (0-100)
    public int Percentage => TotalFiles == 0 
        ? 0 
        : (CurrentFileIndex * 100) / TotalFiles;

    public GeneratorProgress(int current, int total, string message)
    {
        CurrentFileIndex = current;
        TotalFiles = total;
        Message = message;
    }
}
