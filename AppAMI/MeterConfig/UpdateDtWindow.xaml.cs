using AppAMI.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace AppAMI.MeterConfig
{
    /// <summary>
    /// Interaction logic for UpdateDtWindow.xaml
    /// </summary>
    public partial class UpdateDtWindow : Window
    {
        DT selectedDt;

        string CurrentDistrictCode;
        string CurrentEsdCode;

        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;

        public UpdateDtWindow(DT selectedDt, string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1)
        {
            InitializeComponent();

            CurrentUserId2 = CurrentUserId1;
            CurrentUserRole2 = CurrentUserRole1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;

            this.selectedDt = selectedDt;

            CurrentDistrictCode = this.selectedDt.district_code;
            CurrentEsdCode = this.selectedDt.esd_code;

            txtDistrictName.Text = this.selectedDt.district_name;
            txtDistrictCode.Text = this.selectedDt.district_code;
            txtEsdName.Text = this.selectedDt.esd_name;
            txtEsdCode.Text = this.selectedDt.esd_code;
            txtFeederName.Text = this.selectedDt.feeder_name;
            txtFeederId.Text = this.selectedDt.feeder_id ;
            txtPortionId.Text = this.selectedDt.portion_id ;
            txtRootId.Text = this.selectedDt.root_id;


            txtDtName.Text = this.selectedDt.dt_name;
            txtDtId.Text = this.selectedDt.dt_id;
            txtDtTransSerial.Text = this.selectedDt.transformer_serial_number;
            txtPrimaryRatedVoltage.Text = this.selectedDt.rated_voltage_primary;
            txtSecondaryRatedVoltage .Text = this.selectedDt.rated_voltage_secondary;         
            txtDtCapacityKva.Text = this.selectedDt.dt_capacity_kva;

            txtPrimaryCtRatio.Text = this.selectedDt.ct_primary;
            txtSecondaryCtRatio.Text = this.selectedDt.ct_secondary;
            txtPrimaryVtRatio.Text = this.selectedDt.vt_primary;
            txtSecondaryVtRatio.Text = this.selectedDt.vt_secondary;


            txtRphase.Text = this.selectedDt.r_phase;
            txtYphase.Text = this.selectedDt.y_phase;
            txtBphase.Text = this.selectedDt.b_phase;
            txtTotalCustomerCount.Text = this.selectedDt.total_customer_count;

            txtLocation.Text = this.selectedDt.location;
            txtLatitude.Text = this.selectedDt.latitude;
            txtLongitude.Text = this.selectedDt.longitude;
            txtElevation.Text = this.selectedDt.elevation;
        }

        private async  void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDtId.Text))
            {
                MessageBox.Show("Please fill in the fields marked with a star (*).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                progressLogin.Visibility = Visibility.Collapsed;
            }

            else
            {
                progressLogin.Visibility = Visibility.Visible;
                try
                {

                    DT dT = new DT()
                    {


                        //DistrictName = txtDistrictName.Text,
                        //DistrictCode = txtDistrictCode .Text,
                        //EsdName = txtEsdName .Text,
                        EsdCode = txtEsdCode.Text,
                        FeederName = txtFeederName.Text,
                        FeederId = txtFeederId.Text,
                        PortionId = txtPortionId.Text,
                        RootId = txtRootId.Text,

                        DtName = txtDtName.Text,
                        DtId = txtDtId.Text,
                        TransformerSerialNo = txtDtTransSerial.Text,


                        DtCapacityKva = txtDtCapacityKva.Text,
                        RatedVoltagePrimary = txtPrimaryRatedVoltage.Text,
                        RatedVoltageSecondary = txtSecondaryRatedVoltage.Text,
                        CTPrimary = txtPrimaryCtRatio.Text,
                        CTSecondary = txtSecondaryCtRatio.Text,
                        VTPrimary = txtPrimaryVtRatio.Text,
                        VTSecondary = txtSecondaryVtRatio.Text,



                        Rphase = txtRphase.Text,
                        Yphase = txtYphase.Text,
                        Bphase = txtBphase.Text,
                        TotalCustomerCount = txtTotalCustomerCount.Text,

                        Latitude = txtLatitude.Text,
                        Longitude = txtLongitude.Text,
                        Elevation = txtElevation.Text,
                        Location = txtLocation.Text

                    };


                    string url = string.Format("http://103.234.126.43:3080/dtmeter/district/dtinfo/{0}/{1}/{2}", CurrentDistrictCode, CurrentEsdCode, txtDtId.Text);


                    HttpClient client = new HttpClient();
                    string jsonData = JsonConvert.SerializeObject(dT);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {

                        addNewUserEvent();
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show("Error updating DT configuration. Please check the details and try again.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

                    EventLogs = "Edited DT",

                    Date = UserEventDate,
                    Time = UserEventTime,

                    StatusAdmin1 = "Not Acknowledged",
                    StatusAdmin2 = "Not Acknowledged",
                    StatusAdmin3 = "Not Acknowledged",

                    Remarks = "Edited : " + txtDtId.Text,
                };

                string url = "http://103.234.126.43:3080/dtmeter/logs/events/userevents";
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(userEventPost);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    GetDtIdAutoSuggest();

                    MessageBox.Show("DT successfully updated.", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);

                    Close();

                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Error updating DT configuration. Please check the details and try again.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        #region Auto suggest save xml


        private async void GetDtIdAutoSuggest()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Replace "YourApiUrl" with the actual URL of your API
                    //string apiUrl = "http://103.234.126.43:3080/dtmeter/info/esd_essd_info/all";

                    string apiUrl = "http://103.234.126.43:3080/dtmeter/info/esd_essd_info/all";


                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();

                        var dtIdValues = ParseApiResponse(result);

                        ClearDtIdValuesInXml("XMLFile1.xml");

                        SaveToXml(dtIdValues, "XMLFile1.xml");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ClearDtIdValuesInXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                doc.Root.Elements("dt_id").Remove();

                doc.Save(filePath);

                //MessageBox.Show($"Existing dt_id values cleared in XML file '{filePath}'.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing dt_id values in XML file: {ex.Message}");
            }
        }



        private List<string> ParseApiResponse(string apiResponse)
        {

            var jsonArray = JArray.Parse(apiResponse);
            var dtIdValues = jsonArray.Select(item => item["dt_id"].ToString()).ToList();

            return dtIdValues;
        }

        private void SaveToXml(List<string> dtIdValues, string fileName)
        {
            try
            {
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                XDocument doc;

                if (File.Exists(filePath))
                {
                    doc = XDocument.Load(filePath);

                    doc.Root.Add(dtIdValues.Select(dtId => new XElement("dt_id", dtId)));
                }
                else
                {
                    doc = new XDocument(
                        new XElement("dt_ids",
                            dtIdValues.Select(dtId => new XElement("dt_id", dtId))
                        )
                    );
                }

                doc.Save(filePath);

                //MessageBox.Show($"XML file '{fileName}' saved successfully at '{filePath}'.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving XML file: {ex.Message}");
            }
        }


        #endregion Auto suggest save xml
    }
}
