using System.ComponentModel.DataAnnotations;

namespace PsCmdletHelpEditor.Core.Models;

public enum PsVersionSupport {
    [Display(Name = "Windows PowerShell 2.0")]
    Ps20 = 0,
    [Display(Name = "Windows PowerShell 3.0")]
    Ps30 = 1,
    [Display(Name = "Windows PowerShell 4.0")]
    Ps40 = 2,
    [Display(Name = "Windows PowerShell 5.0")]
    Ps50 = 3,
    [Display(Name = "Windows PowerShell 5.1")]
    Ps51 = 4,
    [Display(Name = "PowerShell 6.0")]
    Ps60 = 5,
    [Display(Name = "PowerShell 6.1")]
    Ps61 = 6,
    [Display(Name = "PowerShell 6.2")]
    Ps62 = 7,
    [Display(Name = "PowerShell 7.0")]
    Ps70 = 8,
    [Display(Name = "PowerShell 7.1")]
    Ps71 = 9,
    [Display(Name = "PowerShell 7.2")]
    Ps72 = 10,
    [Display(Name = "PowerShell 7.3")]
    Ps73 = 11,
    [Display(Name = "PowerShell 7.4")]
    Ps74 = 12,
}