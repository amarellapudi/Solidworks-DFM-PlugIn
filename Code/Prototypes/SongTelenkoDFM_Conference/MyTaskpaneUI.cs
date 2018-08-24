using System.Windows.Forms;
using System.Runtime.InteropServices;
using AngelSix.SolidDna;

namespace SongTelenkoDFM_Conference
{
    [ProgId(MyProgId)]
    public partial class MyTaskpaneUI : UserControl, ITaskpaneControl
    {
        #region Private Members

        /// <summary>
        /// Our unique ProgId for SolidWorks to find and load us
        /// </summary>
        private const string MyProgId = "SongTelenkoDFM_Offline";

        #endregion

        #region Public Properties

        public string ProgId => MyProgId;

        #endregion
    }
}
