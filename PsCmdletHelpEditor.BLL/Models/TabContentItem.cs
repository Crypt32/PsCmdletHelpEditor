using System;
using System.IO;
using PsCmdletHelpEditor.BLL.Abstraction;
using PsCmdletHelpEditor.BLL.Utility;
using PsCmdletHelpEditor.BLL.ViewModels;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace PsCmdletHelpEditor.BLL.Models {
    public class TabItem : ViewModelBase, ITabItem {
        ModuleObject module;
        Boolean isSaved;
        String header, errorInfo;
        ITabItemContent tabContent;


        void ModuleOnPendingSave(Object source, SavePendingEventArgs e) {
            IsSaved = false;
        }

        public String Header {
            get => header;
            set {
                header = value;
                OnPropertyChanged(nameof(Header));
            }
        }
        public ITabItemContent TabContent {
            get => tabContent;
            set {
                tabContent = value;
                OnPropertyChanged(nameof(TabContent));
            }
        }
        public Boolean IsSaved {
            get => isSaved;
            set {
                if (isSaved != value) {
                    isSaved = value;
                    Header = isSaved
                        ? Header.Replace("*", String.Empty)
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

        public Boolean RequestClose() {
            return false;
        }
        public void Save() {

        }
    }
}
