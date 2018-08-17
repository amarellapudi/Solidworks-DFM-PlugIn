using AngelSix.SolidDna;
using System;
using System.Linq;
using System.Collections.Generic;
using static System.Windows.Visibility;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using static SongTelenkoDFM_Conference.Methods_FileExporting;
using System.IO;
using System.Reflection;
using System.Text;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace SongTelenkoDFM_Conference
{    /// <summary>
     /// Interaction logic for UI_SolidWorks_SideBar_PlugIn.xaml
     /// Please read SolidWorks_DFM_PlugIn_Documentation.docx to gain an overview before reading/editing this code
     /// </summary>
    public partial class UI_SolidWorks_SideBar_PlugIn : System.Windows.Controls.UserControl
    {
        #region Public Members

        // Instance
        public static UI_SolidWorks_SideBar_PlugIn Instance;

        // This assembly's location
        public string mHome;

        // Location of the SculptPrint folder relative to this assembly's location
        public static string MSculptPrint_Folder;
        public static string FeedbackPNG_Save_Location;

        // Our SFTP client, logged into www.marellapudi.com/public_ftp/
        //public static SftpClient MClient;

        // Object definition for user-specified tolerances for each feature
        public class FeatureToleranceObject
        {
            public string FeatureName { get; set; }
            public string FeatureTolerance { get; set; }
        }

        // List of all FeatureTolerances
        // Used when writing custom properties to the .SLDPRT file
        public List<FeatureToleranceObject> mFeatureTolerances = new List<FeatureToleranceObject>();

        // Global variable for whether we have a mill or lathe part
        // we need to communicate this to the researcher feedback app
        public bool mill = false;

        // Global int containing current stage in manufacturing check process
        public int step;

        #endregion

        #region Private Members

        /// <summary>
        /// Define private strings for custom properties buttons in UI_SolidWorks_SideBar_PlugIn.xaml
        /// These help format the user-specified custom property strings before they are saved into 
        /// the part's custom properties values
        /// </summary>

        // Notes
        private const string CustomPropertyNote = "Note";

        // Design Recommendations
        private const string CustomPropertyRecommendation = "Recommendation";

        // Feature Tolerances
        private const string CustomPropertyFeatureTolerance = "Tolerance for ";

        #endregion

        #region Constructor and Startup

        /// <summary>
        /// Default constructor and initialization of public members
        /// </summary>
        public UI_SolidWorks_SideBar_PlugIn()
        {
            // Set data context for FeatureTolerance List
            DataContext = this;

            // Default Initialization
            InitializeComponent();
            Instance = this;

            // Set the data context globally, and the item source for the FeatureTolerance List
            FeatureTolerance_Display.ItemsSource = mFeatureTolerances;

            // Set home location to the location of this assembly (aka where this compiled .dll file is)
            // IMPORTANT: The location of this assembly relative to the SculptPrint folder matters
            // The folder hierarchy is defined in SolidWorks_DFM_PlugIn_Documentation.docx, Section 2c
            mHome = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            mHome += "\\";

            // Define the SculptPrint Folder
            // IMPORTANT: This is where files are exported to and uploaded from using SFTP
            // Keep this folder hierarchy consistent with that defined in SolidWorks_DFM_PlugIn_Documentation.docx, Section 2c
            MSculptPrint_Folder = mHome.Replace("\\Code\\Prototypes\\SongTelenkoDFM_Conference\\bin\\Debug", "\\SculptPrint\\Experiment Files\\");

            // Connect to SFTP client
            //MClient = SFTPConnect();

            // Delete previous experiment files stored on the local machine
            // Not doing so may cause errors in the work flow 
            // (The subject may think the researcher already sent a finished flag)
            //DeleteLocal(MSculptPrint_Folder + "DONE_researcher");
            //DeleteLocal(MSculptPrint_Folder + "DONE_subject");
            //DeleteLocal(MSculptPrint_Folder + "test.stl");
            //DeleteLocal(MSculptPrint_Folder + "View_SW.png");
            //DeleteLocal(MSculptPrint_Folder + "View_SP.png");
            //DeleteLocal(MSculptPrint_Folder + "View_Researcher_Feedback.png");
        }

        /// <summary>
        /// Fired when the control is fully loaded
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // By default show the no part open screen
            // and hide the main content screen
            NoPartContent.Visibility = Visible;
            MainContent.Visibility = Hidden;

            // Listen out for the active model changing
            SolidWorksEnvironment.Application.ActiveModelInformationChanged += Application_ActiveModelInformationChanged;
        }

        #endregion

        #region Model Events

        /// <summary>
        /// Fired when the active SolidWorks model is changed
        /// </summary>
        private void Application_ActiveModelInformationChanged(Model obj)
        {
            step = 0;
            // When the active model is changed (meaning, when the user opens or closes a part, assembly, or drawing),
            // read the details of the changed model
            ReadDetails();
        }

        /// <summary>
        /// Checks for change in model selection
        /// Currently unused, but can be used to determine what the user is clicking on (feature, face, drawing, dimension, etc)
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

        /// <summary>
        /// Reads all the details from the active model
        /// </summary>
        private void ReadDetails()
        {
            ThreadHelpers.RunOnUIThread(() =>
            {
                // Get the active model
                var model = SolidWorksEnvironment.Application.ActiveModel;

                // If we have no model, or the model is not a part or assembly
                // then we show the No Part screen and return
                if (model == null || (!model.IsPart && !model.IsAssembly))
                {
                    // Show No Part screen
                    NoPartContent.Visibility = Visible;
                    MainContent.Visibility = Hidden;
                    return;
                }

                // If we got here, we have a part
                var filePath = model.FilePath.Split(new string[] { "\\" }, StringSplitOptions.None);
                var fileName = filePath[filePath.Count() - 1];
                if (fileName.Contains("pawn")) mill = false;
                if (fileName.Contains("door")) mill = true;

                // Listen out for selection changes
                model.SelectionChanged += Model_SelectionChanged;

                // Show the main content
                NoPartContent.Visibility = Hidden;
                MainContent.Visibility = Visible;
                DesignCheckButton.IsEnabled = true;

                // Query all custom properties
                model.CustomProperties((properties) =>
                {
                    // Clear the NoteGrid, which is the grid of user defined Notes
                    // See the design of UI_SolidWorks_SideBar_PlugIn.xaml
                    NoteGrid.Children.Clear();

                    // Find all the properties saved onto the part (onto the .SLDPRT) 
                    // whose name includes the public member CustomPropertyNote = "Note"
                    List<CustomProperty> noteFound = properties.FindAll(property => property.Name.Contains(CustomPropertyNote));

                    // Add each of the properties into the NoteGrid
                    // This way, if a part is opened having many Note custom properties,
                    // then we load them in properly
                    foreach (CustomProperty note in noteFound) AddNewNote(note.Value);
                });

                // Mass
                MassText.Text = model.MassProperties?.MassInMetric();

                // Clear material selection
                List<Material> FilteredMaterials = GetMaterialList();
                RawMaterialList.ItemsSource = FilteredMaterials;
                RawMaterialList.DisplayMemberPath = "DisplayName";
                RawMaterialList.SelectedIndex = -1;

                // Select existing material
                var existingMaterial = model.GetMaterial();

                // If we have a material
                if (existingMaterial != null)
                    RawMaterialList.SelectedItem = FilteredMaterials?.FirstOrDefault(f => f.Database == existingMaterial.Database && f.Name == existingMaterial.Name);
            });
        }
        
        #endregion

        #region Button Events

        /// <summary>
        /// Called when the read button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            ReadDetails();
        }

        /// <summary>
        /// Called when the reset button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var model = SolidWorksEnvironment.Application.ActiveModel;
            var model2 = (ModelDoc2)SolidWorksEnvironment.Application.UnsafeObject.ActiveDoc;
            ModelDocExtension extension = model2.Extension;
            CustomPropertyManager propertyManager = default(CustomPropertyManager);
            propertyManager = extension.get_CustomPropertyManager("");

            // Remove feature tolerances manually
            // since there is a variable number of them
            model.CustomProperties((properties) =>
            {
                // Find all feature-tolerance custom properties
                List<CustomProperty> found = properties.FindAll( property => property.Name.Contains(CustomPropertyFeatureTolerance));

                // Delete each one
                foreach (CustomProperty item in found) propertyManager.Delete(item.Name);
            });

            mFeatureTolerances.Clear();
            FeatureTolerance_Display.Items.Refresh();

            RawMaterialList.SelectedIndex = -1;
            OverallTolerance.SelectedIndex = 2;

            NoteGrid.Children.Clear();
            AddNewNote();
        }

        /// <summary>
        /// Called when the apply button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            var model = SolidWorksEnvironment.Application.ActiveModel;
            var model2 = (ModelDoc2)SolidWorksEnvironment.Application.UnsafeObject.ActiveDoc;
            ModelDocExtension extension = model2.Extension;
            CustomPropertyManager propertyManager = default(CustomPropertyManager);
            propertyManager = extension.get_CustomPropertyManager("");

            // Check if we have a part
            if (model == null || !model.IsPart)
                return;

            // Notes
            // First clear the existing note custom properties
            model.CustomProperties((properties) =>
            {
                List<CustomProperty> found = properties.FindAll(property => property.Name.Contains(CustomPropertyNote));
                foreach (CustomProperty item in found) propertyManager.Delete(item.Name);
            });

            int j = 1;
            foreach (var child in NoteGrid.Children)
            {
                if (child.GetType() == typeof(System.Windows.Controls.TextBox))
                {
                    model.SetCustomProperty(CustomPropertyNote + " " + j.ToString(), ((System.Windows.Controls.TextBox)child).Text);
                    j++;
                }
            }

            // Set overall tolerance
            var overallTol = OverallTolerance.SelectedItem.ToString();
            overallTol = overallTol.Split(':')[1].TrimStart(' ');
            model.SetCustomProperty("Overall Part Tolerance", overallTol);

            // Feature Tolerances
            for (int i = 0; i < mFeatureTolerances.Count; i++)
            {
                FeatureToleranceObject item = mFeatureTolerances.ElementAt(i);
                string CustomPropertyName = CustomPropertyFeatureTolerance + item.FeatureName;
                model.SetCustomProperty(CustomPropertyName, item.FeatureTolerance);
            }

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
        /// Add new note to the section of user-provided notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            AddNewNote();
        }

        /// <summary>
        /// Delete note corresponding to the close-button clicked by the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            // Use enumerator to iterate through the grid
            // the item before our sender (the close button) is the item to remove
            var enumerator = NoteGrid.Children.GetEnumerator();
            enumerator.MoveNext();
            var prev = enumerator.Current;

            // Loop through NoteGrid
            while (prev != null)
            {
                enumerator.MoveNext();
                if (enumerator.Current.Equals(sender))
                {
                    NoteGrid.Children.Remove((UIElement)prev);
                    break;
                }
                prev = enumerator.Current;
            }

            // Remove the sender, the close button, itself
            NoteGrid.Children.Remove((UIElement)sender);
        }

        /// <summary>
        /// Get selected feature and ask user to provide specific tolerances
        /// </summary>
        private void DesignCheckButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Tolerance_Check_Loop();
        }
        
        /// <summary>
        /// Export STL and wait for virtual machine to provide test.png in a given location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManufacturingCheck_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            step++;

            // Disable the manufacturing check button while we load
            ManufacturingCheck.IsEnabled = false;

            // Disable translation of part into positive space when exporting
            SldWorks app = SolidWorksEnvironment.Application.UnsafeObject;
            app.SetUserPreferenceToggle(((int)swUserPreferenceToggle_e.swSTLDontTranslateToPositive), true);

            // Clear current selection so STL export contains all bodies
            var model = (ModelDoc2)app.ActiveDoc;
            model.ClearSelection();
           
            // Set export location
            var STL_Save_Location = string.Concat(MSculptPrint_Folder, "test.stl");

            // Export SolidWorks View - bottom, z-symmetric view for lathe pieces
            model.ShowNamedView2("", (int)swStandardViews_e.swBottomView);

            // Isometric view useful for mill parts
            if (mill==true)
            {
                model.ShowNamedView2("", (int)swStandardViews_e.swIsometricView);
            }

            model.ViewZoomtofit2();
            var PNG_Save_Location = string.Concat(MSculptPrint_Folder, "View_SW.png");

            // Export to fixed location
            bool saved = ExportModelAsStl(STL_Save_Location);
            bool savedPNG = ExportModelAsPNG(PNG_Save_Location);

            if (saved & savedPNG)
            {
                if (mill == false)
                {
                    if (step == 1)
                    {
                        // Show DFM Reults loading message box
                        MessageBox_DFMLoading DFMLoading = new MessageBox_DFMLoading(mHome + "View_Researcher_Pawn1.png");
                        DialogResult DFM_Result = DFMLoading.ShowDialog();

                        // If the form outputs a DialogResult of Yes, then we have the file!
                        if (DFM_Result == DialogResult.Yes)
                        {
                            DFMLoading.Close();
                            MessageBox_DFMResults DFMResults = new MessageBox_DFMResults(mHome + "View_Researcher_Pawn1.png");
                            DFMResults.Show();
                        }
                        return;
                    }

                    if (step >= 2)
                    {
                        // Show DFM Reults loading message box
                        MessageBox_DFMLoading DFMLoading = new MessageBox_DFMLoading(mHome + "View_Researcher_Pawn2.png");
                        DialogResult DFM_Result = DFMLoading.ShowDialog();

                        // If the form outputs a DialogResult of Yes, then we have the file!
                        if (DFM_Result == DialogResult.Yes)
                        {
                            DFMLoading.Close();
                            MessageBox_DFMResults DFMResults = new MessageBox_DFMResults(mHome + "View_Researcher_Pawn2.png");
                            DFMResults.Show();
                        }
                    }
                }
                else if (mill == true)
                {
                    if (step == 1)
                    {
                        // Show DFM Reults loading message box
                        MessageBox_DFMLoading DFMLoading = new MessageBox_DFMLoading(mHome + "View_Researcher_Doorstop1.png");
                        DialogResult DFM_Result = DFMLoading.ShowDialog();

                        // If the form outputs a DialogResult of Yes, then we have the file!
                        if (DFM_Result == DialogResult.Yes)
                        {
                            DFMLoading.Close();
                            MessageBox_DFMResults DFMResults = new MessageBox_DFMResults(mHome + "View_Researcher_Doorstop1.png");
                            DFMResults.Show();
                        }
                        return;
                    }

                    if (step >= 2)
                    {
                        // Show DFM Reults loading message box
                        MessageBox_DFMLoading DFMLoading = new MessageBox_DFMLoading(mHome + "View_Researcher_Doorstop2.png");
                        DialogResult DFM_Result = DFMLoading.ShowDialog();

                        // If the form outputs a DialogResult of Yes, then we have the file!
                        if (DFM_Result == DialogResult.Yes)
                        {
                            DFMLoading.Close();
                            MessageBox_DFMResults DFMResults = new MessageBox_DFMResults(mHome + "View_Researcher_Doorstop2.png");
                            DFMResults.Show();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when the "Reload Previous Feedback" button is clicked
        /// </summary>
        private void ReloadResults_Click(object sender, RoutedEventArgs e)
        {
            MessageBox_DFMResults DFMResults;
            if (mill == false)
            {
                if (step <= 1)
                {
                    DFMResults = new MessageBox_DFMResults(mHome + "View_Researcher_Pawn1.png");
                }
                else
                {
                    DFMResults = new MessageBox_DFMResults(mHome + "View_Researcher_Pawn2.png");
                }
            }
            else
            {
                if (step <= 1)
                {
                    DFMResults = new MessageBox_DFMResults(mHome + "View_Researcher_Doorstop1.png");
                }
                else
                {
                    DFMResults = new MessageBox_DFMResults(mHome + "View_Researcher_Doorstop2.png");
                }
            }
            DFMResults.Show();
        }

        /// <summary>
        /// Called when "Submit Final Design" is clicked
        /// </summary>
        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            //SldWorks app = SolidWorksEnvironment.Application.UnsafeObject;
            //app.CloseAllDocuments(true);
        }

        #endregion

        #region Private Helper Functions

        /// <summary>
        /// Loops through all features in a part, asking the user to 
        /// specify a tolerance for a given feature by calling GetFeatureTolerance
        /// </summary>
        private void Tolerance_Check_Loop()
        {
            // Clear the old tolerances
            mFeatureTolerances.Clear();

            // Get the active model and begin SolidWorks' SelectionManager and FeatureManager objects
            var model = (ModelDoc2)SolidWorksEnvironment.Application.UnsafeObject.ActiveDoc;
            var selectionManager = model.SelectionManager;
            var featureManager = model.FeatureManager;

            // Get all features in the .sldprt file
            var AllFeatures = (object[])featureManager.GetFeatures(false);

            List<List<Feature>> filteredFeatures = GetFeatureDictionary(AllFeatures);

            for (int i=0; i<filteredFeatures.Count; i++)
            {
                // Get the head feature of each feature-sketch-list
                // This list of lists contains more information than we need
                // So here, we just use the first element in each list - which corresponds to the main feature
                // (and excludes dimension-bearing sketches and any other underlying sketches)
                Feature headFeature = (filteredFeatures[i]).First();

                // Highlight the feature
                model.Extension.SelectByID2(headFeature.Name, "BODYFEATURE", 0, 0, 0, false, 0, null, 0);

                // Get the user specified or default tolerance for this feature
                FeatureToleranceObject thisFeatureTolerance = GetFeatureTolerance(headFeature);

                // Add this tolerance to the list of all tolerances
                mFeatureTolerances.Add(thisFeatureTolerance);

                // Refesh the plug-in's display of all of the tolerances
                FeatureTolerance_Display.Items.Refresh();
            }

            // Clear the selection after assigning all tolerances
            model.ClearSelection();
        }

        /// <summary>
        /// Ask user to provide a tolerance for a given feature
        /// This will open a message box with a UI that the user can fill out
        /// </summary>
        /// <param name="feature">SolidWorks feature currently selected</param>
        private FeatureToleranceObject GetFeatureTolerance(Feature feature)
        {
            // Default tolerance is +/- 0.25mm
            string default_tolerance = "+/- 0.25mm";

            // Configure the message box to ask user if this feature is critical
            string messageBoxText = "Does the highlighted feature,\r\n" + feature.Name + ",\r\n need a specific tolerance?";
            string formTitle = "Design Tolerances";

            // Use the custom FeatureCriticalMessageBox class to ask the user
            MessageBox_FeatureCritical isFeatureCritical = new MessageBox_FeatureCritical(messageBoxText, formTitle);
            DialogResult criticalFeature_result = isFeatureCritical.ShowDialog();

            // Based on the result, either ask the user for a more specific tolerance or use the default tolerance
            if (criticalFeature_result == DialogResult.Yes)
            {
                // User has identified this feature as critical, so it may require a tighter tolerance
                string messageBoxText2 = "The default tolerance is +/- 0.25mm. Choosing a tolerance under +/- 0.1mm will increase manufacturing cost and time.\r\n" +
                    "\r\n Please specify a tolerance value for\r\n" + feature.Name + ":";
                string formTitle2 = "Critical Feature Tolerance";

                // Display Tolerance Message Box to the user and wait for tolerance selection
                MessageBox_Tolerance CriticalFeatureTolerance = new MessageBox_Tolerance(messageBoxText2, formTitle2);
                DialogResult tolerance_result = CriticalFeatureTolerance.ShowDialog();

                FeatureToleranceObject result = new FeatureToleranceObject()
                {
                    FeatureName = feature.Name,
                    FeatureTolerance = CriticalFeatureTolerance.Tolerance_Value
                };

                return result;
            }

            // User has not identified this feature as critical, so return the default tolerance
            else
            {
                FeatureToleranceObject result2 = new FeatureToleranceObject()
                {
                    FeatureName = feature.Name,
                    FeatureTolerance = default_tolerance
                };

                return result2;
            }
        }

        /// <summary>
        /// Determine if a feature is necessary
        /// SolidWorks includes many non-sketch and non-feature "features"
        /// We are filtering out the non-essential features here
        /// </summary>
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
                else if (((Feature)feature).Name.Contains("Mirror")) return null;
                else if (((Feature)feature).Name.Contains("Plane")) return null;
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
        /// Make a list of of lists where each member is:
        /// a list of features containing the parent feature and the child sketches
        /// </summary>
        private List<List<Feature>> GetFeatureDictionary(object[] FeatureSet)
        {
            // Start empty dictionary
            List<List<Feature>> featureDictionary = new List<List<Feature>>();

            // Check each feature in the unfiltered feature array that is passed in
            foreach (object feature in FeatureSet)
            {
                // Is the feature necessary?
                Feature featureIteration = IsNecessaryFeature(feature);
                
                // If it is necessary, then find the feature's sketches
                if (featureIteration != null)
                {
                    // Start the feature list entry
                    List<Feature> entry = new List<Feature>();

                    // add the parent feature
                    entry.Add(featureIteration);
                    
                    // Find and add the sketches underlying the feature
                    List<Feature> sketches = GetSubFeatures(featureIteration);
                    if (sketches != null)
                    {
                        entry.AddRange(sketches);
                    }

                    // Add list of feature and sketches to the main featureDictionary list
                    featureDictionary.Add(entry);
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

        /// <summary>
        /// Get novice-friendly materials list
        /// </summary>
        /// <returns></returns>
        private List<Material> GetMaterialList()
        {
            // Get all materials
            var materials = SolidWorksEnvironment.Application.GetMaterials();

            var filteredMaterials = new List<Material>();

            foreach (Material material in materials)
            {
                if (material.Name.Equals("ABS") | material.Name.Equals("AISI 304") | material.Name.Contains("Delrin") |
                    material.Name.Equals("6061-T6 (SS)") | material.Name.Contains("Acrylic (Medium-high impact)") |
                    material.Name.Contains("7075-T6 (SN)") | material.Name.Contains("Plain Carbon Steel"))
                {
                    filteredMaterials.Add(material);
                }
            }
            filteredMaterials = filteredMaterials.OrderBy(f => f.Classification).ToList();
            filteredMaterials.Insert(0, new Material { Name = "Remove Material", Classification = "Not specified", DatabaseFileFound = false });

            return filteredMaterials;
        }

        /// <summary>
        /// Add a new note to NoteGrid, the grid of notes defined in XAML
        /// </summary>
        /// <param name="optionalText"></param>
        private void AddNewNote(string optionalText = "")
        {
            // Add new row defintion
            NoteGrid.RowDefinitions.Add(new RowDefinition());

            // Create close button
            var closeButton = new System.Windows.Controls.Button
            {
                // Set button properties
                Margin = new Thickness(2, 0, 0, 5),
                Padding = new Thickness(4, 2, 4, 2)
            };

            // Set content and content alignment
            closeButton.Content = "x";
            closeButton.VerticalContentAlignment = VerticalAlignment.Top;
            closeButton.Click += new RoutedEventHandler(DeleteNote_Click);

            // Create new textbox
            var newNote = new System.Windows.Controls.TextBox
            {
                // Set textbox properties
                Margin = new Thickness(0, 0, 2, 5),
                Padding = new Thickness(2),
            };

            // Set text alignment to center
            newNote.TextAlignment = TextAlignment.Center;

            // Set optional text value paramter
            if (optionalText != "") newNote.Text = optionalText;

            // Set grid position
            Grid.SetRow(newNote, NoteGrid.RowDefinitions.Count - 1);
            Grid.SetRow(closeButton, NoteGrid.RowDefinitions.Count - 1);
            Grid.SetColumn(newNote, 1);
            Grid.SetColumn(closeButton, 2);

            // Add the new textbox as a child of the note grid
            NoteGrid.Children.Add(newNote);
            NoteGrid.Children.Add(closeButton);
        }
        
        #endregion 

        #region File Methods

        // Connect to the domain via SFTP
        private SftpClient SFTPConnect()
        {
            PrivateKeyFile key = new PrivateKeyFile(mHome+"\\rsa.key", "Aniruddh123");
            var connectionInfo = new ConnectionInfo("marellapudi.com", 18765, "apollome",
                                        new PrivateKeyAuthenticationMethod("apollome", key));

            SftpClient client = new SftpClient(connectionInfo);
            client.Connect();
            client.ChangeDirectory(client.WorkingDirectory + "/public_ftp/");
            return client;
        }

        // Upload a file given it's file name with extension
        public static bool SFTPUploadFile(SftpClient client, string fileName)
        {
            // Try uploading the file
            try
            {
                using (var fileStream = File.Open((MSculptPrint_Folder + fileName), FileMode.Open))
                {
                    client.UploadFile(fileStream, fileName);
                }
            }
            catch (SftpPathNotFoundException ex)
            {
                // The path/file was not found
                return false;
            }
            // If we get here, we have successfully uploaded the file
            return true;
        }

        // Download a file given it's file name with extension
        public static bool DownloadFile(SftpClient client, string fileName)
        {
            // Try downloading the file
            try
            {
                using (Stream fileStream = File.Create(string.Concat(MSculptPrint_Folder, fileName)))
                {
                    client.DownloadFile(fileName, fileStream);
                }
            }
            catch (SftpPathNotFoundException ex)
            {
                // The path/file was not found 
                return false;
            }

            // If we get here, we have successfully downloaded the file
            return true;
        }

        // Upload a file named "DONE_subject" to reflect completion of subject's portion
        // of the work flow (uploading test.stl and View_SW.png)
        public static void CreateFinishedFlag()
        {
            try
            {
                // Create the file.
                using (FileStream fs = File.Create(MSculptPrint_Folder + "DONE_subject"))
                {
                    var info = new UTF8Encoding(true).GetBytes("");
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //SFTPUploadFile(MClient, "DONE_subject");
        }

        // Delete local file - used to clear SculptPrint experiment files when intializing
        private void DeleteLocal(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        #endregion

    }
}