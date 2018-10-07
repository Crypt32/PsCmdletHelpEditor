﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Xml;

namespace CmdletHelpEditor.UI.Dialogs {
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    sealed partial class AboutBox {
        /// <summary>
        /// Default constructor is protected so callers must use one with a parent.
        /// </summary>
        AboutBox() {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor that takes a parent for this AboutBox dialog.
        /// </summary>
        /// <param name="parent">Parent window for this dialog.</param>
        public AboutBox(Window parent) : this() {
            Owner = parent;
        }

        /// <summary>
        /// Handles click navigation on the hyperlink in the About dialog.
        /// </summary>
        /// <param name="sender">Object the sent the event.</param>
        /// <param name="e">Navigation events arguments.</param>
        void HyperlinkRequestNavigate(Object sender, RequestNavigateEventArgs e) {
            if (e.Uri != null && String.IsNullOrEmpty(e.Uri.OriginalString) == false) {
                String uri = e.Uri.AbsoluteUri;
                Process.Start(new ProcessStartInfo(uri));
                e.Handled = true;
            }
        }

        XmlDocument xmlDoc;

        const String propertyNameTitle = "Title";
        const String propertyNameDescription = "Description";
        const String propertyNameProduct = "Product";
        const String propertyNameCopyright = "Copyright";
        const String propertyNameCompany = "Company";
        const String xPathRoot = "ApplicationInfo/";
        const String xPathTitle = xPathRoot + propertyNameTitle;
        const String xPathVersion = xPathRoot + "Version";
        const String xPathDescription = xPathRoot + propertyNameDescription;
        const String xPathProduct = xPathRoot + propertyNameProduct;
        const String xPathCopyright = xPathRoot + propertyNameCopyright;
        const String xPathCompany = xPathRoot + propertyNameCompany;
        const String xPathLink = xPathRoot + "Link";
        const String xPathLinkUri = xPathRoot + "Link/@Uri";

        #region Properties
        /// <summary>
        /// Gets the title property, which is display in the About dialogs window title.
        /// </summary>
        public String ProductTitle {
            get {
                String result = CalculatePropertyValue<AssemblyTitleAttribute>(propertyNameTitle, xPathTitle);
                if (String.IsNullOrEmpty(result)) {
                    // otherwise, just get the name of the assembly itself.
                    result = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the application's version information to show.
        /// </summary>
        public String Version {
            get {
                // first, try to get the version string from the assembly.
                Version v = Assembly.GetExecutingAssembly().GetName().Version;
                String result = v != null ? v.ToString() : GetLogicalResourceString(xPathVersion);
                return result;
            }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public String Description {
            get {
                String desc = CalculatePropertyValue<AssemblyDescriptionAttribute>(propertyNameDescription, xPathDescription);
                desc += Environment.NewLine + Environment.NewLine;
                desc += "Credits:" +Environment.NewLine;
                desc += "    Wassim Fayed (Microsoft) — for original idea;" +Environment.NewLine;
                desc += "    Sergey Zwezdin — for WPF consultation;" + Environment.NewLine;
                desc += "    Ali Badereddin — for XML syntax highlighter;";
                return desc;
            }
        }

        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        public String Product => CalculatePropertyValue<AssemblyProductAttribute>(propertyNameProduct, xPathProduct);

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public String Copyright => CalculatePropertyValue<AssemblyCopyrightAttribute>(propertyNameCopyright, xPathCopyright);

        /// <summary>
        /// Gets the product's company name.
        /// </summary>
        public String Company => CalculatePropertyValue<AssemblyCompanyAttribute>(propertyNameCompany, xPathCompany);

        /// <summary>
        /// Gets the link text to display in the About dialog.
        /// </summary>
        public String LinkText => GetLogicalResourceString(xPathLink);

        /// <summary>
        /// Gets the link uri that is the navigation target of the link.
        /// </summary>
        public String LinkUri => GetLogicalResourceString(xPathLinkUri);

        #endregion

        #region Resource location methods
        /// <summary>
        /// Gets the specified property value either from a specific attribute, or from a resource dictionary.
        /// </summary>
        /// <typeparam name="T">Attribute type that we're trying to retrieve.</typeparam>
        /// <param name="propertyName">Property name to use on the attribute.</param>
        /// <param name="xpathQuery">XPath to the element in the XML data resource.</param>
        /// <returns>The resulting string to use for a property.
        /// Returns null if no data could be retrieved.</returns>
        String CalculatePropertyValue<T>(String propertyName, String xpathQuery) {
            String result = String.Empty;
            // first, try to get the property value from an attribute.
            Object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            if (attributes.Length > 0) {
                T attrib = (T)attributes[0];
                PropertyInfo property = attrib.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property != null) {
                    result = property.GetValue(attributes[0], null) as String;
                }
            }

            // if the attribute wasn't found or it did not have a value, then look in an xml resource.
            if (result == String.Empty) {
                // if that fails, try to get it from a resource.
                result = GetLogicalResourceString(xpathQuery);
            }
            return result;
        }

        /// <summary>
        /// Gets the XmlDataProvider's document from the resource dictionary.
        /// </summary>
        XmlDocument ResourceXmlDocument {
            get {
                if (xmlDoc == null) {
                    // if we haven't already found the resource XmlDocument, then try to find it.
                    XmlDataProvider provider = TryFindResource("aboutProvider") as XmlDataProvider;
                    if (provider != null) {
                        // save away the XmlDocument, so we don't have to get it multiple times.
                        xmlDoc = provider.Document;
                    }
                }
                return xmlDoc;
            }
        }

        /// <summary>
        /// Gets the specified data element from the XmlDataProvider in the resource dictionary.
        /// </summary>
        /// <param name="xpathQuery">An XPath query to the XML element to retrieve.</param>
        /// <returns>The resulting string value for the specified XML element. 
        /// Returns empty string if resource element couldn't be found.</returns>
        String GetLogicalResourceString(String xpathQuery) {
            String result = String.Empty;
            // get the About xml information from the resources.
            XmlDocument doc = ResourceXmlDocument;
            // if we found the XmlDocument, then look for the specified data. 
            XmlNode node = doc?.SelectSingleNode(xpathQuery);
            if (node != null) {
                result = node is XmlAttribute ? node.Value : node.InnerText;
            }
            return result;
        }
        #endregion

        void OkButton_OnClick(Object Sender, RoutedEventArgs E) {
            Close();
        }
    }
}
