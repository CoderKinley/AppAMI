using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using AppAMI.Classes;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AppAMI.UserConfig
{
    /// <summary>
    /// Interaction logic for WebUserAdd.xaml
    /// </summary>
    public partial class WebUserAdd : Window
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

        string WebUserId;
        string WebUserName;
        string WebUserRole = "Basic User";
        string WebUserPassword;
        string WebUserOrganization;
        string WebUserDepartment;
        string WebUserDesignation;
        string WebUserEmail;
        string WebUserPhone;
        string WebUserEmployeeId;




        

        public WebUserAdd(string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1)
        {
            InitializeComponent();

            CurrentUserId2 = CurrentUserId1;
            CurrentUserRole2 = CurrentUserRole1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;



            ReadDatabase();
        }


        void ReadDatabase()
        {
            try
            {
                using (WebClient web = new WebClient())
                {
                    string url = string.Format("http://103.234.126.43:3080/dtmeter/newuser");
                    string json = web.DownloadString(url);

                    List<AdminConfig> all_data = JsonConvert.DeserializeObject<List<AdminConfig>>(json);

                    datagridUser.ItemsSource = all_data;

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Unable to Connect to the Server" + ex.Message);
            }
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {          
            Close();
        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {

            AdminConfig selectedAdminConfig = datagridUser.SelectedItem as AdminConfig;

             WebUserName = selectedAdminConfig.user_name;

            WebUserOrganization = selectedAdminConfig.organisation ;
            WebUserDepartment = selectedAdminConfig.department ;
            WebUserDesignation = selectedAdminConfig.designation ;

            WebUserEmail = selectedAdminConfig.email ;
            WebUserPhone = selectedAdminConfig.phone_number ;
            WebUserEmployeeId = selectedAdminConfig.employee_id ;

            generateBacisUserId();

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

                    StrUserCount = GetUniqueId(serials);

                    StrUserCount = "u3" + StrUserCount;

                    getUserCount();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to Connect to the Server" + ex.Message);
            }

            getUserCount();
            generatePassword();
        }

        private string GetUniqueId(string[] serials)
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
     
        private void getUserCount()
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

            WebUserId = adminUserInitials + genDate + currentAdminUserCount;

        }

        private void generatePassword()
        {
            Random rand = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            WebUserPassword = new string(Enumerable.Repeat(chars + "!@#$%^&*()_+", 10).Select(s => s[rand.Next(s.Length)]).ToArray());

            saveNewUserInfo();
        }

        private async void saveNewUserInfo()
        {
            try
            {
                AdminConfigPost adminConfigPost = new AdminConfigPost()
                {
                    UserID = WebUserId ,
                    UserName = WebUserName,
                    Password = WebUserPassword,

                    Organization = WebUserOrganization ,
                    Department = WebUserDepartment,
                    Designation = WebUserDesignation,

                    UserRole = WebUserRole,
                    Email = WebUserEmail,
                    Phone = WebUserPhone,

                    EmployeeId = WebUserEmployeeId
                };

                string url = "http://103.234.126.43:3080/dtmeter/user";
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(adminConfigPost);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {

                    addNewUserEvent();
                    updateUserId();
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

                    Remarks = "Added : " + WebUserId,
                };

                string url = "http://103.234.126.43:3080/dtmeter/logs/events/userevents";
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(userEventPost);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    DeleteFromWeb();
                    AddImage();

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

        private async  void AddImage()
        {
            try
            {
                AdminConfigPost adminConfigPost = new AdminConfigPost()
                {
                    UserID = WebUserId,
                    //UserName = txtName.Text,
                    //Password = passPassword.Password,

                    //Organization = cbOrganization.Text,
                    //Department = cbDepartment.Text,
                    //Designation = cbDesignation.Text,

                    //UserRole = cbUserRole.Text,
                    //Email = txtEmail.Text,
                    //Phone = txtPhone.Text,

                    //EmployeeId = txtEmployeeId.Text
                };

                string url = "http://103.234.126.43:3080/dtmeter/newuser/Image";
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(adminConfigPost);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);

            }

            catch
            {
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }


        private async  void DeleteFromWeb()
        {
            try
            {
                AdminConfig adminConfig = new AdminConfig()
                {
                    //user_id = txtUserId.Text,
                    //user_name = txtName.Text,
                    //password = txtPassword.Text,

                    //oranisation = txt
                    //department = txtDepartment.Text,
                    //designation = txtDesignation.Text,
                    //user_role = cbUserRole.Text,


                };


                string url = string.Format("http://103.234.126.43:3080/dtmeter/newuser/{0}", WebUserEmployeeId);
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(adminConfig);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.DeleteAsync(url);

                //if (response.IsSuccessStatusCode)
                //{

                //    //addNewUserEvent();

                //}
                //else
                //{
                //    string errorMessage = await response.Content.ReadAsStringAsync();
                //    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //}
            }

            catch
            {
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            finally
            {

                progressLogin.Visibility = Visibility.Collapsed;

            }

            sendEmail();
        }

        private async  void sendEmail()
        {

            progressLogin.Visibility = Visibility.Visible;

            if (string.IsNullOrWhiteSpace(WebUserEmail) || string.IsNullOrWhiteSpace(WebUserRole))
            {
                MessageBox.Show("Please fill in the fields marked with a star (*).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                progressLogin.Visibility = Visibility.Collapsed;
            }


            else
            {
                try
                {
                    
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("fuzzymri2023@gmail.com", "fjzjgdlvesuonohj"),
                        EnableSsl = true,
                    };

                    MailMessage message = new MailMessage
                    {
                        From = new MailAddress("fuzzymri2023@gmail.com"),
                        To = { WebUserEmail }, // Set the recipient email address here
                        Subject = "New User Credentials",
                        Body = $"<html><body> You have been registered as a new User for DT MRI.<br/> Your new user Id is: <b><font color='red'>{WebUserId}</font></b><br/> Your new password is: <b><font color='red'>{WebUserPassword}</font></b></body></html>",
                        IsBodyHtml = true
                    };

                    // Run the email sending logic on a background thread
                    await Task.Run(() => SendEmail(smtpClient, message));

                    // The email sending is complete; you can now update UI or perform other tasks
                   
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

        private void btnDisApprovelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteFromWeb();

            MessageBox.Show("User Declined ");

        }
    }
}
