using System;
using System.ComponentModel;

namespace CmdletHelpEditor.API.Models {
    public class Example : INotifyPropertyChanged {
		String ename, cmd, description, output;
        readonly Int32 _uid;

        public Example() {
            _uid = Guid.NewGuid().GetHashCode();
        }
        public String Name {
			get { return ename ?? String.Empty; }
			set {
                if (ename != value) {
                    ename = value;
                    OnPropertyChanged("Name");
                }
			}
		}
		public String Cmd {
			get { return cmd ?? String.Empty; }
			set {
                if (cmd != value) {
                    cmd = value;
                    OnPropertyChanged("Cmd");
                }
			}
		}
		public String Description {
			get { return description ?? String.Empty; }
			set {
                if (description != value) {
                    description = value;
                    OnPropertyChanged("Description");
                }
			}
		}
		public String Output {
			get { return output ?? String.Empty; }
			set {
                if (output != value) {
                    output = value;
                    OnPropertyChanged("Output");
                }
			}
		}

        public override Boolean Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Example)obj);
        }

        protected bool Equals(Example other) {
            return _uid == other._uid;
        }

        public override int GetHashCode() {
            unchecked {
                return _uid.GetHashCode() * 397;
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
