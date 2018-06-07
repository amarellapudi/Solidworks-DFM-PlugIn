using AngelSix.SolidDna;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows.Controls;
using static AngelSix.SolidDna.SolidWorksEnvironment;
using static System.Windows.Visibility;

namespace SongTelenkoDFM
{
    /// <summary>
    /// Interaction logic for CustomPropertiesUI.xaml
    /// </summary>
    public partial class CustomPropertiesUI : UserControl
    {
        #region Private Members

        private const string CustomPropertyDescription = "Description";
        private const string CustomPropertyStatus = "Status";
        private const string CustomPropertyRevision = "Revision";
        private const string CustomPropertyManufacturingInformation = "Manufacturing Information";
        private const string CustomPropertyLength = "Length";
        private const string CustomPropertyPrecisionInformation = "Precision Information";
        private const string CustomPropertySupplierName = "Supplier";
        private const string CustomPropertyNote = "Note";

        private const string ManufacturingWeld = "WELD";
        private const string ManufacturingAssembly = "ASSEMBLY";
        private const string ManufacturingPlasma = "PLASMA";
        private const string ManufacturingLaser = "LASER";
        private const string ManufacturingRoll = "ROLL";
        private const string ManufacturingSaw = "SAW";

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
            AnalyzePartContent.Visibility = Hidden;
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

        #endregion

        /// <summary>
        /// Reads all the details from the active
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
                    NoPartContent.Visibility = System.Windows.Visibility.Visible;
                    MainContent.Visibility = System.Windows.Visibility.Hidden;

                    return;
                }

                // If we got here, we have a part

                // Listen out for selection changes
                model.SelectionChanged += Model_SelectionChanged;

                // Show the main content
                NoPartContent.Visibility = System.Windows.Visibility.Hidden;
                MainContent.Visibility = System.Windows.Visibility.Visible;

                // Query all custom properties
                model.CustomProperties((properties) =>
                {
                    // Description
                    DescriptionText.Text = properties.FirstOrDefault(property => string.Equals(CustomPropertyDescription, property.Name, StringComparison.InvariantCultureIgnoreCase))?.Value;

                    // Status
                    StatusText.Text = properties.FirstOrDefault(property => string.Equals(CustomPropertyStatus, property.Name, StringComparison.InvariantCultureIgnoreCase))?.ResolvedValue;

                    // Revision
                    RevisionText.Text = properties.FirstOrDefault(property => string.Equals(CustomPropertyRevision, property.Name, StringComparison.InvariantCultureIgnoreCase))?.ResolvedValue;

                    // Manufacturing Information
                    // Clear previous checks
                    MaterialWeldCheck.IsChecked = MaterialAssemblyCheck.IsChecked = MaterialPlasmaCheck.IsChecked = 
                       MaterialRollCheck.IsChecked = MaterialSawCheck.IsChecked = MaterialLaserCheck.IsChecked = false;

                    // Read in value
                    var manufacturingInfo = properties.FirstOrDefault(property => string.Equals(CustomPropertyManufacturingInformation, property.Name, StringComparison.InvariantCultureIgnoreCase))?.Value;

                    // If we have some property, parse it
                    if (!string.IsNullOrWhiteSpace(manufacturingInfo))
                    {
                        // Remove white spaces, capitalize and split by ,
                        foreach (var part in manufacturingInfo.Replace(" ", "").ToUpper().Split(','))
                        {
                            switch (part)
                            {
                                case ManufacturingWeld:
                                    MaterialWeldCheck.IsChecked = true;
                                    break;
                                case ManufacturingAssembly:
                                    MaterialAssemblyCheck.IsChecked = true;
                                    break;
                                case ManufacturingPlasma:
                                    MaterialPlasmaCheck.IsChecked = true;
                                    break;
                                case ManufacturingLaser:
                                    MaterialLaserCheck.IsChecked = true;
                                    break;
                                case ManufacturingRoll:
                                    MaterialRollCheck.IsChecked = true;
                                    break;
                                case ManufacturingSaw:
                                    MaterialSawCheck.IsChecked = true;
                                    break;
                            }
                        }
                    }

                    // Length
                    SheetMetalLengthText.Text = properties.FirstOrDefault(property => string.Equals(CustomPropertyLength, property.Name, StringComparison.InvariantCultureIgnoreCase))?.Value;
                    SheetMetalLengthEvaluatedText.Text = properties.FirstOrDefault(property => string.Equals(CustomPropertyLength, property.Name, StringComparison.InvariantCultureIgnoreCase))?.ResolvedValue;

                    // Purchase Information
                    var purchaseInfo = properties.FirstOrDefault(property => string.Equals(CustomPropertyPrecisionInformation, property.Name, StringComparison.InvariantCultureIgnoreCase))?.Value;

                    // Clear the selection first
                    PrecisionInformationList.SelectedIndex = -1;

                    // Try and find matching item
                    foreach (var item in PrecisionInformationList.Items)
                    {
                        // Check if the combo box item has the same name
                        if ((string)((ComboBoxItem)item).Content == purchaseInfo)
                        {
                            // If so select it
                            PrecisionInformationList.SelectedItem = item;
                            break;
                        }
                    }

                    // Supplier Name
                    SupplierNameText.Text = properties.FirstOrDefault(property => string.Equals(CustomPropertySupplierName, property.Name, StringComparison.InvariantCultureIgnoreCase))?.Value;

                    // Note
                    NoteText.Text = properties.FirstOrDefault(property => string.Equals(CustomPropertyNote, property.Name, StringComparison.InvariantCultureIgnoreCase))?.Value;

                });

