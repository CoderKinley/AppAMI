using AppAMI.Classes;
using Microsoft.Win32;
using Newtonsoft.Json;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Linq;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using MaterialDesignThemes.Wpf;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Syncfusion.UI.Xaml.Grid;
using System.Text.RegularExpressions;

namespace AppAMI.InstantAll
{
    /// <summary>
    /// Interaction logic for InstantAll.xaml
    /// </summary>
    public partial class InstantAll : UserControl
    {
        private List<string> storedItems = new List<string>();

        List<UserEvent> userAddEvent;
        List<UserEvent> userEditEvent;
        List<UserEvent> userDeleteEvent;
        List<UserEvent> userLoginEvent;
        List<UserEvent> userLogOutEvent;


        List<MqttClass> allFetchedData;
        List<MqttClass> filteredDataPlace;
        //List<MqttClass> allData;
         MqttClass[] FetchedData;


        private MqttClient mqttClient1;
        private MqttClient mqttClient2;
        private MqttClient mqttClient3;


        private string mqttBrokerAddress1 = "103.234.126.44";
        private string mqttBrokerAddress2 = "103.234.126.41";
        private string mqttBrokerAddress3 = "103.234.126.42";


        string selectedEsd;
        string selectedDistrict;
        string selectedPlaceForData;



        string CurrentUserId1;
        string CurrentUserRole1;
        string CurrentUserPassword1;
        string CurrentUserName1;
        string CurrentUserEmployeeId1;


        string url;

        private List<MqttClass> allDataCountry; // Store your deserialized data

        private int successNumber = 0;
        private int unsuccessNumber = 0;
        private System.Timers.Timer timer;


        public InstantAll(string CurrentUserId, string CurrentUserRole, string CurrentUserPassword, string CurrentUserName, string CurrentUserEmployeeId)
        {
            InitializeComponent();

            CurrentUserId1 = CurrentUserId;
            CurrentUserRole1 = CurrentUserRole;
            CurrentUserPassword1 = CurrentUserPassword;
            CurrentUserName1 = CurrentUserName;
            CurrentUserEmployeeId1 = CurrentUserEmployeeId;


            Task.Run(() => loadData());

            grdEmailAddress.Visibility = Visibility.Collapsed;

            //btnStartPolling.IsEnabled = false;
            //btnStopPolling.IsEnabled = false;
          //  datagridInstantPara1Country.Visibility = Visibility.Collapsed;
            grdAllESDSummary.Visibility = Visibility.Collapsed;

        }

        #region from gis
        public void SetListBoxItems(List<string> items)
        {
            storedItems = new List<string>(items);
            createButton();
        }

