using OfficeTools.ExcelGenerator.Core;
using Xunit;

namespace OfficeTools.ExcelGenerator.Tests
{
    public class GeneratorLogicTests
    {
        private readonly GeneratorService _service;

        public GeneratorLogicTests()
        {
            _service = new GeneratorService();
        }

        [Theory]
        [InlineData("CA050_Raport.xlsx", "CA", 50, "_Raport.xlsx")]
        [InlineData("Test001.xlsx", "Test", 1, ".xlsx")]
        [InlineData("Plik999-Kopia.xls", "Plik", 999, "-Kopia.xls")]
        public void ParseFileName_ShouldCorrectlySplitName(string fileName, string expectedPrefix, int expectedNum, string expectedSuffix)
        {
            var result = _service.ParseFileName(fileName);

            Assert.Equal(expectedPrefix, result.Prefix);
            Assert.Equal(expectedNum, result.Number);
            Assert.Equal(expectedSuffix, result.Suffix);
        }

        [Fact]
        public void ParseFileName_ShouldThrowException_WhenFormatIsInvalid()
        {
            string badFileName = "RaportBezNumeru.xlsx";

            Assert.Throws<FormatException>(() => _service.ParseFileName(badFileName));
        }

        [Fact]
        public void GetNextBusinessDay_ShouldSkipWeekend_WhenFriday()
        {
            var friday = new DateTime(2023, 10, 27, 0, 0, 0, DateTimeKind.Local);

            var nextDay = _service.GetNextBusinessDay(friday);

            var monday = new DateTime(2023, 10, 30, 0, 0, 0, DateTimeKind.Local);
            Assert.Equal(monday, nextDay);
        }

        [Fact]
        public void GetNextBusinessDay_ShouldReturnTuesday_WhenMonday()
        {
            // Arrange
            var monday = new DateTime(2023, 10, 30, 0, 0, 0, DateTimeKind.Local);

            // Act
            var nextDay = _service.GetNextBusinessDay(monday);

            // Assert
            var tuesday = new DateTime(2023, 10, 31, 0, 0, 0, DateTimeKind.Local);
            Assert.Equal(tuesday, nextDay);
        }
    }
}