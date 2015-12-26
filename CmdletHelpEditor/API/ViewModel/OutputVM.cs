using System.Text;
using CmdletHelpEditor.API.BaseClasses;
using CmdletHelpEditor.API.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CmdletHelpEditor.Controls;

namespace CmdletHelpEditor.API.ViewModel {
	public class OutputVM : DependencyObject, INotifyPropertyChanged {
		FlowDocument document;
		Boolean xmlChecked = true, htmlSourceChecked, htmlChecked, textChecked;
		Visibility busyControlVisible, rtbVisible, webBrowserVisible;

		public OutputVM(ClosableTabItem parent) {
			BusyControlVisible = Visibility.Collapsed;
			RtbVisible = Visibility.Collapsed;
			WebBrowserVisible = Visibility.Collapsed;
			Tab = parent;
			GenerateOutputCommand = new RelayCommand(GenerateOutput, CanGenerateView);
		}

		public ICommand GenerateOutputCommand { get; set; }

		public ClosableTabItem Tab { get; set; }

		// dependency property is required for HtmlText property
		public static readonly DependencyProperty HtmlTextProperty = DependencyProperty.Register("HtmlText", typeof(String), typeof(OutputVM), new PropertyMetadata("<br />"));

		public String HtmlText {
			get { return (String)GetValue(HtmlTextProperty); }
			set { SetValue(HtmlTextProperty, value); }
		}
		public Boolean XmlChecked {
			get { return xmlChecked; }
			set {
				xmlChecked = value;
				OnPropertyChanged("XmlChecked");
			}
		}
		public Boolean HtmlSourceChecked {
			get { return htmlSourceChecked; }
			set {
				htmlSourceChecked = value;
				OnPropertyChanged("HtmlSourceChecked");
			}
		}
		public Boolean HtmlChecked {
			get { return htmlChecked; }
			set {
				htmlChecked = value;
				OnPropertyChanged("HtmlChecked");
			}
		}
		public Boolean TextChecked {
			get { return textChecked; }
			set {
				textChecked = value;
				OnPropertyChanged("TextChecked");
			}
		}

		public Visibility BusyControlVisible {
			get { return busyControlVisible; }
			set {
				busyControlVisible = value;
				OnPropertyChanged("BusyControlVisible");
			}
		}
		public Visibility RtbVisible {
			get { return rtbVisible; }
			set {
				rtbVisible = value;
				OnPropertyChanged("RtbVisible");
			}
		}
		public Visibility WebBrowserVisible {
			get { return webBrowserVisible; }
			set {
				webBrowserVisible = value;
				OnPropertyChanged("WebBrowserVisible");
			}
		}

		public FlowDocument Document {
			get { return document; }
			set {
				document = value;
				OnPropertyChanged("Document");
			}
		}

		async void GenerateOutput(Object obj) {
			CmdletObject cmd = Tab.EditorContext.CurrentCmdlet;
			ModuleObject module = Tab.Module;
			if (cmd == null) { return; }
			
			BusyControlVisible = Visibility.Visible;
			RtbVisible = Visibility.Collapsed;
			WebBrowserVisible = Visibility.Collapsed;
			
			if (HtmlChecked) {
				HtmlText = await HtmlProcessor.GenerateHtmlView(cmd, module);
				HtmlText = String.Format(Properties.Resources.HtmlTemplate, cmd.Name, HtmlText, cmd.ExtraHeader, cmd.ExtraFooter);
				BusyControlVisible = Visibility.Collapsed;
				RtbVisible = Visibility.Collapsed;
				WebBrowserVisible = Visibility.Visible;
				return;
			}

			IEnumerable<XmlToken> data = new List<XmlToken>();
			if (XmlChecked) {
				if (module.UpgradeRequired) {
					Utils.MsgBox("Warning", "The module is offline and requires upgrade. Upgrade the project to allow XML view.", MessageBoxButton.OK, MessageBoxImage.Warning);
					BusyControlVisible = Visibility.Collapsed;
					return;
				}
				List<CmdletObject> cmdlets = new List<CmdletObject> { cmd };
				StringBuilder SB = new StringBuilder();
				await XmlProcessor.XmlGenerateHelp(SB, cmdlets, null, module.IsOffline);
				data = XmlTokenizer.LoopTokenize(SB.ToString());
			} else if (HtmlSourceChecked) {
				data = await HtmlProcessor.GenerateHtmlSourceHelp(cmd, module);
			}
			Paragraph para = new Paragraph();
			para.Inlines.AddRange(ColorizeSource(data));
			Document = new FlowDocument();
			Document.Blocks.Add(para);
			BusyControlVisible = Visibility.Collapsed;
			WebBrowserVisible = Visibility.Collapsed;
			RtbVisible = Visibility.Visible;
		}
		Boolean CanGenerateView(Object obj) {
			return BusyControlVisible != Visibility.Visible;
		}
		static IEnumerable<Run> ColorizeSource(IEnumerable<XmlToken> data) {
			List<Run> blocks = new List<Run>();
			foreach (XmlToken token in data) {
				Run run = new Run(token.Text);
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
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
