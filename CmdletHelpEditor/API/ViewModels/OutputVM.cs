using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using PsCmdletHelpEditor.Core.Services.Formatters;
using PsCmdletHelpEditor.Core.Services.MAML;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using Unity;

namespace CmdletHelpEditor.API.ViewModels;

public class OutputVM : DependencyObject, INotifyPropertyChanged {
    readonly IMsgBox _msgBox;
    FlowDocument document;
    Boolean xmlChecked, htmlSourceChecked, mdSourceChecked, htmlChecked, textChecked, isBusy;

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
            OnPropertyChanged(nameof(IsBusy));
        }
    }
    public Boolean XmlChecked {
        get => xmlChecked;
        set {
            xmlChecked = value;
            OnPropertyChanged(nameof(XmlChecked));
            OnPropertyChanged(nameof(RtbChecked));
        }
    }
    public Boolean HtmlSourceChecked {
        get => htmlSourceChecked;
        set {
            htmlSourceChecked = value;
            OnPropertyChanged(nameof(HtmlSourceChecked));
            OnPropertyChanged(nameof(RtbChecked));
        }
    }
    public Boolean MdSourceChecked {
        get => mdSourceChecked;
        set {
            mdSourceChecked = value;
            OnPropertyChanged(nameof(MdSourceChecked));
            OnPropertyChanged(nameof(RtbChecked));
        }
    }
    public Boolean HtmlChecked {
        get => htmlChecked;
        set {
            htmlChecked = value;
            OnPropertyChanged(nameof(HtmlChecked));
            OnPropertyChanged(nameof(RtbChecked));
        }
    }
    public Boolean TextChecked {
        get => textChecked;
        set {
            textChecked = value;
            OnPropertyChanged(nameof(TextChecked));
            OnPropertyChanged(nameof(RtbChecked));
        }
    }
    public Boolean RtbChecked => XmlChecked || HtmlSourceChecked || MdSourceChecked;

    public FlowDocument Document {
        get => document;
        set {
            document = value;
            OnPropertyChanged(nameof(Document));
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

        if (HtmlChecked) {
            await renderHtml(cmd, Tab);
        } else if (MdSourceChecked) {
            var t = OutputFormatterFactory.GetMarkdownFormatter();
            var rawMd = await t.GenerateViewAsync(cmd.ToXmlObject(), Tab.ToXmlObject());
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
        HtmlText = await htmlProcessor.GenerateViewAsync(cmdlet.ToXmlObject(), module.ToXmlObject());
        HtmlText = String.Format(Properties.Resources.HtmlTemplate, cmdlet.Name, HtmlText, cmdlet.ExtraHeader, cmdlet.ExtraFooter);
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

    void OnPropertyChanged(String name) {
        PropertyChangedEventHandler handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}