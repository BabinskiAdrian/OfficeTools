using ClosedXML.Excel;
using OfficeTools.ExcelGenerator.Core;
using OfficeTools.ExcelGenerator.Core.Models;
using Xunit;

namespace OfficeTools.ExcelGenerator.Tests
{
    public class GeneratorIntegrationTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _baseFilePath;
        private readonly GeneratorConfig _config;

        public GeneratorIntegrationTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            _baseFilePath = Path.Combine(_testDirectory, "CA010_Base.xlsx");
            CreateDummyExcelFile(_baseFilePath);

            _config = new GeneratorConfig
            {
                OutputDirectory = Path.Combine(_testDirectory, "Output"),
                FilesCount = 3,
                CarriersPerDay = 1,
                WorksheetIndex = 1,
                DeviceNumberCell = new(1, 1),
                DeviceIp1Cell = new(1, 2),
                DeviceIp2Cell = new(1, 3),
                DateCell = new(1, 4)
            };
        }

        [Fact]
        public async Task Generate_ShouldCreateFiles_WithCorrectValues()
        {
            var service = new GeneratorService();

            await service.GenerateAsync(_baseFilePath, _config);

            string expectedFile1 = Path.Combine(_config.OutputDirectory, "CA011_Base.xlsx");
            string expectedFile3 = Path.Combine(_config.OutputDirectory, "CA013_Base.xlsx");

            Assert.True(File.Exists(expectedFile1), "Plik 1 nie powstał");
            Assert.True(File.Exists(expectedFile3), "Plik 3 nie powstał");

            using (var workbook = new XLWorkbook(expectedFile1))
            {
                var ws = workbook.Worksheet(1);
                Assert.Equal("011", ws.Cell(1, 1).Value.ToString());
                Assert.Equal(193, ws.Cell(1, 2).GetValue<int>());
                Assert.False(ws.Cell(1, 4).IsEmpty());
            }
        }

        private void CreateDummyExcelFile(string path)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Sheet1");

            ws.Cell(1, 1).Value = "010";
            ws.Cell(1, 2).Value = 192;
            ws.Cell(1, 3).Value = 10;
            ws.Cell(1, 4).Value = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Local);

            wb.SaveAs(path);
        }

        public void Dispose()
        {
            if (!Directory.Exists(_testDirectory)) return;

            try
            {
                Directory.Delete(_testDirectory, true);
            }
            catch (IOException)
            {
                Thread.Sleep(100);
                try
                {
                    Directory.Delete(_testDirectory, true);
                }
                catch
                {
                    // Didn't work, well
                }
            }
            catch (Exception)
            {
                // Didn't work, well
            }
        }
    }
}