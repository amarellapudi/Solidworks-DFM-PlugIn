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
using System.Windows.Forms;

namespace SongTelenkoDFM2
{    /// <summary>
    /// Interaction logic for CustomPropertiesUI.xaml
    /// </summary>
    public partial class CustomPropertiesUI : System.Windows.Controls.UserControl
    {
        #region Public Members

        public class FeatureNameAndTolerance
        {
            public string FeatureName { get; set; }
            public string FeatureTolerance { get; set; }
        }

        public List<FeatureNameAndTolerance> mFeatureTolerances = new List<FeatureNameAndTolerance>();

        #endregion

        #region Private Members

        /// <summary>
        /// Define private strings for custom properties buttons in CustomPropertiesUI.xaml
        /// </summary>

        private const string CustomPropertyNote1 = "Note1";
        private const string CustomPropertyNote2 = "Note2";
        private const string CustomPropertyNote3 = "Note3";

        private const string CustomPropertyTolerance_Feature1 = "Tolerance_Feature1";
        private const string CustomPropertyTolerance_Value1 = "Tolerance_Value1";

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomPropertiesUI()
        {
            InitializeComponent();

            FeatureTolerance_Display.ItemsSource = mFeatureTolerances;
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
                    // TO DO: What's the best place to put the feature tolerances?

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
            //mFeatureTolerances.Clear();

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
                Tolerance_Check();

                // Set the feature button text
                ThreadHelpers.RunOnUIThread(() =>
                {

                });
            });
        }

        #endregion

        #region Private Helper Functions

        /// <summary>
        /// Asks user to specify tolerances for each feature
        /// </summary>
        private void Tolerance_Check()
        {
            // Clear the old tolerances
            mFeatureTolerances.Clear();

            var model = (ModelDoc2)SolidWorksEnvironment.Application.UnsafeObject.ActiveDoc;
            var selectionManager = model.SelectionManager;
            var featureManager = model.FeatureManager;

            var AllFeatures = (object[])featureManager.GetFeatures(false);

            List<List<Feature>> filteredFeatures = GetFeatureDictionary(AllFeatures);

            List<string[]> featureTolerances = new List<string[]>();

            for (int i=0; i<filteredFeatures.Count; i++)
            {
                // Get the head feature of each feature-sketch-list
                Feature headFeature = (filteredFeatures[i]).First();

                // Get the dictionary of feature dimensions and values 
                //Dictionary<string, double> dims = GetDimensions(headFeature);

                // Highlight the feature
                model.Extension.SelectByID2(headFeature.Name, "BODYFEATURE", 0, 0, 0, false, 0, null, 0);

                // Get the user specified or default tolerance for this feature
                //string[] thisFeatureTolerance = GetFeatureTolerance(headFeature);
                FeatureNameAndTolerance thisFeatureTolerance = GetFeatureTolerance(headFeature);

                // Add this tolerance to the list of all tolerances
                mFeatureTolerances.Add(thisFeatureTolerance);
            }

            FeatureTolerance_Display.Items.Refresh();
        }

        /// <summary>
        /// Generate dictionary of user-specified tolerances
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private FeatureNameAndTolerance GetFeatureTolerance(Feature feature)
        {
            // Default tolerance string
            string default_tolerance = "+/- 0.1mm";

            // Configure the message box to ask user if this feature is critical
            string messageBoxText = "Is the highlighted feature, " + feature.Name + ", necessary for this part?";
            string formTitle = "Critical Features";

            // Use the custom FeatureCriticalMessageBox class to ask the user
            FeatureCriticalMessageBox isFeatureCritical = new FeatureCriticalMessageBox(messageBoxText, formTitle);
            DialogResult criticalFeature_result = isFeatureCritical.ShowDialog();

            // Based on the result, either ask the user for a more specific tolerance or use the default tolerance
            if (criticalFeature_result == DialogResult.Yes)
            {
                // User has identified this feature as critical, so it may require a tighter tolerance
                string messageBoxText2 = "The default tolerance is +/- 0.1mm. Would you like to change this value for " + feature.Name + "?";
                string formTitle2 = "Critical Feature Tolerance";

                // Display Tolerance Message Box to the user and wait for tolerance selection
                ToleranceMessageBox CriticalFeatureTolerance = new ToleranceMessageBox(messageBoxText2, formTitle2);
                DialogResult tolerance_result = CriticalFeatureTolerance.ShowDialog();

                string[] res = { feature.Name, CriticalFeatureTolerance.Tolerance_Value };

                FeatureNameAndTolerance result = new FeatureNameAndTolerance()
                {
                    FeatureName = feature.Name,
                    FeatureTolerance = CriticalFeatureTolerance.Tolerance_Value
                };

                return result;
            }
            // User has not identified this feature as critical, so return the default tolerance
            else
            {
                string[] res2 = { feature.Name, default_tolerance };

                FeatureNameAndTolerance result2 = new FeatureNameAndTolerance()
                {
                    FeatureName = feature.Name,
                    FeatureTolerance = default_tolerance
                };

                return result2;
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
                else if (((Feature)feature).GetTypeName2().Contains("Material")) return null;
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
                    //Debug.Print("Parent feauture: " + featureIteration.Name);

                    // Find and add the sketches underlying the feature
                    List<Feature> sketches = GetSubFeatures(featureIteration);
                    if (sketches != null)
                    {
                        entry.AddRange(sketches);
                    }

                    // Add list of feature and sketches to the main featureDictionary list
                    featureDictionary.Add(entry);

                    //Debug.Print(" ");
                }
            }

            // return the main featureDictionary list
            return featureDictionary;
        }

        /// <summary>
        /// Get all levels of subfeatures of a given feature
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private List<Feature> GetSubFeatures(Feature feature)
        {
            // Make an empty list of features
            List<Feature> sketches = new List<Feature>();

            // Find first subfeature
            var sketch = feature.GetFirstSubFeature();

            while (sketch != null)
            {
                // If the subfeature is not null, add it to the dictionary
                // Debug.Print("Child Sketch: " + ((Feature)sketch).Name);
                sketches.Add((Feature)sketch);

                // Find the next subfeature
                sketch = ((Feature)sketch).GetNextSubFeature();
            }

            if (sketches.Count == 0)
            {
                return null;
            } else {
                return sketches;
            }
        }

        /// <summary>
        /// Get all dimensions relating to a feature
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
                    //Debug.Print(dimension.FullName + " " + dimension.GetSystemValue2("") + "rads");
                } else {
                    //Debug.Print(dimension.FullName + " " + 1000.0 * dimension.GetSystemValue2("") + "mm");
                }
                
                dims.Add(dimension.FullName, dimension.GetSystemValue2(""));

                // Get the next display dimension
                displayDimension = (DisplayDimension)feature.GetNextDisplayDimension(displayDimension);
            }

            //Debug.Print(" ");

            if (dims.Count == 0)
            {
                return null;
            }
            else
            {
                return dims;
            }
        }
        
        #endregion
    }
}