using AppAMI.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace AppAMI.RootUser
{
    /// <summary>
    /// Interaction logic for UpdateAdminSetAccWindow.xaml
    /// </summary>
    public partial class UpdateAdminSetAccWindow : Window
    {
        string CurrentUserId1;
        string CurrentUserRole1;
        string CurrentUserPassword1;
        string CurrentUserName1;
        string CurrentUserEmployeeId1;

        public UpdateAdminSetAccWindow(string CurrentUserId, string CurrentUserRole, string CurrentUserPassword, string CurrentUserName, string CurrentUserEmployeeId)
        {
            InitializeComponent();

            txtUserId.Text = CurrentUserId;

            CurrentUserId1 = CurrentUserId;
            CurrentUserRole1 = CurrentUserRole;
            CurrentUserPassword1 = CurrentUserPassword;
            CurrentUserName1 = CurrentUserName;
            CurrentUserEmployeeId1 = CurrentUserEmployeeId;

            GetUserOrganisation();
            GetUserDepartment();
            GetUserDesignation();

            ReadDataBase();
        }



        #region Get Combobox Items
        private void GetUserOrganisation()
        {
            try
            {
                var userOrgaList = ReadUserOrgaFromXml("UserOrga.xml")
                    .ToList();

                UpdateOrgaComboBox(userOrgaList);
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
        private void UpdateOrgaComboBox(List<string> userOrgaList)
        {
            cbOrganization.ItemsSource = userOrgaList;
        }



        private void GetUserDepartment()
        {
            try
            {
                var userDepartList = ReadUserDepartFromXml("UserDepart.xml")
                    .ToList();

                UpdateUserDepartComboBox(userDepartList);
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

                var UserOrgas = doc.Descendants("user_depart")
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

        private void UpdateUserDepartComboBox(List<string> userDepartList)
        {
            cbDepartment.ItemsSource = userDepartList;
        }




        private void GetUserDesignation()
        {
            try
            {
                var userDesigList = ReadUserDesigFromXml("UserDesig.xml")
                    .ToList();

                UpdateuserDesigComboBox(userDesigList);
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

        private void UpdateuserDesigComboBox(List<string> userDesigList)
        {
            cbDesignation.ItemsSource = userDesigList;
        }

        #endregion Get Combobox Items


        async void ReadDataBase()
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync("http://103.234.126.43:3080/dtmeter/user");
                var responseBody = await response.Content.ReadAsStringAsync();

                var adminConfigs = JsonConvert.DeserializeObject<List<AdminConfig>>(responseBody);

                foreach (var adminConfig in adminConfigs)
                {
                    if (adminConfig.user_id.Equals(txtUserId.Text))
                    {

                        txtName.Text = adminConfig.user_name;
                        passPassword.Password = adminConfig.password;

                        cbOrganization.Text = adminConfig.organisation;
                        cbDepartment.Text = adminConfig.department;
                        cbDesignation.Text = adminConfig.designation;

                        txtUserRole.Text = adminConfig.user_role;
                        txtEmail.Text = adminConfig.email;
                        txtPhone.Text = adminConfig.phone_number;

                        txtEmployeeId .Text = adminConfig.employee_id ;
                        return;
                    }


                }

                MessageBox.Show("No match found for employee ID: " + txtUserId.Text + ". Incorrect Emloyee ID or Password");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(passPassword.Password))
            {
                MessageBox.Show("Please fill in the fields marked with a star (*)");
            }

            else
            {
                try
                {
                    AdminConfigPost adminConfigPost = new AdminConfigPost()
                    {

                        UserID = txtUserId.Text,
                        UserName = txtName.Text,
                        Password = passPassword.Password,

                        Organization = cbOrganization.Text,
                        Department = cbDepartment.Text,
                        Designation = cbDesignation.Text,

                        UserRole = txtUserRole.Text,
                        Email = txtEmail.Text,
                        Phone = txtPhone.Text,

                        EmployeeId = txtEmployeeId.Text

                    };


                    string url = string.Format("http://103.234.126.43:3080/dtmeter/user/{0}", txtUserId.Text);
                    HttpClient client = new HttpClient();
                    string jsonData = JsonConvert.SerializeObject(adminConfigPost);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(responseMessage);

                        addNewUserEvent();


                        Close();
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show("Error updating meter configuration: " + errorMessage);
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Unable to connect to the server" + ex.Message);
                    Close();
                }
            }

            
        }


        private async void addNewUserEvent()
        {
            DateTime now = DateTime.Now;
            string UserEventDate = now.ToString("dd-MM-yyyy");
            string UserEventTime = now.ToString("h:mm tt");

            try
            {
                UserEventPost userEventPost = new UserEventPost()
                {
                    UserID = CurrentUserId1,
                    EmployeeId = CurrentUserEmployeeId1,
                    UserName = CurrentUserName1,

                    EventLogs = "Edited User",

                    Date = UserEventDate,
                    Time = UserEventTime,

                    StatusAdmin1 = "Not Acknowledged",
                    StatusAdmin2 = "Not Acknowledged",
                    StatusAdmin3 = "Not Acknowledged",

                    Remarks = "Edited : " + txtUserId.Text,
                };

                string url = "http://103.234.126.43:3080/dtmeter/logs/events/userevents";
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(userEventPost);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    //string responseMessage = await response.Content.ReadAsStringAsync();
                    //MessageBox.Show(responseMessage);

                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Error updating meter configuration: " + errorMessage);
                }
            }


            catch (Exception ex)
            {
                MessageBox.Show("Unable to Connect to the Server" + ex.Message);
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
