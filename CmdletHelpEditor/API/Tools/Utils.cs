using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using CmdletHelpEditor.API.MetaWeblog;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.Properties;

namespace CmdletHelpEditor.API.Tools {
    static class Utils {
        public static String ArgPath { get; set; }
        public static Double CurrentFormatVersion => 1.1;

        public static IEnumerable<Double> SupportedFormatVersions => new[] { 0, 1.0, 1.1 };
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
        public static MessageBoxResult MsgBox(String header, String message, MessageBoxImage image = MessageBoxImage.Error, MessageBoxButton button = MessageBoxButton.OK) {
            WindowCollection windows = Application.Current.Windows;
            Window hwnd = null;
            if (windows.Count > 0) {
                hwnd = windows[windows.Count - 1];
            }
            return hwnd == null
                ? MessageBox.Show(message, header, button, image)
                : MessageBox.Show(hwnd, message, header, button, image);
        }
        public static String GetCommandTypes() {
            List<String> cmds = new List<String>();
            if (Settings.Default.FunctionChecked) { cmds.Add("Function"); }
            if (Settings.Default.FilterChecked) { cmds.Add("Filter"); }
            if (Settings.Default.CmdletChecked) { cmds.Add("Cmdlet"); }
            if (Settings.Default.ExternalScriptChecked) { cmds.Add("ExternalScript"); }
            if (Settings.Default.ScriptChecked) { cmds.Add("Script"); }
            if (PowerShellProcessor.PsVersion >= 3 && Settings.Default.WorkflowChecked) { cmds.Add("Workflow"); }
            if (PowerShellProcessor.PsVersion >= 4 && Settings.Default.ApplicationChecked) { cmds.Add("Application"); }
            try {
                return String.Join(",", cmds);
            } catch {
                return null;
            }
            
        }
        public static Blogger InitializeBlogger(ProviderInformation provInfo) {
            if (
                String.IsNullOrEmpty(provInfo.ProviderURL) ||
                String.IsNullOrEmpty(provInfo.UserName) ||
                provInfo.SecurePassword == null
            ) { return null; }
            Blogger blogger = new Blogger(provInfo.ProviderURL, provInfo.UserName, provInfo.SecurePassword);
            if (provInfo.Blog != null && !String.IsNullOrEmpty(provInfo.Blog.BlogID)) {
                blogger.SetBlog(provInfo.Blog.BlogID);
            }
            return blogger;
        }
    }
}
