using System;
using PsCmdletHelpEditor.BLL.Abstraction;

namespace PsCmdletHelpEditor.BLL.Models {
    class ConfigProvider : IConfigProvider {

        public ConfigProvider() {
            readSettings();
        }

        public Boolean LoadPsFunctions { get; set; }
        public Boolean LoadPsFilter { get; set; }
        public Boolean LoadPsCmdlets { get; set; }
        public Boolean LoadPsWorkflows { get; set; }
        public Boolean LoadPsApplications { get; set; }
        public Boolean LoadPsDscConfigurations { get; set; }
        public Boolean LoadExternalPsScripts { get; set; }
        public Boolean LoadPsScripts { get; set; }
        public Boolean ShowToolbar { get; set; }
        public Boolean ShowStatusBar { get; set; }
        public Boolean ToolbarLocked { get; set; }

        void readSettings() {
            Properties.Settings.Default.Reload();
            LoadPsFunctions         = Properties.Settings.Default.LoadPsFunctions;
            LoadPsFilter            = Properties.Settings.Default.LoadPsFilter;
            LoadPsCmdlets           = Properties.Settings.Default.LoadPsCmdlets;
            LoadPsWorkflows         = Properties.Settings.Default.LoadPsWorkflows;
            LoadPsApplications      = Properties.Settings.Default.LoadPsApplications;
            LoadPsDscConfigurations = Properties.Settings.Default.LoadPsDscConfigurations;
            LoadExternalPsScripts   = Properties.Settings.Default.LoadExternalPsScripts;
            LoadPsScripts           = Properties.Settings.Default.LoadPsScripts;
            ShowToolbar             = Properties.Settings.Default.ShowToolbar;
            ShowStatusBar           = Properties.Settings.Default.ShowStatusBar;
            ToolbarLocked           = Properties.Settings.Default.ToolbarLocked;
        }

        public void SaveSettings() {
            Properties.Settings.Default.LoadPsFunctions         = LoadPsFunctions;
            Properties.Settings.Default.LoadPsFilter            = LoadPsFilter;
            Properties.Settings.Default.LoadPsCmdlets           = LoadPsCmdlets;
            Properties.Settings.Default.LoadPsWorkflows         = LoadPsWorkflows;
            Properties.Settings.Default.LoadPsApplications      = LoadPsApplications;
            Properties.Settings.Default.LoadPsDscConfigurations = LoadPsDscConfigurations;
            Properties.Settings.Default.LoadExternalPsScripts   = LoadExternalPsScripts;
            Properties.Settings.Default.LoadPsScripts           = LoadPsScripts;
            Properties.Settings.Default.ShowToolbar             = ShowToolbar;
            Properties.Settings.Default.ShowStatusBar           = ShowStatusBar;
            Properties.Settings.Default.ToolbarLocked           = ToolbarLocked;
            Properties.Settings.Default.Save();
        }
    }
}
