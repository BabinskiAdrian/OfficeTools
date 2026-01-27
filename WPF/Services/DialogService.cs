using System.Windows;

namespace WPF.Services;

public class DialogService : IDialogService
{
    public string? OpenFile(string filter)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog { Filter = filter };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public void ShowMessage(string message, string title)
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

    public void ShowError(string message, string title)
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
}
