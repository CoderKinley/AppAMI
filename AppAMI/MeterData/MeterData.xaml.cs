using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using AppAMI.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Xml.Linq;

namespace AppAMI.MeterData
{
    /// <summary>
    /// Interaction logic for MeterData.xaml
    /// </summary>
    public partial class MeterData : UserControl
    {
        UserControl usc = null;


        string CurrentUserId1;
        string CurrentUserRole1;
        string CurrentUserPassword1;
        string CurrentUserName1;
        string CurrentUserEmployeeId1;

        string DTId1;

        public MeterData(string CurrentUserId, string CurrentUserRole, string CurrentUserPassword, string CurrentUserName, string CurrentUserEmployeeId)
        {
            InitializeComponent();
            CurrentUserId1 = CurrentUserId;
            CurrentUserRole1 = CurrentUserRole;
            CurrentUserPassword1 = CurrentUserPassword;
            CurrentUserName1 = CurrentUserName;
            CurrentUserEmployeeId1 = CurrentUserEmployeeId;


            txtDtId.Foreground = new SolidColorBrush(Colors.Gray);

            GridCursor.Visibility = Visibility.Hidden;
            GridCursor1.Visibility = Visibility.Hidden;

            listBoxSuggestions.Visibility = Visibility.Collapsed;


        }


        #region Rest Api
        private void btnSearchDt_Click(object sender, RoutedEventArgs e)
        {
            SearchMeter();

            listBoxSuggestions.Visibility = Visibility.Collapsed;
        }

        private void txtDtId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchMeter();

