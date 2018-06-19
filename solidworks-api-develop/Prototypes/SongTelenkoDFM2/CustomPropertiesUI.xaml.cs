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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SongTelenkoDFM2
{
    /// <summary>
    /// Interaction logic for CustomPropertiesUI.xaml
    /// </summary>
    public partial class CustomPropertiesUI : UserControl
    {
        #region Public Members

        #endregion

        #region Private Members

        /// <summary>
        /// Define private strings for custom properties buttons in CustomPropertiesUI.xaml
        /// </summary>

        private const string CustomPropertyNote1 = "Note1";
        private const string CustomPropertyNote2 = "Note2";
        private const string CustomPropertyNote3 = "Note3";

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
            SolidWorksEnvironment.Application.ActiveModelInformationChanged += Application_ActiveModelInformationChanged;
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
                var model = SolidWorksEnvironment.Application.ActiveModel;

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
                    // Design Recommendations
                    
                    // Design Tolerances

                    // Note2
                    NoteText1.Text = properties.FirstOrDefault(property => string.Equals(CustomPropertyNote1, property.Name, StringComparison.InvariantCultureIgnoreCase))?.Value;
                    NoteText2.Text = properties.FirstOrDefault(property => string.Equals(CustomPropertyNote2, property.Name, StringComparison.InvariantCultureIgnoreCase))?.Value;
                    NoteText3.Text = properties.FirstOrDefault(property => string.Equals(CustomPropertyNote3, property.Name, StringComparison.InvariantCultureIgnoreCase))?.Value;
                });

                // Mass
                MassText.Text = model.MassProperties?.MassInMetric();

                // Get all materials
                var materials = SolidWorksEnvironment.Application.GetMaterials();
                materials.Insert(0, new Material { Name = "Remove Material", Classification = "Not specified", DatabaseFileFound = false });

                RawMaterialList.ItemsSource = materials;
                RawMaterialList.DisplayMemberPath = "DisplayName";

                // Clear selection
                RawMaterialList.SelectedIndex = -1;

                // Select existing material
                var existingMaterial = model.GetMaterial();

                // If we have a material
                if (existingMaterial != null)
                    RawMaterialList.SelectedItem = materials?.FirstOrDefault(f => f.Database == existingMaterial.Database && f.Name == existingMaterial.Name);
            });
        }

        /// <summary>
        /// Checks for change in model selection
        /// This is what the user is clicking on (feature, face, drawing, dimension, etc)
        /// </summary>
        private void Model_SelectionChanged()
        {
            SolidWorksEnvironment.Application.ActiveModel?.SelectedObjects((objects) =>
            {
                
                ThreadHelpers.RunOnUIThread(() =>
                {
                                        
                });
            });
        }

        #endregion

        #region Button Events

        /// <summary>
        /// Called when the read button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ReadDetails();
        }

        /// <summary>
        /// Called when the reset button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Clear all values
            RawMaterialList.SelectedIndex = -1;
            NoteText1.Text = string.Empty;
            NoteText2.Text = string.Empty;
            NoteText3.Text = string.Empty;
        }

        /// <summary>
        /// Called when the apply button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var model = SolidWorksEnvironment.Application.ActiveModel;

            // Check if we have a part
            if (model == null || !model.IsPart)
                return;

            // Note textboxes
            model.SetCustomProperty(CustomPropertyNote1, NoteText1.Text);
            model.SetCustomProperty(CustomPropertyNote2, NoteText2.Text);
            model.SetCustomProperty(CustomPropertyNote3, NoteText3.Text);

            // If user does not have a material selected, clear it
            if (RawMaterialList.SelectedIndex < 0)
                model.SetMaterial(null);
            // Otherwise set the material to the selected one
            else
                model.SetMaterial((Material)RawMaterialList.SelectedItem);

            // Re-read details to confirm they are correct
            ReadDetails();
        }

        /// <summary>
        /// Get selected feature and analyze it with a DMF routine
        /// TO DO: actually get feature data (dimensions, coordinates, etc)
        /// </summary>
        private void DesignCheckButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SolidWorksEnvironment.Application.ActiveModel?.SelectedObjects((objects) =>
            {
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
            var model = (ModelDoc2)SolidWorksEnvironment.Application.UnsafeObject.ActiveDoc;
            var selectionManager = model.SelectionManager;
            var featureManager = model.FeatureManager;

            var AllFeatures = (object[])featureManager.GetFeatures(false);

            List<List<Feature>> filteredFeatures = GetFeatureDictionary(AllFeatures);

            foreach (List<Feature> featureSketchList in filteredFeatures)
            {
                Feature headFeature = featureSketchList.First();
                var name = headFeature.Name;
                Dictionary<string, double> dims = GetDimensions(headFeature);

                model.Extension.SelectByID2(headFeature.Name, "BODYFEATURE", 0, 0, 0, false, 0, null, 0);

                // Configure the message box to be displayed
                string messageBoxText = "Is the highlighted feature, " + name + ", necessary for this part?";
                string formTitle = "Critical Features";

                ToleranceMessageBox.ShowMessage(messageBoxText, formTitle);


                //MessageBoxButton button = MessageBoxButton.YesNo;
                //MessageBoxImage icon = MessageBoxImage.Question;
                //// Display message box
                //MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                //switch (result)
                //{
                //    case MessageBoxResult.Yes:
                //        break;
                //    case MessageBoxResult.No:
                //        break;
                //}
            } 
        }

        /// <summary>
        /// Determine if a feature is necessary
        /// </summary>
        /// <param name="feature"></param>
        /// <returns>The passed-in feature if it is necessary</returns>
        private Feature IsNecessaryFeature(object feature)
        {
            if (feature != null)
            {
                if (((Feature)feature).Name == "Comments") return null;
                else if (((Feature)feature).Name == "Favorites") return null;
                else if (((Feature)feature).Name == "History") return null;
                else if (((Feature)feature).Name == "Selection Sets") return null;
                else if (((Feature)feature).Name == "Sensors") return null;
                else if (((Feature)feature).Name == "Design Binder") return null;
                else if (((Feature)feature).Name == "Annotations") return null;
                else if (((Feature)feature).Name == "Surface Bodies") return null;
                else if (((Feature)feature).Name == "Solid Bodies") return null;
                else if (((Feature)feature).Name == "Lights, Cameras and Scene") return null;
                else if (((Feature)feature).Name == "Equations") return null;
                else if (((Feature)feature).Name.Contains("Material")) return null;
                else if (((Feature)feature).Name == "Front Plane") return null;
                else if (((Feature)feature).Name == "Top Plane") return null;
                else if (((Feature)feature).Name == "Right Plane") return null;
                else if (((Feature)feature).Name == "Origin") return null;
                else if (((Feature)feature).Name.Contains("Notes")) return null;
                else if (((Feature)feature).Name == "Ambient") return null;
                else if (((Feature)feature).Name.Contains("Directional")) return null;
                else if (((Feature)feature).Name.Contains("Sketch")) return null;
                else
                {
                    return (Feature)feature;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Make a list of necessary features in insertion order
        /// Each value is a list of features containing the parent feature and the child sketches
        /// </summary>
        /// <param name="FeatureSet"></param>
        /// <returns></returns>
        private List<List<Feature>> GetFeatureDictionary(object[] FeatureSet)
        {
            // Start empty dictionary
            List<List<Feature>> featureDictionary = new List<List<Feature>>();

            // Check each feature in the unfiltered feature array that is passed in
            foreach (object feature in FeatureSet)
            {
                // Is the feature necessary?
                Feature featureIteration = IsNecessaryFeature(feature);
                
                // If it is, then find the feature's sketches
                if (featureIteration != null)
                {
                    // Start the feature list entry
                    List<Feature> entry = new List<Feature>();

                    // add the parent feature
                    entry.Add(featureIteration);

                    // Print parent feature's name
                    Debug.Print("Parent feauture: " + featureIteration.Name);

                    // Find and add the sketches underlying the feature
                    List<Feature> sketches = GetSubFeatures(featureIteration);
                    if (sketches != null)
                    {
                        entry.AddRange(sketches);
                    }

                    // Add list of feature and sketches to the main featureDictionary list
                    featureDictionary.Add(entry);

                    Debug.Print(" ");
                }
            }

            // return the main featureDictionary list
            return featureDictionary;
        }

        /// <summary>
        /// Get three levels of subfeatures
        /// TO DO: might need to check more levels, find better way to do this
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private List<Feature> GetSubFeatures(Feature feature)
        {
            // Make an empty list of features
            List<Feature> sketches = new List<Feature>();

            // Find first subfeature
            var sketch = feature.GetFirstSubFeature();
            if (sketch != null)
            {
                // If the subfeature is not null, add it to the dictionary
                Debug.Print("Child Sketch: " + ((Feature)sketch).Name);
                sketches.Add((Feature)sketch);

                // Find second subfeature
                var sketch2 = ((Feature)sketch).GetNextSubFeature();
                if (sketch2 != null)
                {
                    // If the subfeature is not null, add it to the dictionary
                    Debug.Print("Child Sketch: " + ((Feature)sketch2).Name);
                    sketches.Add((Feature)sketch2);

                    // Find third subfeature
                    var sketch3 = ((Feature)sketch2).GetNextSubFeature();
                    if (sketch3 != null)
                    {
                        // If the subfeature is not null, add it to the dictionary
                        Debug.Print("Child Sketch: " + ((Feature)sketch3).Name);
                        sketches.Add((Feature)sketch3);
                    }
                }
            }

            if (sketches.Count == 0)
            {
                return null;
            } else {
                return sketches;
            }
        }

        /// <summary>
        /// Get three levels of dimensions
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private Dictionary<string, double> GetDimensions(Feature feature)
        {
            // Make an empty list of dimensions
            Dictionary<string, double> dims = new Dictionary<string, double>();

            // Find first dimension
            var displayDimension = (DisplayDimension)feature.GetFirstDisplayDimension();

            while (displayDimension != null)
            {
                // If the first display dimension is not null, add it to the list
                Dimension dimension = (Dimension)displayDimension.GetDimension();
                if (dimension.FullName.Contains("Angle")) {
                    Debug.Print(dimension.FullName + " " + dimension.GetSystemValue2("") + "rads");
                } else {
                    Debug.Print(dimension.FullName + " " + 1000.0 * dimension.GetSystemValue2("") + "mm");
                }
                
                dims.Add(dimension.FullName, dimension.GetSystemValue2(""));

                // Get the next display dimension
                displayDimension = (DisplayDimension)feature.GetNextDisplayDimension(displayDimension);
            }

            Debug.Print(" ");

            if (dims.Count == 0)
            {
                return null;
            }
            else
            {
                return dims;
            }
        }

        /// <summary>
        /// Get a specific dimension by string
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="model"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private double GetDimension(Feature feature, ModelDoc2 model, string param)
        {
            var dimension = default(Dimension);
            object configNames = null;
            double[] values = null;
            configNames = model.GetConfigurationNames();

            //dimension = (Dimension)model.Parameter(parameter);
            dimension = (Dimension)feature.Parameter(param);
            Debug.Assert((dimension != null));
            
            values = (double[])dimension.GetSystemValue3((int)swInConfigurationOpts_e.swThisConfiguration, (configNames));
            // Debug.Print("Dimension = " + values[0] * 1000.0 + "" + " mm");
            return (values[0] * 1000.0);
        }

        /// <summary>
        /// Debugging function for printing information about features in current part
        /// </summary>
        /// <param name="featureManager"></param>
        /// <param name="model"></param>
        private void FeatureStatistics(FeatureManager featureManager, ModelDoc2 model)
        {
            var featureStatistics = featureManager.FeatureStatistics;
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