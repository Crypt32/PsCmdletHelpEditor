#nullable enable
using System;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.ViewModels;
/// <summary>
/// Represents asynchronous base view model. This class shall be used in conjunction with LoadingSpinner WPF control.
/// </summary>
public abstract class AsyncViewModel : ViewModelBase {
    Boolean isBusy;
    String? spinnerText;

    /// <summary>
    /// Gets asynchronous operation run status.
    /// </summary>
    public Boolean IsBusy {
        get => isBusy;
        private set {
            isBusy = value;
            OnPropertyChanged();
        }
    }
    /// <summary>
    /// Gets asynchronous operation optional display text.
    /// </summary>
    public String? SpinnerText {
        get => spinnerText;
        private set {
            spinnerText = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Initiates asynchronous operation. This method is used to trigger LoadingSpinner WPF control.
    /// </summary>
    /// <param name="text">Optional operation display text.</param>
    public void StartSpinner(String? text = null) {
        IsBusy = true;
        SpinnerText = text;
    }
    /// <summary>
    /// Finishes asynchronous operation. This method is used to unload LoadingSpinner WPF control.
    /// </summary>
    public void StopSpinner() {
        IsBusy = false;
        SpinnerText = null;
    }
}