using CmdletHelpEditor.API.BaseClasses;
using CmdletHelpEditor.API.Tools;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace CmdletHelpEditor.API.ViewModel {
	public class RelatedLinkVM : INotifyPropertyChanged {
		readonly EditorVM _evm;
		Boolean linkTextBoxEnabled, removeLinkEnabled, upLinkEnabled, downLinkEnabled;
		RelatedLink currentRelink;
		Int32 currentLinkIndex = -1;

		public RelatedLinkVM(EditorVM parent) {
			_evm = parent;
			NewLinkCommand = new RelayCommand(NewLink);
			RemoveLinkCommand = new RelayCommand(RemoveLink);
			UpLinkCommand = new RelayCommand(UpLink);
			DownLinkCommad = new RelayCommand(DownLink);
		}
		
		public ICommand NewLinkCommand { get; set; }
		public ICommand RemoveLinkCommand { get; set; }
		public ICommand UpLinkCommand { get; set; }
		public ICommand DownLinkCommad { get; set; }

		public Boolean RemoveLinkEnabled {
			get { return removeLinkEnabled; }
			set {
				removeLinkEnabled = value;
				OnPropertyChanged("RemoveLinkEnabled");
			}
		}
		public Boolean UpLinkEnabled {
			get { return upLinkEnabled; }
			set {
				upLinkEnabled = value;
				OnPropertyChanged("UpLinkEnabled");
			}
		}
		public Boolean DownLinkEnabled {
			get { return downLinkEnabled; }
			set {
				downLinkEnabled = value;
				OnPropertyChanged("DownLinkEnabled");
			}
		}
		public Int32 CurrentLinkIndex {
			get { return currentLinkIndex; }
			set {
				currentLinkIndex = value;
				SetLinkButtonState();
				LinkTextBoxEnabled = currentLinkIndex >= 0;
				OnPropertyChanged("CurrentLinkIndex");
			}
		}
		public Boolean LinkTextBoxEnabled {
			get { return linkTextBoxEnabled; }
			set {
				linkTextBoxEnabled = value;
				OnPropertyChanged("LinkTextBoxEnabled");
			}
		}
		public RelatedLink CurrentRelink {
			get { return currentRelink; }
			set {
				currentRelink = value;
				OnPropertyChanged("CurrentRelink");
			}
		}

		void NewLink(Object obj) {
			if (_evm.CurrentCmdlet == null) { return; }
			_evm.CurrentCmdlet.RelatedLinks.Add(new RelatedLink());
			CurrentLinkIndex = _evm.CurrentCmdlet.RelatedLinks.Count - 1;
			Utils.MarkUnsaved();
		}
		void RemoveLink(Object obj) {
			if (CurrentLinkIndex >= 0) {
				_evm.CurrentCmdlet.RelatedLinks.RemoveAt(CurrentLinkIndex);
				Utils.MarkUnsaved();
			}
			if (CurrentLinkIndex > 0) { CurrentLinkIndex--; }
			Utils.MarkUnsaved();
		}
		void UpLink(Object obj) {
			Int32 old = CurrentLinkIndex;
			RelatedLink temp = _evm.CurrentCmdlet.RelatedLinks[CurrentLinkIndex - 1];
			_evm.CurrentCmdlet.RelatedLinks[CurrentLinkIndex - 1] = CurrentRelink;
			_evm.CurrentCmdlet.RelatedLinks[CurrentLinkIndex] = temp;
			CurrentLinkIndex = old - 1;
			Utils.MarkUnsaved();
			Utils.MarkUnsaved();
		}
		void DownLink(Object obj) {
			Int32 old = CurrentLinkIndex;
			RelatedLink temp = _evm.CurrentCmdlet.RelatedLinks[CurrentLinkIndex + 1];
			_evm.CurrentCmdlet.RelatedLinks[CurrentLinkIndex + 1] = CurrentRelink;
			_evm.CurrentCmdlet.RelatedLinks[CurrentLinkIndex] = temp;
			CurrentLinkIndex = old + 1;
			Utils.MarkUnsaved();
		}
		void SetLinkButtonState() {
			if (CurrentLinkIndex < 0) {
				RemoveLinkEnabled = UpLinkEnabled = DownLinkEnabled = false;
			} else if (CurrentLinkIndex == 0) {
				RemoveLinkEnabled = true;
				UpLinkEnabled = false;
				DownLinkEnabled = _evm.CurrentCmdlet.RelatedLinks.Count > 1;
			} else if (CurrentLinkIndex > 0 && CurrentLinkIndex < _evm.CurrentCmdlet.RelatedLinks.Count - 1) {
				RemoveLinkEnabled = UpLinkEnabled = DownLinkEnabled = true;
			} else {
				RemoveLinkEnabled = UpLinkEnabled = true;
				DownLinkEnabled = false;
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
