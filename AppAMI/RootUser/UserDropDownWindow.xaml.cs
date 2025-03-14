using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

namespace AppAMI.RootUser
{
    /// <summary>
    /// Interaction logic for UserDropDownWindow.xaml
    /// </summary>
    public partial class UserDropDownWindow : Window
    {


        public UserDropDownWindow()
        {
            InitializeComponent();

            GetUserOrganisation();
            GetUserDepartment();
            GetUserDesignation();
        }

     

        #region Organisation

        private void GetUserOrganisation()
        {
            try
            {
                var userOrgaList = ReadUserOrgaFromXml("UserOrga.xml")
                    .ToList();

                UpdateuserOrgaList(userOrgaList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
 
        private List<string> ReadUserOrgaFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var UserOrgas = doc.Descendants("user_orga")
                                    .Select(element => element.Value)
                                    .ToList();

                return UserOrgas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }

        private void UpdateuserOrgaList(List<string> userOrgaList)
        {
            lbOrga.ItemsSource = userOrgaList;
        }

        private void btnAddOrga_Click(object sender, RoutedEventArgs e)
        {
            string UserOrgas = txtNewOrga.Text;

            SaveToUserOrgaXml(UserOrgas, "UserOrga.xml");
        }

        private void SaveToUserOrgaXml(string UserOrgas, string fileName)
        {
            try
            {
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                XDocument doc;

                if (File.Exists(filePath))
                {
                    doc = XDocument.Load(filePath);

                    doc.Root.Add(new XElement("user_orga", UserOrgas));
                }
                else
                {
                    doc = new XDocument(
                        new XElement("user_orgas",
                            new XElement("user_orga", UserOrgas)
                        )
                    );
                }

                doc.Save(filePath);

                MessageBox.Show($"User Organisation '{UserOrgas}' saved successfully at '{filePath}'.");

                GetUserOrganisation();
                txtNewOrga.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving User Organisation: {ex.Message}");
            }
        }

        private void btnDeleteOrga_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                List<string> selectedItems = lbOrga.SelectedItems.Cast<string>().ToList();

                if (selectedItems.Count == 0)
                {
                    MessageBox.Show("Please select at least one item to delete.");
                    return;
                }

                DeleteUserOrgaFromXml(selectedItems, "UserOrga.xml");

                GetUserOrganisation();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting User Organisation: {ex.Message}");
            }
        }

        private void DeleteUserOrgaFromXml(List<string> userOrgaToDelete, string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                // Remove selected meter_type elements from the document
                foreach (string userOrga in userOrgaToDelete)
                {
                    XElement userOrgaElement = doc.Root.Elements("user_orga").FirstOrDefault(x => x.Value == userOrga);

                    if (userOrgaElement != null)
                    {
                        userOrgaElement.Remove();
                    }
                }

                // Save the updated document
                doc.Save(filePath);

                MessageBox.Show($"Selected User Organisation deleted successfully from '{filePath}'.");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error User Organisation from XML file: {ex.Message}");
            }
        }

        #endregion  Organisation


        #region Department
        private void GetUserDepartment()
        {
            try
            {
                var userDepartList = ReadUserDepartFromXml("UserDepart1.xml")
                    .ToList();

                UpdateUserDepartList(userDepartList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private List<string> ReadUserDepartFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var UserDeparts = doc.Descendants("user_depart")
                                    .Select(element => element.Value)
                                    .ToList();

                return UserDeparts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }

        private void UpdateUserDepartList(List<string> userDepartList)
        {
            lbDepart.ItemsSource = userDepartList;
        }




        private void btnAddDepart_Click(object sender, RoutedEventArgs e)
        {
            string UserDeparts = txtNewDepart.Text;

            SaveToDepartXml(UserDeparts, "UserDepart1.xml");
        }

        private void SaveToDepartXml(string UserDeparts, string fileName)
        {
            try
            {
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                XDocument doc;

                if (File.Exists(filePath))
                {
                    doc = XDocument.Load(filePath);

                    // Add the entire MeterTypes string as a new element
                    doc.Root.Add(new XElement("user_depart", UserDeparts));
                }
                else
                {
                    doc = new XDocument(
                        new XElement("user_departs",
                            // Add the entire MeterTypes string as a new element
                            new XElement("user_depart", UserDeparts)
                        )
                    );
                }

                doc.Save(filePath);

                MessageBox.Show($"User Department '{UserDeparts}' saved successfully at '{filePath}'.");

                GetUserDepartment();

                txtNewDepart.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving User Department: {ex.Message}");
            }

        }

        private void btnDeleteDepart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the selected items from the ListBox
                List<string> selectedItems = lbDepart.SelectedItems.Cast<string>().ToList();

                if (selectedItems.Count == 0)
                {
                    MessageBox.Show("Please select at least one item to delete.");
                    return;
                }

                // Delete selected items from the XML file
                DeleteUserDepartsFromXml(selectedItems, "UserDepart1.xml");

                // Refresh the ListBox with updated data
                GetUserDepartment();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting User Department: {ex.Message}");
            }
        }


