﻿using AngelSix.SolidDna;
using Dna;
using System.IO;
using static AngelSix.SolidDna.SolidWorksEnvironment;

namespace SongTelenkoDFM_Conference
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
    /// Side Bar UI + Command Manager Tools
    /// This section includes strings for this software's description and title (shown in SolidWorks)
    /// Also includes the reference to the icon
    /// </summary>
    public class MySideBar : SolidPlugIn
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
        public override string AddInDescription => "Making machining knowledge more accessible to novices";

        /// <summary>
        /// My Add-in title
        /// </summary>
        public override string AddInTitle => "SongTelenkoDFM v1.2";

        #endregion

        #region Connect To SolidWorks

        public override void ConnectedToSolidWorks()
        {
            /// <summary>
            /// Create our taskpane
            /// <summary>
            mTaskpane = new TaskpaneIntegration<MyTaskpaneUI>()
            {
                Icon = Path.Combine(this.AssemblyPath(), "Image_Logo.png"),
                WpfControl = new UI_SolidWorks_SideBar_PlugIn()
            };

            mTaskpane.AddToTaskpaneAsync();
        }

        public override void DisconnectedFromSolidWorks()
        {

        }

        #endregion
    }
}
