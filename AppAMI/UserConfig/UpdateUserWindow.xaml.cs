using AppAMI.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace AppAMI.UserConfig
{
    /// <summary>
    /// Interaction logic for UpdateUserWindow.xaml
    /// </summary>
    public partial class UpdateUserWindow : Window
    {
        AdminConfig selectedAdminConfig;

        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;



        public UpdateUserWindow(AdminConfig selectedAdminConfig, string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1)
        {
            InitializeComponent();

            this.selectedAdminConfig = selectedAdminConfig;

            txtUserId.Text = this.selectedAdminConfig.user_id;
            //txtName.Text = this.selectedAdminConfig.user_name;
            //passPassword.Password = this.selectedAdminConfig.password;

            //cbOrganization .Text = this.selectedAdminConfig.organisation;
            //cbDepartment .Text = this.selectedAdminConfig.department;
            //cbDesignation .Text = this.selectedAdminConfig.designation;

            //cbUserRole.Text = this.selectedAdminConfig.user_role;
            //txtEmail.Text = this.selectedAdminConfig.email;
            //txtPhone.Text = this.selectedAdminConfig.phone_number;
            //txtEmployeeId.Text = this.selectedAdminConfig.employee_id;

            CurrentUserId2 = CurrentUserId1;
            CurrentUserRole2 = CurrentUserRole1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;

            GetUserOrganisation();
            GetUserDepartment();
            GetUserDesignation();

            //Task.Run(() => ReadDataBase());

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

                        txtEmployeeId.Text = adminConfig.employee_id;
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

        //private async Task ReadDataBase()
        //{
        //    try
        //    {
        //        // Show progress bar at the beginning
        //        Dispatcher.Invoke(() => progressLogin.Visibility = Visibility.Visible);

        //        using (WebClient web = new WebClient())
        //        {
        //            string url = "http://103.234.126.43:3080/dtmeter/user";
        //            string json = await web.DownloadStringTaskAsync(url);

        //            List<AdminConfig> all_data = JsonConvert.DeserializeObject<List<AdminConfig>>(json);

        //            List<AdminConfig> operators = all_data.Where(a => a.user_role == "Operator").ToList();

        //            int operatorCount = operators.Count;

        //            if (operatorCount >= 3)
        //            {
        //                // Update UI on the UI thread
        //                Dispatcher.Invoke(() => cbOperator.Visibility = Visibility.Collapsed);
        //            }
        //        }
        //    }
        //    catch 
        //    {
        //        // Update UI on the UI thread
        //        Dispatcher.Invoke(() =>
        //        {
        //            MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        });
        //    }
        //    finally
        //    {
        //        // Hide progress bar at the end
        //        Dispatcher.Invoke(() => progressLogin.Visibility = Visibility.Collapsed);
        //    }
        //}




        private async void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtUserRole .Text))
            {
                MessageBox.Show("Please fill in the fields marked with a star (*).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                progressLogin.Visibility = Visibility.Collapsed ;
            }

            else
            {
                progressLogin.Visibility = Visibility.Visible;

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
                        EmployeeId = txtEmployeeId.Text,

                    };


                    string url = string.Format("http://103.234.126.43:3080/dtmeter/user/{0}", txtUserId.Text);
                    HttpClient client = new HttpClient();
                    string jsonData = JsonConvert.SerializeObject(adminConfigPost);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                      
                        addNewUserEvent();

                       
                    }
                    else
                    {
                       

                        MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                catch 
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }

                finally
                {
                  
                    progressLogin.Visibility = Visibility.Collapsed;
                  
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
                    UserID = CurrentUserId2,
                    EmployeeId = CurrentUserEmployeeId2,
                    UserName = CurrentUserName2,

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
                   
                    MessageBox.Show("User successfully updated.", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    
                }
            }


            catch 
            {
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
