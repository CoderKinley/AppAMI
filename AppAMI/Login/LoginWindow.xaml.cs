using AppAMI.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Management;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;

namespace AppAMI.Login
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        string userId;
        string userRole;
        string userName;
        string password;
        string employeeId;

        string macAddress1;
        string hardDiskSerialNumber1;
        string motherBoardSerialNumber1;
        string biosIdentifier1;

        public LoginWindow()
        {
            InitializeComponent();

            this.Title = "omniAMI";

            //   Loaded += MainWindow_Loaded;
        }

        #region Get Computer Information
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string macAddress = GetMacAddress();
            macAddress1 = macAddress;


            string serialNumber = GetHardDiskSerialNumber();
            hardDiskSerialNumber1 = serialNumber;


            string MotherBoardSerialNumber = GetMotherboardSerialNumber();
            motherBoardSerialNumber1 = MotherBoardSerialNumber;


            string BiosIdentifier = GetBIOSIdentifier();
            biosIdentifier1 = BiosIdentifier;

            //My Computer
            if (hardDiskSerialNumber1.Equals("CY09N054010509I5M   _00000001.") && motherBoardSerialNumber1.Equals("NBA2211004044BA96B3400") && biosIdentifier1.Equals("NXA29SG002107000480201"))
            {
                txtUserId.IsEnabled = true;
                passPassword.IsEnabled = true;
                btnLogin.IsEnabled = true;
            }

            //DMS PABX Computer

            else if (hardDiskSerialNumber1.Equals("Z9AY7KLC") && motherBoardSerialNumber1.Equals("PGSXR0E0GBBDBC") && biosIdentifier1.Equals("8CG8382KSB"))
            {
                txtUserId.IsEnabled = true;
                passPassword.IsEnabled = true;
                btnLogin.IsEnabled = true;
            }


            else
            {
                txtUserId.IsEnabled = false;
                passPassword.IsEnabled = false;
                btnLogin.IsEnabled = false;

                MessageBoxResult result = MessageBox.Show("Access Denied!!!", "Incompatible System", MessageBoxButton.OK, MessageBoxImage.Warning);

                if (result == MessageBoxResult.OK)
                {
                    Close();
                }
            }

        }

        private string GetMacAddress()
        {
            string macAddress = string.Empty;
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface nic in nics)
            {
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                    nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    macAddress = nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            return macAddress;
        }

        private string GetHardDiskSerialNumber()
        {
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='IDE' OR InterfaceType='SCSI'");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                return queryObj["SerialNumber"].ToString().Trim();
            }

            return string.Empty;
        }

        private string GetMotherboardSerialNumber()
        {
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                return queryObj["SerialNumber"].ToString().Trim();
            }

            return string.Empty;
        }

        private string GetBIOSIdentifier()
        {
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                return queryObj["SerialNumber"].ToString().Trim();
            }

            return string.Empty;
        }

        #endregion Get Computer Information

       
        async void Button_Click(object sender, RoutedEventArgs e)
        {
           
            try
            {
                progressLogin.Visibility = Visibility.Visible;

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync("http://103.234.126.43:3080/dtmeter/user");
                string responseBody = await response.Content.ReadAsStringAsync();

                List<AdminConfig> adminConfigs = JsonConvert.DeserializeObject<List<AdminConfig>>(responseBody);

                foreach (AdminConfig adminConfig in adminConfigs)
                {
                    if (adminConfig.user_id.Equals(txtUserId.Text) && adminConfig.password.Equals(passPassword.Password) )
                    {
                        userId = adminConfig.user_id;
                        userRole = adminConfig.user_role;
                        password = adminConfig.password;
                        userName = adminConfig.user_name;
                        employeeId = adminConfig.employee_id;

                        if(adminConfig.user_role.Equals("Operator") || adminConfig.user_role.Equals("Administrator") || adminConfig.user_role.Equals("Root User"))
                        {

                            MainWindow mainWindow = new MainWindow(userId, userRole, password, userName, employeeId);
                            mainWindow.Show();
                            addNewUserEvent();

                            Close();
                            return;
                        }

                        else if(adminConfig.user_role.Equals("Basic User"))
                        {
                            MessageBox.Show("Access denied. You do not have permission to perform this operation.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                            Close();
                        }


                        else 
                        {
                            MessageBox.Show("Incorrect Employee ID or Password. Please double-check your credentials and try again.", "Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error);

                            Close();
                        }

                        progressLogin.Visibility = Visibility.Collapsed;
                    }

                }

            }
            catch 
            {
                progressLogin.Visibility = Visibility.Collapsed ;
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void addNewUserEvent()
        {
            DateTime now = DateTime.Now;
            string UserEventDate = now.ToString("dd-MM-yyyy");
            string UserEventTime = now.ToString("h:mm tt");

            try
            {
                UserEventPost userEventPost = new UserEventPost()
                {
                    UserID = userId,
                    EmployeeId = employeeId,
                    UserName = userName,

                    //EventLogs = "Added User, UID: " + txtUserId.Text,
                    EventLogs = "User Logged In",

                    Date = UserEventDate,
                    Time = UserEventTime,

                    StatusAdmin1 = "Not Acknowledged",
                    StatusAdmin2 = "Not Acknowledged",
                    StatusAdmin3 = "Not Acknowledged",

                    Remarks = "Logged In : " + userId,
                };

                string url = "http://103.234.126.43:3080/dtmeter/logs/events/userevents";
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(userEventPost);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    //string responseMessage = await response.Content.ReadAsStringAsync();
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
                Close();
            }
        }

        private void btnForgotPw_Click(object sender, RoutedEventArgs e)
        {
            sendOtp();
        }

        private void sendOtp()
        {
            //Random rand = new Random();
            //const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            //string password = new string(Enumerable.Repeat(chars + "!@#$%^&*()_+", 10)
            //                             .Select(s => s[rand.Next(s.Length)]).ToArray());

            //passPassword.Password = password;

            //string fromMail = "fuzzymri2023@gmail.com";
            //string fromPassword = "fjzjgdlvesuonohj";


            //MailMessage message2 = new MailMessage();
            //message2.From = new MailAddress(fromMail);
            //message2.Subject = "New User Password";
            //message2.To.Add(new MailAddress(txtEmail.Text));
            //message2.Body = "<html><body> You have been registered as new user for DT MRI. <br/> Your new password is: <b><font color='red'>" + password + "</font></b></body></html>";
            //message2.IsBodyHtml = true;

            //SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            //{
            //    Port = 587,
            //    Credentials = new NetworkCredential(fromMail, fromPassword),
            //    EnableSsl = true,
            //};

            //try
            //{
            //    smtpClient.Send(message2);
            //    MessageBox.Show("New Password sent successfully.");

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Mail sending failed. Error message: " + ex.Message);
            //}
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}



