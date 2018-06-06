using Dna;
using AngelSix.SolidDna;
using static AngelSix.SolidDna.SolidWorksEnvironment;
using System;
using Microsoft.Win32;
using SolidWorks.Interop.swconst;

namespace Exporting
{
    /// <summary>
    /// Exports Exports SolidWorks files into various formats
    /// </summary>
    public static class FileExporting
    {
        #region Public Methods

        /// <summary>
        /// Exports the currently active part as a DXF
        /// </summary>
        public static void ExportPartAsDxf()
        {
            // Make Sure it's a part
            if (Application.ActiveModel?.IsPart != true)
            {
                // Tell user
                Application.ShowMessageBox("Active model is not a part", SolidWorksMessageBoxIcon.Stop);

                return;
            }

            // Ask user for location
            var location = GetSaveLocation("DXF Flat Pattern|*.dxf", "Save Part as DXF");

            // Check if user clicked cancel
            if (string.IsNullOrEmpty(location))
                return;

            if (Application.ActiveModel.AsPart().ExportFlatPatternView(location, (int)swExportFlatPatternViewOptions_e.swExportFlatPatternOption_RemoveBends))
                // Tell user success
                Application.ShowMessageBox("Successfully saved part as DXF");
            else
                // Tell user failed
                Application.ShowMessageBox("Failed to save part as DXF", SolidWorksMessageBoxIcon.Stop);
        }


        public static void ExportModelAsStep()
        {
            // Make Sure it's a part or assembly
            if (Application.ActiveModel?.IsPart != true && Application.ActiveModel?.IsAssembly != true)
            {
                // Tell user
                Application.ShowMessageBox("Active model is not a part or assembly", SolidWorksMessageBoxIcon.Stop);

                return;
            }

            // Ask user for location
            var location = GetSaveLocation("STEP File|*.step", "Save Model as STEP");

            // Check if user clicked cancel
            if (string.IsNullOrEmpty(location))
                return;

            if (!SaveModelAs(location))
                // Tell user failed
                Application.ShowMessageBox("Failed to save model as STEP", SolidWorksMessageBoxIcon.Stop);
            else
                // Tell user success
                Application.ShowMessageBox("Successfully saved model as STEP");
        }

        public static void ExportDrawingAsPdf()
        {

        }
        #endregion

        #region Private Helpers

        /// <summary>
        /// Attempts to save a model as a different format
        /// </summary>
        /// <param name="location">The absolute path to save the model</param>
        /// <param name="exportData">The export data, if any</param>
        /// <returns></returns>
        private static bool SaveModelAs(string location, object exportData = null)
        {
            var error = -1;
            var warning = -1;

            var version = (int)swSaveAsVersion_e.swSaveAsCurrentVersion;
            var options = (int)(swSaveAsOptions_e.swSaveAsOptions_Copy | swSaveAsOptions_e.swSaveAsOptions_Silent | swSaveAsOptions_e.swSaveAsOptions_UpdateInactiveViews);

            // Save model as...
            Application.ActiveModel.Extension.UnsafeObject.SaveAs(location, version, options, exportData, ref error, ref warning);

            // If this fails, try one other wa
            if (error != 0)
                Application.ActiveModel.UnsafeObject.SaveAs4(location, version, options, ref error, ref warning);

            // Return success result
            return error == 0;
        }

        /// <summary>
        /// Asks the user for a save location of a file
        /// </summary>
        /// <param name="filter">The filter for the save dialog</param>
        /// <param name="title">The title for the save dialog</param>
        /// <returns></returns>
        private static string GetSaveLocation(string filter, string title)
        {
            // Create dialog
            var dialog = new SaveFileDialog { Filter = filter, Title = title, AddExtension = true };

            // Het dialog result
            if (dialog.ShowDialog() == true)
                return dialog.FileName;

            return null;
        }

        #endregion
    }
}
