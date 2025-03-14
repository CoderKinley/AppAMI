using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AppAMI.Classes;
using Newtonsoft.Json;
using System.Net;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System.IO;
using System;
using MaterialDesignThemes.Wpf;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Data;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Net.Http;

namespace AppAMI.Events
{
    /// <summary>
    /// Interaction logic for Events.xaml
    /// </summary>
    public partial class Events : UserControl
    {
        private List<string> storedItems = new List<string>();

        string selectedEsd;
        string selectedDistrict;
        string selectedPlaceForData;

        string CurrentUserId1;
        string CurrentUserRole1;
        string CurrentUserPassword1;
        string CurrentUserName1;
        string CurrentUserEmployeeId1;


        public Events(string CurrentUserId, string CurrentUserRole, string CurrentUserPassword, string CurrentUserName, string CurrentUserEmployeeId)
        {
            InitializeComponent();

            CurrentUserId1 = CurrentUserId;
            CurrentUserRole1 = CurrentUserRole;
            CurrentUserPassword1 = CurrentUserPassword;
            CurrentUserName1 = CurrentUserName;
            CurrentUserEmployeeId1 = CurrentUserEmployeeId;

            dtPickerStart.SelectedDate = DateTime.Today;
            dtPickerEnd.SelectedDate = DateTime.Today;

            grdEmailAddress.Visibility = Visibility.Collapsed;

            Task.Run(() => loadData());
        }


        #region From GIS
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
            dtPickerStart.SelectedDate = DateTime.Today;
            dtPickerEnd.SelectedDate = DateTime.Today;

            buttonText = "Data from GIS";
            // MessageBox.Show("Creating Button3");

            key = $"{selectedDistrict}_{selectedEsd}_{dtPickerStart.SelectedDate.Value.ToString("yyyyMMdd")}_{dtPickerEnd.SelectedDate.Value.ToString("yyyyMMdd")}";
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

                contentPanel.Children.Add(btnViewData);
                contentPanel.Children.Add(btnViewClose);

                UpdateContentPanelBackground(contentPanel, isSelected: true);

                foreach (StackPanel existingPanel in stackCurrentData.Children.OfType<StackPanel>().Where(panel => panel != contentPanel))
                {
                    UpdateContentPanelBackground(existingPanel, isSelected: false);
                }

                stackCurrentData.Children.Add(contentPanel);

                contentPanel.Tag = GetContentPanelKey(selectedDistrict, selectedEsd, dtPickerStart.SelectedDate.Value, dtPickerEnd.SelectedDate.Value);

                ViewDataButtonInfo buttonInfo = new ViewDataButtonInfo
                {
                    District = selectedDistrict,
                    ESD = selectedEsd,
                    StartDate = dtPickerStart.SelectedDate.Value,
                    EndDate = dtPickerEnd.SelectedDate.Value,
                };

                btnViewData.Tag = buttonInfo;

                btnViewData.Click += BtnViewData_Click;
                btnViewClose.Click += BtnViewClose_Click;

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
                        var apiResponseList = JsonConvert.DeserializeObject<List<MeterEvents>>(jsonResult);

                        if (apiResponseList != null && apiResponseList.Any())
                        {
                            // After storing the information, make requests for billing data
                            GetEventDataForStoredInfo(apiResponseList);
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

        private List<MeterEvents> accumulatedEventData = new List<MeterEvents>();

        private async void GetEventDataForStoredInfo(List<MeterEvents> apiResponseList)
        {
            datagridEvents.ClearFilters();

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

                        // Now make a request for billing data
                        await GetEventDataForDistrictCode(districtCode, dtId);
                    }
                    else
                    {
                        // Handle case where dt_id is not found
                        MessageBox.Show($"Info for {dtId} not found.");
                    }
                }

                // After processing all items, set the accumulated data to the datagrid
                await Dispatcher.InvokeAsync(() =>
                {
                    datagridEvents.ItemsSource = accumulatedEventData;

                    // Calculate the counts
                    int totalEventsCount = 0;
                    int totalUniqueDtCount = 0;

                    HashSet<string> uniqueDtIds = new HashSet<string>();

                    if (datagridEvents.View?.Records != null)
                    {
                        foreach (var item in datagridEvents.View.Records)
                        {
                            var rowData = item.Data as MeterEvents;

                            if (!string.IsNullOrEmpty(rowData?.error_code))
                            {
                                totalEventsCount++;
                            }

                            if (!string.IsNullOrEmpty(rowData?.dt_id))
                            {
                                uniqueDtIds.Add(rowData.dt_id);
                            }
                        }

                        totalUniqueDtCount = uniqueDtIds.Count;
                    }

                    lblTotalTripCount.Content = totalEventsCount;
                    lblUniqueDtCount.Content = totalUniqueDtCount;
                });


