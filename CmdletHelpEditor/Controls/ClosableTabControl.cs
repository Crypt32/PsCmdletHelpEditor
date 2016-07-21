using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CmdletHelpEditor.API.Models;

namespace CmdletHelpEditor.Controls {
	public class ClosableTabControl : TabControl {
		public static readonly DependencyProperty CloseTabCommandProperty = DependencyProperty.Register(
			"CloseTabCommand",
			typeof(ICommand),
			typeof(ClosableTabControl),
			new PropertyMetadata(null, CloseCommandChanged));
		public static readonly DependencyProperty AddTabCommandProperty = DependencyProperty.Register(
			"AddTabCommand",
			typeof(ICommand),
			typeof(ClosableTabControl),
			new PropertyMetadata(null, AddCommandChanged));

		static void CloseCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ClosableTabControl cs = (ClosableTabControl)d;
			cs.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
		}
		static void AddCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ClosableTabControl cs = (ClosableTabControl)d;
			cs.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
		}
		void HookUpCommand(ICommand oldCommand, ICommand newCommand) {
			// If oldCommand is not null, then we need to remove the handlers. 
			if (oldCommand != null) {
				RemoveCommand(oldCommand);
			}
			AddCommand(newCommand);
		}
		void RemoveCommand(ICommand oldCommand) {
			EventHandler handler = CanExecuteChanged;
			oldCommand.CanExecuteChanged -= handler;
		}
		private void CanExecuteChanged(object sender, EventArgs e) {
			if (Command != null) {
				RoutedCommand command = Command as RoutedCommand;
				// If a RoutedCommand. 
				IsEnabled = command != null
					? command.CanExecute(CommandParameter, CommandTarget)
					: Command.CanExecute(CommandParameter);
			}
		}

		// Add the command. 
		void AddCommand(ICommand newCommand) {
			EventHandler handler = CanExecuteChanged;
			if (newCommand != null) {
				newCommand.CanExecuteChanged += handler;
			}
		}
		[ContentProperty("ItemsSource")]
		public class TabItems : ObservableCollection<ClosableModuleItem> {
			public IList<ClosableModuleItem> MyItems {
				get { return Items; }
				set {
					foreach (ClosableModuleItem item in value) {
						Items.Add(item);
					}
				}
			}
		}
		public ICommand AddTabCommand {
			get { return (ICommand)GetValue(AddTabCommandProperty); }
			set { SetValue(AddTabCommandProperty, value); }
		}
		public ICommand CloseTabCommand {
			get { return (ICommand)GetValue(CloseTabCommandProperty); }
			set { SetValue(CloseTabCommandProperty, value); }
		}
		ICommand Command { get; set; }
		Object CommandParameter { get; set; }
		IInputElement CommandTarget { get; set; }
	}
}
