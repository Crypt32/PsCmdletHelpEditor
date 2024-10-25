using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Xml.Serialization;

namespace PsCmdletHelpEditor.Core.Models.Xml;

[XmlRoot("ModuleObject")]
[XmlInclude(typeof(XmlPsCommand))]
public class XmlPsModuleProject : IPsModuleProject {
    [XmlAttribute("fVersion")]
    public Double FormatVersion { get; set; }
    public String Name { get; set; }
    [XmlAttribute("type")]
    public ModuleType ModuleType { get; set; }
    public String Version { get; set; }
    public String Description { get; set; }
    [XmlAttribute("mclass")]
    public String ModuleClass { get; set; }
    public String ModulePath { get; set; }
    [XmlAttribute("useSupports")]
    public Boolean UseSupports { get; set; }
    public Boolean HasManifest { get; set; }
    public XmlRpcProviderInformation? Provider { get; set; }
    public Boolean OverridePostCount { get; set; }
    public Int32? FetchPostCount { get; set; }
    public String ExtraHeader { get; set; }
    public String ExtraFooter { get; set; }
    // editor
    [XmlArrayItem("CmdletObject")]
    public List<XmlPsCommand> Cmdlets { get; set; }
    [XmlIgnore]
    public String ProjectPath { get; set; }


    public IReadOnlyList<IPsCommandInfo> GetCmdlets() {
        return Cmdlets;
    }
    public IXmlRpcProviderInformation? GetXmlRpcProviderInfo() {
        return Provider;
    }
}

public class XmlRpcProviderInformation : IXmlRpcProviderInformation {
    public String ProviderName { get; set; }
    public String ProviderURL { get; set; }
    public BlogInfo Blog { get; set; }
    public String UserName { get; set; }
    public String Password { get; set; }
    public Int32 FetchPostCount { get; set; }
}

[XmlInclude(typeof(XmlCommandParameterSet))]
[XmlInclude(typeof(XmlPsCommandParameterDescription))]
[XmlInclude(typeof(XmlPsExample))]
[XmlInclude(typeof(XmlPsRelatedLink))]
public class XmlPsCommand : IPsCommandInfo {
    public String Name { get; set; }
    [XmlAttribute("verb")]
    public String? Verb { get; set; }
    [XmlAttribute("noun")]
    public String? Noun { get; set; }
    public List<String> Syntax { get; set; }
    public XmlPsGeneralDescription GeneralHelp { get; set; }
    [XmlArrayItem("CommandParameterSetInfo2")]
    public List<XmlCommandParameterSet> ParamSets { get; set; } = [];
    [XmlArrayItem("ParameterDescription")]
    public List<XmlPsCommandParameterDescription> Parameters { get; set; } = [];
    [XmlArrayItem("Example")]
    public List<XmlPsExample> Examples { get; set; } = [];
    [XmlArrayItem("RelatedLink")]
    public List<XmlPsRelatedLink> RelatedLinks { get; set; } = [];
    public XmlPsCommandSupportInfo? SupportInformation { get; set; }
    public String? ExtraHeader { get; set; }
    public String? ExtraFooter { get; set; }
    public Boolean Publish { get; set; }
    public String? URL { get; set; }
    public String? ArticleIDString { get; set; }

    public IPsCommandGeneralDescription GetDescription() {
        return GeneralHelp;
    }
    public IPsCommandSupportInfo? GetSupportInfo() {
        return SupportInformation;
    }
    public IReadOnlyList<IPsCommandParameterSetInfo> GetParameterSets() {
        return ParamSets;
    }
    public IReadOnlyList<IPsCommandParameterDescription> GetParameters() {
        return Parameters;
    }
    public IReadOnlyList<IPsCommandExample> GetExamples() {
        return Examples;
    }
    public IReadOnlyList<IPsCommandRelatedLink> GetRelatedLinks() {
        return RelatedLinks;
    }
}

public class XmlPsGeneralDescription : IPsCommandGeneralDescription {
    public String? Synopsis { get; set; }
    public String? Description { get; set; }
    public String Notes { get; set; }
    public String? InputType { get; set; }
    public String? InputUrl { get; set; }
    public String? ReturnType { get; set; }
    public String? ReturnUrl { get; set; }
    public String? InputTypeDescription { get; set; }
    public String? ReturnTypeDescription { get; set; }
}

public class XmlCommandParameterSet : IPsCommandParameterSetInfo {
    [XmlAttribute]
    public String Name { get; set; }
    [XmlAttribute("Params")]
    public List<String> Parameters { get; set; }

