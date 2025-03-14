using System.Collections.Generic;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using System.Net.Http;
using AppAMI.Classes;
using System;
using System.Threading.Tasks;

namespace AppAMI.MeterConfig
{
    /// <summary>
    /// Interaction logic for DeleteMeterWindow.xaml
    /// </summary>
    public partial class DeleteMeterWindow : Window
    {
        DtMeter selectedDtMeter;

        string CurrentDistrictCode;
        string CurrentEsdCode;

        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;

        public DeleteMeterWindow(DtMeter selectedDtMeter, string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1)
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


                        DtMeterSerialNo = "",

                        MeterType = "",
                        MeterFirmwareVersion = "",


                        MeterManufacturingYear = "",
                        MeterInstallationDate = ""
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
                        MessageBox.Show("Error deleting Meter configuration. Please check the details and try again.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

                    EventLogs = "Deleted Meter",

                    Date = UserEventDate,
                    Time = UserEventTime,

                    StatusAdmin1 = "Not Acknowledged",
                    StatusAdmin2 = "Not Acknowledged",
                    StatusAdmin3 = "Not Acknowledged",

                    Remarks = "Deleted Meter for : " + txtDTId.Text,
                };

                string url = "http://103.234.126.43:3080/dtmeter/logs/events/userevents";
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(userEventPost);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Meter successfully deleted.", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);

                    Close();
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Error deleting Meter configuration. Please check the details and try again.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
