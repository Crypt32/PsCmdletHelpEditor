using System;
using CmdletHelpEditor.API.Abstractions;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.Models {

    public class Example : ViewModelBase, IPsExample {
        String name, cmd, description, output;
        readonly Int32 _uid;

        public Example() {
            _uid = Guid.NewGuid().GetHashCode();
        }
        public String Name {
            get => name ?? String.Empty;
            set {
                if (name != value) {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public String Cmd {
            get => cmd ?? String.Empty;
            set {
                if (cmd != value) {
                    cmd = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public String Description {
            get => description ?? String.Empty;
            set {
                if (description != value) {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        public String Output {
            get => output ?? String.Empty;
            set {
                if (output != value) {
                    output = value;
                    OnPropertyChanged(nameof(Output));
                }
            }
        }

        public override Boolean Equals(Object obj) {
            return !ReferenceEquals(null, obj) &&
                   (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Example)obj));
        }

        protected Boolean Equals(Example other) {
            return _uid == other._uid;
        }

        public override Int32 GetHashCode() {
            unchecked {
                return _uid.GetHashCode() * 397;
            }
        }
    }
}
