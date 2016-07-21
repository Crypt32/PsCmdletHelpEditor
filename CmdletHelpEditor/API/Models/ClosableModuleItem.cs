using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CmdletHelpEditor.API.Utility;
using CmdletHelpEditor.API.ViewModels;

namespace CmdletHelpEditor.API.Models {
	public class ClosableModuleItem : TabItem, INotifyPropertyChanged {
        ModuleObject module;
        Boolean isSaved;
        String errorInfo;

        // must be DependencyProperty
        public static readonly DependencyProperty IsClosableProperty = DependencyProperty.Register("IsClosable", typeof(Boolean), typeof(ClosableModuleItem), new FrameworkPropertyMetadata(false));

	    void ModuleOnPendingSave(Object source, SavePendingEventArgs e) {
            IsSaved = false;
	    }

        public Boolean IsClosable { 
 			get { return (Boolean)GetValue(IsClosableProperty); } 
			set { SetValue(IsClosableProperty, value); } 
 		}

        public Boolean IsSaved {
			get { return isSaved; }
			set {
                if (isSaved != value) {
                    isSaved = value;
                    Header = isSaved
                        ? Header.ToString().Replace("*", String.Empty)
                        : Header + "*";
                    OnPropertyChanged("IsSaved");
                }
            }
		}
		public ModuleObject Module {
	        get { return module; }
            set {
                module = value;
                OnPropertyChanged("Module");
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
			get { return errorInfo; }
			set {
                errorInfo = value;
                OnPropertyChanged("ErrorInfo");
			}
		}
		public EditorVM EditorContext { get; set; }
        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
