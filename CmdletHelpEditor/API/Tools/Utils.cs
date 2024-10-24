using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.Properties;
using PsCmdletHelpEditor.Core.Services;
using PsCmdletHelpEditor.XmlRpc;
using Unity;

namespace CmdletHelpEditor.API.Tools;
static class Utils {
    public static Double CurrentFormatVersion => 1.1;

    public static IEnumerable<Double> SupportedFormatVersions => [0, 1.0, 1.1];
    public static IEnumerable<ProviderInformation> EnumProviders() {
        return new ObservableCollection<ProviderInformation> {
            new ProviderInformation {
                ProviderName = "CodePlex",
                ProviderURL = "https://www.codeplex.com/site/metaweblog"
            },
            new ProviderInformation {
                ProviderName = "Custom"
            }
        };
    }
    public static String GetCommandTypes() {
        var psProcessor = App.Container.Resolve<IPowerShellProcessor>();
        List<String> commandTypes = new List<String>();
        if (Settings.Default.FunctionChecked) { commandTypes.Add("Function"); }
        if (Settings.Default.FilterChecked) { commandTypes.Add("Filter"); }
        if (Settings.Default.CmdletChecked) { commandTypes.Add("Cmdlet"); }
        if (Settings.Default.ExternalScriptChecked) { commandTypes.Add("ExternalScript"); }
        if (Settings.Default.ScriptChecked) { commandTypes.Add("Script"); }
        if (psProcessor.PsVersion >= 3 && Settings.Default.WorkflowChecked) { commandTypes.Add("Workflow"); }
        if (psProcessor.PsVersion >= 4 && Settings.Default.ApplicationChecked) { commandTypes.Add("Application"); }
        try {
            return String.Join(",", commandTypes);
        } catch {
            return null;
        }
        
    }
    public static WpXmlRpcClient InitializeBlogger(ProviderInformation provInfo) {
        if (
            String.IsNullOrEmpty(provInfo.ProviderURL) ||
            String.IsNullOrEmpty(provInfo.UserName) ||
            provInfo.SecurePassword == null
        ) { return null; }
        var xProvInfo = new XmlRpcProviderInfo(provInfo.ProviderURL, provInfo.UserName, provInfo.SecurePassword);
        var blogger = new WpXmlRpcClient(xProvInfo);
        xProvInfo.ProviderID = provInfo.Blog?.BlogID;
        return blogger;
    }
}
