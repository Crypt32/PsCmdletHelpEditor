using CmdletHelpEditor.API.Tools;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CmdletHelpEditor.API.BaseClasses {
	public class GeneralDescription : INotifyPropertyChanged {
		String synopsis, description, notes, inputType, inputUrl, inputDescritpion, returnType, returnUrl, returnDescription;
		ItemStatus status = ItemStatus.New;

		public String Synopsis {
			get { return synopsis ?? String.Empty; }
			set {
				if (synopsis != value) {
					synopsis = value;
					status = ItemStatus.Incomplete;
					OnPropertyChanged("Synopsis");
					OnPropertyChanged("Status");
				}
			}
		}
		public String Description {
			get { return description ?? String.Empty; }
			set {
				if (description != value) {
					description = value;
					status = ItemStatus.Incomplete;
					OnPropertyChanged("Description");
					OnPropertyChanged("Status");
				}
			}
		}
		public String Notes {
			get { return notes ?? String.Empty; }
			set {
				notes = value;
				OnPropertyChanged("Notes");
			}
		}
		public String InputType {
			get { return inputType ?? String.Empty; }
			set {
				inputType = value;
				OnPropertyChanged("InputType");
			}
		}
		public String InputUrl {
			get { return inputUrl ?? String.Empty; }
			set {
				inputUrl = value;
				OnPropertyChanged("InputUrl");
			}
		}
		public String InputTypeDescription {
			get { return inputDescritpion ?? String.Empty; }
			set {
				inputDescritpion = value;
				OnPropertyChanged("InputTypeDescription");
			}
		}
		public String ReturnType {
			get { return returnType ?? String.Empty; }
			set {
				returnType = value;
				OnPropertyChanged("ReturnType");
			}
		}
		public String ReturnUrl {
			get { return returnUrl ?? String.Empty; }
			set {
				returnUrl = value;
				OnPropertyChanged("ReturnUrl");
			}
		}
		public String ReturnTypeDescription {
			get { return returnDescription ?? String.Empty; }
			set {
				returnDescription = value;
				OnPropertyChanged("ReturnTypeDescription");
			}
		}
		[XmlIgnore]
		public ItemStatus Status {
			get {
				if (status == ItemStatus.Missing || status == ItemStatus.New) { return status; }
				return status = String.IsNullOrEmpty(Description) || String.IsNullOrEmpty(Synopsis)
								? ItemStatus.Incomplete
								: ItemStatus.Valid;
			}
			set {
				status = value;
				OnPropertyChanged("Status");
			}
		}

		void OnPropertyChanged(String name) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				Utils.MarkUnsaved();
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
