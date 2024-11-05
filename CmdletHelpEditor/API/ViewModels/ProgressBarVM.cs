using System;
using CmdletHelpEditor.Abstract;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.ViewModels;

class ProgressBarVM : ViewModelBase, IProgressBar {
    Boolean isRunning;
    Double progress;
    String progressText;

    public Double Progress {
        get => progress;
        set {
            progress = value;
            OnPropertyChanged();
        }
    }
    public String ProgressText {
        get => progressText;
        set {
            progressText = value;
            OnPropertyChanged();
        }
    }
    public Boolean IsRunning {
        get => isRunning;
        private set {
            isRunning = value;
            OnPropertyChanged();
        }
    }
    public void Start() {
        Progress = 0;
        ProgressText = "Started";
        IsRunning = true;
    }
    public void Stop() {
        Progress = 100;
        ProgressText = "Completed";
    }
    public void End() {
        Stop();
        IsRunning = false;
    }
}