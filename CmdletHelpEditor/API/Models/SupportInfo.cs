﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CmdletHelpEditor.API.Utility;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.Xml;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.Models;
// TODO: need to refactor entire class
public class SupportInfo : ViewModelBase {
    Boolean ad, rsat, ps2, ps3, ps4, ps5, ps51, ps60, ps61,
        wxp, wv, w7, w8, w81, w10, w11,
        w2k3s, w2k3e, w2k3d,
        w2k8s, w2k8e, w2k8d,
        w2k8r2s, w2k8r2e, w2k8r2d,
        w2k12s, w2k12d,
        w2k12r2s, w2k12r2d,
        w2k16s, w2k16d,
        w2k19s, w2k19d,
        w2k22s, w2k22d,
        w2k25s, w2k25d;

    public Boolean RequiresAD {
        get => ad;
        set {
            ad = value;
            OnPropertyChanged();
        }
    }
    public Boolean RequiresRSAT {
        get => rsat;
        set {
            rsat = value;
            OnPropertyChanged();
        }
    }
    [PSVersion(PsVersionSupport.Ps20, "Windows PowerShell 2.0")]
    public Boolean Ps2Checked {
        get => ps2;
        set {
            ps2 = value;
            OnPropertyChanged();
        }
    }
    [PSVersion(PsVersionSupport.Ps30, "Windows PowerShell 3.0")]
    public Boolean Ps3Checked {
        get => ps3;
        set {
            ps3 = value;
            OnPropertyChanged();
        }
    }
    [PSVersion(PsVersionSupport.Ps40, "Windows PowerShell 4.0")]
    public Boolean Ps4Checked {
        get => ps4;
        set {
            ps4 = value;
            OnPropertyChanged();
        }
    }
    [PSVersion(PsVersionSupport.Ps50, "Windows PowerShell 5.0")]
    public Boolean Ps5Checked {
        get => ps5;
        set {
            ps5 = value;
            OnPropertyChanged();
        }
    }
    [PSVersion(PsVersionSupport.Ps51, "Windows PowerShell 5.1")]
    public Boolean Ps51Checked {
        get => ps51;
        set {
            ps51 = value;
            OnPropertyChanged();
        }
    }
    [PSVersion(PsVersionSupport.Ps60, "PowerShell 6.0")]
    public Boolean Ps60Checked {
        get => ps60;
        set {
            ps60 = value;
            OnPropertyChanged();
        }
    }
    [PSVersion(PsVersionSupport.Ps61, "PowerShell 6.1")]
    public Boolean Ps61Checked {
        get => ps61;
        set {
            ps61 = value;
            OnPropertyChanged();
        }
    }
    [OSVersion(WinOsVersionSupport.WinXP, "Windows XP")]
    public Boolean WinXpChecked {
        get => wxp;
        set {
            wxp = value;
            OnPropertyChanged();
        }
    }
    [OSVersion(WinOsVersionSupport.WinVista, "Windows Vista")]
    public Boolean WinVistaChecked {
        get => wv;
        set {
            wv = value;
            OnPropertyChanged();
        }
    }
    [OSVersion(WinOsVersionSupport.Win7, "Windows 7")]
    public Boolean Win7Checked {
        get => w7;
        set {
            w7 = value;
            OnPropertyChanged();
        }
    }
    [OSVersion(WinOsVersionSupport.Win8, "Windows 8")]
    public Boolean Win8Checked {
        get => w8;
        set {
            w8 = value;
            OnPropertyChanged();
        }
    }
    [OSVersion(WinOsVersionSupport.Win81, "Windows 8.1")]
    public Boolean Win81Checked {
        get => w81;
        set {
            w81 = value;
            OnPropertyChanged();
        }
    }
    [OSVersion(WinOsVersionSupport.Win10, "Windows 10")]
    public Boolean Win10Checked {
        get => w10;
        set {
            w10 = value;
            OnPropertyChanged();
        }
    }
    [OSVersion(WinOsVersionSupport.Win11, "Windows 11")]
    public Boolean Win11Checked {
        get => w11;
        set {
            w11 = value;
            OnPropertyChanged();
        }
    }
    [OSVersion(WinOsVersionSupport.Win2003, "Windows Server 2003 all editions")]
    public Boolean Win2003Checked {
        get => w2k3s && w2k3e && w2k3d;
        set {
            w2k3s = w2k3e = w2k3d = value;
            raisePropertyChangedWithDependants(propertyNames:
                [nameof(Win2003Checked),
                    nameof(Win2003StdChecked),
                    nameof(Win2003EEChecked),
                    nameof(Win2003DCChecked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2003Std, "Windows Server 2003 Standard")]
    public Boolean Win2003StdChecked {
        get => w2k3s;
        set {
            w2k3s = value;
            raisePropertyChangedWithDependants(propertyNames: nameof(Win2003Checked));
        }
    }
    [OSVersion(WinOsVersionSupport.Win2003EE, "Windows Server 2003 Enterprise")]
    public Boolean Win2003EEChecked {
        get => w2k3e;
        set {
            w2k3e = value;
            raisePropertyChangedWithDependants(propertyNames: nameof(Win2003Checked));
        }
    }
    [OSVersion(WinOsVersionSupport.Win2003DC, "Windows Server 2003 Datacenter")]
    public Boolean Win2003DCChecked {
        get => w2k3d;
        set {
            w2k3d = value;
            raisePropertyChangedWithDependants(propertyNames: nameof(Win2003Checked));
        }
    }
    [OSVersion(WinOsVersionSupport.Win2008, "Windows Server 2008 all editions")]
    public Boolean Win2008Checked {
        get => w2k8s && w2k8e && w2k8d;
        set {
            w2k8s = w2k8e = w2k8d = value;
            raisePropertyChangedWithDependants(propertyNames:
                [nameof(Win2008Checked),
                    nameof(Win2008StdChecked),
                    nameof(Win2008EEChecked),
                    nameof(Win2008DCChecked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2008Std, "Windows Server 2008 Standard")]
    public Boolean Win2008StdChecked {
        get => w2k8s;
        set {
            w2k8s = value;
            raisePropertyChangedWithDependants(propertyNames: nameof(Win2008Checked));
        }
    }
    [OSVersion(WinOsVersionSupport.Win2008EE, "Windows Server 2008 Enterprise")]
    public Boolean Win2008EEChecked {
        get => w2k8e;
        set {
            w2k8e = value;
            raisePropertyChangedWithDependants(propertyNames: nameof(Win2008Checked));
        }
    }
    [OSVersion(WinOsVersionSupport.Win2008DC, "Windows Server 2008 Datacenter")]
    public Boolean Win2008DCChecked {
        get => w2k8d;
        set {
            w2k8d = value;
            raisePropertyChangedWithDependants(propertyNames: nameof(Win2008Checked));
        }
    }
    [OSVersion(WinOsVersionSupport.Win2008R2, "Windows Server 2008 R2 all editions")]
    public Boolean Win2008R2Checked {
        get => w2k8r2s && w2k8r2e && w2k8r2d;
        set {
            w2k8r2s = w2k8r2e = w2k8r2d = value;
            raisePropertyChangedWithDependants(propertyNames:
                [nameof(Win2008R2Checked),
                    nameof(Win2008R2StdChecked),
                    nameof(Win2008R2EEChecked),
                    nameof(Win2008R2DCChecked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2008R2Std, "Windows Server 2008 R2 Standard")]
    public Boolean Win2008R2StdChecked {
        get => w2k8r2s;
        set {
            w2k8r2s = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2008R2Checked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2008R2EE, "Windows Server 2008 R2 Enterprise")]
    public Boolean Win2008R2EEChecked {
        get => w2k8r2e;
        set {
            w2k8r2e = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2008R2Checked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2008R2DC, "Windows Server 2008 R2 Enterprise")]
    public Boolean Win2008R2DCChecked {
        get => w2k8r2d;
        set {
            w2k8r2d = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2008R2Checked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2012, "Windows Server 2012 all editions")]
    public Boolean Win2012Checked {
        get => w2k12s && w2k12d;
        set {
            w2k12s = w2k12d = value;
            raisePropertyChangedWithDependants(propertyNames:
                [nameof(Win2012Checked),
                    nameof(Win2012StdChecked),
                    nameof(Win2012DCChecked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2012Std, "Windows Server 2012 Standard")]
    public Boolean Win2012StdChecked {
        get => w2k12s;
        set {
            w2k12s = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2012Checked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2012DC, "Windows Server 2012 Datacenter")]
    public Boolean Win2012DCChecked {
        get => w2k12d;
        set {
            w2k12d = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2012Checked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2012R2, "Windows Server 2012 R2 all editions")]
    public Boolean Win2012R2Checked {
        get => w2k12r2s && w2k12r2d;
        set {
            w2k12r2s = w2k12r2d = value;
            raisePropertyChangedWithDependants(propertyNames:
                [nameof(Win2012R2Checked),
                    nameof(Win2012R2StdChecked),
                    nameof(Win2012R2DCChecked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2012R2Std, "Windows Server 2012 R2 Standard")]
    public Boolean Win2012R2StdChecked {
        get => w2k12r2s;
        set {
            w2k12r2s = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2012R2Checked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2012R2DC, "Windows Server 2012 R2 Datacenter")]
    public Boolean Win2012R2DCChecked {
        get => w2k12r2d;
        set {
            w2k12r2d = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2012R2Checked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2016, "Windows Server 2016 all editions")]
    public Boolean Win2016Checked {
        get => w2k16s && w2k16d;
        set {
            w2k16s = w2k16d = value;
            raisePropertyChangedWithDependants(propertyNames:
                [nameof(Win2016Checked),
                    nameof(Win2016StdChecked),
                    nameof(Win2016DCChecked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2016Std, "Windows Server 2016 Standard")]
    public Boolean Win2016StdChecked {
        get => w2k16s;
        set {
            w2k16s = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2016Checked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2016DC, "Windows Server 2016 Datacenter")]
    public Boolean Win2016DCChecked {
        get => w2k16d;
        set {
            w2k16d = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2016Checked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2019, "Windows Server 2019 all editions")]
    public Boolean Win2019Checked {
        get => w2k19s && w2k19d;
        set {
            w2k19s = w2k19d = value;
            raisePropertyChangedWithDependants(propertyNames:
                [nameof(Win2019Checked),
                    nameof(Win2019StdChecked),
                    nameof(Win2019DCChecked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2019Std, "Windows Server 2019 Standard")]
    public Boolean Win2019StdChecked {
        get => w2k19s;
        set {
            w2k19s = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2019Checked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2019DC, "Windows Server 2019 Datacenter")]
    public Boolean Win2019DCChecked {
        get => w2k19d;
        set {
            w2k19d = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2019Checked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2022, "Windows Server 2022 all editions")]
    public Boolean Win2022Checked {
        get => w2k22s && w2k22d;
        set {
            w2k22s = w2k22d = value;
            raisePropertyChangedWithDependants(propertyNames:
                [nameof(Win2022Checked),
                    nameof(Win2022StdChecked),
                    nameof(Win2022DCChecked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2022Std, "Windows Server 2022 Standard")]
    public Boolean Win2022StdChecked {
        get => w2k22s;
        set {
            w2k22s = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2022Checked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2022DC, "Windows Server 2022 Datacenter")]
    public Boolean Win2022DCChecked {
        get => w2k22d;
        set {
            w2k22d = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2022Checked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2025, "Windows Server 2025 all editions")]
    public Boolean Win2025Checked {
        get => w2k25s && w2k25d;
        set {
            w2k25s = w2k25d = value;
            raisePropertyChangedWithDependants(propertyNames:
                [nameof(Win2025Checked),
                    nameof(Win2025StdChecked),
                    nameof(Win2025DCChecked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2025Std, "Windows Server 2025 Standard")]
    public Boolean Win2025StdChecked {
        get => w2k25s;
        set {
            w2k25s = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2025Checked)]);
        }
    }
    [OSVersion(WinOsVersionSupport.Win2025DC, "Windows Server 2025 Datacenter")]
    public Boolean Win2025DCChecked {
        get => w2k25d;
        set {
            w2k25d = value;
            raisePropertyChangedWithDependants(propertyNames: [nameof(Win2025Checked)]);
        }
    }

    void raisePropertyChangedWithDependants([CallerMemberName] String? propertyName = null, params String[]? propertyNames) {
        if (propertyNames is not null) {
            foreach (String depPropertyName in propertyNames) {
                OnPropertyChanged(depPropertyName);
            }
        }
        OnPropertyChanged(propertyName);
    }

    public void SetPsVersion(PsVersionSupport psVersion) {
        _psAttributeMap[psVersion].SetValue(this, true);
    }
    public void SetWinOsVersion(WinOsVersionSupport winOsVersion) {
        var flags = Enum.GetValues(typeof(WinOsVersionSupport))
            .Cast<WinOsVersionSupport>()
            .Where(x => x != 0 && (x & winOsVersion) == x);
        foreach (WinOsVersionSupport enabledFlags in flags) {
            var propInfo = _osAttributeMap[enabledFlags];
            propInfo.SetValue(this, true);
        }
    }
    public XmlPsCommandSupportInfo ToXmlObject() {
        var retValue = new XmlPsCommandSupportInfo {
            RequiresAD = RequiresAD,
            RequiresRSAT = RequiresRSAT
        };
        var os = WinOsVersionSupport.None;
        foreach (PropertyInfo p in _osAttributeProps) {
            OSVersionAttribute attr = p.GetCustomAttribute<OSVersionAttribute>();
            if ((Boolean)p.GetValue(this)) {
                os |= attr.OsVersion;
            }
        }
        retValue.WinOsVersionAsInt = (Int32)os;
        var ps = PsVersionSupport.Ps20;
        foreach (PropertyInfo p in _psAttributeProps) {
            PSVersionAttribute attr = p.GetCustomAttribute<PSVersionAttribute>();
            if ((Boolean)p.GetValue(this)) {
                ps = attr.PsVersion;
                break;
            }
        }
        retValue.PsVersionAsInt = (Int32)ps;

        return retValue;
    }

    #region Reflection helpers
    // do some caching to minimize penalty hit imposed by reflection.
    static readonly List<PropertyInfo> _osAttributeProps = [];
    static readonly Dictionary<WinOsVersionSupport, PropertyInfo> _osAttributeMap = [];
    static readonly List<PropertyInfo> _psAttributeProps = [];
    static readonly Dictionary<PsVersionSupport, PropertyInfo> _psAttributeMap = [];

    static SupportInfo() {
        _osAttributeProps.AddRange(typeof(SupportInfo)
            .GetProperties()
            .Where(p => p.GetCustomAttribute<OSVersionAttribute>(true) is not null));
        foreach (PropertyInfo prop in _osAttributeProps) {
            var attr = prop.GetCustomAttribute<OSVersionAttribute>();
            _osAttributeMap[attr.OsVersion] = prop;
        }
        _psAttributeProps.AddRange(typeof(SupportInfo)
            .GetProperties()
            .Where(p => p.GetCustomAttribute<PSVersionAttribute>(true) is not null));
        foreach (PropertyInfo prop in _psAttributeProps) {
            var attr = prop.GetCustomAttribute<PSVersionAttribute>();
            _psAttributeMap[attr.PsVersion] = prop;
        }
    }

    #endregion
}
