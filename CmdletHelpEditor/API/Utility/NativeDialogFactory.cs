using System;
using Microsoft.Win32;

namespace CmdletHelpEditor.API.Utility;

/// <summary>
/// Contains common factory methods for Windows native dialogs.
/// </summary>
public static class NativeDialogFactory {
    /// <summary>
    /// Creates a open file dialog with <c>.pshproj</c> filter.
    /// </summary>
    /// <returns>An instance of dialog.</returns>
    /// <remarks>This method doesn't invoke the dialog. It is caller responsibility to call <see cref="OpenFileDialog.ShowDialog()"/></remarks>
    public static OpenFileDialog CreateOpenHelpProjectDialog() {
        return new OpenFileDialog {
            DefaultExt = ".pshproj",
            Filter = "PowerShell Help Project file (.pshproj)|*.pshproj"
        };
    }
    /// <summary>
    /// Creates a open file dialog with <c>.psm1</c> and <c>.psd1</c> filter.
    /// </summary>
    /// <returns>An instance of dialog.</returns>
    /// <remarks>This method doesn't invoke the dialog. It is caller responsibility to call <see cref="OpenFileDialog.ShowDialog()"/></remarks>
    public static OpenFileDialog CreateOpenPsManifestDialog() {
        return new OpenFileDialog {
            DefaultExt = ".psm1",
            Filter = "PowerShell module files (*.psm1, *.psd1)|*.psm1;*.psd1"
        };
    }
    /// <summary>
    /// Creates a save file dialog with <c>.pshproj</c> filter.
    /// </summary>
    /// <returns>An instance of dialog.</returns>
    /// <remarks>This method doesn't invoke the dialog. It is caller responsibility to call <see cref="OpenFileDialog.ShowDialog()"/></remarks>
    public static SaveFileDialog CreateSaveHelpProjectDialog(String fileNamePrefix) {
        return new SaveFileDialog {
            FileName = fileNamePrefix + ".Help.pshproj",
            DefaultExt = ".pshproj",
            Filter = "PowerShell Help Project file (.pshproj)|*.pshproj"
        };
    }
    /// <summary>
    /// Creates a save file dialog with <c>.xml</c> filter.
    /// </summary>
    /// <returns>An instance of dialog.</returns>
    /// <remarks>This method doesn't invoke the dialog. It is caller responsibility to call <see cref="OpenFileDialog.ShowDialog()"/></remarks>
    public static SaveFileDialog CreateSaveHelpAsXmlDialog(String fileNamePrefix) {
        return new SaveFileDialog {
            FileName = fileNamePrefix + ".Help.xml",
            DefaultExt = ".xml",
            Filter = "PowerShell Help Xml files (.xml)|*.xml"
        };
    }
}