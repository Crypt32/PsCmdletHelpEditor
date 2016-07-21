using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CmdletHelpEditor.API.ViewModels {
	sealed class RelayCommand : ICommand {
		readonly Predicate<Object> _canExecute;
		readonly Action<Object> _execute;
		List<WeakReference> CanExecuteChangedHandlers;

		// constructors
		public RelayCommand(Action<Object> execute, Predicate<Object> canExecute = null) {
			_execute = execute;
			_canExecute = canExecute;
			CommandManagerHelper.RemoveHandlersFromRequerySuggested(CanExecuteChangedHandlers);
		}

		void OnCanExecuteChanged() {
			// we may not be on the UI thread, and things are unhappy when they aren't on the UI thread, so make sure we are on the UI thread
			Debug.Assert(Application.Current != null);
			if (!Application.Current.Dispatcher.CheckAccess()) {
				Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)OnCanExecuteChanged);
			} else {
				CommandManagerHelper.CallWeakReferenceHandlers(CanExecuteChangedHandlers);
			}
		}

		// public methods
		public Boolean CanExecute(Object parameter) {
			return _canExecute == null || _canExecute(parameter);
		}
		public void Execute(Object parameter) {
			_execute(parameter);
		}
		public void RaiseCanExecuteChanged() {
			OnCanExecuteChanged();
		}

		// events
		public event EventHandler CanExecuteChanged {
			add {
				CommandManager.RequerySuggested += value;
				CommandManagerHelper.AddWeakReferenceHandler(ref CanExecuteChangedHandlers, value, 2);
			}
			remove {
				CommandManager.RequerySuggested -= value;
				CommandManagerHelper.RemoveWeakReferenceHandler(CanExecuteChangedHandlers, value);
			}
		}
	}
	sealed class RelayCommand<T> : ICommand {
		readonly Action<T> _executeMethod;
		readonly Func<T, Boolean> _canExecuteMethod;
		Boolean _isAutomaticRequeryDisabled;
		List<WeakReference> CanExecuteChangedHandlers;

		public RelayCommand(Action<T> executeMethod) : this(executeMethod, null, false) { }
		public RelayCommand(Action<T> executeMethod, Func<T, Boolean> canExecute) : this(executeMethod, canExecute, false) { }
		//public RelayCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod) : this(executeMethod, canExecuteMethod, false) { }
		RelayCommand(Action<T> executeMethod, Func<T, Boolean> canExecuteMethod, Boolean isAutomaticRequeryDisabled) {
			if (executeMethod == null) { throw new ArgumentNullException("executeMethod"); }
			_executeMethod = executeMethod;
			_canExecuteMethod = canExecuteMethod;
			_isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
		}

		public Boolean CanExecute(T parameter) {
			return _canExecuteMethod == null || _canExecuteMethod(parameter);
		}
		public void Execute(T parameter) {
			if (_executeMethod != null) { _executeMethod(parameter); }
		}
		public void RaiseCanExecuteChanged() {
			OnCanExecuteChanged();
		}
		public Boolean IsAutomaticRequeryDisabled {
			get { return _isAutomaticRequeryDisabled; }
			set {
				if (_isAutomaticRequeryDisabled != value) {
					if (value) {
						CommandManagerHelper.RemoveHandlersFromRequerySuggested(CanExecuteChangedHandlers);
					} else {
						CommandManagerHelper.AddHandlersToRequerySuggested(CanExecuteChangedHandlers);
					}
					_isAutomaticRequeryDisabled = value;
				}
			}
		}

		private void OnCanExecuteChanged() {
			// we may not be on the UI thread, and things are unhappy when they aren't on the UI thread, so make sure we are on the UI thread
			Debug.Assert(Application.Current != null);
			if (!Application.Current.Dispatcher.CheckAccess()) {
				Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)OnCanExecuteChanged);
			} else {
				CommandManagerHelper.CallWeakReferenceHandlers(CanExecuteChangedHandlers);
			}
		}

		Boolean ICommand.CanExecute(Object parameter) {
			// if T is of value type and the parameter is not
			// set yet, then return false if CanExecute delegate
			// exists, else return true
			if (parameter == null && typeof(T).IsValueType) { return (_canExecuteMethod == null); }
			return CanExecute((T)parameter);
		}
		void ICommand.Execute(Object parameter) {
			Execute((T)parameter);
		}

		// events
		public event EventHandler CanExecuteChanged {
			add {
				if (!_isAutomaticRequeryDisabled) { CommandManager.RequerySuggested += value; }
				CommandManagerHelper.AddWeakReferenceHandler(ref CanExecuteChangedHandlers, value, 2);
			}
			remove {
				if (!_isAutomaticRequeryDisabled) { CommandManager.RequerySuggested -= value; }
				CommandManagerHelper.RemoveWeakReferenceHandler(CanExecuteChangedHandlers, value);
			}
		}
	}
	static class CommandManagerHelper {
		internal static void CallWeakReferenceHandlers(List<WeakReference> handlers) {
			if (handlers != null) {
				// Take a snapshot of the handlers before we call out to them since the handlers
				// could cause the array to me modified while we are reading it.

				EventHandler[] callees = new EventHandler[handlers.Count];
				int count = 0;

				for (int i = handlers.Count - 1; i >= 0; i--) {
					WeakReference reference = handlers[i];
					EventHandler handler = reference.Target as EventHandler;
					if (handler == null) {
						// Clean up old handlers that have been collected
						handlers.RemoveAt(i);
					} else {
						callees[count] = handler;
						count++;
					}
				}

				// Call the handlers that we snapshotted
				for (int i = 0; i < count; i++) {
					EventHandler handler = callees[i];
					handler(null, EventArgs.Empty);
				}
			}
		}
		internal static void AddHandlersToRequerySuggested(IEnumerable<WeakReference> handlers) {
			if (handlers != null) {
				foreach (EventHandler handler in handlers.Select(handlerRef => handlerRef.Target).OfType<EventHandler>()) {
					CommandManager.RequerySuggested += handler;
				}
			}
		}
		internal static void RemoveHandlersFromRequerySuggested(IEnumerable<WeakReference> handlers) {
			if (handlers != null) {
				foreach (WeakReference handlerRef in handlers) {
					EventHandler handler = handlerRef.Target as EventHandler;
					if (handler != null) {
						CommandManager.RequerySuggested -= handler;
					}
				}
			}
		}
		internal static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler) {
			AddWeakReferenceHandler(ref handlers, handler, -1);
		}
		internal static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler, int defaultListSize) {
			if (handlers == null) {
				handlers = (defaultListSize > 0 ? new List<WeakReference>(defaultListSize) : new List<WeakReference>());
			}

			handlers.Add(new WeakReference(handler));
		}
		internal static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler) {
			if (handlers != null) {
				for (int i = handlers.Count - 1; i >= 0; i--) {
					WeakReference reference = handlers[i];
					EventHandler existingHandler = reference.Target as EventHandler;
					if ((existingHandler == null) || (existingHandler == handler)) {
						// Clean up old handlers that have been collected
						// in addition to the handler that is to be removed.
						handlers.RemoveAt(i);
					}
				}
			}
		}
	}
}