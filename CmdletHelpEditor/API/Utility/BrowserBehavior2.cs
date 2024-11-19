using System;
using System.Windows;
using Microsoft.Web.WebView2.Wpf;

namespace CmdletHelpEditor.API.Utility;
public class BrowserBehavior2 {
    public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
        "Html",
        typeof(String),
        typeof(BrowserBehavior2),
        new FrameworkPropertyMetadata(OnHtmlChanged));

    [AttachedPropertyBrowsableForType(typeof(WebView2))]
    public static String GetHtml(WebView2 browser) {
        return (String)browser.GetValue(HtmlProperty);
    }

    public static void SetHtml(WebView2 browser, String value) {
        browser.SetValue(HtmlProperty, value);
    }

    static async void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
        if (dependencyObject is WebView2 browser) {
            await browser.EnsureCoreWebView2Async();
            if (e.NewValue == null) {
                browser.NavigateToString("<br />");
            } else {
                browser.NavigateToString((String)e.NewValue);
            }
        }
    }
}
