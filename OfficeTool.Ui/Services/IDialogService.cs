using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeTool.Ui.Services;

/// <summary>
/// Communication with systems dialogs window
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Asynchronously opens a directiory picker dialog
    /// </summary>
    Task<string?> PickFolderAsync();

    /// <summary>
    /// Asynchronously opens a file picker dialog
    /// </summary>
    Task<string?> PickFileAsync();


    /// <summary>
    /// Asynchronously opens a files picker dialog
    /// </summary>
    Task<IEnumerable<string>?> PickMultipleFilesAsync();
}