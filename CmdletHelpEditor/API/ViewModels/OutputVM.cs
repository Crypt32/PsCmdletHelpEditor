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
using CmdletHelpEditor.API.Tools;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;
using Unity;

namespace CmdletHelpEditor.API.ViewModels;

public class OutputVM : DependencyObject, IAsyncVM {
    readonly IMsgBox _msgBox;
    FlowDocument document;
    Boolean xmlChecked, htmlSourceChecked, htmlChecked, textChecked, isBusy;

    public OutputVM(ClosableModuleItem parent) {
        _msgBox = App.Container.Resolve<IMsgBox>();
        XmlChecked = true;
        Tab = parent;
        GenerateOutputCommand = new AsyncCommand(generateOutput, canGenerateView);
    }

    public IAsyncCommand GenerateOutputCommand { get; }

    public ClosableModuleItem Tab { get; }

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
    public Boolean RtbChecked => XmlChecked || HtmlSourceChecked;

    public FlowDocument Document {
        get => document;
        set {
            document = value;
            OnPropertyChanged(nameof(Document));
        }
    }

    async Task generateOutput(Object obj, CancellationToken cancellationToken) {
        CmdletObject cmd = Tab.EditorContext.CurrentCmdlet;
        ModuleObject module = Tab.Module;
        if (cmd == null) { return; }
        if (XmlChecked && module.UpgradeRequired) {
            _msgBox.ShowWarning("Warning", "The module is offline and requires upgrade. Upgrade the project to allow XML view.");
            return;
        }
        IsBusy = true;

        if (HtmlChecked) {
            await renderHtml(cmd, module);
        } else {
            IEnumerable<XmlToken> data = XmlChecked
                ? await generateXml(cmd, module)
                : await generateHtmlSource(cmd, module);
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
    async Task<IEnumerable<XmlToken>> generateXml(CmdletObject cmdlet, ModuleObject module) {
        String rawXml = await XmlProcessor.XmlGenerateHelp(new[] { cmdlet }, null, module.IsOffline);
        return XmlTokenizer.LoopTokenize(XElement.Parse(rawXml).ToString());
    }
    async Task<IEnumerable<XmlToken>> generateHtmlSource(CmdletObject cmdlet, ModuleObject module) {
        String rawHtml = await HtmlProcessor.GenerateHtmlView(cmdlet, module);
        return XmlTokenizer.LoopTokenize(XElement.Parse("<div>" + rawHtml + "</div>").ToString());
    }
    async Task renderHtml(CmdletObject cmdlet, ModuleObject module) {
        HtmlText = await HtmlProcessor.GenerateHtmlView(cmdlet, module);
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