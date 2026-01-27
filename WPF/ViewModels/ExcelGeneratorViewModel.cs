using global::OfficeTools.ExcelGenerator.Core;
using global::OfficeTools.ExcelGenerator.Core.Models;
using OfficeTools.Shared;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace WPF.ViewModels
{
    public class ExcelGeneratorViewModel : ViewModelBase
    {
        private readonly GeneratorService _generatorService;

        // Backing fields
        private string _sourceFilePath = string.Empty;
        private string _statusMessage = "Finished";
        private bool _isBusy;
        private int _progressValue;
        private int _filesCount = 0;

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

        public ExcelGeneratorViewModel()
        {
            _generatorService = new GeneratorService();

            // Connecting methods buttons/commands
            SelectFileCommand = new RelayCommand(_ => SelectFile());
            GenerateCommand = new RelayCommand(async _ => await GenerateData(), _ => CanGenerate());
        }

        private void SelectFile()
        {
            // Standard window for file selection
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xlsm",
                Title = "Wybierz plik wzorcowy"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SourceFilePath = openFileDialog.FileName;
                StatusMessage = "Wybrano plik. Gotowy do startu.";
            }
        }

        private bool CanGenerate()
        {
            return 
                !IsBusy 
                && !string.IsNullOrWhiteSpace(SourceFilePath) 
                && File.Exists(SourceFilePath);
        }

        private async Task GenerateData()
        {
            IsBusy = true;
            StatusMessage = "Generowanie w toku...";
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
                MessageBox.Show("Files have been generated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = "Error!";
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false; // Unlock buttons
            }
        }
    }
}