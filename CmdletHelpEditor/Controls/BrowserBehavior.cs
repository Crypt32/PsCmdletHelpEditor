using System;
using System.Windows;
using System.Windows.Controls;

namespace CmdletHelpEditor.Controls;
public class BrowserBehavior {
    public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
        "Html", typeof(String), typeof(BrowserBehavior), new FrameworkPropertyMetadata(OnHtmlChanged));

    [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
    public static String GetHtml(WebBrowser browser) {
        return (String)browser.GetValue(HtmlProperty);
    }

    public static void SetHtml(WebBrowser browser, String value) {
        browser.SetValue(HtmlProperty, value);
    }

    static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
        if (dependencyObject is WebBrowser browser) {
            if (e.NewValue == null) {
                browser.NavigateToString("<br />");
            } else {
                browser.NavigateToString((String)e.NewValue);
            }
        }
    }
}
