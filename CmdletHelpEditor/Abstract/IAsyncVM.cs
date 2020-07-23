using System;
using System.ComponentModel;

namespace CmdletHelpEditor.Abstract {
    // represents model that contains async operations
    public interface IAsyncVM : INotifyPropertyChanged {
        /// <summary>
        /// Gets the value that indicates whether the async operation is in progress.
        /// </summary>
        Boolean IsBusy { get; }
    }
}