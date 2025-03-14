using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using AppAMI.Classes;
using Newtonsoft.Json;
using System.Net;
using System;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using Microsoft.Win32;
using System.IO;
using MaterialDesignThemes.Wpf;
using System.Globalization;
using System.Threading;
using System.Linq;
using System.Data;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Net.Http;

namespace AppAMI.Report
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : UserControl
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

        DateTime currentDate = DateTime.Today;

        public Report(string CurrentUserId, string CurrentUserRole, string CurrentUserPassword, string CurrentUserName, string CurrentUserEmployeeId)
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

            DateTime twelveMonthsAgo = currentDate.AddMonths(-12);

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
                        var apiResponseList = JsonConvert.DeserializeObject<List<ReportDt>>(jsonResult);

                        if (apiResponseList != null && apiResponseList.Any())
                        {
                            // After storing the information, make requests for billing data
                            GetReportDataForStoredInfo(apiResponseList);
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

        private List<ReportDt> accumulatedReportData = new List<ReportDt>();

        private async void GetReportDataForStoredInfo(List<ReportDt> apiResponseList)
        {
            datagridReport.ClearFilters();

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
                        await GetBillingDataForDistrictCode(districtCode, dtId);
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
                    datagridReport.ItemsSource = accumulatedReportData;

                    // Calculate the counts
                    int totalTrippingCount = 0;
                    int totalFormFilledCount = 0;
                    int totalUnfilledCount = 0;

                    if (datagridReport.View?.Records != null)
                    {
                        foreach (var item in datagridReport.View.Records)
                        {
                            var rowData = item.Data as ReportDt;

                            if (!string.IsNullOrEmpty(rowData?.tripping_id))
                            {
                                totalTrippingCount++;
                            }

                            if (!string.IsNullOrEmpty(rowData?.substation))
                            {
                                totalFormFilledCount++;
                            }
                        }



                        totalUnfilledCount = totalTrippingCount - totalFormFilledCount;


                    }

                    // Update labels with the calculated counts
                    lblTotalTripCount.Content = totalTrippingCount;
                    lblFilledCount.Content = totalFormFilledCount;
                    lblUnfilledCount.Content = totalUnfilledCount;
                });


                SaveDataToTemporaryStorage(selectedDistrict, selectedEsd, dtPickerStart.SelectedDate.Value, dtPickerEnd.SelectedDate.Value, accumulatedReportData);

            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show($"Exception while processing billing data: {ex.Message}");
            }
        }

        private async Task GetBillingDataForDistrictCode(string districtCode, string dtId)
        {
            progressLogin.Visibility = Visibility.Visible;

            try
            {
                string apiUrl = $"http://103.234.126.43:3080/reports/dt/{dtId}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON response to your LoadSurveyTab model
                        List<ReportDt> ReportData = JsonConvert.DeserializeObject<List<ReportDt>>(jsonResult);

                        if (ReportData != null)
                        {
                            // Filter the loadSurveyData for the specific dt_id and date range
                            List<ReportDt> filteredDateData = ReportData
                                ?.Where(x => x.dt_id == dtId &&
                                             DateTime.TryParseExact(x.trip_date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate) &&
                                             parsedDate >= dtPickerStart.SelectedDate.Value &&
                                             parsedDate <= dtPickerEnd.SelectedDate.Value)
                                .ToList();

                            if (filteredDateData.Any())
                            {
                                // Append the filtered load survey data to the accumulated data
                                accumulatedReportData.AddRange(filteredDateData);
                            }
                            else
                            {
                                // Handle case where no data is found for the dt_id
                                MessageBox.Show($"No load profile data found for {dtId}.");
                            }
                        }
                        else
                        {
                            // Handle case where loadSurveyData is null
                            MessageBox.Show($"Load profile data for {dtId} is null or has an unexpected structure.");
                        }
                    }
                    else
                    {
                        // Handle unsuccessful response
                        MessageBox.Show($"Failed to get load profile data for {dtId}. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show($"Exception while getting load profile data for {dtId}: {ex.Message}");
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

        private void SaveDataToTemporaryStorage(string district, string esd, DateTime startDate, DateTime endDate, List<ReportDt> data)
        {
            string key = $"{district}_{esd}_{startDate.ToString("yyyyMMdd")}_{endDate.ToString("yyyyMMdd")}";

            temporaryStorage[key] = data;
        }

        private void BtnViewData_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnViewData && btnViewData.Tag is ViewDataButtonInfo buttonInfo)
            {
                //MessageBox.Show($"Button Clicked: {buttonInfo.District}, {buttonInfo.ESD}, {buttonInfo.StartDate}, {buttonInfo.EndDate}");

                List<ReportDt> temporaryData = GetTemporaryData(buttonInfo.District, buttonInfo.ESD, buttonInfo.StartDate, buttonInfo.EndDate);

                if (temporaryData != null)
                {
                    //MessageBox.Show($"Fetched Data Count: {temporaryData.Count}");
                    datagridReport.ItemsSource = temporaryData;
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

        private List<ReportDt> GetTemporaryData(string district, string esd, DateTime startDate, DateTime endDate)
        {
            string key = $"{district}_{esd}_{startDate.ToString("yyyyMMdd")}_{endDate.ToString("yyyyMMdd")}";

            if (temporaryStorage.TryGetValue(key, out List<ReportDt> temporaryData))
            {
                return temporaryData;
            }
            else
            {
                return new List<ReportDt>();
            }
        }

        private Dictionary<string, List<ReportDt>> temporaryStorage = new Dictionary<string, List<ReportDt>>();

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
                    List<ReportDt> temporaryData = GetTemporaryData(firstButtonInfo.District, firstButtonInfo.ESD, firstButtonInfo.StartDate, firstButtonInfo.EndDate);

                    datagridReport.ItemsSource = temporaryData;
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


        #region Get Load Survey Data


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

            datagridReport.ClearFilters();

            progressLogin.Visibility = Visibility.Visible;

            if (!string.IsNullOrWhiteSpace(dtPickerStart.Text) || !string.IsNullOrWhiteSpace(dtPickerEnd.Text))
            {
                try
                {
                    using (WebClient web = new WebClient())
                    {
                       // string url = string.Format("http://103.234.126.43:3080/reports/district/{0}", selectedDistrict);

                        string startDate = dtPickerStart.SelectedDate?.ToString("yyyy-MM-dd");
                        string endDate = dtPickerEnd.SelectedDate?.ToString("yyyy-MM-dd");

                        string url = string.Format("http://103.234.126.43:3080/dtmeter/reports/district/{0}/{1}/{2}", selectedDistrict, startDate, endDate);

                        string json = await web.DownloadStringTaskAsync(url);

                        if (string.IsNullOrWhiteSpace(json))
                        {
                            MessageBox.Show("No data found from the server.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }

                        List<ReportDt> allData = JsonConvert.DeserializeObject<List<ReportDt>>(json);

                        if (allData == null)
                        {
                            MessageBox.Show("Error parsing JSON data. Deserialized data is null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        List<ReportDt> filtered_date_data = allData
                            ?.Where(x => DateTime.TryParseExact(x.trip_date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                                        && DateTime.ParseExact(x.trip_date, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= dtPickerStart.SelectedDate.Value
                                        && DateTime.ParseExact(x.trip_date, "yyyy-MM-dd", CultureInfo.InvariantCulture) <= dtPickerEnd.SelectedDate.Value)
                            .ToList();

                        if (allData == null)
                        {
                            MessageBox.Show("Error filtering data. Filtered data is null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        List<ReportDt> filteredDataPlace = new List<ReportDt>();


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
                            datagridReport.ItemsSource = filteredDataPlace;

                            // Calculate the counts
                            int totalTrippingCount = 0;
                            int totalFormFilledCount = 0;
                            int totalUnfilledCount = 0;

                            if (datagridReport.View?.Records != null)
                            {
                                foreach (var item in datagridReport.View.Records)
                                {
                                    var rowData = item.Data as ReportDt;

                                    if (!string.IsNullOrEmpty(rowData?.tripping_id))
                                    {
                                        totalTrippingCount++;
                                    }

                                    if (!string.IsNullOrEmpty(rowData?.substation))
                                    {
                                        totalFormFilledCount++;
                                    }
                                }



                                totalUnfilledCount = totalTrippingCount - totalFormFilledCount;


                            }

                            // Update labels with the calculated counts
                            lblTotalTripCount.Content = totalTrippingCount;
                            lblFilledCount.Content = totalFormFilledCount;
                            lblUnfilledCount.Content = totalUnfilledCount;
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



        #endregion Get Load Survey Data


        #region Export DataGrid
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
            var excelEngine = datagridReport.ExportToExcel(datagridReport.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                FilterIndex = 2,
                Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx",
                //FileName = "Report_" + " " + selectedDataType + "_" + selectedEsd + " " + ".xlsx"
                FileName = "Report_"  + selectedEsd +  ".xlsx"

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

            var excelEngine = datagridReport.ExportToExcel(datagridReport.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV Files(*.csv)|*.csv",
                //FileName = "Report_" + " " + selectedDataType + "_" + selectedEsd + " " + ".csv"

                FileName = "Report_"  + selectedEsd + " " + ".csv"
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
            var document = datagridReport.ExportToPdf();
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Files(*.pdf)|*.pdf",
                //FileName = "Report_" + " " + selectedDataType + "_" + selectedEsd + " " + ".pdf"

                FileName = "Report_"  + selectedEsd + " " + ".pdf"
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
                    Subject = "Report",
                    To = { new MailAddress(txtEmailAdress.Text) },
                    Body = "<html><body> Kindly find the attached Report</body></html>",
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
                    var excelEngine = datagridReport.ExportToExcel(datagridReport.View, options);
                    var workBook = excelEngine.Excel.Workbooks[0];

                    // Save the Excel file to a temporary location
                    string tempFilePath = Path.GetTempFileName();
                    workBook.SaveAs(tempFilePath);
                    workBook.Close();

                    // Attach the Excel file to the email
                    Attachment attachment = new Attachment(tempFilePath);
                    attachment.ContentType.MediaType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    attachment.Name = "Report_" + selectedEsd + "_From_" + dtPickerStart.Text + "_To_" + dtPickerEnd.Text + ".xlsx";
                    message2.Attachments.Add(attachment);

                    // Run the email sending logic on a background thread
                    await Task.Run(() => SendEmailWithAttachment(smtpClient, message2));

                    // The email sending is complete; you can now update UI or perform other tasks
                    MessageBox.Show("Report Successfully Sent", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);

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

        #endregion Export DataGrid

       

        private async void btnUpdateReport_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUserRole1.Equals("Operator"))
            {
                MessageBox.Show("Only Administrator can Delete DT Information", "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            else
            {
                ReportDt selectedDt = datagridReport.SelectedItem as ReportDt;

                if (selectedDt != null)
                {
                    ReportEditAll reportEditWindow = new ReportEditAll(selectedDt, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);


                    reportEditWindow.ShowDialog();


                }
            }

            await ReadDatabase();
        }
    }
}

