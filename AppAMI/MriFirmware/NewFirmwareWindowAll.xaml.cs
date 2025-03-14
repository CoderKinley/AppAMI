using AppAMI.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace AppAMI.MriFirmware
{
    /// <summary>
    /// Interaction logic for NewFirmwareWindowAll.xaml
    /// </summary>
    public partial class NewFirmwareWindowAll : Window
    {
        string DistrictCode;
        string EsdCode;
        string UserId;
        string UserRole;


        string BrokerAddress = "103.234.126.43";
        string MqttPort = "1883";


        MqttClient clientUpdateFirmware;
        MqttClient clientFirmwareStatus;
        //MqttClient clientPing;
        //MqttClient clientPingReport;

        string clientId;

        string recieved_data1;


        string mqttTopicFirmwareUpdate;
        string mqttTopicFirmwareUpdateStatus;

        double  totalCount = 0;
        private double esdCompleted = 0;
        private double percentCompleted = 0;

        public NewFirmwareWindowAll(string selectedDistrict, string selectedEsd, string CurrentUserId1, string CurrentUserRole1)
        {
            InitializeComponent();

            DistrictCode = selectedDistrict;
            EsdCode = selectedEsd;
            UserId = CurrentUserId1;
            UserRole = CurrentUserRole1;

            GetCurrentFirmwareVersion();
            GetNewFirmwareVersion();
           
        }


        private async void GetCurrentFirmwareVersion()
        {
            string url = string.Format("http://103.234.126.43:3080/dtmeter/district/{0}/{1}", DistrictCode, EsdCode);

            try
            {
                using (WebClient web = new WebClient())
                {
                    string json = await web.DownloadStringTaskAsync(url);

                    List<Mri> mris = JsonConvert.DeserializeObject<List<Mri>>(json);

                    if (mris != null && mris.Count > 0)
                    {
                        var mostCommonFirmwareVersion = mris.GroupBy(mri => mri.mri_firmware_version)
                                                             .OrderByDescending(group => group.Count())
                                                             .Select(group => group.Key)
                                                             .FirstOrDefault();

                        lblDtCurrentFirmwareVersion.Content = mostCommonFirmwareVersion;
                    }
                    else
                    {
                        lblDtCurrentFirmwareVersion.Content = "No MRIs found";
                    }

                    totalCount = mris.Count;
                    lblDtCount.Content = totalCount.ToString();
                    lblAllDtinEsd.Content = totalCount.ToString();
                }
            }
            catch 
            {
                // Handle exceptions (e.g., display an error message)
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void GetNewFirmwareVersion()
        {
            string url = string.Format("http://103.234.126.43:3080/dtmeter/district/{0}/{1}", DistrictCode, EsdCode);

            try
            {
                using (WebClient web = new WebClient())
                {
                    string json = await web.DownloadStringTaskAsync(url);

                    List<Mri> mris = JsonConvert.DeserializeObject<List<Mri>>(json);

                    if (mris != null && mris.Count > 0)
                    {
                        var mostCommonFirmwareVersion = mris.GroupBy(mri => mri.mri_firmware_version)
                                                             .OrderByDescending(group => group.Count())
                                                             .Select(group => group.Key)
                                                             .FirstOrDefault();

                        lblDtNewFirmwareVersion.Content = mostCommonFirmwareVersion;
                    }
                    else
                    {
                        lblDtNewFirmwareVersion.Content = "No MRIs found";
                    }

                    
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., display an error message)
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async  void btnSave_Click(object sender, RoutedEventArgs e)
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
                AdminConfig matchingUser = adminConfigs.FirstOrDefault(user =>user.user_id == userId && user.password == password);

                if (matchingUser != null)
                {
                    if (mqttTopicFirmwareUpdate != null)
                    {
                        MessageBoxResult result = MessageBox.Show("Ready to Upload New Version. Do you want to proceed?", "Confirmation", MessageBoxButton.OKCancel);

                        if (result == MessageBoxResult.OK)
                        {
                            GetFirmwareUpdatePingNmsTopic();
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

        private async void GetFirmwareUpdatePingNmsTopic()
        {
            try
            {
                string url = string.Format("http://103.234.126.43:3080/dtmeter/district/{0}/{1}", DistrictCode, EsdCode);

                using (WebClient web = new WebClient())
                {
                    string json = await web.DownloadStringTaskAsync(url);

                    List<MqttClass> mris = JsonConvert.DeserializeObject<List<MqttClass>>(json);

                    List<MqttClass> dtIdFirmwareUpdateList = mris.Select(x => new MqttClass
                    {
                        dt_id = x.dt_id,
                        firmware_update = x.firmware_update,
                        firmware_status = x.firmware_status
                    }).ToList();

                    foreach (var item in dtIdFirmwareUpdateList)
                    {
                        // Dispatch to UI thread
                        Dispatcher.Invoke(() =>
                        {
                            lblDtId.Content = item.dt_id;
                        });

                        mqttTopicFirmwareUpdate = item.firmware_update;
                        mqttTopicFirmwareUpdateStatus = item.firmware_status;
                        await UploadFirmware();
                    }
                }
            }
            catch 
            {
                // Handle exceptions
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool isPublishingFirmwareUpdate = false;

        private async Task UploadFirmware()
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

            // Assuming lblUpdateFirmware.Content is a string
            byte[] firmwareData = Encoding.UTF8.GetBytes(lblUpdateFirmware.Content.ToString());

            await Task.Run(() => clientUpdateFirmware.Publish(mqttTopicFirmwareUpdate, firmwareData));

            isPublishingFirmwareUpdate = false;

            // Display status on UI
            Dispatcher.Invoke(() => displayUpoadStatus());
        }

        private void displayUpoadStatus()
        {
            clientFirmwareStatus = new MqttClient(BrokerAddress);
            clientFirmwareStatus.MqttMsgPublishReceived += client_MqttMsgPublishReceived1;
            clientId = Guid.NewGuid().ToString();
            clientFirmwareStatus.Connect(clientId);
            clientFirmwareStatus.Subscribe(new string[] { mqttTopicFirmwareUpdateStatus }, new byte[] { 2 });
            recieved_data1 = "";

            // Add logic to display status on the UI
        }


        void client_MqttMsgPublishReceived1(object sender, MqttMsgPublishEventArgs e)
        {

            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
            Dispatcher.Invoke(delegate {
                recieved_data1 = ReceivedMessage;

                lblCurrentFirmUpdateProgress.Content = recieved_data1;




                esdCompleted++;

                esdCompleted = 100;
                lblEsdCompleted.Content = esdCompleted.ToString();

                percentCompleted = esdCompleted / totalCount*100;
                lblUpdateProgerssESD.Content = percentCompleted.ToString();


            });
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {          
            Close();
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {          
            Close();
        }

    }
}
