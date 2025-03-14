using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using Microsoft.Maps.MapControl.WPF;
using AppAMI.Classes;
using System.Linq;
using System;
using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppAMI.GIS
{
    /// <summary>
    /// Interaction logic for GIS.xaml
    /// </summary>
    public partial class GIS : UserControl
    {
        List<DT> fetchedData;
        List<DT> allFetchedData = new List<DT>();
        private List<string> clickedDtIds = new List<string>();

        Location[] locations;
        UserControl usc = null;

        string selectedEsd;

        string selectedDistrict;
        private string selectedPlaceForData;


        string CurrentUserId1;
        string CurrentUserRole1;
        string CurrentUserPassword1;
        string CurrentUserName1;
        string CurrentUserEmployeeId1;

        string DTId1;

        string url;

        private const double MaxZoomLevel = 18.0;
        private const double MinZoomLevel = 8.0;

        public GIS(string CurrentUserId, string CurrentUserRole, string CurrentUserPassword, string CurrentUserName, string CurrentUserEmployeeId)
        {
            InitializeComponent();


            CurrentUserId1 = CurrentUserId;
            CurrentUserRole1 = CurrentUserRole;
            CurrentUserPassword1 = CurrentUserPassword;
            CurrentUserName1 = CurrentUserName;
            CurrentUserEmployeeId1 = CurrentUserEmployeeId;


            txtDtIdSearch.Text = "Enter DT ID";
            txtDtIdSearch.Foreground = new SolidColorBrush(Colors.Gray);

            Task.Run(() => loadData());

            myMap.ViewChangeEnd += MyMap_ViewChangeEnd;

            stkAllData.Visibility = Visibility.Collapsed;

            //ShowAllData();
        }

        

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

        private Dictionary<string, MapLayer> pushpinLayers = new Dictionary<string, MapLayer>();


        private void BtnViewData_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btnViewData && btnViewData.Tag is ViewDataButtonInfo buttonInfo)
            {


                selectedDistrict = buttonInfo.District;
                selectedEsd = buttonInfo.ESD;

                //if (pushpinLayers.TryGetValue(GetContentPanelKey(selectedDistrict, selectedEsd), out var existingPushpinLayer))
                //{
                //    // Show existing pushpin layer
                //    existingPushpinLayer.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    // Create and show a new pushpin layer
                //    MapLayer pushpinLayer = CreatePushpinLayer();
                //    pushpinLayers[GetContentPanelKey(selectedDistrict, selectedEsd)] = pushpinLayer;
                //    myMap.Children.Add(pushpinLayer);
                //}


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

                        if (pushpinLayers.TryGetValue(GetContentPanelKey(buttonInfo.District, buttonInfo.ESD), out var existingPushpinLayer))
                        {
                            // Hide the pushpin layer
                            existingPushpinLayer.Visibility = Visibility.Collapsed;
                        }
                    }
                }


                foreach (StackPanel existingPanel in stackCurrentData.Children.OfType<StackPanel>().Where(panel => panel != contentPanel))
                {
                    UpdateContentPanelBackground(existingPanel, isSelected: false);
                }

                if (stackCurrentData.Children.Count > 0 && stackCurrentData.Children[0] is StackPanel remainingContentPanel)
                {
                    UpdateContentPanelBackground(remainingContentPanel, isSelected: true);
                    selectedContentPanel = remainingContentPanel;
                }
            }
        }
   
        private async  void myTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            progressLogin.Visibility = Visibility.Visible;

            if (e.NewValue == null || !(e.NewValue is TreeViewItem selectedTreeViewItem))
            {
                return;
            }

            try
            {
                string buttonText;
                StackPanel existingContentPanel = null;


                if (selectedTreeViewItem.Parent is TreeViewItem selectedDistrictItem && selectedDistrictItem.Tag != null && selectedTreeViewItem.Tag != null)
                {
                    selectedDistrict = (string)selectedDistrictItem.Tag;
                    selectedEsd = (string)selectedTreeViewItem.Tag;
                    selectedPlaceForData = "DataEsd";

                    buttonText = (string)selectedTreeViewItem.Header;

                    string key = $"{selectedDistrict}_{selectedEsd}";

                    existingContentPanel = stackCurrentData.Children.OfType<StackPanel>().FirstOrDefault(panel => panel.Tag != null && (string)panel.Tag == key);

                    url = string.Format("http://103.234.126.43:3080/dtmeter/district/{0}/{1}", selectedDistrictItem.Tag, selectedTreeViewItem.Tag);
                }

                else if (e.NewValue is TreeViewItem districtItem)
                {
                    selectedDistrict = (string)districtItem.Tag;
                    selectedPlaceForData = "DataDistrict";
                    selectedEsd = "Multiple ESD";

                    buttonText = (string)districtItem.Header;
                    string key = $"{selectedDistrict}_{selectedEsd}";

                    existingContentPanel = stackCurrentData.Children.OfType<StackPanel>().FirstOrDefault(panel => panel.Tag != null && (string)panel.Tag == key);

                    url = string.Format("http://103.234.126.43:3080/dtmeter/dtinfo/all/{0}", districtItem.Tag);

                }

                else
                {
                    return;
                }

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

                    contentPanel.Tag = GetContentPanelKey(selectedDistrict, selectedEsd);

                    ViewDataButtonInfo buttonInfo = new ViewDataButtonInfo
                    {
                        District = selectedDistrict,
                        ESD = selectedEsd,

                    };

                    btnViewData.Tag = buttonInfo;
                    btnViewClose.Tag = buttonInfo;

                    btnViewData.Click += BtnViewData_Click;
                    btnViewClose.Click += BtnViewClose_Click;

                    selectedContentPanel = contentPanel;


                    if (pushpinLayers.TryGetValue(GetContentPanelKey(selectedDistrict, selectedEsd), out var existingPushpinLayer))
                    {
                        // Show existing pushpin layer
                        existingPushpinLayer.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        // Create and show a new pushpin layer
                        MapLayer pushpinLayer = await CreatePushpinLayerAsync();
                        pushpinLayers[GetContentPanelKey(selectedDistrict, selectedEsd)] = pushpinLayer;
                        myMap.Children.Add(pushpinLayer);
                    }
                }
            }

            finally
            {
                progressLogin.Visibility = Visibility.Collapsed;
            }

        }

        private async Task<MapLayer> CreatePushpinLayerAsync()
        {
            MapLayer pushpinLayer = new MapLayer();

            try
            {
                progressLogin.Visibility = Visibility.Visible;

                using (WebClient web = new WebClient())
                {
                    string json = web.DownloadString(url);

                    List<DT> fetchedData = JsonConvert.DeserializeObject<List<DT>>(json);

                    allFetchedData.AddRange(fetchedData);

                    if (fetchedData == null || !fetchedData.Any())
                    {
                        return pushpinLayer;
                    }

                    double maxTotalCustomerCount = fetchedData.Max(dt => double.TryParse(dt.total_customer_count, out var value) ? value : 0);

                    foreach (DT dtData in fetchedData)
                    {
                        if (dtData == null ||
                            string.IsNullOrEmpty(dtData.latitude) || string.IsNullOrEmpty(dtData.longitude) ||
                            string.IsNullOrEmpty(dtData.total_customer_count) || string.IsNullOrEmpty(dtData.location))
                        {
                            // Skip data with null or empty properties or null dtData
                            continue;
                        }

                        if (!double.TryParse(dtData.latitude, out double latitude) ||
                            !double.TryParse(dtData.longitude, out double longitude) ||
                            !double.TryParse(dtData.total_customer_count, out double totalCustomerCount))
                        {
                            // Skip invalid data
                            continue;
                        }

                        // Define your valid latitude and longitude range here
                        double minLatitude = -90.0;
                        double maxLatitude = 90.0;
                        double minLongitude = -180.0;
                        double maxLongitude = 180.0;

                        // Check if latitude and longitude are within the valid range
                        if (latitude < minLatitude || latitude > maxLatitude || longitude < minLongitude || longitude > maxLongitude)
                        {
                            // Skip this data point
                            continue;
                        }

                        double markerSizeFactor = 1 + 2 * (totalCustomerCount / maxTotalCustomerCount);
                        double markerWidth = 20 * markerSizeFactor;
                        double markerHeight = 20 * markerSizeFactor;

                        double iconSizeFactor = 1 + 2 * (totalCustomerCount / maxTotalCustomerCount);
                        double packIconSize = 20 * iconSizeFactor;

                        Button btn = new Button
                        {
                            Content = new PackIcon { Kind = PackIconKind.Circle, Height = packIconSize, Width = packIconSize },
                            Tag = (dtData.dt_id, dtData.feeder_id),
                            Background = new SolidColorBrush(Colors.Transparent),
                            VerticalContentAlignment = VerticalAlignment.Bottom,
                            Width = markerWidth * 1.2,
                            Height = markerHeight * 1.2,
                            BorderBrush = new SolidColorBrush(Colors.Transparent),
                            VerticalAlignment = VerticalAlignment.Center,
                        };

                        btn.ToolTip = $"DT Id: {dtData.dt_id}\nLocation: {dtData.location}\nTotal Customers: {totalCustomerCount}";

                        btn.Padding = new Thickness(2);

                        // Check which checkboxes are checked and set foreground accordingly
                        if (cbFeederView.IsChecked == true)
                        {
                            btn.Foreground = GetForegroundByFeederId(dtData.feeder_id);
                        }
                        else if (cbMriStatus.IsChecked == true)
                        {
                            // Fetch MRI status from the URL
                            string mriStatus = await GetMRIStatusFromUrl(dtData.dt_id);
                            // Update foreground color based on MRI status
                            btn.Foreground = GetForegroundByMRIStatus(mriStatus);
                        }
                        
                        else
                        {
                            // Set default foreground color
                            btn.Foreground = Brushes.Black; // Or any other default color
                        }

                        pushpinLayer.AddChild(btn, new Location(latitude, longitude), new Point(-btn.Width / 2, -btn.Height / 2));

                        btn.MouseRightButtonDown += Btn_MouseRightButtonDown;
                        btn.Click += Btn_Click;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (display a message, log, etc.)
                MessageBox.Show($"An error occurred while fetching or processing data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressLogin.Visibility = Visibility.Collapsed;
            }

            return pushpinLayer;
        }

        private void cbFeederView_Checked(object sender, RoutedEventArgs e)
        {
            //cbMriStatus.IsChecked = false;


            cbMriStatus.IsChecked = false;
            // Iterate through all existing pushpin layers and update the foreground color
            foreach (var pushpinLayer in myMap.Children.OfType<MapLayer>())
            {
                UpdatePushpinLayerForeground(pushpinLayer);
            }
        }

        private void cbMriStatus_Checked(object sender, RoutedEventArgs e)
        {
            cbFeederView.IsChecked = false;

            foreach (var pushpinLayer in myMap.Children.OfType<MapLayer>())
            {
                UpdatePushpinLayerForeground(pushpinLayer);
            }
        }

        private void cbDtDown_Checked(object sender, RoutedEventArgs e)
        {
            cbMriStatus.IsChecked = false;
            cbFeederView.IsChecked = false;
            foreach (var pushpinLayer in myMap.Children.OfType<MapLayer>())
            {
                UpdatePushpinLayerForeground(pushpinLayer);
            }
        }

        private async void UpdatePushpinLayerForeground(MapLayer pushpinLayer)
        {
            // Iterate through all children of the pushpin layer
            foreach (var child in pushpinLayer.Children)
            {
                if (child is Button btn)
                {
                    // Find the associated data item for the button
                    var tag = btn.Tag;
                    if (tag is ValueTuple<string, string> tuple)
                    {
                        var dtData = allFetchedData.FirstOrDefault(dt => dt.dt_id == tuple.Item1 && dt.feeder_id == tuple.Item2);
                        if (dtData != null)
                        {
                            // Check which checkboxes are checked and set foreground accordingly
                            if (cbFeederView.IsChecked == true)
                            {
                                // Update foreground color based on feeder ID
                                btn.Foreground = GetForegroundByFeederId(dtData.feeder_id);
                            }
                            else if (cbMriStatus.IsChecked == true)
                            {
                                // Fetch MRI status from the URL
                                string mriStatus = await GetMRIStatusFromUrl(dtData.dt_id);
                                //MessageBox.Show($"Fetched MRI status: {mriStatus}");

                                // Update foreground color based on MRI status
                                btn.Foreground = GetForegroundByMRIStatus(mriStatus);
                            }
                        
                            else
                            {
                                // Set default foreground color
                                btn.Foreground = Brushes.Black; // Or any other default color
                            }
                        }
                    }
                }
            }
        }

        private async Task<string> GetMRIStatusFromUrl(string dtId)
        {
            string url = $"http://103.234.126.43:3080/api/dt/mri/status/{dtId}";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Assuming the response is in JSON format and contains the status as "mri_status"
                    var json = JObject.Parse(responseBody);
                    return json["mri_status"]?.ToString() ?? "Unknown";
                }
                catch (Exception ex)
                {
                    // Handle any errors
                    Console.WriteLine($"Error fetching MRI status for dt_id {dtId}: {ex.Message}");
                    return "Unknown";
                }
            }
        }

        private Brush GetForegroundByMRIStatus(string mriStatus)
        {
            // Determine color for MRI status
            switch (mriStatus.ToLower())
            {
                case "online":
                    return Brushes.Lime;
                case "offline":
                    return Brushes.Red;
                default:
                    return Brushes.Yellow;  // Change this if needed for a different default
            }
        }


        #region Pushpin colors 

        Color[] colorRange =
        {
    Colors.Lime,
    Colors.RoyalBlue,
    Colors.Magenta,
    Colors.Yellow,
    Colors.Orange,
    Colors.Pink,
    Colors.Chartreuse,
    Colors.DeepPink,
    Colors.Gold,
    Colors.GreenYellow,
    Colors.HotPink,
    Colors.LightBlue,
    Colors.LightCoral,
    Colors.LightGreen,
    Colors.LightPink,
    Colors.LightSalmon,
    Colors.LightSkyBlue,
    Colors.Orchid,
    Colors.Cyan,
    Colors.BlueViolet,
    Colors.DarkGreen,
    Colors.DarkOrange,
    Colors.DarkRed,
    Colors.DarkTurquoise,
    Colors.DarkSlateBlue,
    Colors.DarkOliveGreen,
    Colors.DarkOrchid,
    Colors.DarkSlateGray,
    Colors.DarkKhaki,
    Colors.DarkMagenta,
    Colors.DarkSeaGreen,
    Colors.DarkSlateGray,
    Colors.DarkViolet,
    Colors.DodgerBlue,
    Colors.Firebrick,
    Colors.ForestGreen
};


        Dictionary<string, Brush> assignedColors = new Dictionary<string, Brush>();

        private Brush GetForegroundByFeederId(string feederId)
        {
            if (string.IsNullOrEmpty(feederId))
            {
                // If feederId is null or empty, return a default color (you can change this to fit your needs)
                return Brushes.Black; // Change this to the default color you want
            }

            if (assignedColors.ContainsKey(feederId))
            {
                // If the color for the feeder_id is already assigned, return it
                return assignedColors[feederId];
            }

            // Rest of the code for assigning a new color
            Color baseColor = colorRange[assignedColors.Count % colorRange.Length];
            byte transparency = 220;
            Color transparentColor = Color.FromArgb(transparency, baseColor.R, baseColor.G, baseColor.B);
            Brush newColor = new SolidColorBrush(transparentColor);

            // Add the color to the dictionary
            assignedColors.Add(feederId, newColor);

            // Return the new color
            return newColor;
        }

        private SolidColorBrush GetForegroundByDtStatus(string dtStatus)
        {
            SolidColorBrush brush;

            switch (dtStatus)
            {

                case "10":
                    brush = new SolidColorBrush(Colors.LimeGreen);
                    break;
                default:
                    brush = new SolidColorBrush(Colors.Black); // Default color if status doesn't match
                    break;

                case "01":
                case "11":
                    brush = new SolidColorBrush(Colors.Red );
                    break;

                case "00":
                    brush = new SolidColorBrush(Colors.Red );
                    break;
            }

            return brush;
        }

        #endregion Pushpin colors 



        #endregion add tree view items
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton1 = sender as Button;

            string tagString = string.Join(",", clickedButton1.Tag);
            string tagString1 = tagString.Replace("(", "").Replace(")", "");
            string[] tagString2 = tagString1.Split(',');

            string listDtIdList = tagString2[0];

            if (clickedDtIds.Contains(listDtIdList))
            {
                // Change color back and remove from the list
                clickedButton1.Foreground = GetForegroundByFeederId(tagString2[1]);
                clickedDtIds.Remove(listDtIdList);

                // Remove the corresponding ListBoxItem
                RemoveListBoxItem(listDtIdList);
            }
            else
            {
                // Change color to indicate selection and add to the list
                clickedDtIds.Add(listDtIdList);

                // Add the custom ListBoxItem with 'X' button
                AddListBoxItem(listDtIdList);
            }

            // Toggle visibility based on the count of clicked items
            stkAllData.Visibility = clickedDtIds.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AddListBoxItem(string dtId)
        {
            // Create a StackPanel to hold the dt_id and the X button
            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            // Create a TextBlock for the dt_id
            TextBlock textBlock = new TextBlock
            {
                Text = dtId,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };

            // Create the X button
            Button removeButton = new Button
            {
                Content = "X",  // Set content to "X"
                Width = 25,
                Height = 25,
                VerticalAlignment = VerticalAlignment.Center,
                Tag = dtId,  // Store the dt_id in the Tag for easy removal
                FontSize = 12,  // Set a larger font size for visibility
                Padding = new Thickness(0),  // Remove any extra padding
                Margin = new Thickness(5, 0, 0, 0),  // Add margin to separate from the text
                BorderThickness = new Thickness (0),
                Background = new SolidColorBrush(Color.FromRgb(32, 32, 32)),  // Set background to #202020
                Opacity = 0.8  // Set opacity to 0.8
            };

            // Attach the click event handler for the X button
            removeButton.Click += RemoveButton_Click;

            // Add the TextBlock and the X button to the panel
            panel.Children.Add(textBlock);
            panel.Children.Add(removeButton);

            // Create a ListBoxItem to hold the panel
            ListBoxItem listBoxItem = new ListBoxItem
            {
                Content = panel,
                Tag = dtId  // Store the dt_id in the Tag of the ListBoxItem
            };

            // Add the ListBoxItem to the ListBox
            listBoxDtId.Items.Add(listBoxItem);
        }

        //private void AddListBoxItem(string dtId)
        //{
        //    // Create a StackPanel to hold the dt_id and the X button
        //    StackPanel panel = new StackPanel
        //    {
        //        Orientation = Orientation.Horizontal
        //    };

        //    // Create a TextBlock for the dt_id
        //    TextBlock textBlock = new TextBlock
        //    {
        //        Text = dtId,
        //        VerticalAlignment = VerticalAlignment.Center,
        //        Margin = new Thickness(0, 0, 10, 0)
        //    };

        //    // Create the X button
        //    Button removeButton = new Button
        //    {
        //        Content = "X",
        //        Width = 20,
        //        Height = 20,
        //        VerticalAlignment = VerticalAlignment.Center,
        //        Tag = dtId // Store the dt_id in the Tag for easy removal
        //    };

        //    // Attach the click event handler for the X button
        //    removeButton.Click += RemoveButton_Click;

        //    // Add the TextBlock and the X button to the panel
        //    panel.Children.Add(textBlock);
        //    panel.Children.Add(removeButton);

        //    // Create a ListBoxItem to hold the panel
        //    ListBoxItem listBoxItem = new ListBoxItem
        //    {
        //        Content = panel,
        //        Tag = dtId // Also store the dt_id in the Tag of the ListBoxItem
        //    };

        //    // Add the ListBoxItem to the ListBox
        //    listBoxDtId.Items.Add(listBoxItem);
        //}

        private void RemoveListBoxItem(string dtId)
        {
            // Find the item in the ListBox that matches the dtId
            ListBoxItem itemToRemove = null;
            foreach (ListBoxItem item in listBoxDtId.Items)
            {
                if ((string)item.Tag == dtId)
                {
                    itemToRemove = item;
                    break;
                }
            }

            // If the item is found, remove it
            if (itemToRemove != null)
            {
                listBoxDtId.Items.Remove(itemToRemove);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Button removeButton = sender as Button;
            string dtId = (string)removeButton.Tag;

            // Remove the dtId from the list of selected items
            if (clickedDtIds.Contains(dtId))
            {
                clickedDtIds.Remove(dtId);
            }

            // Remove the corresponding ListBoxItem
            RemoveListBoxItem(dtId);

            // Toggle visibility based on the count of clicked items
            stkAllData.Visibility = clickedDtIds.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }



        private void btnViewDataAll_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button clickedViewAll = sender as Button;


            string CurrentUserId = CurrentUserId1;
            string CurrentUserRole = CurrentUserRole1;
            string CurrentUserPassword = CurrentUserPassword1;
            string CurrentUserName = CurrentUserName1;
            string CurrentUserEmployeeId = CurrentUserEmployeeId1;

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Background = (Brush)new BrushConverter().ConvertFrom("#202020");

            MenuItem itemTopic = new MenuItem();
            itemTopic.Header = "Check All Data";
            itemTopic.Foreground = Brushes.White;
            itemTopic.FontSize = 12.0;
            itemTopic.FontWeight = FontWeights.Bold;
            itemTopic.HorizontalContentAlignment = HorizontalAlignment.Center;

            itemTopic.IsEnabled = false;
            contextMenu.Items.Add(itemTopic);

            MenuItem itemLine = new MenuItem();
            itemLine.Foreground = Brushes.White;
            itemLine.Margin = new Thickness(20, -5, 20, 5);
            itemLine.IsEnabled = false;
            itemLine.Height = 1;
            itemLine.Background = Brushes.Red;
            contextMenu.Items.Add(itemLine);



            string userId = CurrentUserId1;
            string userRole = CurrentUserRole1;
            string userPassword = CurrentUserPassword1;
            string userName = CurrentUserName1;
            string userEmployeeId = CurrentUserEmployeeId1;

            MenuItem itemInstantPara = new MenuItem();
            itemInstantPara.Header = "Instantaneous Para";
            itemInstantPara.Foreground = Brushes.White;
            itemInstantPara.FontSize = 12.0;
            itemInstantPara.Margin = new Thickness(20, 5, 20, 5);

            itemInstantPara.Icon = new PackIcon
            {
                Kind = PackIconKind.ClockTimeEightOutline,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3")
            };
            itemInstantPara.Click += (s, ea) =>
            {
                MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

                InstantAll.InstantAll InstantAllControl = new InstantAll.InstantAll(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                InstantAllControl.SetListBoxItems(clickedDtIds);

                mainWindow.GridAmrView.Children.Clear();
                mainWindow.GridAmrView.Children.Add(InstantAllControl);

                mainWindow.MoveGridCursor(3);
            };
            contextMenu.Items.Add(itemInstantPara);


            MenuItem itemBilling = new MenuItem();
            itemBilling.Header = "Instant HIS";
            itemBilling.Foreground = Brushes.White;
            itemBilling.FontSize = 12.0;
            itemBilling.Margin = new Thickness(20, 5, 20, 5);

            itemBilling.Icon = new PackIcon
            {
                Kind = PackIconKind.AttachMoney,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3")
            };
            itemBilling.Click += (s, ea) =>
            {
                

                MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

                BillingAll.BillingAll billingControl = new BillingAll.BillingAll(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                billingControl.SetListBoxItems(clickedDtIds);

                mainWindow.GridAmrView.Children.Clear();
                mainWindow.GridAmrView.Children.Add(billingControl);

                mainWindow.MoveGridCursor(4); 
            };
            contextMenu.Items.Add(itemBilling);


            MenuItem itemLoadProfile0 = new MenuItem();
            itemLoadProfile0.Header = "Load Profile 0";
            itemLoadProfile0.Foreground = Brushes.White;
            itemLoadProfile0.FontSize = 12.0;
            itemLoadProfile0.Margin = new Thickness(20, 5, 20, 5);

            itemLoadProfile0.Icon = new PackIcon
            {
                Kind = PackIconKind.ChartLine,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3")
            };
            itemLoadProfile0.Click += (s, ea) =>
            {
               

                MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

                LoadSurveyAll.LoadSurveyAll loadSurvey0Control = new LoadSurveyAll.LoadSurveyAll(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                loadSurvey0Control.SetListBoxItems(clickedDtIds);

                mainWindow.GridAmrView.Children.Clear();
                mainWindow.GridAmrView.Children.Add(loadSurvey0Control);

                mainWindow.MoveGridCursor(5); 
            };
            contextMenu.Items.Add(itemLoadProfile0);


            MenuItem itemLoadProfile1 = new MenuItem();
            itemLoadProfile1.Header = "Load Profile 1";
            itemLoadProfile1.Foreground = Brushes.White;
            itemLoadProfile1.FontSize = 12.0;
            itemLoadProfile1.Margin = new Thickness(20, 5, 20, 5);

            itemLoadProfile1.Icon = new PackIcon
            {
                Kind = PackIconKind.ChartLine,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3")
            };
            itemLoadProfile1.Click += (s, ea) =>
            {
                MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

                LoadSurveyAll1 loadSurvey1Control = new LoadSurveyAll1(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                loadSurvey1Control.SetListBoxItems(clickedDtIds);

                mainWindow.GridAmrView.Children.Clear();
                mainWindow.GridAmrView.Children.Add(loadSurvey1Control);

                mainWindow.MoveGridCursor(6); 
            };
            contextMenu.Items.Add(itemLoadProfile1);


            MenuItem itemEvents = new MenuItem();
            itemEvents.Header = "Events";
            itemEvents.Foreground = Brushes.White;
            itemEvents.FontSize = 12.0;
            itemEvents.Margin = new Thickness(20, 5, 20, 5);

            itemEvents.Icon = new PackIcon
            {
                Kind = PackIconKind.Event,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3")
            };
            itemEvents.Click += (s, ea) =>
            {
                MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

                Events.Events  EventsControl = new Events.Events (CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                EventsControl.SetListBoxItems(clickedDtIds);

                mainWindow.GridAmrView.Children.Clear();
                mainWindow.GridAmrView.Children.Add(EventsControl);

                mainWindow.MoveGridCursor(7);
            };
            contextMenu.Items.Add(itemEvents);


            MenuItem itemReport = new MenuItem();
            itemReport.Header = "POI";
            itemReport.Foreground = Brushes.White;
            itemReport.FontSize = 12.0;
            itemReport.Margin = new Thickness(20, 5, 20, 5);

            itemReport.Icon = new PackIcon
            {
                Kind = PackIconKind.FileReport,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3")
            };
            itemReport.Click += (s, ea) =>
            {
                MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

                Report.Report ReportControl = new Report.Report(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                ReportControl.SetListBoxItems(clickedDtIds);

                mainWindow.GridAmrView.Children.Clear();
                mainWindow.GridAmrView.Children.Add(ReportControl);

                mainWindow.MoveGridCursor(8);
            };
            contextMenu.Items.Add(itemReport);


            MenuItem itemReliabilityIndices = new MenuItem();
            itemReliabilityIndices.Header = "Reliability Indices";
            itemReliabilityIndices.Foreground = Brushes.White;
            itemReliabilityIndices.FontSize = 12.0;
            itemReliabilityIndices.Margin = new Thickness(20, 5, 20, 5);

            itemReliabilityIndices.Icon = new PackIcon
            {
                Kind = PackIconKind.FileReport,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3")
            };
            itemReliabilityIndices.Click += (s, ea) =>
            {
                MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

                ReliabilityIndices.ReliabilityIndices ReliabilityIndicesControl = new ReliabilityIndices.ReliabilityIndices(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

                ReliabilityIndicesControl.SetListBoxItems(clickedDtIds);

                mainWindow.GridAmrView.Children.Clear();
                mainWindow.GridAmrView.Children.Add(ReliabilityIndicesControl);

                mainWindow.MoveGridCursor(9);
            };
            contextMenu.Items.Add(itemReliabilityIndices);


            //MenuItem itemNms = new MenuItem();
            //itemNms.Header = "NMS";
            //itemNms.Foreground = Brushes.White;
            //itemNms.FontSize = 12.0;
            //itemNms.Margin = new Thickness(20, 5, 20, 5);

            //itemNms.Icon = new PackIcon
            //{
            //    Kind = PackIconKind.Graph,
            //    Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3")
            //};
            //itemNms.Click += (s, ea) =>
            //{
            //    MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

            //    NMS.NMS NMSControl = new NMS.NMS(CurrentUserId, CurrentUserRole, CurrentUserPassword, CurrentUserName, CurrentUserEmployeeId);

            //    NMSControl.SetListBoxItems(clickedDtIds);

            //    mainWindow.GridAmrView.Children.Clear();
            //    mainWindow.GridAmrView.Children.Add(NMSControl);

            //    mainWindow.MoveGridCursor(10);
            //};
            //contextMenu.Items.Add(itemNms);


            clickedViewAll.ContextMenu = contextMenu;






            if (clickedDtIds != null && clickedDtIds.Count > 0)
            {
                stkAllData.Visibility = Visibility.Visible;
            }
            else
            {
                stkAllData.Visibility = Visibility.Collapsed;
            }
        }

        private void Btn_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button clickedButton = sender as Button;

            string tagString = string.Join(",", clickedButton.Tag);
            string tagString1 = tagString.Replace("(", "").Replace(")", "");

            string[] tagString2 = tagString1.Split(',');

            string tagString3 = tagString2[0];

            txtDtIdSearch.Text = tagString3;
            DTId1 = tagString3;

            //ViewModel1 viewModel = new ViewModel1();
            //DataContext = viewModel;
            //viewModel.VmDtId = DTId1;
            //viewModel.VmUserRole = CurrentUserRole1;

            //viewModel.OnMeterNoPropertyChanged(nameof(ViewModel1.VmDtId));
            //viewModel.OnMeterNoPropertyChanged(nameof(ViewModel1.VmUserRole));

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Background = (Brush)new BrushConverter().ConvertFrom("#202020");

            MenuItem itemTopic = new MenuItem();
            itemTopic.Header = "DT ID: " + txtDtIdSearch.Text;
            itemTopic.Foreground = Brushes.White;
            itemTopic.FontSize = 12.0;
            itemTopic.FontWeight = FontWeights.Bold;
            itemTopic.HorizontalContentAlignment = HorizontalAlignment.Center;

            itemTopic.IsEnabled = false;
            contextMenu.Items.Add(itemTopic);

            MenuItem itemLine = new MenuItem();
            itemLine.Foreground = Brushes.White;
            itemLine.Margin = new Thickness(20, -5, 20, 5);
            itemLine.IsEnabled = false;
            itemLine.Height = 1;
            itemLine.Background = Brushes.Red;
            contextMenu.Items.Add(itemLine);

            

            string userId = CurrentUserId1;
            string userRole = CurrentUserRole1;
            string userPassword = CurrentUserPassword1;
            string userName = CurrentUserName1;
            string userEmployeeId = CurrentUserEmployeeId1;

            MenuItem itemMeterData = new MenuItem();
            itemMeterData.Header = "Meter Data";
            itemMeterData.Foreground = Brushes.White;
            itemMeterData.FontSize = 12.0;
            itemMeterData.Margin = new Thickness(20, 5, 20, 5);

            itemMeterData.Icon = new PackIcon
            {
                Kind = PackIconKind.Table,
                Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3")
            };
            itemMeterData.Click += (s, ea) =>
            {

                

                MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

                // Use the existing ViewModel instance from MainWindow
                ViewModel1 viewModel = (ViewModel1)mainWindow.DataContext;

                // Update properties
                //viewModel.VmDtId = txtDtIdSearch.Text;
                viewModel.VmDtId =  DTId1;
                viewModel.VmUserRole = CurrentUserRole1;


                // Raise property changed events
                viewModel.OnMeterNoPropertyChanged(nameof(ViewModel1.VmDtId));
                viewModel.OnMeterNoPropertyChanged(nameof(ViewModel1.VmUserRole));

                // Navigate to MeterData
                mainWindow.NavigateToMeterData();
            };
            contextMenu.Items.Add(itemMeterData);

            clickedButton.ContextMenu = contextMenu;
        }

        #region Single DT
        private void txtDtIdSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            txtDtIdSearch.Text = "";

            txtDtIdSearch.Foreground = new SolidColorBrush(Colors.White);
        }

        private void txtDtIdSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddSingleDt();
            }
        }

        private void btnSerachDt_Click(object sender, RoutedEventArgs e)
        {
            AddSingleDt();
        }

        private async void AddSingleDt()
        {
            try
            {
                // Show progress bar
                progressMeterData.Visibility = Visibility.Visible;

                myMap.Children.Clear();

                HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync("http://103.234.126.43:3080/dtmeter/info/esd_essd_info/all");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                List<DT> dtMeterList = JsonConvert.DeserializeObject<List<DT>>(responseBody);

                DT dtMeter = dtMeterList.FirstOrDefault(x => x.dt_id == txtDtIdSearch.Text);

                if (dtMeter != null)
                {
                    if (!double.TryParse(dtMeter.latitude, out double latitude1) || !double.TryParse(dtMeter.longitude, out double longitude1))
                    {
                        // Skip invalid data
                        MessageBox.Show($"Invalid latitude or longitude for DT Id: {dtMeter.dt_id}", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string dtId = dtMeter.dt_id;
                    string dtFeederId = dtMeter.feeder_id;
                    string dtLocation = dtMeter.location;
                    string dtConsumerCount = dtMeter.total_customer_count;

                    locations = new[] { new Location(latitude1, longitude1) };

                    MapLayer pushpinLayer = new MapLayer();
                    double height = new Pushpin().Height;
                    double width = new Pushpin().Width;

                    for (int j = 0; j < locations.Length; j++)
                    {
                        Button btn = new Button
                        {
                            Content = new PackIcon
                            { Kind = MaterialDesignThemes.Wpf.PackIconKind.Circle, Height = 25, Width = 25 },
                            Tag = (dtId, dtFeederId),
                            Background = new SolidColorBrush(Colors.Transparent),
                            VerticalContentAlignment = VerticalAlignment.Bottom,
                            Foreground = new SolidColorBrush(Colors.Red),
                            Width = 30,
                            MinHeight = 25,
                            BorderBrush = new SolidColorBrush(Colors.Transparent),
                            VerticalAlignment = VerticalAlignment.Center,
                        };

                        btn.ToolTip = $"DT Id: {dtId }\nLocation: {dtLocation}\nTotal Customer: {dtConsumerCount}";

                        btn.Padding = new Thickness(2);

                        pushpinLayer.AddChild(btn, locations[j],
                            new Point(-btn.Width / 2, -height / 2));

                        btn.MouseRightButtonDown += Btn_MouseRightButtonDown;
                    }

                    await Dispatcher.InvokeAsync(() =>
                    {
                        _ = myMap.Children.Add(pushpinLayer);
                        myMap.Center = new Location(latitude1, longitude1);
                        myMap.ZoomLevel = 17;
                    });
                }
                else
                {
                    MessageBox.Show("Invalid DT Id. Please double-check and enter a valid DT Id.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Hide progress bar
                //progressLogin.Visibility = Visibility.Collapsed;
                progressMeterData.Visibility = Visibility.Collapsed;
            }
        }


        #endregion Single DT
      
        #region MapControl

        private void MyMap_ViewChangeEnd(object sender, MapEventArgs e)
        {
            if (myMap.ZoomLevel > MaxZoomLevel)
            {
                myMap.ZoomLevel = MaxZoomLevel;
            }

            if (myMap.ZoomLevel <= MinZoomLevel)
            {
                myMap.ZoomLevel = MinZoomLevel;
            }

        }
        private void btnMapHome_Click(object sender, RoutedEventArgs e)
        {
            myMap.Center = new Location(27.52, 90.4);
            myMap.ZoomLevel = 8.0;
        }
        private void btnZoomPlus_Click(object sender, RoutedEventArgs e)
        {
            double zoomPlus = 0.1;
            myMap.ZoomLevel = myMap.ZoomLevel + zoomPlus;

        }
        private void btnZoomMinus_Click(object sender, RoutedEventArgs e)
        {
            double zoomMinus = 0.1;
            myMap.ZoomLevel = myMap.ZoomLevel - zoomMinus;
        }

        #endregion MapControl

       



       
    }
}



