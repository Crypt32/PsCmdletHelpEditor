using System;

namespace CmdletHelpEditor.API.Utility;
public class SavePendingEventArgs : EventArgs;
public delegate void SavePendingEventHandler(Object source, SavePendingEventArgs e);
