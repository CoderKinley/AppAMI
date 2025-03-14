using AppAMI.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Net;

namespace AppAMI.MriFirmware
{
    /// <summary>
    /// Interaction logic for NewFirmwareWindow.xaml
    /// </summary>
    public partial class NewFirmwareWindow : Window
    {
        #region public variable mqttClient

        string BrokerAddress = "103.234.126.43";
        string MqttPort = "1883";

        MqttClient clientUpdateFirmware;
        MqttClient clientFirmwareStatus;
        MqttClient clientPing;
        MqttClient clientPingReport;

        string clientId;

        string recieved_data1;
        string recieved_data2;

        private System.Timers.Timer timer;

        string mqttTopicFirmwareUpdate;
        string mqttTopicFirmwareUpdateStatus;
        string mqttTopicPing;
        string mqttTopicPingReport;

        #endregion public variable mqttClient


        private DispatcherTimer timer1;
        private int counter;

        string DTId3;
        public NewFirmwareWindow(string DTId2)
        {
            InitializeComponent();

            DTId3 = DTId2;
            iconHeartBeatSuccess.Visibility = Visibility.Collapsed;

            CheckDtId();
        }
       
        private async void CheckDtId()
        {
            //await Task.Delay(TimeSpan.FromMilliseconds(500));

            if (string.IsNullOrEmpty(DTId3))
            {
                MessageBox.Show("Enter DT Id", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                lblDtId.Content = DTId3;

                await GetFirmwareUpdatePingNmsTopic();
                await GetCurrentFirmwareVersion();
                await GetNewFirmwareVersion();
                
            }
        }

        private async Task GetCurrentFirmwareVersion()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                progressLogin.Visibility = Visibility.Visible;
            });

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            try
            {
                using (WebClient web = new WebClient())
                {
                    string url = string.Format("http://103.234.126.43:3080/dtmeter/info/esd_essd_info/all");
                    string json = web.DownloadString(url);

                    List<Mri> all_data = JsonConvert.DeserializeObject<List<Mri>>(json);

                    List<Mri> dtMeters = all_data.Where(a => a.dt_id == DTId3).ToList();

                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (dtMeters.Count > 0)
                        {
                            txCurrentFirmwareVer.Text = dtMeters[0].mri_firmware_version;
                        }
                        else
                        {
                            MessageBox.Show("Enter DT Id", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    });
                }
            }
            catch 
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    progressLogin.Visibility = Visibility.Collapsed;
                });
            }
        }

        private async Task GetNewFirmwareVersion()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                progressLogin.Visibility = Visibility.Visible;
            });

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            try
            {
                using (WebClient web = new WebClient())
                {
                    string url = string.Format("http://103.234.126.43:3080/dtmeter/info/esd_essd_info/all");
                    string json = web.DownloadString(url);

                    List<Mri> all_data = JsonConvert.DeserializeObject<List<Mri>>(json);

                    List<Mri> dtMeters = all_data.Where(a => a.dt_id == DTId3).ToList();

                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (dtMeters.Count > 0)
                        {
                            txNewFirmwareVer.Text = dtMeters[0].mri_firmware_version;
                        }
                        else
                        {
                            MessageBox.Show("Enter DT Id", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    });
                }
            }
            catch 
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    progressLogin.Visibility = Visibility.Collapsed;
                });
            }
        }

        private async Task GetFirmwareUpdatePingNmsTopic()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                progressLogin.Visibility = Visibility.Visible;
            });

            try
            {
                HttpClient httpClient = new HttpClient();
                string url = "http://103.234.126.43:3080/dtmeter/info/esd_essd_info/all";
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    List<MqttClass> mqttClasses = JsonConvert.DeserializeObject<List<MqttClass>>(jsonResponse);

                    string dtId = DTId3;
                    MqttClass filteredMqttClass = mqttClasses.FirstOrDefault(dt => dt.dt_id == dtId);

                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (filteredMqttClass != null)
                        {
                            mqttTopicFirmwareUpdate = filteredMqttClass.firmware_update;
                            mqttTopicFirmwareUpdateStatus = filteredMqttClass.firmware_status;

                            mqttTopicPing = filteredMqttClass.ping;
                            mqttTopicPingReport = filteredMqttClass.ping_report;
                        }
                        else
                        {
                            MessageBox.Show("Enter DT Id", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    });
                }
                else
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            }
            catch
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    progressLogin.Visibility = Visibility.Collapsed;
                });
            }
        }

        #region Upload New Firmware

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync("http://103.234.126.43:3080/dtmeter/user");
                string responseBody = await response.Content.ReadAsStringAsync();

                List<AdminConfig> adminConfigs = JsonConvert.DeserializeObject<List<AdminConfig>>(responseBody);

                string userId = txtUserId.Text;
                string password = passPassword.Password;

                // Check if any matching user exists
                AdminConfig matchingUser = adminConfigs.FirstOrDefault(user =>
                    user.user_id == userId && user.password == password);

                if (matchingUser != null)
                {
                    if(mqttTopicFirmwareUpdate != null)
                    {
                        MessageBoxResult result = MessageBox.Show("Ready to Upload New Version. Do you want to proceed?", "Confirmation", MessageBoxButton.OKCancel);

                        if (result == MessageBoxResult.OK)
                        {
                            UploadFirmware();
                        }

                        else if (result == MessageBoxResult.Cancel)
                        {

                        }
                    }

                    else
                    {
                        MessageBox.Show("Cannot Upload New Firmware. Please check your internet connection or contact support for assistance.", "Upload Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Incorrect User ID or Password. Please check your credentials and try again.", "Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private bool isPublishingFirmwareUpdate = false;

        private  void UploadFirmware()
        {

            if (isPublishingFirmwareUpdate)
            {
                MessageBox.Show("Already publishing. Please wait for the current publishing process to complete.", "Publishing In Progress", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            isPublishingFirmwareUpdate = true;

            clientUpdateFirmware = new MqttClient(BrokerAddress, Convert.ToInt32(MqttPort), false, MqttSslProtocols.None, null, null);
            string clientId = Guid.NewGuid().ToString();

            try
            {
                clientUpdateFirmware.Connect(clientId);
            }
            catch 
            {
                MessageBox.Show("Error connecting to broker. Please check your network connection or broker settings and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                isPublishingFirmwareUpdate = false;
                return;
            }

            if (!clientUpdateFirmware.IsConnected)
            {
                MessageBox.Show("Client is not connected to the broker. Please ensure the connection is established before proceeding.", "Connection Status", MessageBoxButton.OK, MessageBoxImage.Warning);
                isPublishingFirmwareUpdate = false;
                return;
            }
            clientUpdateFirmware.Publish(mqttTopicFirmwareUpdate, Encoding.UTF8.GetBytes((string)lblUpdateFirmware.Content));

            isPublishingFirmwareUpdate = false;

            displayUpoadStatus();
        }

        private  void displayUpoadStatus()
        {

            clientFirmwareStatus = new MqttClient(BrokerAddress);
            clientFirmwareStatus.MqttMsgPublishReceived += client_MqttMsgPublishReceived1;
            clientId = Guid.NewGuid().ToString();
            clientFirmwareStatus.Connect(clientId);
            clientFirmwareStatus.Subscribe(new string[] { mqttTopicFirmwareUpdateStatus }, new byte[] { 2 });   
            recieved_data1 = "";
           
        }


        void client_MqttMsgPublishReceived1(object sender, MqttMsgPublishEventArgs e)
        {

            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
            Dispatcher.Invoke(delegate {              
                recieved_data1 = ReceivedMessage;

                lblFirmUpdateProgress.Content = recieved_data1;
            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (clientUpdateFirmware != null && clientUpdateFirmware.IsConnected)
            {
                clientUpdateFirmware.Disconnect();
            }

            if (clientFirmwareStatus != null && clientFirmwareStatus.IsConnected)
            {
                clientFirmwareStatus.Disconnect();
            }

            if (clientPing != null && clientPing.IsConnected)
            {
                clientPing.Disconnect();
            }

            if (clientPingReport != null && clientPingReport.IsConnected)
            {
                clientPingReport.Disconnect();
            }
            Close();
        }


        #endregion Upload New Firmware

        #region Heart Beat


        private bool isPublishingPing = false;

        private void btnCheckHeartbeat_Click(object sender, RoutedEventArgs e)
        {
            if (isPublishingPing)
            {
                MessageBox.Show("Already publishing.");
                return;
            }

            isPublishingPing = true;

            clientPing = new MqttClient(BrokerAddress, Convert.ToInt32(MqttPort), false, MqttSslProtocols.None, null, null);
            string clientId = Guid.NewGuid().ToString();

            try
            {
                clientPing.Connect(clientId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to broker: {ex.Message}");
                isPublishingPing = false; 
                return;
            }

            if (!clientPing.IsConnected)
            {
                MessageBox.Show("Client is not connected to the broker.");
                isPublishingPing = false; 
                return;
            }
            clientPing.Publish(mqttTopicPing, Encoding.UTF8.GetBytes((string)lblHeartbeatOn.Content));

            isPublishingPing = false;


            mqttGetPing();
        }

        private  void mqttGetPing()
        {

            clientPingReport = new MqttClient(BrokerAddress);
            clientPingReport.MqttMsgPublishReceived += client_MqttMsgPublishReceived2;
            clientId = Guid.NewGuid().ToString();
            clientPingReport.Connect(clientId);
            clientPingReport.Subscribe(new string[] { mqttTopicPingReport }, new byte[] { 2 });
            recieved_data2 = "";

        }

        void client_MqttMsgPublishReceived2(object sender, MqttMsgPublishEventArgs e)
        {

            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
            Dispatcher.Invoke(delegate {              // we need this construction because the receiving code in the library and the UI with textbox run on different threads
                recieved_data2 = ReceivedMessage;


                try
                {


                    string[] words = recieved_data2.Split(',');

                    if (recieved_data2.Equals("NC"))
                    {

                    }


                    else if (recieved_data2.Equals("I am alive"))
                    {

                        timer1 = new DispatcherTimer();
                        timer1.Interval = TimeSpan.FromSeconds(1);
                        timer1.Tick += Timer_Tick;
                        timer1.Start();

                        lblHeartbeat.Content = "1";


                       
                    }

                }
                catch
                {

                }


            });



        }


        private void Timer_Tick(object sender, EventArgs e)
        {

            if (lblHeartbeat.Content.Equals("1"))
            {
                iconHeartBeatSuccess.Visibility = Visibility.Visible;
                counter++;

              

                if (counter % 2 == 0)
                {
                    iconHeartBeatSuccess.Height = 60;
                    iconHeartBeatSuccess.Width = 60;
                }


                if (counter % 2 != 0)
                {
                    iconHeartBeatSuccess.Height = 40;
                    iconHeartBeatSuccess.Width = 40;
                }


                if (counter == 100)
                {
                    timer1.Stop();
                }
            }

            else 
            {
                iconHeartBeatSuccess.Height = 40;
                iconHeartBeatSuccess.Width = 40;
                MessageBox.Show("Heart Beat Failed!!! Try Again");
            }
        }

        #endregion Heart Beat

        private  void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (clientUpdateFirmware != null && clientUpdateFirmware.IsConnected)
            {
                clientUpdateFirmware.Disconnect();
            }

            if (clientFirmwareStatus != null && clientFirmwareStatus.IsConnected)
            {
                clientFirmwareStatus.Disconnect();
            }

            if (clientPing != null && clientPing.IsConnected)
            {
                clientPing.Disconnect();
            }

            if (clientPingReport != null && clientPingReport.IsConnected)
            {
                clientPingReport.Disconnect();
            }

          

            Close();
        }

    }
}
