using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PsCmdletHelpEditor.BLL.Models;
using PsCmdletHelpEditor.XmlRpc;

namespace PsCmdletHelpEditor.BLL.Tools {
    public static class Utils {
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
                    ProviderName = "XML-RPC"
                }
            };
        }
        public static IXmlRpcClient InitializeBlogger(ProviderInformation provInfo) {
            if (
                String.IsNullOrEmpty(provInfo.ProviderURL) ||
                String.IsNullOrEmpty(provInfo.UserName) ||
                provInfo.SecurePassword == null
            ) { return null; }
            var prov = new XmlRpcProviderInfo(provInfo.ProviderURL, provInfo.UserName, provInfo.SecurePassword);
            if (provInfo.Blog != null && !String.IsNullOrEmpty(provInfo.Blog.BlogID)) {
                prov.ProviderID = provInfo.Blog.BlogID;
            }
            var blogger = new XmlRpcClient(prov);

            return blogger;
        }
    }
}
