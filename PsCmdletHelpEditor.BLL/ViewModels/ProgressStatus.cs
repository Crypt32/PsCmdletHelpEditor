using System;
using PsCmdletHelpEditor.BLL.Abstraction;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    class ProgressStatus : ViewModelBase, IProgressStatus {
        String text;
        Double progress;

        public String StatusText {
            get => text;
            set {
                text = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }
        public Double Progress {
            get => progress;
            set {
                progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }
    }
}
