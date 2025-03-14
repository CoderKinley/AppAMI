using AppAMI.Classes;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace AppAMI.RootUser
{
    /// <summary>
    /// Interaction logic for DeleteAdminWindow.xaml
    /// </summary>
    public partial class DeleteAdminWindow : Window
    {
        AdminConfig selectedAdminConfig;

        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;

        public DeleteAdminWindow(AdminConfig selectedAdminConfig, string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1)
        {
            InitializeComponent();

            this.selectedAdminConfig = selectedAdminConfig;

            txtUserId.Text = this.selectedAdminConfig.user_id ;
            txtName.Text = this.selectedAdminConfig.user_name;
            passPassword.Password  = this.selectedAdminConfig.password;

            txtOrganisation.Text = this.selectedAdminConfig.organisation;
            txtDepartment.Text = this.selectedAdminConfig.department;
            txtDesignation.Text = this.selectedAdminConfig.designation;


            txtUserRole.Text = this.selectedAdminConfig.user_role;
            txtEmail.Text = this.selectedAdminConfig.email;
            txtPhone.Text = this.selectedAdminConfig.phone_number;
            txtEmployeeId.Text = this.selectedAdminConfig.employee_id;


            CurrentUserId2 = CurrentUserId1;
            CurrentUserRole2 = CurrentUserRole1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;
        }



        private async void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AdminConfig adminConfig   = new AdminConfig()
                {
                    //user_id = txtUserId.Text,
                    //user_name = txtName.Text,
                    //password = txtPassword.Text,

                    //oranisation = txt
                    //department = txtDepartment.Text,
                    //designation = txtDesignation.Text,
                    //user_role = cbUserRole.Text,

                };


                string url = string.Format("http://103.234.126.43:3080/dtmeter/user/{0}", txtUserId.Text);
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(adminConfig);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.DeleteAsync(url);

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
                    UserID = CurrentUserId2,
                    EmployeeId = CurrentUserEmployeeId2,
                    UserName = CurrentUserName2,

                    EventLogs = "Deleted User",

                    Date = UserEventDate,
                    Time = UserEventTime,

                    StatusAdmin1 = "Not Acknowledged",
                    StatusAdmin2 = "Not Acknowledged",
                    StatusAdmin3 = "Not Acknowledged",

                    Remarks = "Deleted : " + txtUserId.Text,
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