                listBoxSuggestions.Visibility = Visibility.Collapsed;

            }
        }

        private async void SearchMeter()
        {

            txtDtId.Foreground = new SolidColorBrush(Colors.Gray);

            if (txtDtId.Text == "")
            {
                //MessageBox.Show("Meter Number Field is Empty" + "\n" + "\n" + "Enter Meter Number");
                MessageBox.Show("Enter DT Id", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);

            }

            else
            {
                DTId1 = txtDtId.Text;
                await getData();
                await getStatus();
                ViewModel1 viewModel = new ViewModel1();
                DataContext = viewModel;
                viewModel.VmDtId = DTId1;
                viewModel.VmUserRole = CurrentUserRole1;

                viewModel.OnMeterNoPropertyChanged(nameof(ViewModel1.VmDtId));
                viewModel.OnMeterNoPropertyChanged(nameof(ViewModel1.VmUserRole));
            }
        }

        private async Task getData()
        {
            progressMeterData.Visibility = Visibility.Visible;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    //string url = "http://103.234.126.43:3080/dtmeter/info/esd_essd_info/all";
                    string url = "http://103.234.126.43:3080/dtmeter/info/esd_essd_info/all";

                    string json = await httpClient.GetStringAsync(url);

                    List<DtMeter> all_data = JsonConvert.DeserializeObject<List<DtMeter>>(json);

                    List<DtMeter> dtMeters = all_data.Where(a => a.dt_id == txtDtId.Text).ToList();

                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (dtMeters.Count > 0)
                        {
                            lblDtName.Content = dtMeters[0].dt_name;
                            lblMeterID.Content = dtMeters[0].dt_meter_serial_no;
                            lblMriID.Content = dtMeters[0].mri_serial_no;



                            


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
            
            finally
            {
                progressMeterData.Visibility = Visibility.Collapsed;

            }
        }

        private async Task getStatus()
        {
            progressMeterData.Visibility = Visibility.Visible;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string url = $"http://103.234.126.43:3080/api/dt/mri/status/{txtDtId.Text}";

                    string json = await httpClient.GetStringAsync(url);

                    // Assuming the JSON is a single object, not an array
                    NmsMriClass dtMeter = JsonConvert.DeserializeObject<NmsMriClass>(json);

                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (dtMeter != null)
                        {
                            lblMriStatus.Content = dtMeter.mri_status;
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
                progressMeterData.Visibility = Visibility.Collapsed;
            }
        }


        #endregion Rest Api


        private Dictionary<int, UserControl> cachedMeterDataControls1 = new Dictionary<int, UserControl>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GridCursor.Visibility = Visibility.Visible;
            GridCursor1.Visibility = Visibility.Visible;

            int index = int.Parse(((Button)e.Source).Uid);

            GridCursor.Margin = new Thickness(0, 0 + (70 * index), 0, 0);
            GridCursor1.Margin = new Thickness(0, 0 + (70 * index), 0, 0);

            if (!cachedMeterDataControls1.ContainsKey(index))
            {
                // Create a new instance only if it doesn't exist in the cache
                UserControl usc = CreateNewMeterDataControl(index);
                cachedMeterDataControls1[index] = usc;
            }

            // Show the cached user control
            GridMeterDataView.Children.Clear();
            GridMeterDataView.Children.Add(cachedMeterDataControls1[index]);
        }

        private UserControl CreateNewMeterDataControl(int index)
        {
            // Create a new instance based on the index
            switch (index)
            {
                case 0:
                    return new InstantaneousTypical(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);

                case 1:
                    return new BillingTabular(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);
                case 2:
                    return new LoadSurveyTabular(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);
                case 3:
                    return new LoadSurveyTabular1(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);
                case 4:
                    return new Events(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);
                case 5:
                    return new ReportMeter(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);
                case 6:
                    return new ReliabilityIndices(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);
                //case 7:
                //    return new NMS(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);

                case 7:
                    return new uscNmsMri(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);

                // Add other cases as needed

                default:
                    // Handle unknown index
                    return null;
            }
        }



        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    GridCursor.Visibility = Visibility.Visible;
        //    GridCursor1.Visibility = Visibility.Visible;

        //    int index = int.Parse(((Button)e.Source).Uid);

        //    GridCursor.Margin = new Thickness(0, 0 + (70 * index), 0, 0);
        //    GridCursor1.Margin = new Thickness(0, 0 + (70 * index), 0, 0);

        //    switch (index)
        //    {
        //        case 0:

        //            GridMeterDataView.Children.Clear();
        //            usc = new InstantaneousSelector(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);
        //            GridMeterDataView.Children.Add(usc);

        //            break;
        //        case 1:

        //            GridMeterDataView.Children.Clear();
        //            usc = new BillingTabular(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);
        //            GridMeterDataView.Children.Add(usc);


        //            break;
        //        case 2:
        //            GridMeterDataView.Children.Clear();
        //            usc = new LoadSurveyTabular(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);
        //            GridMeterDataView.Children.Add(usc);
        //            break;

        //        case 3:
        //            GridMeterDataView.Children.Clear();
        //            usc = new LoadSurveyTabular1(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);
        //            GridMeterDataView.Children.Add(usc);
        //            break;
        //        case 4:
        //            GridMeterDataView.Children.Clear();
        //            usc = new Events(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);
        //            GridMeterDataView.Children.Add(usc);
        //            break;
        //        case 5:
        //            GridMeterDataView.Children.Clear();
        //            usc = new ReportMeter(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);
        //            GridMeterDataView.Children.Add(usc);
        //            break;

        //        case 6:
        //            GridMeterDataView.Children.Clear();
        //            usc = new ReliabilityIndices(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);
        //            GridMeterDataView.Children.Add(usc);
        //            break;
        //        case 7:
        //            GridMeterDataView.Children.Clear();
        //            usc = new NMS(CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1, DTId1);
        //            GridMeterDataView.Children.Add(usc);
        //            break;




        //    }
        //}

        private void txtDtId_GotFocus(object sender, RoutedEventArgs e)
        {
            txtDtId.Text = "";

            txtDtId.Foreground = new SolidColorBrush(Colors.White);


            listBoxSuggestions.Visibility = Visibility.Visible;
        }








        #region Auto Suggesstion
        private void txtDtId_TextChanged(object sender, TextChangedEventArgs e)
        {
            string inputText = txtDtId.Text;
            GetAutoSuggestions(inputText);

        }

        private void GetAutoSuggestions(string inputText)
        {
            try
            {
                var suggestions = ReadDtIdValuesFromXml("XMLFile1.xml")
                    .Where(dtId => dtId.StartsWith(inputText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                UpdateSuggestions(suggestions);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private List<string> ReadDtIdValuesFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var dtIdValues = doc.Descendants("dt_id")
                                    .Select(element => element.Value)
                                    .ToList();

                return dtIdValues;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }

        private void UpdateSuggestions(List<string> suggestions)
        {
            listBoxSuggestions.ItemsSource = suggestions;
        }


        private void listBoxSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxSuggestions.SelectedItem != null)
            {
                txtDtId.Text = listBoxSuggestions.SelectedItem.ToString();

                SearchMeter();


            }

            listBoxSuggestions.Visibility = Visibility.Collapsed;

        }

        #endregion Auto Suggesstion

        
    }
}
