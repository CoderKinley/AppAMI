using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Windows.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Collections.Generic;
using AppAMI.Classes;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using AppAMI.MriFirmware;
using System.Net;
using System.Net.Mail;

namespace AppAMI.MeterData
{
    /// <summary>
    /// Interaction logic for NMS.xaml
    /// </summary>
    public partial class NMS : UserControl
    {

        #region  variable mqttClient

        string BrokerAddress = "103.234.126.44";

        MqttClient clientNms;

        string clientId;

        string recieved_data1;

        string mqttTopicNms;

        #endregion  variable mqttClient

        #region Data Variable

        string meterMriConn;
        string sdStorage;

        string sdUsed;
        string sdUsedPercent;
        string purgingDate;

        string purgingTime;
        string sdReadNo;
        string sdWriteNo;

        string netStrength;
        string dataSentDate;
        string dataSentTime;

        string dataReceivedTime;


        #endregion Data Variable

        private DispatcherTimer timer;
        private int counter;


        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;



        public NMS(string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1, string DTId1)
        {
            InitializeComponent();

            CurrentUserId2 = CurrentUserId1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;


            //dtPickerStart.DisplayDateEnd = DateTime.Today;
            //dtPickerEnd.DisplayDateEnd = DateTime.Today;

            //grdEmailAddress.Visibility = Visibility.Collapsed ;

            //dtPickerStart.IsEnabled = false;
            //dtPickerEnd.IsEnabled = false;

            //txtDtId.Loaded += TxtDtId_Loaded;
            //txtUserRole.Loaded += TxtUserRole_Loaded;
            //txtDtId.TextChanged += TxtDtId_TextChanged;
        }


        private   void TxtDtId_Loaded(object sender, RoutedEventArgs e)
        {
            dtPickerStart.IsEnabled = true;
            dtPickerEnd.IsEnabled = true;
        }

        private void TxtUserRole_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentUserRole2 = txtUserRole.Text;
        }

        private async void TxtDtId_TextChanged(object sender, TextChangedEventArgs e)
        {
            await GetTopic();
            await GetCurrentFirmwareVersion();
            await GetFirmwareVersionHistory();

            if (dtPickerEnd.SelectedDate.HasValue && dtPickerEnd.SelectedDate > dtPickerStart.SelectedDate)
            {
                await ReadDatabase();
            }
        }


        #region Fault and Accounting  Management

        private async  void cbCritical_Checked(object sender, RoutedEventArgs e)
        {
            cbMajor.IsChecked = false;
            cbMinor.IsChecked = false;

            await ReadDatabase();
        }
        private void cbCritical_Unchecked(object sender, RoutedEventArgs e)
        {
            //ReadDatabase();
        }
        private async void cbMajor_Checked(object sender, RoutedEventArgs e)
        {
            cbCritical.IsChecked = false;
            cbMinor.IsChecked = false;
            await ReadDatabase();
        }
        private void cbMajor_Unchecked(object sender, RoutedEventArgs e)
        {
            //ReadDatabase();
        }
        private async  void cbMinor_Checked(object sender, RoutedEventArgs e)
        {
            cbCritical.IsChecked = false;
            cbMajor.IsChecked = false;
            await ReadDatabase();
        }
        private void cbMinor_Unchecked(object sender, RoutedEventArgs e)
        {
            //ReadDatabase();
        }

        private async void dtPickerStart_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy"; //for the second type
            Thread.CurrentThread.CurrentCulture = ci;

