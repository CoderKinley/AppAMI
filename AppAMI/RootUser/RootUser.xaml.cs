using AppAMI.Classes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AppAMI.RootUser
{
    /// <summary>
    /// Interaction logic for RootUser.xaml
    /// </summary>
    public partial class RootUser : UserControl
    {
        string CurrentUserId1;
        string CurrentUserRole1;
        string CurrentUserPassword1;
        string CurrentUserName1;
        string CurrentUserEmployeeId1;


        public RootUser(string CurrentUserId, string CurrentUserRole, string CurrentUserPassword, string CurrentUserName, string CurrentUserEmployeeId)
        {
            InitializeComponent();

            CurrentUserId1 = CurrentUserId;
            CurrentUserRole1 = CurrentUserRole;
            CurrentUserPassword1 = CurrentUserPassword;
            CurrentUserName1 = CurrentUserName;
            CurrentUserEmployeeId1 = CurrentUserEmployeeId;

            GetData();

            
        }

        private async void GetData()
        {
            await ReadDatabase();
            await ReadRootDatabase();
            await ReadRootEvents();
        }


        #region admin

       

        private async Task ReadDatabase()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                progressLogin.Visibility = Visibility.Visible;
            });

            try
            {
                using (WebClient web = new WebClient())
                {
                    string url = string.Format("http://103.234.126.43:3080/dtmeter/user");
                    string json = await web.DownloadStringTaskAsync(url);

                    List<AdminConfig> all_data = JsonConvert.DeserializeObject<List<AdminConfig>>(json);

                    List<AdminConfig> admins = all_data.Where(a => a.user_role == "Administrator").ToList();

                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (admins.Count > 0)
                        {
                            datagridAdmin.ItemsSource = admins;

                            if (admins.Count == 3)
                            {
                                btnAddAdmin.IsEnabled = false;
                            }

                            else
                            {
                                btnAddAdmin.IsEnabled = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    });
                }
            }
            catch
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    progressLogin.Visibility = Visibility.Collapsed;
                });
            }


        }


        private async  void btnAddAdmin_Click(object sender, RoutedEventArgs e)
        {
            NewAdminWindow newAdminWindow = new NewAdminWindow(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);
            newAdminWindow.ShowDialog();

            await ReadDatabase();
            await ReadRootEvents();

        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            AdminConfig selectedAdminConfig = datagridAdmin.SelectedItem as AdminConfig;

            if (selectedAdminConfig != null)
            {
                DeleteAdminWindow deleteAdminWindow  = new DeleteAdminWindow(selectedAdminConfig, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);
                deleteAdminWindow.ShowDialog();

                await ReadDatabase();
                await ReadRootEvents();

            }
        }

        #endregion admin


        #region rootUser

        private async Task ReadRootDatabase()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                progressLogin.Visibility = Visibility.Visible;
            });

            try
            {
                using (WebClient web = new WebClient())
                {
                    string url = "http://103.234.126.43:3080/dtmeter/user";
                    string json = await web.DownloadStringTaskAsync(url);

                    List<AdminConfig> allData = JsonConvert.DeserializeObject<List<AdminConfig>>(json);

                    List<AdminConfig> rootUsers = allData.Where(a => a.user_role == "Root User").ToList();
                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (rootUsers.Count > 0)
                        {
                            //txtUserId.Text = rootUsers[0].user_id;
                            //txtUserRole.Text = rootUsers[0].user_role;
                            txtName.Text = rootUsers[0].user_name;

                            txtOrga.Text = rootUsers[0].organisation;
                            txtDept.Text = rootUsers[0].department;
                            txtDesig.Text = rootUsers[0].designation;

                            txtEmail.Text = rootUsers[0].email;
                            txtPhone.Text = rootUsers[0].phone_number;
                            //txUserPassword.Text = rootUsers[0].password;

                            txtEmployeeId.Text = rootUsers[0].employee_id;



                           
                        }
                    });
                }
            }
            catch
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    progressLogin.Visibility = Visibility.Collapsed;
                });
            }
        }

        private async Task ReadRootEvents()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                progressLogin.Visibility = Visibility.Visible;
            });


            try
            {
                using (WebClient web = new WebClient())
                {
                    string url = string.Format("http://103.234.126.43:3080/dtmeter/logs/events/userevents");
                    string json = await web.DownloadStringTaskAsync(url);

                    List<UserEvent> all_data = JsonConvert.DeserializeObject<List<UserEvent>>(json);

                    List<UserEvent> userAddEvent = all_data.Where(a => a.user_id == CurrentUserId1).ToList();

                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (userAddEvent.Count > 0)
                        {
                            datagridUser.ItemsSource = userAddEvent;
                        }
                        else
                        {
                            MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    });
                }
            }
            catch
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    progressLogin.Visibility = Visibility.Collapsed;
                });
            }

        }







        #endregion rootUser


        private async void menEditUser_Click(object sender, RoutedEventArgs e)
        {

            UpdateRootWindow updateRootWindow = new UpdateRootWindow(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);
            updateRootWindow.Show();

            await ReadRootDatabase();
            await ReadRootEvents();


        }

        private async void menTransCre_Click(object sender, RoutedEventArgs e)
        {
            string currentRootEmail = txtEmail.Text;
           

            NewRootWindow newRootWindow = new NewRootWindow(currentRootEmail, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);
            newRootWindow.Show();

            await ReadRootDatabase();
            await ReadRootEvents();

        }



        private void menUserDropDown_Click(object sender, RoutedEventArgs e)
        {
            UserDropDownWindow userDropDownWindow  = new UserDropDownWindow();
            userDropDownWindow.ShowDialog();
        }

        private void menuMeterDropDown_Click(object sender, RoutedEventArgs e)
        {
            MeterDropDownWindow meterDropDownWindow   = new MeterDropDownWindow();
            meterDropDownWindow.ShowDialog();
        }

        private void menuMriDropDown_Click(object sender, RoutedEventArgs e)
        {
            MriDropDownWindow mriDropDownWindow   = new MriDropDownWindow();
            mriDropDownWindow.ShowDialog();
        }

    }
}
