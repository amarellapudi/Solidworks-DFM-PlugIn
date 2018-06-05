using AngelSix.SolidDna;
using Dna;

namespace DynamicReload
{
    public class SolidDnaIntegration : AddInIntegration
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
            PlugInIntegration.AddPlugIn(@"C:\Users\Aniruddh\Desktop\solidworks-api-develop\Tutorials\DynamicReload\DynamicReload\bin\Debug\DynamicReload.Main.dll");
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
}
