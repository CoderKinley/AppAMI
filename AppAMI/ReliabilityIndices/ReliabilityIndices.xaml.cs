using System;
using System.Collections.Generic;
using System.Linq;
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
using MaterialDesignThemes.Wpf;
using System.Globalization;
using System.Threading;
using System.Data;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Net.Http;

namespace AppAMI.ReliabilityIndices
{
    /// <summary>
    /// Interaction logic for ReliabilityIndices.xaml
    /// </summary>
    public partial class ReliabilityIndices : UserControl
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

        public ReliabilityIndices(string CurrentUserId, string CurrentUserRole, string CurrentUserPassword, string CurrentUserName, string CurrentUserEmployeeId)
        {
            InitializeComponent();

            CurrentUserId1 = CurrentUserId;
            CurrentUserRole1 = CurrentUserRole;
            CurrentUserPassword1 = CurrentUserPassword;
            CurrentUserName1 = CurrentUserName;
            CurrentUserEmployeeId1 = CurrentUserEmployeeId;

            dtPickerStart.DisplayDateEnd = DateTime.Today;
            dtPickerEnd.DisplayDateEnd = DateTime.Today;

            DateTime twelveMonthsAgo = currentDate.AddMonths(-12);
            dtPickerStart.SelectedDate = twelveMonthsAgo;
            dtPickerEnd.SelectedDate = DateTime.Today;

            grdEmailAddress.Visibility = Visibility.Collapsed;


            Task.Run(() => loadData());

            datagridReliabilityCountry.Visibility = Visibility.Collapsed;
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
            dtPickerStart.SelectedDate = twelveMonthsAgo;
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
                        var apiResponseList = JsonConvert.DeserializeObject<List<ReliabilityIndexClass>>(jsonResult);

                        if (apiResponseList != null && apiResponseList.Any())
                        {
                            // After storing the information, make requests for billing data
                            GetBillingDataForStoredInfo(apiResponseList);
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

        private List<ReliabilityIndexClass> accumulatedBillingData = new List<ReliabilityIndexClass>();

        private async void GetBillingDataForStoredInfo(List<ReliabilityIndexClass> apiResponseList)
        {
            datagridReliability.ClearFilters();

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
                    datagridReliability.ItemsSource = accumulatedBillingData;

                    // Initialize totals and counts
                    long totalCustomerCount = 0;
                    long totalSustainInterruptionTimes = 0;
                    long totalMomentaryInterruptionTimes = 0;
                    int totalSeconds = 0;
                    double totalSaifi = 0;
                    double totalSaidi = 0;
                    double totalMaifi = 0;
                    int validRowCount = 0;

                    if (datagridReliability.View?.Records != null)
                    {
                        foreach (var data in accumulatedBillingData)
                        {
                            // Sum total_customer_count
                            if (!string.IsNullOrWhiteSpace(data.total_customer_count) && long.TryParse(data.total_customer_count, out long customerCount))
                            {
                                totalCustomerCount += customerCount;
                            }

                            // Sum total_sustain_interruption_times
                            if (!string.IsNullOrWhiteSpace(data.sustain_interruption_times) && long.TryParse(data.sustain_interruption_times, out long sustainInterruptionTimes))
                            {
                                totalSustainInterruptionTimes += sustainInterruptionTimes;
                            }

                            // Sum total_momentary_interruption_times
                            if (!string.IsNullOrWhiteSpace(data.momentary_interruption_times) && long.TryParse(data.momentary_interruption_times, out long momentaryInterruptionTimes))
                            {
                                totalMomentaryInterruptionTimes += momentaryInterruptionTimes;
                            }

                            // Sum total_sustain_interruption_duration_hrs
                            if (!string.IsNullOrWhiteSpace(data.sustain_interruption_duration_hrs))
                            {
                                var timeParts = data.sustain_interruption_duration_hrs.Split(':');
                                if (timeParts.Length == 3)
                                {
                                    if (int.TryParse(timeParts[0], out int Hours) &&
                                        int.TryParse(timeParts[1], out int Minutes) &&
                                        int.TryParse(timeParts[2], out int Seconds))
                                    {
                                        totalSeconds += Hours * 3600 + Minutes * 60 + Seconds;
                                    }
                                }
                            }

                            // Sum and count saifi, saidi, maifi
                            if (!string.IsNullOrWhiteSpace(data.saifi) && double.TryParse(data.saifi, out double saifi))
                            {
                                totalSaifi += saifi;
                            }

                            if (!string.IsNullOrWhiteSpace(data.saidi) && double.TryParse(data.saidi, out double saidi))
                            {
                                totalSaidi += saidi;
                            }

                            if (!string.IsNullOrWhiteSpace(data.maifi) && double.TryParse(data.maifi, out double maifi))
                            {
                                totalMaifi += maifi;
                            }

                            // Count valid rows
                            validRowCount++;
                        }


                    }


                    // Calculate averages
                    double averageSaifi = validRowCount > 0 ? totalSaifi / validRowCount : 0;
                    double averageSaidi = validRowCount > 0 ? totalSaidi / validRowCount : 0;
                    double averageMaifi = validRowCount > 0 ? totalMaifi / validRowCount : 0;

                    // Convert totalSeconds to a format of more than 24 hours
                    int hours = totalSeconds / 3600;
                    int minutes = (totalSeconds % 3600) / 60;
                    int seconds = totalSeconds % 60;
                    string formattedDuration = $"{hours:D2}:{minutes:D2}:{seconds:D2}";

                    // Update the labels with the calculated totals and averages
                    lblConsumerServed.Content = totalCustomerCount.ToString();
                    lblSusInterruptionTimes.Content = totalSustainInterruptionTimes.ToString();
                    lblInterruptionDuration.Content = formattedDuration;  // Format as HH:mm:ss
                    lblMomentaryInterruptionTimes.Content = totalMomentaryInterruptionTimes.ToString();
                    lblSaifi.Content = averageSaifi.ToString("F2");
                    lblSaidi.Content = averageSaidi.ToString("F2");
                    lblMaifi.Content = averageMaifi.ToString("F2");
                });




                SaveDataToTemporaryStorage(selectedDistrict, selectedEsd, dtPickerStart.SelectedDate.Value, dtPickerEnd.SelectedDate.Value, accumulatedBillingData);

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
                string apiUrl = $"http://103.234.126.43:3080/dtmeter/reliability/index/{dtId}";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON response to a single ReliabilityIndexClass object
                        ReliabilityIndexClass billingData = JsonConvert.DeserializeObject<ReliabilityIndexClass>(jsonResult);

                        if (billingData != null)
                        {
                            // Check if the dt_id matches
                            if (billingData.dt_id == dtId)
                            {
                                accumulatedBillingData.Add(billingData);
                            }
                            else
                            {
                                // Handle case where the dt_id does not match
                                MessageBox.Show($"No billing data found for {dtId}.");
                            }
                        }
                        else
                        {
                            // Handle case where billingData is null
                            MessageBox.Show($"Billing data for {dtId} is null or has unexpected structure.");
                        }
                    }
                    else
                    {
                        // Handle unsuccessful response
                        MessageBox.Show($"Failed to get billing data for {dtId}. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show($"Exception while getting billing data for {dtId}: {ex.Message}");
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

        private void SaveDataToTemporaryStorage(string district, string esd, DateTime startDate, DateTime endDate, List<ReliabilityIndexClass> data)
        {
            string key = $"{district}_{esd}_{startDate.ToString("yyyyMMdd")}_{endDate.ToString("yyyyMMdd")}";

            temporaryStorage[key] = data;
        }

        private void BtnViewData_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnViewData && btnViewData.Tag is ViewDataButtonInfo buttonInfo)
            {
                //MessageBox.Show($"Button Clicked: {buttonInfo.District}, {buttonInfo.ESD}, {buttonInfo.StartDate}, {buttonInfo.EndDate}");

                List<ReliabilityIndexClass> temporaryData = GetTemporaryData(buttonInfo.District, buttonInfo.ESD, buttonInfo.StartDate, buttonInfo.EndDate);

                if (temporaryData != null)
                {
                    //MessageBox.Show($"Fetched Data Count: {temporaryData.Count}");
                    datagridReliability.ItemsSource = temporaryData;

                    // Initialize totals and counts
                    long totalCustomerCount = 0;
                    long totalSustainInterruptionTimes = 0;
                    long totalMomentaryInterruptionTimes = 0;
                    int totalSeconds = 0;
                    double totalSaifi = 0;
                    double totalSaidi = 0;
                    double totalMaifi = 0;
                    int validRowCount = 0;

                    foreach (var data in temporaryData)
                    {
                        // Sum total_customer_count
                        if (!string.IsNullOrWhiteSpace(data.total_customer_count) && long.TryParse(data.total_customer_count, out long customerCount))
                        {
                            totalCustomerCount += customerCount;
                        }

                        // Sum total_sustain_interruption_times
                        if (!string.IsNullOrWhiteSpace(data.total_sustain_interruption_times) && long.TryParse(data.total_sustain_interruption_times, out long sustainInterruptionTimes))
                        {
                            totalSustainInterruptionTimes += sustainInterruptionTimes;
                        }

                        // Sum total_momentary_interruption_times
                        if (!string.IsNullOrWhiteSpace(data.total_momentary_interruption_times) && long.TryParse(data.total_momentary_interruption_times, out long momentaryInterruptionTimes))
                        {
                            totalMomentaryInterruptionTimes += momentaryInterruptionTimes;
                        }

                        // Sum total_sustain_interruption_duration_hrs
                        if (!string.IsNullOrWhiteSpace(data.total_sustain_interruption_duration_hrs))
                        {
                            var timeParts = data.total_sustain_interruption_duration_hrs.Split(':');
                            if (timeParts.Length == 3)
                            {
                                if (int.TryParse(timeParts[0], out int Hours) &&
                                    int.TryParse(timeParts[1], out int Minutes) &&
                                    int.TryParse(timeParts[2], out int Seconds))
                                {
                                    totalSeconds += Hours * 3600 + Minutes * 60 + Seconds;
                                }
                            }
                        }

                        // Sum and count saifi, saidi, maifi
                        if (!string.IsNullOrWhiteSpace(data.saifi) && double.TryParse(data.saifi, out double saifi))
                        {
                            totalSaifi += saifi;
                        }

                        if (!string.IsNullOrWhiteSpace(data.saidi) && double.TryParse(data.saidi, out double saidi))
                        {
                            totalSaidi += saidi;
                        }

                        if (!string.IsNullOrWhiteSpace(data.maifi) && double.TryParse(data.maifi, out double maifi))
                        {
                            totalMaifi += maifi;
                        }

                        // Count valid rows
                        validRowCount++;
                    }

                    // Calculate averages
                    double averageSaifi = validRowCount > 0 ? totalSaifi / validRowCount : 0;
                    double averageSaidi = validRowCount > 0 ? totalSaidi / validRowCount : 0;
                    double averageMaifi = validRowCount > 0 ? totalMaifi / validRowCount : 0;

                    // Convert totalSeconds to a format of more than 24 hours
                    int hours = totalSeconds / 3600;
                    int minutes = (totalSeconds % 3600) / 60;
                    int seconds = totalSeconds % 60;
                    string formattedDuration = $"{hours:D2}:{minutes:D2}:{seconds:D2}";

                    // Update the labels with the calculated totals and averages
                    lblConsumerServed.Content = totalCustomerCount.ToString();
                    lblSusInterruptionTimes.Content = totalSustainInterruptionTimes.ToString();
                    lblInterruptionDuration.Content = formattedDuration;  // Format as HH:mm:ss
                    lblMomentaryInterruptionTimes.Content = totalMomentaryInterruptionTimes.ToString();
                    lblSaifi.Content = averageSaifi.ToString("F2");
                    lblSaidi.Content = averageSaidi.ToString("F2");
                    lblMaifi.Content = averageMaifi.ToString("F2");
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

        private List<ReliabilityIndexClass> GetTemporaryData(string district, string esd, DateTime startDate, DateTime endDate)
        {
            string key = $"{district}_{esd}_{startDate.ToString("yyyyMMdd")}_{endDate.ToString("yyyyMMdd")}";

            if (temporaryStorage.TryGetValue(key, out List<ReliabilityIndexClass> temporaryData))
            {
                return temporaryData;
            }
            else
            {
                return new List<ReliabilityIndexClass>();
            }
        }

        private Dictionary<string, List<ReliabilityIndexClass>> temporaryStorage = new Dictionary<string, List<ReliabilityIndexClass>>();

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
                    List<ReliabilityIndexClass> temporaryData = GetTemporaryData(firstButtonInfo.District, firstButtonInfo.ESD, firstButtonInfo.StartDate, firstButtonInfo.EndDate);

                    datagridReliability.ItemsSource = temporaryData;
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
                StackPanel existingContentPanel = null;

                if (selectedTreeViewItem.Parent is TreeViewItem selectedDistrictItem && selectedDistrictItem.Tag != null && selectedTreeViewItem.Tag != null)
                {
                    selectedDistrict = (string)selectedDistrictItem.Tag;
                    selectedEsd = (string)selectedTreeViewItem.Tag;
                    selectedPlaceForData = "DataEsd";

                    buttonText = (string)selectedTreeViewItem.Header;
                    key = $"{selectedDistrict}_{selectedEsd}_{dtPickerStart.SelectedDate.Value.ToString("yyyyMMdd")}_{dtPickerEnd.SelectedDate.Value.ToString("yyyyMMdd")}";

                    existingContentPanel = stackCurrentData.Children.OfType<StackPanel>().FirstOrDefault(panel => panel.Tag != null && (string)panel.Tag == key);

                }
                else if (e.NewValue is TreeViewItem districtItem)
                {
                    selectedDistrict = (string)districtItem.Tag;
                    selectedPlaceForData = "DataDistrict";
                    selectedEsd = "Multiple ESD";

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

        // Create a list to store selected items
        private List<SelectedItemInfo> selectedItemsList = new List<SelectedItemInfo>();


        #endregion add tree view items




        #region Read DataBase

        private  void dtPicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;


            //if (cbMultipleData.IsChecked == false)
            //{

            //    if (!dtPickerEnd.SelectedDate.HasValue)
            //    {
            //        MessageBox.Show("Please insert the end date.", "End Date Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            //        return;
            //    }

            //    if (dtPickerEnd.SelectedDate.HasValue && dtPickerEnd.SelectedDate > dtPickerStart.SelectedDate)
            //    {
            //        if (!string.IsNullOrEmpty(selectedDistrict) || !string.IsNullOrEmpty(selectedEsd))
            //        {
            //            await ReadDatabase();
            //        }

            //        else
            //        {
            //            MessageBox.Show("Please make sure the District or ESD is selected.", "District Or ESD Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            //        }
            //    }


            //    else
            //    {
            //        MessageBox.Show("Please make sure the date range is valid.", "Invalid Date Range", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    }
            //}

            //else
            //{
            //    if (dtPickerEnd.SelectedDate < dtPickerStart.SelectedDate)
            //    {
            //        MessageBox.Show("Please make sure the date range is valid.", "Invalid Date Range", MessageBoxButton.OK, MessageBoxImage.Warning);

            //    }
            //}


        }

        private  void dtPickerEnd_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;


            //if (cbMultipleData.IsChecked == false)
            //{
            //    if (!dtPickerStart.SelectedDate.HasValue)
            //    {
            //        MessageBox.Show("Please insert the start date.", "Start Date Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            //        return;
            //    }

            //    if (dtPickerEnd.SelectedDate.HasValue && dtPickerEnd.SelectedDate > dtPickerStart.SelectedDate)
            //    {
            //        if (!string.IsNullOrEmpty(selectedDistrict) || !string.IsNullOrEmpty(selectedEsd))
            //        {
            //            await ReadDatabase();
            //        }

            //        else
            //        {
            //            MessageBox.Show("Please make sure the District or ESD is selected.", "District Or ESD Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Please make sure the date range is valid.", "Invalid Date Range", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    }

            //}
            //else
            //{
            //    if (dtPickerEnd.SelectedDate < dtPickerStart.SelectedDate)
            //    {
            //        MessageBox.Show("Please make sure the date range is valid.", "Invalid Date Range", MessageBoxButton.OK, MessageBoxImage.Warning);

            //    }
            //}

        }


        //private async Task ReadDatabase()
        //{
        //    datagridReliability.ClearFilters();

        //    progressLogin.Visibility = Visibility.Visible;

        //    string url = string.Empty;

        //    if (!string.IsNullOrWhiteSpace(dtPickerStart.Text) || !string.IsNullOrWhiteSpace(dtPickerEnd.Text))
        //    {
        //        try
        //        {
        //            using (WebClient web = new WebClient())
        //            {
        //                // string url = string.Format("http://103.234.126.43:3080/dtmeter/reliability/index/all/{0}", selectedDistrict);
        //                // string url = string.Format("http://103.234.126.43:3080/dtmeter/reliability/index/all/{0}/{1}", selectedDistrict, cbMonths.Text );

        //                if (cbDataCurrentYear.IsChecked == true)
        //                {
        //                    url = string.Format("http://103.234.126.43:3080/dtmeter/reliability/index/all/{0}", selectedDistrict);

        //                }

        //                else if (cbDataCurrentYear.IsChecked == false)
        //                {
        //                    url = string.Format("http://103.234.126.43:3080/dtmeter/reliability/index/all/{0}/{1}", selectedDistrict, cbMonths.Text);

        //                }

        //                string json = await web.DownloadStringTaskAsync(url);

        //                if (string.IsNullOrWhiteSpace(json))
        //                {
        //                    MessageBox.Show("No data found from the server.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //                    return;
        //                }

        //                List<ReliabilityIndexClass> allData = JsonConvert.DeserializeObject<List<ReliabilityIndexClass>>(json);

        //                if (allData == null)
        //                {
        //                    MessageBox.Show("Error parsing JSON data. Deserialized data is null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //                    return;
        //                }

        //                List<ReliabilityIndexClass> filteredDataPlace = new List<ReliabilityIndexClass>();


        //                if (selectedPlaceForData == "DataDistrict")
        //                {
        //                    filteredDataPlace = allData.Where(a => a?.district_code == selectedDistrict).ToList();
        //                    SaveDataToTemporaryStorage(selectedDistrict, selectedEsd, dtPickerStart.SelectedDate.Value, dtPickerEnd.SelectedDate.Value, filteredDataPlace);

        //                }
        //                else if (selectedPlaceForData == "DataEsd")
        //                {
        //                    filteredDataPlace = allData.Where(a => a?.esd_code == selectedEsd).ToList();
        //                    SaveDataToTemporaryStorage(selectedDistrict, selectedEsd, dtPickerStart.SelectedDate.Value, dtPickerEnd.SelectedDate.Value, filteredDataPlace);

        //                }

        //                await Dispatcher.InvokeAsync(() =>
        //                {
        //                    datagridReliability.ItemsSource = filteredDataPlace;



        //                    // Initialize totals and counts
        //                    long totalCustomerCount = 0;
        //                    long totalSustainInterruptionTimes = 0;
        //                    long totalMomentaryInterruptionTimes = 0;
        //                    int totalSeconds = 0;
        //                    double totalSaifi = 0;
        //                    double totalSaidi = 0;
        //                    double totalMaifi = 0;
        //                    int validRowCount = 0;

        //                    if (datagridReliability.View?.Records != null)
        //                    {
        //                        foreach (var data in allData)
        //                        {
        //                            // Sum total_customer_count
        //                            if (!string.IsNullOrWhiteSpace(data.total_customer_count) && long.TryParse(data.total_customer_count, out long customerCount))
        //                            {
        //                                totalCustomerCount += customerCount;
        //                            }

        //                            // Sum total_sustain_interruption_times
        //                            if (!string.IsNullOrWhiteSpace(data.sustain_interruption_times) && long.TryParse(data.sustain_interruption_times, out long sustainInterruptionTimes))
        //                            {
        //                                totalSustainInterruptionTimes += sustainInterruptionTimes;
        //                            }

        //                            // Sum total_momentary_interruption_times
        //                            if (!string.IsNullOrWhiteSpace(data.momentary_interruption_times) && long.TryParse(data.momentary_interruption_times, out long momentaryInterruptionTimes))
        //                            {
        //                                totalMomentaryInterruptionTimes += momentaryInterruptionTimes;
        //                            }

        //                            // Sum total_sustain_interruption_duration_hrs
        //                            if (!string.IsNullOrWhiteSpace(data.sustain_interruption_duration_hrs))
        //                            {
        //                                var timeParts = data.sustain_interruption_duration_hrs.Split(':');
        //                                if (timeParts.Length == 3)
        //                                {
        //                                    if (int.TryParse(timeParts[0], out int Hours) &&
        //                                        int.TryParse(timeParts[1], out int Minutes) &&
        //                                        int.TryParse(timeParts[2], out int Seconds))
        //                                    {
        //                                        totalSeconds += Hours * 3600 + Minutes * 60 + Seconds;
        //                                    }
        //                                }
        //                            }

        //                            // Sum and count saifi, saidi, maifi
        //                            if (!string.IsNullOrWhiteSpace(data.saifi) && double.TryParse(data.saifi, out double saifi))
        //                            {
        //                                totalSaifi += saifi;
        //                            }

        //                            if (!string.IsNullOrWhiteSpace(data.saidi) && double.TryParse(data.saidi, out double saidi))
        //                            {
        //                                totalSaidi += saidi;
        //                            }

        //                            if (!string.IsNullOrWhiteSpace(data.maifi) && double.TryParse(data.maifi, out double maifi))
        //                            {
        //                                totalMaifi += maifi;
        //                            }

        //                            // Count valid rows
        //                            validRowCount++;
        //                        }


        //                    }


        //                    // Calculate averages
        //                    double averageSaifi = validRowCount > 0 ? totalSaifi / validRowCount : 0;
        //                    double averageSaidi = validRowCount > 0 ? totalSaidi / validRowCount : 0;
        //                    double averageMaifi = validRowCount > 0 ? totalMaifi / validRowCount : 0;

        //                    // Convert totalSeconds to a format of more than 24 hours
        //                    int hours = totalSeconds / 3600;
        //                    int minutes = (totalSeconds % 3600) / 60;
        //                    int seconds = totalSeconds % 60;
        //                    string formattedDuration = $"{hours:D2}:{minutes:D2}:{seconds:D2}";

        //                    // Update the labels with the calculated totals and averages
        //                    lblConsumerServed.Content = totalCustomerCount.ToString();
        //                    lblSusInterruptionTimes.Content = totalSustainInterruptionTimes.ToString();
        //                    lblInterruptionDuration.Content = formattedDuration;  // Format as HH:mm:ss
        //                    lblMomentaryInterruptionTimes.Content = totalMomentaryInterruptionTimes.ToString();
        //                    lblSaifi.Content = averageSaifi.ToString("F2");
        //                    lblSaidi.Content = averageSaidi.ToString("F2");
        //                    lblMaifi.Content = averageMaifi.ToString("F2");
        //                });



        //            }
        //        }
        //        catch (WebException ex)
        //        {
        //            MessageBox.Show("No Data Found for the current date range Select Date range" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //        catch (JsonException ex)
        //        {
        //            MessageBox.Show("Error parsing JSON data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //        finally
        //        {
        //            progressLogin.Visibility = Visibility.Collapsed;
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Kindly Select Date Range", "Date Range Selection", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //}



        private async Task ReadDatabase()
        {
            datagridReliability.ClearFilters();
            progressLogin.Visibility = Visibility.Visible;

            try
            {
                string url = string.Empty;

                if (!string.IsNullOrWhiteSpace(dtPickerStart.Text) || !string.IsNullOrWhiteSpace(dtPickerEnd.Text))
                {
                    using (WebClient web = new WebClient())
                    {
                        if (cbDataCurrentYear.IsChecked == true)
                        {
                            url = string.Format("http://103.234.126.43:3080/dtmeter/reliability/index/all/{0}", selectedDistrict);
                        }
                        else
                        {
                            url = string.Format("http://103.234.126.43:3080/dtmeter/reliability/index/all/{0}/{1}", selectedDistrict, cbMonths.Text);
                        }

                        string json = await web.DownloadStringTaskAsync(url);

                        if (string.IsNullOrWhiteSpace(json))
                        {
                            MessageBox.Show("No data found from the server.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }

                        List<ReliabilityIndexClass> allData = JsonConvert.DeserializeObject<List<ReliabilityIndexClass>>(json);

                        if (allData == null)
                        {
                            MessageBox.Show("Error parsing JSON data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        List<ReliabilityIndexClass> filteredDataPlace = allData;

                        if (selectedPlaceForData == "DataDistrict")
                        {
                            filteredDataPlace = allData.Where(a => a?.district_code == selectedDistrict).ToList();
                            SaveDataToTemporaryStorage(selectedDistrict, selectedEsd, dtPickerStart.SelectedDate.Value, dtPickerEnd.SelectedDate.Value, filteredDataPlace);
                        }
                        else if (selectedPlaceForData == "DataEsd")
                        {
                            filteredDataPlace = allData.Where(a => a?.esd_code == selectedEsd).ToList();
                            SaveDataToTemporaryStorage(selectedDistrict, selectedEsd, dtPickerStart.SelectedDate.Value, dtPickerEnd.SelectedDate.Value, filteredDataPlace);
                        }

                        if (cbViewByFeeder.IsChecked == true)
                        {
                            var groupedData = filteredDataPlace
    .GroupBy(a => a.feeder_id)
    .Select(g =>
    {
        long totalCustomerCount = g.Sum(x => long.TryParse(x.total_customer_count, out long count) ? count : 0);
        long totalSustainInterruptionTimes = g.Sum(x => long.TryParse(x.sustain_interruption_times, out long times) ? times : 0);
        long totalMomentaryInterruptionTimes = g.Sum(x => long.TryParse(x.momentary_interruption_times, out long mTimes) ? mTimes : 0);

        // Parse and sum durations in hours
        double totalSustainInterruptionDurationHrs = g.Sum(x =>
            TimeSpan.TryParse(x.sustain_interruption_duration_hrs, out TimeSpan duration)
            ? duration.TotalHours
            : 0);

        return new ReliabilityIndexClass
        {
            feeder_id = g.Key,
            esd_name = g.FirstOrDefault()?.esd_name ?? string.Empty,  // Keep esd_name consistent for the group
            total_customer_count = totalCustomerCount.ToString(),
            sustain_interruption_times = totalSustainInterruptionTimes.ToString(),
            momentary_interruption_times = totalMomentaryInterruptionTimes.ToString(),
            sustain_interruption_duration_hrs = totalSustainInterruptionDurationHrs.ToString("F2"), // Keep hours formatted to 2 decimal places
            saifi = totalSustainInterruptionTimes > 0 ? (totalCustomerCount / (double)totalSustainInterruptionTimes).ToString("F4") : "0.0000",
            saidi = totalSustainInterruptionDurationHrs > 0 ? (totalCustomerCount / totalSustainInterruptionDurationHrs).ToString("F4") : "0.0000",
            maifi = totalMomentaryInterruptionTimes > 0 ? (totalCustomerCount / (double)totalMomentaryInterruptionTimes).ToString("F4") : "0.0000",
            dt_id = "N/A"
        };
    })
    .ToList();


                            filteredDataPlace = groupedData;
                        }

                        await Dispatcher.InvokeAsync(() =>
                        {
                            datagridReliability.ItemsSource = filteredDataPlace;




                            // Initialize totals and counts
                            long totalCustomerCount = 0;
                            long totalSustainInterruptionTimes = 0;
                            long totalMomentaryInterruptionTimes = 0;
                            int totalSeconds = 0;
                            double totalSaifi = 0;
                            double totalSaidi = 0;
                            double totalMaifi = 0;
                            int validRowCount = 0;

                            if (datagridReliability.View?.Records != null)
                            {
                                foreach (var data in allData)
                                {
                                    // Sum total_customer_count
                                    if (!string.IsNullOrWhiteSpace(data.total_customer_count) && long.TryParse(data.total_customer_count, out long customerCount))
                                    {
                                        totalCustomerCount += customerCount;
                                    }

                                    // Sum total_sustain_interruption_times
                                    if (!string.IsNullOrWhiteSpace(data.sustain_interruption_times) && long.TryParse(data.sustain_interruption_times, out long sustainInterruptionTimes))
                                    {
                                        totalSustainInterruptionTimes += sustainInterruptionTimes;
                                    }

                                    // Sum total_momentary_interruption_times
                                    if (!string.IsNullOrWhiteSpace(data.momentary_interruption_times) && long.TryParse(data.momentary_interruption_times, out long momentaryInterruptionTimes))
                                    {
                                        totalMomentaryInterruptionTimes += momentaryInterruptionTimes;
                                    }

                                    // Sum total_sustain_interruption_duration_hrs
                                    if (!string.IsNullOrWhiteSpace(data.sustain_interruption_duration_hrs))
                                    {
                                        var timeParts = data.sustain_interruption_duration_hrs.Split(':');
                                        if (timeParts.Length == 3)
                                        {
                                            if (int.TryParse(timeParts[0], out int Hours) &&
                                                int.TryParse(timeParts[1], out int Minutes) &&
                                                int.TryParse(timeParts[2], out int Seconds))
                                            {
                                                totalSeconds += Hours * 3600 + Minutes * 60 + Seconds;
                                            }
                                        }
                                    }

                                    // Sum and count saifi, saidi, maifi
                                    if (!string.IsNullOrWhiteSpace(data.saifi) && double.TryParse(data.saifi, out double saifi))
                                    {
                                        totalSaifi += saifi;
                                    }

                                    if (!string.IsNullOrWhiteSpace(data.saidi) && double.TryParse(data.saidi, out double saidi))
                                    {
                                        totalSaidi += saidi;
                                    }

                                    if (!string.IsNullOrWhiteSpace(data.maifi) && double.TryParse(data.maifi, out double maifi))
                                    {
                                        totalMaifi += maifi;
                                    }

                                    // Count valid rows
                                    validRowCount++;
                                }


                            }


                            // Calculate averages
                            double averageSaifi = validRowCount > 0 ? totalSaifi / validRowCount : 0;
                            double averageSaidi = validRowCount > 0 ? totalSaidi / validRowCount : 0;
                            double averageMaifi = validRowCount > 0 ? totalMaifi / validRowCount : 0;

                            // Convert totalSeconds to a format of more than 24 hours
                            int hours = totalSeconds / 3600;
                            int minutes = (totalSeconds % 3600) / 60;
                            int seconds = totalSeconds % 60;
                            string formattedDuration = $"{hours:D2}:{minutes:D2}:{seconds:D2}";

                            // Update the labels with the calculated totals and averages
                            lblConsumerServed.Content = totalCustomerCount.ToString();
                            lblSusInterruptionTimes.Content = totalSustainInterruptionTimes.ToString();
                            lblInterruptionDuration.Content = formattedDuration;  // Format as HH:mm:ss
                            lblMomentaryInterruptionTimes.Content = totalMomentaryInterruptionTimes.ToString();
                            lblSaifi.Content = averageSaifi.ToString("F2");
                            lblSaidi.Content = averageSaidi.ToString("F2");
                            lblMaifi.Content = averageMaifi.ToString("F2");
                        });
                    }
                }
                else
                {
                    MessageBox.Show("Kindly select a date range.", "Date Range Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressLogin.Visibility = Visibility.Collapsed;
            }
        }

        // Helper methods remain the same

        // Helper method to parse and sum durations (hh:mm:ss) in seconds
        private int ParseDuration(string duration)
        {
            if (string.IsNullOrWhiteSpace(duration)) return 0;

            var timeParts = duration.Split(':');
            if (timeParts.Length == 3 &&
                int.TryParse(timeParts[0], out int hours) &&
                int.TryParse(timeParts[1], out int minutes) &&
                int.TryParse(timeParts[2], out int seconds))
            {
                return hours * 3600 + minutes * 60 + seconds;
            }
            return 0;
        }

        // Helper method to format total seconds into hh:mm:ss
        private string FormatDuration(int totalSeconds)
        {
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;
            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }




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

        #endregion Read DataBase

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


            ExcelEngine excelEngine;
            if (cbNationData.IsChecked == true)
            {
                 excelEngine = datagridReliabilityCountry.ExportToExcel(datagridReliabilityCountry.View, options);

            }
            else
            {
                 excelEngine = datagridReliability.ExportToExcel(datagridReliability.View, options);

            }
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                FilterIndex = 2,
                Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx",
                FileName = "Reliability Indices_" + selectedEsd + ".xlsx"

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

            var excelEngine = datagridReliability.ExportToExcel(datagridReliability.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV Files(*.csv)|*.csv",
                FileName = "Billing_" + selectedEsd + " " + ".csv"

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
            var document = datagridReliability.ExportToPdf();
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Files(*.pdf)|*.pdf",
                FileName = "Billing_" + selectedEsd + " " + ".pdf"

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
                    Subject = "Reliability Indices",
                    To = { new MailAddress(txtEmailAdress.Text) },
                    Body = "<html><body> Kindly find the attached Reliability Indices</body></html>",
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
                    var excelEngine = datagridReliability.ExportToExcel(datagridReliability.View, options);
                    var workBook = excelEngine.Excel.Workbooks[0];

                    // Save the Excel file to a temporary location
                    string tempFilePath = Path.GetTempFileName();
                    workBook.SaveAs(tempFilePath);
                    workBook.Close();

                    // Attach the Excel file to the email
                    Attachment attachment = new Attachment(tempFilePath);
                    attachment.ContentType.MediaType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    attachment.Name = "Reliability_" + selectedEsd + "_From_" + dtPickerStart.Text + "_To_" + dtPickerEnd.Text + ".xlsx";
                    message2.Attachments.Add(attachment);

                    // Run the email sending logic on a background thread
                    await Task.Run(() => SendEmailWithAttachment(smtpClient, message2));

                    // The email sending is complete; you can now update UI or perform other tasks
                    MessageBox.Show("Reliability Indices Successfully Sent", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);

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

       
        private async  void cbNationData_Checked(object sender, RoutedEventArgs e)
        {
            datagridReliability.Visibility = Visibility.Collapsed;
            datagridReliabilityCountry.Visibility = Visibility.Visible;
            grdHideButton.Visibility = Visibility.Visible;

            //await getReliabilityBhutan();
            await ReadDatabase1();

        }

        private  void cbDataCurrentYear_Checked(object sender, RoutedEventArgs e)
        {
            cbMonths.SelectedIndex = -1;
        }

        private void cbDataCurrentYear_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void cbNationData_Unchecked(object sender, RoutedEventArgs e)
        {
            datagridReliability.Visibility = Visibility.Visible;
            datagridReliabilityCountry.Visibility = Visibility.Collapsed;
            grdHideButton.Visibility = Visibility.Collapsed ;

        }

        //private async Task ReadDatabase1()
        //{
        //    datagridReliability.ClearFilters();

        //    progressLogin.Visibility = Visibility.Visible;

        //    if (!string.IsNullOrWhiteSpace(dtPickerStart.Text) || !string.IsNullOrWhiteSpace(dtPickerEnd.Text))
        //    {
        //        try
        //        {
        //            using (WebClient web = new WebClient())
        //            {
        //                string url = string.Format("http://103.234.126.43:3080/dtmeter/reliability/realtime-summary");

        //                // string url = string.Format("http://119.2.105.142:3080/dtmeter/reliability/realtime-summary");

        //                string json = await web.DownloadStringTaskAsync(url);

        //                if (string.IsNullOrWhiteSpace(json))
        //                {
        //                    MessageBox.Show("No data found from the server.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //                    return;
        //                }

        //                List<ReliabilityIndexClass> allData = JsonConvert.DeserializeObject<List<ReliabilityIndexClass>>(json);

        //                if (allData == null)
        //                {
        //                    MessageBox.Show("Error parsing JSON data. Deserialized data is null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //                    return;
        //                }

        //                // Bind the data to the DataGrid
        //                _ = Dispatcher.InvokeAsync(() =>
        //                {
        //                    datagridReliabilityCountry.ItemsSource = allData;
        //                });

        //                long totalCustomerCount = allData
        //                    .Where(x => long.TryParse(x.total_customer_count, out _))  
        //                    .Sum(x => long.Parse(x.total_customer_count));            

        //                long totalSustainInterruptionTimes = allData
        //                    .Where(x => long.TryParse(x.total_sustain_interruption_times, out _))  
        //                    .Sum(x => long.Parse(x.total_sustain_interruption_times));

        //                long totalDuration = allData
        //                .Where(x => long.TryParse(x.total_sustain_interruption_duration_hrs, out _))
        //                .Sum(x => long.Parse(x.total_sustain_interruption_duration_hrs));


        //                long totalMomentaryInterruptionTimes = allData
        //                    .Where(x => long.TryParse(x.total_momentary_interruption_times, out _))  
        //                    .Sum(x => long.Parse(x.total_momentary_interruption_times));


        //                long totalSaifi = allData
        //                   .Where(x => long.TryParse(x.saifi, out _))
        //                   .Sum(x => long.Parse(x.saifi));

        //                long totalSaidi = allData
        //                   .Where(x => long.TryParse(x.saidi, out _))
        //                   .Sum(x => long.Parse(x.saidi));

        //                long totalMaidi = allData
        //                   .Where(x => long.TryParse(x.maifi, out _))
        //                   .Sum(x => long.Parse(x.maifi));
        //                // Update the labels with the calculated totals
        //                lblConsumerServed.Content = totalCustomerCount.ToString();
        //                lblSusInterruptionTimes.Content = totalSustainInterruptionTimes.ToString();
        //                lblInterruptionDuration.Content = totalDuration.ToString();
        //                lblMomentaryInterruptionTimes.Content = totalMomentaryInterruptionTimes.ToString();
        //                lblSaifi.Content = totalSaifi.ToString();
        //                lblSaidi.Content = totalSaidi.ToString();
        //                lblMaifi.Content = totalMaidi.ToString();
        //            }
        //        }
        //        catch (WebException ex)
        //        {
        //            MessageBox.Show("Error fetching data from the server: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //        catch (JsonException ex)
        //        {
        //            MessageBox.Show("Error parsing JSON data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //        finally
        //        {
        //            progressLogin.Visibility = Visibility.Collapsed;
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Kindly Select Date Range", "Date Range Selection", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //}


        private async Task ReadDatabase1()
        {
            datagridReliability.ClearFilters();

            progressLogin.Visibility = Visibility.Visible;

            string url = string.Empty;

            if (!string.IsNullOrWhiteSpace(dtPickerStart.Text) || !string.IsNullOrWhiteSpace(dtPickerEnd.Text))
            {
                try
                {
                    using (WebClient web = new WebClient())
                    {

                        if (cbDataCurrentYear.IsChecked== true)
                        {
                              url = string.Format("http://103.234.126.43:3080/dtmeter/reliability/realtime-summary");

                        }

                        else if(cbDataCurrentYear.IsChecked == false)
                        {
                             url = string.Format("http://103.234.126.43:3080/dtmeter/reliability/realtime-summary/{0}", cbMonths.Text);

                        }



                        string json = await web.DownloadStringTaskAsync(url);

                        if (string.IsNullOrWhiteSpace(json))
                        {
                            MessageBox.Show("No data found from the server.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }

                        List<ReliabilityIndexClass> allData = JsonConvert.DeserializeObject<List<ReliabilityIndexClass>>(json);

                        if (allData == null)
                        {
                            MessageBox.Show("Error parsing JSON data. Deserialized data is null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Bind the data to the DataGrid
                        _ = Dispatcher.InvokeAsync(() =>
                        {
                            datagridReliabilityCountry.ItemsSource = allData;

                            // Initialize totals and counts
                            long totalCustomerCount = 0;
                            long totalSustainInterruptionTimes = 0;
                            long totalMomentaryInterruptionTimes = 0;
                            int totalSeconds = 0;
                            double totalSaifi = 0;
                            double totalSaidi = 0;
                            double totalMaifi = 0;
                            int validRowCount = 0;

                            foreach (var data in allData)
                            {
                                // Sum total_customer_count
                                if (!string.IsNullOrWhiteSpace(data.total_customer_count) && long.TryParse(data.total_customer_count, out long customerCount))
                                {
                                    totalCustomerCount += customerCount;
                                }

                                // Sum total_sustain_interruption_times
                                if (!string.IsNullOrWhiteSpace(data.total_sustain_interruption_times) && long.TryParse(data.total_sustain_interruption_times, out long sustainInterruptionTimes))
                                {
                                    totalSustainInterruptionTimes += sustainInterruptionTimes;
                                }

                                // Sum total_momentary_interruption_times
                                if (!string.IsNullOrWhiteSpace(data.total_momentary_interruption_times) && long.TryParse(data.total_momentary_interruption_times, out long momentaryInterruptionTimes))
                                {
                                    totalMomentaryInterruptionTimes += momentaryInterruptionTimes;
                                }

                                // Sum total_sustain_interruption_duration_hrs
                                if (!string.IsNullOrWhiteSpace(data.total_sustain_interruption_duration_hrs))
                                {
                                    var timeParts = data.total_sustain_interruption_duration_hrs.Split(':');
                                    if (timeParts.Length == 3)
                                    {
                                        if (int.TryParse(timeParts[0], out int Hours) &&
                                            int.TryParse(timeParts[1], out int Minutes) &&
                                            int.TryParse(timeParts[2], out int Seconds))
                                        {
                                            totalSeconds += Hours * 3600 + Minutes * 60 + Seconds;
                                        }
                                    }
                                }

                                // Sum and count saifi, saidi, maifi
                                if (!string.IsNullOrWhiteSpace(data.saifi) && double.TryParse(data.saifi, out double saifi))
                                {
                                    totalSaifi += saifi;
                                }

                                if (!string.IsNullOrWhiteSpace(data.saidi) && double.TryParse(data.saidi, out double saidi))
                                {
                                    totalSaidi += saidi;
                                }

                                if (!string.IsNullOrWhiteSpace(data.maifi) && double.TryParse(data.maifi, out double maifi))
                                {
                                    totalMaifi += maifi;
                                }

                                // Count valid rows
                                validRowCount++;
                            }

                            // Calculate averages
                            double averageSaifi = validRowCount > 0 ? totalSaifi / validRowCount : 0;
                            double averageSaidi = validRowCount > 0 ? totalSaidi / validRowCount : 0;
                            double averageMaifi = validRowCount > 0 ? totalMaifi / validRowCount : 0;

                            // Convert totalSeconds to a format of more than 24 hours
                            int hours = totalSeconds / 3600;
                            int minutes = totalSeconds % 3600 / 60;
                            int seconds = totalSeconds % 60;
                            string formattedDuration = $"{hours:D2}:{minutes:D2}:{seconds:D2}";

                            // Update the labels with the calculated totals and averages
                            lblConsumerServed.Content = totalCustomerCount.ToString();
                            lblSusInterruptionTimes.Content = totalSustainInterruptionTimes.ToString();
                            lblInterruptionDuration.Content = formattedDuration;  // Format as HH:mm:ss
                            lblMomentaryInterruptionTimes.Content = totalMomentaryInterruptionTimes.ToString();
                            lblSaifi.Content = averageSaifi.ToString("F2");
                            lblSaidi.Content = averageSaidi.ToString("F2");
                            lblMaifi.Content = averageMaifi.ToString("F2");
                        });

                        
                    }
                }
                catch (WebException ex)
                {
                    MessageBox.Show("Error fetching data from the server: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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



        //private async Task ReadDatabase1()
        //{
        //    datagridReliability.ClearFilters();

        //    progressLogin.Visibility = Visibility.Visible;

        //    if (!string.IsNullOrWhiteSpace(dtPickerStart.Text) || !string.IsNullOrWhiteSpace(dtPickerEnd.Text))
        //    {
        //        try
        //        {
        //            using (WebClient web = new WebClient())
        //            {
        //                string url = string.Format("http://103.234.126.43:3080/dtmeter/reliability/realtime-summary");

        //                //  string url = string.Format("http://119.2.105.142:3080/dtmeter/reliability/realtime-summary");

        //                string json = await web.DownloadStringTaskAsync(url);

        //                if (string.IsNullOrWhiteSpace(json))
        //                {
        //                    MessageBox.Show("No data found from the server.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //                    return;
        //                }

        //                List<ReliabilityIndexClass> allData = JsonConvert.DeserializeObject<List<ReliabilityIndexClass>>(json);

        //                if (allData == null)
        //                {
        //                    MessageBox.Show("Error parsing JSON data. Deserialized data is null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //                    return;
        //                }

        //                _ = Dispatcher.InvokeAsync(() =>
        //                {
        //                    datagridReliabilityCountry.ItemsSource = allData;

        //                });


        //            }
        //        }
        //        catch (WebException ex)
        //        {
        //            MessageBox.Show("Error fetching data from the server: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //        catch (JsonException ex)
        //        {
        //            MessageBox.Show("Error parsing JSON data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //        finally
        //        {
        //            progressLogin.Visibility = Visibility.Collapsed;
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Kindly Select Date Range", "Date Range Selection", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //}



        private async Task getReliabilityBhutan()
        {
            //progressMeterData.Visibility = Visibility.Visible;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    //string url = $"http://103.234.126.43:3080/reliability/grand-totals/bhutan";
                    string url = $"http://103.234.126.43:3080/reliability/grand-totals/bhutan/{0}";


                    string json = await httpClient.GetStringAsync(url);

                    // Assuming the JSON is a single object, not an array
                    ReliabilityIndexClass dtMeter = JsonConvert.DeserializeObject<ReliabilityIndexClass>(json);

                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (dtMeter != null)
                        {
                            lblConsumerServed.Content = dtMeter.total_customer_served;
                            lblInterruptionDuration.Content = dtMeter.sum_of_sustained_customer_interruption_duration_hrs;
                            lblSusInterruptionTimes.Content = dtMeter.sum_of_sustained_customer_interruption_times;
                            lblMomentaryInterruptionTimes.Content = dtMeter.sum_of_momentary_customer_interruption_times;

                            lblSaifi.Content = dtMeter.saifi;
                            lblSaidi.Content = dtMeter.saidi;
                            lblMaifi.Content = dtMeter.maifi;

                        }
                        else
                        {
                            MessageBox.Show("Invalid DT Id", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    });
                }
            }
            catch (HttpRequestException)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            catch (JsonException jsonEx)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error parsing JSON data: {jsonEx.Message}", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            catch (Exception ex)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                //progressMeterData.Visibility = Visibility.Collapsed;
            }
        }

        private void cbMonths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbDataCurrentYear.IsChecked = false;
        }

        private void cbViewByFeeder_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void cbViewByFeeder_Unchecked(object sender, RoutedEventArgs e)
        {

        }

      
    }
}
