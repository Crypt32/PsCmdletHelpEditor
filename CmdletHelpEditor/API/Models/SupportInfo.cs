using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CmdletHelpEditor.API.Models {
    public class SupportInfo : INotifyPropertyChanged {
        Boolean ad, rsat, ps2, ps3, ps4, ps5, ps51, ps60, ps61,
            wxp, wv, w7, w8, w81, w10,
            w2k3, w2k3s, w2k3e, w2k3d,
            w2k8, w2k8s, w2k8e, w2k8d,
            w2k8r2, w2k8r2s, w2k8r2e, w2k8r2d,
            w2k12, w2k12s, w2k12d,
            w2k12r2, w2k12r2s, w2k12r2d,
            w2k16, w2k16s, w2k16d,
            w2k19, w2k19s, w2k19d;

        [XmlAttribute(nameof(ad))]
        public Boolean ADChecked {
            get => ad;
            set {
                ad = value;
                OnPropertyChanged(nameof(ADChecked));
            }
        }
        [XmlAttribute(nameof(rsat))]
        public Boolean RsatChecked {
            get => rsat;
            set {
                rsat = value;
                OnPropertyChanged(nameof(RsatChecked));
            }
        }
        [XmlAttribute(nameof(ps2))]
        public Boolean Ps2Checked {
            get => ps2;
            set {
                ps2 = value;
                OnPropertyChanged(nameof(Ps2Checked));
            }
        }
        [XmlAttribute(nameof(ps3))]
        public Boolean Ps3Checked {
            get => ps3;
            set {
                ps3 = value;
                OnPropertyChanged(nameof(Ps3Checked));
            }
        }
        [XmlAttribute(nameof(ps4))]
        public Boolean Ps4Checked {
            get => ps4;
            set {
                ps4 = value;
                OnPropertyChanged(nameof(Ps4Checked));
            }
        }
        [XmlAttribute(nameof(ps5))]
        public Boolean Ps5Checked {
            get => ps5;
            set {
                ps5 = value;
                OnPropertyChanged(nameof(Ps5Checked));
            }
        }
        [XmlAttribute(nameof(ps51))]
        public Boolean Ps51Checked {
            get => ps51;
            set {
                ps51 = value;
                OnPropertyChanged(nameof(Ps51Checked));
            }
        }
        [XmlAttribute(nameof(ps60))]
        public Boolean Ps60Checked {
            get => ps60;
            set {
                ps60 = value;
                OnPropertyChanged(nameof(Ps60Checked));
            }
        }
        [XmlAttribute(nameof(ps61))]
        public Boolean Ps61Checked {
            get => ps61;
            set {
                ps61 = value;
                OnPropertyChanged(nameof(Ps61Checked));
            }
        }
        [XmlAttribute(nameof(wxp))]
        public Boolean WinXpChecked {
            get => wxp;
            set {
                wxp = value;
                OnPropertyChanged(nameof(WinXpChecked));
            }
        }
        [XmlAttribute(nameof(wv))]
        public Boolean WinVistaChecked {
            get => wv;
            set {
                wv = value;
                OnPropertyChanged(nameof(WinVistaChecked));
            }
        }
        [XmlAttribute(nameof(w7))]
        public Boolean Win7Checked {
            get => w7;
            set {
                w7 = value;
                OnPropertyChanged(nameof(Win7Checked));
            }
        }
        [XmlAttribute(nameof(w8))]
        public Boolean Win8Checked {
            get => w8;
            set {
                w8 = value;
                OnPropertyChanged(nameof(Win8Checked));
            }
        }
        [XmlAttribute(nameof(w81))]
        public Boolean Win81Checked {
            get => w81;
            set {
                w81 = value;
                OnPropertyChanged(nameof(Win81Checked));
            }
        }
        [XmlAttribute(nameof(w10))]
        public Boolean Win10Checked {
            get => w10;
            set {
                w10 = value;
                OnPropertyChanged(nameof(Win10Checked));
            }
        }
        [XmlIgnore]
        public Boolean Win2003Checked {
            get => w2k3;
            set {
                w2k3 = value;
                if (w2k3) {
                    w2k3s = w2k3e = w2k3d = true;
                    foreach (String str in new[] {
                                                      nameof(Win2003StdChecked),
                                                      nameof(Win2003EEChecked),
                                                      nameof(Win2003DCChecked)
                                                  }) {
                        OnPropertyChanged(str);
                    }
                } else {
                    w2k3s = w2k3e = w2k3d = false;
                }
                foreach (String str in new[] {
                                                 nameof(Win2003Checked),
                                                 nameof(Win2003StdChecked),
                                                 nameof(Win2003EEChecked),
                                                 nameof(Win2003DCChecked)
                                             }) {
                    OnPropertyChanged(str);
                }
            }
        }
        [XmlAttribute(nameof(w2k3s))]
        public Boolean Win2003StdChecked {
            get => w2k3s;
            set {
                w2k3s = value;
                if (w2k3s) {
                    if (Win2003EEChecked && Win2003DCChecked) {
                        w2k3 = true;
                        OnPropertyChanged(nameof(Win2003Checked));
                    }
                } else {
                    w2k3 = false;
                    OnPropertyChanged(nameof(Win2003Checked));
                }
                OnPropertyChanged(nameof(Win2003StdChecked));
            }
        }
        [XmlAttribute(nameof(w2k3e))]
        public Boolean Win2003EEChecked {
            get => w2k3e;
            set {
                w2k3e = value;
                if (w2k3e) {
                    if (Win2003StdChecked && Win2003DCChecked) {
                        w2k3 = true;
                        OnPropertyChanged(nameof(Win2003Checked));
                    }
                } else {
                    w2k3 = false;
                    OnPropertyChanged(nameof(Win2003Checked));
                }
                OnPropertyChanged(nameof(Win2003EEChecked));
            }
        }
        [XmlAttribute(nameof(w2k3d))]
        public Boolean Win2003DCChecked {
            get => w2k3d;
            set {
                w2k3d = value;
                if (w2k3d) {
                    if (Win2003StdChecked && Win2003EEChecked) {
                        w2k3 = true;
                        OnPropertyChanged(nameof(Win2003Checked));
                    }
                } else {
                    w2k3 = false;
                    OnPropertyChanged(nameof(Win2003Checked));
                }
                OnPropertyChanged(nameof(Win2003DCChecked));
            }
        }
        [XmlIgnore]
        public Boolean Win2008Checked {
            get => w2k8;
            set {
                w2k8 = value;
                if (w2k8) {
                    w2k8s = w2k8e = w2k8d = true;
                    foreach (String str in new[] {
                                                     nameof(Win2008StdChecked),
                                                     nameof(Win2008EEChecked),
                                                     nameof(Win2008DCChecked)
                                                 }) {
                        OnPropertyChanged(str);
                    }
                } else {
                    w2k8s = w2k8e = w2k8d = false;
                }
                foreach (String str in new[] {
                                                 nameof(Win2008Checked),
                                                 nameof(Win2008StdChecked),
                                                 nameof(Win2008EEChecked),
                                                 nameof(Win2008DCChecked)
                                             }) {
                    OnPropertyChanged(str);
                }
            }
        }
        [XmlAttribute(nameof(w2k8s))]
        public Boolean Win2008StdChecked {
            get => w2k8s;
            set {
                w2k8s = value;
                if (w2k8s) {
                    if (Win2008EEChecked && Win2008DCChecked) {
                        w2k8 = true;
                        OnPropertyChanged(nameof(Win2008Checked));
                    }
                } else {
                    w2k8 = false;
                    OnPropertyChanged(nameof(Win2008Checked));
                }
                OnPropertyChanged(nameof(Win2008StdChecked));
            }
        }
        [XmlAttribute(nameof(w2k8e))]
        public Boolean Win2008EEChecked {
            get => w2k8e;
            set {
                w2k8e = value;
                if (w2k8e) {
                    if (Win2008StdChecked && Win2008DCChecked) {
                        w2k8 = true;
                        OnPropertyChanged(nameof(Win2008Checked));
                    }
                } else {
                    w2k8 = false;
                    OnPropertyChanged(nameof(Win2008Checked));
                }
                OnPropertyChanged(nameof(Win2008EEChecked));
            }
        }
        [XmlAttribute(nameof(w2k8d))]
        public Boolean Win2008DCChecked {
            get => w2k8d;
            set {
                w2k8d = value;
                if (w2k8d) {
                    if (Win2008StdChecked && Win2008EEChecked) {
                        w2k8 = true;
                        OnPropertyChanged(nameof(Win2008Checked));
                    }
                } else {
                    w2k8 = false;
                    OnPropertyChanged(nameof(Win2008Checked));
                }
                OnPropertyChanged(nameof(Win2008DCChecked));
            }
        }
        [XmlIgnore]
        public Boolean Win2008R2Checked {
            get => w2k8r2;
            set {
                w2k8r2 = value;
                if (w2k8r2) {
                    w2k8r2s = w2k8r2e = w2k8r2d = true;
                    foreach (String str in new[] {
                                                     nameof(Win2008R2StdChecked),
                                                     nameof(Win2008R2EEChecked),
                                                     nameof(Win2008R2DCChecked)
                                                 }) {
                        OnPropertyChanged(str);
                    }
                } else {
                    w2k8r2s = w2k8r2e = w2k8r2d = false;
                }
                foreach (String str in new[] {
                                                 nameof(Win2008R2Checked),
                                                 nameof(Win2008R2StdChecked),
                                                 nameof(Win2008R2EEChecked),
                                                 nameof(Win2008R2DCChecked)
                                             }) {
                    OnPropertyChanged(str);
                }
            }
        }
        [XmlAttribute(nameof(w2k8r2s))]
        public Boolean Win2008R2StdChecked {
            get => w2k8r2s;
            set {
                w2k8r2s = value;
                if (w2k8r2s) {
                    if (Win2008R2EEChecked && Win2008R2DCChecked) {
                        w2k8r2 = true;
                        OnPropertyChanged(nameof(Win2008R2Checked));
                    }
                } else {
                    w2k8r2 = false;
                    OnPropertyChanged(nameof(Win2008R2Checked));
                }
                OnPropertyChanged(nameof(Win2008R2StdChecked));
            }
        }
        [XmlAttribute(nameof(w2k8r2e))]
        public Boolean Win2008R2EEChecked {
            get => w2k8r2e;
            set {
                w2k8r2e = value;
                if (w2k8r2e) {
                    if (Win2008R2StdChecked && Win2008R2DCChecked) {
                        w2k8r2 = true;
                        OnPropertyChanged(nameof(Win2008R2Checked));
                    }
                } else {
                    w2k8r2 = false;
                    OnPropertyChanged(nameof(Win2008R2Checked));
                }
                OnPropertyChanged(nameof(Win2008R2EEChecked));
            }
        }
        [XmlAttribute(nameof(w2k8r2d))]
        public Boolean Win2008R2DCChecked {
            get => w2k8r2d;
            set {
                w2k8r2d = value;
                if (w2k8r2d) {
                    if (Win2008R2StdChecked && Win2008R2EEChecked) {
                        w2k8r2 = true;
                        OnPropertyChanged(nameof(Win2008R2Checked));
                    }
                } else {
                    w2k8r2 = false;
                    OnPropertyChanged(nameof(Win2008R2Checked));
                }
                OnPropertyChanged(nameof(Win2008R2DCChecked));
            }
        }
        [XmlIgnore]
        public Boolean Win2012Checked {
            get => w2k12;
            set {
                w2k12 = value;
                if (w2k12) {
                    w2k12s = w2k12d = true;
                    foreach (String str in new[] {
                                                     nameof(Win2012StdChecked),
                                                     nameof(Win2012DCChecked)
                                                 }) {
                        OnPropertyChanged(str);
                    }
                } else {
                    w2k12s = w2k12d = false;
                }
                foreach (String str in new[] {
                                                 nameof(Win2012Checked),
                                                 nameof(Win2012StdChecked),
                                                 nameof(Win2012DCChecked)
                                             }) {
                    OnPropertyChanged(str);
                }
            }
        }
        [XmlAttribute(nameof(w2k12s))]
        public Boolean Win2012StdChecked {
            get => w2k12s;
            set {
                w2k12s = value;
                if (w2k12s) {
                    if (Win2012DCChecked) {
                        w2k12 = true;
                        OnPropertyChanged(nameof(Win2012Checked));
                    }
                } else {
                    w2k12 = false;
                    OnPropertyChanged(nameof(Win2012Checked));
                }
                OnPropertyChanged(nameof(Win2012StdChecked));
            }
        }
        [XmlAttribute(nameof(w2k12d))]
        public Boolean Win2012DCChecked {
            get => w2k12d;
            set {
                w2k12d = value;
                if (w2k12d) {
                    if (Win2012StdChecked) {
                        w2k12 = true;
                        OnPropertyChanged(nameof(Win2012Checked));
                    }
                } else {
                    w2k12 = false;
                    OnPropertyChanged(nameof(Win2012Checked));
                }
                OnPropertyChanged(nameof(Win2012DCChecked));
            }
        }
        [XmlIgnore]
        public Boolean Win2012R2Checked {
            get => w2k12r2;
            set {
                w2k12r2 = value;
                if (w2k12r2) {
                    w2k12r2s = w2k12r2d = true;
                    foreach (String str in new[] {
                                                     nameof(Win2012R2StdChecked),
                                                     nameof(Win2012R2DCChecked)
                                                 }) {
                        OnPropertyChanged(str);
                    }
                } else {
                    w2k12r2s = w2k12r2d = false;
                }
                foreach (String str in new[] {
                                                 nameof(Win2012R2Checked),
                                                 nameof(Win2012R2StdChecked),
                                                 nameof(Win2012R2DCChecked)
                                             }) {
                    OnPropertyChanged(str);
                }
            }
        }
        [XmlAttribute(nameof(w2k12r2s))]
        public Boolean Win2012R2StdChecked {
            get => w2k12r2s;
            set {
                w2k12r2s = value;
                if (w2k12r2s) {
                    if (Win2012R2DCChecked) {
                        w2k12r2 = true;
                        OnPropertyChanged(nameof(Win2012R2Checked));
                    }
                } else {
                    w2k12r2 = false;
                    OnPropertyChanged(nameof(Win2012R2Checked));
                }
                OnPropertyChanged(nameof(Win2012R2StdChecked));
            }
        }
        [XmlAttribute(nameof(w2k12r2d))]
        public Boolean Win2012R2DCChecked {
            get => w2k12r2d;
            set {
                w2k12r2d = value;
                if (w2k12r2d) {
                    if (Win2012R2StdChecked) {
                        w2k12r2 = true;
                        OnPropertyChanged(nameof(Win2012R2Checked));
                    }
                } else {
                    w2k12r2 = false;
                    OnPropertyChanged(nameof(Win2012R2Checked));
                }
                OnPropertyChanged(nameof(Win2012R2DCChecked));
            }
        }
        [XmlIgnore]
        public Boolean Win2016Checked {
            get => w2k16;
            set {
                w2k16 = value;
                if (w2k16) {
                    w2k16s = w2k16d = true;
                    foreach (String str in new[] {
                                                     nameof(Win2016StdChecked),
                                                     nameof(Win2016DCChecked)
                                                 }) {
                        OnPropertyChanged(str);
                    }
                } else {
                    w2k16s = w2k16d = false;
                }
                foreach (String str in new[] {
                                                 nameof(Win2016Checked),
                                                 nameof(Win2016StdChecked),
                                                 nameof(Win2016DCChecked)
                                             }) {
                    OnPropertyChanged(str);
                }
            }
        }
        [XmlAttribute(nameof(w2k16s))]
        public Boolean Win2016StdChecked {
            get => w2k16s;
            set {
                w2k16s = value;
                if (w2k16s) {
                    if (Win2016DCChecked) {
                        w2k16 = true;
                        OnPropertyChanged(nameof(Win2016Checked));
                    }
                } else {
                    w2k16 = false;
                    OnPropertyChanged(nameof(Win2016Checked));
                }
                OnPropertyChanged(nameof(Win2016StdChecked));
            }
        }
        [XmlAttribute(nameof(w2k16d))]
        public Boolean Win2016DCChecked {
            get => w2k16d;
            set {
                w2k16d = value;
                if (w2k16d) {
                    if (Win2016StdChecked) {
                        w2k16 = true;
                        OnPropertyChanged(nameof(Win2016Checked));
                    }
                } else {
                    w2k16 = false;
                    OnPropertyChanged(nameof(Win2016Checked));
                }
                OnPropertyChanged(nameof(Win2016DCChecked));
            }
        }
        [XmlIgnore]
        public Boolean Win2019Checked {
            get => w2k19;
            set {
                w2k19 = value;
                if (w2k19) {
                    w2k19s = w2k19d = true;
                    foreach (String str in new[] {
                                                     nameof(Win2019StdChecked),
                                                     nameof(Win2019DCChecked)
                                                 }) {
                        OnPropertyChanged(str);
                    }
                } else {
                    w2k19s = w2k19d = false;
                }
                foreach (String str in new[] {
                                                 nameof(Win2019Checked),
                                                 nameof(Win2019StdChecked),
                                                 nameof(Win2019DCChecked)
                                             }) {
                    OnPropertyChanged(str);
                }
            }
        }
        [XmlAttribute(nameof(w2k19s))]
        public Boolean Win2019StdChecked {
            get => w2k19s;
            set {
                w2k19s = value;
                if (w2k19s) {
                    if (Win2019DCChecked) {
                        w2k19 = true;
                        OnPropertyChanged(nameof(Win2019Checked));
                    }
                } else {
                    w2k19 = false;
                    OnPropertyChanged(nameof(Win2019Checked));
                }
                OnPropertyChanged(nameof(Win2019StdChecked));
            }
        }
        [XmlAttribute(nameof(w2k19d))]
        public Boolean Win2019DCChecked {
            get => w2k19d;
            set {
                w2k19d = value;
                if (w2k19d) {
                    if (Win2019StdChecked) {
                        w2k19 = true;
                        OnPropertyChanged(nameof(Win2019Checked));
                    }
                } else {
                    w2k19 = false;
                    OnPropertyChanged(nameof(Win2019Checked));
                }
                OnPropertyChanged(nameof(Win2019DCChecked));
            }
        }

        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
