using CommunityToolkit.Mvvm.Input;
using System;

namespace OfficeTool.Ui.ViewModels;

public partial class MainMenuViewModel : ViewModelBase
{
    // Ta "akcja" to pilot do telewizora. Menu używa jej, by przełączyć kanał (stronę) w głównym oknie.
    private readonly Action<ViewModelBase> _navigateAction;

    public MainMenuViewModel(Action<ViewModelBase> navigateAction)
    {
        _navigateAction = navigateAction;
    }

    // Pusty konstruktor wymagany dla designera Avalonii (podglądu na żywo)
    public MainMenuViewModel()
    {
        _navigateAction = (_) => { };
    }

    [RelayCommand]
    private void OpenBackupConfigsChanger()
    {
        // Tutaj w przyszłości powiemy: _navigateAction(new BackupConfigsViewModel());
        // Na razie zostawiamy puste, aż stworzymy ten nowy ekran.
    }

    [RelayCommand]
    private void OpenExcelToNewExcels()
    {
        // Tutaj w przyszłości powiemy: _navigateAction(new ExcelGeneratorViewModel());
    }
}