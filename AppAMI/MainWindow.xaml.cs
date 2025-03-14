using AppAMI.Classes;
using AppAMI.Login;
using AppAMI.RootUser;
using AppAMI.UserConfig;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AppAMI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        UserControl usc = null;

        string CurrentUserId;
        string CurrentUserRole;
        string CurrentUserPassword;
        string CurrentUserName;
        string CurrentUserEmployeeId;

        public MainWindow(string userId, string userRole, string password, string userName, string employeeId)
        {
            InitializeComponent();

            this.Title = "omniAMI";

            windowsMax();
            GridCursor.Visibility = Visibility.Hidden;


            CurrentUserId = userId;
            CurrentUserRole = userRole;
            CurrentUserPassword = password;
            CurrentUserName = userName;
            CurrentUserEmployeeId = employeeId;

            AccessLevelMethod();
            AddUserInitials();


            ViewModel1 viewModel = new ViewModel1();
            DataContext = viewModel;

            //_ = getImageAsync();
        }

        //private async Task getImageAsync()
        //{
        //    string apiUrl = $"http://103.234.126.43:3080/api/user/image/{CurrentUserId}"; // Replace with your actual API endpoint

        //    HttpClient client = new HttpClient();

        //    try
        //    {
        //        HttpResponseMessage response = await client.GetAsync(apiUrl);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            // Assuming the API returns the image as a byte array
        //            byte[] imageData = await response.Content.ReadAsByteArrayAsync();

        //            // Create a BitmapImage from the byte array
        //            BitmapImage bitmapImage = new BitmapImage();
        //            bitmapImage.BeginInit();
        //            bitmapImage.StreamSource = new System.IO.MemoryStream(imageData);
        //            bitmapImage.EndInit();

        //            // Set the Source property of the Image control
        //            imgPic.Source = bitmapImage;
        //        }
        //        else
        //        {
        //            //MessageBox.Show($"Error: {response.StatusCode}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show($"Error: {ex.Message}");
        //    }
        //    finally
        //    {
        //        client.Dispose();
        //    }
        //}



        private void AddUserInitials()
        {

            string fullName = CurrentUserName?.ToString();
            if (!string.IsNullOrEmpty(fullName))
            {
                string[] names = fullName.Split(' ');

                StringBuilder initials = new StringBuilder();
                foreach (string name in names)
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        initials.Append(name[0]);
                    }
                }

                lblUserInitials.Content = initials.ToString();
            }
            else
            {
            }
        }


        private void AccessLevelMethod()
        {
            if (CurrentUserRole.Equals("Root User"))
            {
                GridCursor.Visibility = Visibility.Visible;

                btnUser.Visibility = Visibility.Collapsed;
                btnMeterConfiguration.Visibility = Visibility.Collapsed;
                btnMeterData.Visibility = Visibility.Collapsed;

                btnInstant.Visibility = Visibility.Collapsed;
                btnBilling.Visibility = Visibility.Collapsed;
                btnLoadSurvey.Visibility = Visibility.Collapsed;
                btnLoadSurvey1.Visibility = Visibility.Collapsed;
                btnReliability.Visibility = Visibility.Collapsed;
                btnEvents.Visibility = Visibility.Collapsed;

                btnReport.Visibility = Visibility.Collapsed;
                btnGIS.Visibility = Visibility.Collapsed;
                //btnNMS.Visibility = Visibility.Collapsed;
                btnMriMonitoring.Visibility = Visibility.Collapsed;

                btnRoot.Visibility = Visibility.Visible;


                GridAmrView.Children.Clear();
                usc = new RootUser.RootUser(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
                GridAmrView.Children.Add(usc);

                //menuAccSetting.Visibility = Visibility.Collapsed;
            }

            else if(CurrentUserRole.Equals("Administrator"))
            {
                GridCursor.Visibility = Visibility.Collapsed;

                btnUser.Visibility = Visibility.Visible ;
                btnMeterConfiguration.Visibility = Visibility.Visible;
                btnMeterData.Visibility = Visibility.Visible;

                btnInstant.Visibility = Visibility.Visible;
                btnBilling.Visibility = Visibility.Visible;
                btnLoadSurvey.Visibility = Visibility.Visible;
                btnLoadSurvey1.Visibility = Visibility.Visible;
                btnReliability.Visibility = Visibility.Visible;

                btnEvents.Visibility = Visibility.Visible;

                btnReport.Visibility = Visibility.Visible;
                btnGIS.Visibility = Visibility.Visible;
                //btnNMS.Visibility = Visibility.Visible;
                btnMriMonitoring.Visibility = Visibility.Visible;

                btnRoot.Visibility = Visibility.Collapsed;
            }

            else if (CurrentUserRole.Equals("Operator"))
            {
                GridCursor.Visibility = Visibility.Collapsed;

                btnUser.Visibility = Visibility.Visible;
                btnMeterConfiguration.Visibility = Visibility.Visible;
                btnMeterData.Visibility = Visibility.Visible;

                btnInstant.Visibility = Visibility.Visible;
                btnBilling.Visibility = Visibility.Visible;
                btnLoadSurvey.Visibility = Visibility.Visible;
                btnLoadSurvey1.Visibility = Visibility.Visible;
                btnReliability.Visibility = Visibility.Visible;

                btnEvents.Visibility = Visibility.Visible;

                btnReport.Visibility = Visibility.Visible;
                btnGIS.Visibility = Visibility.Visible;
                //btnNMS.Visibility = Visibility.Visible;
                btnMriMonitoring.Visibility = Visibility.Visible;

                btnRoot.Visibility = Visibility.Collapsed;

            }         
        }


        private Dictionary<int, UserControl> cachedUserControls = new Dictionary<int, UserControl>();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GridCursor.Visibility = Visibility.Visible;

            int index = int.Parse(((Button)e.Source).Uid);

            GridCursor.Margin = new Thickness(15 + (100 * index), 0, 0, 0);

            NavigateToUserControl(index);
        }

        //private void NavigateToUserControl(int index)
        //{
        //    if (!cachedUserControls.ContainsKey(index))
        //    {
        //        // Create a new instance only if it doesn't exist in the cache
        //        UserControl usc = CreateNewUserControl(index);
        //        cachedUserControls[index] = usc;
        //    }

        //    // Show the cached user control
        //    GridAmrView.Children.Clear();
        //    GridAmrView.Children.Add(cachedUserControls[index]);

        //    // Move the GridCursor accordingly
        //    MoveGridCursor(index);
        //}


        public void NavigateToUserControl(int index, bool moveCursor = true)
        {
            if (!cachedUserControls.ContainsKey(index))
            {
                UserControl usc = CreateNewUserControl(index);
                cachedUserControls[index] = usc;
            }

            // Show the cached user control
            GridAmrView.Children.Clear();
            GridAmrView.Children.Add(cachedUserControls[index]);

            // Set the DataContext to the existing ViewModel1 instance
            if (cachedUserControls[index] is GIS.GIS  gisControl)
            {
                gisControl.DataContext = DataContext;
            }

            // Move the GridCursor if specified
            if (moveCursor)
            {
                MoveGridCursor(index);
            }
        }

        public  void MoveGridCursor(int index)
        {
            GridCursor.Margin = new Thickness(15 + (100 * index), 0, 0, 0);
        }

        // Public method for external navigation
        public void NavigateToMeterData()
        {
            NavigateToUserControl(2);
        }

        private UserControl CreateNewUserControl(int index)
        {
            // Create a new instance based on the index
            switch (index)
            {
                case 0:
                    return new UserConfiguration(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                case 1:
                    return new MeterConfig.MeterConfiguration(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                case 2:
                    return new MeterData.MeterData(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                case 3:
                    return new InstantAll.InstantAll(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                case 4:
                    return new BillingAll.BillingAll(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                case 5:
                    return new LoadSurveyAll.LoadSurveyAll(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                case 6:
                    return new LoadSurveyAll1(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                case 7:
                    return new Events.Events(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                case 8:
                    return new Report.Report(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                case 9:
                    return new ReliabilityIndices.ReliabilityIndices(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                //case 10:
                //    return new NMS.NMS(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                case 10:
                    return new NmsMri.NmsMri(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                case 11:
                    return new GIS.GIS(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);


                // Add other cases as needed

                default:
                    // Handle unknown index
                    return null;
            }
        }



        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    GridCursor.Visibility = Visibility.Visible;

        //    int index = int.Parse(((Button)e.Source).Uid);

        //    GridCursor.Margin = new Thickness(15 + (100 * index), 0, 0, 0);

        //    switch (index)
        //    {
        //        case 0:
        //            GridAmrView.Children.Clear();
        //            usc = new UserConfiguration(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
        //            GridAmrView.Children.Add(usc);

        //            break;
        //        case 1:

        //            GridAmrView.Children.Clear();
        //            usc = new MeterConfig.MeterConfiguration(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
        //            GridAmrView.Children.Add(usc);


        //            break;
        //        case 2:
        //            GridAmrView.Children.Clear();
        //            usc = new MeterData.MeterData(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
        //            GridAmrView.Children.Add(usc);
        //            break;
        //        case 3:
        //            GridAmrView.Children.Clear();
        //            usc = new InstantAll.InstantAll(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
        //            GridAmrView.Children.Add(usc);
        //            break;
        //        case 4:
        //            GridAmrView.Children.Clear();
        //            usc = new BillingAll.BillingAll(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
        //            GridAmrView.Children.Add(usc);
        //            break;
        //        case 5:
        //            GridAmrView.Children.Clear();
        //            usc = new LoadSurveyAll.LoadSurveyAll(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
        //            GridAmrView.Children.Add(usc);
        //            break;

        //        case 6:
        //            GridAmrView.Children.Clear();
        //            usc = new LoadSurveyAll1(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
        //            GridAmrView.Children.Add(usc);
        //            break;

        //        case 7:
        //            GridAmrView.Children.Clear();
        //            usc = new Events.Events(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
        //            GridAmrView.Children.Add(usc);
        //            break;

        //        case 8:
        //            GridAmrView.Children.Clear();
        //            usc = new Report.Report(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
        //            GridAmrView.Children.Add(usc);
        //            break;

        //        case 9:
        //            GridAmrView.Children.Clear();
        //            usc = new ReliabilityIndices.ReliabilityIndices(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
        //            GridAmrView.Children.Add(usc);
        //            break;

        //        case 10:
        //            GridAmrView.Children.Clear();
        //            usc = new NMS.NMS(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
        //            GridAmrView.Children.Add(usc);
        //            break;

        //        case 11:
        //            GridAmrView.Children.Clear();
        //            usc = new GIS.GIS(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
        //            GridAmrView.Children.Add(usc);
        //            break;
        //    }

        //}



        private void btnRoot_Click(object sender, RoutedEventArgs e)
        {

            GridAmrView.Children.Clear();
            usc = new RootUser.RootUser(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
            GridAmrView.Children.Add(usc);
        }


        private void btnUserAccount_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();

            contextMenu.Background = (Brush)new BrushConverter().ConvertFrom("#202020");

            Style menuItemStyle = new Style(typeof(MenuItem));
            menuItemStyle.Setters.Add(new Setter(ForegroundProperty, Brushes.White));
            menuItemStyle.Setters.Add(new Setter(FontSizeProperty, 12.0));
            menuItemStyle.Setters.Add(new Setter(MarginProperty, new Thickness(20, 5, 20, 5)));


            MenuItem menLogout = new MenuItem { Header = "Logout", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.ExitToApp, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
            menLogout.Click += menLogout_Click;
            contextMenu.Items.Add(menLogout);

            MenuItem menAccSet = new MenuItem { Header = "Account Setting", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.AccountSettings, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
            menAccSet.Click += menAccSet_Click;
            contextMenu.Items.Add(menAccSet);



            btnUserAccount.ContextMenu = contextMenu;
            btnUserAccount.Focus();
        }

        private void menAccSet_Click(object sender, RoutedEventArgs e)
        {
           

            if (CurrentUserRole.Equals("Administrator"))
            {
                UpdateAdminSetAccWindow updateAdminSetAccWindow = new UpdateAdminSetAccWindow(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
                updateAdminSetAccWindow.Show();
            }

            else if (CurrentUserRole.Equals("Operator"))
            {
                UpdateOperatorSetAccWindow updateOperatorSetAccWindow = new UpdateOperatorSetAccWindow(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);
                updateOperatorSetAccWindow.Show();
            }
        }

        private void menLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Logging out.  Are you sure?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                addNewUserEvent();

                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();

                Close();
            }
            else
            {
                // Code to cancel the action
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

                    //EventLogs = "Added User, UID: " + txtUserId.Text,
                    EventLogs = "User Logged Out",

                    Date = UserEventDate,
                    Time = UserEventTime,

                    StatusAdmin1 = "Not Acknowledged",
                    StatusAdmin2 = "Not Acknowledged",
                    StatusAdmin3 = "Not Acknowledged",

                    Remarks = "Logged Out : " + CurrentUserId,
                };

                string url = "http://103.234.126.43:3500/dtmeter/logs/events/userevents";
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

        #region Windows Control

        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
 
            windowsMax();
        }

        private void windowsMax()
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                IcoMaximize.Visibility = Visibility.Collapsed;
                IcoNormalize.Visibility = Visibility.Visible;
            }

            else if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                IcoMaximize.Visibility = Visibility.Visible;
                IcoNormalize.Visibility = Visibility.Collapsed;
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {

            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Minimized;
            }

            else if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            addNewUserEvent();

            Application.Current.Shutdown();
        }


        private void grdTop1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }



        #endregion Windows Control





       
    }


}
