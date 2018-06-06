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

                    }
                }
            }), "", "Exports parts in other formats", "Export Part");
        }

        public override void DisconnectedFromSolidWorks()
        {
            throw new NotImplementedException();
        }
    }
}
