﻿using System;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.Xml;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.Models;

public class PsCommandExampleVM : ViewModelBase, IPsCommandExample {
    String name, cmd, description, output;
    readonly Int32 _uid;

    public PsCommandExampleVM() {
        _uid = Guid.NewGuid().GetHashCode();
    }
    public String Name {
        get => name ?? String.Empty;
        set {
            if (name != value) {
                name = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
    }
    public String Output {
        get => output ?? String.Empty;
        set {
            if (output != value) {
                output = value;
                OnPropertyChanged();
            }
        }
    }

    public XmlPsCommandExample ToXmlObject() {
        return new XmlPsCommandExample {
            Name = Name,
            Cmd = Cmd,
            Description = Description,
            Output = Output
        };
    }

    public static PsCommandExampleVM FromCommandInfo(IPsCommandExample example) {
        return new PsCommandExampleVM {
            Name = example.Name,
            Cmd = example.Cmd,
            Description = example.Description,
            Output = example.Output,
        };
    }

    #region Equals

    public override Boolean Equals(Object obj) {
        return !ReferenceEquals(null, obj) &&
               (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((PsCommandExampleVM)obj));
    }

    protected Boolean Equals(PsCommandExampleVM other) {
        return _uid == other._uid;
    }

    public override Int32 GetHashCode() {
        unchecked {
            return _uid.GetHashCode() * 397;
        }
    }

    #endregion
}
