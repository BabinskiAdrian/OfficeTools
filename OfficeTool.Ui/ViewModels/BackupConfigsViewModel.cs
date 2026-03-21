using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OfficeTool.Core.Services;

namespace OfficeTool.Ui.ViewModels;

public partial class BackupConfigsViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly Action<ViewModelBase> _navigateAction;

    [ObservableProperty]
    private string _baseZipPath = "Nie wybrano pliku bazowego (.zip)";

    [ObservableProperty]
    private string _destinationFolderPath = "Nie wybrano folderu docelowego";

    [ObservableProperty]
    private bool _isAmountMode = true;

    [ObservableProperty]
    private int _amountOfFiles = 1; // Domyślnie 1 plik

    [ObservableProperty]
    private int _generationMode = 0; // 0 = Up, 1 = Down, 2 = Up & Down

    public BackupConfigsViewModel(IDialogService dialogService, Action<ViewModelBase> navigateAction)
    {
        _dialogService = dialogService;
        _navigateAction = navigateAction;
    }

    public BackupConfigsViewModel() { } // Dla designera

    [RelayCommand]
    private async Task PickBaseZipAsync()
    {
        // Używamy naszej metody do wielu plików, ale bierzemy tylko pierwszy (możesz w przyszłości dopisać w IDialogService metodę na 1 plik)
        var files = await _dialogService.PickMultipleFilesAsync();
        var firstFile = files?.FirstOrDefault();

        if (firstFile != null)
        {
            BaseZipPath = firstFile;
        }
    }

    [RelayCommand]
    private async Task PickDestinationFolderAsync()
    {
        var folder = await _dialogService.PickFolderAsync();
        if (folder != null)
        {
            DestinationFolderPath = folder;
        }
    }

    [RelayCommand]
    private void GoBack()
    {
        // Wracamy do menu!
        _navigateAction(new MainMenuViewModel(_navigateAction, _dialogService));
    }

    [RelayCommand]
    private void Generate()
    {
        // Tutaj podepniemy logikę z OfficeTool.Core, która wykorzysta:
        // BaseZipPath, DestinationFolderPath, AmountOfFiles oraz GenerationMode
    }
}