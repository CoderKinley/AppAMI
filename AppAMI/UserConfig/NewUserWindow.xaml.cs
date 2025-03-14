using AppAMI.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace AppAMI.UserConfig
{
    /// <summary>
    /// Interaction logic for NewUserWindow.xaml
    /// </summary>
    public partial class NewUserWindow : Window
    {
        string StrUserCount;

        string adminUserInitials;
        string adminUserCount;
        string currentAdminUserCount;


        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;


        public NewUserWindow(string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1)
        {
            InitializeComponent();

            CurrentUserId2 = CurrentUserId1;
            CurrentUserRole2 = CurrentUserRole1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;

            GetUserOrganisation();
            GetUserDepartment();
            GetUserDesignation();

            Task.Run(() => ReadDataBase());


        }

        #region Get Combobox Items
        private void GetUserOrganisation()
        {
            try
            {
                var userOrgaList = ReadUserOrgaFromXml("UserOrga.xml")
                    .ToList();

                UpdateOrgaComboBox(userOrgaList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private List<string> ReadUserOrgaFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var UserOrgas = doc.Descendants("user_orga")
                                    .Select(element => element.Value)
                                    .ToList();

                return UserOrgas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }
        private void UpdateOrgaComboBox(List<string> userOrgaList)
        {
            cbOrganization.ItemsSource = userOrgaList;
        }



        private void GetUserDepartment()
        {
            try
            {
                var userDepartList = ReadUserDepartFromXml("UserDepart.xml")
                    .ToList();

                UpdateUserDepartComboBox(userDepartList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private List<string> ReadUserDepartFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var UserOrgas = doc.Descendants("user_depart")
                                    .Select(element => element.Value)
                                    .ToList();

                return UserOrgas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }

        private void UpdateUserDepartComboBox(List<string> userDepartList)
        {
            cbDepartment.ItemsSource = userDepartList;
        }




        private void GetUserDesignation()
        {
            try
            {
                var userDesigList = ReadUserDesigFromXml("UserDesig.xml")
                    .ToList();

                UpdateuserDesigComboBox(userDesigList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private List<string> ReadUserDesigFromXml(string filePath)
        {
            try
            {
                XDocument doc = XDocument.Load(filePath);

                var UserDesigs = doc.Descendants("user_desig")
                                    .Select(element => element.Value)
                                    .ToList();

                return UserDesigs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading XML file: {ex.Message}");
                return new List<string>();
            }
        }

        private void UpdateuserDesigComboBox(List<string> userDesigList)
        {
            cbDesignation.ItemsSource = userDesigList;
        }

        #endregion Get Combobox Items




        #region restrict operator count to 3 
        private async Task ReadDataBase()
        {
            try
            {
                // Show progress bar at the beginning
                Dispatcher.Invoke(() => progressLogin.Visibility = Visibility.Visible);

                using (WebClient web = new WebClient())
                {
                    string url = "http://103.234.126.43:3080/dtmeter/user";
                    string json = await web.DownloadStringTaskAsync(url);

                    List<AdminConfig> all_data = JsonConvert.DeserializeObject<List<AdminConfig>>(json);

                    // Update UI on the UI thread
                    Dispatcher.Invoke(() =>
                    {
                        List<AdminConfig> operators = all_data.Where(a => a.user_role == "Operator").ToList();

                        int operatorCount = operators.Count;

                        if (operatorCount >= 3)
                        {
                            cbOperator.Visibility = Visibility.Collapsed;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                // Update UI on the UI thread
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                // Hide progress bar at the end
                Dispatcher.Invoke(() => progressLogin.Visibility = Visibility.Collapsed);
            }
        }


        #endregion restrict operator count to 3 


        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
            progressLogin.Visibility = Visibility.Visible;

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(cbUserRole.Text))
            {
                MessageBox.Show("Please fill in the fields marked with a star (*).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                progressLogin.Visibility = Visibility.Collapsed;
            }


            else
            {
                try
                {
                    // Generate password and update UI on the UI thread
                    Random rand = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    string password = new string(Enumerable.Repeat(chars + "!@#$%^&*()_+", 10).Select(s => s[rand.Next(s.Length)]).ToArray());

                    // Use the Dispatcher to update UI controls on the UI thread
                    await Dispatcher.InvokeAsync(() =>
                    {
                        passPassword.Password = password;
                    });

                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("fuzzymri2023@gmail.com", "fjzjgdlvesuonohj"),
                        EnableSsl = true,
                    };

                    MailMessage message = new MailMessage
                    {
                        From = new MailAddress("fuzzymri2023@gmail.com"),
                        To = { txtEmail.Text }, // Set the recipient email address here
                        Subject = "New User Credentials",
                        Body = $"<html><body> You have been registered as a new User for DT MRI.<br/> Your new user Id is: <b><font color='red'>{txtUserId.Text}</font></b><br/> Your new password is: <b><font color='red'>{password}</font></b></body></html>",
                        IsBodyHtml = true
                    };

                    // Run the email sending logic on a background thread
                    await Task.Run(() => SendEmail(smtpClient, message));

                    // The email sending is complete; you can now update UI or perform other tasks
                    await Dispatcher.InvokeAsync(() =>
                    {
                        updateUserId();
                        saveNewUserInfo();
                    });
                }
                catch 
                {

                    MessageBox.Show("Failed to send email. Please check your email settings and try again.", "Send Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    // Hide the progressLogin control after the email is sent or an error occurs
                    await Dispatcher.InvokeAsync(() =>
                    {
                        progressLogin.Visibility = Visibility.Collapsed;
                    });
                }
            }


        }

        private void SendEmail(SmtpClient smtpClient, MailMessage message)
        {
            smtpClient.Send(message);

        }


        private async void updateUserId()
        {
            try
            {
                UserCount userCount = new UserCount()
                {
                    UserCountNumber = currentAdminUserCount,
                    //UserCountNumber = "1",
                };


                string url = string.Format("http://103.234.126.43:3080/dtmeter/usercount/{0}", adminUserInitials);
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(userCount);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseMessage = await response.Content.ReadAsStringAsync();
                    //MessageBox.Show(responseMessage);
                }
                else
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            catch 
            {
               
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void saveNewUserInfo()
        {         
            try
            {
                AdminConfigPost adminConfigPost = new AdminConfigPost()
                {
                    UserID = txtUserId.Text,
                    UserName = txtName.Text,
                    Password = passPassword.Password,

                    Organization = cbOrganization.Text,
                    Department = cbDepartment.Text,
                    Designation = cbDesignation.Text,

                    UserRole = cbUserRole.Text,
                    Email = txtEmail.Text,
                    Phone = txtPhone.Text,

                    EmployeeId = txtEmployeeId.Text
                };

                string url = "http://103.234.126.43:3080/dtmeter/user";
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(adminConfigPost);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                   
                    addNewUserEvent();
                }
                else
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }



            catch 
            {
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);

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
                    //EventLogs = "Added User, UID: " + txtUserId.Text,
                    EventLogs = "Added User",
                    Date = UserEventDate,
                    Time = UserEventTime,

                    StatusAdmin1 = "Not Acknowledged",
                    StatusAdmin2 = "Not Acknowledged",
                    StatusAdmin3 = "Not Acknowledged",

                    Remarks = "Added : " + txtUserId.Text,
                };

                string url = "http://103.234.126.43:3080/dtmeter/logs/events/userevents";
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(userEventPost);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {

                    MessageBox.Show("User successfully added.", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }


            catch 
            {
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //Close();
            }
        }

        private void cbOperator_Selected(object sender, RoutedEventArgs e)
        {
            generateOperatorUserId();
        }

        private void cbBasicUser_Selected(object sender, RoutedEventArgs e)
        {
            generateBacisUserId();
        }

        private void generateOperatorUserId()
        {

            try
            {
                using (WebClient web = new WebClient())
                {
                    string url = string.Format("http://103.234.126.43:3080/dtmeter/user");
                    string json = web.DownloadString(url);

                    List<AdminConfig> all_data = JsonConvert.DeserializeObject<List<AdminConfig>>(json);

                    List<AdminConfig> admins = all_data.Where(a => a.user_role == "Operator").ToList();

                    string serial1 = admins.Count > 0 ? admins[0].user_id.Substring(2, 2) : "";
                    string serial2 = admins.Count > 1 ? admins[1].user_id.Substring(2, 2) : "";

                    StrUserCount = (serial1 == "01" && serial2 == "02") || (serial1 == "02" && serial2 == "01") ? "03" :
                                 (serial1 == "01" && serial2 == "03") || (serial1 == "03" && serial2 == "01") ? "02" :
                                 (serial1 == "02" && serial2 == "03") || (serial1 == "03" && serial2 == "02") ? "01" :
                                 admins.Count == 0 ? "01" : admins.Count == 1 ? "02" : "03";


                    StrUserCount = "u2" + StrUserCount ;
                    getUserCountOperator();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to Connect to the Server" + ex.Message);
            }

        }

        private void generateBacisUserId()
        {
            try
            {
                using (WebClient web = new WebClient())
                {
                    string url = string.Format("http://103.234.126.43:3080/dtmeter/user");
                    string json = web.DownloadString(url);

                    List<AdminConfig> all_data = JsonConvert.DeserializeObject<List<AdminConfig>>(json);

                    List<AdminConfig> basicUser = all_data.Where(a => a.user_role == "Basic User").ToList();

                    string[] serials = new string[50];

                    for (int i = 0; i < basicUser.Count && i < 50; i++)
                    {
                        serials[i] = basicUser[i].user_id.Substring(2, 2);
                    }

                    StrUserCount = GetUniqueAdminId(serials);

                    StrUserCount = "u3" + StrUserCount;

                    getUserCountOperator();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to Connect to the Server" + ex.Message);
            }

            //getUserCount();
        }

        private void getUserCountOperator()
        {

            try
            {
                using (WebClient web = new WebClient())
                {
                    string url = string.Format("http://103.234.126.43:3080/dtmeter/usercount");
                    string json = web.DownloadString(url);

                    List<UserCount> all_data = JsonConvert.DeserializeObject<List<UserCount>>(json);
                    List<UserCount> userCounts = all_data.Where(a => a.user_id == StrUserCount).ToList();

                    if (userCounts.Count > 0)
                    {
                        adminUserInitials = userCounts[0].user_id;
                        adminUserCount = userCounts[0].user_count;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to Connect to the Server" + ex.Message);
            }

            DateTime now = DateTime.Now;
            string genDate = now.ToString("ddMMyy");

            int rootUserCountInt = int.Parse(adminUserCount);
            int currentRootUserCountInt = rootUserCountInt + 1;
            currentAdminUserCount = currentRootUserCountInt.ToString();

            txtUserId.Text = adminUserInitials + genDate + currentAdminUserCount;

        }

        private string GetUniqueAdminId(string[] serials)
        {
            string uniqueId = "";

            for (int i = 1; i <= 50; i++)
            {
                bool isUnique = true;

                for (int j = 0; j < serials.Length && serials[j] != null; j++)
                {
                    if (serials[j] == i.ToString("D2"))
                    {
                        isUnique = false;
                        break;
                    }
                }

                if (isUnique)
                {
                    uniqueId = i.ToString("D2");
                    break;
                }
            }

            if (uniqueId == "")
            {
                uniqueId = "01";
            }

            return uniqueId;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}


