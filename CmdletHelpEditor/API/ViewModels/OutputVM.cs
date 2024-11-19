using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

public class OutputVM : AsyncViewModel {
    static readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    readonly IMsgBox _msgBox;
    String htmlText;
    FlowDocument document;
    Boolean xmlChecked, htmlViewChecked, textChecked, isBusy, mdViewChecked;

    public OutputVM(ModuleObject parent) {
        _msgBox = App.Container.Resolve<IMsgBox>();
        XmlChecked = true;
        Tab = parent;
        GenerateOutputCommand = new AsyncCommand(generateOutput, canGenerateView);
    }

    public IAsyncCommand GenerateOutputCommand { get; }

    public ModuleObject Tab { get; }

    public String HtmlText {
        get => htmlText;
        set {
            htmlText = value;
            OnPropertyChanged();
        }
    }
    public Boolean XmlChecked {
        get => xmlChecked;
        set {
            xmlChecked = value;
            OnPropertyChanged();
            resetViewControls();
        }
    }
    public Boolean HtmlViewChecked {
        get => htmlViewChecked;
        set {
            htmlViewChecked = value;
            OnPropertyChanged();
            resetViewControls();
        }
    }
    public Boolean MdViewChecked {
        get => mdViewChecked;
        set {
            mdViewChecked = value;
            OnPropertyChanged();
            resetViewControls();
        }
    }
    public Boolean TextChecked {
        get => textChecked;
        set {
            textChecked = value;
            OnPropertyChanged();
            resetViewControls();
        }
    }
    public Boolean WebViewChecked => HtmlViewChecked || MdViewChecked;

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
        StartSpinner();
        String rawSource;
        if (HtmlViewChecked) {
            rawSource = await generateHtmlSource(cmd, Tab);
            renderXSource(rawSource);
            renderHtml(rawSource, cmd);
        } else if (MdViewChecked) {
            rawSource = await generateMdSource(cmd, Tab);
            renderMdSource(rawSource);
            renderMdHtml(rawSource);
        } else if (XmlChecked) {
            rawSource = XmlChecked
                ? await generateXml(cmd, Tab)
                : await generateHtmlSource(cmd, Tab);
            renderXSource(rawSource);
        }

        StopSpinner();
    }
    Boolean canGenerateView(Object obj) {
        return !IsBusy;
    }
    static Task<String> generateXml(CmdletObject command, ModuleObject module) {
        var mamlService = App.Container.Resolve<IMamlService>();
        return mamlService.ExportMamlHelp([command.ToXmlObject()], null);
    }

    static async Task<String> generateHtmlSource(CmdletObject command, ModuleObject module) {
        IHelpOutputFormatter formatter = OutputFormatterFactory.GetHtmlFormatter();
        return "<div>" + await formatter.GenerateViewAsync(command.ToXmlObject(), module.ToXmlObject()) + "</div>";
    }
    void renderXSource(String rawSource) {
        IEnumerable<XmlToken> tokens = XmlTokenizer.LoopTokenize(XElement.Parse(rawSource).ToString());
        var para = new Paragraph();
        para.Inlines.AddRange(colorizeSource(tokens));
        Document = new FlowDocument();
        Document.Blocks.Add(para);
    }
    void renderHtml(String rawSource, CmdletObject command) {
        HtmlText = String.Format(Properties.Resources.HtmlTemplate, command.Name, rawSource, command.ExtraHeader, command.ExtraFooter);
    }


    static Task<String> generateMdSource(CmdletObject command, ModuleObject module) {
        IHelpOutputFormatter formatter = OutputFormatterFactory.GetMarkdownFormatter();
        return formatter.GenerateViewAsync(command.ToXmlObject(), module.ToXmlObject());
    }
    void renderMdSource(String rawSource) {
        var para = new Paragraph();
        para.Inlines.Add(new Run(rawSource));
        Document = new FlowDocument();
        Document.Blocks.Add(para);
    }
    void renderMdHtml(String rawSource) {
        HtmlText = Markdown.ToHtml(rawSource, _pipeline);
    }

    void resetViewControls() {
        Document = new FlowDocument();
        HtmlText = "<br/>";
        OnPropertyChanged(nameof(WebViewChecked));
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
}