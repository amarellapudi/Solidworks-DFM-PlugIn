using AngelSix.SolidDna;
using Dna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
        }

        public override void DisconnectedFromSolidWorks()
        {
            throw new NotImplementedException();
        }
    }
}
