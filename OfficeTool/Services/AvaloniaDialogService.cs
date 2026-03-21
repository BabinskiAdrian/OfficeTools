using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using OfficeTool.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace OfficeTool.Ui.Services;

public class AvaloniaDialogService : IDialogService
{
    public async Task<string?> PickFolderAsync()
    {
        var storageProvider = GetStorageProvider();
        if (storageProvider == null) return null;

        var result = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Wybierz folder"
        });

        return result.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<string?> PickFileAsync()
    {
        var storageProvider = GetStorageProvider();
        if (storageProvider == null) return null;

        var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Wybierz plik",
            AllowMultiple = false
        });

        return result.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<IEnumerable<string>?> PickMultipleFilesAsync()
    {
        var storageProvider = GetStorageProvider();
        if (storageProvider == null) return null;

        var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Wybierz pliki do przetworzenia",
            // To ta flaga umożliwia zaznaczanie wielu elementów z klawiszem Shift/Ctrl
            AllowMultiple = true
        });

        if (result == null || result.Count == 0)
            return null;

        // Przekształcamy listę obiektów na zwykłą, lekką kolekcję ścieżek tekstowych
        return result.Select(file => file.Path.LocalPath).ToArray();
    }

    private IStorageProvider? GetStorageProvider()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow?.StorageProvider;
        }
        return null;
    }
}
