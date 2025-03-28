using System;

namespace CmdletHelpEditor.API.Models;
public class SavePendingEventArgs : EventArgs;
public delegate void SavePendingEventHandler(Object source, SavePendingEventArgs e);
