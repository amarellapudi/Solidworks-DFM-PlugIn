using AngelSix.SolidDna;
using Dna;
using System;
using System.Collections.Generic;
using static AngelSix.SolidDna.SolidWorksEnvironment;

namespace Exporting
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
        public override string AddInTitle => "Exporting Functionality";

        public override string AddInDescription => "Hoping to export .STL";

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

            // Drawing commands
            var drawingGroup = Application.CommandManager.CreateCommands("Export Drawing", new List<CommandManagerItem>(new[]
            {
                new CommandManagerItem
                {
                    Name = "PDF",
                    Tooltip = "PDF",
                    Hint = "Export part as PDF",
                    VisibleForAssemblies = false,
                    VisibleForParts = false,
                    OnClick = () =>
                    {
                        FileExporting.ExportDrawingAsPdf();
                    }
                }
            }), "", "Exports drawing in other formats", "Export Drawing");
        }

        public override void DisconnectedFromSolidWorks()
        {
            throw new NotImplementedException();
        }
    }
}
