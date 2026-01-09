namespace OfficeTools.ExcelGenerator.Core.Models;

public record ExcelAddress(int Row, int Column);

public class GeneratorConfig
{
    public string OutputDirectory { get; set; } = string.Empty;
    public int FilesCount { get; set; } = 0;
    public int CarriersPerDay { get; set; } = 1;

    public int WorksheetIndex { get; set; } = 1;

    public ExcelAddress DeviceNumberCell { get; set; } = new(3, 6);
    public ExcelAddress DeviceIp1Cell { get; set; } = new(11, 7);
    public ExcelAddress DeviceIp2Cell { get; set; } = new(13, 7);
    public ExcelAddress MacAddressCell { get; set; } = new(16, 6);
    public ExcelAddress DateCell { get; set; } = new(23, 6);
}
