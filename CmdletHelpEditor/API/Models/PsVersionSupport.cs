using System;
using System.ComponentModel.DataAnnotations;

namespace CmdletHelpEditor.API.Models {
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

    [Flags]
    public enum WinOsVersionSupport {
        [Display(Name = "None")]
        None         = 0,
        [Display(Name = "Windows XP")]
        WinXP        = 1,
        [Display(Name = "Windows Server 2003 Standard")]
        Win2003Std   = 2,
        [Display(Name = "Windows Server 2003 Enterprise")]
        Win2003EE    = 4,
        [Display(Name = "Windows Server 2003 Datacenter")]
        Win2003DC    = 8,
        [Display(Name = "Windows Server 2003 Family")]
        Win2003      = Win2003Std | Win2003EE | Win2003DC,
        [Display(Name = "Windows Vista")]
        WinVista     = 0x10,
        [Display(Name = "Windows Server 2008 Standard")]
        Win2008Std   = 0x20,
        [Display(Name = "Windows Server 2008 Enterprise")]
        Win2008EE    = 0x40,
        [Display(Name = "Windows Server 2008 Datacenter")]
        Win2008DC    = 0x80,
        [Display(Name = "Windows Server 2008 Family")]
        Win2008      = Win2008Std | Win2008EE | Win2008DC,
        [Display(Name = "Windows 7")]
        Win7         = 0x100,
        [Display(Name = "Windows Server 2008 R2 Standard")]
        Win2008R2Std = 0x200,
        [Display(Name = "Windows Server 2008 R2 Enterprise")]
        Win2008R2EE  = 0x400,
        [Display(Name = "Windows Server 2008 R2 Datacenter")]
        Win2008R2DC  = 0x800,
        [Display(Name = "Windows Server 2008 R2 Family")]
        Win2008R2    = Win2008R2Std | Win2008R2EE | Win2008R2DC,
        [Display(Name = "Windows 8")]
        Win8         = 0x1000,
        [Display(Name = "Windows Server 2012 Standard")]
        Win2012Std   = 0x2000,
        [Display(Name = "Windows Server 2012 Datacenter")]
        Win2012DC    = 0x4000,
        [Display(Name = "Windows Server 2012 Family")]
        Win2012      = Win2012Std | Win2012DC,
        [Display(Name = "Windows 8.1")]
        Win81        = 0x8000,
        [Display(Name = "Windows Server 2012 R2 Standard")]
        Win2012R2Std = 0x10000,
        [Display(Name = "Windows Server 2012 R2 Datacenter")]
        Win2012R2DC  = 0x20000,
        [Display(Name = "Windows Server 2012 R2 Family")]
        Win2012R2    = Win2012R2Std | Win2012R2DC,
        [Display(Name = "Windows 10")]
        Win10        = 0x40000,
        [Display(Name = "Windows Server 2016 Standard")]
        Win2016Std   = 0x80000,
        [Display(Name = "Windows Server 2016 Datacenter")]
        Win2016DC    = 0x100000,
        [Display(Name = "Windows Server 2016 Family")]
        Win2016      = Win2016Std | Win2016DC,
        [Display(Name = "Windows Server 2019 Standard")]
        Win2019Std   = 0x200000,
        [Display(Name = "Windows Server 2019 Datacenter")]
        Win2019DC    = 0x400000,
        [Display(Name = "Windows Server 2019 Family")]
        Win2019      = Win2019Std | Win2019DC,
        [Display(Name = "Windows 11")]
        Win11        = 0x800000,
        [Display(Name = "Windows Server 2022 Standard")]
        Win2022Std   = 0x1000000,
        [Display(Name = "Windows Server 2022 Datacenter")]
        Win2022DC    = 0x2000000,
        [Display(Name = "Windows Server 2022 Family")]
        Win2022      = Win2022Std | Win2022DC,
    }
}