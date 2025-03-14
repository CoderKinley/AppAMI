using System.Collections.Generic;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using System.Net.Http;
using AppAMI.Classes;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Xml.Linq;

namespace AppAMI.MeterConfig
{
    /// <summary>
    /// Interaction logic for UpdateMeterWindow.xaml
    /// </summary>
    public partial class UpdateMeterWindow : Window
    {
        DtMeter  selectedDtMeter;

        string CurrentDistrictCode;
        string CurrentEsdCode;


        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;

        public UpdateMeterWindow(DtMeter  selectedDtMeter, string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1)
        {
            InitializeComponent();


            CurrentUserId2 = CurrentUserId1;
            CurrentUserRole2 = CurrentUserRole1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;

            this.selectedDtMeter = selectedDtMeter;

            CurrentDistrictCode = this.selectedDtMeter.district_code;
            CurrentEsdCode = this.selectedDtMeter.esd_code;

            txtDTId.Text = this.selectedDtMeter.dt_id;
            txtMeterSerialNo.Text = this.selectedDtMeter.dt_meter_serial_no;
            cbMeterType.Text = this.selectedDtMeter.meter_type;
            cbMeterFirmwareVer.Text = this.selectedDtMeter.meter_firmware_version;
            dtMeterManuYear.Text = this.selectedDtMeter.meter_manufacturing_year;
            dtMeterInstallationDate.Text = this.selectedDtMeter.meter_installation_date;


            GetMeterType();
            GetMeterFirmV();
        }


        private void GetMeterType()
        {
            try
            {
                var meterTypeList = ReadMeterTypesFromXml("MeterType.xml")
                    .ToList();

                UpdateComboBox(meterTypeList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void UpdateComboBox(List<string> meterTypeList)
        {
            cbMeterType.ItemsSource = meterTypeList;
        }

        private List<string> ReadMeterTypesFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var MeterTypes = doc.Descendants("meter_type")
                                    .Select(element => element.Value)
                                    .ToList();

                return MeterTypes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }



        private void GetMeterFirmV()
        {
            try
            {
                var meterFirmVList = ReadMeterFirmVFromXml("MeterFirmV.xml")
                    .ToList();

                UpdateComboBoxFirmV(meterFirmVList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void UpdateComboBoxFirmV(List<string> meterFirmVList)
        {
            cbMeterFirmwareVer.ItemsSource = meterFirmVList;
        }

        private List<string> ReadMeterFirmVFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var MeterFirmVs = doc.Descendants("meter_firm_v")
                                    .Select(element => element.Value)
                                    .ToList();

                return MeterFirmVs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }









        async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDTId.Text))
            {
                MessageBox.Show("Please fill in the fields marked with a star (*).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                progressLogin.Visibility = Visibility.Collapsed;
            }

            else
            {
                progressLogin.Visibility = Visibility.Visible;

                try
                {
                    DtMeter dtMeter = new DtMeter()
                    {

                        DtMeterSerialNo = txtMeterSerialNo.Text,
                        MeterType = cbMeterType.Text,
                        MeterFirmwareVersion = cbMeterFirmwareVer.Text,
                        MeterManufacturingYear = dtMeterManuYear.Text,
                        MeterInstallationDate = dtMeterInstallationDate.Text
                    };

                    string url = string.Format("http://103.234.126.43:3080/dtmeter/district/meterinfo/{0}/{1}/{2}", CurrentDistrictCode, CurrentEsdCode, txtDTId.Text);

                    HttpClient client = new HttpClient();
                    string jsonData = JsonConvert.SerializeObject(dtMeter);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {


                        addNewUserEvent();
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show("Error updating  Meter configuration. Please check the details and try again.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
           
        }

        private async void addNewUserEvent()
        {
            DateTime now = DateTime.Now;
            //string UserEventDate = now.ToString("dddd, MMMM d, yyyy");
            string UserEventDate = now.ToString("dd-MM-yyyy");

            string UserEventTime = now.ToString("h:mm tt");

            try
            {
                UserEventPost userEventPost = new UserEventPost()
                {
                    UserID = CurrentUserId2,
                    EmployeeId = CurrentUserEmployeeId2,
                    UserName = CurrentUserName2,

                    EventLogs = "Edited Meter",

                    Date = UserEventDate,
                    Time = UserEventTime,

                    StatusAdmin1 = "Not Acknowledged",
                    StatusAdmin2 = "Not Acknowledged",
                    StatusAdmin3 = "Not Acknowledged",

                    Remarks = "Edited Meter for : " + txtDTId.Text,
                };

                string url = "http://103.234.126.43:3080/dtmeter/logs/events/userevents";
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(userEventPost);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("MRI successfully updated.", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);

                    Close();

                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Error updating Meter configuration. Please check the details and try again.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


            catch 
            {
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

       

      
    }
}
