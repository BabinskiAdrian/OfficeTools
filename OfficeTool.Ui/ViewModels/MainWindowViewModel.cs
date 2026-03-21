using CommunityToolkit.Mvvm.ComponentModel;
using OfficeTool.Core.Services;

namespace OfficeTool.Ui.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService? _dialogService;

    [ObservableProperty]
    private ViewModelBase _currentPage;

    // Konstruktor główny
    public MainWindowViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;

        // Startujemy z widokiem Menu. Przekazujemy mu metodę "Navigate", 
        // żeby Menu mogło samo przełączać strony!
        _currentPage = new MainMenuViewModel(Navigate);
    }

    // Konstruktor dla Designera
    public MainWindowViewModel()
    {
        _currentPage = new MainMenuViewModel();
    }

    // To jest nasz "silnik nawigacji"
    private void Navigate(ViewModelBase viewModel)
    {
        CurrentPage = viewModel;
    }
}