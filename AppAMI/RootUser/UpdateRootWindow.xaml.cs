using AppAMI.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace AppAMI.RootUser
{
    /// <summary>
    /// Interaction logic for UpdateRootWindow.xaml
    /// </summary>
    public partial class UpdateRootWindow : Window
    {
        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;

        public UpdateRootWindow(string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1)
        {
            InitializeComponent();


            CurrentUserId2 = CurrentUserId1;
            CurrentUserRole2 = CurrentUserRole1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;

            ReadDataBase();
        }

        async void ReadDataBase()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync("http://103.234.126.43:3080/dtmeter/user");
                string responseBody = await response.Content.ReadAsStringAsync();

                List<AdminConfig> adminConfigs = JsonConvert.DeserializeObject<List<AdminConfig>>(responseBody);

                foreach (AdminConfig adminConfig in adminConfigs)
                {
                    if (adminConfig.user_id.Equals(CurrentUserId2))
                    {
                        txtName.Text = adminConfig.user_name;
                        passPassword.Password = adminConfig.password;
                        cbOrganization .Text = adminConfig.organisation;

                        cbDepartment.Text = adminConfig.department;
                        cbDesignation.Text = adminConfig.designation;
                        //txtUserRole.Text = adminConfig.user_role;

                        txtEmail.Text = adminConfig.email;
                        txtPhone.Text = adminConfig.phone_number;
                        txtEmployeeId.Text = adminConfig.employee_id;
                        return;
                    }

                }

                MessageBox.Show("No match found for employee ID: " + CurrentUserId2 + ". Incorrect Emloyee ID or Password");

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
                        UserID = CurrentUserId2,
                        EmployeeId = txtEmployeeId.Text,
                        UserName = txtName.Text,
                        Password = passPassword.Password,

                        Organization = cbOrganization.Text,
                        Department = cbDepartment.Text,
                        Designation = cbDesignation.Text,

                        UserRole = CurrentUserRole2,
                        Email = txtEmail.Text,
                        Phone = txtPhone.Text,



                    };


                    string url = string.Format("http://103.234.126.43:3080/dtmeter/user/{0}", CurrentUserId2);
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
                    UserID = CurrentUserId2,
                    EmployeeId = CurrentUserEmployeeId2,
                    UserName = CurrentUserName2,

                    EventLogs = "Edited User",

                    Date = UserEventDate,
                    Time = UserEventTime,

                    StatusAdmin1 = "Not Acknowledged",
                    StatusAdmin2 = "Not Acknowledged",
                    StatusAdmin3 = "Not Acknowledged",

                    Remarks = "Edited : " + CurrentUserId2,
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