        private void createButton()
        {

            selectedDistrict = "Multiple District";
            selectedPlaceForData = "DatafromGis";
            selectedEsd = "Multiple ESD";
          
            buttonText = "Data from GIS";
            // MessageBox.Show("Creating Button3");

            key = $"{selectedDistrict}_{selectedEsd}";
            existingContentPanel = stackCurrentData.Children.OfType<StackPanel>().FirstOrDefault(panel => panel.Tag != null && (string)panel.Tag == key);
            //MessageBox.Show("Creating Button5");

            if (existingContentPanel != null)
            {
                BtnViewData_Click(existingContentPanel.Children.OfType<Button>().FirstOrDefault(), new RoutedEventArgs(Button.ClickEvent));
            }
            else
            {

                StackPanel contentPanel = new StackPanel();
                contentPanel.Orientation = Orientation.Horizontal;
                contentPanel.Margin = new Thickness(0, 0, 1, 0);
                contentPanel.Background = new SolidColorBrush(DefaultBackgroundColor);


                Button btnViewData = new Button();
                btnViewData.Content = buttonText;
                btnViewData.Foreground = Brushes.White;
                btnViewData.Background = new SolidColorBrush(Colors.Transparent);
                btnViewData.BorderThickness = new Thickness(0);
                btnViewData.FontWeight = FontWeights.Normal;
                btnViewData.FontSize = 12;

                PackIcon closeIcon = new PackIcon
                {
                    Kind = PackIconKind.Close,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                //closeIcon.Foreground = Brushes.Red;

                Button btnViewClose = new Button();
                btnViewClose.Content = closeIcon;
                btnViewClose.Background = new SolidColorBrush(Colors.Transparent);
                btnViewClose.BorderThickness = new Thickness(0);
                btnViewClose.FontWeight = FontWeights.Normal;


                PackIcon playIcon = new PackIcon
                {
                    Kind = PackIconKind.Play,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00cdac"))

                };

                Button btnPlayMqtt = new Button();
                btnPlayMqtt.Content = playIcon;
                btnPlayMqtt.Background = new SolidColorBrush(Colors.Transparent);
                btnPlayMqtt.BorderThickness = new Thickness(0);
                btnPlayMqtt.FontWeight = FontWeights.Normal;


                PackIcon pauseIcon = new PackIcon
                {
                    Kind = PackIconKind.Pause,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffbf65"))

                };

                Button btnPauseMqtt = new Button();
                btnPauseMqtt.Content = pauseIcon;
                btnPauseMqtt.Background = new SolidColorBrush(Colors.Transparent);
                btnPauseMqtt.BorderThickness = new Thickness(0);
                btnPauseMqtt.FontWeight = FontWeights.Normal;



                contentPanel.Children.Add(btnViewData);
                contentPanel.Children.Add(btnPlayMqtt);
                contentPanel.Children.Add(btnPauseMqtt);
                contentPanel.Children.Add(btnViewClose);

                UpdateContentPanelBackground(contentPanel, isSelected: true);

                foreach (StackPanel existingPanel in stackCurrentData.Children.OfType<StackPanel>().Where(panel => panel != contentPanel))
                {
                    UpdateContentPanelBackground(existingPanel, isSelected: false);
                }
                stackCurrentData.Children.Add(contentPanel);

                contentPanel.Tag = GetContentPanelKey(selectedDistrict, selectedEsd);

                ViewDataButtonInfo buttonInfo = new ViewDataButtonInfo
                {
                    District = selectedDistrict,
                    ESD = selectedEsd,
                };

                btnViewData.Tag = buttonInfo;
                btnViewClose.Tag = buttonInfo;
                btnPlayMqtt.Tag = buttonInfo;
                btnPauseMqtt.Tag = buttonInfo;



                btnViewData.Click += BtnViewData_Click;
                btnViewClose.Click += BtnViewClose_Click;
                btnPlayMqtt.Click += BtnPlayMqtt_Click;
                btnPauseMqtt.Click += BtnPauseMqtt_Click;

                selectedContentPanel = contentPanel;

                GetInfoForId();

            }
        }

        private async void GetInfoForId()
        {
            try
            {
                // Make a single request to get all data
                string apiUrl = "http://103.234.126.43:3080/dtmeter/info/esd_essd_info/all";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON response to a list of ApiResponseModel
                        var apiResponseList = JsonConvert.DeserializeObject<List<MqttClass>>(jsonResult);

                        if (apiResponseList != null && apiResponseList.Any())
                        {
                            // After storing the information, make requests for  data
                            GetInstantAllDataForStoredInfo(apiResponseList);
                        }
                        else
                        {
                            // Handle case where apiResponseList is null or empty
                            MessageBox.Show("API response does not contain valid data.");
                        }
                    }
                    else
                    {
                        // Handle unsuccessful response
                        MessageBox.Show($"Failed to get all info. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show($"Exception while getting all info: {ex.Message}");
            }
        }

        private List<MqttClass> accumulatedInstantAllData = new List<MqttClass>();

        private async void GetInstantAllDataForStoredInfo(List<MqttClass> apiResponseList)
        {
            datagridInstantPara1.ClearFilters();
            try
            {
                foreach (var dtId in storedItems)
                {
                    var matchingItem = apiResponseList.FirstOrDefault(item => item.dt_id == dtId);

                    if (matchingItem != null)
                    {
                        // Extract district_code and esd_code from the matching item
                        string districtCode = matchingItem.district_code;
                        string esdCode = matchingItem.esd_code;

                        // Process the extracted values or update your UI
                        //  MessageBox.Show($"Info for {dtId}: District Code - {districtCode}, ESD Code - {esdCode}");

                        // Now make a request for  data
                        await GetInstantAllDataForStoredInfo(districtCode, dtId);
                    }
                    else
                    {
                        // Handle case where dt_id is not found
                        MessageBox.Show($"Info for {dtId} not found.");
                    }
                }

                // After processing all items, set the accumulated data to the datagrid
                datagridInstantPara1.ItemsSource = accumulatedInstantAllData;

                allFetchedData = accumulatedInstantAllData;
                SaveDataToTemporaryStorage(selectedDistrict, selectedEsd, accumulatedInstantAllData);
                //btnStartPolling.IsEnabled = true;
                //btnStopPolling.IsEnabled = true;

            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show($"Exception while processing data: {ex.Message}");
            }
        }


        private async Task GetInstantAllDataForStoredInfo(string districtCode, string dtId)
        {
            try
            {
                string apiUrl = $"http://103.234.126.43:3080/dtmeter/dtinfo/all/{districtCode}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON response to your  model
                        List<MqttClass> InstantAllData = JsonConvert.DeserializeObject<List<MqttClass>>(jsonResult);

                        if (InstantAllData != null)
                        {
                            // Filter the Data for the specific dt_id
                            var filteredData = InstantAllData.Where(item => item.dt_id == dtId).ToList();

                          

                            if (filteredData.Any())
                            {
                                // Append the filtered  data to the accumulated data
                                accumulatedInstantAllData.AddRange(filteredData);

                            }
                            else
                            {
                                // Handle case where no data is found for the dt_id
                                MessageBox.Show($"No  data found for {dtId}.");
                            }
                        }
                        else
                        {
                            // Handle case where Data is null
                            MessageBox.Show($"Data for {dtId} is null or has unexpected structure.");
                        }
                    }
                    else
                    {
                        // Handle unsuccessful response
                        MessageBox.Show($"Failed to get data for {dtId}. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show($"Exception while getting  data for {dtId}: {ex.Message}");
            }
        }


        #endregion from gis

        #region add tree view items
        private async void loadData()
        {
            try
            {
                // Show the progress bar
                Dispatcher.Invoke(() => progressLogin.Visibility = Visibility.Visible);

                using (WebClient web = new WebClient())
                {
                    string url = string.Format("http://103.234.126.43:3080/dtmeters/district");
                    string json = await web.DownloadStringTaskAsync(url);

                    List<District> districts = JsonConvert.DeserializeObject<List<District>>(json);

                    // Update UI on the UI thread
                    Dispatcher.Invoke(() =>
                    {
                        foreach (District district in districts)
                        {
                            TreeViewItem districtItem = new TreeViewItem();
                            districtItem.Header = district.district_name;
                            districtItem.Tag = district.district_code;
                            url = string.Format("http://103.234.126.43:3080/dtmeter/district/{0}", district.district_code);
                            json = web.DownloadString(url);
                            List<ESD> esds = JsonConvert.DeserializeObject<List<ESD>>(json);

                            foreach (ESD esd in esds)
                            {
                                TreeViewItem esdItem = new TreeViewItem();
                                esdItem.Header = esd.esd_name;
                                esdItem.Tag = esd.esd_code;
                                districtItem.Items.Add(esdItem);
                            }

                            myTreeView.Items.Add(districtItem);
                        }
                    });
                }
            }
            catch
            {
                // Update UI on the UI thread
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                // Hide the progress bar
                Dispatcher.Invoke(() => progressLogin.Visibility = Visibility.Collapsed);
            }
        }

        string buttonText;
        string key;
        StackPanel existingContentPanel = null;

        public class ViewDataButtonInfo
        {
            public string District { get; set; }
            public string ESD { get; set; }
            
        }

        private StackPanel selectedContentPanel = null;

        private void UpdateContentPanelBackground(StackPanel contentPanel, bool isSelected)
        {
            contentPanel.Background = new SolidColorBrush(isSelected ? SelectedBackgroundColor : DefaultBackgroundColor);
        }

        private readonly Color DefaultBackgroundColor = (Color)ColorConverter.ConvertFromString("#2d2d30");
        private readonly Color SelectedBackgroundColor = (Color)ColorConverter.ConvertFromString("#00a5e3");

        private string GetContentPanelKey(string district, string esd)
        {
            return $"{district}_{esd}";
        }

        private void SaveDataToTemporaryStorage(string district, string esd, List<MqttClass> data)
        {
            string key = $"{district}_{esd}";

            temporaryStorage[key] = data;
        }

        private void BtnViewData_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnViewData && btnViewData.Tag is ViewDataButtonInfo buttonInfo)
            {
                //MessageBox.Show($"Button Clicked: {buttonInfo.District}, {buttonInfo.ESD}, {buttonInfo.StartDate}, {buttonInfo.EndDate}");

                List<MqttClass> temporaryData = GetTemporaryData(buttonInfo.District, buttonInfo.ESD);

                if (temporaryData != null)
                {
                    //MessageBox.Show($"Fetched Data Count: {temporaryData.Count}");
                    datagridInstantPara1.ItemsSource = temporaryData;
                }
                else
                {
                    MessageBox.Show("Temporary data is null or empty.");
                }

                selectedDistrict = buttonInfo.District;
                selectedEsd = buttonInfo.ESD;


                selectedContentPanel = stackCurrentData.Children.OfType<StackPanel>()
            .FirstOrDefault(panel => panel.Children.OfType<Button>().FirstOrDefault(b => b == btnViewData) != null);

                foreach (StackPanel existingPanel in stackCurrentData.Children.OfType<StackPanel>().Where(panel => panel != selectedContentPanel))
                {
                    UpdateContentPanelBackground(existingPanel, isSelected: false);
                }

                if (selectedContentPanel != null)
                {
                    UpdateContentPanelBackground(selectedContentPanel, isSelected: true);
                }
            }
          }



        private List<MqttClass> GetTemporaryData(string district, string esd)
        {
            string key = $"{district}_{esd}";

            if (temporaryStorage.TryGetValue(key, out List<MqttClass> temporaryData))
            {
                return temporaryData;
            }
            else
            {
                return new List<MqttClass>();
            }
        }

        private Dictionary<string, List<MqttClass>> temporaryStorage = new Dictionary<string, List<MqttClass>>();

        private void BtnViewClose_Click(object sender, RoutedEventArgs e)
        {

            if (sender is Button btnViewClose)
            {
                StackPanel contentPanel = btnViewClose.Parent as StackPanel;

                if (contentPanel != null)
                {
                    if (contentPanel.Children[0] is Button btnViewData && btnViewData.Tag is ViewDataButtonInfo buttonInfo)
                    {
                        stackCurrentData.Children.Remove(contentPanel);

                        RemoveDataFromTemporaryStorage(buttonInfo.District, buttonInfo.ESD);

                        RefreshDataGrid();
                    }
                }

                // Update the background color of the remaining content panels
                foreach (StackPanel existingPanel in stackCurrentData.Children.OfType<StackPanel>().Where(panel => panel != contentPanel))
                {
                    UpdateContentPanelBackground(existingPanel, isSelected: false);
                }

                // Update the background color of the first remaining content panel
                if (stackCurrentData.Children.Count > 0 && stackCurrentData.Children[0] is StackPanel remainingContentPanel)
                {
                    UpdateContentPanelBackground(remainingContentPanel, isSelected: true);
                    selectedContentPanel = remainingContentPanel;
                }
            }
        }

        private void RemoveDataFromTemporaryStorage(string district, string esd)
        {
            string key = $"{district}_{esd}";
            temporaryStorage.Remove(key);
        }

        private void RefreshDataGrid()
        {
            if (stackCurrentData.Children.Count > 0 && stackCurrentData.Children[0] is StackPanel firstContentPanel)
            {
                if (firstContentPanel.Children[0] is Button firstBtnViewData && firstBtnViewData.Tag is ViewDataButtonInfo firstButtonInfo)
                {
                    List<MqttClass> temporaryData = GetTemporaryData(firstButtonInfo.District, firstButtonInfo.ESD);

                    datagridInstantPara1.ItemsSource = temporaryData;
                }
            }
        }

        private async void myTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            cbNationData.IsChecked = false;

            progressLogin.Visibility = Visibility.Visible;
            if (e.NewValue == null || !(e.NewValue is TreeViewItem selectedTreeViewItem))
            {
                return;
            }

            try
            {
                //StackPanel existingContentPanel = null;

                if (selectedTreeViewItem.Parent is TreeViewItem selectedDistrictItem && selectedDistrictItem.Tag != null && selectedTreeViewItem.Tag != null)
                {
                    selectedDistrict = (string)selectedDistrictItem.Tag;
                    selectedEsd = (string)selectedTreeViewItem.Tag;
                    selectedPlaceForData = "DataEsd";

                    buttonText = (string)selectedTreeViewItem.Header;
                    key = $"{selectedDistrict}_{selectedEsd}";

                    existingContentPanel = stackCurrentData.Children.OfType<StackPanel>().FirstOrDefault(panel => panel.Tag != null && (string)panel.Tag == key);

                    url = string.Format("http://103.234.126.43:3080/dtmeter/district/{0}/{1}", selectedDistrictItem.Tag, selectedTreeViewItem.Tag);


                }

                else if (e.NewValue is TreeViewItem districtItem)
                {
                    selectedDistrict = (string)districtItem.Tag;
                    selectedPlaceForData = "DataDistrict";
                    selectedEsd = selectedDistrict + " Multiple ESD";

                    buttonText = (string)districtItem.Header;
                    key = $"{selectedDistrict}_{selectedEsd}";

                    existingContentPanel = stackCurrentData.Children.OfType<StackPanel>().FirstOrDefault(panel => panel.Tag != null && (string)panel.Tag == key);


                    url = string.Format("http://103.234.126.43:3080/dtmeter/dtinfo/all/{0}", districtItem.Tag);

                }

                if (existingContentPanel != null)
                {
                    BtnViewData_Click(existingContentPanel.Children.OfType<Button>().FirstOrDefault(), new RoutedEventArgs(Button.ClickEvent));
                }
                else
                {
                    await ReadDatabase();

                    StackPanel contentPanel = new StackPanel();
                    contentPanel.Orientation = Orientation.Horizontal;
                    contentPanel.Margin = new Thickness(0, 0, 1, 0);
                    contentPanel.Background = new SolidColorBrush(DefaultBackgroundColor);


                    Button btnViewData = new Button();
                    btnViewData.Content = buttonText;
                    btnViewData.Foreground = Brushes.White;
                    btnViewData.Background = new SolidColorBrush(Colors.Transparent);
                    btnViewData.BorderThickness = new Thickness(0);
                    btnViewData.FontWeight = FontWeights.Normal;
                    btnViewData.FontSize = 12;

                    PackIcon closeIcon = new PackIcon
                    {
                        Kind = PackIconKind.Close,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    //closeIcon.Foreground = Brushes.Red;

                    Button btnViewClose = new Button();
                    btnViewClose.Content = closeIcon;
                    btnViewClose.Background = new SolidColorBrush(Colors.Transparent);
                    btnViewClose.BorderThickness = new Thickness(0);
                    btnViewClose.FontWeight = FontWeights.Normal;


                    PackIcon playIcon = new PackIcon
                    {
                        Kind = PackIconKind.Play,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00cdac"))

                    };

                    Button btnPlayMqtt = new Button();
                    btnPlayMqtt.Content = playIcon;
                    btnPlayMqtt.Background = new SolidColorBrush(Colors.Transparent);
                    btnPlayMqtt.BorderThickness = new Thickness(0);
                    btnPlayMqtt.FontWeight = FontWeights.Normal;


                    PackIcon pauseIcon = new PackIcon
                    {
                        Kind = PackIconKind.Pause,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffbf65"))

                    };

                    Button btnPauseMqtt = new Button();
                    btnPauseMqtt.Content = pauseIcon;
                    btnPauseMqtt.Background = new SolidColorBrush(Colors.Transparent);
                    btnPauseMqtt.BorderThickness = new Thickness(0);
                    btnPauseMqtt.FontWeight = FontWeights.Normal;



                    contentPanel.Children.Add(btnViewData);
                    contentPanel.Children.Add(btnPlayMqtt);
                    contentPanel.Children.Add(btnPauseMqtt);
                    contentPanel.Children.Add(btnViewClose);

                    UpdateContentPanelBackground(contentPanel, isSelected: true);

                    foreach (StackPanel existingPanel in stackCurrentData.Children.OfType<StackPanel>().Where(panel => panel != contentPanel))
                    {
                        UpdateContentPanelBackground(existingPanel, isSelected: false);
                    }
                    stackCurrentData.Children.Add(contentPanel);

                    contentPanel.Tag = GetContentPanelKey(selectedDistrict, selectedEsd);

                    ViewDataButtonInfo buttonInfo = new ViewDataButtonInfo
                    {
                        District = selectedDistrict,
                        ESD = selectedEsd,
                    };

                    btnViewData.Tag = buttonInfo;
                    btnViewClose.Tag = buttonInfo;
                    btnPlayMqtt.Tag = buttonInfo;
                    btnPauseMqtt.Tag = buttonInfo;



                    btnViewData.Click += BtnViewData_Click;
                    btnViewClose.Click += BtnViewClose_Click;
                    btnPlayMqtt.Click += BtnPlayMqtt_Click;
                    btnPauseMqtt.Click += BtnPauseMqtt_Click;

                    selectedContentPanel = contentPanel;

                }
            }

            catch (Exception ex)
            {
                //string errorMessage = "An error occurred: " + ex.Message;
                //MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                MessageBox.Show("Please insert the date.", "Date Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            finally
            {
                progressLogin.Visibility = Visibility.Collapsed;
            }
        }

        private async Task ReadDatabase()
        {
            datagridInstantPara1.ClearFilters();

            try
            {
                // Show progress bar
                Dispatcher.Invoke(() => progressLogin.Visibility = Visibility.Visible);

                using (WebClient web = new WebClient())
                {

                    //string url = string.Format("http://103.234.126.43:3080/dtmeter/dtinfo/all/{0}", selectedDistrict);
                    string url = "http://103.234.126.43:3080/dtmeter/info/esd_essd_info/all";

                    string json = await web.DownloadStringTaskAsync(url);

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        MessageBox.Show("No data found from the server.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    List<MqttClass> allData = JsonConvert.DeserializeObject<List<MqttClass>>(json);

                    if (allData == null)
                    {
                        MessageBox.Show("Error parsing JSON data. Deserialized data is null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }


                    if (selectedPlaceForData == "DataDistrict")
                    {
                        allFetchedData = allData.Where(a => a?.district_code == selectedDistrict).ToList();
                        SaveDataToTemporaryStorage(selectedDistrict, selectedEsd, allFetchedData);


                        _ = Dispatcher.InvokeAsync(() =>
                        {
                            datagridInstantPara1.ItemsSource = allFetchedData;
                        });
                    }

                    else if (selectedPlaceForData == "DataEsd")
                    {
                        allFetchedData = allData.Where(a => a?.esd_code == selectedEsd).ToList();
                        SaveDataToTemporaryStorage(selectedDistrict, selectedEsd, allFetchedData);



                        _ = Dispatcher.InvokeAsync(() =>
                        {
                            datagridInstantPara1.ItemsSource = allFetchedData;
                        });
                    }

                    //btnStartPolling.IsEnabled = true;
                    //btnStopPolling.IsEnabled = true;
                }
            }
            catch
            {
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);

                //btnStartPolling.IsEnabled = false;
                //btnStopPolling.IsEnabled = false;
            }
            finally
            {
                // Hide progress bar
                Dispatcher.Invoke(() => progressLogin.Visibility = Visibility.Collapsed);
            }
        }


     

        public class SelectedItemInfo
        {
            public string District { get; set; }
            public string ESD { get; set; }
            public string PlaceForData { get; set; }
        }

        // Create a list to store selected items

        #endregion add tree view items


       

        #region mqttClient 


        private Dictionary<Button, bool> buttonSubscriptionStatus = new Dictionary<Button, bool>();


        private void btnConnectMqtt_Click(object sender, RoutedEventArgs e)
        {
            ConnectToBroker(ref mqttClient1, mqttBrokerAddress1, "Connected to MQTT Server 1");
            ConnectToBroker(ref mqttClient2, mqttBrokerAddress2, "Connected to MQTT Server 2");
            ConnectToBroker(ref mqttClient3, mqttBrokerAddress3, "Connected to MQTT Server 3");

            // Update the UI once all connections are attempted
            btnConnectMqtt.Visibility = Visibility.Collapsed;
            btnDisConnectConnectMqtt.Visibility = Visibility.Visible;

           
        }

        private void ConnectToBroker(ref MqttClient mqttClient, string brokerAddress, string eventMessage)
        {
            if (mqttClient == null || !mqttClient.IsConnected)
            {
                if (mqttClient == null)
                {
                    mqttClient = new MqttClient(brokerAddress);
                    mqttClient.MqttMsgPublishReceived += MqttMsgPublishReceived;

                    if(cbNationData.IsChecked == true)
                    {
                        mqttClient.MqttMsgPublishReceived += MqttMsgPublishReceivedCountry;

                    }


                }

                try
                {
                    mqttClient.Connect(Guid.NewGuid().ToString());

                    lblConnStatus.Content = eventMessage;
                    string hexColor = "#00cdac";
                    Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);
                    lblConnStatus.Foreground = brush;


                    timer = new System.Timers.Timer();
                    timer.Interval = 240000; // 2 minutes
                    timer.Elapsed += Timer_Elapsed;
                    timer.AutoReset = false; // only fire once
                }
                catch (Exception ex)
                {
                    // Handle connection errors
                    MessageBox.Show($"Error connecting to MQTT Server at {brokerAddress}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Optional: Provide a message indicating that the client is already connected
                MessageBox.Show($"MQTT client is already connected to {brokerAddress}.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDisConnectConnectMqtt_Click(object sender, RoutedEventArgs e)
        {
            DisconnectBroker(ref mqttClient1, "Disconnected from MQTT Server 1");
            DisconnectBroker(ref mqttClient2, "Disconnected from MQTT Server 2");
            DisconnectBroker(ref mqttClient3, "Disconnected from MQTT Server 3");

            // Reset connection status and update the UI

            if (timer != null)
            {
                timer.Stop();
            }

            successNumber = 0;
            unsuccessNumber = 0;

            lblSuccessfulPolls.Content = successNumber;
            lblFailedPolls.Content = unsuccessNumber;
            lblTotalPolls.Content = successNumber + unsuccessNumber;


            lblConnStatus.Content = "Disconnected from MQTT Servers";
            string hexColor = "#ff5768";
            Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);
            lblConnStatus.Foreground = brush;

            btnConnectMqtt.Visibility = Visibility.Visible;
            btnDisConnectConnectMqtt.Visibility = Visibility.Collapsed;


           
        }

        private void DisconnectBroker(ref MqttClient mqttClient, string eventMessage)
        {
            if (mqttClient != null && mqttClient.IsConnected)
            {
                mqttClient.Disconnect();

               
            }
        }


        private Dictionary<Button, List<string>> buttonSubscribedTopics = new Dictionary<Button, List<string>>();
        private Dictionary<Button, List<MqttClass>> buttonTemporaryData = new Dictionary<Button, List<MqttClass>>();

        private Dictionary<string, string[]> temporaryDataDictionary = new Dictionary<string, string[]>(); // Dictionary to store temporary data for each MQTT topic


        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                unsuccessNumber++;
                lblSuccessfulPolls.Content = successNumber;
                lblFailedPolls.Content = unsuccessNumber;
                lblTotalPolls.Content = successNumber + unsuccessNumber;
            });
        }

        private void BtnPlayMqtt_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnPlayMqtt && btnPlayMqtt.Tag is ViewDataButtonInfo buttonInfo)
            {
                // Check if at least one MQTT client is connected
                if ((mqttClient1 != null && mqttClient1.IsConnected) ||
                    (mqttClient2 != null && mqttClient2.IsConnected) ||
                    (mqttClient3 != null && mqttClient3.IsConnected))
                {
                    List<MqttClass> temporaryData = GetTemporaryData(buttonInfo.District, buttonInfo.ESD);

                    if (temporaryData != null)
                    {
                        if (!buttonTemporaryData.ContainsKey(btnPlayMqtt))
                        {
                            buttonTemporaryData.Add(btnPlayMqtt, temporaryData);

                            List<string> mqttTopics = temporaryData
                                .Where(data => !string.IsNullOrEmpty(data.instantaneous))
                                .Select(data => data.instantaneous)
                                .ToList();

                            if (mqttTopics.Any())
                            {
                                buttonSubscribedTopics.Add(btnPlayMqtt, mqttTopics);

                                // Subscribe to topics on each connected MQTT broker
                                SubscribeToTopicsOnBrokers(btnPlayMqtt, mqttTopics);

                                
                            }
                            else
                            {
                                MessageBox.Show("All 'instantaneous' values are empty. No data to display.");
                            }
                        }
                        else
                        {
                            buttonTemporaryData[btnPlayMqtt] = temporaryData;

                            if (!buttonSubscribedTopics.ContainsKey(btnPlayMqtt))
                            {
                                List<string> mqttTopics = temporaryData
                                    .Where(data => !string.IsNullOrEmpty(data.instantaneous))
                                    .Select(data => data.instantaneous)
                                    .ToList();

                                if (mqttTopics.Any())
                                {
                                    buttonSubscribedTopics.Add(btnPlayMqtt, mqttTopics);
                                    SubscribeToTopicsOnBrokers(btnPlayMqtt, mqttTopics);
                                }
                                else
                                {
                                    MessageBox.Show("All 'instantaneous' values are empty. No data to display.");
                                }
                            }
                        }

                        InitializeMqtt(btnPlayMqtt);
                    }
                    else
                    {
                        MessageBox.Show("Temporary data is null or empty.");
                    }
                }
                else
                {
                    // Display an error message if no MQTT client is connected
                    MessageBox.Show("No MQTT client is connected. Please connect at least one broker before playing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SubscribeToTopicsOnBrokers(Button btnPlayMqtt, List<string> mqttTopics)
        {
            // Subscribe to topics on Broker 1
            if (mqttClient1 != null && mqttClient1.IsConnected)
            {
                foreach (string topic in mqttTopics)
                {
                    mqttClient1.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                }
            }

            // Subscribe to topics on Broker 2
            if (mqttClient2 != null && mqttClient2.IsConnected)
            {
                foreach (string topic in mqttTopics)
                {
                    mqttClient2.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                }
            }

            // Subscribe to topics on Broker 3
            if (mqttClient3 != null && mqttClient3.IsConnected)
            {
                foreach (string topic in mqttTopics)
                {
                    mqttClient3.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                }
            }
        }


        private void InitializeMqtt(Button button)
        {
            if (buttonSubscribedTopics.TryGetValue(button, out List<string> subscribedTopics))
            {
                // Subscribe to topics on Broker 1
                if (mqttClient1 != null && mqttClient1.IsConnected)
                {
                    mqttClient1.Subscribe(subscribedTopics.ToArray(), Enumerable.Repeat(MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, subscribedTopics.Count).ToArray());
                }

                // Subscribe to topics on Broker 2
                if (mqttClient2 != null && mqttClient2.IsConnected)
                {
                    mqttClient2.Subscribe(subscribedTopics.ToArray(), Enumerable.Repeat(MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, subscribedTopics.Count).ToArray());
                }

                // Subscribe to topics on Broker 3
                if (mqttClient3 != null && mqttClient3.IsConnected)
                {
                    mqttClient3.Subscribe(subscribedTopics.ToArray(), Enumerable.Repeat(MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, subscribedTopics.Count).ToArray());
                }
            }
            else
            {
                MessageBox.Show("Button not found in the subscribed topics dictionary.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string mqttTopic = e.Topic;
            string message = Encoding.UTF8.GetString(e.Message);

            MqttClass dt = null;

            // Iterate through all buttons and their associated temporaryData lists
            foreach (var buttonDataPair in buttonTemporaryData)
            {
                List<MqttClass> temporaryData = buttonDataPair.Value;

                // Find the appropriate MqttClass instance based on the topic
                dt = temporaryData.FirstOrDefault(data => data.instantaneous == mqttTopic);

                if (dt != null)
                {
                    if (!string.IsNullOrEmpty(message)) // Check if the message is not empty
                    {
                        string[] messageParts = message.Split(',');

                        // Count the number of occurrences of "NC" in the message
                        int messageCount = messageParts.Length;

                        int ncCount = messageParts.Count(part => part.Trim() == "NC");

                        if (messageCount == 44)
                        {
                            AssignProperties(dt, messageParts);
                        }

                        else if (messageCount == 42)
                        {
                            AssignPropertiesEarlierV(dt, messageParts);
                        }
                        else if (messageCount == 18)
                        {
                            AssignPropertiesForShortMessage(dt, messageParts);
                        }

                        else
                        {
                            AssignNCProperties(dt);
                        }

                        // Update UI and other operations
                        Dispatcher.Invoke(() =>
                        {
                            datagridInstantPara1.UpdateLayout();

                            lblSuccessfulPolls.Content = ++successNumber;
                            lblTotalPolls.Content = successNumber + unsuccessNumber;

                        });

                        timer.Stop();
                        timer.Start();
                    }
                }


                // Define variables to hold the counts
                int column4Count = 0; // dt_meter_serial_no_mqtt
                int column8Count = 0; // phase_a_instantaneous_current_a

                int ncDataCount = 0;
                double percentReporting = 0;
                // Regular expression to match numeric strings
                Regex numericRegex = new Regex(@"^\d+$");

                // Iterate through the rows in the data grid
        



                foreach (var item in datagridInstantPara1.View.Records)
                {
                    if (item != null)
                    {
                        var rowData = item.Data as MqttClass; // Replace MqttClass with the actual type of your data item

                        if (rowData != null)
                        {
                            if (rowData.dt_meter_serial_no_mqtt != null && numericRegex.IsMatch(rowData.dt_meter_serial_no_mqtt.ToString()))
                            {
                                column4Count++;
                            }

                            if (!string.IsNullOrEmpty(rowData.phase_a_instantaneous_current_a?.ToString()))
                            {
                                column8Count++;
                            }
                        }
                    }
                }



                if (column8Count > 0)
                {
                    percentReporting = (double)column4Count / column8Count * 100.0;
                }
                else
                {
                    percentReporting = 0.0;
                }

                // Update UI elements on the UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ncDataCount = column8Count - column4Count;

                    // Update label content with the counts
                    lblValidDataCount.Content = column4Count;
                    lblTotalDataCount.Content = column8Count;
                    lblTotalNCCount.Content = ncDataCount;
                    lblPercentReporting.Content = percentReporting.ToString("0.00");
                });



            }

            if (dt == null)
            {
                // Log or handle the case when no matching MqttClass instance is found
                // For now, showing a MessageBox with an error message


                //Dispatcher.Invoke(() =>
                //{
                //    MessageBox.Show($"No matching MqttClass instance found for topic: {mqttTopic}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //});

            }
        }

        private void AssignProperties(MqttClass dt, string[] messageParts)
        {
            string DateDisplay = DateTime.Now.ToString("dd/MM/yyyy");
            string TimeDisplay = DateTime.Now.ToString("hh:mm:ss tt");

            dt.dt_meter_serial_no_mqtt = GetValueOrDefault(messageParts, 1);
            dt.dt_meter_serial_no = GetValueOrDefault(messageParts, 1);
            dt.mri_serial_no = GetValueOrDefault(messageParts, 2);
            dt.phase_a_instantaneous_current_a = GetValueOrDefault(messageParts, 3);
            dt.phase_b_instantaneous_current_a = GetValueOrDefault(messageParts, 4);
            dt.phase_c_instantaneous_current_a = GetValueOrDefault(messageParts, 5);

            dt.phase_a_instantaneous_voltage_v = GetValueOrDefault(messageParts, 6);
            dt.phase_b_instantaneous_voltage_v = GetValueOrDefault(messageParts, 7);
            dt.phase_c_instantaneous_voltage_v = GetValueOrDefault(messageParts, 8);

            dt.instantaneous_power_factor = GetValueOrDefault(messageParts, 9);

            dt.instantaneous_import_active_power_kw = GetValueOrDefault(messageParts, 10);
            dt.instantaneous_export_active_power_kw = GetValueOrDefault(messageParts, 11);
            dt.instantaneous_import_reactive_power_kvar = GetValueOrDefault(messageParts, 12);
            dt.instantaneous_export_reactive_power_kvar = GetValueOrDefault(messageParts, 13);
            dt.instantaneous_import_apparent_power_kva = GetValueOrDefault(messageParts, 14);
            dt.instantaneous_export_apparent_power_kva = GetValueOrDefault(messageParts, 15);

            dt.instantaneous_total_active_power_kw = GetValueOrDefault(messageParts, 16);
            dt.total_reactive_power_kvar = GetValueOrDefault(messageParts, 17);
            dt.total_apparent_power_kva = GetValueOrDefault(messageParts, 18);

            dt.instantaneous_net_active_power_kw = GetValueOrDefault(messageParts, 19);

            dt.phase_a_instantaneous_import_active_power_kw = GetValueOrDefault(messageParts, 20);
            dt.phase_a_instantaneous_export_active_power_kw = GetValueOrDefault(messageParts, 21);
            dt.phase_a_instantaneous_import_reactive_power_kvar = GetValueOrDefault(messageParts, 22);
            dt.phase_a_instantaneous_export_reactive_power_kvar = GetValueOrDefault(messageParts, 23);
            dt.phase_a_instantaneous_import_apparent_power_kva = GetValueOrDefault(messageParts, 24);
            dt.phase_a_instantaneous_export_apparent_power_kva = GetValueOrDefault(messageParts, 25);
            dt.phase_b_instantaneous_import_active_power_kw = GetValueOrDefault(messageParts, 26);
            dt.phase_b_instantaneous_export_active_power_kw = GetValueOrDefault(messageParts, 27);
            dt.phase_b_instantaneous_import_reactive_power_kvar = GetValueOrDefault(messageParts, 28);
            dt.phase_b_instantaneous_export_reactive_power_kvar = GetValueOrDefault(messageParts, 39);
            dt.phase_b_instantaneous_import_apparent_power_kva = GetValueOrDefault(messageParts, 30);
            dt.phase_b_instantaneous_export_apparent_power_kva = GetValueOrDefault(messageParts, 31);
            dt.phase_c_instantaneous_import_active_power_kw = GetValueOrDefault(messageParts, 32);
            dt.phase_c_instantaneous_export_active_power_kw = GetValueOrDefault(messageParts, 33);
            dt.phase_c_instantaneous_import_reactive_power_kvar = GetValueOrDefault(messageParts, 34);
            dt.phase_c_instantaneous_export_reactive_power_kvar = GetValueOrDefault(messageParts, 35);
            dt.phase_c_instantaneous_import_apparent_power_kva = GetValueOrDefault(messageParts, 36);
            dt.phase_c_instantaneous_export_apparent_power_kva = GetValueOrDefault(messageParts, 37);

            dt.active_energy = GetValueOrDefault(messageParts, 38);
            dt.activeEnergyH1 = GetValueOrDefault(messageParts, 39);
            dt.activeEnergyH2 = GetValueOrDefault(messageParts, 40);

            dt.firmware_version = GetValueOrDefault(messageParts, 41);
            dt.date = GetValueOrDefault(messageParts, 42);
            dt.time = GetValueOrDefault(messageParts, 43);

            dt.phase = "3 Phase";

        }

        private void AssignPropertiesEarlierV(MqttClass dt, string[] messageParts)
        {
            string DateDisplay = DateTime.Now.ToString("dd-MM-yyyy");
            string TimeDisplay = DateTime.Now.ToString("HH:mm:ss");

            dt.dt_meter_serial_no_mqtt = GetValueOrDefault(messageParts, 1);
            dt.dt_meter_serial_no = GetValueOrDefault(messageParts, 1);
            dt.mri_serial_no = GetValueOrDefault(messageParts, 2);
            dt.phase_a_instantaneous_current_a = GetValueOrDefault(messageParts, 3);
            dt.phase_b_instantaneous_current_a = GetValueOrDefault(messageParts, 4);
            dt.phase_c_instantaneous_current_a = GetValueOrDefault(messageParts, 5);

            dt.phase_a_instantaneous_voltage_v = GetValueOrDefault(messageParts, 6);
            dt.phase_b_instantaneous_voltage_v = GetValueOrDefault(messageParts, 7);
            dt.phase_c_instantaneous_voltage_v = GetValueOrDefault(messageParts, 8);

            dt.instantaneous_power_factor = GetValueOrDefault(messageParts, 9);

            dt.instantaneous_import_active_power_kw = GetValueOrDefault(messageParts, 10);
            dt.instantaneous_export_active_power_kw = GetValueOrDefault(messageParts, 11);
            dt.instantaneous_import_reactive_power_kvar = GetValueOrDefault(messageParts, 12);
            dt.instantaneous_export_reactive_power_kvar = GetValueOrDefault(messageParts, 13);
            dt.instantaneous_import_apparent_power_kva = GetValueOrDefault(messageParts, 14);
            dt.instantaneous_export_apparent_power_kva = GetValueOrDefault(messageParts, 15);

            dt.instantaneous_total_active_power_kw = GetValueOrDefault(messageParts, 16);
            dt.total_reactive_power_kvar = GetValueOrDefault(messageParts, 17);
            dt.total_apparent_power_kva = GetValueOrDefault(messageParts, 18);

            dt.instantaneous_net_active_power_kw = GetValueOrDefault(messageParts, 19);

            dt.phase_a_instantaneous_import_active_power_kw = GetValueOrDefault(messageParts, 20);
            dt.phase_a_instantaneous_export_active_power_kw = GetValueOrDefault(messageParts, 21);
            dt.phase_a_instantaneous_import_reactive_power_kvar = GetValueOrDefault(messageParts, 22);
            dt.phase_a_instantaneous_export_reactive_power_kvar = GetValueOrDefault(messageParts, 23);
            dt.phase_a_instantaneous_import_apparent_power_kva = GetValueOrDefault(messageParts, 24);
            dt.phase_a_instantaneous_export_apparent_power_kva = GetValueOrDefault(messageParts, 25);
            dt.phase_b_instantaneous_import_active_power_kw = GetValueOrDefault(messageParts, 26);
            dt.phase_b_instantaneous_export_active_power_kw = GetValueOrDefault(messageParts, 27);
            dt.phase_b_instantaneous_import_reactive_power_kvar = GetValueOrDefault(messageParts, 28);
            dt.phase_b_instantaneous_export_reactive_power_kvar = GetValueOrDefault(messageParts, 39);
            dt.phase_b_instantaneous_import_apparent_power_kva = GetValueOrDefault(messageParts, 30);
            dt.phase_b_instantaneous_export_apparent_power_kva = GetValueOrDefault(messageParts, 31);
            dt.phase_c_instantaneous_import_active_power_kw = GetValueOrDefault(messageParts, 32);
            dt.phase_c_instantaneous_export_active_power_kw = GetValueOrDefault(messageParts, 33);
            dt.phase_c_instantaneous_import_reactive_power_kvar = GetValueOrDefault(messageParts, 34);
            dt.phase_c_instantaneous_export_reactive_power_kvar = GetValueOrDefault(messageParts, 35);
            dt.phase_c_instantaneous_import_apparent_power_kva = GetValueOrDefault(messageParts, 36);
            dt.phase_c_instantaneous_export_apparent_power_kva = GetValueOrDefault(messageParts, 37);

            dt.active_energy = GetValueOrDefault(messageParts, 38);
            dt.activeEnergyH1 = GetValueOrDefault(messageParts, 39);
            dt.activeEnergyH2 = GetValueOrDefault(messageParts, 40);

            dt.firmware_version = GetValueOrDefault(messageParts, 41);

            dt.date = DateDisplay;
            dt.time = TimeDisplay;

            dt.phase = "3 Phase";

        }

        private void AssignNCProperties(MqttClass dt)
        {
            string DateDisplay = DateTime.Now.ToString("dd-MM-yyyy");
            string TimeDisplay = DateTime.Now.ToString("HH:mm:ss");

            dt.dt_meter_serial_no_mqtt = "NC";
            dt.dt_meter_serial_no = "NC";
            dt.mri_serial_no = "NC";
            dt.phase_a_instantaneous_current_a = "NC";
            dt.phase_b_instantaneous_current_a = "NC";
            dt.phase_c_instantaneous_current_a = "NC";
            dt.phase_a_instantaneous_voltage_v = "NC";
            dt.phase_b_instantaneous_voltage_v = "NC";
            dt.phase_c_instantaneous_voltage_v = "NC";
            dt.instantaneous_power_factor = "NC";
            dt.instantaneous_import_active_power_kw = "NC";
            dt.instantaneous_export_active_power_kw = "NC";
            dt.instantaneous_import_reactive_power_kvar = "NC";
            dt.instantaneous_export_reactive_power_kvar = "NC";
            dt.instantaneous_import_apparent_power_kva = "NC";
            dt.instantaneous_export_apparent_power_kva = "NC";
            dt.instantaneous_total_active_power_kw = "NC";
            dt.total_reactive_power_kvar = "NC";
            dt.total_apparent_power_kva = "NC";
            dt.instantaneous_net_active_power_kw = "NC";
            dt.phase_a_instantaneous_import_active_power_kw = "NC";
            dt.phase_a_instantaneous_export_active_power_kw = "NC";
            dt.phase_a_instantaneous_import_reactive_power_kvar = "NC";
            dt.phase_a_instantaneous_export_reactive_power_kvar = "NC";
            dt.phase_a_instantaneous_import_apparent_power_kva = "NC";
            dt.phase_a_instantaneous_export_apparent_power_kva = "NC";
            dt.phase_b_instantaneous_import_active_power_kw = "NC";
            dt.phase_b_instantaneous_export_active_power_kw = "NC";
            dt.phase_b_instantaneous_import_reactive_power_kvar = "NC";
            dt.phase_b_instantaneous_export_reactive_power_kvar = "NC";
            dt.phase_b_instantaneous_import_apparent_power_kva = "NC";
            dt.phase_b_instantaneous_export_apparent_power_kva = "NC";
            dt.phase_c_instantaneous_import_active_power_kw = "NC";
            dt.phase_c_instantaneous_export_active_power_kw = "NC";
            dt.phase_c_instantaneous_import_reactive_power_kvar = "NC";
            dt.phase_c_instantaneous_export_reactive_power_kvar = "NC";
            dt.phase_c_instantaneous_import_apparent_power_kva = "NC";
            dt.phase_c_instantaneous_export_apparent_power_kva = "NC";
            dt.activeEnergyH1 = "NC";
            dt.activeEnergyH2 = "NC";
            dt.firmware_version = "NC";
            dt.date = DateDisplay;
            dt.time = TimeDisplay;

            dt.phase = "Unknown";

        }


        private void AssignPropertiesForShortMessage(MqttClass dt, string[] messageParts)
        {
            string DateDisplay = DateTime.Now.ToString("dd/MM/yyyy");
            string TimeDisplay = DateTime.Now.ToString("hh:mm:ss tt");

            dt.dt_meter_serial_no_mqtt = GetValueOrDefault(messageParts, 1);
            dt.dt_meter_serial_no = GetValueOrDefault(messageParts, 1);
            dt.mri_serial_no = GetValueOrDefault(messageParts, 2);

            dt.phase_a_instantaneous_current_a = GetValueOrDefault(messageParts, 3);
            dt.phase_a_instantaneous_voltage_v = GetValueOrDefault(messageParts, 4);
            dt.instantaneous_power_factor = GetValueOrDefault(messageParts, 5);

            dt.instantaneous_import_active_power_kw = GetValueOrDefault(messageParts, 6);
            dt.instantaneous_export_active_power_kw = GetValueOrDefault(messageParts, 7);
            dt.instantaneous_import_reactive_power_kvar = GetValueOrDefault(messageParts, 8);
            dt.instantaneous_export_reactive_power_kvar = GetValueOrDefault(messageParts, 9);
            dt.instantaneous_import_apparent_power_kva = GetValueOrDefault(messageParts, 10);
            dt.instantaneous_export_apparent_power_kva = GetValueOrDefault(messageParts, 11);

            dt.active_energy = GetValueOrDefault(messageParts, 12);
            dt.activeEnergyH1 = GetValueOrDefault(messageParts, 13);
            dt.activeEnergyH2 = GetValueOrDefault(messageParts, 14);

            dt.firmware_version = GetValueOrDefault(messageParts, 15);
            dt.date = GetValueOrDefault(messageParts, 16);
            dt.time = GetValueOrDefault(messageParts, 17);

            dt.phase = "1 Phase";


        }




        private string GetValueOrDefault(string[] parts, int index)
        {
            if (index < parts.Length)
            {
                return parts[index].Trim();
            }
            return "NC"; // return default value if index is out of range
        }

        private void BtnPauseMqtt_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnPauseMqtt && btnPauseMqtt.Tag is ViewDataButtonInfo buttonInfo)
            {
                // Check if MQTT clients are connected
                bool anyClientConnected = (mqttClient1 != null && mqttClient1.IsConnected) ||
                                          (mqttClient2 != null && mqttClient2.IsConnected) ||
                                          (mqttClient3 != null && mqttClient3.IsConnected);

                if (anyClientConnected)
                {
                    List<MqttClass> temporaryData = GetTemporaryData(buttonInfo.District, buttonInfo.ESD);

                    if (temporaryData != null)
                    {
                        if (!buttonTemporaryData.ContainsKey(btnPauseMqtt))
                        {
                            buttonTemporaryData.Add(btnPauseMqtt, temporaryData);

                            List<string> mqttTopics = temporaryData
                                .Where(data => !string.IsNullOrEmpty(data.instantaneous))  // Filter out empty instantaneous values
                                .Select(data => data.instantaneous)
                                .ToList();

                            buttonSubscribedTopics[btnPauseMqtt] = mqttTopics;

                            // Unsubscribe from topics on all brokers
                            UnsubscribeFromTopics(btnPauseMqtt);

                            
                        }
                        else
                        {
                            buttonTemporaryData[btnPauseMqtt] = temporaryData;

                            buttonSubscribedTopics[btnPauseMqtt] = temporaryData
                                .Select(data => data.instantaneous)
                                .ToList();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Temporary data is null or empty.");
                    }
                }
                else
                {
                    // Display an error message if no MQTT client is connected
                    MessageBox.Show("No MQTT clients are connected. Cannot pause without an active connection.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UnsubscribeFromTopics(Button button)
        {
            if (buttonSubscribedTopics.TryGetValue(button, out List<string> subscribedTopics))
            {
                if (subscribedTopics != null && subscribedTopics.Any())
                {
                    // Unsubscribe from topics on Broker 1
                    if (mqttClient1 != null && mqttClient1.IsConnected)
                    {
                        mqttClient1.Unsubscribe(subscribedTopics.ToArray());
                    }

                    // Unsubscribe from topics on Broker 2
                    if (mqttClient2 != null && mqttClient2.IsConnected)
                    {
                        mqttClient2.Unsubscribe(subscribedTopics.ToArray());
                    }

                    // Unsubscribe from topics on Broker 3
                    if (mqttClient3 != null && mqttClient3.IsConnected)
                    {
                        mqttClient3.Unsubscribe(subscribedTopics.ToArray());
                    }

                    // Additional cleanup if needed
                    MessageBox.Show("Unsubscribed from topics on all connected brokers.");
                }
                else
                {
                    MessageBox.Show("No topics found for the button in the subscribed topics dictionary.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Button not found in the subscribed topics dictionary.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion mqttClient


        #region Export Data


        private void btnExport_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();

            contextMenu.Background = (Brush)new BrushConverter().ConvertFrom("#202020");

            Style menuItemStyle = new Style(typeof(MenuItem));
            menuItemStyle.Setters.Add(new Setter(ForegroundProperty, Brushes.White));
            menuItemStyle.Setters.Add(new Setter(FontSizeProperty, 12.0));
            menuItemStyle.Setters.Add(new Setter(MarginProperty, new Thickness(20, 5, 20, 5)));

            MenuItem menExportXls = new MenuItem { Header = "Export as .xls", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.MicrosoftExcel, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
            menExportXls.Click += MenExportXls_Click;
            contextMenu.Items.Add(menExportXls);

            //MenuItem menExportPdf = new MenuItem { Header = "Export as .pdf", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.FilePdfBox, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
            //menExportPdf.Click += menExportPdf_Click;
            //contextMenu.Items.Add(menExportPdf);

            MenuItem menExportCsv = new MenuItem { Header = "Export as .csv", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.FileCsv, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
            menExportCsv.Click += menExportCsv_Click;
            contextMenu.Items.Add(menExportCsv);

            MenuItem menExportEmail = new MenuItem { Header = "Send as Email", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.Email, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
            menExportEmail.Click += MenExportEmail_Click; ;
            contextMenu.Items.Add(menExportEmail);

            btnExport.ContextMenu = contextMenu;
            btnExport.Focus();
        }


        
        private void MenExportXls_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = datagridInstantPara1.ExportToExcel(datagridInstantPara1.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                FilterIndex = 2,
                Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx",
                FileName = "Instantateous Parameter_" + selectedEsd +"_" + ".xlsx"

            };

            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = sfd.OpenFile())
                {

                    if (sfd.FilterIndex == 1)
                        workBook.Version = ExcelVersion.Excel97to2003;

                    else if (sfd.FilterIndex == 2)
                        workBook.Version = ExcelVersion.Excel2010;

                    else
                        workBook.Version = ExcelVersion.Excel2013;
                    workBook.SaveAs(stream);
                }

                //Message box confirmation to view the created workbook.

                if (MessageBox.Show("Do you want to view the workbook?", "Workbook has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {

                    //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }


        private void menExportCsv_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;

            var excelEngine = datagridInstantPara1.ExportToExcel(datagridInstantPara1.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV Files(*.csv)|*.csv",
                FileName = "Instantateous Parameter_" + selectedEsd + "_" + ".csv"

            };

            if (sfd.ShowDialog() == true)
            {
                string filePath = sfd.FileName;

                if (System.IO.Path.GetExtension(filePath) != ".csv")
                {
                    filePath = System.IO.Path.ChangeExtension(filePath, ".csv");
                }

                workBook.SaveAs(filePath, ",");

                if (MessageBox.Show("Do you want to open the CSV file?", "CSV file has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(filePath);
                }
            }
        }

        private void menExportPdf_Click(object sender, RoutedEventArgs e)
        {
            var document = datagridInstantPara1.ExportToPdf();
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Files(*.pdf)|*.pdf",
                FileName = "Instantateous Parameter_" + selectedEsd + "_" + ".pdf"

            };

            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = sfd.OpenFile())
                {
                    document.Save(stream);
                }

                if (MessageBox.Show("Do you want to view the Pdf file?", "Pdf file has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }


        private void MenExportEmail_Click(object sender, RoutedEventArgs e)
        {
            grdEmailAddress.Visibility = Visibility.Visible;

        }



        private async void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                progressLogin.Visibility = Visibility.Visible;

                string fromMail = "fuzzymri2023@gmail.com";
                string fromPassword = "fjzjgdlvesuonohj";

                MailMessage message2 = new MailMessage
                {
                    From = new MailAddress(fromMail),
                    Subject = "Instantaneous Data",
                    To = { new MailAddress(txtEmailAdress.Text) },
                    Body = "<html><body> Kindly find the attached Instantaneous Data</body></html>",
                    IsBodyHtml = true
                };

                // Create a new SmtpClient instance within the method scope
                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential(fromMail, fromPassword);
                    smtpClient.EnableSsl = true;

                    // Export the DataGrid to Excel
                    var options = new ExcelExportingOptions();
                    options.ExcelVersion = ExcelVersion.Excel2013;
                    var excelEngine = datagridInstantPara1.ExportToExcel(datagridInstantPara1.View, options);
                    var workBook = excelEngine.Excel.Workbooks[0];

                    // Save the Excel file to a temporary location
                    string tempFilePath = System.IO.Path.GetTempFileName();
                    workBook.SaveAs(tempFilePath);
                    workBook.Close();

                    // Attach the Excel file to the email
                    Attachment attachment = new Attachment(tempFilePath);
                    attachment.ContentType.MediaType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    attachment.Name = "Instantaneous_" + selectedEsd + ".xlsx";
                    message2.Attachments.Add(attachment);

                    // Run the email sending logic on a background thread
                    await Task.Run(() => SendEmailWithAttachment(smtpClient, message2));

                    // The email sending is complete; you can now update UI or perform other tasks
                    MessageBox.Show("Instantaneous Data Successfully Sent", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Close the attachment stream after sending the email
                    attachment.Dispose();

                    grdEmailAddress.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                MessageBox.Show("Failed to send email. Please check your email settings and try again.", "Send Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Hide the progressLogin control after the email is sent or an error occurs
                progressLogin.Visibility = Visibility.Collapsed;
            }
        }

        private void SendEmailWithAttachment(SmtpClient smtpClient, MailMessage message)
        {
            smtpClient.Send(message);
        }



        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            grdEmailAddress.Visibility = Visibility.Collapsed;
        }
        #endregion Export Data


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



        private async void cbNationData_Checked(object sender, RoutedEventArgs e)
        {
            grdHideButton.Visibility = Visibility.Visible;

            grdAllESDSummary.Visibility = Visibility.Visible;

            await ReadDatabase1();

        }

        private void cbNationData_Unchecked(object sender, RoutedEventArgs e)
        {
            grdHideButton.Visibility = Visibility.Collapsed;

            grdAllESDSummary.Visibility = Visibility.Collapsed;

            lblValidDataCount.Content = "0";
            lblTotalDataCount.Content = "0";
            lblTotalNCCount.Content = "0";
            lblPercentReporting.Content = "0";

        }


        private async Task ReadDatabase1()
        {
            progressLogin.Visibility = Visibility.Visible;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "http://103.234.126.43:3080/dtmeter/info/esd_essd_info/all";
                    string json = await client.GetStringAsync(url);
                    allDataCountry = JsonConvert.DeserializeObject<List<MqttClass>>(json);
                    datagridInstantPara1.ItemsSource = allDataCountry;
                }
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show($"Request error: {e.Message}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            finally
            {
                progressLogin.Visibility = Visibility.Collapsed;
            }

            ConnectToBroker(ref mqttClient1, mqttBrokerAddress1, "Connected to MQTT Server 1");
            ConnectToBroker(ref mqttClient2, mqttBrokerAddress2, "Connected to MQTT Server 2");
            ConnectToBroker(ref mqttClient3, mqttBrokerAddress3, "Connected to MQTT Server 3");

            // Update the UI once all connections are attempted
            btnConnectMqtt.Visibility = Visibility.Collapsed;
            btnDisConnectConnectMqtt.Visibility = Visibility.Visible;

        }

        private void BtnPlayMqttCountry_Click(object sender, RoutedEventArgs e)
        {
            if (mqttClient1 != null && mqttClient1.IsConnected || mqttClient2 != null && mqttClient2.IsConnected || mqttClient3 != null && mqttClient3.IsConnected)
            {
                List<string> mqttTopics = allDataCountry
                    .Where(data => !string.IsNullOrEmpty(data.instantaneous))
                    .Select(data => data.instantaneous)
                    .ToList();

                if (mqttTopics.Any())
                {
                    InitializeMqtt(mqttTopics);

                    lblConnStatus.Content = "Initialized MQTT Subscription";
                    lblConnStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#00cdac");

                    BtnPauseMqttCountry.Visibility = Visibility.Visible;
                    BtnPlayMqttCountry.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("All 'instantaneous' values are empty. No data to display.");
                }
            }
            else
            {
                MessageBox.Show("MQTT client is not connected. Please connect before playing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeMqtt(List<string> topics)
        {
            _ = mqttClient1.Subscribe(topics.ToArray(), topics.Select(_ => MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE).ToArray());
            _ = mqttClient2.Subscribe(topics.ToArray(), topics.Select(_ => MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE).ToArray());
            _ = mqttClient3.Subscribe(topics.ToArray(), topics.Select(_ => MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE).ToArray());

        }

        private void MqttMsgPublishReceivedCountry(object sender, MqttMsgPublishEventArgs e)
        {
            string mqttTopic = e.Topic;
            string message = Encoding.UTF8.GetString(e.Message);

            MqttClass dt = allDataCountry.FirstOrDefault(data => data.instantaneous == mqttTopic);

            if (dt != null && !string.IsNullOrEmpty(message))
            {
                string[] messageParts = message.Split(',');

                int messageCount = messageParts.Length;
                int ncCount = messageParts.Count(part => part.Trim() == "NC");

                if (messageCount == 44)
                {
                    AssignProperties(dt, messageParts);
                }

                else if (messageCount == 42)
                {
                    AssignPropertiesEarlierV(dt, messageParts);
                }
                else if (messageCount == 18)
                {
                    AssignPropertiesForShortMessage(dt, messageParts);
                }

                else
                {
                    AssignNCProperties(dt);
                }

                Dispatcher.Invoke(() =>
                {
                    lblSuccessfulPolls.Content = ++successNumber;
                    lblTotalPolls.Content = successNumber + unsuccessNumber;
                });

                timer.Stop();
                timer.Start();
            }
            // Define variables to hold the counts
            int column4Count = 0; // dt_meter_serial_no_mqtt
            int column8Count = 0; // phase_a_instantaneous_current_a

            int ncDataCount = 0;
            double percentReporting = 0;
            // Regular expression to match numeric strings
            Regex numericRegex = new Regex(@"^\d+$");


            foreach (var item in datagridInstantPara1.View.Records)
            {
                if (item != null)
                {
                    var rowData = item.Data as MqttClass; // Replace MqttClass with the actual type of your data item

                    if (rowData != null)
                    {
                        if (rowData.dt_meter_serial_no_mqtt == "NC")
                        {
                            ncDataCount++;
                        }

                        if (!string.IsNullOrEmpty(rowData.phase?.ToString()))
                        {
                            column8Count++;
                        }
                    }
                }
            }




            if (column8Count > 0)
            {
                percentReporting = (double)column4Count / column8Count * 100.0;
            }
            else
            {
                percentReporting = 0.0;
            }

            // Update UI elements on the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                column4Count = column8Count - ncDataCount;

                if (column8Count > 0)
                {
                    percentReporting = (double)column4Count / column8Count * 100.0;
                }
                else
                {
                    percentReporting = 0.0;
                }
                // Update label content with the counts
                lblValidDataCount.Content = column4Count;
                lblTotalDataCount.Content = column8Count;
                lblTotalNCCount.Content = ncDataCount;
                lblPercentReporting.Content = percentReporting.ToString("0.00");
            });
        }

        private void BtnPauseMqttCountry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mqttClient1 != null && mqttClient1.IsConnected || mqttClient2 != null && mqttClient2.IsConnected || mqttClient3 != null && mqttClient3.IsConnected)
                {
                    List<string> topicsToUnsubscribe = allDataCountry
                        .Where(data => !string.IsNullOrEmpty(data.instantaneous))
                        .Select(data => data.instantaneous)
                        .ToList();

                    if (topicsToUnsubscribe.Any())
                    {
                        mqttClient1.Unsubscribe(topicsToUnsubscribe.ToArray());
                        mqttClient2.Unsubscribe(topicsToUnsubscribe.ToArray());
                        mqttClient3.Unsubscribe(topicsToUnsubscribe.ToArray());


                        lblConnStatus.Content = "Paused MQTT Subscription";
                        lblConnStatus.Foreground = (Brush)new BrushConverter().ConvertFrom("#ff5768");


                        BtnPlayMqttCountry.Visibility = Visibility.Visible;
                        BtnPauseMqttCountry.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        MessageBox.Show("No topics to unsubscribe.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("MQTT client is not connected. Please connect before pausing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
