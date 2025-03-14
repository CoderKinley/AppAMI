using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Windows.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Windows.Media;
using System.Net.Http;
using AppAMI.Classes;
using System.Collections.Generic;
using System.Linq;

namespace AppAMI.MeterData
{
    /// <summary>
    /// Interaction logic for InstantaneousCustom.xaml
    /// </summary>
    public partial class InstantaneousCustom : UserControl
    {
       

        #region public variable mqttClient
        private MqttClient client;
        private int successNumber = 0;
        private int unsuccessNumber = 0;
        private System.Timers.Timer timer;


        string mqttTopicInstant;
        #endregion public variable mqttClient

        //  DispatcherTimer timer = new DispatcherTimer();
        string CurrentUserId3;
        string CurrentUserRole3;
        string CurrentUserPassword3;
        string CurrentUserName3;
        string CurrentUserEmployeeId3;


        public InstantaneousCustom(string CurrentUserId2, string CurrentUserRole2, string CurrentUserPassword2, string CurrentUserName2, string CurrentUserEmployeeId2, string DTId2)
        {
            InitializeComponent();

            CurrentUserId3 = CurrentUserId2;
            CurrentUserPassword3 = CurrentUserPassword2;
            CurrentUserName3 = CurrentUserName2;
            CurrentUserEmployeeId3 = CurrentUserEmployeeId2;


            stkInstantData.Visibility = Visibility.Visible ;

            btnStartPolling.IsEnabled = false;
            btnStopPolling.IsEnabled = false;

            txtDtId.Loaded += TxtDtId_Loaded;
            txtUserRole.Loaded += TxtUserRole_Loaded;
        }

        private async  void TxtDtId_Loaded(object sender, RoutedEventArgs e)
        {
            await GetTopic();
        }

        private void TxtUserRole_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentUserRole3 = txtUserRole.Text;
        }

      


