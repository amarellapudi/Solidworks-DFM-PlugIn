using AngelSix.SolidDna;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows.Controls;
using static AngelSix.SolidDna.SolidWorksEnvironment;
using static System.Windows.Visibility;
using SolidWorks.Interop.sldworks;
using System.Diagnostics;
using SolidWorks.Interop.swconst;

namespace SongTelenkoDFM2
{
    /// <summary>
    /// Interaction logic for CustomPropertiesUI.xaml
    /// </summary>
    public partial class CustomPropertiesUI : UserControl
    {
        #region Private Members

        /// <summary>
        /// Define private strings for custom properties buttons in CustomPropertiesUI.xaml
        /// </summary>

        private const string FeatureCheck = "Feature DFM-Ready";

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

                // Query all custom properties
                model.CustomProperties((properties) =>
                {
                    // Feature Data
                    FeatureData1.Text = properties.FirstOrDefault(property => string.Equals(FeatureCheck, property.Name, StringComparison.InvariantCultureIgnoreCase))?.ResolvedValue;
                });
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
                    if (haveFeature)
                        FeatureButton.IsEnabled = haveFeature;
                    else
                        FeatureButton.IsEnabled = false;
                        FeatureData1.Text = $"";
                        FeatureData2.Text = $"";
                        FeatureData3.Text = $"";
                        FeatureData4.Text = $"";
                        
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
                var haveFeature = objects.Any(f => f.IsFeature);
                
                // Get the newest feature
                var lastFeature = objects.LastOrDefault(f => f.IsFeature);

                // Double check we have one
                if (lastFeature == null)
                    return;

                var featureSelectionName = string.Empty;

                // Get the feature type name
                var copy = lastFeature;
                lastFeature.AsFeature((feature) => featureSelectionName = feature.FeatureTypeName);

                IterateDimensions();

                Feature_Check();

