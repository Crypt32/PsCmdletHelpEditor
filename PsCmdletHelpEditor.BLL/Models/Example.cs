using System;
using System.ComponentModel;

namespace PsCmdletHelpEditor.BLL.Models {
    public class Example : INotifyPropertyChanged {
        String ename, cmd, description, output;
        readonly Int32 _uid;

        public Example() {
            _uid = Guid.NewGuid().GetHashCode();
        }
        public String Name {
            get => ename ?? String.Empty;
            set {
                if (ename != value) {
                    ename = value;
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
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Example)obj);
        }

        protected Boolean Equals(Example other) {
            return _uid == other._uid;
        }

        public override Int32 GetHashCode() {
            unchecked {
                return _uid.GetHashCode() * 397;
            }
        }

        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
