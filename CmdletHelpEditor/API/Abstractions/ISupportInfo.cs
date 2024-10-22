﻿using PsCmdletHelpEditor.Core.Models;
using System;

namespace CmdletHelpEditor.API.Abstractions;
public interface ISupportInfo {
    PsVersionSupport PsVersion { get; set; }
    WinOsVersionSupport WinOsVersion { get; set; }
    Boolean ADChecked { get; set; }
    Boolean RsatChecked { get; set; }
}