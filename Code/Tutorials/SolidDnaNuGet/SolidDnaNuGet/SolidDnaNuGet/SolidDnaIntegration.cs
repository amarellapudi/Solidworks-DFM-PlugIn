using AngelSix.SolidDna;
using Dna;
using static AngelSix.SolidDna.SolidWorksEnvironment;

namespace SolidDnaNuGet
{
    /// <summary>
    /// Register as a SolidWorks Add-In
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
        /// Called when Dependency Injection is being setup
        /// </summary>
        /// <param name="construction">The framework construction</param>
        public override void ConfigureServices(FrameworkConstruction construction)
        {
            // Example
            // -------
            //
            // Add a service like this (include using Microsoft.Extensions.DependencyInjection):
            // construction.Services.AddSingleton(new SomeClass());
            //
            // Retrieve the service anywhere in our application like this
            // Dna.Framework.Service<SomeClass>();
        }

        /// <summary>
        /// Use this to do early initialization and any configuration of the
        /// PlugInIntegration class properties such as <see cref="PlugInIntegration.UseDetachedAppDomain"/>
        /// </summary>
        public override void PreConnectToSolidWorks()
        {
        }

        /// <summary>
        /// Steps to take before any plug-in loads
        /// </summary>
        public override void PreLoadPlugIns()
        {
        }
    }

    /// <summary>
    /// Registers as a SolidDna PlugIn to be laoded by our AddIn Integration class
    /// when the SOlidWorks ass-in gets loaded.
    /// 
    /// NOTE: We can have multiple plug-ins for a single add-in
    /// </summary>
    public class MySolidDnaPlugIn : SolidPlugIn
    {
        #region Public Properties

        /// <summary>
        /// My Add-in description
        /// </summary>
        public override string AddInTitle => "My AddIn Title";

        /// <summary>
        /// My Add-in description
        /// </summary>
        public override string AddInDescription => "My AddIn Description";

        #endregion

        #region Connect to SolidWorks

        public override void ConnectedToSolidWorks()
        {
            Application.ShowMessageBox("Our first SolidDna add-in.... how easy was that? :)", SolidWorksMessageBoxIcon.Information);
        }

        public override void DisconnectedFromSolidWorks()
        {
        }

        #endregion
    }
}
