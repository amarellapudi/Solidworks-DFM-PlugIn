﻿using AngelSix.SolidDna;
using Microsoft.Win32;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AngelSix.SolidDna.SolidWorksEnvironment;

namespace SongTelenkoDFM
{
    /// <summary>
    /// Exports SolidWorks files into various formats
    /// </summary>
    public static class Methods_FileExporting
    {
        #region Public Methods

        /// <summary>
        /// Exports the currently active part as a STL
        /// </summary>
        public static bool ExportModelAsStl(string filter = "STL File|*.stl", string title = "Save Model as STL")
        {
            // Make Sure it's a part or assembly
            if (Application.ActiveModel?.IsPart != true && Application.ActiveModel?.IsAssembly != true)
            {
                // Tell user
                Application.ShowMessageBox("Active model is not a part or assembly", SolidWorksMessageBoxIcon.Stop);

                return false;
            }
            string location;
            // Ask user for location
            if (filter != "STL File|*.stl")
                location = filter;
            else
                location = GetSaveLocation(filter, title);

            // Check if user clicked cancel
            if (string.IsNullOrEmpty(location))
                return false;

            if (!SaveModelAs(location))
            {
                // Tell user failed
                Application.ShowMessageBox("Failed to save model as STL", SolidWorksMessageBoxIcon.Stop);
                return false;
            }
            else
            {
                // Tell user success
                //Application.ShowMessageBox("Successfully exported model as STL");
                return true;
            }
        }

        /// <summary>
        /// Exports the currently active part as a STL
        /// </summary>
        public static bool ExportModelAsPNG(string filter = "PNG File|*.png", string title = "Save Model as PNG")
        {
            // Make Sure it's a part or assembly
            if (Application.ActiveModel?.IsPart != true && Application.ActiveModel?.IsAssembly != true)
            {
                // Tell user
                Application.ShowMessageBox("Active model is not a part or assembly", SolidWorksMessageBoxIcon.Stop);

                return false;
            }
            string location;
            // Ask user for location
            if (filter != "PNG File|*.png")
                location = filter;
            else
                location = GetSaveLocation(filter, title);

            // Check if user clicked cancel
            if (string.IsNullOrEmpty(location))
                return false;

            if (!SaveModelAs(location))
            {
                // Tell user failed
                Application.ShowMessageBox("Failed to save model as PNG", SolidWorksMessageBoxIcon.Stop);
                return false;
            }
            else
            {
                // Tell user success
                //Application.ShowMessageBox("Successfully exported model as PNG");
                return true;
            }
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

            // Save model as... (this is the new method)
            //Application.ActiveModel.Extension.UnsafeObject.SaveAs(location, version, options, exportData, ref error, ref warning);
            //if (error != 0) Application.ActiveModel.UnsafeObject.SaveAs4(location, version, options, ref error, ref warning);
            //return error == 0;

            // If this fails, try one other way (legacy method)
            bool success = Application.ActiveModel.UnsafeObject.SaveAs4(location, version, options, ref error, ref warning);

            // Return success result
            return success;
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
