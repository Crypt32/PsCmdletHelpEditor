using System;
using System.Threading.Tasks;
using PsCmdletHelpEditor.Core.Models;

namespace PsCmdletHelpEditor.Core.Services.Formatters;

public interface IHelpOutputFormatter {
    String GenerateView(IPsCommandInfo cmdlet, IPsModuleProject moduleObject);
    Task<String> GenerateViewAsync(IPsCommandInfo cmdlet, IPsModuleProject moduleObject);
}