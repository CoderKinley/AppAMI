using AppAMI.Classes;
using AppAMI.Login;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AppAMI
{
    /// <summary>
    /// Interaction logic for NewPasswordWindow.xaml
    /// </summary>
    public partial class NewPasswordWindow : Window
    {
        public NewPasswordWindow(string employeeIdPassChange)
        {
            InitializeComponent();
            txtUserId.Text = employeeIdPassChange;

            ReadDataBase();


            PassCurrPass .IsEnabled = false;
            PassNewPass .IsEnabled = false;

        }

       
        async  void ReadDataBase()
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync("http://103.234.126.43:3500/dtmeter/user");
                var responseBody = await response.Content.ReadAsStringAsync();

                var adminConfigs = JsonConvert.DeserializeObject<List<AdminConfig>>(responseBody);

                foreach (var adminConfig in adminConfigs)
                {
                    if (adminConfig.employee_id.Equals(txtUserId.Text))
                    {
                        txtName.Text = adminConfig.user_name;
                        txtPass.Text = adminConfig.password;
                        txtOrganisation.Text = adminConfig.organisation;

                        txtDepartment .Text = adminConfig.department;
                        txtDesignation .Text = adminConfig.designation;
                        txtUserRole.Text = adminConfig.user_role;

                        txtEmail.Text = adminConfig.email;
                        txtPhone.Text = adminConfig.phone_number;

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

        private void txtVeri1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtVeri1.Text.Length == 1)
            {
                txtVeri2.Focus();
            }
        }

        private void txtVeri2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtVeri2.Text.Length == 1)
            {
                txtVeri3.Focus();
            }
        }

        private void txtVeri3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtVeri3.Text.Length == 1)
            {
                txtVeri4.Focus();
            }
        }

        private void txtVeri4_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtVeri4.Text.Length == 1)
            {
                txtVeri4.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                PassCurrPass.IsEnabled = true;
                PassNewPass.IsEnabled = true;
            }


            else
            {
                MessageBox.Show("Enter correct verification code");
            }



        }


        private void btnVerifyEmail_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("send to email address: " + txtEmail.Text);
        }

        private async  void btnSavePass_Click(object sender, RoutedEventArgs e)
        {
           
            if (PassCurrPass.Password.Equals(txtPass.Text))
            {
                try
                {

                    AdminConfigPost adminConfigPost = new AdminConfigPost()
                    {
                        EmployeeId = txtUserId.Text,
                        UserName = txtName.Text,
                        Password = PassNewPass.Password,

                        Organization = txtOrganisation.Text,
                        Department = txtDepartment.Text,
                        Designation = txtDesignation.Text,

                        UserRole = txtUserRole.Text,
                        Email = txtEmail.Text,
                        Phone = txtPhone.Text,


                    };


                    string url = string.Format("http://103.234.126.43:3500/dtmeter/user/{0}", txtUserId.Text);
                    HttpClient client = new HttpClient();
                    string jsonData = JsonConvert.SerializeObject(adminConfigPost);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(responseMessage);

                      // Close();


                        MessageBoxResult result = MessageBox.Show("Logging out.  Log in with your new password", "Confirmation", MessageBoxButton.OK , MessageBoxImage.Question);

                        if (result == MessageBoxResult.OK )
                        {
                            LoginWindow loginWindow = new LoginWindow();
                            loginWindow.Show();

                            Close();

                            foreach (Window window in Application.Current.Windows)
                            {
                                if (window != loginWindow && window != this)
                                {
                                    window.Close();
                                }
                            }


                        }
                        else
                        {
                            // Code to cancel the action
                        }
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show("Error updating Password: " + errorMessage);
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Unable to connect to the server" + ex.Message);
                    Close();
                }
            }

            else
            {
                MessageBox.Show("Incorrect current password");
            }


        }

        
    }
}