        private void DeleteUserDepartsFromXml(List<string> userDepartsToDelete, string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                // Remove selected meter_type elements from the document
                foreach (string userDepart in userDepartsToDelete)
                {
                    XElement userDepartElement = doc.Root.Elements("user_depart").FirstOrDefault(x => x.Value == userDepart);

                    if (userDepartElement != null)
                    {
                        userDepartElement.Remove();
                    }
                }

                // Save the updated document
                doc.Save(filePath);

                MessageBox.Show($"Selected User department deleted successfully from '{filePath}'.");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting User Department from XML file: {ex.Message}");
            }
        }

        #endregion  Designation


        #region Designtion

        private void GetUserDesignation()
        {
            try
            {
                var userDesigList = ReadUserDesigFromXml("UserDesig.xml")
                    .ToList();

                UpdateuserDesigList(userDesigList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private List<string> ReadUserDesigFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var UserDesigs = doc.Descendants("user_desig")
                                    .Select(element => element.Value)
                                    .ToList();

                return UserDesigs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }

        private void UpdateuserDesigList(List<string> userDesigList)
        {
            lbDesig.ItemsSource = userDesigList;
        }

        private void btnAddDesig_Click(object sender, RoutedEventArgs e)
        {
            string UserDesigs = txtNewDesig.Text;

            SaveToUserDesigXml(UserDesigs, "UserDesig.xml");
        }

        private void SaveToUserDesigXml(string UserDesigs, string fileName)
        {
            try
            {
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                XDocument doc;

                if (File.Exists(filePath))
                {
                    doc = XDocument.Load(filePath);

                    doc.Root.Add(new XElement("user_desig", UserDesigs));
                }
                else
                {
                    doc = new XDocument(
                        new XElement("user_desigs",
                            new XElement("user_desig", UserDesigs)
                        )
                    );
                }

                doc.Save(filePath);

                MessageBox.Show($"User Designation '{UserDesigs}' saved successfully at '{filePath}'.");

                GetUserDesignation();
                txtNewDesig.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving User Designation: {ex.Message}");
            }
        }

        private void btnDeleteDesig_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                List<string> selectedItems = lbDesig.SelectedItems.Cast<string>().ToList();

                if (selectedItems.Count == 0)
                {
                    MessageBox.Show("Please select at least one item to delete.");
                    return;
                }

                DeleteUserDesigFromXml(selectedItems, "UserDesig.xml");

                GetUserDesignation();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting User Designation: {ex.Message}");
            }
        }

        private void DeleteUserDesigFromXml(List<string> userDesigToDelete, string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                // Remove selected meter_type elements from the document
                foreach (string userDesig in userDesigToDelete)
                {
                    XElement userDesigElement = doc.Root.Elements("user_desig").FirstOrDefault(x => x.Value == userDesig);

                    if (userDesigElement != null)
                    {
                        userDesigElement.Remove();
                    }
                }

                // Save the updated document
                doc.Save(filePath);

                MessageBox.Show($"Selected User Designation deleted successfully from '{filePath}'.");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error User Designation from XML file: {ex.Message}");
            }
        }


        #endregion  Designation


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