        private async Task GetTopic()
        {
            progressLogin.Visibility = Visibility.Visible;

            // Check if txtDtId is still loaded
            if (txtDtId.IsLoaded && string.IsNullOrEmpty(txtDtId.Text))
            {
                MessageBox.Show("Enter DT Id", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
                progressLogin.Visibility = Visibility.Collapsed;
                return;
            }

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string url = "http://103.234.126.43:3080/dtmeter/info/esd_essd_info/all";
                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    // Check if txtDtId is still loaded
                    if (txtDtId.IsLoaded)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string jsonResponse = await response.Content.ReadAsStringAsync();
                            List<MqttClass> mqttClasses = JsonConvert.DeserializeObject<List<MqttClass>>(jsonResponse);

                            string dtId = txtDtId.Text;
                            MqttClass filteredMqttClass = mqttClasses.FirstOrDefault(dt => dt.dt_id == dtId);

                            if (filteredMqttClass != null)
                            {
                                mqttTopicInstant = filteredMqttClass.instantaneous;
                                //lblData.Content = mqttTopicInstant;

                                btnStartPolling.IsEnabled = true;
                                btnStopPolling.IsEnabled = true;
                            }
                            else
                            {
                                MessageBox.Show("No Topic found, Enter Valid DT Id", "Topic Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);

                                btnStartPolling.IsEnabled = false;
                                btnStopPolling.IsEnabled = false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressLogin.Visibility = Visibility.Collapsed;
            }
        }

        #region mqtt method
        private void btnStartPolling_Click(object sender, RoutedEventArgs e)
        {
            mqttInitializeMethod();

            btnStartPolling.Visibility = Visibility.Hidden;
            btnPausePolling.Visibility = Visibility.Visible;



            lblPollingStatus.Content = "Reading Data";

            sfProgress.Progress = 50;
            sfProgress.Progress = 50;
            sfProgress.IsIndeterminate = true;
            sfProgress.Width = 250;
            sfProgress.Height = 4;
            sfProgress.AnimationDuration = new TimeSpan(0, 0, 1);
        }
  
        private void mqttInitializeMethod()
        {
            client = new MqttClient("103.234.126.44");
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            client.Connect(Guid.NewGuid().ToString());

            lblConnStatus.Content = "Connected to MQTT Server";
            string hexColor = "#00cdac";
            Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);
            lblConnStatus.Foreground = brush;

            client.Subscribe(new string[] { mqttTopicInstant }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
            //client.Subscribe(new string[] { "DTDemoPunakha1" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });


            timer = new System.Timers.Timer();
            timer.Interval = 300000; // 5 minutes
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false; // only fire once
        }

        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Message);
            Dispatcher.Invoke(() =>
            {
                message = message.Replace("N/a", "0.00");
               // lblData.Content = message;
                string[] words = message.Split(',');

                if (message.Equals("NC"))
                {
                    lblConnStatus.Content = "Meter Disconnected";

                    string hexColor = "#ff5768";
                    Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);
                    lblConnStatus.Foreground = brush;
                }

                else
                {
                    lblConnStatus.Content = "Meter Connected";

                    string hexColor = "#00cdac";
                    Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);
                    lblConnStatus.Foreground = brush;

                    lblPhaseAInstantaneousCurrentA.Content = words[3];
                    lblPhaseBInstantaneousCurrentA.Content = words[4];
                    lblPhaseCInstantaneousCurrentA.Content = words[5];

                    lblPhaseAInstantaneousVoltageV.Content = words[6];
                    lblPhaseBInstantaneousVoltageV.Content = words[7];
                    lblPhaseCInstantaneousVoltageV.Content = words[8];

                    lblInstantaneousPowerFactor.Content = words[9];

                    lblInstantaneousImportActivePowerkW.Content = words[10];
                    lblInstantaneousExportActivePowerkW.Content = words[11];
                    lblInstantaneousImportReactivePowerkvar.Content = words[12];
                    lblInstantaneousExportReactivePowerkvar.Content = words[13];
                    lblInstantaneousImportApparentPowerkVA.Content = words[14];
                    lblInstantaneousExportApparentPowerkVA.Content = words[15];

                    lblInstantaneousTotalActivePowerkW.Content = words[16];
                    lblTotalReactivePowerkvar.Content = words[17];
                    lblTotalApparentPowerkVA.Content = words[18];
                    lblInstantaneousNetActivePowerkW.Content = words[19];

                    lblPhaseAInstantaneousImportActivePowerkW.Content = words[20];
                    lblPhaseAInstantaneousExportActivePowerkW.Content = words[21];
                    lblPhaseAInstantaneousImportReactivePowerkvar.Content = words[22];
                    lblPhaseAInstantaneousExportReactivePowerkvar.Content = words[23];
                    lblPhaseAInstantaneousImportApparentPowerkVA.Content = words[24];
                    lblPhaseAInstantaneousExportApparentPowerkVA.Content = words[25];

                    lblPhaseBInstantaneousImportActivePowerkW.Content = words[26];
                    lblPhaseBInstantaneousExportActivePowerkW.Content = words[27];
                    lblPhaseBInstantaneousImportReactivePowerkvar.Content = words[28];
                    lblPhaseBInstantaneousExportReactivePowerkvar.Content = words[29];
                    lblPhaseBInstantaneousImportApparentPowerkVA.Content = words[30];
                    lblPhaseBInstantaneousExportApparentPowerkVA.Content = words[31];

                    lblPhaseCInstantaneousImportActivePowerkW.Content = words[32];
                    lblPhaseCInstantaneousExportActivePowerkW.Content = words[33];
                    lblPhaseCInstantaneousImportReactivePowerkvar.Content = words[34];
                    lblPhaseCInstantaneousExportReactivePowerkvar.Content = words[35];
                    lblPhaseCInstantaneousImportApparentPowerkVA.Content = words[36];
                    lblPhaseCInstantaneousExportApparentPowerkVA.Content = words[37];

                    lblImportActiveCurrentAverageDemandkW.Content = words[38];
                    lblExportActiveCurrentAverageDemandkW.Content = words[39];
                    lblImportReactiveCurrentAverageDemandkvar.Content = words[40];
                    lblExportReactiveCurrentAverageDemandkvar.Content = words[41];
                    lblImportApparentCurrentAverageDemandkVA.Content = words[42];
                    lblExportApparentCurrentAverageDemandkVA.Content = words[43];

                    lblDate.Content = words[44];
                    lblTime.Content = words[45];



                    stkInstantData.Visibility = Visibility.Visible;

                    successNumber++;

                    Dispatcher.Invoke(() => lblSuccessfulPolls.Content = successNumber);

                    Dispatcher.Invoke(() => lblTotalPolls.Content = successNumber + unsuccessNumber);

                    timer.Stop();
                    timer.Start();
                }

            });

           
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() => {
                unsuccessNumber++;
                lblSuccessfulPolls.Content = successNumber;
                lblFailedPolls.Content = unsuccessNumber;
                lblTotalPolls.Content = successNumber + unsuccessNumber;
            });
        }

        private void btnPausePolling_Click(object sender, RoutedEventArgs e)
        {
            if (client != null && client.IsConnected)
            {
                client.Unsubscribe(new string[] { "meter" }); // Unsubscribe from the topic
            }

            if (timer != null)
            {
                timer.Stop(); // Stop the timer
            }

            lblConnStatus.Content = "Paused";

            btnPausePolling.Visibility = Visibility.Hidden;
            btnStartPolling.Visibility = Visibility.Visible;


            lblPollingStatus.Content = "Reading Paused";

            sfProgress.AnimationDuration = new TimeSpan(0, 0, 0);
        }

        private void btnStopPolling_Click(object sender, RoutedEventArgs e)
        {

            if (client != null && client.IsConnected)
            {
                client.Disconnect();
                // lblConnStatus.Content = "Disconnected from MQTT broker";
            }

            if (timer != null)
            {
                timer.Stop();
            }

            successNumber = 0;
            unsuccessNumber = 0;

            lblSuccessfulPolls.Content = successNumber;
            lblFailedPolls.Content = unsuccessNumber;
            lblTotalPolls.Content = successNumber + unsuccessNumber;

            lblPollingStatus.Content = "Reading Stopped";

            btnPausePolling.Visibility = Visibility.Hidden;
            btnStartPolling.Visibility = Visibility.Visible;

            sfProgress.AnimationDuration = new TimeSpan(0, 0, 0);
        }

        #endregion mqtt method


        #region scroll Viewer Synchronization

        private void scrollVSelectrdPara_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            // Reduce the speed of the scrolling by a factor of 2

            if (e.Handled)
            {
                return;
            }

            e.Handled = true;

            ScrollViewer scrollViewer = sender as ScrollViewer;

            if (scrollViewer != null)
            {
                // Reduce the speed of the scrolling by a factor of 2
                double delta = e.Delta / 2.0;

                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - delta);

                ScrollViewer scrollVData = FindVisualChild<ScrollViewer>(this, "scrollVData");

                if (scrollVData != null)
                {
                    scrollVData.ScrollToVerticalOffset(scrollViewer.VerticalOffset - delta);
                }
            }


            //if (e.Handled)
            //    return;

            //e.Handled = true;

            //var scrollViewer = sender as ScrollViewer;

            //if (scrollViewer != null)
            //{
            //    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);

            //    // Find the other ScrollViewer named "scrollVData"
            //    var scrollVData = FindVisualChild<ScrollViewer>(this, "scrollVData");

            //    if (scrollVData != null)
            //    {
            //        // Scroll the other ScrollViewer at the same time
            //        scrollVData.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            //    }
            //}
        }


        public static T FindVisualChild<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            int count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T && (child as FrameworkElement)?.Name == name)
                {
                    return child as T;
                }

                T result = FindVisualChild<T>(child, name);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        #endregion scroll Viewer Synchronization


        #region CheckBox EventHandler

        #region Parent 
        private void checkParentCheckBox()
        {
            cbParent.IsChecked = null;
            if (

                (cbDate.IsChecked == true) &&
                (cbTime.IsChecked == true) &&

                (cbPhaseAInstantaneousCurrentA.IsChecked == true) &&
                (cbPhaseBInstantaneousCurrentA.IsChecked == true) &&
                (cbPhaseCInstantaneousCurrentA.IsChecked == true) &&

                (cbPhaseAInstantaneousVoltageV.IsChecked == true) &&
                (cbPhaseBInstantaneousVoltageV.IsChecked == true) &&
                (cbPhaseCInstantaneousVoltageV.IsChecked == true) &&

                (cbInstantaneousPowerFactor.IsChecked == true) &&

                (cbInstantaneousImportActivePowerkW.IsChecked == true) &&
                (cbInstantaneousExportActivePowerkW.IsChecked == true) &&
                (cbInstantaneousImportReactivePowerkvar.IsChecked == true) &&
                (cbInstantaneousExportReactivePowerkvar.IsChecked == true) &&
                (cbInstantaneousImportApparentPowerkVA.IsChecked == true) &&
                (cbInstantaneousExportApparentPowerkVA.IsChecked == true) &&

                (cbInstantaneousTotalActivePowerkW.IsChecked == true) &&
                (cbTotalReactivePowerkvar.IsChecked == true) &&
                (cbTotalApparentPowerkVA.IsChecked == true) &&
                (cbInstantaneousNetActivePowerkW.IsChecked == true) &&

                (cbPhaseAInstantaneousImportActivePowerkW.IsChecked == true) &&
                (cbPhaseAInstantaneousExportActivePowerkW.IsChecked == true) &&
                (cbPhaseAInstantaneousImportReactivePowerkvar.IsChecked == true) &&
                (cbPhaseAInstantaneousExportReactivePowerkvar.IsChecked == true) &&
                (cbPhaseAInstantaneousImportApparentPowerkVA.IsChecked == true) &&
                (cbPhaseAInstantaneousExportApparentPowerkVA.IsChecked == true) &&


                (cbPhaseBInstantaneousImportActivePowerkW.IsChecked == true) &&
                (cbPhaseBInstantaneousExportActivePowerkW.IsChecked == true) &&
                (cbPhaseBInstantaneousImportReactivePowerkvar.IsChecked == true) &&
                (cbPhaseBInstantaneousExportReactivePowerkvar.IsChecked == true) &&
                (cbPhaseBInstantaneousImportApparentPowerkVA.IsChecked == true) &&
                (cbPhaseBInstantaneousExportApparentPowerkVA.IsChecked == true) &&

                (cbPhaseCInstantaneousImportActivePowerkW.IsChecked == true) &&
                (cbPhaseCInstantaneousExportActivePowerkW.IsChecked == true) &&
                (cbPhaseCInstantaneousImportReactivePowerkvar.IsChecked == true) &&
                (cbPhaseCInstantaneousExportReactivePowerkvar.IsChecked == true) &&
                (cbPhaseCInstantaneousImportApparentPowerkVA.IsChecked == true) &&
                (cbPhaseCInstantaneousExportApparentPowerkVA.IsChecked == true) &&

                (cbImportActiveCurrentAverageDemandkW.IsChecked == true) &&
                (cbExportActiveCurrentAverageDemandkW.IsChecked == true) &&
                (cbImportReactiveCurrentAverageDemandkvar.IsChecked == true) &&
                (cbExportReactiveCurrentAverageDemandkvar.IsChecked == true) &&
                (cbImportApparentCurrentAverageDemandkVA.IsChecked == true) &&
                (cbExportApparentCurrentAverageDemandkVA.IsChecked == true) 

                 )

            {
                cbParent.IsChecked = true;
            }
        }
   
        private void uncheckParentCheckBox()
        {

            cbParent.IsChecked = null;
            if (
                

                (cbDate.IsChecked == false) &&
                (cbTime.IsChecked == false) &&

                (cbPhaseAInstantaneousCurrentA.IsChecked == false) &&
                (cbPhaseBInstantaneousCurrentA.IsChecked == false) &&
                (cbPhaseCInstantaneousCurrentA.IsChecked == false) &&

                (cbPhaseAInstantaneousVoltageV.IsChecked == false) &&
                (cbPhaseBInstantaneousVoltageV.IsChecked == false) &&
                (cbPhaseCInstantaneousVoltageV.IsChecked == false) &&

                (cbInstantaneousPowerFactor.IsChecked == false) &&

                (cbInstantaneousImportActivePowerkW.IsChecked == false) &&
                (cbInstantaneousExportActivePowerkW.IsChecked == false) &&
                (cbInstantaneousImportReactivePowerkvar.IsChecked == false) &&
                (cbInstantaneousExportReactivePowerkvar.IsChecked == false) &&
                (cbInstantaneousImportApparentPowerkVA.IsChecked == false) &&
                (cbInstantaneousExportApparentPowerkVA.IsChecked == false) &&

                (cbInstantaneousTotalActivePowerkW.IsChecked == false) &&
                (cbTotalReactivePowerkvar.IsChecked == false) &&
                (cbTotalApparentPowerkVA.IsChecked == false) &&
                (cbInstantaneousNetActivePowerkW.IsChecked == false) &&

                (cbPhaseAInstantaneousImportActivePowerkW.IsChecked == false) &&
                (cbPhaseAInstantaneousExportActivePowerkW.IsChecked == false) &&
                (cbPhaseAInstantaneousImportReactivePowerkvar.IsChecked == false) &&
                (cbPhaseAInstantaneousExportReactivePowerkvar.IsChecked == false) &&
                (cbPhaseAInstantaneousImportApparentPowerkVA.IsChecked == false) &&
                (cbPhaseAInstantaneousExportApparentPowerkVA.IsChecked == false) &&


                (cbPhaseBInstantaneousImportActivePowerkW.IsChecked == false) &&
                (cbPhaseBInstantaneousExportActivePowerkW.IsChecked == false) &&
                (cbPhaseBInstantaneousImportReactivePowerkvar.IsChecked == false) &&
                (cbPhaseBInstantaneousExportReactivePowerkvar.IsChecked == false) &&
                (cbPhaseBInstantaneousImportApparentPowerkVA.IsChecked == false) &&
                (cbPhaseBInstantaneousExportApparentPowerkVA.IsChecked == false) &&

                (cbPhaseCInstantaneousImportActivePowerkW.IsChecked == false) &&
                (cbPhaseCInstantaneousExportActivePowerkW.IsChecked == false) &&
                (cbPhaseCInstantaneousImportReactivePowerkvar.IsChecked == false) &&
                (cbPhaseCInstantaneousExportReactivePowerkvar.IsChecked == false) &&
                (cbPhaseCInstantaneousImportApparentPowerkVA.IsChecked == false) &&
                (cbPhaseCInstantaneousExportApparentPowerkVA.IsChecked == false) &&

                (cbImportActiveCurrentAverageDemandkW.IsChecked == false) &&
                (cbExportActiveCurrentAverageDemandkW.IsChecked == false) &&
                (cbImportReactiveCurrentAverageDemandkvar.IsChecked == false) &&
                (cbExportReactiveCurrentAverageDemandkvar.IsChecked == false) &&
                (cbImportApparentCurrentAverageDemandkVA.IsChecked == false) &&
                (cbExportApparentCurrentAverageDemandkVA.IsChecked == false)


                 )

            {
                cbParent.IsChecked = false;
            }


           
        }

        private void cbParent_Checked(object sender, RoutedEventArgs e)
        {
            bool newVal = cbParent.IsChecked == true;

            cbDate.IsChecked = newVal;
            cbTime.IsChecked = newVal;

            cbPhaseAInstantaneousCurrentA.IsChecked = newVal;
            cbPhaseBInstantaneousCurrentA.IsChecked = newVal;
            cbPhaseCInstantaneousCurrentA.IsChecked = newVal;

            cbPhaseAInstantaneousVoltageV.IsChecked = newVal;
            cbPhaseBInstantaneousVoltageV.IsChecked = newVal;
            cbPhaseCInstantaneousVoltageV.IsChecked = newVal;

            cbInstantaneousPowerFactor.IsChecked = newVal;

            cbInstantaneousImportActivePowerkW.IsChecked = newVal;
            cbInstantaneousExportActivePowerkW.IsChecked = newVal;
            cbInstantaneousImportReactivePowerkvar.IsChecked = newVal;
            cbInstantaneousExportReactivePowerkvar.IsChecked = newVal;
            cbInstantaneousImportApparentPowerkVA.IsChecked = newVal;
            cbInstantaneousExportApparentPowerkVA.IsChecked = newVal;

            cbInstantaneousTotalActivePowerkW.IsChecked = newVal;
            cbTotalReactivePowerkvar.IsChecked = newVal;
            cbTotalApparentPowerkVA.IsChecked = newVal;
            cbInstantaneousNetActivePowerkW.IsChecked = newVal;

            cbPhaseAInstantaneousImportActivePowerkW.IsChecked = newVal;
            cbPhaseAInstantaneousExportActivePowerkW.IsChecked = newVal;
            cbPhaseAInstantaneousImportReactivePowerkvar.IsChecked = newVal;
            cbPhaseAInstantaneousExportReactivePowerkvar.IsChecked = newVal;
            cbPhaseAInstantaneousImportApparentPowerkVA.IsChecked = newVal;
            cbPhaseAInstantaneousExportApparentPowerkVA.IsChecked = newVal;

            cbPhaseBInstantaneousImportActivePowerkW.IsChecked = newVal;
            cbPhaseBInstantaneousExportActivePowerkW.IsChecked = newVal;
            cbPhaseBInstantaneousImportReactivePowerkvar.IsChecked = newVal;
            cbPhaseBInstantaneousExportReactivePowerkvar.IsChecked = newVal;
            cbPhaseBInstantaneousImportApparentPowerkVA.IsChecked = newVal;
            cbPhaseBInstantaneousExportApparentPowerkVA.IsChecked = newVal;

            cbPhaseCInstantaneousImportActivePowerkW.IsChecked = newVal;
            cbPhaseCInstantaneousExportActivePowerkW.IsChecked = newVal;
            cbPhaseCInstantaneousImportReactivePowerkvar.IsChecked = newVal;
            cbPhaseCInstantaneousExportReactivePowerkvar.IsChecked = newVal;
            cbPhaseCInstantaneousImportApparentPowerkVA.IsChecked = newVal;
            cbPhaseCInstantaneousExportApparentPowerkVA.IsChecked = newVal;

            cbImportActiveCurrentAverageDemandkW.IsChecked = newVal;
            cbExportActiveCurrentAverageDemandkW.IsChecked = newVal;
            cbImportReactiveCurrentAverageDemandkvar.IsChecked = newVal;
            cbExportReactiveCurrentAverageDemandkvar.IsChecked = newVal;
            cbImportApparentCurrentAverageDemandkVA.IsChecked = newVal;
            cbExportApparentCurrentAverageDemandkVA.IsChecked = newVal;
        }

        private void cbParent_Unchecked(object sender, RoutedEventArgs e)
        {
            bool newVal = cbParent.IsChecked == true;

            cbDate.IsChecked = newVal;
            cbTime.IsChecked = newVal;

            cbPhaseAInstantaneousCurrentA.IsChecked = newVal;
            cbPhaseBInstantaneousCurrentA.IsChecked = newVal;
            cbPhaseCInstantaneousCurrentA.IsChecked = newVal;

            cbPhaseAInstantaneousVoltageV.IsChecked = newVal;
            cbPhaseBInstantaneousVoltageV.IsChecked = newVal;
            cbPhaseCInstantaneousVoltageV.IsChecked = newVal;

            cbInstantaneousPowerFactor.IsChecked = newVal;

            cbInstantaneousImportActivePowerkW.IsChecked = newVal;
            cbInstantaneousExportActivePowerkW.IsChecked = newVal;
            cbInstantaneousImportReactivePowerkvar.IsChecked = newVal;
            cbInstantaneousExportReactivePowerkvar.IsChecked = newVal;
            cbInstantaneousImportApparentPowerkVA.IsChecked = newVal;
            cbInstantaneousExportApparentPowerkVA.IsChecked = newVal;

            cbInstantaneousTotalActivePowerkW.IsChecked = newVal;
            cbTotalReactivePowerkvar.IsChecked = newVal;
            cbTotalApparentPowerkVA.IsChecked = newVal;
            cbInstantaneousNetActivePowerkW.IsChecked = newVal;

            cbPhaseAInstantaneousImportActivePowerkW.IsChecked = newVal;
            cbPhaseAInstantaneousExportActivePowerkW.IsChecked = newVal;
            cbPhaseAInstantaneousImportReactivePowerkvar.IsChecked = newVal;
            cbPhaseAInstantaneousExportReactivePowerkvar.IsChecked = newVal;
            cbPhaseAInstantaneousImportApparentPowerkVA.IsChecked = newVal;
            cbPhaseAInstantaneousExportApparentPowerkVA.IsChecked = newVal;

            cbPhaseBInstantaneousImportActivePowerkW.IsChecked = newVal;
            cbPhaseBInstantaneousExportActivePowerkW.IsChecked = newVal;
            cbPhaseBInstantaneousImportReactivePowerkvar.IsChecked = newVal;
            cbPhaseBInstantaneousExportReactivePowerkvar.IsChecked = newVal;
            cbPhaseBInstantaneousImportApparentPowerkVA.IsChecked = newVal;
            cbPhaseBInstantaneousExportApparentPowerkVA.IsChecked = newVal;

            cbPhaseCInstantaneousImportActivePowerkW.IsChecked = newVal;
            cbPhaseCInstantaneousExportActivePowerkW.IsChecked = newVal;
            cbPhaseCInstantaneousImportReactivePowerkvar.IsChecked = newVal;
            cbPhaseCInstantaneousExportReactivePowerkvar.IsChecked = newVal;
            cbPhaseCInstantaneousImportApparentPowerkVA.IsChecked = newVal;
            cbPhaseCInstantaneousExportApparentPowerkVA.IsChecked = newVal;

            cbImportActiveCurrentAverageDemandkW.IsChecked = newVal;
            cbExportActiveCurrentAverageDemandkW.IsChecked = newVal;
            cbImportReactiveCurrentAverageDemandkvar.IsChecked = newVal;
            cbExportReactiveCurrentAverageDemandkvar.IsChecked = newVal;
            cbImportApparentCurrentAverageDemandkVA.IsChecked = newVal;
            cbExportApparentCurrentAverageDemandkVA.IsChecked = newVal;



        }

        #endregion Parent

        #region date time 
    
        private void cbDate_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbDate.Content);
            lblDate.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbDate_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbDate.Content);
            lblDate.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbTime_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTime.Content);
            lblTime.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbTime_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTime.Content);
            lblTime.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        #endregion date time

        #region Current 

        private void cbPhaseAInstantaneousCurrentA_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAInstantaneousCurrentA.Content);
            lblPhaseAInstantaneousCurrentA.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseAInstantaneousCurrentA_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAInstantaneousCurrentA.Content);
            lblPhaseAInstantaneousCurrentA.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseBInstantaneousCurrentA_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBInstantaneousCurrentA.Content);
            lblPhaseBInstantaneousCurrentA.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseBInstantaneousCurrentA_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBInstantaneousCurrentA.Content);
            lblPhaseBInstantaneousCurrentA.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseCInstantaneousCurrentA_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCInstantaneousCurrentA.Content);
            lblPhaseCInstantaneousCurrentA.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseCInstantaneousCurrentA_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCInstantaneousCurrentA.Content);
            lblPhaseCInstantaneousCurrentA.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }


        #endregion Current

        #region Voltage

        private void cbPhaseAInstantaneousVoltageV_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAInstantaneousVoltageV.Content);
            lblPhaseAInstantaneousVoltageV.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseAInstantaneousVoltageV_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAInstantaneousVoltageV.Content);
            lblPhaseAInstantaneousVoltageV.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseBInstantaneousVoltageV_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBInstantaneousVoltageV.Content);
            lblPhaseBInstantaneousVoltageV.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseBInstantaneousVoltageV_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBInstantaneousVoltageV.Content);
            lblPhaseBInstantaneousVoltageV.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseCInstantaneousVoltageV_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCInstantaneousVoltageV.Content);
            lblPhaseCInstantaneousVoltageV.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseCInstantaneousVoltageV_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCInstantaneousVoltageV.Content);
            lblPhaseCInstantaneousVoltageV.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        #endregion Voltage

        #region Power Factor

        private void cbInstantaneousPowerFactor_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbInstantaneousPowerFactor.Content);
            lblInstantaneousPowerFactor.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbInstantaneousPowerFactor_Unchecked(object sender, RoutedEventArgs e)
        {

            lvSelectedPara.Items.Remove(cbInstantaneousPowerFactor.Content);
            lblInstantaneousPowerFactor.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        #endregion Power Factor

        #region Instantaneous Import Export
        private void cbInstantaneousImportActivePowerkW_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbInstantaneousImportActivePowerkW.Content);
            lblInstantaneousImportActivePowerkW.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbInstantaneousImportActivePowerkW_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbInstantaneousImportActivePowerkW.Content);
            lblInstantaneousImportActivePowerkW.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbInstantaneousExportActivePowerkW_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbInstantaneousExportActivePowerkW.Content);
            lblInstantaneousExportActivePowerkW.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbInstantaneousExportActivePowerkW_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbInstantaneousExportActivePowerkW.Content);
            lblInstantaneousExportActivePowerkW.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbInstantaneousImportReactivePowerkvar_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbInstantaneousImportReactivePowerkvar.Content);
            lblInstantaneousImportReactivePowerkvar.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbInstantaneousImportReactivePowerkvar_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbInstantaneousImportReactivePowerkvar.Content);
            lblInstantaneousImportReactivePowerkvar.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbInstantaneousExportReactivePowerkvar_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbInstantaneousExportReactivePowerkvar.Content);
            lblInstantaneousExportReactivePowerkvar.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbInstantaneousExportReactivePowerkvar_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbInstantaneousExportReactivePowerkvar.Content);
            lblInstantaneousExportReactivePowerkvar.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbInstantaneousImportApparentPowerkVA_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbInstantaneousImportApparentPowerkVA.Content);
            lblInstantaneousImportApparentPowerkVA.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbInstantaneousImportApparentPowerkVA_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbInstantaneousImportApparentPowerkVA.Content);
            lblInstantaneousImportApparentPowerkVA.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbInstantaneousExportApparentPowerkVA_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbInstantaneousExportApparentPowerkVA.Content);
            lblInstantaneousExportApparentPowerkVA.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbInstantaneousExportApparentPowerkVA_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbInstantaneousExportApparentPowerkVA.Content);
            lblInstantaneousExportApparentPowerkVA.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        #endregion Instantaneous Import Export

        #region Total Power

        private void cbInstantaneousTotalActivePowerkW_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbInstantaneousTotalActivePowerkW.Content);
            lblInstantaneousTotalActivePowerkW.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbInstantaneousTotalActivePowerkW_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbInstantaneousTotalActivePowerkW.Content);
            lblInstantaneousTotalActivePowerkW.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbTotalReactivePowerkvar_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotalReactivePowerkvar.Content);
            lblTotalReactivePowerkvar.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbTotalReactivePowerkvar_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotalReactivePowerkvar.Content);
            lblTotalReactivePowerkvar.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbTotalApparentPowerkVA_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotalApparentPowerkVA.Content);
            lblTotalApparentPowerkVA.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbTotalApparentPowerkVA_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotalApparentPowerkVA.Content);
            lblTotalApparentPowerkVA.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbInstantaneousNetActivePowerkW_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbInstantaneousNetActivePowerkW.Content);
            lblInstantaneousNetActivePowerkW.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbInstantaneousNetActivePowerkW_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbInstantaneousNetActivePowerkW.Content);
            lblInstantaneousNetActivePowerkW.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        #endregion Total Power

        #region Instantaneous Phase A Import Export

        private void cbPhaseAInstantaneousImportActivePowerkW_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAInstantaneousImportActivePowerkW.Content);
            lblPhaseAInstantaneousImportActivePowerkW.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseAInstantaneousImportActivePowerkW_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAInstantaneousImportActivePowerkW.Content);
            lblPhaseAInstantaneousImportActivePowerkW.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseAInstantaneousExportActivePowerkW_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAInstantaneousExportActivePowerkW.Content);
            lblPhaseAInstantaneousExportActivePowerkW.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseAInstantaneousExportActivePowerkW_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAInstantaneousExportActivePowerkW.Content);
            lblPhaseAInstantaneousExportActivePowerkW.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseAInstantaneousImportReactivePowerkvar_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAInstantaneousImportReactivePowerkvar.Content);
            lblPhaseAInstantaneousImportReactivePowerkvar.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseAInstantaneousImportReactivePowerkvar_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAInstantaneousImportReactivePowerkvar.Content);
            lblPhaseAInstantaneousImportReactivePowerkvar.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseAInstantaneousExportReactivePowerkvar_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAInstantaneousExportReactivePowerkvar.Content);
            lblPhaseAInstantaneousExportReactivePowerkvar.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseAInstantaneousExportReactivePowerkvar_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAInstantaneousExportReactivePowerkvar.Content);
            lblPhaseAInstantaneousExportReactivePowerkvar.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseAInstantaneousImportApparentPowerkVA_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAInstantaneousImportApparentPowerkVA.Content);
            lblPhaseAInstantaneousImportApparentPowerkVA.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseAInstantaneousImportApparentPowerkVA_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAInstantaneousImportApparentPowerkVA.Content);
            lblPhaseAInstantaneousImportApparentPowerkVA.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseAInstantaneousExportApparentPowerkVA_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAInstantaneousExportApparentPowerkVA.Content);
            lblPhaseAInstantaneousExportApparentPowerkVA.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseAInstantaneousExportApparentPowerkVA_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAInstantaneousExportApparentPowerkVA.Content);
            lblPhaseAInstantaneousExportApparentPowerkVA.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        #endregion Instantaneous Phase A Import Export

        #region Instantaneous Phase B Import Export
        private void cbPhaseBInstantaneousImportActivePowerkW_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBInstantaneousImportActivePowerkW.Content);
            lblPhaseBInstantaneousImportActivePowerkW.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseBInstantaneousImportActivePowerkW_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBInstantaneousImportActivePowerkW.Content);
            lblPhaseBInstantaneousImportActivePowerkW.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseBInstantaneousExportActivePowerkW_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBInstantaneousExportActivePowerkW.Content);
            lblPhaseBInstantaneousExportActivePowerkW.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseBInstantaneousExportActivePowerkW_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBInstantaneousExportActivePowerkW.Content);
            lblPhaseBInstantaneousExportActivePowerkW.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseBInstantaneousImportReactivePowerkvar_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBInstantaneousImportReactivePowerkvar.Content);
            lblPhaseBInstantaneousImportReactivePowerkvar.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseBInstantaneousImportReactivePowerkvar_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBInstantaneousImportReactivePowerkvar.Content);
            lblPhaseBInstantaneousImportReactivePowerkvar.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseBInstantaneousExportReactivePowerkvar_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBInstantaneousExportReactivePowerkvar.Content);
            lblPhaseBInstantaneousExportReactivePowerkvar.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseBInstantaneousExportReactivePowerkvar_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBInstantaneousExportReactivePowerkvar.Content);
            lblPhaseBInstantaneousExportReactivePowerkvar.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseBInstantaneousImportApparentPowerkVA_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBInstantaneousImportApparentPowerkVA.Content);
            lblPhaseBInstantaneousImportApparentPowerkVA.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseBInstantaneousImportApparentPowerkVA_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBInstantaneousImportApparentPowerkVA.Content);
            lblPhaseBInstantaneousImportApparentPowerkVA.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseBInstantaneousExportApparentPowerkVA_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBInstantaneousExportApparentPowerkVA.Content);
            lblPhaseBInstantaneousExportApparentPowerkVA.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseBInstantaneousExportApparentPowerkVA_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBInstantaneousExportApparentPowerkVA.Content);
            lblPhaseBInstantaneousExportApparentPowerkVA.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        #endregion Instantaneous Phase B Import Export


        #region Instantaneous Phase C Import Export

        private void cbPhaseCInstantaneousImportActivePowerkW_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCInstantaneousImportActivePowerkW.Content);
            lblPhaseCInstantaneousImportActivePowerkW.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseCInstantaneousImportActivePowerkW_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCInstantaneousImportActivePowerkW.Content);
            lblPhaseCInstantaneousImportActivePowerkW.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseCInstantaneousExportActivePowerkW_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCInstantaneousExportActivePowerkW.Content);
            lblPhaseCInstantaneousExportActivePowerkW.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseCInstantaneousExportActivePowerkW_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCInstantaneousExportActivePowerkW.Content);
            lblPhaseCInstantaneousExportActivePowerkW.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseCInstantaneousImportReactivePowerkvar_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCInstantaneousImportReactivePowerkvar.Content);
            lblPhaseCInstantaneousImportReactivePowerkvar.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseCInstantaneousImportReactivePowerkvar_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCInstantaneousImportReactivePowerkvar.Content);
            lblPhaseCInstantaneousImportReactivePowerkvar.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseCInstantaneousExportReactivePowerkvar_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCInstantaneousExportReactivePowerkvar.Content);
            lblPhaseCInstantaneousExportReactivePowerkvar.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseCInstantaneousExportReactivePowerkvar_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCInstantaneousExportReactivePowerkvar.Content);
            lblPhaseCInstantaneousExportReactivePowerkvar.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseCInstantaneousImportApparentPowerkVA_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCInstantaneousImportApparentPowerkVA.Content);
            lblPhaseCInstantaneousImportApparentPowerkVA.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseCInstantaneousImportApparentPowerkVA_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCInstantaneousImportApparentPowerkVA.Content);
            lblPhaseCInstantaneousImportApparentPowerkVA.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbPhaseCInstantaneousExportApparentPowerkVA_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCInstantaneousExportApparentPowerkVA.Content);
            lblPhaseCInstantaneousExportApparentPowerkVA.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbPhaseCInstantaneousExportApparentPowerkVA_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCInstantaneousExportApparentPowerkVA.Content);
            lblPhaseCInstantaneousExportApparentPowerkVA.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        #endregion Instantaneous Phase C Import Export

        #region Average Demand

        private void cbImportActiveCurrentAverageDemandkW_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveCurrentAverageDemandkW.Content);
            lblImportActiveCurrentAverageDemandkW.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbImportActiveCurrentAverageDemandkW_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveCurrentAverageDemandkW.Content);
            lblImportActiveCurrentAverageDemandkW.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbExportActiveCurrentAverageDemandkW_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbExportActiveCurrentAverageDemandkW.Content);
            lblExportActiveCurrentAverageDemandkW.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbExportActiveCurrentAverageDemandkW_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbExportActiveCurrentAverageDemandkW.Content);
            lblExportActiveCurrentAverageDemandkW.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbImportReactiveCurrentAverageDemandkvar_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportReactiveCurrentAverageDemandkvar.Content);
            lblImportReactiveCurrentAverageDemandkvar.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbImportReactiveCurrentAverageDemandkvar_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportReactiveCurrentAverageDemandkvar.Content);
            lblImportReactiveCurrentAverageDemandkvar.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbExportReactiveCurrentAverageDemandkvar_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbExportReactiveCurrentAverageDemandkvar.Content);
            lblExportReactiveCurrentAverageDemandkvar.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbExportReactiveCurrentAverageDemandkvar_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbExportReactiveCurrentAverageDemandkvar.Content);
            lblExportReactiveCurrentAverageDemandkvar.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbImportApparentCurrentAverageDemandkVA_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportApparentCurrentAverageDemandkVA.Content);
            lblImportApparentCurrentAverageDemandkVA.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbImportApparentCurrentAverageDemandkVA_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportApparentCurrentAverageDemandkVA.Content);
            lblImportApparentCurrentAverageDemandkVA.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        private void cbExportApparentCurrentAverageDemandkVA_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbExportApparentCurrentAverageDemandkVA.Content);
            lblExportApparentCurrentAverageDemandkVA.Visibility = Visibility.Visible;
            checkParentCheckBox();
        }

        private void cbExportApparentCurrentAverageDemandkVA_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbExportApparentCurrentAverageDemandkVA.Content);
            lblExportApparentCurrentAverageDemandkVA.Visibility = Visibility.Collapsed;
            uncheckParentCheckBox();
        }

        #endregion Average Demand

        #endregion CheckBox EventHandler


        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Grid).Children.Remove(this);

        }

        
    }
}

