namespace WPF.Services;

public interface IDialogService
{
    string? OpenFile(string filter);
    void ShowMessage(string message, string title);
    void ShowError(string message, string title);
}
