using System;
using PsCmdletHelpEditor.Core.Models;
using PsCmdletHelpEditor.Core.Models.Xml;
using SysadminsLV.WPF.OfficeTheme.Toolkit.ViewModels;

namespace CmdletHelpEditor.API.Models;
public class PsCommandGeneralDescriptionVM : ViewModelBase {
    String synopsis, description, notes, inputType, inputUrl, inputDescription, returnType, returnUrl, returnDescription;
    ItemStatus status = ItemStatus.New;

    public String Synopsis {
        get => synopsis ?? String.Empty;
        set {
            if (synopsis != value) {
                synopsis = value;
                status = ItemStatus.Incomplete;
                OnPropertyChanged();
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
                OnPropertyChanged();
                OnPropertyChanged(nameof(Status));
            }
        }
    }
    public String Notes {
        get => notes ?? String.Empty;
        set {
            if (notes != value) {
                notes = value;
                OnPropertyChanged();
            }
        }
    }
    public String InputType {
        get => inputType ?? String.Empty;
        set {
            if (inputType != value) {
                inputType = value;
                OnPropertyChanged();
            }
        }
    }
    public String InputUrl {
        get => inputUrl ?? String.Empty;
        set {
            if (inputUrl != value) {
                inputUrl = value;
                OnPropertyChanged();
            }
        }
    }
    public String InputTypeDescription {
        get => inputDescription ?? String.Empty;
        set {
            if (inputDescription != value) {
                inputDescription = value;
                OnPropertyChanged();
            }
        }
    }
    public String ReturnType {
        get => returnType ?? String.Empty;
        set {
            if (returnType != value) {
                returnType = value;
                OnPropertyChanged();
            }
        }
    }
    public String ReturnUrl {
        get => returnUrl ?? String.Empty;
        set {
            if (returnUrl != value) {
                returnUrl = value;
                OnPropertyChanged();
            }
        }
    }
    public String ReturnTypeDescription {
        get => returnDescription ?? String.Empty;
        set {
            if (returnDescription != value) {
                returnDescription = value;
                OnPropertyChanged();
            }
        }
    }
    public ItemStatus Status {
        get {
            if (status is ItemStatus.Missing or ItemStatus.New) {
                return status;
            }
            return status = String.IsNullOrEmpty(Description) || String.IsNullOrEmpty(Synopsis)
                            ? ItemStatus.Incomplete
                            : ItemStatus.Valid;
        }
        set {
            status = value;
            OnPropertyChanged();
        }
    }

    public void ImportFromCommandInfo(IPsCommandGeneralDescription generalDescription) {
        Synopsis = generalDescription.Synopsis;
        Description = generalDescription.Description;
        Notes = generalDescription.Notes;
        InputType = generalDescription.InputType;
        InputUrl = generalDescription.InputUrl;
        InputTypeDescription = generalDescription.InputTypeDescription;
        ReturnType = generalDescription.ReturnType;
        ReturnUrl = generalDescription.ReturnUrl;
        ReturnTypeDescription = generalDescription.ReturnTypeDescription;
    }

    public XmlPsCommandGeneralDescription ToXmlObject() {
        return new XmlPsCommandGeneralDescription {
            Synopsis = Synopsis,
            Description = Description,
            Notes = Notes,
            InputType = InputType,
            InputUrl = InputUrl,
            InputTypeDescription = InputTypeDescription,
            ReturnType = ReturnType,
            ReturnUrl = ReturnUrl,
            ReturnTypeDescription = ReturnTypeDescription,
        };
    }
}
