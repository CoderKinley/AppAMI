using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Net.Http;
using AppAMI.Classes;
using Newtonsoft.Json;
using System;
using System.Xml.Linq;
using System.Linq;

namespace AppAMI.MeterConfig
{
    /// <summary>
    /// Interaction logic for UpdateMriWindow.xaml
    /// </summary>
    public partial class UpdateMriWindow : Window
    {
        Mri selectedDtMri;

        string CurrentDistrictCode;
        string CurrentEsdCode;

        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;

        public UpdateMriWindow(Mri selectedDtMri, string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1)
        {
            InitializeComponent();

            CurrentUserId2 = CurrentUserId1;
            CurrentUserRole2 = CurrentUserRole1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;

            this.selectedDtMri = selectedDtMri;

            CurrentDistrictCode = this.selectedDtMri.district_code;
            CurrentEsdCode = this.selectedDtMri.esd_code;

            txtDTId.Text = this.selectedDtMri.dt_id;
            txtMriSerialNo.Text = this.selectedDtMri.mri_serial_no;
            cbMriVersion.Text = this.selectedDtMri.mri_version;

            cbMriFirmwareVer.Text = this.selectedDtMri.mri_firmware_version;
            dtMriManuYear.Text = this.selectedDtMri.mri_manufacturing_year;
            dttxtMriInstallationDate.Text = this.selectedDtMri.mri_installation_date;



            GetMriVersion();
            GetMriFirmV();
        }


        private void GetMriVersion()
        {
            try
            {
                var mriVersionList = ReadMriVersionFromXml("MriVersion.xml")
                    .ToList();

                UpdateMriVersionList(mriVersionList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private List<string> ReadMriVersionFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var MriVersions = doc.Descendants("mri_version")
                                    .Select(element => element.Value)
                                    .ToList();

                return MriVersions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }

        private void UpdateMriVersionList(List<string> mriVersionList)
        {
            cbMriVersion.ItemsSource = mriVersionList;
        }


        private void GetMriFirmV()
        {
            try
            {
                var mriFirmVList = ReadMriFirmVFromXml("MriFirmV.xml")
                    .ToList();

                UpdateMriFirmVList(mriFirmVList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private List<string> ReadMriFirmVFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var MriFirmVs = doc.Descendants("mri_firm_v")
                                    .Select(element => element.Value)
                                    .ToList();

                return MriFirmVs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }

        private void UpdateMriFirmVList(List<string> mriFirmVList)
        {
            cbMriFirmwareVer.ItemsSource = mriFirmVList;
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

                    Mri mri = new Mri()
                    {
                        MriSerialNo = txtMriSerialNo.Text,
                        MriVersion = cbMriVersion.Text,
                        MriFirmwareVersion = cbMriFirmwareVer.Text,
                        MriManufacturingYear = dtMriManuYear.Text,
                        MriInstallationDate = dttxtMriInstallationDate.Text
                    };

                    string url = string.Format("http://103.234.126.43:3080/dtmeter/district/mriinfo/{0}/{1}/{2}", CurrentDistrictCode, CurrentEsdCode, txtDTId.Text);

                    HttpClient client = new HttpClient();
                    string jsonData = JsonConvert.SerializeObject(mri);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {

                        addNewUserEvent();

                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show("Error updating MRI configuration. Please check the details and try again.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

                    EventLogs = "Edited Mri",

                    Date = UserEventDate,
                    Time = UserEventTime,

                    StatusAdmin1 = "Not Acknowledged",
                    StatusAdmin2 = "Not Acknowledged",
                    StatusAdmin3 = "Not Acknowledged",

                    Remarks = "Edited Mri for : " + txtDTId.Text,
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
                    MessageBox.Show("Error updating MRI configuration. Please check the details and try again.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
