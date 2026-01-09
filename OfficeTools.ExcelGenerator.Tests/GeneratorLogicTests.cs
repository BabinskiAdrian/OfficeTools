using OfficeTools.ExcelGenerator.Core;
using Xunit;

namespace OfficeTools.ExcelGenerator.Tests;

public class GeneratorLogicTests
{
    private readonly GeneratorService _service;

    public GeneratorLogicTests()
    {
        _service = new GeneratorService();
    }


    [Theory] // Theory pozwala uruchomić ten sam test dla wielu danych
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

    [Fact] // Fact to pojedynczy test
    public void ParseFileName_ShouldThrowException_WhenFormatIsInvalid()
    {
        // Arrange
        string badFileName = "RaportBezNumeru.xlsx";

        // Act & Assert
        Assert.Throws<FormatException>(() => _service.ParseFileName(badFileName));
    }

    // --- TESTOWANIE DAT (GetNextBusinessDay) ---

    [Fact]
    public void GetNextBusinessDay_ShouldSkipWeekend_WhenFriday()
    {
        // Arrange
        var friday = new DateTime(2023, 10, 27); // Piątek

        // Act
        var nextDay = _service.GetNextBusinessDay(friday);

        // Assert
        var monday = new DateTime(2023, 10, 30);
        Assert.Equal(monday, nextDay);
    }

    [Fact]
    public void GetNextBusinessDay_ShouldReturnTuesday_WhenMonday()
    {
        // Arrange
        var monday = new DateTime(2023, 10, 30);

        // Act
        var nextDay = _service.GetNextBusinessDay(monday);

        // Assert
        var tuesday = new DateTime(2023, 10, 31);
        Assert.Equal(tuesday, nextDay);
    }
}