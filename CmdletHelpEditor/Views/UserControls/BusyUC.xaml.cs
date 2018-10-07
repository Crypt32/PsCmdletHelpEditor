using System;
using System.ComponentModel;
using CmdletHelpEditor.API.Tools;

namespace CmdletHelpEditor.Views.UserControls {
    /// <summary>
    /// Interaction logic for BusyControl.xaml
    /// </summary>
    public partial class BusyUC : INotifyPropertyChanged {
        String txt = Strings.InfoDataLoading;

        public BusyUC() : this(Strings.InfoDataLoading) { }
        public BusyUC(String text) {
            Text = text;
            InitializeComponent();
        }

        public String Text {
            get => txt;
            set {
                txt = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
