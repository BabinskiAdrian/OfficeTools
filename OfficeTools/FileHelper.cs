namespace OfficeTools.Shared;

public static class FileHelper
{
    public static string[] FindFileWithExtension(string directoryPath, string extension)
    {
        ArgumentException.ThrowIfNullOrEmpty(directoryPath);
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
        }

        ArgumentException.ThrowIfNullOrEmpty(extension);
        if (!extension.StartsWith('.'))
        {
            throw new ArgumentException("Extension must start with a dot", nameof(extension));
        }

        return Directory.GetFiles(directoryPath, extension)
            .Where(path => !Path.GetFileName(path).StartsWith("~"))
            .ToArray();
    }

    public static bool CreateNewDirectory(string directoryPath)
    {
        ArgumentException.ThrowIfNullOrEmpty(directoryPath);

        if (Directory.Exists(directoryPath))
        {
            return false;
        }

        Directory.CreateDirectory(directoryPath);
        return true;
    }
}

