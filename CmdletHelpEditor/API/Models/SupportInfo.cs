using System;
using System.ComponentModel;
using System.Xml.Serialization;
using CmdletHelpEditor.API.Tools;

namespace CmdletHelpEditor.API.Models {
	public class SupportInfo : INotifyPropertyChanged {
        Boolean ADChecked1, RsatChecked1, Ps2Checked1, Ps3Checked1, Ps4Checked1, Ps5Checked1, WinXpChecked1,
            WinVistaChecked1, Win7Checked1, Win8Checked1, Win81Checked1, win10Checked, Win2003Checked1,
            Win2003StdChecked1, Win2003EEChecked1, Win2003DCChecked1, Win2008Checked1, Win2008StdChecked1,
            Win2008DCChecked1, Win2008R2Checked1, Win2008R2StdChecked1, Win2008R2EEChecked1, Win2008EEChecked1,
            Win2008R2DCChecked1, Win2012Checked1, Win2012StdChecked1, Win2012DCChecked1, Win2012R2Checked1,
            Win2012R2StdChecked1, Win2012R2DCChecked1;
		[XmlAttribute("ad")]
		public Boolean ADChecked {
			get { return ADChecked1; }
			set {
				ADChecked1 = value;
				OnPropertyChanged("ADChecked");
			}
		}
		[XmlAttribute("rsat")]
		public Boolean RsatChecked {
			get { return RsatChecked1; }
			set {
				RsatChecked1 = value;
				OnPropertyChanged("RsatChecked");
			}
		}
		[XmlAttribute("ps2")]
		public Boolean Ps2Checked {
			get { return Ps2Checked1; }
			set {
				Ps2Checked1 = value;
				OnPropertyChanged("Ps2Checked");
			}
		}
		[XmlAttribute("ps3")]
		public Boolean Ps3Checked {
			get { return Ps3Checked1; }
			set {
				Ps3Checked1 = value;
				OnPropertyChanged("Ps3Checked");
			}
		}
		[XmlAttribute("ps4")]
		public Boolean Ps4Checked {
			get { return Ps4Checked1; }
			set {
				Ps4Checked1 = value;
				OnPropertyChanged("Ps4Checked");
			}
		}
		[XmlAttribute("ps5")]
		public Boolean Ps5Checked {
			get { return Ps5Checked1; }
			set {
				Ps5Checked1 = value;
				OnPropertyChanged("Ps5Checked");
			}
		}
		[XmlAttribute("wxp")]
		public Boolean WinXpChecked {
			get { return WinXpChecked1; }
			set {
				WinXpChecked1 = value;
				OnPropertyChanged("WinXpChecked");
			}
		}
		[XmlAttribute("wv")]
		public Boolean WinVistaChecked {
			get { return WinVistaChecked1; }
			set {
				WinVistaChecked1 = value;
				OnPropertyChanged("WinVistaChecked");
			}
		}
		[XmlAttribute("w7")]
		public Boolean Win7Checked {
			get { return Win7Checked1; }
			set {
				Win7Checked1 = value;
				OnPropertyChanged("Win7Checked");
			}
		}
		[XmlAttribute("w8")]
		public Boolean Win8Checked {
			get { return Win8Checked1; }
			set {
				Win8Checked1 = value;
				OnPropertyChanged("Win8Checked");
			}
		}
		[XmlAttribute("w81")]
		public Boolean Win81Checked {
			get { return Win81Checked1; }
			set {
				Win81Checked1 = value;
				OnPropertyChanged("Win81Checked");
			}
		}
        [XmlAttribute("w10")]
        public Boolean Win10Checked {
            get { return win10Checked; }
            set {
                win10Checked = value;
                OnPropertyChanged("Win10Checked");
            }
        }
        [XmlIgnore]
		public Boolean Win2003Checked {
			get { return Win2003Checked1; }
			set {
				Win2003Checked1 = value;
				if (Win2003Checked1) {
					Win2003StdChecked1 = Win2003EEChecked1 = Win2003DCChecked1 = true;
					foreach (String str in new []{"Win2003StdChecked","Win2003EEChecked","Win2003DCChecked"}) {
						OnPropertyChanged(str);
					}
				} else {
					Win2003StdChecked1 = Win2003EEChecked1 = Win2003DCChecked1 = false;
				}
				foreach (String str in new[] { "Win2003Checked", "Win2003StdChecked", "Win2003EEChecked", "Win2003DCChecked" }) {
					OnPropertyChanged(str);
				}
			}
		}
		[XmlAttribute("w2k3s")]
		public Boolean Win2003StdChecked {
			get { return Win2003StdChecked1; }
			set {
				Win2003StdChecked1 = value;
				if (Win2003StdChecked1) {
					if (Win2003EEChecked && Win2003DCChecked) {
						Win2003Checked1 = true;
						OnPropertyChanged("Win2003Checked");
					}
				} else {
					Win2003Checked1 = false;
					OnPropertyChanged("Win2003Checked");
				}
				OnPropertyChanged("Win2003StdChecked");
			}
		}
		[XmlAttribute("w2k3e")]
		public Boolean Win2003EEChecked {
			get { return Win2003EEChecked1; }
			set {
				Win2003EEChecked1 = value;
				if (Win2003EEChecked1) {
					if (Win2003StdChecked && Win2003DCChecked) {
						Win2003Checked1 = true;
						OnPropertyChanged("Win2003Checked");
					}
				} else {
					Win2003Checked1 = false;
					OnPropertyChanged("Win2003Checked");
				}
				OnPropertyChanged("Win2003EEChecked");
			}
		}
		[XmlAttribute("w2k3d")]
		public Boolean Win2003DCChecked {
			get { return Win2003DCChecked1; }
			set {
				Win2003DCChecked1 = value;
				if (Win2003DCChecked1) {
					if (Win2003StdChecked && Win2003EEChecked) {
						Win2003Checked1 = true;
						OnPropertyChanged("Win2003Checked");
					}
				} else {
					Win2003Checked1 = false;
					OnPropertyChanged("Win2003Checked");
				}
				OnPropertyChanged("Win2003DCChecked");
			}
		}
		[XmlIgnore]
		public Boolean Win2008Checked {
			get { return Win2008Checked1; }
			set {
				Win2008Checked1 = value;
				if (Win2008Checked1) {
					Win2008StdChecked1 = Win2008EEChecked1 = Win2008DCChecked1 = true;
					foreach (String str in new[] { "Win2008StdChecked", "Win2008EEChecked", "Win2008DCChecked" }) {
						OnPropertyChanged(str);
					}
				} else {
					Win2008StdChecked1 = Win2008EEChecked1 = Win2008DCChecked1 = false;
				}
				foreach (String str in new[] { "Win2008Checked", "Win2008StdChecked", "Win2008EEChecked", "Win2008DCChecked" }) {
					OnPropertyChanged(str);
				}
			}
		}
		[XmlAttribute("w2k8s")]
		public Boolean Win2008StdChecked {
			get { return Win2008StdChecked1; }
			set {
				Win2008StdChecked1 = value;
				if (Win2008StdChecked1) {
					if (Win2008EEChecked && Win2008DCChecked) {
						Win2008Checked1 = true;
						OnPropertyChanged("Win2008Checked");
					}
				} else {
					Win2008Checked1 = false;
					OnPropertyChanged("Win2008Checked");
				}
				OnPropertyChanged("Win2008StdChecked");
			}
		}
		[XmlAttribute("w2k8e")]
		public Boolean Win2008EEChecked {
			get { return Win2008EEChecked1; }
			set {
				Win2008EEChecked1 = value;
				if (Win2008EEChecked1) {
					if (Win2008StdChecked && Win2008DCChecked) {
						Win2008Checked1 = true;
						OnPropertyChanged("Win2008Checked");
					}
				} else {
					Win2008Checked1 = false;
					OnPropertyChanged("Win2008Checked");
				}
				OnPropertyChanged("Win2008EEChecked");
			}
		}
		[XmlAttribute("w2k8d")]
		public Boolean Win2008DCChecked {
			get { return Win2008DCChecked1; }
			set {
				Win2008DCChecked1 = value;
				if (Win2008DCChecked1) {
					if (Win2008StdChecked && Win2008EEChecked) {
						Win2008Checked1 = true;
						OnPropertyChanged("Win2008Checked");
					}
				} else {
					Win2008Checked1 = false;
					OnPropertyChanged("Win2008Checked");
				}
				OnPropertyChanged("Win2008DCChecked");
			}
		}
		[XmlIgnore]
		public Boolean Win2008R2Checked {
			get { return Win2008R2Checked1; }
			set {
				Win2008R2Checked1 = value;
				if (Win2008R2Checked1) {
					Win2008R2StdChecked1 = Win2008R2EEChecked1 = Win2008R2DCChecked1 = true;
					foreach (String str in new[] { "Win2008R2StdChecked", "Win2008R2EEChecked", "Win2008R2DCChecked" }) {
						OnPropertyChanged(str);
					}
				} else {
					Win2008R2StdChecked1 = Win2008R2EEChecked1 = Win2008R2DCChecked1 = false;
				}
				foreach (String str in new[] { "Win2008R2Checked", "Win2008R2StdChecked", "Win2008R2EEChecked", "Win2008R2DCChecked" }) {
					OnPropertyChanged(str);
				}
			}
		}
		[XmlAttribute("w2k8r2s")]
		public Boolean Win2008R2StdChecked {
			get { return Win2008R2StdChecked1; }
			set {
				Win2008R2StdChecked1 = value;
				if (Win2008R2StdChecked1) {
					if (Win2008R2EEChecked && Win2008R2DCChecked) {
						Win2008R2Checked1 = true;
						OnPropertyChanged("Win2008R2Checked");
					}
				} else {
					Win2008R2Checked1 = false;
					OnPropertyChanged("Win2008R2Checked");
				}
				OnPropertyChanged("Win2008R2StdChecked");
			}
		}
		[XmlAttribute("w2k8r2e")]
		public Boolean Win2008R2EEChecked {
			get { return Win2008R2EEChecked1; }
			set {
				Win2008R2EEChecked1 = value;
				if (Win2008R2EEChecked1) {
					if (Win2008R2StdChecked && Win2008R2DCChecked) {
						Win2008R2Checked1 = true;
						OnPropertyChanged("Win2008R2Checked");
					}
				} else {
					Win2008R2Checked1 = false;
					OnPropertyChanged("Win2008R2Checked");
				}
				OnPropertyChanged("Win2008R2EEChecked");
			}
		}
		[XmlAttribute("w2k8r2d")]
		public Boolean Win2008R2DCChecked {
			get { return Win2008R2DCChecked1; }
			set {
				Win2008R2DCChecked1 = value;
				if (Win2008R2DCChecked1) {
					if (Win2008R2StdChecked && Win2008R2EEChecked) {
						Win2008R2Checked1 = true;
						OnPropertyChanged("Win2008R2Checked");
					}
				} else {
					Win2008R2Checked1 = false;
					OnPropertyChanged("Win2008R2Checked");
				}
				OnPropertyChanged("Win2008R2DCChecked");
			}
		}
		[XmlIgnore]
		public Boolean Win2012Checked {
			get { return Win2012Checked1; }
			set {
				Win2012Checked1 = value;
				if (Win2012Checked1) {
					Win2012StdChecked1 = Win2012DCChecked1 = true;
					foreach (String str in new[] { "Win2012StdChecked", "Win2012DCChecked" }) {
						OnPropertyChanged(str);
					}
				} else {
					Win2012StdChecked1 = Win2012DCChecked1 = false;
				}
				foreach (String str in new[] { "Win2012Checked", "Win2012StdChecked", "Win2012DCChecked" }) {
					OnPropertyChanged(str);
				}
			}
		}
		[XmlAttribute("w2k12s")]
		public Boolean Win2012StdChecked {
			get { return Win2012StdChecked1; }
			set {
				Win2012StdChecked1 = value;
				if (Win2012StdChecked1) {
					if (Win2012DCChecked) {
						Win2012Checked1 = true;
						OnPropertyChanged("Win2012Checked");
					}
				} else {
					Win2012Checked1 = false;
					OnPropertyChanged("Win2012Checked");
				}
				OnPropertyChanged("Win2012StdChecked");
			}
		}
		[XmlAttribute("w2k12d")]
		public Boolean Win2012DCChecked {
			get { return Win2012DCChecked1; }
			set {
				Win2012DCChecked1 = value;
				if (Win2012DCChecked1) {
					if (Win2012StdChecked) {
						Win2012Checked1 = true;
						OnPropertyChanged("Win2012Checked");
					}
				} else {
					Win2012Checked1 = false;
					OnPropertyChanged("Win2012Checked");
				}
				OnPropertyChanged("Win2012DCChecked");
			}
		}
		[XmlIgnore]
		public Boolean Win2012R2Checked {
			get { return Win2012R2Checked1; }
			set {
				Win2012R2Checked1 = value;
				if (Win2012R2Checked1) {
					Win2012R2StdChecked1 = Win2012R2DCChecked1 = true;
					foreach (String str in new[] { "Win2012R2StdChecked", "Win2012R2DCChecked" }) {
						OnPropertyChanged(str);
					}
				} else {
					Win2012R2StdChecked1 = Win2012R2DCChecked1 = false;
				}
				foreach (String str in new[] { "Win2012R2Checked", "Win2012R2StdChecked", "Win2012R2DCChecked" }) {
					OnPropertyChanged(str);
				}
			}
		}
		[XmlAttribute("w2k12r2s")]
		public Boolean Win2012R2StdChecked {
			get { return Win2012R2StdChecked1; }
			set {
				Win2012R2StdChecked1 = value;
				if (Win2012R2StdChecked1) {
					if (Win2012R2DCChecked) {
						Win2012R2Checked1 = true;
						OnPropertyChanged("Win2012R2Checked");
					}
				} else {
					Win2012R2Checked1 = false;
					OnPropertyChanged("Win2012R2Checked");
				}
				OnPropertyChanged("Win2012R2StdChecked");
			}
		}
		[XmlAttribute("w2k12r2d")]
		public Boolean Win2012R2DCChecked {
			get { return Win2012R2DCChecked1; }
			set {
				Win2012R2DCChecked1 = value;
				if (Win2012R2DCChecked1) {
					if (Win2012R2StdChecked) {
						Win2012R2Checked1 = true;
						OnPropertyChanged("Win2012R2Checked");
					}
				} else {
					Win2012R2Checked1 = false;
					OnPropertyChanged("Win2012R2Checked");
				}
				OnPropertyChanged("Win2012R2DCChecked");
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
