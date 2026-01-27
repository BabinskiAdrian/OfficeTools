using global::OfficeTools.ExcelGenerator.Core;
using global::OfficeTools.ExcelGenerator.Core.Models;
using OfficeTools.Shared;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WPF.Services;

namespace WPF.ViewModels;

public class ExcelGeneratorViewModel : ViewModelBase
{
    private readonly GeneratorService _generatorService;
    private readonly IDialogService _dialogService;

    // Backing fields
    private string _sourceFilePath = string.Empty;
    private string _statusMessage = "Ready";
    private bool _isBusy;
    private int _progressValue;
    private int _filesCount = 10;

    // Properites bound to the UI elements
    public string SourceFilePath
    {
        get => _sourceFilePath;
        set => SetProperty(ref _sourceFilePath, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public int ProgressValue
    {
        get => _progressValue;
        set => SetProperty(ref _progressValue, value);
    }

    // Locking buttons during processing
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public int FilesCount
    {
        get => _filesCount;
        set => SetProperty(ref _filesCount, value);
    }

    // Comends bound to buttons
    public ICommand SelectFileCommand { get; }
    public ICommand GenerateCommand { get; }

    public ExcelGeneratorViewModel(GeneratorService generatorService, IDialogService dialogService)
    {
        _generatorService = generatorService;
        _dialogService = dialogService;

        // Connecting methods buttons/commands
        SelectFileCommand = new RelayCommand(_ => SelectFile());
        GenerateCommand = new RelayCommand(async _ => await GenerateData(), _ => CanGenerate());
    }
    
    // Konstruktor bezparametrowy (Dla podglądu w XAML - opcjonalne, ale przydatne)
    public ExcelGeneratorViewModel() : this(new GeneratorService(), new DialogService()) { }

    private void SelectFile()
    {
        var path = _dialogService.OpenFile("Excel Files|*.xlsx;*.xlsm");
        if (!string.IsNullOrEmpty(path))
        {
            SourceFilePath = path;
            StatusMessage = "File selected.";
        }
    }

    private bool CanGenerate()
    {
        return !IsBusy 
            && !string.IsNullOrWhiteSpace(SourceFilePath) 
            && File.Exists(SourceFilePath);
    }

    private async Task GenerateData()
    {
        IsBusy = true;
        StatusMessage = "Generation in progress....";
        ProgressValue = 0;

        try
        {
            // TODO: update configuration, to get values from UI
            var config = new GeneratorConfig
            {
                OutputDirectory = Path.Combine(Path.GetDirectoryName(SourceFilePath)!, "Wyniki"),
                FilesCount = FilesCount,
                CarriersPerDay = 2,
                WorksheetIndex = 2
            };

            // Porgress reporting
            var progress = new Progress<GeneratorProgress>(p =>
            {
                ProgressValue = p.Percentage;
                StatusMessage = p.Message;
            });

            await _generatorService.GenerateAsync(SourceFilePath, config, progress);

            StatusMessage = "Success! Generation completed.";
            _dialogService.ShowMessage("Files have been generated!", "Success");
        }
        catch (Exception ex)
        {
            StatusMessage = "Error!"; 
            _dialogService.ShowError($"An error occurred: {ex.Message}", "Critical Error");
        }
        finally
        {
            IsBusy = false; // Unlock buttons
        }
    }
}