                // Set the feature button text
                ThreadHelpers.RunOnUIThread(() =>
                {
                    if (haveFeature)
                        FeatureData1.Text = $"{featureSelectionName}";
                });
            });
        }

        #endregion

        #region Private Helper Functions

        /// <summary>
        /// Determines if the user has selected a feature in SolidWorks
        /// </summary>
        /// <param name="objects"></param>
        /// <returns>A tuple of a boolean, a string containing the name, and an object of the feature itself</returns>
        private Tuple<bool, string, SelectedObject> HaveFeature(List<SelectedObject> objects)
        {
            var haveFeature = objects.Any(f => f.IsFeature);

            // Get the newest feature
            var lastFeature = objects.LastOrDefault(f => f.IsFeature);

            // Double check we have one
            if (lastFeature == null)
                return Tuple.Create(false, string.Empty, lastFeature);

            var featureSelectionName = string.Empty;

            // Get the feature type name
            lastFeature.AsFeature((feature) => featureSelectionName = feature.FeatureTypeName);

            return Tuple.Create(true, featureSelectionName, lastFeature);
        }

        /// <summary>
        /// Gathers feature data
        /// TO DO: gather all relevant feature data for feature-specific DFM check
        /// </summary>
        private void Feature_Check()
        {
            var model = (ModelDoc2)Application.UnsafeObject.ActiveDoc;
            var featureManager = model.FeatureManager;
            var featureStatistics = featureManager.FeatureStatistics;
            var modelExtension = model.Extension;

            // FeatureDebug(featureStatistics, model);
         
            var featureArray = (object[])featureManager.GetFeatures(false);

            for (var i = featureArray.GetLowerBound(0); i <= featureArray.GetUpperBound(0); i++)
            {
                var featureIter = default(Feature);
                featureIter = (Feature)featureArray[i];
                var featureName = featureIter.Name;

                if (featureName.Contains("Hole"))
                {
                    var depth = GetFeatureDimension(featureIter, model, "Hole Depth@Sketch4");
                    var diameter = GetFeatureDimension(featureIter, model, "Hole Dia.@Sketch4");
                    if ((double)depth/diameter >= 2.75)
                    {
                        Application.ShowMessageBox("The drill hole is too narrow and deep", SolidWorksMessageBoxIcon.Stop);
                    }
                }

                if (featureName.Contains("Sketch"))
                {
                    var parents = (object[])featureIter.GetParents();
                    if (parents != null)
                        foreach (var parent in parents)
                            Debug.Print(((Feature)parent).Name);
                }

                //GetFeatureDimension(featureIter, model, "Extrude", "D1@Boss-Extrude1");
            }
        }

        private double GetFeatureDimension(Feature f, ModelDoc2 model, string parameter)
        {
            var swDim = default(Dimension);
            object configNames = null;
            double[] values = null;
            swDim = (Dimension)model.Parameter(parameter);
            Debug.Assert((swDim != null));
            configNames = model.GetConfigurationNames();
            values = (double[])swDim.GetSystemValue3((int)swInConfigurationOpts_e.swThisConfiguration, (configNames));
            Debug.Print("Dimension = " + values[0] * 1000.0 + "" + " mm");
            return (values[0] * 1000.0);
        }

        private void FeatureDebug(FeatureStatistics featureStatistics, ModelDoc2 model)
        {
            featureStatistics.Refresh();

            Debug.Print("Number of features: " + featureStatistics.FeatureCount);
            Debug.Print("Number of solid bodies: " + featureStatistics.SolidBodiesCount);

            var features2 = (object[])featureStatistics.Features;
            var featnames = (string[])featureStatistics.FeatureNames;
            var feattypes = (int[])featureStatistics.FeatureTypes;
            if ((featnames != null))
            {
                for (var iter = 0; iter <= featnames.GetUpperBound(0); iter++)
                {
                    Debug.Print("Feature name: " + featnames[iter]);
                    Debug.Print("Feature type as defined in sw_SelectType_e: " + feattypes[iter]);
                    Debug.Print("");
                }
            }
        }

        private void IterateDimensions()
        {
            var model = (ModelDoc2)Application.UnsafeObject.ActiveDoc;
            var featureManager = model.FeatureManager;
            var featureStatistics = featureManager.FeatureStatistics;
            var modelExtension = model.Extension;

            Feature swSubFeat;
            DisplayDimension swDispDim;
            Dimension swDim;
            Annotation swAnn;

            var swFeat = (Feature)model.FirstFeature();

            while (swFeat != null)
            {
                Debug.Print("Name: " + swFeat.Name);
                swSubFeat = (Feature)swFeat.GetFirstSubFeature();
                while (swSubFeat != null)
                {
                    Debug.Print("Sub Feature: " + swSubFeat.Name);
                    swDispDim = (DisplayDimension)swSubFeat.GetFirstDisplayDimension();
                    while (swDispDim != null)
                    {
                        swAnn = (Annotation)swDispDim.GetAnnotation();
                        swDim = (Dimension)swDispDim.GetDimension();
                        Debug.Print(swDim.FullName + " " + swDim.GetSystemValue2(""));
                        swDispDim = (DisplayDimension)swSubFeat.GetNextDisplayDimension(swDispDim);
                    }

                    swSubFeat = (Feature)swSubFeat.GetNextSubFeature();
                }
                swDispDim = (DisplayDimension)swFeat.GetFirstDisplayDimension();
                while (swDispDim != null)
                {
                    swAnn = (Annotation)swDispDim.GetAnnotation();
                    swDim = (Dimension)swDispDim.GetDimension();
                    swDispDim = (DisplayDimension)swSubFeat.GetNextDisplayDimension(swDispDim);
                    Debug.Print(swDim.FullName + " " + swDim.GetSystemValue2(""));
                    swSubFeat = (Feature)swSubFeat.GetNextSubFeature();
                }
                swFeat = (Feature)swFeat.GetNextFeature();
            }
        }
        #endregion
    }
}