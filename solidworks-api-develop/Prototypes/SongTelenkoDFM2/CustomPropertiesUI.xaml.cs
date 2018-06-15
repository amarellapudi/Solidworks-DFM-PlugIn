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
                DesignCheckButton.IsEnabled = true;

                // Query all custom properties
                model.CustomProperties((properties) =>
                {
                    // Feature Data
                    // FeatureData1.Text = properties.FirstOrDefault(property => string.Equals(FeatureCheck, property.Name, StringComparison.InvariantCultureIgnoreCase))?.ResolvedValue;
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
                // var haveFeature = objects.Any(f => f.IsFeature);
                // var haveDimension = objects.Any(f => f.IsDimension);

                ThreadHelpers.RunOnUIThread(() =>
                {
                    // DesignCheckButton.IsEnabled = haveFeature | haveDimension;                      
                });
            });
        }

        #endregion

        #region Button Events

        /// <summary>
        /// Get selected feature and analyze it with a DMF routine
        /// TO DO: actually get feature data (dimensions, coordinates, etc)
        /// </summary>
        private void DesignCheckButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.ActiveModel?.SelectedObjects((objects) =>
            {
                var haveFeature = objects.Any(f => f.IsFeature);

                Feature_Check();

                // Set the feature button text
                ThreadHelpers.RunOnUIThread(() =>
                {

                });
            });
        }

        #endregion

        #region Private Helper Functions

        /// <summary>
        /// Gathers feature data
        /// TO DO: gather all relevant feature data for feature-specific DFM check
        /// </summary>
        private void Feature_Check()
        {
            var model = (ModelDoc2)Application.UnsafeObject.ActiveDoc;
            var featureManager = model.FeatureManager;
            
            //var featureStatistics = featureManager.FeatureStatistics;
            //var modelExtension = model.Extension;
            // FeatureDebug(featureStatistics, model);

            List<Feature> filteredFeatures = RemoveUnnecessaryFeatures((object[])featureManager.GetFeatures(false));
            var filteredFeaturesNames = new List<string>();
            foreach (Feature feature in filteredFeatures)
            {
                filteredFeaturesNames.Add(feature.Name);
            }


            foreach (Feature feature in filteredFeatures)
            {
                string name = feature.Name;
                Debug.Print("Parent Feature: " + name);
                if (name.Contains("Hole"))
                {
                    var child = feature.GetFirstSubFeature();
                    if (child != null)
                    {
                        Debug.Print("Child Feature: "+((Feature)child).Name);
                    }

                    var child2 = feature.GetNextSubFeature();
                    if (child2 != null)
                    {
                        Debug.Print("Child Feature: " + ((Feature)child2).Name);

                    }

                    var depth = GetFeatureDimension(feature, model, "Hole Depth@Sketch4");
                    var diameter = GetFeatureDimension(feature, model, "Hole Dia.@Sketch4");
                    if ((double)depth/diameter >= 2.75)
                    {
                        Application.ShowMessageBox("The drill hole is too narrow and deep", SolidWorksMessageBoxIcon.Stop);
                    }
                }

                if (name.Contains("Extrude"))
                {
                    var child = feature.GetFirstSubFeature();
                    if (child != null)
                    {
                        Debug.Print("Child Feature: " + ((Feature)child).Name);
                    }

                    var child2 = feature.GetNextSubFeature();
                    if (child2 != null)
                    {
                        Debug.Print("Child Feature: " + ((Feature)child2).Name);

                    }
                    //GetFeatureDimension(featureIter, model, "Extrude", "D1@Boss-Extrude1");
                }

                if (name.Contains("Sketch4"))
                {
                    var child = feature.GetChildren();
                    if (child != null)
                    {
                        Debug.Print("Child Feature: " + ((Feature)child).Name);
                    }
                }
            }
        }

        private List<Feature> RemoveUnnecessaryFeatures(object[] features)
        {
            if (features != null)
            {
                var featureList = new List<Feature>();
                foreach (var feature in features)
                {
                    if (((Feature)feature).Name == "Comments") continue;
                    else if (((Feature)feature).Name == "Favorites") continue;
                    else if (((Feature)feature).Name == "History") continue;
                    else if (((Feature)feature).Name == "Selection Sets") continue;
                    else if (((Feature)feature).Name == "Sensors") continue;
                    else if (((Feature)feature).Name == "Design Binder") continue;
                    else if (((Feature)feature).Name == "Annotations") continue;
                    else if (((Feature)feature).Name == "Surface Bodies") continue;
                    else if (((Feature)feature).Name == "Solid Bodies") continue;
                    else if (((Feature)feature).Name == "Lights, Cameras and Scene") continue;
                    else if (((Feature)feature).Name == "Equations") continue;
                    else if (((Feature)feature).Name.Contains("Material")) continue;
                    else if (((Feature)feature).Name == "Front Plane") continue;
                    else if (((Feature)feature).Name == "Top Plane") continue;
                    else if (((Feature)feature).Name == "Right Plane") continue;
                    else if (((Feature)feature).Name == "Origin") continue;
                    else if (((Feature)feature).Name.Contains("Notes")) continue;
                    else if (((Feature)feature).Name == "Ambient") continue;
                    else if (((Feature)feature).Name.Contains("Directional")) continue;
                    else
                    {
                        featureList.Add((Feature)feature);
                    }
                }
                return featureList;
            }
            else return new List<Feature>();
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
            // Debug.Print("Dimension = " + values[0] * 1000.0 + "" + " mm");
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

       
        #endregion
    }
}