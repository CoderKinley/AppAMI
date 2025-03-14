using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using System.IO;
using System;
using System.Collections.Generic;

namespace AppAMI.RootUser
{
    /// <summary>
    /// Interaction logic for MeterDropDownWindow.xaml
    /// </summary>
    public partial class MeterDropDownWindow : Window
    {

        public MeterDropDownWindow()
        {
            InitializeComponent();

            GetMeterType();
            GetMeterFirmV();
        }



        #region Meter Type

        private void GetMeterType()
        {
            try
            {
                var meterTypeList = ReadMeterTypesFromXml("MeterType.xml")
                    .ToList();

                UpdatemeterTypeList(meterTypeList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private List<string> ReadMeterTypesFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var MeterTypes = doc.Descendants("meter_type")
                                    .Select(element => element.Value)
                                    .ToList();

                return MeterTypes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }

        private void UpdatemeterTypeList(List<string> meterTypeList)
        {
            lbMeterType.ItemsSource = meterTypeList;
        }

       
        private void btnAddMeterType_Click(object sender, RoutedEventArgs e)
        {
            string MeterTypes = txtNewMeterType.Text;

            SaveToMeterTypeXml(MeterTypes, "MeterType.xml");
        }

        private void SaveToMeterTypeXml(string MeterTypes, string fileName)
        {
            try
            {
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                XDocument doc;

                if (File.Exists(filePath))
                {
                    doc = XDocument.Load(filePath);

                    // Add the entire MeterTypes string as a new element
                    doc.Root.Add(new XElement("meter_type", MeterTypes));
                }
                else
                {
                    doc = new XDocument(
                        new XElement("meter_types",
                            // Add the entire MeterTypes string as a new element
                            new XElement("meter_type", MeterTypes)
                        )
                    );
                }

                doc.Save(filePath);

                MessageBox.Show($"Meter Type '{MeterTypes}' saved successfully at '{filePath}'.");

                GetMeterType();

                txtNewMeterType.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving Meter Type: {ex.Message}");
            }
        }

        private void btnDeleteMeterType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the selected items from the ListBox
                List<string> selectedItems = lbMeterType.SelectedItems.Cast<string>().ToList();

                if (selectedItems.Count == 0)
                {
                    MessageBox.Show("Please select at least one item to delete.");
                    return;
                }

                // Delete selected items from the XML file
                DeleteMeterTypesFromXml(selectedItems, "MeterType.xml");

                // Refresh the ListBox with updated data
                GetMeterType();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting Meter Types: {ex.Message}");
            }
        }

        private void DeleteMeterTypesFromXml(List<string> meterTypesToDelete, string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                // Remove selected meter_type elements from the document
                foreach (string meterType in meterTypesToDelete)
                {
                    XElement meterTypeElement = doc.Root.Elements("meter_type").FirstOrDefault(x => x.Value == meterType);

                    if (meterTypeElement != null)
                    {
                        meterTypeElement.Remove();
                    }
                }

                // Save the updated document
                doc.Save(filePath);

                MessageBox.Show($"Selected Meter Types deleted successfully from '{filePath}'.");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting Meter Types from XML file: {ex.Message}");
            }
        }


        #endregion Meter Type


        #region Meter Firware Versions
        private void GetMeterFirmV()
        {
            try
            {
                var meterFirmVList = ReadMeterFirmVFromXml("MeterFirmV.xml")
                    .ToList();

                UpdatemeterFirmVList(meterFirmVList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
   
        private List<string> ReadMeterFirmVFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var MeterFirmVs = doc.Descendants("meter_firm_v")
                                    .Select(element => element.Value)
                                    .ToList();

                return MeterFirmVs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }

        private void UpdatemeterFirmVList(List<string> meterFirmVList)
        {
            lbMeterFirmV.ItemsSource = meterFirmVList;
        }


        private void btnAddMeterFirmV_Click(object sender, RoutedEventArgs e)
        {
            string MeterFirmVs = txtNewMeterFirmV.Text;

            SaveToMeterFirmVXml(MeterFirmVs, "MeterFirmV.xml");
        }

        private void SaveToMeterFirmVXml(string MeterFirmVs, string fileName)
        {
            try
            {
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                XDocument doc;

                if (File.Exists(filePath))
                {
                    doc = XDocument.Load(filePath);

                    // Add the entire MeterTypes string as a new element
                    doc.Root.Add(new XElement("meter_firm_v", MeterFirmVs));
                }
                else
                {
                    doc = new XDocument(
                        new XElement("meter_firm_vs",
                            // Add the entire MeterTypes string as a new element
                            new XElement("meter_firm_v", MeterFirmVs)
                        )
                    );
                }

                doc.Save(filePath);

                MessageBox.Show($"Meter  '{MeterFirmVs}' saved successfully at '{filePath}'.");

                GetMeterFirmV();

                txtNewMeterFirmV.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving Meter Firmware Verision: {ex.Message}");
            }
        }

        private void btnDeleteMeterFirmV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the selected items from the ListBox
                List<string> selectedItems = lbMeterFirmV.SelectedItems.Cast<string>().ToList();

                if (selectedItems.Count == 0)
                {
                    MessageBox.Show("Please select at least one item to delete.");
                    return;
                }

                // Delete selected items from the XML file
                DeleteMeterFirmVFromXml(selectedItems, "MeterFirmV.xml");

                // Refresh the ListBox with updated data

                GetMeterFirmV();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting Meter Firmware Versions: {ex.Message}");
            }
        }

        private void DeleteMeterFirmVFromXml(List<string> meterFirmVToDelete, string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                // Remove selected meter_type elements from the document
                foreach (string meterFirmV in meterFirmVToDelete)
                {
                    XElement meterFirmVElement = doc.Root.Elements("meter_firm_v").FirstOrDefault(x => x.Value == meterFirmV);

                    if (meterFirmVElement != null)
                    {
                        meterFirmVElement.Remove();
                    }
                }

                // Save the updated document
                doc.Save(filePath);

                MessageBox.Show($"Selected Meter Firmware Versions deleted successfully from '{filePath}'.");


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting Meter Firmware Versions from XML file: {ex.Message}");
            }
        }

        #endregion Meter Firware Versions


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }



    }
}
