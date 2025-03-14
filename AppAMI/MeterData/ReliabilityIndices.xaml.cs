using System.Windows;
using System.Windows.Controls;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using Microsoft.Win32;
using System.IO;
using System;
using System.Net;
using AppAMI.Classes;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Globalization;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net.Http;

namespace AppAMI.MeterData
{
    /// <summary>
    /// Interaction logic for ReliabilityIndices.xaml
    /// </summary>
    public partial class ReliabilityIndices : UserControl
    {
        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;


        public ReliabilityIndices(string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1, string DTId1)
        {
            InitializeComponent();

            CurrentUserId2 = CurrentUserId1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;

            grdEmailAddress.Visibility = Visibility.Collapsed;


            txtDtId.Loaded += TxtDtId_Loaded;
            txtUserRole.Loaded += TxtUserRole_Loaded;
            txtDtId.TextChanged += TxtDtId_TextChanged;

        }

        private void TxtDtId_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private void TxtUserRole_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentUserRole2 = txtUserRole.Text;
        }

        private async void TxtDtId_TextChanged(object sender, TextChangedEventArgs e)
        {
            await ReadDatabaseAsync();

            await getReliabilitySum();
        }


        #region ReadDatabase
        


        private bool showErrorMessage = true;


        private async Task ReadDatabaseAsync()
        {
            progressLogin.Visibility = Visibility.Visible;

            try
            {
                if (string.IsNullOrEmpty(txtDtId.Text))
                {
                    if (showErrorMessage)
                    {
                        MessageBox.Show("Enter DT Id", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    return;
                }

                using (WebClient webClient = new WebClient())
                {
                    //string url = $"http://103.234.126.43:3080/dtmeter/reliability/index/{txtDtId.Text}";

                    string url = $"http://103.234.126.43:3080/dtmeter/reliability/monthly/{txtDtId.Text}";

                    string json = await webClient.DownloadStringTaskAsync(url);

                    // Deserialize the JSON into a list of objects
                    List<ReliabilityIndexClass> allData = JsonConvert.DeserializeObject<List<ReliabilityIndexClass>>(json);

                    await Dispatcher.InvokeAsync(() =>
                    {
                        datagridReliability.ItemsSource = allData;
                    });

                    showErrorMessage = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data: {ex.Message}. Please try again.", "Data Retrieval Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressLogin.Visibility = Visibility.Collapsed;
            }
        }


        private async Task getReliabilitySum()
        {
            //progressMeterData.Visibility = Visibility.Visible;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string url = $"http://103.234.126.43:3080/dtmeter/reliability/index/{txtDtId.Text}";


                    string json = await httpClient.GetStringAsync(url);

                    // Assuming the JSON is a single object, not an array
                    ReliabilityIndexClass dtMeter = JsonConvert.DeserializeObject<ReliabilityIndexClass>(json);

                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (dtMeter != null)
                        {
                            lblConsumerServed.Content = dtMeter.total_customer_count;
                            lblInterruptionDuration.Content = dtMeter.sustain_interruption_duration_hrs;
                            lblSusInterruptionTimes.Content = dtMeter.sustain_interruption_times;
                            lblMomentaryInterruptionTimes.Content = dtMeter.momentary_interruption_times;

                            lblSaifi.Content = dtMeter.saifi;
                            lblSaidi.Content = dtMeter.saidi;
                            lblMaifi.Content = dtMeter.maifi;

                        }
                        else
                        {
                            MessageBox.Show("Invalid DT Id", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    });
                }
            }
            catch (HttpRequestException)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            catch (JsonException jsonEx)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error parsing JSON data: {jsonEx.Message}", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            catch (Exception ex)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                //progressMeterData.Visibility = Visibility.Collapsed;
            }
        }

        #endregion ReadDatabase

        #region Export

        private void btnExport_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();

            // Set the background color of the context menu
            contextMenu.Background = (Brush)new BrushConverter().ConvertFrom("#202020");

            // Create a reusable style for the menu items
            Style menuItemStyle = new Style(typeof(MenuItem));
            menuItemStyle.Setters.Add(new Setter(ForegroundProperty, Brushes.White));
            menuItemStyle.Setters.Add(new Setter(FontSizeProperty, 12.0));

