using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OfficeTool.Ui.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // Ta właściwość przechowuje aktualnie wyświetlaną "stronę" (inny ViewModel)
    [ObservableProperty]
    private ViewModelBase _currentPage;

    public MainWindowViewModel()
    {
        // Na ten moment ustawiamy pustą stronę startową, żeby aplikacja się nie wysypała.
        // W następnym kroku podmienimy to na "MenuViewModel".
        _currentPage = new ViewModelBase();
    }
}