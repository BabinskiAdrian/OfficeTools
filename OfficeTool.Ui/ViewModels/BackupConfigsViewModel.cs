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
    private string _baseIpAddress= "Brak odczytanych danych";

    // Mode: true = Amount, false = Range
    [ObservableProperty]
    private bool _isAmountMode = true;

    // Range mode - low octet
    [ObservableProperty] private int _startOctet1 = 192;
    [ObservableProperty] private int _startOctet2 = 168;
    [ObservableProperty] private int _startOctet3 = 0;
    [ObservableProperty] private int _startOctet4 = 1;

    // Range mode - high octet
    [ObservableProperty] private int _endOctet1 = 192;
    [ObservableProperty] private int _endOctet2 = 168;
    [ObservableProperty] private int _endOctet3 = 0;
    [ObservableProperty] private int _endOctet4 = 10;

    [ObservableProperty]
    private int _calculatedTotalFiles = 1;

    [ObservableProperty]
    private int _amountOfFiles = 1; // Domyślnie 1 plik

    [ObservableProperty]
    private int _generationMode = 0; // 0 = Up, 1 = Down, 2 = Up & Down

    [ObservableProperty]
    private int _startOctet = 1;

    [ObservableProperty]
    private int _endOctet = 10;

    public BackupConfigsViewModel(IDialogService dialogService, Action<ViewModelBase> navigateAction)
    {
        _dialogService = dialogService;
        _navigateAction = navigateAction;
    }

    // For designer Avalonia
    public BackupConfigsViewModel() 
    {
        _navigateAction = (_) => { };
    }

    [RelayCommand]
    private async Task PickBaseZipAsync()
    {
        if (_dialogService is null) return;


        var file = await _dialogService.PickFileAsync();
        if (file != null)
        {
            BaseZipPath = file;


            // TEMPORARY
            // TODO: implement logic
            StartOctet1 = 10;
            StartOctet2 = 0;
            StartOctet3 = 32;
            StartOctet4 = 201;

            // Dla wygody użytkownika, początkowo ustawiamy zakres końcowy na to samo
            EndOctet1 = StartOctet1;
            EndOctet2 = StartOctet2;
            EndOctet3 = StartOctet3;
            EndOctet4 = StartOctet4;
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
        // Back to Main Menu
        _navigateAction(new MainMenuViewModel(_navigateAction, _dialogService));
    }

    [RelayCommand]
    private void Generate()
    {
        // Generator Start
    }

    // Update logic
    partial void OnIsAmountModeChanged(bool value) => RecalculateTotal();
    partial void OnAmountOfFilesChanged(int value) => RecalculateTotal();
    partial void OnStartOctet1Changed(int value) => RecalculateTotal();
    partial void OnStartOctet2Changed(int value) => RecalculateTotal();
    partial void OnStartOctet3Changed(int value) => RecalculateTotal();
    partial void OnStartOctet4Changed(int value) => RecalculateTotal();
    partial void OnEndOctet1Changed(int value) => RecalculateTotal();
    partial void OnEndOctet2Changed(int value) => RecalculateTotal();
    partial void OnEndOctet3Changed(int value) => RecalculateTotal();
    partial void OnEndOctet4Changed(int value) => RecalculateTotal();

    private void RecalculateTotal()
    {
        if (IsAmountMode)
        {
            CalculatedTotalFiles = AmountOfFiles;
        }
        else
        {
            // Przeliczamy IP na pojedyncze liczby, żeby łatwo wyciągnąć różnicę (ilość kroków)
            long startIp = (StartOctet1 * 16777216L) + (StartOctet2 * 65536L) + (StartOctet3 * 256L) + StartOctet4;
            long endIp = (EndOctet1 * 16777216L) + (EndOctet2 * 65536L) + (EndOctet3 * 256L) + EndOctet4;

            // Wartość bezwzględna + 1 (bo sam plik bazowy to też 1 plik docelowy)
            CalculatedTotalFiles = (int)Math.Abs(endIp - startIp) + 1;
        }
    }
}