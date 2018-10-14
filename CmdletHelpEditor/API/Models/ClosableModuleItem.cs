using System;
using System.ComponentModel;
using System.IO;
using CmdletHelpEditor.API.Utility;
using CmdletHelpEditor.API.ViewModels;
using SysadminsLV.WPF.OfficeTheme.Controls;

namespace CmdletHelpEditor.API.Models {
    public class ClosableModuleItem : ClosableTabItem, INotifyPropertyChanged {
        ModuleObject module;
        Boolean isSaved;
        String errorInfo;

        
        void ModuleOnPendingSave(Object source, SavePendingEventArgs e) {
            IsSaved = false;
        }

        public Boolean IsSaved {
            get => isSaved;
            set {
                if (isSaved != value) {
                    isSaved = value;
                    Header = isSaved
                        ? Header.ToString().Replace("*", String.Empty)
                        : Header + "*";
                    OnPropertyChanged(nameof(IsSaved));
                }
            }
        }
        public ModuleObject Module {
            get => module;
            set {
                module = value;
                OnPropertyChanged(nameof(Module));
                if (value != null) {
                    module.PendingSave += ModuleOnPendingSave;
                    if (!String.IsNullOrEmpty(value.ProjectPath)) {
                        FileInfo fi = new FileInfo(value.ProjectPath);
                        Header = fi.Name;
                    }
                }
            }
        }
        public String ErrorInfo {
            get => errorInfo;
            set {
                errorInfo = value;
                OnPropertyChanged(nameof(ErrorInfo));
            }
        }
        public EditorVM EditorContext { get; set; }
        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
