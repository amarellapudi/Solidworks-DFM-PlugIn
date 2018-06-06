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
                Application.ShowMessageBox("Failed to save part as DXF", SolidWorksMessageBoxIcon.Stop);
        }


        public static void ExportModelAsStep()
        {

        }

        public static void ExportDrawingAsPdf()
        {

        }
        #endregion

        #region Private Helpers

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
