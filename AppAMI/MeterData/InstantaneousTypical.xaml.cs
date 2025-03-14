using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Windows.Media;
using System.Net;
using AppAMI.Classes;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;

namespace AppAMI.MeterData
{
    /// <summary>
    /// Interaction logic for InstantaneousTypical.xaml
    /// </summary>
    public partial class InstantaneousTypical : UserControl
    {



        #region public variable mqttClient

        private MqttClient mqttClient1;
        private MqttClient mqttClient2;
        private MqttClient mqttClient3;


        private string mqttBrokerAddress1 = "103.234.126.44";
        private string mqttBrokerAddress2 = "103.234.126.41";
        private string mqttBrokerAddress3 = "103.234.126.42";


        private int successNumber = 0;
        private int unsuccessNumber = 0;
        private System.Timers.Timer timer;

        string mqttTopicInstant;

        #endregion public variable mqttClient


        #region ObservableCollection  chart
        public ObservableCollection<KeyValuePair<string, string>> ImportApparentPowerValueA
      = new ObservableCollection<KeyValuePair<string, string>>();
        public ObservableCollection<KeyValuePair<string, string>> ImportApparentPowerValueB
      = new ObservableCollection<KeyValuePair<string, string>>();
        public ObservableCollection<KeyValuePair<string, string>> ImportApparentPowerValueC
      = new ObservableCollection<KeyValuePair<string, string>>();


        public ObservableCollection<KeyValuePair<string, string>> VoltageValueA
              = new ObservableCollection<KeyValuePair<string, string>>();
        public ObservableCollection<KeyValuePair<string, string>> VoltageValueB
      = new ObservableCollection<KeyValuePair<string, string>>();
        public ObservableCollection<KeyValuePair<string, string>> VoltageValueC
      = new ObservableCollection<KeyValuePair<string, string>>();


        public ObservableCollection<KeyValuePair<string, string>> CurrentValueA
        = new ObservableCollection<KeyValuePair<string, string>>();
        public ObservableCollection<KeyValuePair<string, string>> CurrentValueB
      = new ObservableCollection<KeyValuePair<string, string>>();
        public ObservableCollection<KeyValuePair<string, string>> CurrentValueC
      = new ObservableCollection<KeyValuePair<string, string>>();

        #endregion ObservableCollection  chart

        #region max min avg data
        

        List<double> receivedCurrentAData = new List<double>();
        List<double> receivedCurrentBData = new List<double>();
        List<double> receivedCurrentCData = new List<double>();

        List<double> receivedVoltageAData = new List<double>();
        List<double> receivedVoltageBData = new List<double>();
        List<double> receivedVoltageCData = new List<double>();

        List<double> receivedImportApparentPowerAData = new List<double>();
        List<double> receivedImportApparentPowerBData = new List<double>();
        List<double> receivedImportApparentPowerCData = new List<double>();
        #endregion max min avg data




        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;

        string DTId2;

        public InstantaneousTypical(string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1, string DTId1)
        {
            InitializeComponent();

            CurrentUserId2 = CurrentUserId1;
            CurrentUserRole2 = CurrentUserRole1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;

            btnStartPolling.IsEnabled = false;
            btnStopPolling.IsEnabled = false;


            ChartPowerR.DataContext = ImportApparentPowerValueA;
            ChartPowerY.DataContext = ImportApparentPowerValueB;
            ChartPowerB.DataContext = ImportApparentPowerValueC;


            ChartVoltage1.DataContext = VoltageValueA;
            ChartVoltage2.DataContext = VoltageValueB;
            ChartVoltage3.DataContext = VoltageValueC;

            ChartCurrent1.DataContext = CurrentValueA;
            ChartCurrent2.DataContext = CurrentValueB;
            ChartCurrent3.DataContext = CurrentValueC;

            txtDtId.Loaded += TxtDtId_Loaded;
            txtUserRole.Loaded += TxtUserRole_Loaded;

        }

        private async void TxtDtId_Loaded(object sender, RoutedEventArgs e)
        {
            await GetTopic();
        }

        private void TxtUserRole_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentUserRole2 = txtUserRole.Text;
        }

