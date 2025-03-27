#nullable enable
using System;
using CmdletHelpEditor.Abstract;
using Microsoft.Win32;

namespace CmdletHelpEditor.API.Utility;

/// <summary>
/// Contains common factory methods for Windows native dialogs.
/// </summary>
public static class UIMessengerExtensions {
    /// <summary>
    /// Creates an open file dialog with <c>.pshproj</c> filter.
    /// </summary>
    /// <returns>An instance of dialog.</returns>
    /// <remarks>This method doesn't invoke the dialog. It is caller responsibility to call <see cref="OpenFileDialog.ShowDialog()"/></remarks>
    public static Boolean CreateOpenHelpProjectDialog(this IUIMessenger uiMessenger, out String? fileName) {
        return uiMessenger.TryGetOpenFileName(
            out fileName,
            "PowerShell Help Project file (.pshproj)|*.pshproj",
            defaultExtension: ".pshproj");
    }
    /// <summary>
    /// Creates an open file dialog with <c>.psm1</c> and <c>.psd1</c> filter.
    /// </summary>
    /// <returns>An instance of dialog.</returns>
    /// <remarks>This method doesn't invoke the dialog. It is caller responsibility to call <see cref="OpenFileDialog.ShowDialog()"/></remarks>
    public static Boolean CreateOpenPsManifestDialog(this IUIMessenger uiMessenger, out String? fileName) {
        return uiMessenger.TryGetSaveFileName(
            out fileName,
            "PowerShell module files (*.psm1, *.psd1)|*.psm1;*.psd1",
            defaultExtension: ".psm1");
    }
    /// <summary>
    /// Creates an open file dialog with <c>.Help.xml</c> filter.
    /// </summary>
    /// <returns>An instance of dialog.</returns>
    /// <remarks>This method doesn't invoke the dialog. It is caller responsibility to call <see cref="OpenFileDialog.ShowDialog()"/></remarks>
    public static Boolean CreateOpenMamlHelpDialog(this IUIMessenger uiMessenger, out String? fileName, String fileNamePrefix) {
        String suggestedFileName = fileNamePrefix + ".Help.xml";

        return uiMessenger.TryGetOpenFileName(
            out fileName,
            "PowerShell Help Xml files (.xml)|*.xml",
            suggestedFileName,
            defaultExtension: ".xml");
    }
    /// <summary>
    /// Creates a save file dialog with <c>.pshproj</c> filter.
    /// </summary>
    /// <returns>An instance of dialog.</returns>
    /// <remarks>This method doesn't invoke the dialog. It is caller responsibility to call <see cref="OpenFileDialog.ShowDialog()"/></remarks>
    public static Boolean CreateSaveHelpProjectDialog(this IUIMessenger uiMessenger, out String? fileName, String fileNamePrefix) {
        String suggestedFileName = fileNamePrefix + ".Help.pshproj";

        return uiMessenger.TryGetSaveFileName(
            out fileName,
            "PowerShell Help Project file (.pshproj)|*.pshproj",
            suggestedFileName,
            ".pshproj");
    }
    /// <summary>
    /// Creates a save file dialog with <c>.xml</c> filter.
    /// </summary>
    /// <returns>An instance of dialog.</returns>
    /// <remarks>This method doesn't invoke the dialog. It is caller responsibility to call <see cref="OpenFileDialog.ShowDialog()"/></remarks>
    public static Boolean CreateSaveMamlHelpDialog(this IUIMessenger uiMessenger, out String? fileName, String fileNamePrefix) {
        String suggestedFileName = fileNamePrefix + ".Help.xml";

        return uiMessenger.TryGetSaveFileName(
            out fileName,
            "PowerShell Help Xml files (.xml)|*.xml",
            suggestedFileName,
            ".xml");
    }
}