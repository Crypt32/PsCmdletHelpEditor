using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public static class FormatCommands {
        static FormatCommands() {
            SetCommonFormatCommand = new RelayCommand(SetFormat, CanFormat);
        }

        public static ICommand SetCommonFormatCommand { get; set; }

        static void SetFormat(Object obj) {
            if (obj == null) { return; }
            Object[] param = (Object[]) obj;
            IInputElement felement = FocusManager.GetFocusedElement((MainWindow)param[0]);
            if (!(felement is TextBox)) { return; }
            Int32 index = ((TextBox)felement).CaretIndex;
            switch (((Button)param[1]).Name) {
                case "Bold":
                    ((TextBox)felement).SelectedText = "[b]" + ((TextBox)felement).SelectedText + "[/b]";
                    break;
                case "Italic":
                    ((TextBox)felement).SelectedText = "[i]" + ((TextBox)felement).SelectedText + "[/i]";
                    break;
                case "Underline":
                    ((TextBox)felement).SelectedText = "[u]" + ((TextBox)felement).SelectedText + "[/u]";
                    break;
                case "Strike":
                    ((TextBox)felement).SelectedText = "[s]" + ((TextBox)felement).SelectedText + "[/s]";
                    break;
            }
            ((TextBox)felement).CaretIndex = index + 3;
        }
        static Boolean CanFormat(Object obj) {
            //return true;
            try {
                if (obj == null) { return false; }
                Object[] param = (Object[])obj;
                IInputElement felement = FocusManager.GetFocusedElement((MainWindow)param[0]);
                if (felement == null) { return false; }
                return felement is TextBox && (String)((TextBox)felement).Tag == "AllowFormat";
            }
            catch (Exception e) {
                return false;
            }
        }
    }
}