        private async void txtDtId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mqttClient1 != null && mqttClient1.IsConnected)
            {
                mqttClient1.Disconnect();
                // lblConnStatus.Content = "Disconnected from MQTT broker"; 
                lblSuccessfulPolls.Content = "0";
                lblFailedPolls.Content = "0";
            }

            if (timer != null)
            {
                timer.Stop();
            }

            

            lblSuccessfulPolls.Content = successNumber;
            lblFailedPolls.Content = unsuccessNumber;
            lblTotalPolls.Content = successNumber + unsuccessNumber;

            lblPollingStatus.Content = "Reading Stopped";

            btnPausePolling.Visibility = Visibility.Hidden;
            btnStartPolling.Visibility = Visibility.Visible;

            sfProgress.AnimationDuration = new TimeSpan(0, 0, 0);


            //lblCurrentA.Content ="0";
            //lblCurrentB.Content = "0";
            //lblCurrentC.Content = "0";

            //lblVolatgeA.Content = "0";
            //lblVolatgeB.Content = "0";
            //lblVolatgeC.Content = "0";

            //lblImportAppPowerA.Content = "0";
            //lblImportAppPowerB.Content = "0";
            //lblImportAppPowerC.Content = "0";

            //lblTotActivePower.Content = "0";
            //lblTotReactivePower.Content = "0";
            //lblTotApparentPower.Content = "0";

            //lblInstantaneousPowerFactor.Content = "0";

            //AddPieData();
            //addPowerChart();
            //addVoltageChart();
            //addCurrentChart();

            successNumber = 0;
            unsuccessNumber = 0;

            //successNumber++;

            Dispatcher.Invoke(() => lblSuccessfulPolls.Content = successNumber);

            Dispatcher.Invoke(() => lblTotalPolls.Content = successNumber + unsuccessNumber);


           ImportApparentPowerValueA.Clear();
            ImportApparentPowerValueB.Clear();
             ImportApparentPowerValueC.Clear();


             VoltageValueA.Clear();
            VoltageValueB.Clear();
             VoltageValueC.Clear();

            CurrentValueA.Clear();
            CurrentValueB.Clear();
            CurrentValueC.Clear();





            //if (double.TryParse((string)lblCurrentA.Content, out double currentA))
            //{
            //    receivedCurrentAData.Add(currentA);

            //    double minimumCurrentA = receivedCurrentAData.Min();
            //    double maximumCurrentA = receivedCurrentAData.Max();
            //    double averageCurrentA = receivedCurrentAData.Average();

            //    lblCurrentAmin.Content = minimumCurrentA.ToString("N2");
            //    lblCurrentAmax.Content = maximumCurrentA.ToString("N2");
            //    lblCurrentAavg.Content = averageCurrentA.ToString("N2");
            //}

            //if (double.TryParse((string)lblCurrentB.Content, out double currentB))
            //{
            //    receivedCurrentBData.Add(currentB);

            //    double minimumCurrentB = receivedCurrentBData.Min();
            //    double maximumCurrentB = receivedCurrentBData.Max();
            //    double averageCurrentB = receivedCurrentBData.Average();

            //    lblCurrentBmin.Content = minimumCurrentB.ToString("N2");
            //    lblCurrentBmax.Content = maximumCurrentB.ToString("N2");
            //    lblCurrentBavg.Content = averageCurrentB.ToString("N2");
            //}

            //if (double.TryParse((string)lblCurrentC.Content, out double currentC))
            //{
            //    receivedCurrentCData.Add(currentC);

            //    double minimumCurrentC = receivedCurrentCData.Min();
            //    double maximumCurrentC = receivedCurrentCData.Max();
            //    double averageCurrentC = receivedCurrentCData.Average();

            //    lblCurrentCmin.Content = minimumCurrentC.ToString("N2");
            //    lblCurrentCmax.Content = maximumCurrentC.ToString("N2");
            //    lblCurrentCavg.Content = averageCurrentC.ToString("N2");
            //}


            //if (double.TryParse((string)lblVolatgeA.Content, out double voltageA))
            //{
            //    receivedVoltageAData.Add(voltageA);

            //    double minimumVoltageA = receivedVoltageAData.Min();
            //    double maximumVoltageA = receivedVoltageAData.Max();
            //    double averageVoltageA = receivedVoltageAData.Average();

            //    lblVoltageAmin.Content = minimumVoltageA.ToString("N2");
            //    lblVoltageAmax.Content = maximumVoltageA.ToString("N2");
            //    lblVoltageAavg.Content = averageVoltageA.ToString("N2");
            //}

            //if (double.TryParse((string)lblVolatgeB.Content, out double voltageB))
            //{
            //    receivedVoltageBData.Add(voltageB);

            //    double minimumVoltageB = receivedVoltageBData.Min();
            //    double maximumVoltageB = receivedVoltageBData.Max();
            //    double averageVoltageB = receivedVoltageBData.Average();

            //    lblVoltageBmin.Content = minimumVoltageB.ToString("N2");
            //    lblVoltageBmax.Content = maximumVoltageB.ToString("N2");
            //    lblVoltageBavg.Content = averageVoltageB.ToString("N2");
            //}

            //if (double.TryParse((string)lblVolatgeC.Content, out double voltageC))
            //{
            //    receivedVoltageCData.Add(voltageC);

            //    double minimumVoltageC = receivedVoltageCData.Min();
            //    double maximumVoltageC = receivedVoltageCData.Max();
            //    double averageVoltageC = receivedVoltageCData.Average();

            //    lblVoltageCmin.Content = minimumVoltageC.ToString("N2");
            //    lblVoltageCmax.Content = maximumVoltageC.ToString("N2");
            //    lblVoltageCavg.Content = averageVoltageC.ToString("N2");
            //}



            //if (double.TryParse((string)lblImportAppPowerA.Content , out double powerA))
            //{
            //    receivedImportApparentPowerAData.Add(powerA);

            //    double minimumPowerA = receivedImportApparentPowerAData.Min();
            //    double maximumPowerA = receivedImportApparentPowerAData.Max();
            //    double averagePowerA = receivedImportApparentPowerAData.Average();

            //    lblImportAppPowerAmin.Content = minimumPowerA.ToString("N2");
            //    lblImportAppPowerAmax.Content = maximumPowerA.ToString("N2");
            //    lblImportAppPowerAavg.Content = averagePowerA.ToString("N2");
            //}

            //if (double.TryParse((string)lblImportAppPowerB.Content , out double powerB))
            //{
            //    receivedImportApparentPowerBData.Add(powerB);

            //    double minimumPowerB = receivedImportApparentPowerBData.Min();
            //    double maximumPowerB = receivedImportApparentPowerBData.Max();
            //    double averagePowerB = receivedImportApparentPowerBData.Average();

            //    lblImportAppPowerBmin.Content = minimumPowerB.ToString("N2");
            //    lblImportAppPowerBmax.Content = maximumPowerB.ToString("N2");
            //    lblImportAppPowerBavg.Content = averagePowerB.ToString("N2");
            //}

            //if (double.TryParse((string)lblImportAppPowerC.Content, out double powerC))
            //{
            //    receivedImportApparentPowerCData.Add(powerC);

            //    double minimumPowerC = receivedImportApparentPowerCData.Min();
            //    double maximumPowerC = receivedImportApparentPowerCData.Max();
            //    double averagePowerC = receivedImportApparentPowerCData.Average();

            //    lblImportAppPowerCmin.Content = minimumPowerC.ToString("N2");
            //    lblImportAppPowerCmax.Content = maximumPowerC.ToString("N2");
            //    lblImportAppPowerCavg.Content = averagePowerC.ToString("N2");
            //}


            await GetTopic();

           
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



        #region mqtt methods
        //private void btnStartPolling_Click(object sender, RoutedEventArgs e)
        //{
        //    ConnectToBroker1();
        //    ConnectToBroker2();
        //    ConnectToBroker3();


        //    btnStartPolling.Visibility = Visibility.Hidden;
        //    btnPausePolling.Visibility = Visibility.Visible;

        //    lblPollingStatus.Content = "Reading Data";

        //    sfProgress.Progress = 50;
        //    sfProgress.Progress = 50;
        //    sfProgress.IsIndeterminate = true;
        //    sfProgress.Width = 250;
        //    sfProgress.Height = 4;
        //    sfProgress.AnimationDuration = new TimeSpan(0, 0, 1);


        //    timer = new System.Timers.Timer();
        //    timer.Interval = 240000; // 5 minutes
        //    timer.Elapsed += Timer_Elapsed;
        //    timer.AutoReset = false; // only fire once
        //}


        private void btnStartPolling_Click(object sender, RoutedEventArgs e)
        {
            // Check if each broker is already connected before attempting to connect
            if (mqttClient1 == null || !mqttClient1.IsConnected)
            {
                ConnectToBroker1();
            }

            if (mqttClient2 == null || !mqttClient2.IsConnected)
            {
                ConnectToBroker2();
            }

            if (mqttClient3 == null || !mqttClient3.IsConnected)
            {
                ConnectToBroker3();
            }

            btnStartPolling.Visibility = Visibility.Hidden;
            btnPausePolling.Visibility = Visibility.Visible;

            lblPollingStatus.Content = "Reading Data";

            sfProgress.Progress = 50;
            sfProgress.IsIndeterminate = true;
            sfProgress.Width = 250;
            sfProgress.Height = 4;
            sfProgress.AnimationDuration = new TimeSpan(0, 0, 1);

            // Initialize and start the timer for polling
            timer = new System.Timers.Timer();
            timer.Interval = 240000; // 4 minutes
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false; // Only fire once
            timer.Start(); // Start the timer
        }


        private void ConnectToBroker1()
        {
            

            try
            {
                mqttClient1 = new MqttClient(mqttBrokerAddress1);
                mqttClient1.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                mqttClient1.Connect(Guid.NewGuid().ToString());

                lblConnStatus.Content = "Connected to MQTT Server";
                string hexColor = "#00cdac";
                Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);
                lblConnStatus.Foreground = brush;

                mqttClient1.Subscribe(new string[] { mqttTopicInstant }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

                // lblConnStatus.Content = "Subscribed to Topic " + mqttTopicInstant;

            }

            catch
            {
                MessageBox.Show("MQTT connection failed. Please connect before playing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void ConnectToBroker2()
        {


            try
            {
                mqttClient2 = new MqttClient(mqttBrokerAddress2);
                mqttClient2.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                mqttClient2.Connect(Guid.NewGuid().ToString());

                lblConnStatus.Content = "Connected to MQTT Server";
                string hexColor = "#00cdac";
                Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);
                lblConnStatus.Foreground = brush;

                mqttClient2.Subscribe(new string[] { mqttTopicInstant }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

                // lblConnStatus.Content = "Subscribed to Topic " + mqttTopicInstant;


            }

            catch
            {
                MessageBox.Show("MQTT connection failed. Please connect before playing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void ConnectToBroker3()
        {


            try
            {
                mqttClient3 = new MqttClient(mqttBrokerAddress3);
                mqttClient3.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                mqttClient3.Connect(Guid.NewGuid().ToString());

                lblConnStatus.Content = "Connected to MQTT Server";
                string hexColor = "#00cdac";
                Brush brush = (Brush)new BrushConverter().ConvertFrom(hexColor);
                lblConnStatus.Foreground = brush;

                mqttClient3.Subscribe(new string[] { mqttTopicInstant }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

                // lblConnStatus.Content = "Subscribed to Topic " + mqttTopicInstant;

              
            }

            catch
            {
                MessageBox.Show("MQTT connection failed. Please connect before playing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {

            string message = Encoding.UTF8.GetString(e.Message);
            //txtData.Text = message;
            Dispatcher.Invoke(() =>
            {
                message = message.Replace("N/a", "0.00");

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

                 

                    // Ensure that words array contains at least 46 elements
                    if (words.Length == 44)
                    {
                        lblCurrentA.Content = words[3];
                        lblCurrentB.Content = words[4];
                        lblCurrentC.Content = words[5];

                        lblVolatgeA.Content = words[6];
                        lblVolatgeB.Content = words[7];
                        lblVolatgeC.Content = words[8];

                        lblImportAppPowerA.Content = words[24];
                        lblImportAppPowerB.Content = words[30];
                        lblImportAppPowerC.Content = words[36];

                        lblTotActivePower.Content = words[16];
                        lblTotReactivePower.Content = words[17];
                        lblTotApparentPower.Content = words[18];

                        lblInstantaneousPowerFactor.Content = words[9];
                    }

                    else if (words.Length == 42)
                    {
                        lblCurrentA.Content = words[3];
                        lblCurrentB.Content = words[4];
                        lblCurrentC.Content = words[5];

                        lblVolatgeA.Content = words[6];
                        lblVolatgeB.Content = words[7];
                        lblVolatgeC.Content = words[8];

                        lblImportAppPowerA.Content = words[24];
                        lblImportAppPowerB.Content = words[30];
                        lblImportAppPowerC.Content = words[36];

                        lblTotActivePower.Content = words[16];
                        lblTotReactivePower.Content = words[17];
                        lblTotApparentPower.Content = words[18];

                        lblInstantaneousPowerFactor.Content = words[9];
                    }
                    else if (words.Length == 18)
                    {
                        lblCurrentA.Content = words[3];
                        lblCurrentB.Content = "0";
                        lblCurrentC.Content = "0";

                        lblVolatgeA.Content = words[4];
                        lblVolatgeB.Content = "0";
                        lblVolatgeC.Content = "0";

                        lblImportAppPowerA.Content = words[10];
                        lblImportAppPowerB.Content = "0";
                        lblImportAppPowerC.Content = "0";

                        lblTotActivePower.Content = words[6];
                        lblTotReactivePower.Content = words[8];
                        lblTotApparentPower.Content = words[10];

                        lblInstantaneousPowerFactor.Content = words[9];
                    }

                    else
                    {
                        lblCurrentA.Content = "0";
                        lblCurrentB.Content = "0";
                        lblCurrentC.Content = "0";

                        lblVolatgeA.Content = "0";
                        lblVolatgeB.Content = "0";
                        lblVolatgeC.Content = "0";

                        lblImportAppPowerA.Content = "0";
                        lblImportAppPowerB.Content = "0";
                        lblImportAppPowerC.Content = "0";

                        lblTotActivePower.Content = "0";
                        lblTotReactivePower.Content = "0";
                        lblTotApparentPower.Content = "0";

                        lblInstantaneousPowerFactor.Content = "0";

                    }

                    AddPieData();
                    addPowerChart();
                    addVoltageChart();
                    addCurrentChart();

                    successNumber++;

                    Dispatcher.Invoke(() => lblSuccessfulPolls.Content = successNumber);

                    Dispatcher.Invoke(() => lblTotalPolls.Content = successNumber + unsuccessNumber);

                    timer.Stop();
                    timer.Start();

                    if (double.TryParse((string)lblCurrentA.Content, out double currentA))
                    {
                        receivedCurrentAData.Add(currentA);

                        double minimumCurrentA = receivedCurrentAData.Min();
                        double maximumCurrentA = receivedCurrentAData.Max();
                        double averageCurrentA = receivedCurrentAData.Average();

                        lblCurrentAmin.Content = minimumCurrentA.ToString("N2");
                        lblCurrentAmax.Content = maximumCurrentA.ToString("N2");
                        lblCurrentAavg.Content = averageCurrentA.ToString("N2");
                    }

                    if (double.TryParse((string)lblCurrentB.Content, out double currentB))
                    {
                        receivedCurrentBData.Add(currentB);

                        double minimumCurrentB = receivedCurrentBData.Min();
                        double maximumCurrentB = receivedCurrentBData.Max();
                        double averageCurrentB = receivedCurrentBData.Average();

                        lblCurrentBmin.Content = minimumCurrentB.ToString("N2");
                        lblCurrentBmax.Content = maximumCurrentB.ToString("N2");
                        lblCurrentBavg.Content = averageCurrentB.ToString("N2");
                    }

                    if (double.TryParse((string)lblCurrentC.Content, out double currentC))
                    {
                        receivedCurrentCData.Add(currentC);

                        double minimumCurrentC = receivedCurrentCData.Min();
                        double maximumCurrentC = receivedCurrentCData.Max();
                        double averageCurrentC = receivedCurrentCData.Average();

                        lblCurrentCmin.Content = minimumCurrentC.ToString("N2");
                        lblCurrentCmax.Content = maximumCurrentC.ToString("N2");
                        lblCurrentCavg.Content = averageCurrentC.ToString("N2");
                    }


                    if (double.TryParse((string)lblVolatgeA.Content, out double voltageA))
                    {
                        receivedVoltageAData.Add(voltageA);

                        double minimumVoltageA = receivedVoltageAData.Min();
                        double maximumVoltageA = receivedVoltageAData.Max();
                        double averageVoltageA = receivedVoltageAData.Average();

                        lblVoltageAmin.Content = minimumVoltageA.ToString("N2");
                        lblVoltageAmax.Content = maximumVoltageA.ToString("N2");
                        lblVoltageAavg.Content = averageVoltageA.ToString("N2");
                    }

                    if (double.TryParse((string)lblVolatgeB.Content, out double voltageB))
                    {
                        receivedVoltageBData.Add(voltageB);

                        double minimumVoltageB = receivedVoltageBData.Min();
                        double maximumVoltageB = receivedVoltageBData.Max();
                        double averageVoltageB = receivedVoltageBData.Average();

                        lblVoltageBmin.Content = minimumVoltageB.ToString("N2");
                        lblVoltageBmax.Content = maximumVoltageB.ToString("N2");
                        lblVoltageBavg.Content = averageVoltageB.ToString("N2");
                    }

                    if (double.TryParse((string)lblVolatgeC.Content, out double voltageC))
                    {
                        receivedVoltageCData.Add(voltageC);

                        double minimumVoltageC = receivedVoltageCData.Min();
                        double maximumVoltageC = receivedVoltageCData.Max();
                        double averageVoltageC = receivedVoltageCData.Average();

                        lblVoltageCmin.Content = minimumVoltageC.ToString("N2");
                        lblVoltageCmax.Content = maximumVoltageC.ToString("N2");
                        lblVoltageCavg.Content = averageVoltageC.ToString("N2");
                    }



                    if (double.TryParse((string)lblImportAppPowerA.Content, out double powerA))
                    {
                        receivedImportApparentPowerAData.Add(powerA);

                        double minimumPowerA = receivedImportApparentPowerAData.Min();
                        double maximumPowerA = receivedImportApparentPowerAData.Max();
                        double averagePowerA = receivedImportApparentPowerAData.Average();

                        lblImportAppPowerAmin.Content = minimumPowerA.ToString("N2");
                        lblImportAppPowerAmax.Content = maximumPowerA.ToString("N2");
                        lblImportAppPowerAavg.Content = averagePowerA.ToString("N2");
                    }

                    if (double.TryParse((string)lblImportAppPowerB.Content, out double powerB))
                    {
                        receivedImportApparentPowerBData.Add(powerB);

                        double minimumPowerB = receivedImportApparentPowerBData.Min();
                        double maximumPowerB = receivedImportApparentPowerBData.Max();
                        double averagePowerB = receivedImportApparentPowerBData.Average();

                        lblImportAppPowerBmin.Content = minimumPowerB.ToString("N2");
                        lblImportAppPowerBmax.Content = maximumPowerB.ToString("N2");
                        lblImportAppPowerBavg.Content = averagePowerB.ToString("N2");
                    }

                    if (double.TryParse((string)lblImportAppPowerC.Content, out double powerC))
                    {
                        receivedImportApparentPowerCData.Add(powerC);

                        double minimumPowerC = receivedImportApparentPowerCData.Min();
                        double maximumPowerC = receivedImportApparentPowerCData.Max();
                        double averagePowerC = receivedImportApparentPowerCData.Average();

                        lblImportAppPowerCmin.Content = minimumPowerC.ToString("N2");
                        lblImportAppPowerCmax.Content = maximumPowerC.ToString("N2");
                        lblImportAppPowerCavg.Content = averagePowerC.ToString("N2");
                    }


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
            if (mqttClient1 != null && mqttClient1.IsConnected || mqttClient2 != null && mqttClient2.IsConnected || mqttClient3 != null && mqttClient3.IsConnected)
            {
                mqttClient1.Unsubscribe(new string[] { mqttTopicInstant }); // Unsubscribe from the topic
                mqttClient2.Unsubscribe(new string[] { mqttTopicInstant }); // Unsubscribe from the topic
                mqttClient3.Unsubscribe(new string[] { mqttTopicInstant }); // Unsubscribe from the topic

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

            if (mqttClient1 != null && mqttClient1.IsConnected || mqttClient2 != null && mqttClient2.IsConnected || mqttClient3 != null && mqttClient3.IsConnected)
            {
                mqttClient1.Disconnect();
                mqttClient2.Disconnect();
                mqttClient3.Disconnect();

                // lblConnStatus.Content = "Disconnected from MQTT broker"; 
                lblSuccessfulPolls.Content = "0";
                lblFailedPolls.Content = "0";
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

        #region Charts

        private void addPowerChart()
        {
            DateTime dateTime = DateTime.Now;
            string inputX = dateTime.ToString("hh:mm tt");

            string inputYA = (string)lblImportAppPowerA.Content;
            ImportApparentPowerValueA.Add(new KeyValuePair<string, string>(inputX, inputYA));

            string inputYB = (string)lblImportAppPowerB.Content;
            ImportApparentPowerValueB.Add(new KeyValuePair<string, string>(inputX, inputYB));

            string inputYC = (string)lblImportAppPowerC.Content;
            ImportApparentPowerValueC.Add(new KeyValuePair<string, string>(inputX, inputYC));

            if (ImportApparentPowerValueA.Count > 1000)
            {
                ImportApparentPowerValueA.RemoveAt(0);
                ImportApparentPowerValueB.RemoveAt(0);
                ImportApparentPowerValueC.RemoveAt(0);
            }
        }

        private void addVoltageChart()
        {
            DateTime dateTime = DateTime.Now;
            string inputX = dateTime.ToString("hh:mm tt");

            string inputYA = (string)lblVolatgeA.Content;
            VoltageValueA.Add(new KeyValuePair<string, string>(inputX, inputYA));

            string inputYB = (string)lblVolatgeB.Content;
            VoltageValueB.Add(new KeyValuePair<string, string>(inputX, inputYB));

            string inputYC = (string)lblVolatgeC.Content;
            VoltageValueC.Add(new KeyValuePair<string, string>(inputX, inputYC));

            if (VoltageValueA.Count > 1000)
            {
                VoltageValueA.RemoveAt(0);
                VoltageValueB.RemoveAt(0);
                VoltageValueC.RemoveAt(0);
            }

        }

        private void addCurrentChart()
        {
            DateTime dateTime = DateTime.Now;
            string inputX = dateTime.ToString("hh:mm tt");

            string inputYA = (string)lblCurrentA.Content;
            CurrentValueA.Add(new KeyValuePair<string, string>(inputX, inputYA));

            string inputYB = (string)lblCurrentB.Content;
            CurrentValueB.Add(new KeyValuePair<string, string>(inputX, inputYB));

            string inputYC = (string)lblCurrentC.Content;
            CurrentValueC.Add(new KeyValuePair<string, string>(inputX, inputYC));


            if (CurrentValueA.Count > 1000)
            {
                CurrentValueA.RemoveAt(0);
                CurrentValueB.RemoveAt(0);
                CurrentValueC.RemoveAt(0);
            }
        }

        #endregion Charts

        #region Pie chart

        private void AddPieData()
        {
            List<KeyValuePair<string, string>> valueList = new List<KeyValuePair<string, string>>();
            valueList.Add(new KeyValuePair<string, string>("Active", (string)lblTotActivePower.Content));
            valueList.Add(new KeyValuePair<string, string>("Reactive", (string)lblTotReactivePower.Content));
            valueList.Add(new KeyValuePair<string, string>("Apparent", (string)lblTotApparentPower.Content));

            TotalPowerPieChart.DataContext = valueList;
        }
        #endregion Pie chart




       
    }
}
