using AngelSix.SolidDna;
using Dna;
using System;
using System.Collections.Generic;
using static AngelSix.SolidDna.SolidWorksEnvironment;

namespace Exporting_STL
{   
    /// <summary>
    /// Register as a SolidWorks Add-In
    /// </summary>
    public class SolidDnaAddinIntegration : AddInIntegration
    {
        public override void ApplicationStartup()
        {
        }

        public override void ConfigureServices(FrameworkConstruction construction)
        {
            
        }

        public override void PreConnectToSolidWorks()
        {
            
        }

        public override void PreLoadPlugIns()
        {
            
        }
    }

    /// <summary>
    /// Register as SolidDna Plugin
    /// </summary>
    public class MySolidDnaPlugIn : SolidPlugIn
    {
        #region Public Properties

        public override string AddInTitle => "Exporting Functionality";

        public override string AddInDescription => "Export .STL";

        #endregion

        public override void ConnectedToSolidWorks()
        {
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
    }
}
