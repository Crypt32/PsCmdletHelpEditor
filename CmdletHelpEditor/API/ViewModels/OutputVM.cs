using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using Markdig;
using PsCmdletHelpEditor.Core.Services.Formatters;
using PsCmdletHelpEditor.Core.Services.MAML;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using Unity;

namespace CmdletHelpEditor.API.ViewModels;

public class OutputVM : DependencyObject, INotifyPropertyChanged {
    static readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    readonly IMsgBox _msgBox;
    FlowDocument document;
    Boolean xmlChecked, htmlSourceChecked, mdSourceChecked, htmlWebViewChecked, textChecked, isBusy, mdWebViewChecked;

    public OutputVM(ModuleObject parent) {
        _msgBox = App.Container.Resolve<IMsgBox>();
        XmlChecked = true;
        Tab = parent;
        GenerateOutputCommand = new AsyncCommand(generateOutput, canGenerateView);
    }

    public IAsyncCommand GenerateOutputCommand { get; }

    public ModuleObject Tab { get; }

    // dependency property is required for HtmlText property
    public static readonly DependencyProperty HtmlTextProperty = DependencyProperty.Register(
        nameof(HtmlText),
        typeof(String),
        typeof(OutputVM),
        new PropertyMetadata("<br />"));

    public String HtmlText {
        get => (String)GetValue(HtmlTextProperty);
        set => SetValue(HtmlTextProperty, value);
    }
    public Boolean IsBusy {
        get => isBusy;
        set {
            isBusy = value;
            OnPropertyChanged();
        }
    }
    public Boolean XmlChecked {
        get => xmlChecked;
        set {
            xmlChecked = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(RtbChecked));
            OnPropertyChanged(nameof(WebViewChecked));
        }
    }
    public Boolean HtmlSourceChecked {
        get => htmlSourceChecked;
        set {
            htmlSourceChecked = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(RtbChecked));
            OnPropertyChanged(nameof(WebViewChecked));
        }
    }
    public Boolean MdSourceChecked {
        get => mdSourceChecked;
        set {
            mdSourceChecked = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(RtbChecked));
            OnPropertyChanged(nameof(WebViewChecked));
        }
    }
    public Boolean HtmlWebViewChecked {
        get => htmlWebViewChecked;
        set {
            htmlWebViewChecked = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(RtbChecked));
            OnPropertyChanged(nameof(WebViewChecked));
        }
    }
    public Boolean MdWebViewChecked {
        get => mdWebViewChecked;
        set {
            mdWebViewChecked = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(RtbChecked));
            OnPropertyChanged(nameof(WebViewChecked));
        }
    }
    public Boolean TextChecked {
        get => textChecked;
        set {
            textChecked = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(RtbChecked));
            OnPropertyChanged(nameof(WebViewChecked));
        }
    }
    public Boolean RtbChecked => XmlChecked || HtmlSourceChecked || MdSourceChecked;
    public Boolean WebViewChecked => HtmlWebViewChecked || MdWebViewChecked;

    public FlowDocument Document {
        get => document;
        set {
            document = value;
            OnPropertyChanged();
        }
    }

    async Task generateOutput(Object obj, CancellationToken cancellationToken) {
        CmdletObject cmd = Tab.SelectedCmdlet;
        if (cmd == null) { return; }
        if (XmlChecked && Tab.UpgradeRequired) {
            _msgBox.ShowWarning("Warning", "The module is offline and requires upgrade. Upgrade the project to allow XML view.");
            return;
        }
        IsBusy = true;

        if (HtmlWebViewChecked) {
            await renderHtml(cmd, Tab);
        } else if (MdWebViewChecked) {
            await renderMdHtml(cmd, Tab);
        } else if (MdSourceChecked) {
            var t = OutputFormatterFactory.GetMarkdownFormatter();
            String rawMd = await t.GenerateViewAsync(cmd.ToXmlObject(), Tab.ToXmlObject());
            var para = new Paragraph();
            para.Inlines.Add(new Run(rawMd));
            Document = new FlowDocument();
            Document.Blocks.Add(para);
        } else {
            IEnumerable<XmlToken> data = XmlChecked
                ? await generateXml(cmd, Tab)
                : await generateHtmlSource(cmd, Tab);
            var para = new Paragraph();
            para.Inlines.AddRange(colorizeSource(data));
            Document = new FlowDocument();
            Document.Blocks.Add(para);
        }
        IsBusy = false;
    }
    Boolean canGenerateView(Object obj) {
        return !IsBusy;
    }
    static async Task<IEnumerable<XmlToken>> generateXml(CmdletObject cmdlet, ModuleObject module) {
        var mamlService = App.Container.Resolve<IMamlService>();
        String rawXml = await mamlService.ExportMamlHelp([cmdlet.ToXmlObject()], null);
        return XmlTokenizer.LoopTokenize(XElement.Parse(rawXml).ToString());
    }
    static async Task<IEnumerable<XmlToken>> generateHtmlSource(CmdletObject cmdlet, ModuleObject module) {
        var htmlProcessor = OutputFormatterFactory.GetHtmlFormatter();
        String rawHtml = await htmlProcessor.GenerateViewAsync(cmdlet.ToXmlObject(), module.ToXmlObject());
        return XmlTokenizer.LoopTokenize(XElement.Parse("<div>" + rawHtml + "</div>").ToString());
    }
    async Task renderHtml(CmdletObject cmdlet, ModuleObject module) {
        var htmlProcessor = OutputFormatterFactory.GetHtmlFormatter();
        String rawSource = await htmlProcessor.GenerateViewAsync(cmdlet.ToXmlObject(), module.ToXmlObject());
        HtmlText = String.Format(Properties.Resources.HtmlTemplate, cmdlet.Name, rawSource, cmdlet.ExtraHeader, cmdlet.ExtraFooter);
    }
    async Task renderMdHtml(CmdletObject command, ModuleObject module) {
        var mdProcessor = OutputFormatterFactory.GetMarkdownFormatter();
        String rawSource = await mdProcessor.GenerateViewAsync(command.ToXmlObject(), module.ToXmlObject());
        HtmlText = Markdown.ToHtml(rawSource, _pipeline);
    }
    static IEnumerable<Run> colorizeSource(IEnumerable<XmlToken> data) {
        var blocks = new List<Run>();
        foreach (XmlToken token in data) {
            var run = new Run(token.Text);
            switch (token.Type) {
                case XmlTokenEnum.Attribute: run.Foreground = new SolidColorBrush(Colors.Red); break;
                case XmlTokenEnum.Comment: run.Foreground = new SolidColorBrush(Colors.Green); break;
                case XmlTokenEnum.Element: run.Foreground = new SolidColorBrush(Colors.DarkRed); break;
                case XmlTokenEnum.Escape: run.Foreground = new SolidColorBrush(Colors.Orchid); break;
                case XmlTokenEnum.SpecialChar: run.Foreground = new SolidColorBrush(Colors.Blue); break;
                case XmlTokenEnum.Value: run.Foreground = new SolidColorBrush(Colors.Navy); break;
            }
            blocks.Add(run);
        }
        return blocks;
    }

    void OnPropertyChanged([CallerMemberName] String name = null) {
        PropertyChangedEventHandler handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}