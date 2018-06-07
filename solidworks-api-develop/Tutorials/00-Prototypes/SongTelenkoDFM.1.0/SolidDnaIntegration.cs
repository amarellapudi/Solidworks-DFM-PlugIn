using AngelSix.SolidDna;
using Dna;
using System.Collections.Generic;
using System.IO;
using static AngelSix.SolidDna.SolidWorksEnvironment;

namespace SongTelenkoDFM
{
    /// <summary>
    /// Register as a SolidWorks Add-in
    /// </summary>
    public class MyAddinIntegration : AddInIntegration
    {
        /// <summary>
        /// Specific application start-up code
        /// </summary>
        public override void ApplicationStartup()
        {

        }

        /// <summary>
        /// Steps to take before any add-ins load
        /// </summary>
        /// <returns></returns>
        public override void PreLoadPlugIns()
        {

        }

        public override void PreConnectToSolidWorks()
        {
            // NOTE: To run in our own AppDomain do the following
            //       Be aware doing so sometimes causes API's to fail
            //       when they try to load dll's
            //
            // PlugInIntegration.UseDetachedAppDomain = true;
        }

        public override void ConfigureServices(FrameworkConstruction construction)
        {

        }
    }

    /// <summary>
    /// My first SolidDna Plug-in
    /// </summary>
    public class MySolidDnaPlguin : SolidPlugIn
    {
        #region Private Members

        /// <summary>
        /// The Taskpane UI for our plug-in
        /// </summary>
        private TaskpaneIntegration<MyTaskpaneUI> mTaskpane;

        #endregion

        #region Public Properties

        /// <summary>
        /// My Add-in description
        /// </summary>
        public override string AddInDescription => "Making machining knowledge more accessible";

        /// <summary>
        /// My Add-in title
        /// </summary>
        public override string AddInTitle => "Song Telenko DFM Plug-In v1.0";

        #endregion

        #region Connect To SolidWorks

        public override void ConnectedToSolidWorks()
        {
            /// <summary>
            /// Create our taskpane
            /// <summary>
            mTaskpane = new TaskpaneIntegration<MyTaskpaneUI>()
            {
                Icon = Path.Combine(this.AssemblyPath(), "logo-small.png"),
                WpfControl = new CustomPropertiesUI()
            };

            mTaskpane.AddToTaskpaneAsync();

            /// <summary>
            /// Command Manager Items
            /// <summary>

            // Part commands
            var partGroup = Application.CommandManager.CreateCommands("Export Part", new List<CommandManagerItem>(new[]
            {
                new CommandManagerItem
                {
                    Name = "DXF",
                    Tooltip = "DXF",
                    Hint = "Export part as DXF",
                    VisibleForDrawings = false,
                    VisibleForAssemblies = false,
                    OnClick = () =>
                    {
                        FileExporting.ExportPartAsDxf();
                    }
                },

                new CommandManagerItem
                {
                    Name = "STEP",
                    Tooltip = "STEP",
                    Hint = "Export part as STEP",
                    VisibleForDrawings = false,
                    VisibleForAssemblies = false,
                    OnClick = () =>
                    {
                        FileExporting.ExportModelAsStep();
                    }
                },

                new CommandManagerItem
                {
                    Name = "STL",
                    Tooltip = "STL",
                    Hint = "Export part as STL",
                    VisibleForDrawings = false,
                    VisibleForAssemblies = false,
                    OnClick = () =>
                    {
                        FileExporting.ExportModelAsStl();
                    }
                }
            }), "", "Exports parts in other formats", "Export Part");

            // Assembly commands
            var assemblyGroup = Application.CommandManager.CreateCommands("Export Assembly", new List<CommandManagerItem>(new[]
            {
                new CommandManagerItem
                {
                    Name = "STEP",
                    Tooltip = "STEP",
                    Hint = "Export part as STEP",
                    VisibleForDrawings = false,
                    VisibleForParts = false,
                    OnClick = () =>
                    {
                        FileExporting.ExportModelAsStep();
                    }
                },

                new CommandManagerItem
                {
                    Name = "STL",
                    Tooltip = "STL",
                    Hint = "Export part as STL",
                    VisibleForDrawings = false,
                    VisibleForParts = false,
                    OnClick = () =>
                    {
                        FileExporting.ExportModelAsStl();
                    }
                }
            }), "", "Exports assembly in other formats", "Export Assembly");
        }

        public override void DisconnectedFromSolidWorks()
        {

        }

        #endregion
    }
}
