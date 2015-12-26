using CmdletHelpEditor.API.BaseClasses;
using CmdletHelpEditor.API.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CmdletHelpEditor.Controls {
	public class ClosableTabItem : TabItem {

		public static readonly DependencyProperty IsClosableProperty = DependencyProperty.Register("IsClosable", typeof(Boolean), typeof(ClosableTabItem), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty IsSavedProperty = DependencyProperty.Register("IsSaved", typeof(Boolean), typeof(ClosableTabItem), new FrameworkPropertyMetadata(true, SavedPropertyChangedCallback));
		public static readonly DependencyProperty ModuleProperty = DependencyProperty.Register("Module", typeof(ModuleObject), typeof(ClosableTabItem), new FrameworkPropertyMetadata(null, ModulePropertyChangedCallback));

		static void SavedPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs PropertyChangedEventArgs) {
			ClosableTabItem tab = (ClosableTabItem)o;
			tab.Header = (Boolean) PropertyChangedEventArgs.NewValue
				? tab.Header.ToString().Replace("*", String.Empty)
				: tab.Header + "*";
		}
		static void ModulePropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs PropertyChangedEventArgs) {
			ClosableTabItem tab = (ClosableTabItem)o;
			if (PropertyChangedEventArgs.NewValue == null || PropertyChangedEventArgs.Property.Name != "Module") { return; }
			if (!String.IsNullOrEmpty(((ModuleObject)PropertyChangedEventArgs.NewValue).ProjectPath)) {
				FileInfo fi = new FileInfo(((ModuleObject)PropertyChangedEventArgs.NewValue).ProjectPath);
				tab.Header = fi.Name;
			}
		}

		public static readonly DependencyProperty ErrorInfoProperty = DependencyProperty.Register("ErrorInfo", typeof(String), typeof(ClosableTabItem), new FrameworkPropertyMetadata(null));

		public Boolean IsClosable {
			get { return (Boolean)GetValue(IsClosableProperty); }
			set { SetValue(IsClosableProperty, value); }
		}
		public Boolean IsSaved {
			get { return (Boolean)GetValue(IsSavedProperty); }
			set { SetValue(IsSavedProperty, value); }
		}
		public ModuleObject Module {
			get { return (ModuleObject)GetValue(ModuleProperty); }
			set { SetValue(ModuleProperty, value); }
		}
		public String ErrorInfo {
			get { return (String)GetValue(ErrorInfoProperty); }
			set { SetValue(ErrorInfoProperty, value); }
		}
		public EditorVM EditorContext { get; set; }
	}
}
