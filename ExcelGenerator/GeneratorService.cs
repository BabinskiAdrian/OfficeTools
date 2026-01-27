using ClosedXML.Excel;
using OfficeTools.ExcelGenerator.Core.Models;
using OfficeTools.Shared;
using System.Text.RegularExpressions;

namespace OfficeTools.ExcelGenerator.Core;

public class GeneratorService
{
    public async Task GenerateAsync(string baseFilePath, GeneratorConfig config, IProgress<GeneratorProgress>? progress = null, CancellationToken token = default)
    {
        if (!File.Exists(baseFilePath))
        {
            throw new FileNotFoundException("Base file not found.", baseFilePath);
        }

        await Task.Run(() => RunGenerationLoop(baseFilePath, config, progress, token), token);
    }

    private void RunGenerationLoop(string baseFilePath, GeneratorConfig config, IProgress<GeneratorProgress>? progress, CancellationToken token)
    {
        FileHelper.CreateNewDirectory(config.OutputDirectory);

        var fileNameParts = ParseFileName(Path.GetFileName(baseFilePath));
        var baseData = ReadBaseData(baseFilePath, config);

        var state = new GenerationState
        {
            CurrentNumber = fileNameParts.Number,
            Ip1 = baseData.BaseIp1,
            Ip2 = baseData.BaseIp2,
            CurrentDate = baseData.BaseDate
        };

        for (int i = 0; i < config.FilesCount; i++)
        {
            token.ThrowIfCancellationRequested();

            UpdateState(state, i, config);

            GenerateSingleFile(baseFilePath, config.OutputDirectory, fileNameParts, state);

            string fileName = BuildFileName(fileNameParts, state.CurrentNumber);
            progress?.Report(new GeneratorProgress(i + 1, config.FilesCount, $"Create: {fileName}"));
        }
    }

    internal static void UpdateState(GenerationState state, int iterationIndex, GeneratorConfig config)
    {
        state.CurrentNumber++;
        state.Ip1++;
        state.Ip2++;

        if (state.Ip1 >= 256 || state.Ip2 >= 256)
            throw new InvalidOperationException("IP octet out of range (255)");

        if (config.CarriersPerDay > 0 && iterationIndex > 0 && iterationIndex % config.CarriersPerDay == 0)
            state.CurrentDate = GetNextBusinessDay(state.CurrentDate);

        if (iterationIndex == 0 && IsWeekend(state.CurrentDate))
            state.CurrentDate = GetNextBusinessDay(state.CurrentDate);
    }

    private static void GenerateSingleFile(string sourcePath, string outputDir, (string Prefix, int Number, int Len, string Suffix) nameParts, GenerationState state)
    {
        string newFileName = BuildFileName(nameParts, state.CurrentNumber);
        string destPath = Path.Combine(outputDir, newFileName);

        try
        {
            File.Copy(sourcePath, destPath, overwrite: true);

            using var workbook = new XLWorkbook(destPath);
            // TODO: default worksheet, change to read from config
            var ws = workbook.Worksheet(1);

            // TODO: default cell, change to read form config
            ws.Cell(1, 1).Value = state.CurrentNumber.ToString().PadLeft(nameParts.Len, '0');
                         
            workbook.Save();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Nie udało się wygenerować pliku {newFileName}. Powód: {ex.Message}", ex);
        }
    }

    private static string BuildFileName((string Prefix, int Number, int Len, string Suffix) parts, int currentNumber)
    {
        return $"{parts.Prefix}{currentNumber.ToString().PadLeft(parts.Len, '0')}{parts.Suffix}";
    }

    internal (string Prefix, int Number, int NumberLength, string Suffix) ParseFileName(string fileName)
    {
        var regex = new Regex(@"^([A-Za-z]+)(\d+)(.*)$");
        var match = regex.Match(fileName);

        if (!match.Success)
            throw new FormatException($"File name '{fileName}' is invalid. Required format: LettersDigitsRest.");

        return (
            match.Groups[1].Value, 
            int.Parse(match.Groups[2].Value), 
            match.Groups[2].Value.Length, 
            match.Groups[3].Value);
    }

    internal (int BaseIp1, int BaseIp2, DateTime BaseDate) ReadBaseData(string filePath, GeneratorConfig config)
    {
        using var workbook = new XLWorkbook(filePath);
        var ws = workbook.Worksheet(config.WorksheetIndex);

        int GetInt(ExcelAddress excelCell)
        {
            var cell = ws.Cell(excelCell.Row, excelCell.Column);

            return cell.IsEmpty() 
                ? 0 
                : cell.GetValue<int>();
        }
     
        int ip1 = GetInt(config.DeviceIp1Cell);
        int ip2 = GetInt(config.DeviceIp2Cell);

        DateTime date = DateTime.Today;
        var dateCell = ws.Cell(config.DateCell.Row, config.DateCell.Column);

        if (!dateCell.IsEmpty() && dateCell.TryGetValue(out DateTime fileDate))
        {
            date = fileDate;
        }

        return (ip1, ip2, date);
    }

    internal static DateTime GetNextBusinessDay(DateTime date)
    {
        do
        {
            date = date.AddDays(1);
        }
        while (IsWeekend(date));

        return date;
    }

    private static bool IsWeekend(DateTime date) => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
}