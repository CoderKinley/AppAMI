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
    /// Interaction logic for UpdateAdminWindow.xaml
    /// </summary>
    public partial class UpdateAdminWindow : Window
    {
        AdminConfig selectedAdminConfig;

        string CurrentUserId;
        string CurrentUserName;
        string CurrentUserEmployeeId;

        public UpdateAdminWindow(AdminConfig selectedAdminConfig, string currentUserId, string currentUserName, string currentUserEmployeeId)
        {
            InitializeComponent();

            
            this.selectedAdminConfig = selectedAdminConfig;

            txtUserId.Text = this.selectedAdminConfig.user_id ;


            txtName.Text = this.selectedAdminConfig.user_name;
            passPassword.Password = this.selectedAdminConfig.password;

            cbOrganization.Text = this.selectedAdminConfig.organisation;
            cbDepartment.Text = this.selectedAdminConfig.department;
            cbDesignation.Text = this.selectedAdminConfig.designation;


            txtUserRole.Text = this.selectedAdminConfig.user_role;
            txtEmail.Text = this.selectedAdminConfig.email;
            txtPhone.Text = this.selectedAdminConfig.phone_number;

            txtEmployeeId.Text = this.selectedAdminConfig.employee_id;



            CurrentUserId = currentUserId;
            CurrentUserName = currentUserName;
            CurrentUserEmployeeId = currentUserEmployeeId;
        }


      


        private async void updateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                AdminConfigPost adminConfigPost  = new AdminConfigPost()
                {
                    UserID = txtUserId.Text ,
                    UserName = txtName.Text,
                    Password = passPassword.Password ,

                    Organization = cbOrganization .Text,
                    Department = cbDepartment.Text,
                    Designation = cbDesignation.Text,

                    UserRole = txtUserRole.Text,
                    Email = txtEmail.Text,
                    Phone = txtPhone.Text,
                    EmployeeId = txtEmployeeId .Text ,

                };


                string url = string.Format("http://119.2.119.202:3080/dtmeter/user/{0}", txtUserId.Text);
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


        private async void addNewUserEvent()
        {
            DateTime now = DateTime.Now;
            string UserEventDate = now.ToString("dd-MM-yyyy");
            string UserEventTime = now.ToString("h:mm tt");

            try
            {
                UserEventPost userEventPost = new UserEventPost()
                {
                    UserID = CurrentUserId,
                    EmployeeId = CurrentUserEmployeeId,
                    UserName = CurrentUserName,

                    EventLogs = "Edited User",

                    Date = UserEventDate,
                    Time = UserEventTime,

                    StatusAdmin1 = "Not Acknowledged",
                    StatusAdmin2 = "Not Acknowledged",
                    StatusAdmin3 = "Not Acknowledged",

                    Remarks = "Edited : " + txtUserId.Text,
                };

                string url = "http://119.2.119.202:3080/dtmeter/logs/events/userevents";
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
