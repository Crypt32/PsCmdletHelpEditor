using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CmdletHelpEditor.Controls {
	public class BindableRichTextBox : RichTextBox {
		public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(FlowDocument), typeof(BindableRichTextBox), new FrameworkPropertyMetadata(null, OnDocumentChanged));

		public new FlowDocument Document {
			get { return (FlowDocument)GetValue(DocumentProperty); }
			set { SetValue(DocumentProperty, value); }
		}

		public static void OnDocumentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
			RichTextBox rtb = (RichTextBox)obj;
			rtb.Document = args.NewValue == null
				? new FlowDocument()
				: (FlowDocument) args.NewValue;
		}
	}
}
