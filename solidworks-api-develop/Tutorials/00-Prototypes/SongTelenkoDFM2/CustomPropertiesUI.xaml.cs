using AngelSix.SolidDna;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows.Controls;
using static AngelSix.SolidDna.SolidWorksEnvironment;
using static System.Windows.Visibility;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SongTelenkoDFM
{
    /// <summary>
    /// Interaction logic for CustomPropertiesUI.xaml
    /// </summary>
    public partial class CustomPropertiesUI : UserControl
    {
        #region Public Members

        private void Feature_Check()
        {
            var mModel = default(ModelDoc2);
            var mDim = default(Dimension);
            object mConfigNames = null;
            double[] mValue = null;
            mModel = (ModelDoc2)Application.UnsafeObject.ActiveDoc;

            var swFeatStat = default(FeatureStatistics);
            var swFeatMgr = default(FeatureManager);
            string[] featnames = null;
            int[] feattypes = null;
            object[]
            features = null;
            double[] featureUpdateTimes = null;
            double[] featureUpdatePercentTimes = null;
            var iter = 0;

            swFeatMgr = mModel.FeatureManager;
            swFeatStat = swFeatMgr.FeatureStatistics;

            swFeatStat.Refresh();

            Debug.Print("Model name: " + swFeatStat.PartName);
            Debug.Print("Number of features: " + swFeatStat.FeatureCount);
            Debug.Print("Number of solid bodies: " + swFeatStat.SolidBodiesCount);
            Debug.Print("Number of surface bodies: " + swFeatStat.SurfaceBodiesCount);
            Debug.Print("Total rebuild time: " + swFeatStat.TotalRebuildTime);
            Debug.Print("");
            features = (object[])swFeatStat.Features;
            featnames = (string[])swFeatStat.FeatureNames;
            feattypes = (int[])swFeatStat.FeatureTypes;
            featureUpdateTimes = (double[])swFeatStat.FeatureUpdateTimes;
            featureUpdatePercentTimes = (double[])swFeatStat.FeatureUpdatePercentageTimes;
            if ((featnames != null))
            {
                for (iter = 0; iter <= featnames.GetUpperBound(0); iter++)
                {
                    Debug.Print("Feature name: " + featnames[iter]);
                    Debug.Print("Feature created: " + ((Feature)features[iter]).DateCreated);
                    Debug.Print("Feature description: " + ((Feature)features[iter]).EnumDisplayDimensions());
                    Debug.Print("Feature type as defined in sw_SelectType_e: " + feattypes[iter]);
                    Debug.Print("");
                }
            }

            //mDim = (Dimension)mModel.Parameter("D1@Boss-Extrude1");
            //var q = mModel.GetFeatureCount();

            //Debug.Assert((mDim != null));
            //Debug.Print("File = " + mModel.GetPathName());
            //Debug.Print("  Full name = " + mDim.FullName);
            //Debug.Print("  Name = " + mDim.Name);

            //mConfigNames = mModel.GetConfigurationNames();
            //mValue = (double[])mDim.GetSystemValue3((int)swInConfigurationOpts_e.swThisConfiguration, (mConfigNames));

            //Debug.Print("  System value = " + mValue[0] * 1000.0 + "" + " mm");
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Define private strings for custom properties buttons in CustomPropertiesUI.xaml
        /// </summary>
       
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomPropertiesUI()
        {
            InitializeComponent();
        }

        #endregion

        #region Startup

        /// <summary>
        /// Fired when the control is fully loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // By default show the no part open screen
            // and hide the analyze part and main content screens
            NoPartContent.Visibility = Visible;
            MainContent.Visibility = Hidden;

            // Listen out for the active model changing
            Application.ActiveModelInformationChanged += Application_ActiveModelInformationChanged;
        }

        #endregion

        #region Model Events

        /// <summary>
        /// Fired when the actice SolidWorks model is changed
        /// </summary>
        /// <param name="obj"></param>
        private void Application_ActiveModelInformationChanged(Model obj)
        {
            ReadDetails();
        }

        /// <summary>
        /// Reads all the details from the active model
        /// </summary>
        private void ReadDetails()
        {
            ThreadHelpers.RunOnUIThread(() =>
            {
                // Get the active model
                var model = Application.ActiveModel;

                // If we have no model, or the model is nor a part
                // then we show the No Part screen and return
                if (model == null || (!model.IsPart && !model.IsAssembly))
                {
                    // Show No Part screen
                    NoPartContent.Visibility = Visible;
                    MainContent.Visibility = Hidden;

                    return;
                }

                // If we got here, we have a part

                // Listen out for selection changes
                model.SelectionChanged += Model_SelectionChanged;

                // Show the main content
                NoPartContent.Visibility = Hidden;
                MainContent.Visibility = Visible;
            });
        }

        /// <summary>
        /// Checks for change in model selection
        /// This is what the user is clicking on (feature, face, drawing, dimension, etc)
        /// </summary>
        private void Model_SelectionChanged()
        {
            Application.ActiveModel?.SelectedObjects((objects) =>
            {
                var haveFeature = objects.Any(f => f.IsFeature);

                ThreadHelpers.RunOnUIThread(() =>
                {
                    
                });
            });
        }

        #endregion

        #region Button Events

        /// <summary>
        /// Get selected feature and analyze it with a DMF routine
        /// TO DO: actually get feature data (dimensions, coordinates, etc)
        /// </summary>
        private void FeatureButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.ActiveModel?.SelectedObjects((objects) =>
            {
                // Get the newest feature
                var lastFeature = objects.LastOrDefault(f => f.IsFeature);

                // Double check we have one
                if (lastFeature == null)
                    return;

                var featureSelectionName = string.Empty;

                // Get the feature type name
                lastFeature.AsFeature((feature) => featureSelectionName = feature.FeatureTypeName);

                //Perform DFM functionality for drill holes
                //if (featureSelectionName.Equals("Extrusion"))
                //{ 
                    //var type = lastFeature.GetType();
                    //var type2 = lastFeature.ObjectType;
                    //var type3 = lastFeature.UnsafeObject.GetType();
                //}

                Feature_Check();

                // Set the feature button text
                ThreadHelpers.RunOnUIThread(() =>
                {
                   
                });
            });
        }

        #endregion
    }
}