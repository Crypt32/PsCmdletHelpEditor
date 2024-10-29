using System;
using System.Xml.Serialization;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.Xml;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.Models;
// TODO: need to refactor entire class
public class SupportInfo : ViewModelBase, IPsCommandSupportInfo {
    Boolean ad, rsat, ps2, ps3, ps4, ps5, ps51, ps60, ps61,
        wxp, wv, w7, w8, w81, w10, w11,
        w2k3, w2k3s, w2k3e, w2k3d,
        w2k8, w2k8s, w2k8e, w2k8d,
        w2k8r2, w2k8r2s, w2k8r2e, w2k8r2d,
        w2k12, w2k12s, w2k12d,
        w2k12r2, w2k12r2s, w2k12r2d,
        w2k16, w2k16s, w2k16d,
        w2k19, w2k19s, w2k19d,
        w2k22, w2k22s, w2k22d,
        w2k25, w2k25s, w2k25d;

    [XmlAttribute("minPsVersion")]
    public Int32 PsVersionAsInt {
        get => (Int32)PsVersion;
        set => PsVersion = (PsVersionSupport)value;
    }
    [XmlIgnore]
    public PsVersionSupport PsVersion { get; set; }
    [XmlAttribute("winOsSupport")]
    public Int32 WinOsVersionAsInt {
        get => (Int32)WinOsVersion;
        set => WinOsVersion = (WinOsVersionSupport)value;
    }
    [XmlIgnore]
    public WinOsVersionSupport WinOsVersion { get; set; }
    [XmlAttribute(nameof(ad))]
    public Boolean RequiresAD {
        get => ad;
        set {
            ad = value;
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(rsat))]
    public Boolean RequiresRSAT {
        get => rsat;
        set {
            rsat = value;
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(ps2))]
    public Boolean Ps2Checked {
        get => ps2;
        set {
            ps2 = value;
            if (ps2) {
                PsVersion = PsVersionSupport.Ps20;
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(ps3))]
    public Boolean Ps3Checked {
        get => ps3;
        set {
            ps3 = value;
            if (ps3) {
                PsVersion = PsVersionSupport.Ps30;
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(ps4))]
    public Boolean Ps4Checked {
        get => ps4;
        set {
            ps4 = value;
            if (ps4) {
                PsVersion = PsVersionSupport.Ps40;
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(ps5))]
    public Boolean Ps5Checked {
        get => ps5;
        set {
            ps5 = value;
            if (ps5) {
                PsVersion = PsVersionSupport.Ps50;
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(ps51))]
    public Boolean Ps51Checked {
        get => ps51;
        set {
            ps51 = value;
            if (ps51) {
                PsVersion = PsVersionSupport.Ps51;
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(ps60))]
    public Boolean Ps60Checked {
        get => ps60;
        set {
            ps60 = value;
            if (ps60) {
                PsVersion = PsVersionSupport.Ps60;
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(ps61))]
    public Boolean Ps61Checked {
        get => ps61;
        set {
            ps61 = value;
            if (ps61) {
                PsVersion = PsVersionSupport.Ps61;
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(wxp))]
    public Boolean WinXpChecked {
        get => wxp;
        set {
            wxp = value;
            if (wxp) {
                WinOsVersion |= WinOsVersionSupport.WinXP;
            } else {
                WinOsVersion &= ~WinOsVersionSupport.WinXP;
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(wv))]
    public Boolean WinVistaChecked {
        get => wv;
        set {
            wv = value;
            if (wv) {
                WinOsVersion |= WinOsVersionSupport.WinVista;
            } else {
                WinOsVersion &= ~WinOsVersionSupport.WinVista;
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w7))]
    public Boolean Win7Checked {
        get => w7;
        set {
            w7 = value;
            if (w7) {
                WinOsVersion |= WinOsVersionSupport.Win7;
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win7;
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w8))]
    public Boolean Win8Checked {
        get => w8;
        set {
            w8 = value;
            if (w8) {
                WinOsVersion |= WinOsVersionSupport.Win8;
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win8;
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w81))]
    public Boolean Win81Checked {
        get => w81;
        set {
            w81 = value;
            if (w81) {
                WinOsVersion |= WinOsVersionSupport.Win81;
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win81;
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w10))]
    public Boolean Win10Checked {
        get => w10;
        set {
            w10 = value;
            if (w10) {
                WinOsVersion |= WinOsVersionSupport.Win10;
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win10;
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w11))]
    public Boolean Win11Checked {
        get => w11;
        set {
            w11 = value;
            if (w11) {
                WinOsVersion |= WinOsVersionSupport.Win11;
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win11;
            }
            OnPropertyChanged();
        }
    }
    [XmlIgnore]
    public Boolean Win2003Checked {
        get => w2k3;
        set {
            w2k3 = value;
            if (w2k3) {
                WinOsVersion |= WinOsVersionSupport.Win2003;
                w2k3s = w2k3e = w2k3d = true;
                foreach (String str in new[] {
                                                  nameof(Win2003StdChecked),
                                                  nameof(Win2003EEChecked),
                                                  nameof(Win2003DCChecked)
                                              }) {
                    OnPropertyChanged(str);
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2003;
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
                WinOsVersion |= WinOsVersionSupport.Win2003Std;
                if (Win2003EEChecked && Win2003DCChecked) {
                    w2k3 = true;
                    OnPropertyChanged(nameof(Win2003Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2003Std;
                w2k3 = false;
                OnPropertyChanged(nameof(Win2003Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w2k3e))]
    public Boolean Win2003EEChecked {
        get => w2k3e;
        set {
            w2k3e = value;
            if (w2k3e) {
                WinOsVersion |= WinOsVersionSupport.Win2003EE;
                if (Win2003StdChecked && Win2003DCChecked) {
                    w2k3 = true;
                    OnPropertyChanged(nameof(Win2003Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2003EE;
                w2k3 = false;
                OnPropertyChanged(nameof(Win2003Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w2k3d))]
    public Boolean Win2003DCChecked {
        get => w2k3d;
        set {
            w2k3d = value;
            if (w2k3d) {
                WinOsVersion |= WinOsVersionSupport.Win2003DC;
                if (Win2003StdChecked && Win2003EEChecked) {
                    w2k3 = true;
                    OnPropertyChanged(nameof(Win2003Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2003DC;
                w2k3 = false;
                OnPropertyChanged(nameof(Win2003Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlIgnore]
    public Boolean Win2008Checked {
        get => w2k8;
        set {
            w2k8 = value;
            if (w2k8) {
                WinOsVersion |= WinOsVersionSupport.Win2008;
                w2k8s = w2k8e = w2k8d = true;
                foreach (String str in new[] {
                                                 nameof(Win2008StdChecked),
                                                 nameof(Win2008EEChecked),
                                                 nameof(Win2008DCChecked)
                                             }) {
                    OnPropertyChanged(str);
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008;
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
                WinOsVersion |= WinOsVersionSupport.Win2008Std;
                if (Win2008EEChecked && Win2008DCChecked) {
                    w2k8 = true;
                    OnPropertyChanged(nameof(Win2008Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008Std;
                w2k8 = false;
                OnPropertyChanged(nameof(Win2008Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w2k8e))]
    public Boolean Win2008EEChecked {
        get => w2k8e;
        set {
            w2k8e = value;
            if (w2k8e) {
                WinOsVersion |= WinOsVersionSupport.Win2008EE;
                if (Win2008StdChecked && Win2008DCChecked) {
                    w2k8 = true;
                    OnPropertyChanged(nameof(Win2008Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008EE;
                w2k8 = false;
                OnPropertyChanged(nameof(Win2008Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w2k8d))]
    public Boolean Win2008DCChecked {
        get => w2k8d;
        set {
            w2k8d = value;
            if (w2k8d) {
                WinOsVersion |= WinOsVersionSupport.Win2008DC;
                if (Win2008StdChecked && Win2008EEChecked) {
                    w2k8 = true;
                    OnPropertyChanged(nameof(Win2008Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008DC;
                w2k8 = false;
                OnPropertyChanged(nameof(Win2008Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlIgnore]
    public Boolean Win2008R2Checked {
        get => w2k8r2;
        set {
            w2k8r2 = value;
            if (w2k8r2) {
                WinOsVersion |= WinOsVersionSupport.Win2008R2;
                w2k8r2s = w2k8r2e = w2k8r2d = true;
                foreach (String str in new[] {
                                                 nameof(Win2008R2StdChecked),
                                                 nameof(Win2008R2EEChecked),
                                                 nameof(Win2008R2DCChecked)
                                             }) {
                    OnPropertyChanged(str);
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008R2;
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
                WinOsVersion |= WinOsVersionSupport.Win2008R2Std;
                if (Win2008R2EEChecked && Win2008R2DCChecked) {
                    w2k8r2 = true;
                    OnPropertyChanged(nameof(Win2008R2Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008R2Std;
                w2k8r2 = false;
                OnPropertyChanged(nameof(Win2008R2Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w2k8r2e))]
    public Boolean Win2008R2EEChecked {
        get => w2k8r2e;
        set {
            w2k8r2e = value;
            if (w2k8r2e) {
                WinOsVersion |= WinOsVersionSupport.Win2008R2EE;
                if (Win2008R2StdChecked && Win2008R2DCChecked) {
                    w2k8r2 = true;
                    OnPropertyChanged(nameof(Win2008R2Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008R2EE;
                w2k8r2 = false;
                OnPropertyChanged(nameof(Win2008R2Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w2k8r2d))]
    public Boolean Win2008R2DCChecked {
        get => w2k8r2d;
        set {
            w2k8r2d = value;
            if (w2k8r2d) {
                WinOsVersion |= WinOsVersionSupport.Win2008R2DC;
                if (Win2008R2StdChecked && Win2008R2EEChecked) {
                    w2k8r2 = true;
                    OnPropertyChanged(nameof(Win2008R2Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008R2DC;
                w2k8r2 = false;
                OnPropertyChanged(nameof(Win2008R2Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlIgnore]
    public Boolean Win2012Checked {
        get => w2k12;
        set {
            w2k12 = value;
            if (w2k12) {
                WinOsVersion |= WinOsVersionSupport.Win2012;
                w2k12s = w2k12d = true;
                foreach (String str in new[] {
                                                 nameof(Win2012StdChecked),
                                                 nameof(Win2012DCChecked)
                                             }) {
                    OnPropertyChanged(str);
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2012;
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
                WinOsVersion |= WinOsVersionSupport.Win2012Std;
                if (Win2012DCChecked) {
                    w2k12 = true;
                    OnPropertyChanged(nameof(Win2012Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2012Std;
                w2k12 = false;
                OnPropertyChanged(nameof(Win2012Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w2k12d))]
    public Boolean Win2012DCChecked {
        get => w2k12d;
        set {
            w2k12d = value;
            if (w2k12d) {
                WinOsVersion |= WinOsVersionSupport.Win2012DC;
                if (Win2012StdChecked) {
                    w2k12 = true;
                    OnPropertyChanged(nameof(Win2012Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2012DC;
                w2k12 = false;
                OnPropertyChanged(nameof(Win2012Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlIgnore]
    public Boolean Win2012R2Checked {
        get => w2k12r2;
        set {
            w2k12r2 = value;
            if (w2k12r2) {
                WinOsVersion |= WinOsVersionSupport.Win2012R2;
                w2k12r2s = w2k12r2d = true;
                foreach (String str in new[] {
                                                 nameof(Win2012R2StdChecked),
                                                 nameof(Win2012R2DCChecked)
                                             }) {
                    OnPropertyChanged(str);
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2012R2;
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
                WinOsVersion |= WinOsVersionSupport.Win2012R2Std;
                if (Win2012R2DCChecked) {
                    w2k12r2 = true;
                    OnPropertyChanged(nameof(Win2012R2Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2012R2Std;
                w2k12r2 = false;
                OnPropertyChanged(nameof(Win2012R2Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w2k12r2d))]
    public Boolean Win2012R2DCChecked {
        get => w2k12r2d;
        set {
            w2k12r2d = value;
            if (w2k12r2d) {
                WinOsVersion |= WinOsVersionSupport.Win2012R2DC;
                if (Win2012R2StdChecked) {
                    w2k12r2 = true;
                    OnPropertyChanged(nameof(Win2012R2Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2012R2DC;
                w2k12r2 = false;
                OnPropertyChanged(nameof(Win2012R2Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlIgnore]
    public Boolean Win2016Checked {
        get => w2k16;
        set {
            w2k16 = value;
            if (w2k16) {
                WinOsVersion |= WinOsVersionSupport.Win2016;
                w2k16s = w2k16d = true;
                foreach (String str in new[] {
                                                 nameof(Win2016StdChecked),
                                                 nameof(Win2016DCChecked)
                                             }) {
                    OnPropertyChanged(str);
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2016;
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
                WinOsVersion |= WinOsVersionSupport.Win2016Std;
                if (Win2016DCChecked) {
                    w2k16 = true;
                    OnPropertyChanged(nameof(Win2016Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2016Std;
                w2k16 = false;
                OnPropertyChanged(nameof(Win2016Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w2k16d))]
    public Boolean Win2016DCChecked {
        get => w2k16d;
        set {
            w2k16d = value;
            if (w2k16d) {
                WinOsVersion |= WinOsVersionSupport.Win2016DC;
                if (Win2016StdChecked) {
                    w2k16 = true;
                    OnPropertyChanged(nameof(Win2016Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2016DC;
                w2k16 = false;
                OnPropertyChanged(nameof(Win2016Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlIgnore]
    public Boolean Win2019Checked {
        get => w2k19;
        set {
            w2k19 = value;
            if (w2k19) {
                WinOsVersion |= WinOsVersionSupport.Win2019;
                w2k19s = w2k19d = true;
                foreach (String str in new[] {
                                                 nameof(Win2019StdChecked),
                                                 nameof(Win2019DCChecked)
                                             }) {
                    OnPropertyChanged(str);
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2019;
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
                WinOsVersion |= WinOsVersionSupport.Win2019Std;
                if (Win2019DCChecked) {
                    w2k19 = true;
                    OnPropertyChanged(nameof(Win2019Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2019Std;
                w2k19 = false;
                OnPropertyChanged(nameof(Win2019Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w2k19d))]
    public Boolean Win2019DCChecked {
        get => w2k19d;
        set {
            w2k19d = value;
            if (w2k19d) {
                WinOsVersion |= WinOsVersionSupport.Win2019DC;
                if (Win2019StdChecked) {
                    w2k19 = true;
                    OnPropertyChanged(nameof(Win2019Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2019DC;
                w2k19 = false;
                OnPropertyChanged(nameof(Win2019Checked));
            }
            OnPropertyChanged();
        }
    }

    [XmlIgnore]
    public Boolean Win2022Checked {
        get => w2k22;
        set {
            w2k22 = value;
            if (w2k22) {
                WinOsVersion |= WinOsVersionSupport.Win2022;
                w2k22s = w2k22d = true;
                foreach (String str in new[] {
                                                 nameof(Win2022StdChecked),
                                                 nameof(Win2022DCChecked)
                                             }) {
                    OnPropertyChanged(str);
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2022;
                w2k22s = w2k22d = false;
            }
            foreach (String str in new[] {
                                             nameof(Win2022Checked),
                                             nameof(Win2022StdChecked),
                                             nameof(Win2022DCChecked)
                                         }) {
                OnPropertyChanged(str);
            }
        }
    }
    [XmlAttribute(nameof(w2k22s))]
    public Boolean Win2022StdChecked {
        get => w2k22s;
        set {
            w2k22s = value;
            if (w2k22s) {
                WinOsVersion |= WinOsVersionSupport.Win2022Std;
                if (Win2022DCChecked) {
                    w2k22 = true;
                    OnPropertyChanged(nameof(Win2022Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2022Std;
                w2k22 = false;
                OnPropertyChanged(nameof(Win2022Checked));
            }
            OnPropertyChanged();
        }
    }
    [XmlAttribute(nameof(w2k22d))]
    public Boolean Win2022DCChecked {
        get => w2k22d;
        set {
            w2k22d = value;
            if (w2k22d) {
                WinOsVersion |= WinOsVersionSupport.Win2022DC;
                if (Win2022StdChecked) {
                    w2k22 = true;
                    OnPropertyChanged(nameof(Win2022Checked));
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2022DC;
                w2k22 = false;
                OnPropertyChanged(nameof(Win2022Checked));
            }
            OnPropertyChanged();
        }
    }

    public void SetWinOsVersion(WinOsVersionSupport winOsVersion) {
        WinOsVersion = winOsVersion;
    }
    public XmlPsCommandSupportInfo ToXmlObject() {
        var retValue = new XmlPsCommandSupportInfo {
            PsVersionAsInt = PsVersionAsInt,
            RequiresAD = RequiresAD,
            RequiresRSAT = RequiresRSAT
        };
        var os = WinOsVersionSupport.None;
        if (wxp) {
            os |= WinOsVersionSupport.WinXP;
        }
        if (wv) {
            os |= WinOsVersionSupport.WinVista;
        }
        if (w7) {
            os |= WinOsVersionSupport.Win7;
        }
        if (w8) {
            os |= WinOsVersionSupport.Win8;
        }
        if (w81) {
            os |= WinOsVersionSupport.Win81;
        }
        if (w10) {
            os |= WinOsVersionSupport.Win10;
        }
        if (w11) {
            os |= WinOsVersionSupport.Win11;
        }
        if (w2k3s) {
            os |= WinOsVersionSupport.Win2003Std;
        }
        if (w2k3e) {
            os |= WinOsVersionSupport.Win2003EE;
        }
        if (w2k3d) {
            os |= WinOsVersionSupport.Win2003DC;
        }
        if (w2k8s) {
            os |= WinOsVersionSupport.Win2008Std;
        }
        if (w2k8e) {
            os |= WinOsVersionSupport.Win2008EE;
        }
        if (w2k8d) {
            os |= WinOsVersionSupport.Win2008DC;
        }
        if (w2k8r2s) {
            os |= WinOsVersionSupport.Win2008R2Std;
        }
        if (w2k8r2e) {
            os |= WinOsVersionSupport.Win2008R2EE;
        }
        if (w2k8r2d) {
            os |= WinOsVersionSupport.Win2008R2DC;
        }
        if (w2k12s) {
            os |= WinOsVersionSupport.Win2012Std;
        }
        if (w2k12d) {
            os |= WinOsVersionSupport.Win2012DC;
        }
        if (w2k12r2s) {
            os |= WinOsVersionSupport.Win2012R2Std;
        }
        if (w2k12r2d) {
            os |= WinOsVersionSupport.Win2012R2DC;
        }
        if (w2k16s) {
            os |= WinOsVersionSupport.Win2016Std;
        }
        if (w2k16d) {
            os |= WinOsVersionSupport.Win2016DC;
        }
        if (w2k19s) {
            os |= WinOsVersionSupport.Win2019Std;
        }
        if (w2k19d) {
            os |= WinOsVersionSupport.Win2019DC;
        }
        if (w2k22s) {
            os |= WinOsVersionSupport.Win2022Std;
        }
        if (w2k22d) {
            os |= WinOsVersionSupport.Win2022DC;
        }
        if (w2k25s) {
            os |= WinOsVersionSupport.Win2025Std;
        }
        if (w2k25d) {
            os |= WinOsVersionSupport.Win2025DC;
        }
        retValue.WinOsVersionAsInt = (Int32)os;

        return retValue;
    }
}
