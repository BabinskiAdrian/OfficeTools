using CommunityToolkit.Mvvm.Input;
using OfficeTool.Core.Services;
using System;

namespace OfficeTool.Ui.ViewModels;

public partial class MainMenuViewModel : ViewModelBase
{
    private readonly Action<ViewModelBase> _navigateAction;  // For changeing view in run-time
    private readonly IDialogService? _dialogService;

    // Main constructor
    public MainMenuViewModel(Action<ViewModelBase> navigateAction, IDialogService dialogService)
    {
        _navigateAction = navigateAction;
        _dialogService = dialogService;
    }

    // Constructor for live view Avalonii's designer
    public MainMenuViewModel()
    {
        _navigateAction = (_) => { };
    }

    [RelayCommand]
    private void OpenBackupConfigsChanger()
    {
        // Open new view/window
        if (_dialogService != null)
        {
            _navigateAction(new BackupConfigsViewModel(_dialogService, _navigateAction));
        }
    }

    [RelayCommand]
    private void OpenNewExcelsGenerator()
    {
        // Tutaj w przyszłości powiemy: _navigateAction(new ExcelGeneratorViewModel());
    }
}