using System;
using System.ComponentModel;
using System.Xml.Serialization;
using CmdletHelpEditor.API.Tools;

namespace CmdletHelpEditor.API.Models {
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
                if (notes != value) {
                    notes = value;
                    OnPropertyChanged("Notes");
                }
			}
		}
		public String InputType {
			get { return inputType ?? String.Empty; }
			set {
                if (inputType != value) {
                    inputType = value;
                    OnPropertyChanged("InputType");
                }
			}
		}
		public String InputUrl {
			get { return inputUrl ?? String.Empty; }
			set {
                if (inputUrl != value) {
                    inputUrl = value;
                    OnPropertyChanged("InputUrl");
                }
			}
		}
		public String InputTypeDescription {
			get { return inputDescritpion ?? String.Empty; }
			set {
                if (inputDescritpion != value) {
                    inputDescritpion = value;
                    OnPropertyChanged("InputTypeDescription");
                }
			}
		}
		public String ReturnType {
			get { return returnType ?? String.Empty; }
			set {
                if (returnType != value) {
                    returnType = value;
                    OnPropertyChanged("ReturnType");
                }
			}
		}
		public String ReturnUrl {
			get { return returnUrl ?? String.Empty; }
			set {
                if (returnUrl != value) {
                    returnUrl = value;
                    OnPropertyChanged("ReturnUrl");
                }
			}
		}
		public String ReturnTypeDescription {
			get { return returnDescription ?? String.Empty; }
			set {
                if (returnDescription != value) {
                    returnDescription = value;
                    OnPropertyChanged("ReturnTypeDescription");
                }
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
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
