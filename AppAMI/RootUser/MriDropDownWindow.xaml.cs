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
    /// Interaction logic for MriDropDownWindow.xaml
    /// </summary>
    public partial class MriDropDownWindow : Window
    {


        

        public MriDropDownWindow()
        {
            InitializeComponent();

            GetMriVersion();
            GetMriFirmV();
        }

        private void GetMriVersion()
        {
            try
            {
                var mriVersionList = ReadMriVersionFromXml("MriVersion.xml")
                    .ToList();

                UpdateMriVersionList(mriVersionList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private List<string> ReadMriVersionFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var MriVersions = doc.Descendants("mri_version")
                                    .Select(element => element.Value)
                                    .ToList();

                return MriVersions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }

        private void UpdateMriVersionList(List<string> mriVersionList)
        {
            lbMriVersion.ItemsSource = mriVersionList;
        }



        private void btnAddMriVersion_Click(object sender, RoutedEventArgs e)
        {
            string MriVersions = txtNewMriVersion.Text;

            SaveToMriVersionXml(MriVersions, "MriVersion.xml");
        }

        private void SaveToMriVersionXml(string MriVersions, string fileName)
        {
            try
            {
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                XDocument doc;

                if (File.Exists(filePath))
                {
                    doc = XDocument.Load(filePath);

                    // Add the entire MeterTypes string as a new element
                    doc.Root.Add(new XElement("mri_version", MriVersions));
                }
                else
                {
                    doc = new XDocument(
                        new XElement("mri_versions",
                            // Add the entire MeterTypes string as a new element
                            new XElement("mri_version", MriVersions)
                        )
                    );
                }

                doc.Save(filePath);

                MessageBox.Show($"MRI Version '{MriVersions}' saved successfully at '{filePath}'.");

                GetMriVersion();

                txtNewMriVersion.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving MRI Version: {ex.Message}");
            }
        }

        private void btnDeleteMriVersion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the selected items from the ListBox
                List<string> selectedItems = lbMriVersion.SelectedItems.Cast<string>().ToList();

                if (selectedItems.Count == 0)
                {
                    MessageBox.Show("Please select at least one item to delete.");
                    return;
                }

                // Delete selected items from the XML file
                DeleteMriVersionFromXml(selectedItems, "MriVersion.xml");

                // Refresh the ListBox with updated data
                GetMriVersion();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting Mri Version: {ex.Message}");
            }
        }


        private void DeleteMriVersionFromXml(List<string> mriVersionToDelete, string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                // Remove selected meter_type elements from the document
                foreach (string mriVersion in mriVersionToDelete)
                {
                    XElement mriVersionElement = doc.Root.Elements("mri_version").FirstOrDefault(x => x.Value == mriVersion);

                    if (mriVersionElement != null)
                    {
                        mriVersionElement.Remove();
                    }
                }

                // Save the updated document
                doc.Save(filePath);

                MessageBox.Show($"Selected Mri Version deleted successfully from '{filePath}'.");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting Mri Version from XML file: {ex.Message}");
            }
        }





















        private void GetMriFirmV()
        {
            try
            {
                var mriFirmVList = ReadMriFirmVFromXml("MriFirmV.xml")
                    .ToList();

                UpdateMriFirmVList(mriFirmVList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private List<string> ReadMriFirmVFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var MriFirmVs = doc.Descendants("mri_firm_v")
                                    .Select(element => element.Value)
                                    .ToList();

                return MriFirmVs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }

        private void UpdateMriFirmVList(List<string> mriFirmVList)
        {
            lbMriFirmV.ItemsSource = mriFirmVList;
        }




        private void btnAddMriFirmV_Click(object sender, RoutedEventArgs e)
        {
            string MriFirmVs = txtNewMriFirmV.Text;

            SaveToMriFirmVXml(MriFirmVs, "MriFirmV.xml");
        }

        private void SaveToMriFirmVXml(string MriFirmVs, string fileName)
        {
            try
            {
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                XDocument doc;

                if (File.Exists(filePath))
                {
                    doc = XDocument.Load(filePath);

                    // Add the entire MeterTypes string as a new element
                    doc.Root.Add(new XElement("mri_firm_v", MriFirmVs));
                }
                else
                {
                    doc = new XDocument(
                        new XElement("mri_firm_vs",
                            // Add the entire MeterTypes string as a new element
                            new XElement("mri_firm_v", MriFirmVs)
                        )
                    );
                }

                doc.Save(filePath);

                MessageBox.Show($"MRI Firmare Version '{MriFirmVs}' saved successfully at '{filePath}'.");

                GetMriFirmV();

                txtNewMriFirmV.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving MRI Firmare Version: {ex.Message}");
            }
        }

      

        private void btnDeleteMriFirmV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the selected items from the ListBox
                List<string> selectedItems = lbMriFirmV.SelectedItems.Cast<string>().ToList();

                if (selectedItems.Count == 0)
                {
                    MessageBox.Show("Please select at least one item to delete.");
                    return;
                }

                // Delete selected items from the XML file
                DeleteMriFirmVsFromXml(selectedItems, "MriFirmV.xml");

                // Refresh the ListBox with updated data
                GetMriFirmV();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting MRI Firmware Version Types: {ex.Message}");
            }
        }

        private void DeleteMriFirmVsFromXml(List<string> mriFirmVToDelete, string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                // Remove selected meter_type elements from the document
                foreach (string mriFirmV in mriFirmVToDelete)
                {
                    XElement mriFirmVElement = doc.Root.Elements("mri_firm_v").FirstOrDefault(x => x.Value == mriFirmV);

                    if (mriFirmVElement != null)
                    {
                        mriFirmVElement.Remove();
                    }
                }

                // Save the updated document
                doc.Save(filePath);

                MessageBox.Show($"Selected MRI Firmware Version deleted successfully from '{filePath}'.");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting MRI Firmare Version from XML file: {ex.Message}");
            }

        }




        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