                SaveDataToTemporaryStorage(selectedDistrict, selectedEsd, dtPickerStart.SelectedDate.Value, dtPickerEnd.SelectedDate.Value, accumulatedEventData);

            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show($"Exception while processing billing data: {ex.Message}");
            }
        }

        private async Task GetEventDataForDistrictCode(string districtCode, string dtId)
        {
            progressLogin.Visibility = Visibility.Visible;
            try
            {
                string apiUrl = $"http://103.234.126.43:3080/api/events/{dtId}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON response to your MeterEvents model
                        List<MeterEvents> eventData = JsonConvert.DeserializeObject<List<MeterEvents>>(jsonResult);

                        if (eventData != null)
                        {
                            // Filter the eventData for the specific dt_id and date range
                            List<MeterEvents> filteredDateData = eventData
                                ?.Where(x => x.dt_id == dtId &&
                                             DateTime.TryParseExact(x.record_date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate) &&
                                             parsedDate >= dtPickerStart.SelectedDate.Value &&
                                             parsedDate <= dtPickerEnd.SelectedDate.Value)
                                .ToList();

                            if (filteredDateData.Any())
                            {
                                // Append the filtered event data to the accumulated data
                                accumulatedEventData.AddRange(filteredDateData);
                            }
                            else
                            {
                                // Handle case where no data is found for the dt_id
                                MessageBox.Show($"No event data found for {dtId}.");
                            }
                        }
                        else
                        {
                            // Handle case where eventData is null
                            MessageBox.Show($"Event data for {dtId} is null or has an unexpected structure.");
                        }
                    }
                    else
                    {
                        // Handle unsuccessful response
                        MessageBox.Show($"Failed to get event data for {dtId}. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show($"Exception while getting event data for {dtId}: {ex.Message}");
            }

            finally
            {
                progressLogin.Visibility = Visibility.Collapsed;
            }
        }



        #endregion From GIS


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
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        private StackPanel selectedContentPanel = null;

        private void UpdateContentPanelBackground(StackPanel contentPanel, bool isSelected)
        {
            contentPanel.Background = new SolidColorBrush(isSelected ? SelectedBackgroundColor : DefaultBackgroundColor);

        }

        private readonly Color DefaultBackgroundColor = (Color)ColorConverter.ConvertFromString("#2d2d30");
        private readonly Color SelectedBackgroundColor = (Color)ColorConverter.ConvertFromString("#00a5e3");

        private string GetContentPanelKey(string district, string esd, DateTime startDate, DateTime endDate)
        {
            return $"{district}_{esd}_{startDate.ToString("yyyyMMdd")}_{endDate.ToString("yyyyMMdd")}";
        }

        private void SaveDataToTemporaryStorage(string district, string esd, DateTime startDate, DateTime endDate, List<MeterEvents> data)
        {
            string key = $"{district}_{esd}_{startDate.ToString("yyyyMMdd")}_{endDate.ToString("yyyyMMdd")}";

            temporaryStorage[key] = data;
        }

        private void BtnViewData_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnViewData && btnViewData.Tag is ViewDataButtonInfo buttonInfo)
            {
                //MessageBox.Show($"Button Clicked: {buttonInfo.District}, {buttonInfo.ESD}, {buttonInfo.StartDate}, {buttonInfo.EndDate}");

                List<MeterEvents> temporaryData = GetTemporaryData(buttonInfo.District, buttonInfo.ESD, buttonInfo.StartDate, buttonInfo.EndDate);

                if (temporaryData != null)
                {
                    //MessageBox.Show($"Fetched Data Count: {temporaryData.Count}");
                    datagridEvents.ItemsSource = temporaryData;
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

        private List<MeterEvents> GetTemporaryData(string district, string esd, DateTime startDate, DateTime endDate)
        {
            string key = $"{district}_{esd}_{startDate.ToString("yyyyMMdd")}_{endDate.ToString("yyyyMMdd")}";

            if (temporaryStorage.TryGetValue(key, out List<MeterEvents> temporaryData))
            {
                return temporaryData;
            }
            else
            {
                return new List<MeterEvents>();
            }
        }

        private Dictionary<string, List<MeterEvents>> temporaryStorage = new Dictionary<string, List<MeterEvents>>();

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

                        RemoveDataFromTemporaryStorage(buttonInfo.District, buttonInfo.ESD, buttonInfo.StartDate, buttonInfo.EndDate);

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

        private void RemoveDataFromTemporaryStorage(string district, string esd, DateTime startDate, DateTime endDate)
        {
            string key = $"{district}_{esd}_{startDate.ToString("yyyyMMdd")}_{endDate.ToString("yyyyMMdd")}";
            temporaryStorage.Remove(key);
        }

        private void RefreshDataGrid()
        {
            if (stackCurrentData.Children.Count > 0 && stackCurrentData.Children[0] is StackPanel firstContentPanel)
            {
                if (firstContentPanel.Children[0] is Button firstBtnViewData && firstBtnViewData.Tag is ViewDataButtonInfo firstButtonInfo)
                {
                    List<MeterEvents> temporaryData = GetTemporaryData(firstButtonInfo.District, firstButtonInfo.ESD, firstButtonInfo.StartDate, firstButtonInfo.EndDate);

                    datagridEvents.ItemsSource = temporaryData;
                }
            }
        }



        private async void myTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            progressLogin.Visibility = Visibility.Visible;

            if (e.NewValue == null || !(e.NewValue is TreeViewItem selectedTreeViewItem))
            {
                return;
            }

            try
            {
                StackPanel existingContentPanel = null;

                if (selectedTreeViewItem.Parent is TreeViewItem selectedDistrictItem && selectedDistrictItem.Tag != null && selectedTreeViewItem.Tag != null)
                {
                    selectedDistrict = (string)selectedDistrictItem.Tag;
                    selectedEsd = (string)selectedTreeViewItem.Tag;
                    selectedPlaceForData = "DataEsd";
                    dtPickerStart.SelectedDate = DateTime.Today;
                    dtPickerEnd.SelectedDate = DateTime.Today;

                    buttonText = (string)selectedTreeViewItem.Header;
                    key = $"{selectedDistrict}_{selectedEsd}_{dtPickerStart.SelectedDate.Value.ToString("yyyyMMdd")}_{dtPickerEnd.SelectedDate.Value.ToString("yyyyMMdd")}";

                    existingContentPanel = stackCurrentData.Children.OfType<StackPanel>().FirstOrDefault(panel => panel.Tag != null && (string)panel.Tag == key);

                }
                else if (e.NewValue is TreeViewItem districtItem)
                {
                    selectedDistrict = (string)districtItem.Tag;
                    selectedPlaceForData = "DataDistrict";
                    selectedEsd = "Multiple ESD";
                    dtPickerStart.SelectedDate = DateTime.Today;
                    dtPickerEnd.SelectedDate = DateTime.Today;

                    buttonText = (string)districtItem.Header;
                    key = $"{selectedDistrict}_{selectedEsd}_{dtPickerStart.SelectedDate.Value.ToString("yyyyMMdd")}_{dtPickerEnd.SelectedDate.Value.ToString("yyyyMMdd")}";

                    existingContentPanel = stackCurrentData.Children.OfType<StackPanel>().FirstOrDefault(panel => panel.Tag != null && (string)panel.Tag == key);


                }
                else
                {
                    return;
                }

                if (dtPickerStart.SelectedDate.HasValue && dtPickerEnd.SelectedDate.HasValue)
                {

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

                        contentPanel.Children.Add(btnViewData);
                        contentPanel.Children.Add(btnViewClose);

                        UpdateContentPanelBackground(contentPanel, isSelected: true);

                        foreach (StackPanel existingPanel in stackCurrentData.Children.OfType<StackPanel>().Where(panel => panel != contentPanel))
                        {
                            UpdateContentPanelBackground(existingPanel, isSelected: false);
                        }

                        stackCurrentData.Children.Add(contentPanel);

                        contentPanel.Tag = GetContentPanelKey(selectedDistrict, selectedEsd, dtPickerStart.SelectedDate.Value, dtPickerEnd.SelectedDate.Value);

                        ViewDataButtonInfo buttonInfo = new ViewDataButtonInfo
                        {
                            District = selectedDistrict,
                            ESD = selectedEsd,
                            StartDate = dtPickerStart.SelectedDate.Value,
                            EndDate = dtPickerEnd.SelectedDate.Value,
                        };

                        btnViewData.Tag = buttonInfo;

                        btnViewData.Click += BtnViewData_Click;
                        btnViewClose.Click += BtnViewClose_Click;

                        selectedContentPanel = contentPanel;

                        await ReadDatabase();
                    }
                }
                else
                {
                    MessageBox.Show("Please insert the date.", "Date Required", MessageBoxButton.OK, MessageBoxImage.Warning);
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







      

        public class SelectedItemInfo
        {
            public string District { get; set; }
            public string ESD { get; set; }
            public string PlaceForData { get; set; }
        }

        #endregion add tree view items


        #region Read DataBase

        private async void dtPicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd"; //for the second type
            Thread.CurrentThread.CurrentCulture = ci;

            if (!dtPickerEnd.SelectedDate.HasValue)
            {
                dtPickerEnd.SelectedDate = DateTime.Today;
                return;
            }

            if (dtPickerEnd.SelectedDate.HasValue && dtPickerEnd.SelectedDate > dtPickerStart.SelectedDate || dtPickerEnd.SelectedDate == dtPickerStart.SelectedDate)
            {
                if (!string.IsNullOrEmpty(selectedDistrict) || !string.IsNullOrEmpty(selectedEsd))
                {
                    await ReadDatabase();
                }

            }


            else
            {
                MessageBox.Show("Please make sure the date range is valid.", "Invalid Date Range", MessageBoxButton.OK, MessageBoxImage.Warning);
            }




        }

        private async void dtPickerEnd_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd"; //for the second type
            Thread.CurrentThread.CurrentCulture = ci;


            if (!dtPickerStart.SelectedDate.HasValue)
            {
                dtPickerStart.SelectedDate = DateTime.Today;
                return;
            }

            if (dtPickerEnd.SelectedDate.HasValue && dtPickerEnd.SelectedDate > dtPickerStart.SelectedDate || dtPickerEnd.SelectedDate == dtPickerStart.SelectedDate)
            {
                if (!string.IsNullOrEmpty(selectedDistrict) || !string.IsNullOrEmpty(selectedEsd))
                {
                    await ReadDatabase();
                }

            }
            else
            {
                MessageBox.Show("Please make sure the date range is valid.", "Invalid Date Range", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }



        private async Task ReadDatabase()
        {
            datagridEvents.ClearFilters();

            progressLogin.Visibility = Visibility.Visible;

            if (!string.IsNullOrWhiteSpace(dtPickerStart.Text) || !string.IsNullOrWhiteSpace(dtPickerEnd.Text))
            {
                try
                {
                    using (WebClient web = new WebClient())
                    {
                        string startDate = dtPickerStart.SelectedDate?.ToString("yyyy-MM-dd");
                        string endDate = dtPickerEnd.SelectedDate?.ToString("yyyy-MM-dd");

                        string url = string.Format("http://103.234.126.43:3080/dtmeter/events/district/{0}/{1}/{2}", selectedDistrict, startDate, endDate);

                        //string url = string.Format("http://103.234.126.43:3080/dtmeter/events/district/{0}", selectedDistrict);


                        string json = await web.DownloadStringTaskAsync(url);

                        if (string.IsNullOrWhiteSpace(json))
                        {
                            MessageBox.Show("No data found from the server.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }

                        List<MeterEvents> allData = JsonConvert.DeserializeObject<List<MeterEvents>>(json);

                        if (allData == null)
                        {
                            MessageBox.Show("Error parsing JSON data. Deserialized data is null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        List<MeterEvents> filtered_date_data = allData
                            ?.Where(x => DateTime.TryParseExact(x.record_date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                                        && DateTime.ParseExact(x.record_date, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= dtPickerStart.SelectedDate.Value
                                        && DateTime.ParseExact(x.record_date, "yyyy-MM-dd", CultureInfo.InvariantCulture) <= dtPickerEnd.SelectedDate.Value)
                            .ToList();

                        if (filtered_date_data == null)
                        {
                            MessageBox.Show("Error filtering data. Filtered data is null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        List<MeterEvents> filteredDataPlace = new List<MeterEvents>();

                        if (selectedPlaceForData == "DataDistrict")
                        {
                             filteredDataPlace = filtered_date_data.Where(a => a?.district_code == selectedDistrict).ToList();
                            SaveDataToTemporaryStorage(selectedDistrict, selectedEsd, dtPickerStart.SelectedDate.Value, dtPickerEnd.SelectedDate.Value, filteredDataPlace);


                        }
                        else if (selectedPlaceForData == "DataEsd")
                        {
                            filteredDataPlace = filtered_date_data.Where(a => a?.esd_code == selectedEsd).ToList();
                            SaveDataToTemporaryStorage(selectedDistrict, selectedEsd, dtPickerStart.SelectedDate.Value, dtPickerEnd.SelectedDate.Value, filteredDataPlace);

                        }

                        await Dispatcher.InvokeAsync(() =>
                        {
                            datagridEvents.ItemsSource = filteredDataPlace;

                            // Calculate the counts
                            int totalEventsCount = 0;
                            int totalUniqueDtCount = 0;

                            HashSet<string> uniqueDtIds = new HashSet<string>();

                            if (datagridEvents.View?.Records != null)
                            {
                                foreach (var item in datagridEvents.View.Records)
                                {
                                    var rowData = item.Data as MeterEvents;

                                    if (!string.IsNullOrEmpty(rowData?.error_code))
                                    {
                                        totalEventsCount++;
                                    }

                                    if (!string.IsNullOrEmpty(rowData?.dt_id))
                                    {
                                        uniqueDtIds.Add(rowData.dt_id);
                                    }
                                }

                                totalUniqueDtCount = uniqueDtIds.Count;
                            }

                            lblTotalTripCount.Content = totalEventsCount;
                            lblUniqueDtCount.Content = totalUniqueDtCount;
                        });

                    }
                }
                catch (WebException ex)
                {
                    MessageBox.Show("No data found for the current date range. Please change a date range.");
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Error parsing JSON data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    progressLogin.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                MessageBox.Show("Kindly Select Date Range", "Date Range Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        #endregion Read DataBase


        private void Btn_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button clickedButton = sender as Button;

            ContextMenu contextMenu = new ContextMenu();

            MenuItem itemInstaneous = new MenuItem();
            itemInstaneous.Header = "Instantaneous Data";
            itemInstaneous.Click += (s, ea) =>
            {
                MessageBox.Show("Instantaneous Data");
            };
            contextMenu.Items.Add(itemInstaneous);


            MenuItem itemHistoricalPlayBack = new MenuItem();
            itemHistoricalPlayBack.Header = "Historical Playback";
            itemHistoricalPlayBack.Click += (s, ea) =>
            {
                MessageBox.Show("Historical");

            };
            contextMenu.Items.Add(itemHistoricalPlayBack);



            clickedButton.ContextMenu = contextMenu;
        }

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
            var excelEngine = datagridEvents.ExportToExcel(datagridEvents.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                FilterIndex = 2,
                Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx",
                FileName = "Events_" + selectedEsd + "_From_" + dtPickerStart.Text + "_To_" + dtPickerEnd.Text + ".xlsx"

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

            var excelEngine = datagridEvents.ExportToExcel(datagridEvents.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV Files(*.csv)|*.csv",
                FileName = "Events_" + selectedEsd + "_From_" + dtPickerStart.Text + "_To_" + dtPickerEnd.Text + ".csv"

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
            var document = datagridEvents.ExportToPdf();
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Files(*.pdf)|*.pdf",
                FileName = "Events_" + selectedEsd + "_From_" + dtPickerStart.Text + "_To_" + dtPickerEnd.Text + ".pdf"

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
                    Subject = "Events Data",
                    To = { new MailAddress(txtEmailAdress.Text) },
                    Body = "<html><body> Kindly find the attached Events Data</body></html>",
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
                    var excelEngine = datagridEvents.ExportToExcel(datagridEvents.View, options);
                    var workBook = excelEngine.Excel.Workbooks[0];

                    // Save the Excel file to a temporary location
                    string tempFilePath = Path.GetTempFileName();
                    workBook.SaveAs(tempFilePath);
                    workBook.Close();

                    // Attach the Excel file to the email
                    Attachment attachment = new Attachment(tempFilePath);
                    attachment.ContentType.MediaType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    attachment.Name = "Events_" + selectedEsd + "_From_" + dtPickerStart.Text + "_To_" + dtPickerEnd.Text + ".xlsx";
                    message2.Attachments.Add(attachment);

                    // Run the email sending logic on a background thread
                    await Task.Run(() => SendEmailWithAttachment(smtpClient, message2));

                    // The email sending is complete; you can now update UI or perform other tasks
                    MessageBox.Show("Events Data Successfully Sent", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);

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

 
    }
}
