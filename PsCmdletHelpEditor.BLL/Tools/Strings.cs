using System;

namespace PsCmdletHelpEditor.BLL.Tools {
	public static class Strings {
		public const String InfoModuleListLoading = "Please wait while generating the module list...";
		public const String InfoModuleLoading = "Please wait while loading the module...";
		public const String InfoCmdletsLoading = "Please wait while generating the command list...";
		public const String InfoOutputGenerating = "Please wait while the output is generated...";
		public const String InfoDataLoading = "Please wait while the data is loading...";
		public const String InfoSaveRequired = "Current project was modified. Save changes?";
		public const String WarnBloggerNeedsMoreData = "Connection settings are not complete.\nGo to module properties and provide connection information";
		public const String StatusSaved = "Saved";
		public const String StatusUnsaved = "Unsaved";
		public const String StatusDefault = ".";

		public const String E_EmptyCmds = "No command types are specified. Check at least one command type to import in Tools -> Options.";

	}
}
