using AppAMI.Classes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AppAMI.UserConfig
{
    /// <summary>
    /// Interaction logic for UserConfiguration.xaml
    /// </summary>
    public partial class UserConfiguration : UserControl
    {
        List<UserEvent> userAddEvent;
        List<UserEvent> userEditEvent;
        List<UserEvent> userDeleteEvent;
        List<UserEvent> userLoginEvent;
        List<UserEvent> userLogOutEvent;


        string CurrentUserId1;
        string CurrentUserRole1;
        string CurrentUserPassword1;
        string CurrentUserName1;
        string CurrentUserEmployeeId1;


        public UserConfiguration(string CurrentUserId, string CurrentUserRole, string CurrentUserPassword, string CurrentUserName, string CurrentUserEmployeeId)
        {
            InitializeComponent();

            CurrentUserId1 = CurrentUserId;
            CurrentUserRole1= CurrentUserRole;
            CurrentUserPassword1 = CurrentUserPassword;
            CurrentUserName1 = CurrentUserName;
            CurrentUserEmployeeId1 = CurrentUserEmployeeId;

            AccessLevelMethod();

        }

        private async void AccessLevelMethod()
        {
            if (CurrentUserRole1.Equals("Administrator"))
            {
                btnAdd.IsEnabled = true;
                btnShowNoti.IsEnabled = true;
                datagridUser.Columns[9].IsHidden = false;
                datagridUser.Columns[10].IsHidden = false;
            }
            else if (CurrentUserRole1.Equals("Operator"))
            {
                btnAdd.IsEnabled = false;
                btnShowNoti.IsEnabled = false;
                datagridUser.Columns[9].IsHidden = true;
                datagridUser.Columns[10].IsHidden = true;
            }
            await ReadDatabase();
        }

        private async Task ReadDatabase()
        {
            datagridUser.ClearFilters();
            progressLogin.Visibility = Visibility.Visible;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "http://103.234.126.43:3080/dtmeter/user";
                    string json = await client.GetStringAsync(url);
                    await Task.Delay(1);

                    List<AdminConfig> all_data = JsonConvert.DeserializeObject<List<AdminConfig>>(json);
                    List<AdminConfig> users = all_data
                        .Where(a => a.user_role == "Operator" || a.user_role == "Basic User" || a.user_role == "Administrator")
                        .ToList();

                    await Dispatcher.InvokeAsync(() =>
                    {
                        datagridUser.ItemsSource = users;
                    });

                    List<AdminConfig> admins = all_data.Where(a => a.user_role == "Administrator").ToList();
                    List<AdminConfig> operators = all_data.Where(a => a.user_role == "Operator").ToList();
                    List<AdminConfig> basicUsers = all_data.Where(a => a.user_role == "Basic User").ToList();

                    await Dispatcher.InvokeAsync(() =>
                    {
                        lblTotAdmin.Content = admins.Count;
                        lblTotOperator.Content = operators.Count;
                        lblTotBasicUser.Content = basicUsers.Count;

                        int totalUsers = admins.Count + operators.Count + basicUsers.Count;
                        lblTotUser.Content = totalUsers;
                    });
                }
            }
            catch 
            {
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressLogin.Visibility = Visibility.Collapsed;

                await GetUserEvent();
                await updateNotifyBadgeContentWebUser();
            }
        }

        private async Task updateNotifyBadgeContentWebUser()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                progressLogin.Visibility = Visibility.Visible;
            });

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "http://103.234.126.43:3080/dtmeter/newuser";
                    string json = await client.GetStringAsync(url);

                    List<AdminConfig> all_data = JsonConvert.DeserializeObject<List<AdminConfig>>(json);
                   
                    await Dispatcher.InvokeAsync(() =>
                    {
                        int badgeWebUser1 = all_data.Count;
                        badgeNoti.Content = badgeWebUser1.ToString();
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

        

        private async  void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NewUserWindow newUserWindow = new NewUserWindow(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);
            _ = newUserWindow.ShowDialog();

            await ReadDatabase();            
        }

        private async  void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            AdminConfig selectedAdminConfig = datagridUser.SelectedItem as AdminConfig;

          
            if (selectedAdminConfig != null)
            {
                UpdateUserWindow updateUserWindow   = new UpdateUserWindow(selectedAdminConfig, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);
                updateUserWindow.ShowDialog();
            }

            await ReadDatabase();
            //updateNotifyBatgeContent();
        }

        private async  void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            AdminConfig selectedAdminConfig = datagridUser.SelectedItem as AdminConfig;

            if (selectedAdminConfig != null)
            {
                DeleteUserWindow deleteUserWindow   = new DeleteUserWindow(selectedAdminConfig, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);
                deleteUserWindow.ShowDialog();
            }

            await ReadDatabase();
            //updateNotifyBatgeContent();
        }

        private async  void btnShowNoti_Click(object sender, RoutedEventArgs e)
        {

            WebUserAdd newUserWindow = new WebUserAdd(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);
            _ = newUserWindow.ShowDialog();

            await ReadDatabase();
        }
        

        #region Grid Resize
        private bool isResizingBottom = false;

        private void BottomRegion_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizingBottom)
            {
                var rect = (Rectangle)sender;
                var grid = FindParent<Grid>(rect, "grdEvents");

                if (grid != null)
                {
                    double newHeight = grid.ActualHeight - e.GetPosition(grid).Y;

                    if (newHeight > 0)
                    {
                        grid.Height = newHeight;
                        rect.Margin = new Thickness(0, -e.GetPosition(grid).Y, 0, 0);
                    }
                }
            }
        }

        private void BottomRegion_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isResizingBottom = true;
            Mouse.Capture((IInputElement)sender);
            Mouse.OverrideCursor = Cursors.SizeNS; // Set the cursor to SizeNS when moving up and down
        }

        private void BottomRegion_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isResizingBottom = false;
            Mouse.Capture(null);
            Mouse.OverrideCursor = null; // Reset the cursor
        }

        private void BottomRegion_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeWE; // Set the cursor to SizeWE when hovering over recGridResize
        }

        private void BottomRegion_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!isResizingBottom)
            {
                Mouse.OverrideCursor = null; // Reset the cursor when not resizing
            }
        }

        // Helper method to find parent element by name
        private T FindParent<T>(DependencyObject child, string parentName) where T : DependencyObject
        {
            DependencyObject current = child;
            while (current != null && current.GetType() != typeof(T))
            {
                var frameworkElement = current as FrameworkElement;
                if (frameworkElement != null && frameworkElement.Name == parentName)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            return (T)current;
        }

        private void recGridResize_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeNS;
        }

        private void recGridResize_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null; // Reset the cursor when not resizing

        }

        #endregion Grid Resize

        #region Events
        private async Task GetUserEvent()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                progressLogin.Visibility = Visibility.Visible;
            });

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "http://103.234.126.43:3080/dtmeter/logs/events/userevents";
                    string json = await client.GetStringAsync(url);

                    List<UserEvent> all_data = JsonConvert.DeserializeObject<List<UserEvent>>(json);



                    userAddEvent = all_data.Where(a => a.event_log == "Added User").ToList();
                    userEditEvent = all_data.Where(a => a.event_log == "Edited User").ToList();
                    userDeleteEvent = all_data.Where(a => a.event_log == "Deleted User").ToList();
                    userLoginEvent = all_data.Where(a => a.event_log == "User Logged In").ToList();
                    userLogOutEvent = all_data.Where(a => a.event_log == "User Logged Out").ToList();

                    await Dispatcher.InvokeAsync(() =>
                    {
                        lblUserAdded.Content = userAddEvent.Count;
                        lblUserEdited.Content = userEditEvent.Count;
                        lblUserDeleted.Content = userDeleteEvent.Count;
                        lblLogin.Content = userLoginEvent.Count;
                        lblLogout.Content = userLogOutEvent.Count;

                        //int totalUserEvents = userAddEvent.Count + userEditEvent.Count + userDeleteEvent.Count + userLoginEvent.Count + userLogOutEvent.Count;
                        //badgeNoti.Content = totalUserEvents;
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

        private void btnUserAdded_Click(object sender, RoutedEventArgs e)
        {
            datagridUserEvent.ItemsSource = userAddEvent;

            ResetButtonBackground();

            // Set the background color for the clicked button
            Button clickedButton = (Button)sender;
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a5e3"));
        }

        private void btnUserEdited_Click(object sender, RoutedEventArgs e)
        {
            datagridUserEvent.ItemsSource = userEditEvent;

            ResetButtonBackground();

            // Set the background color for the clicked button
            Button clickedButton = (Button)sender;
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a5e3"));

        }

        private void btnUserDeleted_Click(object sender, RoutedEventArgs e)
        {
            datagridUserEvent.ItemsSource = userDeleteEvent;

            ResetButtonBackground();

            // Set the background color for the clicked button
            Button clickedButton = (Button)sender;
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a5e3"));
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            datagridUserEvent.ItemsSource = userLoginEvent;

            ResetButtonBackground();

            // Set the background color for the clicked button
            Button clickedButton = (Button)sender;
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a5e3"));
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            datagridUserEvent.ItemsSource = userLogOutEvent;

            ResetButtonBackground();

            // Set the background color for the clicked button
            Button clickedButton = (Button)sender;
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a5e3"));
        }

        private void ResetButtonBackground()
        {
            btnUserAdded.Background = btnUserEdited.Background = btnUserDeleted.Background = btnLogin.Background = btnLogout.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2d2d30"));
        }

        #endregion Events
    }
}
