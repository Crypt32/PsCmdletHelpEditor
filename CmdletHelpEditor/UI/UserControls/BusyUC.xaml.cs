using System;
using System.ComponentModel;
using CmdletHelpEditor.API.Tools;

namespace CmdletHelpEditor.UI.UserControls {
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
			get { return txt; }
			set {
				txt = value;
				OnPropertyChanged("Text");
			}
		}

		void OnPropertyChanged(String name) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