            if (!dtPickerEnd.SelectedDate.HasValue)
            {
                MessageBox.Show("Please insert the end date.", "End Date Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;  // Exit the method if the start date is not selected
            }

            // Check if both dtPickerStart and dtPickerEnd have valid values
            if (dtPickerEnd.SelectedDate.HasValue && dtPickerEnd.SelectedDate > dtPickerStart.SelectedDate)
            {
                await ReadDatabase();
            }


            else
            {
                // Display MessageBox if conditions are not met
                MessageBox.Show("Please make sure the date range is valid.", "Invalid Date Range", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private async void dtPickerEnd_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy"; //for the second type
            Thread.CurrentThread.CurrentCulture = ci;

            if (!dtPickerStart.SelectedDate.HasValue)
            {
                MessageBox.Show("Please insert the start date.", "Start Date Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;  // Exit the method if the start date is not selected
            }

            // Check if both dtPickerStart and dtPickerEnd have valid values
            if (dtPickerEnd.SelectedDate.HasValue && dtPickerEnd.SelectedDate > dtPickerStart.SelectedDate)
            {
                await ReadDatabase();
            }
            else
            {
                // Display MessageBox if conditions are not met
                MessageBox.Show("Please make sure the date range is valid.", "Invalid Date Range", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }



        private async Task ReadDatabase()
        {
            progressLogin.Visibility = Visibility.Visible;

            try
            {
                if (!string.IsNullOrEmpty(txtDtId.Text))
                {
                    using (WebClient web = new WebClient())
                    {
                        string url = string.Format("http://103.234.126.43:3080/dtmeter/nms/data/{0}", txtDtId.Text);
                        string json = await web.DownloadStringTaskAsync(url);

                        List<Nms> all_data = JsonConvert.DeserializeObject<List<Nms>>(json);

                        List<Nms> filteredDtId = all_data.Where(a => a.dt_id == txtDtId.Text).ToList();

                        List<Nms> filtered_date_data = filteredDtId
                            .Where(x => DateTime.ParseExact(x.date, "dd-MM-yyyy", CultureInfo.InvariantCulture) >= dtPickerStart.SelectedDate.Value
                                && DateTime.ParseExact(x.date, "dd-MM-yyyy", CultureInfo.InvariantCulture) <= dtPickerEnd.SelectedDate.Value)
                            .ToList();

                        filtered_date_data.Reverse();

                        datagridNmsFaultMng.ItemsSource = filtered_date_data;

                        // Filter based on checkbox selection
                        List<Nms> Critical = filtered_date_data.Where(a => a.meter_mri_con == "0").ToList();
                        lblCriticalCount.Content = Critical.Count;

                        List<Nms> Major = filtered_date_data.Where(a => double.TryParse(a.sd_card_percentage, out double value) && value >= 80.0).ToList();
                        lblMajorCount.Content = Major.Count;

                        List<Nms> Minor = filtered_date_data.Except(Critical).Except(Major).ToList();
                        lblMinorCount.Content = Minor.Count;

                        if (cbCritical.IsChecked == true)
                        {
                            datagridNmsFaultMng.ItemsSource = Critical;
                        }
                        else if (cbMajor.IsChecked == true)
                        {
                            datagridNmsFaultMng.ItemsSource = Major;
                        }
                        else if (cbMinor.IsChecked == true)
                        {
                            datagridNmsFaultMng.ItemsSource = Minor;
                        }

                        // Reset the flag if the operation is successful
                        showErrorMessage = false;
                    }
                }
                else
                {
                    // Show the error message only if the flag is true
                    if (showErrorMessage)
                    {
                        MessageBox.Show("Enter DT Id", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error fetching data. Please try again.", "Data Retrieval Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressLogin.Visibility = Visibility.Collapsed;
            }
        }


        #endregion Fault and Accounting  Management


        #region Configuration Management 


        private async Task GetCurrentFirmwareVersion()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                // Check if txtDtId and progressLogin are still loaded
                if (txtDtId.IsLoaded && progressLogin.IsLoaded)
                {
                    progressLogin.Visibility = Visibility.Visible;
                }
            });

            try
            {
                // Check if txtDtId is still loaded
                if (txtDtId.IsLoaded && !string.IsNullOrEmpty(txtDtId.Text))
                {
                    using (WebClient web = new WebClient())
                    {
                        string url = string.Format("http://103.234.126.43:3080/dtmeter/info/esd_essd_info/all");
                        string json = await web.DownloadStringTaskAsync(url);

                        List<Mri> all_data = JsonConvert.DeserializeObject<List<Mri>>(json);

                        List<Mri> filteredDtId = all_data.Where(a => a.dt_id == txtDtId.Text).ToList();

                        // Check if txtDtId is still loaded
                        await Dispatcher.InvokeAsync(() =>
                        {
                            if (txtDtId.IsLoaded && progressLogin.IsLoaded)
                            {
                                if (filteredDtId.Count > 0)
                                {
                                    lblCurrentFirmwareVer.Content = filteredDtId[0].mri_firmware_version;
                                }
                                else
                                {
                                    // Show the error message only if the flag is true
                                    if (showErrorMessage)
                                    {
                                        MessageBox.Show("No DT Id found, Enter Valid DT Id", "DT Id Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    }
                                }

                                // Reset the flag if the operation is successful
                                showErrorMessage = false;
                            }
                        });
                    }
                }
                else
                {
                    // Show the error message only if the flag is true
                    if (showErrorMessage)
                    {
                        MessageBox.Show("Enter DT Id", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch
            {
                // Check if progressLogin is still loaded
                await Dispatcher.InvokeAsync(() =>
                {
                    if (progressLogin.IsLoaded)
                    {
                        MessageBox.Show("Error fetching data. Please try again.", "Data Retrieval Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
            finally
            {
                // Check if progressLogin is still loaded
                await Dispatcher.InvokeAsync(() =>
                {
                    if (progressLogin.IsLoaded)
                    {
                        progressLogin.Visibility = Visibility.Collapsed;
                    }
                });
            }
        }

        private async Task GetFirmwareVersionHistory()
        {
            progressLogin.Visibility = Visibility.Visible;

            try
            {
                if (!string.IsNullOrEmpty(txtDtId.Text))
                {
                    using (WebClient web = new WebClient())
                    {
                        string url = string.Format("http://103.234.126.43:3080/dtmeter/configuration/management/data/{0}", txtDtId.Text);
                        string json = await web.DownloadStringTaskAsync(url);

                        List<FirmwareVerCode> all_data = JsonConvert.DeserializeObject<List<FirmwareVerCode>>(json);

                        List<FirmwareVerCode> filteredDtId = all_data.Where(a => a.dt_id == txtDtId.Text).ToList();

                       

                        // Bind filtered data to DataGrid
                        _ = Dispatcher.InvokeAsync(() =>
                        {
                            dataGridFirmwareVersion.ItemsSource = filteredDtId;
                        });

                        // Reset the flag if the operation is successful
                        showErrorMessage = false;
                    }
                }
                else
                {
                    // Show the error message only if the flag is true
                    if (showErrorMessage)
                    {
                        MessageBox.Show("Enter DT Id", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error fetching data. Please try again.", "Data Retrieval Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressLogin.Visibility = Visibility.Collapsed;
            }
        }
      
        private void btnUpdateFirmware_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentUserRole2))
            {
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else
            {
                if (CurrentUserRole2.Equals("Operator"))
                {
                    MessageBox.Show("Only Administrator can Update Firmware Versions", "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Warning);

                }

                else
                {
                    NewFirmwareWindow newFirmwareWindow = new NewFirmwareWindow(txtDtId.Text);
                    newFirmwareWindow.ShowDialog();

                    if (clientNms  != null && clientNms.IsConnected)
                    {
                        clientNms.Disconnect();
                    }

                }
            }
        }

        #endregion Configuration Management 

        private bool showErrorMessage = true;

        private async Task GetTopic()
        {
            progressLogin.Visibility = Visibility.Visible;

            try
            {
                if (!string.IsNullOrEmpty(txtDtId.Text))
                {
                    using (WebClient web = new WebClient())
                    {
                        string url = string.Format("http://103.234.126.43:3080/dtmeter/info/esd_essd_info/all");
                        string json = await web.DownloadStringTaskAsync(url);

                        List<MqttClass> all_data = JsonConvert.DeserializeObject<List<MqttClass>>(json);

                        List<MqttClass> filteredDtId = all_data.Where(a => a.dt_id == txtDtId.Text).ToList();


                        if (filteredDtId.Count > 0)
                        {
                            MqttClass filteredMqttClass = filteredDtId[0]; // Assuming you want the first item

                            mqttTopicNms = filteredMqttClass.nms;
                            //mqttTopicPing = filteredMqttClass.ping;
                            mqttInitializeMethod();
                            showErrorMessage = false;
                        }

                        // Reset the flag if the operation is successful
                        showErrorMessage = false;
                    }
                }
                else
                {
                    // Show the error message only if the flag is true
                    if (showErrorMessage)
                    {
                        MessageBox.Show("Enter DT Id", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error fetching data. Please try again.", "Data Retrieval Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressLogin.Visibility = Visibility.Collapsed;
            }
        }


        #region mqtt client
        private void mqttInitializeMethod()
        {
            try
            {
                clientNms = new MqttClient(BrokerAddress);
                clientNms.MqttMsgPublishReceived += client_MqttMsgPublishReceived1;
                clientId = Guid.NewGuid().ToString();
                clientNms.Connect(clientId);
                clientNms.Subscribe(new string[] { mqttTopicNms }, new byte[] { 2 });
                recieved_data1 = "";
            }

            catch
            {
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        void client_MqttMsgPublishReceived1(object sender, MqttMsgPublishEventArgs e)
        {
            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
            Dispatcher.Invoke(delegate
            {              // we need this construction because the receiving code in the library and the UI with textbox run on different threads
                recieved_data1 = ReceivedMessage;

               // lblNmsTopic.Content = recieved_data1;

                if (string.IsNullOrEmpty(recieved_data1))
                {
                    MessageBox.Show("No Heart Beat From MRI", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);


                    meterMriConn = "0";
                    sdStorage = "0";

                    sdUsed = "0";
                    sdUsedPercent = "0";
                    purgingDate = "0";

                    purgingTime = "0";
                    sdReadNo = "0";
                    sdWriteNo = "0";

                    netStrength = "0";
                    dataSentDate = "0";
                    dataSentTime = "0";


                    dataReceivedTime = "0";

                    string hexColor = "#FF0000";
                    Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);
                    lblMriHesConnLineTx.Foreground = brush;
                    lblMriHesConnLineRx.Foreground = brush;
                    lblMriHesConnArrowTx.Foreground = brush;
                    lblMriHesConnArrowRx.Foreground = brush;
                    iconMri.Foreground = brush;
                    //iconSim.Foreground = brush;

                    iconHeartBeatSuccess.Visibility = Visibility.Visible;

                    //iconSim.Visibility = Visibility.Collapsed;
                    NetStrength1.Visibility = Visibility.Collapsed;
                    NetStrength2.Visibility = Visibility.Collapsed;
                    NetStrength3.Visibility = Visibility.Collapsed;
                    NetStrength4.Visibility = Visibility.Collapsed;
                    iconSD.Visibility = Visibility.Collapsed;
                    //iconHeartBeatSuccess.Visibility = Visibility.Collapsed;
                    progressSd.Visibility = Visibility.Collapsed;
                    //progressSim.Visibility = Visibility.Collapsed;
                }


                else
                {
                    //lblNmsTopic.Content = recieved_data1;
                    //iconSim.Visibility = Visibility.Visible;
                    NetStrength1.Visibility = Visibility.Visible;
                    NetStrength2.Visibility = Visibility.Visible;
                    NetStrength3.Visibility = Visibility.Visible;
                    NetStrength4.Visibility = Visibility.Visible;
                    iconSD.Visibility = Visibility.Visible;
                    iconHeartBeatSuccess.Visibility = Visibility.Visible;
                    progressSd.Visibility = Visibility.Visible;
                    //progressSim.Visibility = Visibility.Visible;

                    string hexColor = "#00a5e3";
                    Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);

                    lblMriHesConnLineTx.Foreground = brush;
                    lblMriHesConnLineRx.Foreground = brush;
                    lblMriHesConnArrowTx.Foreground = brush;
                    lblMriHesConnArrowRx.Foreground = brush;
                    iconMri.Foreground = brush;
                    //iconSim.Foreground = brush;


                    string[] words = recieved_data1.Split(',');

                    meterMriConn = words[1];
                    sdStorage = words[2];

                    sdUsed = words[3];
                    sdUsedPercent = words[4];
                    sdReadNo = words[5];
                    sdWriteNo = words[6];

                    purgingDate = words[7];
                    purgingTime = words[8];
                   

                    netStrength = words[9];
                    dataSentDate = words[10];
                    dataSentTime = words[11];

                    MeterMRiConnEvent();
                    netStrengthEvent();
                    latencyEvent();
                    sdStatEvent();


                    timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += Timer_Tick;
                    timer.Start();
                }
            });


        }
        #endregion mqtt client


        #region Events

        private void Timer_Tick(object sender, EventArgs e)
        {

            iconHeartBeatSuccess.Visibility = Visibility.Visible;
            counter++;

            if (counter % 2 == 0)
            {
                iconHeartBeatSuccess.Height = 20;
                iconHeartBeatSuccess.Width = 20;
            }

            else if (counter % 2 != 0)
            {
                iconHeartBeatSuccess.Height = 16;
                iconHeartBeatSuccess.Width = 16;
            }

            else if (counter == 3600)
            {
                counter = 0;
                //timer.Stop();
            }
        }

        private void MeterMRiConnEvent()
        {
            if (meterMriConn.Equals("1") || meterMriConn.Equals("0"))
            {
                double meterMriConn1 = double.Parse(meterMriConn);
                if (meterMriConn1 == 1.00)
                {
                    string hexColor = "#00a5e3";
                    Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);
                    lblMeterMriConnLineTx.Foreground = brush;
                    lblMeterMriConnArrowTx.Foreground = brush;
                    lblMeterMriConnLineRx.Foreground = brush;
                    lblMeterMriConnArrowRx.Foreground = brush;
                    iconMeter.Foreground = brush;
                }


                else if (meterMriConn1 == 0.00)
                {

                    string hexColor = "#FF0000";
                    Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);
                    lblMeterMriConnLineTx.Foreground = brush;
                    lblMeterMriConnArrowTx.Foreground = brush;
                    lblMeterMriConnLineRx.Foreground = brush;
                    lblMeterMriConnArrowRx.Foreground = brush;
                    iconMeter.Foreground = brush;
                }
               
            }

            else
            {
                string hexColor = "#FF0000";
                Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);
                lblMeterMriConnLineTx.Foreground = brush;
                lblMeterMriConnArrowTx.Foreground = brush;
                lblMeterMriConnLineRx.Foreground = brush;
                lblMeterMriConnArrowRx.Foreground = brush;
                iconMeter.Foreground = brush;
            }

           
        }

        private void netStrengthEvent()
        {
            string hexColor = "#00cdac";
            Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);

            double netStrength1 = double.Parse(netStrength);
            if (netStrength1 <= 6)
            {
                NetStrength1.Fill = brush;
            }

            else if (netStrength1 >6 && netStrength1 <=12)
            {
                NetStrength1.Fill = brush;
                NetStrength2.Fill = brush;

            }

            else if (netStrength1 >12 && netStrength1 <=16)
            {
                NetStrength1.Fill = brush;
                NetStrength2.Fill = brush;
                NetStrength3.Fill = brush;

            }

            else if (netStrength1 >16 )
            {
                NetStrength1.Fill = brush;
                NetStrength2.Fill = brush;
                NetStrength3.Fill = brush;
                NetStrength4.Fill = brush;
            }
        }

        private void latencyEvent()
        {
            if (dataSentTime.Equals("Na"))
            {
                lblSentTime.Content = "Na";
                lblReceivedTime.Content = "Na";
            }

            else
            {
                lblSentTime.Content = dataSentTime;

                DateTime currentTime = DateTime.Now;
                string dataReceivedTime = currentTime.ToString("hh:mm tt");
                lblReceivedTime.Content = dataReceivedTime;

                DateTime sentTime = DateTime.ParseExact(dataSentTime, "hh:mm tt", CultureInfo.InvariantCulture);
                DateTime receivedTime = DateTime.ParseExact(dataReceivedTime, "hh:mm tt", CultureInfo.InvariantCulture);

                TimeSpan latency = receivedTime - sentTime;

                // Format the latency string to display total seconds with two decimal places
                string latencyString = latency.TotalSeconds.ToString("F2");

                lblNetLatency.Content = latencyString;
            }



        }
    
        private void sdStatEvent()
        {

            if (sdUsedPercent.Equals("Na"))
            {
                string hexColor = "#FF0000";
                Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);
                iconSD.Foreground = brush;

                lblSdUsedPercent.Content = "0";
                progressSd.Visibility = Visibility.Collapsed;
            }

            else 
            {

                string hexColor = "#00a5e3";
                Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);
                iconSD.Foreground = brush;

                lblSdUsedPercent.Content = sdUsedPercent;
                progressSd.Visibility = Visibility.Visible;
            }   
        }

        #endregion Events


        #region Export Fault And Accounting
        private void btnExport_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();

            // Set the background color of the context menu
            contextMenu.Background = (Brush)new BrushConverter().ConvertFrom("#202020");

            // Create a reusable style for the menu items
            Style menuItemStyle = new Style(typeof(MenuItem));
            menuItemStyle.Setters.Add(new Setter(ForegroundProperty, Brushes.White));
            menuItemStyle.Setters.Add(new Setter(FontSizeProperty, 12.0));

            // Export menu items
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
                    Subject = "NMS Data",
                    To = { new MailAddress(txtEmailAdress.Text) },
                    Body = "<html><body> Kindly find the attached NMS Data </body></html>",
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
                    var excelEngine = datagridNmsFaultMng.ExportToExcel(datagridNmsFaultMng.View, options);
                    var workBook = excelEngine.Excel.Workbooks[0];

                    // Save the Excel file to a temporary location
                    string tempFilePath = Path.GetTempFileName();
                    workBook.SaveAs(tempFilePath);
                    workBook.Close();

                    // Attach the Excel file to the email
                    Attachment attachment = new Attachment(tempFilePath);
                    attachment.ContentType.MediaType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    attachment.Name = "NMS_" + txtDtId.Text + "_From_" + dtPickerStart.Text + "_To_" + dtPickerEnd.Text + ".xlsx";

                    message2.Attachments.Add(attachment);

                    // Run the email sending logic on a background thread
                    await Task.Run(() => SendEmailWithAttachment(smtpClient, message2));

                    // The email sending is complete; you can now update UI or perform other tasks
                    MessageBox.Show("NMS Data  Successfully Sent", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);

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



        private void MenExportXls_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = datagridNmsFaultMng.ExportToExcel(datagridNmsFaultMng.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                FilterIndex = 2,
                Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx",
                FileName = "NMS Fault Management_" + txtDtId.Text + "_From_" + dtPickerStart.Text + "To_" + dtPickerEnd.Text + ".xlsx"

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

            var excelEngine = datagridNmsFaultMng.ExportToExcel(datagridNmsFaultMng.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV Files(*.csv)|*.csv",
                FileName = "NMS Data_" + txtDtId.Text + "_From_" + dtPickerStart.Text + "_To_" + dtPickerEnd.Text + ".csv"

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
            var document = datagridNmsFaultMng.ExportToPdf();
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Files(*.pdf)|*.pdf",
                FileName = "NMS Data_" + txtDtId.Text + "_From_" + dtPickerStart.Text + "_To_" + dtPickerEnd.Text + ".pdf"

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



        #endregion Export Fault And Accounting 

       
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (clientNms != null && clientNms.IsConnected)
            {
                clientNms.Disconnect();
            }

            (Parent as Grid).Children.Remove(this);

        }

       
    }
}