                // Mass
                MassText.Text = model.MassProperties?.MassInMetric();

                // Get all materials
                var materials = Application.GetMaterials();
                materials.Insert(0, new Material {Name = "Remove Material", Classification = "Not specified", DatabaseFileFound = false });

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

        private void Model_SelectionChanged()
        {
            Application.ActiveModel?.SelectedObjects((objects) =>
            {
                var haveDimension = objects.Any(f => f.IsDimension);

                ThreadHelpers.RunOnUIThread(() =>
                {
                    LengthButton.IsEnabled = haveDimension;
                });
            });
        }

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
            DescriptionText.Text = string.Empty;
            StatusText.Text = string.Empty;
            RevisionText.Text = string.Empty;

            RawMaterialList.SelectedIndex = -1;

            MaterialWeldCheck.IsChecked = MaterialAssemblyCheck.IsChecked = MaterialPlasmaCheck.IsChecked =
                MaterialRollCheck.IsChecked = MaterialSawCheck.IsChecked = MaterialLaserCheck.IsChecked = false;

            SheetMetalLengthText.Text = string.Empty;
            SheetMetalLengthEvaluatedText.Text = string.Empty;

            PrecisionInformationList.SelectedIndex = -1;

            SupplierNameText.Text = string.Empty;
            NoteText.Text = string.Empty;

        }

        /// <summary>
        /// Called when the apply button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var model = Application.ActiveModel;

            // Check if we have a part
            if (model == null || !model.IsPart)
                return;

            // Description
            model.SetCustomProperty(CustomPropertyDescription, DescriptionText.Text);

            // If user does not have a material selected, clear it
            if (RawMaterialList.SelectedIndex < 0)
                model.SetMaterial(null);
            // Otherwise set the material to the selected one
            else
                model.SetMaterial((Material)RawMaterialList.SelectedItem);

            // Manufacturing Info
            var manufacturingInfo = new List<string>();

            if (MaterialWeldCheck.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingWeld);
            if (MaterialAssemblyCheck.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingAssembly);
            if (MaterialPlasmaCheck.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingPlasma);
            if (MaterialLaserCheck.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingLaser);
            if (MaterialRollCheck.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingRoll);
            if (MaterialSawCheck.IsChecked.Value)
                manufacturingInfo.Add(ManufacturingSaw);

            // Set manufacturing info
            model.SetCustomProperty(CustomPropertyManufacturingInformation, string.Join(",", manufacturingInfo));

            // Length
            model.SetCustomProperty(CustomPropertyLength, SheetMetalLengthText.Text);

            // Purchase Info
            model.SetCustomProperty(CustomPropertyPrecisionInformation, (string)((ComboBoxItem)PrecisionInformationList.SelectedValue)?.Content);

            // Supplier Name
            model.SetCustomProperty(CustomPropertySupplierName, SupplierNameText.Text);
            
            // Note
            model.SetCustomProperty(CustomPropertyNote, NoteText.Text);

            // Re-read details to confirm they are correct
            ReadDetails();
        }

        #endregion

        private void MaterialAssemblyCheck_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            // Uncheck plasma
            MaterialPlasmaCheck.IsChecked = false;
        }

        private void MaterialPlasmaCheck_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            // Uncheck assembly
            MaterialAssemblyCheck.IsChecked = false;
        }

        private void LengthButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.ActiveModel?.SelectedObjects((objects) =>
            {
                // Get the newest dimension
                var lastDimension = objects.LastOrDefault(f => f.IsDimension);

                // Double check we have one
                if (lastDimension == null)
                    return;

                var dimensionSelectionName = string.Empty;
                
                // Get the dimension name
                lastDimension.AsDimension((dimension) => dimensionSelectionName = dimension.SelectionName);

                // Set the length button text
                ThreadHelpers.RunOnUIThread(() =>
                {
                    SheetMetalLengthText.Text = $"\"{dimensionSelectionName}\"";
                });
            });
        }

        private void AnalyzeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AnalyzePartContent.Visibility = Visible;
            NoPartContent.Visibility = Hidden;
            MainContent.Visibility = Hidden;
        }

        private void ReturnButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ReadDetails();
            AnalyzePartContent.Visibility = Hidden;
            NoPartContent.Visibility = Hidden;
            MainContent.Visibility = Visible;
        }

        private void STLButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FileExporting.ExportModelAsStl();
        }
    }
}