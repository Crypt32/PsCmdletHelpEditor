using CmdletHelpEditor.API.BaseClasses;
using CmdletHelpEditor.API.Tools;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace CmdletHelpEditor.API.ViewModel {
	public class ExampleVM : INotifyPropertyChanged {
		readonly EditorVM _evm;
		Example currentExample;
		Boolean exampleTextBoxEnabled, removeExampleEnabled, upExampleEnabled, downExampleEnabled;
		Int32 currentExampleIndex = -1;

		public ExampleVM(EditorVM parent) {
			_evm = parent;
			Examples = new ObservableCollection<Example>();
			NewExampleCommand = new RelayCommand(NewExample);
			RemoveExampleCommand = new RelayCommand(RemoveExample);
			UpExampleCommand = new RelayCommand(UpExample);
			DownExampleCommad = new RelayCommand(DownExample);
		}

		public ObservableCollection<Example> Examples { get; set; }

		public ICommand NewExampleCommand { get; set; }
		public ICommand RemoveExampleCommand { get; set; }
		public ICommand UpExampleCommand { get; set; }
		public ICommand DownExampleCommad { get; set; }

		public Boolean RemoveExampleEnabled {
			get { return removeExampleEnabled; }
			set {
				removeExampleEnabled = value;
				OnPropertyChanged("RemoveExampleEnabled");
			}
		}
		public Boolean UpExampleEnabled {
			get { return upExampleEnabled; }
			set {
				upExampleEnabled = value;
				OnPropertyChanged("UpExampleEnabled");
			}
		}
		public Boolean DownExampleEnabled {
			get { return downExampleEnabled; }
			set {
				downExampleEnabled = value;
				OnPropertyChanged("DownExampleEnabled");
			}
		}
		public Int32 CurrentExampleIndex {
			get { return currentExampleIndex; }
			set {
				currentExampleIndex = value;
				SetExampleButtonState();
				ExampleTextBoxEnabled = currentExampleIndex >= 0;
				OnPropertyChanged("CurrentExampleIndex");
			}
		}
		public Boolean ExampleTextBoxEnabled {
			get { return exampleTextBoxEnabled; }
			set {
				exampleTextBoxEnabled = value;
				OnPropertyChanged("ExampleTextBoxEnabled");
			}
		}
		public Example CurrentExample {
			get { return currentExample; }
			set {
				currentExample = value;
				OnPropertyChanged("CurrentExample");
			}
		}
		
		void NewExample(Object obj) {
			if (_evm.CurrentCmdlet == null) { return; }
			_evm.CurrentCmdlet.Examples.Add(new Example { Name = "Example " + (_evm.CurrentCmdlet.Examples.Count + 1) });
			CurrentExampleIndex = _evm.CurrentCmdlet.Examples.Count - 1;
		}
		void RemoveExample(Object obj) {
			if (CurrentExampleIndex >= 0) {
				_evm.CurrentCmdlet.Examples.RemoveAt(CurrentExampleIndex);
				Utils.MarkUnsaved();
			}
			if (CurrentExampleIndex > 0) { CurrentExampleIndex--; }
		}
		void UpExample(Object obj) {
			Int32 old = CurrentExampleIndex;
			Example temp = _evm.CurrentCmdlet.Examples[CurrentExampleIndex - 1];
			_evm.CurrentCmdlet.Examples[CurrentExampleIndex - 1] = CurrentExample;
			_evm.CurrentCmdlet.Examples[CurrentExampleIndex] = temp;
			CurrentExampleIndex = old - 1;
			Utils.MarkUnsaved();
		}
		void DownExample(Object obj) {
			Int32 old = CurrentExampleIndex;
			Example temp = _evm.CurrentCmdlet.Examples[old + 1];
			_evm.CurrentCmdlet.Examples[old + 1] = CurrentExample;
			_evm.CurrentCmdlet.Examples[old] = temp;
			CurrentExampleIndex = old + 1;
			Utils.MarkUnsaved();
		}
		void SetExampleButtonState() {
			if (CurrentExampleIndex < 0) {
				RemoveExampleEnabled = UpExampleEnabled = DownExampleEnabled = false;
			} else if (CurrentExampleIndex == 0) {
				RemoveExampleEnabled = true;
				UpExampleEnabled = false;
				DownExampleEnabled = _evm.CurrentCmdlet.Examples.Count > 1;
			} else if (CurrentExampleIndex > 0 && CurrentExampleIndex < _evm.CurrentCmdlet.Examples.Count - 1) {
				RemoveExampleEnabled = UpExampleEnabled = DownExampleEnabled = true;
			} else {
				RemoveExampleEnabled = UpExampleEnabled = true;
				DownExampleEnabled = false;
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
