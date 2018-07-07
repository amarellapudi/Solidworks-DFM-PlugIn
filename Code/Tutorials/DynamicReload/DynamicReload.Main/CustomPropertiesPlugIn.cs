using AngelSix.SolidDna;
using Dna;
using SolidWorks.Interop.sldworks;
using System.IO;

namespace DynamicReload
{
    /// <summary>
    /// Register as SolidDna Plug-in
    /// </summary>
    public class CustomPropertiesPlugIn : SolidPlugIn
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
        public override string AddInTitle => "Ruoyu Song DMF Plug-In";

        #endregion

        #region Connect To SolidWorks

        public override void ConnectedToSolidWorks()
        {
            // Create our taskpane
            mTaskpane = new TaskpaneIntegration<MyTaskpaneUI>()
            {
                Icon = Path.Combine(this.AssemblyPath(), "logo-small.png"),
                WpfControl = new CustomPropertiesUI()
            };

            mTaskpane.AddToTaskpaneAsync();
        }

        public override void DisconnectedFromSolidWorks()
        {

        }

        #endregion
    }
}