            // Export menu items
            MenuItem menExportXls = new MenuItem { Header = "Export as .xls", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.MicrosoftExcel, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
            menExportXls.Click += MenExportXls_Click;
            contextMenu.Items.Add(menExportXls);

            //MenuItem menExportPdf = new MenuItem { Header = "Export as .pdf", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.FilePdfBox, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
            //menExportPdf.Click += menExportPdf_Click;
            //contextMenu.Items.Add(menExportPdf);

            MenuItem menExportCsv = new MenuItem { Header = "Export as .csv", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.FileCsv, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
            menExportCsv.Click += menExportCsv_Click;
            contextMenu.Items.Add(menExportCsv);


            MenuItem menExportEmail = new MenuItem { Header = "Send as Email", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.Email, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
            menExportEmail.Click += MenExportEmail_Click; ;
            contextMenu.Items.Add(menExportEmail);


            btnExport.ContextMenu = contextMenu;
            btnExport.Focus();
        }

        private void MenExportEmail_Click(object sender, RoutedEventArgs e)
        {
            grdEmailAddress.Visibility = Visibility.Visible;



        }



        private async void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                progressLogin.Visibility = Visibility.Visible;

                string fromMail = "fuzzymri2023@gmail.com";
                string fromPassword = "fjzjgdlvesuonohj";

                MailMessage message2 = new MailMessage
                {
                    From = new MailAddress(fromMail),
                    Subject = "Reliability Indices",
                    To = { new MailAddress(txtEmailAdress.Text) },
                    Body = "<html><body> Kindly find the attached Reliability Indices</body></html>",
                    IsBodyHtml = true
                };

                // Create a new SmtpClient instance within the method scope
                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential(fromMail, fromPassword);
                    smtpClient.EnableSsl = true;

                    // Export the DataGrid to Excel
                    var options = new ExcelExportingOptions();
                    options.ExcelVersion = ExcelVersion.Excel2013;
                    var excelEngine = datagridReliability.ExportToExcel(datagridReliability.View, options);
                    var workBook = excelEngine.Excel.Workbooks[0];

                    // Save the Excel file to a temporary location
                    string tempFilePath = Path.GetTempFileName();
                    workBook.SaveAs(tempFilePath);
                    workBook.Close();

                    // Attach the Excel file to the email
                    Attachment attachment = new Attachment(tempFilePath);
                    attachment.ContentType.MediaType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    attachment.Name = "Reliability_" + txtDtId.Text + ".xlsx";

                    message2.Attachments.Add(attachment);

                    // Run the email sending logic on a background thread
                    await Task.Run(() => SendEmailWithAttachment(smtpClient, message2));

                    // The email sending is complete; you can now update UI or perform other tasks
                    MessageBox.Show("Reliability Indices Successfully Sent", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Close the attachment stream after sending the email
                    attachment.Dispose();

                    grdEmailAddress.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                MessageBox.Show("Failed to send email. Please check your email settings and try again.", "Send Email Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Hide the progressLogin control after the email is sent or an error occurs
                progressLogin.Visibility = Visibility.Collapsed;
            }
        }

        private void SendEmailWithAttachment(SmtpClient smtpClient, MailMessage message)
        {
            smtpClient.Send(message);
        }




        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            grdEmailAddress.Visibility = Visibility.Collapsed;
        }


        private void MenExportXls_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = datagridReliability.ExportToExcel(datagridReliability.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                FilterIndex = 2,
                Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx",
                FileName = "Reliability Indices_" + txtDtId.Text + ".xlsx"

            };

            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = sfd.OpenFile())
                {

                    if (sfd.FilterIndex == 1)
                        workBook.Version = ExcelVersion.Excel97to2003;

                    else if (sfd.FilterIndex == 2)
                        workBook.Version = ExcelVersion.Excel2010;

                    else
                        workBook.Version = ExcelVersion.Excel2013;
                    workBook.SaveAs(stream);
                }

                //Message box confirmation to view the created workbook.

                if (MessageBox.Show("Do you want to view the workbook?", "Workbook has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {

                    //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }


        private void menExportCsv_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;

            var excelEngine = datagridReliability.ExportToExcel(datagridReliability.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV Files(*.csv)|*.csv",
                FileName = "Reliability Indices_" + txtDtId.Text +  ".csv"

            };

            if (sfd.ShowDialog() == true)
            {
                string filePath = sfd.FileName;

                if (System.IO.Path.GetExtension(filePath) != ".csv")
                {
                    filePath = System.IO.Path.ChangeExtension(filePath, ".csv");
                }

                workBook.SaveAs(filePath, ",");

                if (MessageBox.Show("Do you want to open the CSV file?", "CSV file has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(filePath);
                }
            }
        }

        private void menExportPdf_Click(object sender, RoutedEventArgs e)
        {
            var document = datagridReliability.ExportToPdf();
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Files(*.pdf)|*.pdf",
                FileName = "Reliability Indices_" + txtDtId.Text  + ".pdf"

            };

            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = sfd.OpenFile())
                {
                    document.Save(stream);
                }

                if (MessageBox.Show("Do you want to view the Pdf file?", "Pdf file has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }



        #endregion Export

        private void datagridReliability_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {

        }
    }
}