    public IReadOnlyList<String> GetParameters() {
        return Parameters;
    }
}

public class XmlPsCommandParameterDescription : IPsCommandParameterDescription {
    public String Name { get; set; } = null!;
    [XmlAttribute("type")]
    public String Type { get; set; } = null!;
    [XmlAttribute("varLen")]
    public Boolean AcceptsArray { get; set; }
    [XmlAttribute("required")]
    public Boolean Mandatory { get; set; }
    [XmlAttribute("dynamic")]
    public Boolean Dynamic { get; set; }
    [XmlAttribute("pipeRemaining")]
    public Boolean RemainingArgs { get; set; }
    [XmlAttribute("pipe")]
    public Boolean Pipeline { get; set; }
    [XmlAttribute("pipeProp")]
    public Boolean PipelinePropertyName { get; set; }
    [XmlAttribute("globbing")]
    public Boolean Globbing { get; set; }
    [XmlAttribute("isPos")]
    public Boolean Positional { get; set; }
    [XmlAttribute("pos")]
    public String? Position { get; set; }
    [XmlIgnore]
    public PsCommandParameterOption Options { get; set; }
    public String? Description { get; set; } = null!;
    public String? DefaultValue { get; set; }

    public List<String> Attributes { get; set; } = [];
    public List<String> Aliases { get; set; } = [];

    public IReadOnlyList<String> GetAttributes() {
        return Attributes;
    }
    public IReadOnlyList<String> GetAliases() {
        return Aliases;
    }
}

public class XmlPsRelatedLink : IPsCommandRelatedLink {
    public String? LinkText { get; set; } = null!;
    public String? LinkUrl { get; set; }
}

public class XmlPsExample : IPsCommandExample {
    public String Name { get; set; } = null!;
    public String? Cmd { get; set; } = null!;
    public String? Description { get; set; }
    public String? Output { get; set; }
}

public class XmlPsCommandSupportInfo : IPsCommandSupportInfo {
    Boolean ps2, ps3, ps4, ps5, ps51, ps60, ps61,
        wxp, wv, w7, w8, w81, w10, w11,
        w2k3, w2k3s, w2k3e, w2k3d,
        w2k8, w2k8s, w2k8e, w2k8d,
        w2k8r2, w2k8r2s, w2k8r2e, w2k8r2d,
        w2k12, w2k12s, w2k12d,
        w2k12r2, w2k12r2s, w2k12r2d,
        w2k16, w2k16s, w2k16d,
        w2k19, w2k19s, w2k19d,
        w2k22, w2k22s, w2k22d;

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
    [XmlAttribute("ad")]
    public Boolean ADChecked { get; set; }
    [XmlAttribute("rsat")]
    public Boolean RsatChecked { get; set; }
    [XmlAttribute(nameof(ps2))]
    public Boolean Ps2Checked {
        get => ps2;
        set {
            ps2 = value;
            if (ps2) {
                PsVersion = PsVersionSupport.Ps20;
            }
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
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2003;
                w2k3s = w2k3e = w2k3d = false;
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2003Std;
                w2k3 = false;
            }
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2003EE;
                w2k3 = false;
            }
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2003DC;
                w2k3 = false;
            }
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
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008;
                w2k8s = w2k8e = w2k8d = false;
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008Std;
                w2k8 = false;
            }
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008EE;
                w2k8 = false;
            }
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008DC;
                w2k8 = false;
            }
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
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008R2;
                w2k8r2s = w2k8r2e = w2k8r2d = false;
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008R2Std;
                w2k8r2 = false;
            }
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008R2EE;
                w2k8r2 = false;
            }
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2008R2DC;
                w2k8r2 = false;
            }
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
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2012;
                w2k12s = w2k12d = false;
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2012Std;
                w2k12 = false;
            }
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2012DC;
                w2k12 = false;
            }
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
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2012R2;
                w2k12r2s = w2k12r2d = false;
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2012R2Std;
                w2k12r2 = false;
            }
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2012R2DC;
                w2k12r2 = false;
            }
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
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2016;
                w2k16s = w2k16d = false;
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2016Std;
                w2k16 = false;
            }
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2016DC;
                w2k16 = false;
            }
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
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2019;
                w2k19s = w2k19d = false;
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2019Std;
                w2k19 = false;
            }
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2019DC;
                w2k19 = false;
            }
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
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2022;
                w2k22s = w2k22d = false;
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2022Std;
                w2k22 = false;
            }
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
                }
            } else {
                WinOsVersion &= ~WinOsVersionSupport.Win2022DC;
                w2k22 = false;
            }
        }
    }
}