using AppAMI.Classes;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace AppAMI.UserConfig
{
    /// <summary>
    /// Interaction logic for DeleteUserWindow.xaml
    /// </summary>
    public partial class DeleteUserWindow : Window
    {
        AdminConfig selectedAdminConfig;

        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;

        public DeleteUserWindow(AdminConfig selectedAdminConfig, string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1)
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
            txtEmployeeId.Text = this.selectedAdminConfig.employee_id ;


            CurrentUserId2 = CurrentUserId1;
            CurrentUserRole2 = CurrentUserRole1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;


            if (txtUserRole.Text.Equals("Administrator"))
            {
                deleteButton.IsEnabled = false;
            }

            else
            {
                deleteButton.IsEnabled = true;

            }
        }



        private async void deleteButton_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please fill in the fields marked with a star (*).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                progressLogin.Visibility = Visibility.Collapsed;
            }

            else
            {
                try
                {
                    AdminConfig adminConfig = new AdminConfig()
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

                        addNewUserEvent();

                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                catch
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);

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
                    MessageBox.Show("User successfully deleted.", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);
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
