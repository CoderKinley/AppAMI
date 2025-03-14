using AppAMI.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace AppAMI.RootUser
{
    /// <summary>
    /// Interaction logic for NewRootWindow.xaml
    /// </summary>
    public partial class NewRootWindow : Window
    {
        string rootUserInitials;
        string  rootUserCount;
        string currentRootUserCount;


        string otp1;
        string otp2;

        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;
        string CurrentUserEmail2;


        string NewUserId;

        public NewRootWindow(string currentRootEmail, string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1)
        {
            InitializeComponent();

            CurrentUserId2 = CurrentUserId1;
            CurrentUserRole2 = CurrentUserRole1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;
            CurrentUserEmail2 = currentRootEmail;

            GetUserOrganisation();
            GetUserDepartment();
            GetUserDesignation();

            InitializeAsync();

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




        private async void InitializeAsync()
        {
            // Any synchronous initialization can be done here

            // Asynchronously call getUserId
            await GetUserIdAsync();
        }



        private async Task GetUserIdAsync()
        {
            try
            {
                // Show progress bar at the beginning
                Dispatcher.Invoke(() => progressLogin.Visibility = Visibility.Visible);

                using (WebClient web = new WebClient())
                {
                    string url = "http://103.234.126.43:3080/dtmeter/usercount";
                    string json = await web.DownloadStringTaskAsync(url);

                    List<UserCount> all_data = JsonConvert.DeserializeObject<List<UserCount>>(json);

                    // Update UI on the UI thread
                    Dispatcher.Invoke(() =>
                    {
                        List<UserCount> userCounts = all_data.Where(a => a.user_type == "Root User").ToList();

                        if (userCounts.Count > 0)
                        {
                            rootUserInitials = userCounts[0].user_id;
                            rootUserCount = userCounts[0].user_count;

                            GenerateUserId();
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

        private void GenerateUserId()
        {
            DateTime now = DateTime.Now;
            string genDate = now.ToString("ddMMyy");

            int rootUserCountInt = int.Parse(rootUserCount);
            int currentRootUserCountInt = rootUserCountInt + 1;
            currentRootUserCount = currentRootUserCountInt.ToString();

            NewUserId = rootUserInitials + genDate + currentRootUserCount;
        }






        private async void btnVeriCode_Click(object sender, RoutedEventArgs e)
        {
            progressLogin.Visibility = Visibility.Visible;

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please fill in the fields marked with a star (*).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Hide the progressLogin control when validation fails
                progressLogin.Visibility = Visibility.Collapsed;
            }
            else
            {
                try
                {
                    Random rand = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    otp1 = new string(Enumerable.Repeat(chars, 4).Select(s => s[rand.Next(s.Length)]).ToArray());
                    otp2 = new string(Enumerable.Repeat(chars, 4).Select(s => s[rand.Next(s.Length)]).ToArray());
                  

                    // Use the Dispatcher to update UI controls on the UI thread
                  

                    string fromMail = "fuzzymri2023@gmail.com";
                    string fromPassword = "fjzjgdlvesuonohj";

                    // Create a new SmtpClient instance within the method scope
                    using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                    {
                        smtpClient.Port = 587; // Set the correct port
                        smtpClient.Credentials = new NetworkCredential(fromMail, fromPassword);
                        smtpClient.EnableSsl = true;

                        MailMessage message1 = new MailMessage
                        {
                            From = new MailAddress(fromMail),
                            Subject = "OTP",
                            To = { new MailAddress(CurrentUserEmail2) },
                            Body = $"<html><body>You are transferring Root User credentials for DT MRI. <br/> Your OTP is: <b><font color='red'>{otp1}</font></b></body></html>",
                            IsBodyHtml = true
                        };

                        MailMessage message2 = new MailMessage
                        {
                            From = new MailAddress(fromMail),
                            Subject = "OTP",
                            To = { new MailAddress(txtEmail.Text) },
                            Body = $"<html><body>You are the new Root User for DT MRI. <br/> Your OTP is: <b><font color='red'>{otp2}</font></b></body></html>",

                            //Body = $"<html><body> You have been registered as a new Root User for DT MRI. <br/> Your OTP is: <b><font color='red'>{otp2}</font></b> <br/> Your new user Id is: <b><font color='red'>{txtUserId.Text}</font></b> <br/> Your password is: <b><font color='red'>{password}</font></b></body></html>",
                            IsBodyHtml = true
                        };

                        // Run the email sending logic on a background thread
                        await Task.Run(() => SendEmail(smtpClient, message1));
                        await Task.Run(() => SendEmail(smtpClient, message2));
                    }

                    // The email sending is complete; you can now update UI or perform other tasks
                    MessageBox.Show("Mail sent successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch 
                {
                    MessageBox.Show("Mail sending failed. Please check your email settings and try again.", "Send Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            try
            {
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                // Note: Logging exceptions is generally preferable to silently ignoring them
                MessageBox.Show($"Mail sending failed. Error message: {ex.Message}", "Send Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private async  void btnRootSignOutConfirm_Click(object sender, RoutedEventArgs e)
        {
            if(txtOtpCurrent.Text == otp1 && txtOtpNew.Text  == otp2)
            {
                MessageBox.Show("Current Root User signing out permanently. Good bye!!!");



                progressLogin.Visibility = Visibility.Visible;

                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Please fill in the fields marked with a star (*).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                    // Hide the progressLogin control when validation fails
                    progressLogin.Visibility = Visibility.Collapsed;
                }
                else
                {
                    try
                    {
                        Random rand = new Random();
                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

                        string password = new string(Enumerable.Repeat(chars + "!@#$%^&*()_+", 10).Select(s => s[rand.Next(s.Length)]).ToArray());

                        // Use the Dispatcher to update UI controls on the UI thread
                        await Dispatcher.InvokeAsync(() =>
                        {
                            passPassword.Password = password;
                        });

                        string fromMail = "fuzzymri2023@gmail.com";
                        string fromPassword = "fjzjgdlvesuonohj";

                        // Create a new SmtpClient instance within the method scope
                        using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                        {
                            smtpClient.Port = 587; // Set the correct port
                            smtpClient.Credentials = new NetworkCredential(fromMail, fromPassword);
                            smtpClient.EnableSsl = true;

                            MailMessage message1 = new MailMessage
                            {
                                From = new MailAddress(fromMail),
                                Subject = "Root User Transfer Notice",
                                To = { new MailAddress(CurrentUserEmail2) },
                                //Body = $"<html><body>You are transferring Root User credentials for DT MRI. <br/> Your OTP is: <b><font color='red'>{otp1}</font></b></body></html>",
                                Body = $"<html><body>You are Transferred Root User credentials for DT MRI</body></html>",

                                IsBodyHtml = true
                            };

                            MailMessage message2 = new MailMessage
                            {
                                From = new MailAddress(fromMail),
                                Subject = "New Root User Credentials",
                                To = { new MailAddress(txtEmail.Text) },

                                Body = $"<html><body> You have been registered as a new Root User for DT MRI.  <br/> Your new user Id is: <b><font color='red'>{NewUserId}</font></b> <br/> Your password is: <b><font color='red'>{password}</font></b></body></html>",
                                IsBodyHtml = true
                            };

                            // Run the email sending logic on a background thread
                            await Task.Run(() => SendEmail(smtpClient, message1));
                            await Task.Run(() => SendEmail(smtpClient, message2));


                            await Dispatcher.InvokeAsync(() =>
                            {
                                updateUserId();
                                saveNewUserDetails();
                            });
                        }

                        // The email sending is complete; you can now update UI or perform other tasks
                        MessageBox.Show("Mail sent successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch 
                    {
                        MessageBox.Show("Mail sending failed. Please check your email settings and try again.", "Send Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            else if (txtOtpCurrent.Text == "" || txtOtpNew.Text == "")
            {
                MessageBox.Show("Enter verification code", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            else
            {
                MessageBox.Show("Incorrect verification code", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private async  void updateUserId()
        {

            try
            {

                UserCount userCount   = new UserCount()
                {
                   UserCountNumber = currentRootUserCount,
                    //UserCountNumber = "1",

                };


                string url = string.Format("http://103.234.126.43:3080/dtmeter/usercount/{0}", rootUserInitials);
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(userCount);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(url, content);

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
            }
        }

        async void saveNewUserDetails()
        {
            if (txtEmail.Text != "")
            {
                try
                {

                    AdminConfigPost adminConfigPost = new AdminConfigPost()
                    {
                        UserID  = NewUserId ,
                        UserRole = CurrentUserRole2,
                        UserName = txtName.Text,
                        Organization = cbOrganization.Text,
                        Department = cbDepartment.Text,
                        Designation = cbDesignation.Text,
                        Email = txtEmail.Text,
                        Phone = txtPhone.Text,
                        Password = passPassword.Password,
                        EmployeeId = txtEmployeeId.Text 
                        
                    };


                    string url = string.Format("http://103.234.126.43:3080/dtmeter/user/{0}", CurrentUserId2 );
                    HttpClient client = new HttpClient();
                    string jsonData = JsonConvert.SerializeObject(adminConfigPost);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);

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

            else if (txtEmail.Text == "")
            {
                MessageBox.Show("Please fill in the fields marked with a star (*).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);

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

                    Remarks = "Added : " + NewUserId,
                };

                string url = "http://103.234.126.43:3080/dtmeter/logs/events/userevents";
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(userEventPost);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("User credentials successfully transferred.", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);
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


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
