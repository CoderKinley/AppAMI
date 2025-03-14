using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System.IO;
using System.Globalization;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Net;
using AppAMI.Classes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Windows.Threading;

namespace AppAMI.MeterData
{
    /// <summary>
    /// Interaction logic for Events.xaml
    /// </summary>
    public partial class Events : UserControl
    {
        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;


        public Events(string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1, string DTId1)
        {
            InitializeComponent();

            CurrentUserId2 = CurrentUserId1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;

            grdEmailAddress.Visibility = Visibility.Collapsed;
            
            dtPickerEnd.IsEnabled = false;

            txtDtId.Loaded += TxtDtId_Loaded;
            txtUserRole.Loaded += TxtUserRole_Loaded;
            txtDtId.TextChanged += TxtDtId_TextChanged;
        }

        private async void TxtDtId_Loaded(object sender, RoutedEventArgs e)
        {
            dtPickerStart.IsEnabled = true;
            dtPickerEnd.IsEnabled = true;

            dtPickerStart.SelectedDate = DateTime.Today;
            dtPickerEnd.SelectedDate = DateTime.Today;

            await ReadDatabase();
        }

        private void TxtUserRole_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentUserRole2 = txtUserRole.Text;
        }

        private async void TxtDtId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dtPickerEnd.SelectedDate.HasValue)
            {
                await ReadDatabase();
            }

        }

        #region Read Database

        private async void dtPickerStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!dtPickerEnd.SelectedDate.HasValue)
            {
                return;  // Exit the method if the end date is not selected
            }

            if (dtPickerEnd.SelectedDate.HasValue)
            {
                await ReadDatabase();
            }

        }

        private async void dtPickerEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!dtPickerStart.SelectedDate.HasValue)
            {
                return;  // Exit the method if the start date is not selected
            }

            if (dtPickerEnd.SelectedDate.HasValue)
            {
                await ReadDatabase();
            }

        }

        private bool showErrorMessage = true;

        private async Task ReadDatabase()
        {
            progressLogin.Visibility = Visibility.Visible;

            try
            {
                if (!string.IsNullOrEmpty(txtDtId.Text))
                {
                    using (WebClient web = new WebClient())
                    {
                        string url = string.Format("http://103.234.126.43:3080/api/events/{0}", txtDtId.Text);
                        string json = await web.DownloadStringTaskAsync(url);

                        List<MeterEvents> all_data = JsonConvert.DeserializeObject<List<MeterEvents>>(json);

                        // Filter data based on date range using the format "yyyy-MM-dd"
                        List<MeterEvents> filtered_data = all_data
                            .Where(x => DateTime.ParseExact(x.record_date, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= dtPickerStart.SelectedDate.Value
                                        && DateTime.ParseExact(x.record_date, "yyyy-MM-dd", CultureInfo.InvariantCulture) <= dtPickerEnd.SelectedDate.Value)
                            .ToList();

                        // Bind filtered data to DataGrid
                        _ = Dispatcher.InvokeAsync(() =>
                        {
                            eventsDgv.ItemsSource = filtered_data;
                        });

                        // Reset the flag if the operation is successful
                        showErrorMessage = false;
                    }
                }
                else
                {
                    // Show the error message only if the flag is true
                    if (showErrorMessage)
                    {
                        MessageBox.Show("Enter DT Id", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error fetching data. Please try again.", "Data Retrieval Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressLogin.Visibility = Visibility.Collapsed;
            }
        }




        //private async Task ReadDatabase()
        //{
        //    progressLogin.Visibility = Visibility.Visible;

        //    try
        //    {
        //        if (!string.IsNullOrEmpty(txtDtId.Text))
        //        {
        //            using (WebClient web = new WebClient())
        //            {
        //                string url = string.Format("http://103.234.126.43:3080/api/events/{0}", txtDtId.Text);
        //                string json = await web.DownloadStringTaskAsync(url);

        //                List<MeterEvents> all_data = JsonConvert.DeserializeObject<List<MeterEvents>>(json);

        //                // Bind all data to DataGrid
        //                await Dispatcher.InvokeAsync(() =>
        //                {
        //                    eventsDgv.ItemsSource = all_data;
        //                });

        //                // Reset the flag if the operation is successful
        //                showErrorMessage = false;
        //            }
        //        }
        //        else
        //        {
        //            // Show the error message only if the flag is true
        //            if (showErrorMessage)
        //            {
        //                MessageBox.Show("Enter DT Id", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Error fetching data. Please try again.", "Data Retrieval Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    finally
        //    {
        //        progressLogin.Visibility = Visibility.Collapsed;
        //    }
        //}






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

        private void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {
            string fromMail = "fuzzymri2023@gmail.com";
            string fromPassword = "fjzjgdlvesuonohj";

            MailMessage message2 = new MailMessage();
            message2.From = new MailAddress(fromMail);
            message2.Subject = "Events Data";
            message2.To.Add(new MailAddress(txtEmailAdress.Text));
            message2.Body = "<html><body> Kindly find the attached Events Data</body></html>";
            message2.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            try
            {
                // Export the DataGrid to Excel
                var options = new ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = eventsDgv.ExportToExcel(eventsDgv.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];

                // Save the Excel file to a temporary location
                string tempFilePath = Path.GetTempFileName();
                workBook.SaveAs(tempFilePath);
                workBook.Close();

                // Attach the Excel file to the email
                Attachment attachment = new Attachment(tempFilePath);
                attachment.ContentType.MediaType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                attachment.Name = "Events_" + txtDtId.Text + "_From_" + dtPickerStart.Text + "_To_" + dtPickerEnd.Text + ".xlsx";
                message2.Attachments.Add(attachment);

                smtpClient.Send(message2);

                MessageBox.Show("Events Data Successfully Send");

                grdEmailAddress.Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Mail sending failed. Error message: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            grdEmailAddress.Visibility = Visibility.Collapsed;
        }

        private void MenExportXls_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = eventsDgv.ExportToExcel(eventsDgv.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                FilterIndex = 2,
                Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx",
                FileName = "Events_" + txtDtId.Text + "_From_" + dtPickerStart.Text + "_To_" + dtPickerEnd.Text + ".xlsx"

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

            var excelEngine = eventsDgv.ExportToExcel(eventsDgv.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV Files(*.csv)|*.csv",
                FileName = "Events_" + txtDtId.Text + "_From_" + dtPickerStart.Text + "_To_" + dtPickerEnd.Text + ".csv"

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
            var document = eventsDgv.ExportToPdf();
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Files(*.pdf)|*.pdf",
                FileName = "Events_" + txtDtId.Text + "_From_" + dtPickerStart.Text + "_To_" + dtPickerEnd.Text + ".pdf"

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

       



        //#region Checkboxes
        //private void cbNeutralLoss_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("ZZZ")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = true;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;

        //}

        //private void cbNeutralLoss_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbLowPowerFactor_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("ZZZ")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = true;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbLowPowerFactor_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbOverload_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("ZZZ")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }
        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = true;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbOverload_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbCurrentLoss_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("Current Loss")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = true ;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbCurrentLoss_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbPhaseFailure_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("Phase Loss")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = true ;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbPhaseFailure_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbVoltageOver_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("ZZZ")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;

        //    }
        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = true ;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbVoltageOver_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbMagneticInfluence_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("ZZZ")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }


        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = true ;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbMagneticInfluence_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbMeterTopCoverOpen_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("ZZZ")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = true ;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbMeterTopCoverOpen_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbCurrentPhaseSequenceReverse_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("ZZZ")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = true ;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbCurrentPhaseSequenceReverse_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbCurrentImbalance_Checked(object sender, RoutedEventArgs e)
        //{


        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("Current Imbalance")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = true ;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbCurrentImbalance_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbEnergyReverse_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("ZZZ")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = true ;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbEnergyReverse_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbVoltageLoss_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("Voltage Loss")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }



        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = true ;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbVoltageLoss_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbPowerOff_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("Power Break")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = true ;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbPowerOff_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbContactorFailure_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("ZZZ")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = true ;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbContactorFailure_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbTerminalCoverOpen_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("ZZZ")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = true ;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbTerminalCoverOpen_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbOverCurrent_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("ZZZ")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = true ;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbOverCurrent_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbVoltagePhaseSequenceReverse_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("Voltage Phase Sequence Reverse")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = true ;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbVoltagePhaseSequenceReverse_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbPhaseActivePowerReverse_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("Phase Active Power Reverse")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = true ;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbPhaseActivePowerReverse_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbVoltageUnder_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("Voltage Under")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = true ;
        //    cbFrequencyTransfinite.IsChecked = false;
        //}

        //private void cbVoltageUnder_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}


        //private void cbFrequencyTransfinite_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (filtered_data != null)
        //    {
        //        List<MeterEvents> filteredPowerOff = filtered_data.Where(a => a.events.Contains("ZZZ")).ToList();
        //        eventsDgv.ItemsSource = filteredPowerOff;
        //    }

        //    cbNeutralLoss.IsChecked = false;
        //    cbLowPowerFactor.IsChecked = false;
        //    cbOverload.IsChecked = false;
        //    cbCurrentLoss.IsChecked = false;
        //    cbPhaseFailure.IsChecked = false;

        //    cbVoltageOver.IsChecked = false;
        //    cbMagneticInfluence.IsChecked = false;
        //    cbMeterTopCoverOpen.IsChecked = false;
        //    cbCurrentPhaseSequenceReverse.IsChecked = false;
        //    cbCurrentImbalance.IsChecked = false;

        //    cbEnergyReverse.IsChecked = false;
        //    cbVoltageLoss.IsChecked = false;
        //    cbPowerOff.IsChecked = false;
        //    cbContactorFailure.IsChecked = false;
        //    cbTerminalCoverOpen.IsChecked = false;

        //    cbOverCurrent.IsChecked = false;
        //    cbVoltagePhaseSequenceReverse.IsChecked = false;
        //    cbPhaseActivePowerReverse.IsChecked = false;
        //    cbVoltageUnder.IsChecked = false;
        //    cbFrequencyTransfinite.IsChecked = true ;
        //}

        //private void cbFrequencyTransfinite_Unchecked(object sender, RoutedEventArgs e)
        //{

        //}

        //#endregion Checkboxes
    }
}
