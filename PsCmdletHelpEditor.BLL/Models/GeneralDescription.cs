using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace PsCmdletHelpEditor.BLL.Models {
    public class GeneralDescription : INotifyPropertyChanged {
        String synopsis, description, notes, inputType, inputUrl, inputDescription, returnType, returnUrl, returnDescription;
        ItemStatus status = ItemStatus.New;

        public String Synopsis {
            get => synopsis ?? String.Empty;
            set {
                if (synopsis != value) {
                    synopsis = value;
                    status = ItemStatus.Incomplete;
                    OnPropertyChanged(nameof(Synopsis));
                    OnPropertyChanged(nameof(Status));
                }
            }
        }
        public String Description {
            get => description ?? String.Empty;
            set {
                if (description != value) {
                    description = value;
                    status = ItemStatus.Incomplete;
                    OnPropertyChanged(nameof(Description));
                    OnPropertyChanged(nameof(Status));
                }
            }
        }
        public String Notes {
            get => notes ?? String.Empty;
            set {
                if (notes != value) {
                    notes = value;
                    OnPropertyChanged(nameof(Notes));
                }
            }
        }
        public String InputType {
            get => inputType ?? String.Empty;
            set {
                if (inputType != value) {
                    inputType = value;
                    OnPropertyChanged(nameof(InputType));
                }
            }
        }
        public String InputUrl {
            get => inputUrl ?? String.Empty;
            set {
                if (inputUrl != value) {
                    inputUrl = value;
                    OnPropertyChanged(nameof(InputUrl));
                }
            }
        }
        public String InputTypeDescription {
            get => inputDescription ?? String.Empty;
            set {
                if (inputDescription != value) {
                    inputDescription = value;
                    OnPropertyChanged(nameof(InputTypeDescription));
                }
            }
        }
        public String ReturnType {
            get => returnType ?? String.Empty;
            set {
                if (returnType != value) {
                    returnType = value;
                    OnPropertyChanged(nameof(ReturnType));
                }
            }
        }
        public String ReturnUrl {
            get => returnUrl ?? String.Empty;
            set {
                if (returnUrl != value) {
                    returnUrl = value;
                    OnPropertyChanged(nameof(ReturnUrl));
                }
            }
        }
        public String ReturnTypeDescription {
            get => returnDescription ?? String.Empty;
            set {
                if (returnDescription != value) {
                    returnDescription = value;
                    OnPropertyChanged(nameof(ReturnTypeDescription));
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
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
