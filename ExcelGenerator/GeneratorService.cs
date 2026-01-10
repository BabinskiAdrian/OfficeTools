using ClosedXML.Excel; // Nowy using zamiast OfficeOpenXml
using OfficeTools.ExcelGenerator.Core.Models;
using OfficeTools.Shared;
using System.Text.RegularExpressions;

namespace OfficeTools.ExcelGenerator.Core
{
    public class GeneratorService
    {
        public async Task GenerateAsync(string baseFilePath, GeneratorConfig config, IProgress<GeneratorProgress>? progress = null, CancellationToken token = default)
        {
            if (!File.Exists(baseFilePath))
                throw new FileNotFoundException("Nie znaleziono pliku bazowego", baseFilePath);

            await Task.Run(() =>
            {
                FileHelper.CreateNewDirectory(config.OutputDirectory);

                var fileNameParts = ParseFileName(Path.GetFileName(baseFilePath));

                var baseData = ReadBaseData(baseFilePath, config);

                int currentFileNumber = fileNameParts.Number;
                int currentIpOctet1 = baseData.BaseIp1;
                int currentIpOctet2 = baseData.BaseIp2;
                DateTime currentDate = baseData.BaseDate;

                for (int i = 0; i < config.FilesCount; i++)
                {
                    currentFileNumber++;
                    currentIpOctet1++;
                    if (currentIpOctet1 >= 256)
                    {
                        throw new InvalidOperationException("Przekroczono zakres dla pierwszego oktetu IP (maksymalna wartość to 255).");
                    }
                    currentIpOctet2++;
                    if (currentIpOctet2 >= 256)
                    {
                        throw new InvalidOperationException("Przekroczono zakres dla drugiego oktetu IP (maksymalna wartość to 255).");
                    }

                    if (config.CarriersPerDay > 0 && i > 0 && i % config.CarriersPerDay == 0)
                    {
                        currentDate = GetNextBusinessDay(currentDate);
                    }

                    if (i == 0 && IsWeekend(currentDate))
                    {
                        currentDate = GetNextBusinessDay(currentDate);
                    }

                    string newNumberString = currentFileNumber.ToString().PadLeft(fileNameParts.NumberLength, '0');
                    string newFileName = $"{fileNameParts.Prefix}{newNumberString}{fileNameParts.Suffix}";
                    string newFilePath = Path.Combine(config.OutputDirectory, newFileName);

                    try
                    {
                        File.Copy(baseFilePath, newFilePath, overwrite: true);

                        using (var workbook = new XLWorkbook(newFilePath))
                        {
                            var ws = workbook.Worksheet(config.WorksheetIndex);

                            ws.Cell(config.DeviceNumberCell.Row, config.DeviceNumberCell.Column).Value = newNumberString;
                            ws.Cell(config.DeviceNumberCell.Row, config.DeviceNumberCell.Column).Style.NumberFormat.Format = "@";

                            ws.Cell(config.DeviceIp1Cell.Row, config.DeviceIp1Cell.Column).Value = currentIpOctet1;
                            ws.Cell(config.DeviceIp2Cell.Row, config.DeviceIp2Cell.Column).Value = currentIpOctet2;

                            ws.Cell(config.DateCell.Row, config.DateCell.Column).Value = currentDate;

                            workbook.Save();
                        }

                        progress?.Report(new GeneratorProgress(i + 1, config.FilesCount, $"Utworzono: {newFileName}"));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Błąd przy generowaniu pliku {newFileName}: {ex.Message}", ex);
                    }
                }
            }, token);
        }

        internal (string Prefix, int Number, int NumberLength, string Suffix) ParseFileName(string fileName)
        {
            var regex = new Regex(@"^([A-Za-z]+)(\d+)(.*)$");
            var match = regex.Match(fileName);

            if (!match.Success)
                throw new FormatException($"Nazwa pliku '{fileName}' jest niepoprawna. Wymagany format: LiteryCyfryReszta.");

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
                if (ws.Cell(excelCell.Row, excelCell.Column).IsEmpty()) return 0;

                return ws.Cell(excelCell.Row, excelCell.Column).GetValue<int>();
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

        internal DateTime GetNextBusinessDay(DateTime date)
        {
            do
            {
                date = date.AddDays(1);
            }
            while (IsWeekend(date));

            return date;
        }

        private bool IsWeekend(DateTime date) => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }
}