using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongTelenkoDFM2
{
    public class FeatureMethods
    {
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

            for (int i = 0; i < filteredFeatures.Count; i++)
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

                FeatureTolerance_Display.Items.Refresh();
            }
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
            string messageBoxText = "Does the highlighted feature,\r\n" + feature.Name + ",\r\n need a very tight tolerance?";
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
            }
            else
            {
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
                if (dimension.FullName.Contains("Angle"))
                {
                    //Debug.Print(dimension.FullName + " " + dimension.GetSystemValue2("") + "rads");
                }
                else
                {
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
    }
}
