using AppAMI.Classes;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AppAMI.MeterConfig
{
    /// <summary>
    /// Interaction logic for MeterConfiguration.xaml
    /// </summary>
    public partial class MeterConfiguration : UserControl
    {
        string selectedEsd;
        //string selectedDistrict;
        //string selectedPlaceForData;

        List<UserEvent> dtAddEvent;
        List<UserEvent> dtEditEvent;
        List<UserEvent> dtDeleteEvent;
        List<UserEvent> dtMeterEditEvent;
        List<UserEvent> dtMeterDeleteEvent;
        List<UserEvent> dtMriEditEvent;
        List<UserEvent> dtMriDeleteEvent;
        List<UserEvent> EsdAddEvent;
        List<UserEvent> EsdDeleteEvent;

        private bool isOpControlDisabled = false;

        UserControl usc = null;


        string CurrentUserId1;
        string CurrentUserRole1;
        string CurrentUserPassword1;
        string CurrentUserName1;
        string CurrentUserEmployeeId1;

        string DTId1;

        string url;

        public MeterConfiguration(string CurrentUserId, string CurrentUserRole, string CurrentUserPassword, string CurrentUserName, string CurrentUserEmployeeId)
        {
            InitializeComponent();

            CurrentUserId1 = CurrentUserId;
            CurrentUserRole1 = CurrentUserRole;
            CurrentUserPassword1 = CurrentUserPassword;
            CurrentUserName1 = CurrentUserName;
            CurrentUserEmployeeId1 = CurrentUserEmployeeId;

            AccessLevelMethod();
            //Task.Run(() => UpdateNotifyBadgeContent());

        }

        private  void AccessLevelMethod()
        {
            if (CurrentUserRole1.Equals("Operator"))
            {
                isOpControlDisabled = true;
            }
            else
            {
                isOpControlDisabled = false;
            }

            Task.Run(() => loadData());

        }

        



        #region add tree view items


        private async void loadData()
        {
            try
            {
                // Show the progress bar
                Dispatcher.Invoke(() => progressLogin.Visibility = Visibility.Visible);

                using (WebClient web = new WebClient())
                {
                    string url = string.Format("http://103.234.126.43:3080/dtmeters/district");
                    string json = await web.DownloadStringTaskAsync(url);

                    List<District> districts = JsonConvert.DeserializeObject<List<District>>(json);

                    // Update UI on the UI thread
                    Dispatcher.Invoke(() =>
                    {
                        foreach (District district in districts)
                        {
                            TreeViewItem districtItem = new TreeViewItem();
                            districtItem.Header = district.district_name;
                            districtItem.Tag = district.district_code;
                            url = string.Format("http://103.234.126.43:3080/dtmeter/district/{0}", district.district_code);
                            json = web.DownloadString(url);
                            List<ESD> esds = JsonConvert.DeserializeObject<List<ESD>>(json);

                            foreach (ESD esd in esds)
                            {
                                TreeViewItem esdItem = new TreeViewItem();
                                esdItem.Header = esd.esd_name;
                                esdItem.Tag = esd.esd_code;
                                districtItem.Items.Add(esdItem);
                            }

                            myTreeView.Items.Add(districtItem);
                        }
                    });
                }
            }
            catch
            {
                // Update UI on the UI thread
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                // Hide the progress bar
                Dispatcher.Invoke(() => progressLogin.Visibility = Visibility.Collapsed);
            }
        }


        private async  void myTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(cbDtInfo.IsChecked == false || cbDtMeterInfo.IsChecked == false || cbMriInfo.IsChecked == false)
            {
                cbDtInfo.IsChecked = true ;
            }

            if (e.NewValue == null || !(e.NewValue is TreeViewItem selectedTreeViewItem))
            {
                return;
            }

            if (selectedTreeViewItem.Parent is TreeViewItem selectedDistrictItem && selectedDistrictItem.Tag != null && selectedTreeViewItem.Tag != null)
            {
                datagridEsdFormat.Visibility = Visibility.Collapsed;


                selectedEsd = (string)selectedTreeViewItem.Tag;

                url = string.Format("http://103.234.126.43:3080/dtmeter/district/{0}/{1}", selectedDistrictItem.Tag, selectedTreeViewItem.Tag);

                if(cbDtInfo.IsChecked==true || cbDtMeterInfo .IsChecked == true || cbMriInfo .IsChecked == true )
                {

                    await ReadDatabase();
                }

                

                else
                {
                    MessageBox.Show("Select a View Type", "View Selection", MessageBoxButton.OK, MessageBoxImage.Information);

                }




                myTreeView.PreviewMouseRightButtonDown += MyTreeView_PreviewMouseRightButtonDownAddDt;
            }

            else if (e.NewValue is TreeViewItem districtItem)
            {
                datagridEsdFormat.Visibility = Visibility.Collapsed;

              //  url = string.Format("http://103.234.126.43:3080/dtmeter/dtinfo/all/{0}", districtItem.Tag);
                url = string.Format("http://103.234.126.43:3080/dtmeter/dtinfo/all/{0}", districtItem.Tag);


                if (cbDtInfo.IsChecked == true || cbDtMeterInfo.IsChecked == true || cbMriInfo.IsChecked == true  )
                {
 
                    await ReadDatabase();
                }

                else
                {
                    MessageBox.Show("Select a View Type", "View Selection", MessageBoxButton.OK, MessageBoxImage.Information);

                }

                myTreeView.PreviewMouseRightButtonDown += MyTreeView_PreviewMouseRightButtonDownAddESD;
            }


            //Task.Run(() => UpdateNotifyBadgeContent());

        }

        private async Task ReadDatabase()
        {
            datagridDT.ClearFilters();
          

            //datagridDT.ItemsSource = null;

            try
            {
                // Show progress bar
                Dispatcher.Invoke(() => progressLogin.Visibility = Visibility.Visible);

                using (WebClient web = new WebClient())
                {
                    string json = await web.DownloadStringTaskAsync(url);



                    if (cbDtInfo.IsChecked == true)
                    {
                        List<DT> dtMeters = JsonConvert.DeserializeObject<List<DT>>(json);

                        datagridDT.Visibility = Visibility.Visible;
                        datagridDtMeter.Visibility = Visibility.Collapsed;
                        datagridMri.Visibility = Visibility.Collapsed;

                        datagridDT.ItemsSource = dtMeters;

                    }

                    else if (cbDtMeterInfo.IsChecked == true)
                    {
                        List<DtMeter> dtMeters = JsonConvert.DeserializeObject<List<DtMeter>>(json);

                        datagridDT.Visibility = Visibility.Collapsed;
                        datagridDtMeter.Visibility = Visibility.Visible;
                        datagridMri.Visibility = Visibility.Collapsed;

                        datagridDtMeter.ItemsSource = dtMeters;

                    }


                    else if (cbMriInfo.IsChecked == true)
                    {
                        List<Mri> dtMeters = JsonConvert.DeserializeObject<List<Mri>>(json);

                        datagridDT.Visibility = Visibility.Collapsed;
                        datagridDtMeter.Visibility = Visibility.Collapsed;
                        datagridMri.Visibility = Visibility.Visible;

                        datagridMri.ItemsSource = dtMeters;
                    }


                    else
                    {
                        datagridDT.Visibility = Visibility.Collapsed;
                        datagridDtMeter.Visibility = Visibility.Collapsed;
                        datagridMri.Visibility = Visibility.Collapsed;

                        MessageBox.Show("Select a View Type", "View Selection", MessageBoxButton.OK, MessageBoxImage.Information);

                    }


                }
            }
            catch
            {
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);             
            }
            finally
            {
                // Hide progress bar
                Dispatcher.Invoke(() => progressLogin.Visibility = Visibility.Collapsed);

                await ReadEvent();
            }
        }


        #endregion add tree view items

        private async void cbDtInfo_Checked(object sender, RoutedEventArgs e)
        {
            

            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Select a District or ESD", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                cbDtMeterInfo.IsChecked = false;
                cbMriInfo.IsChecked = false;

                await ReadDatabase();
            }
        }

        private async  void cbDtMeterInfo_Checked(object sender, RoutedEventArgs e)
        {
           
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Select a District or ESD", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                cbDtInfo.IsChecked = false;
                cbMriInfo.IsChecked = false;


                await ReadDatabase();
            }
        }

        private async  void cbMriInfo_Checked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Select a District or ESD", "Input Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                cbDtInfo.IsChecked = false;
                cbDtMeterInfo.IsChecked = false;

                await ReadDatabase();
            }

        }

       
        #region menu item
        private void MyTreeView_PreviewMouseRightButtonDownAddDt(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                ContextMenu contextMenuDt = new ContextMenu();

                contextMenuDt.Background = (Brush)new BrushConverter().ConvertFrom("#202020");

                Style menuItemStyle = new Style(typeof(MenuItem));
                menuItemStyle.Setters.Add(new Setter(ForegroundProperty, Brushes.White));
                menuItemStyle.Setters.Add(new Setter(FontSizeProperty, 12.0));
                menuItemStyle.Setters.Add(new Setter(MarginProperty, new Thickness(20, 5, 20, 5)));

                MenuItem menExportXls = new MenuItem { Header = "Export as .xls", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.MicrosoftExcel, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
                menExportXls.Click += MenExportXls_Click;
                contextMenuDt.Items.Add(menExportXls);

                //MenuItem menExportPdf = new MenuItem { Header = "Export as .pdf", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.FilePdfBox, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
                //menExportPdf.Click += menExportPdf_Click;
                //contextMenuDt.Items.Add(menExportPdf);

                MenuItem menExportCsv = new MenuItem { Header = "Export as .csv", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.FileCsv, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
                menExportCsv.Click += menExportCsv_Click;
                contextMenuDt.Items.Add(menExportCsv);

                MenuItem menAddDt = new MenuItem { Header = "Add DT", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.Plus, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
                menAddDt.Click += MenAddDt_Click;
                contextMenuDt.Items.Add(menAddDt);

                //MenuItem menAddDtFromExcel = new MenuItem { Header = "Add DT from .xls", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.MicrosoftExcel, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
                //menAddDtFromExcel.Click += menAddDtFromExcel_Click;
                //contextMenuDt.Items.Add(menAddDtFromExcel);

                MenuItem menDeleteEsd = new MenuItem { Header = "Delete ESD", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.Delete, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
                menDeleteEsd.Click += menDeleteEsd_Click;
                contextMenuDt.Items.Add(menDeleteEsd);

                if (isOpControlDisabled)
                {
                    menAddDt.IsEnabled = false;
                    //menAddDtFromExcel.IsEnabled = false;
                    menDeleteEsd.IsEnabled = false;
                }


                treeViewItem.ContextMenu = contextMenuDt;
                treeViewItem.Focus();

            }
        }

        private void MyTreeView_PreviewMouseRightButtonDownAddESD(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                ContextMenu contextMenuEsd = new ContextMenu();

                // Set the background color of the context menu
                contextMenuEsd.Background = (Brush)new BrushConverter().ConvertFrom("#202020");

                // Create a reusable style for the menu items
                Style menuItemStyle = new Style(typeof(MenuItem));
                menuItemStyle.Setters.Add(new Setter(ForegroundProperty, Brushes.White));
                menuItemStyle.Setters.Add(new Setter(FontSizeProperty, 12.0));
                menuItemStyle.Setters.Add(new Setter(MarginProperty, new Thickness(20, 5, 20, 5)));

                // Export menu items
                MenuItem menExportXls = new MenuItem { Header = "Export as .xls", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.MicrosoftExcel, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
                menExportXls.Click += MenExportXls_Click;
                contextMenuEsd.Items.Add(menExportXls);

                MenuItem menExportPdf = new MenuItem { Header = "Export as .pdf", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.FilePdfBox, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
                menExportPdf.Click += menExportPdf_Click;
                contextMenuEsd.Items.Add(menExportPdf);

                MenuItem menExportCsv = new MenuItem { Header = "Export as .csv", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.FileCsv, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
                menExportCsv.Click += menExportCsv_Click;
                contextMenuEsd.Items.Add(menExportCsv);

                MenuItem menAddEsd = new MenuItem { Header = "Add ESD", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.Plus, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
                menAddEsd.Click += MenAddEsd_Click;
                contextMenuEsd.Items.Add(menAddEsd);


                if (isOpControlDisabled)
                {
                    menAddEsd.IsEnabled = false;
                }

                treeViewItem.ContextMenu = contextMenuEsd;
                treeViewItem.Focus();


            }
        }

        private TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            return source as TreeViewItem;
        }

        private void MenExportXls_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;

            if (cbDtInfo.IsChecked == true)
            {
                options.ExcludeColumns.Add("View Meter");
                options.ExcludeColumns.Add("View MRI");
                options.ExcludeColumns.Add("Edit DT");
                options.ExcludeColumns.Add("Delete DT");


                var excelEngine = datagridDT.ExportToExcel(datagridDT.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];

                SaveFileDialog sfd = new SaveFileDialog
                {
                    FilterIndex = 2,
                    Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
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


                    if (MessageBox.Show("Do you want to view the workbook?", "Workbook has been created",
                                        MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {

                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }


            else if (cbDtMeterInfo.IsChecked == true)
            {

                options.ExcludeColumns.Add("Edit Meter");
                options.ExcludeColumns.Add("Delete Meter");


                var excelEngine = datagridDtMeter.ExportToExcel(datagridDT.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];

                SaveFileDialog sfd = new SaveFileDialog
                {
                    FilterIndex = 2,
                    Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
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


                    if (MessageBox.Show("Do you want to view the workbook?", "Workbook has been created",
                                        MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {

                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }


            else if (cbMriInfo.IsChecked == true)
            {

                options.ExcludeColumns.Add("Edit Mri");
                options.ExcludeColumns.Add("Delete Mri");


                var excelEngine = datagridMri.ExportToExcel(datagridDT.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];

                SaveFileDialog sfd = new SaveFileDialog
                {
                    FilterIndex = 2,
                    Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
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


                    if (MessageBox.Show("Do you want to view the workbook?", "Workbook has been created",
                                        MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {

                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }

            else
            {
                MessageBox.Show("Choose View Type");
            }

        }

        private void menExportPdf_Click(object sender, RoutedEventArgs e)
        {
            var document = datagridDT.ExportToPdf();
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Files(*.pdf)|*.pdf"
            };

            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = sfd.OpenFile())
                {
                    document.Save(stream);
                }

                //Message box confirmation to view the created Pdf file.

                if (MessageBox.Show("Do you want to view the Pdf file?", "Pdf file has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {

                    //Launching the Pdf file using the default Application.
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }

        private void menExportCsv_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;

            // Export the datagrid to Excel
            var excelEngine = datagridDT.ExportToExcel(datagridDT.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            // Save the workbook as CSV using SaveFileDialog
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV Files(*.csv)|*.csv"
            };

            if (sfd.ShowDialog() == true)
            {
                string filePath = sfd.FileName;

                // If file extension is not .csv, append it to the filename
                if (System.IO.Path.GetExtension(filePath) != ".csv")
                {
                    filePath = System.IO.Path.ChangeExtension(filePath, ".csv");
                }

                // Save the workbook as CSV file
                workBook.SaveAs(filePath, ",");

                // Message box confirmation to open the created CSV file
                if (MessageBox.Show("Do you want to open the CSV file?", "CSV file has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    // Launch the CSV file using the default Application.
                    System.Diagnostics.Process.Start(filePath);
                }
            }
        }

        private  void MenAddEsd_Click(object sender, RoutedEventArgs e)
        {

            if (myTreeView.SelectedItem is TreeViewItem selectedTreeViewItem && selectedTreeViewItem.Tag != null)
            {
                string dtCode = selectedTreeViewItem.Tag.ToString();

                AddEsdWindow addEsdWindow = new AddEsdWindow(dtCode, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);
                addEsdWindow.ShowDialog();

            }

        }

        private async  void MenAddDt_Click(object sender, RoutedEventArgs e)
        {
            if (myTreeView.SelectedItem is TreeViewItem selectedTreeViewItem && selectedTreeViewItem.Tag != null)
            {

                string esdCode = selectedTreeViewItem.Tag.ToString();

                AddDtWindow addDtWindow = new AddDtWindow(esdCode, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);
                addDtWindow.ShowDialog();

            }

            await ReadDatabase();


        }

        private ObservableCollection<DT> dTsEsdFormat = new ObservableCollection<DT>();

        private async void menAddDtFromExcel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Did you download the New ESD Format?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (myTreeView.SelectedItem is TreeViewItem selectedTreeViewItem && selectedTreeViewItem.Tag != null)
                {

                    string esdCode = selectedTreeViewItem.Tag.ToString();


                    AddDtFromExcelWindow addDtFromExcelWindow = new AddDtFromExcelWindow(esdCode, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);
                    addDtFromExcelWindow.ShowDialog();

                }
            }

            else
            {

                datagridDT.Visibility = Visibility.Collapsed;
                datagridDtMeter.Visibility = Visibility.Collapsed;
                datagridMri.Visibility = Visibility.Collapsed;
                datagridEsdFormat.Visibility = Visibility.Visible;


                dTsEsdFormat.Add(new DT { feeder_name = "111 ", feeder_id = "111 ", portion_id = "111", root_id = "111", dt_name = "111", dt_id = "Dt3528111", transformer_serial_number = "111", dt_capacity_kva = "111", rated_voltage_primary = "111", rated_voltage_secondary = "111", ct_primary = "111", ct_secondary = "111", vt_primary = "111", vt_secondary = "111", r_phase = "111", y_phase = "111", b_phase = "111", total_customer_count = "111", location = "111", latitude = "111", longitude = "111", elevation = "111" });
                dTsEsdFormat.Add(new DT { feeder_name = "222 ", feeder_id = "222 ", portion_id = "222", root_id = "222", dt_name = "222", dt_id = "Dtvfe222", transformer_serial_number = "222", dt_capacity_kva = "222", rated_voltage_primary = "222", rated_voltage_secondary = "222", ct_primary = "222", ct_secondary = "222", vt_primary = "222", vt_secondary = "222", r_phase = "222", y_phase = "222", b_phase = "222", total_customer_count = "222", location = "222", latitude = "222", longitude = "222", elevation = "222" });


                datagridEsdFormat.ItemsSource = dTsEsdFormat;

                await Task.Delay(100);
                var options = new ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = datagridEsdFormat.ExportToExcel(datagridEsdFormat.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];

                SaveFileDialog sfd = new SaveFileDialog
                {
                    FilterIndex = 2,
                    Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx",
                    FileName = "New DT Format.xlsx" // Set a default file name
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

                    if (MessageBox.Show("Do you want to view the workbook?", "Workbook has been created",
                                        MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
        }

        private async void menDeleteEsd_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (myTreeView.SelectedItem is TreeViewItem selectedTreeViewItem && selectedTreeViewItem.Tag != null)
                {

                    string esdCode = selectedTreeViewItem.Tag.ToString();

                    try
                    {
                        DtMeter dtMeter = new DtMeter()
                        {

                        };

                        string url = string.Format("http://103.234.126.43:3080/dtmeter/district/post/esd/{0}", esdCode);
                        HttpClient client = new HttpClient();
                        string jsonData = JsonConvert.SerializeObject(dtMeter);
                        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.DeleteAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            //string responseMessage = await response.Content.ReadAsStringAsync();
                            //MessageBox.Show(responseMessage);

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

            }

            else
            {

            }

            await ReadDatabase();

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
                    UserID = CurrentUserId1,
                    EmployeeId = CurrentUserEmployeeId1,
                    UserName = CurrentUserName1,

                    EventLogs = "Deleted ESD",

                    Date = UserEventDate,
                    Time = UserEventTime,

                    StatusAdmin1 = "Not Acknowledged",
                    StatusAdmin2 = "Not Acknowledged",
                    StatusAdmin3 = "Not Acknowledged",
                    Remarks = "Deleted ESD"
                    //Remarks = "Deleted ESD for : " + selectedDistrictrictCode,
                };

                string url = "http://103.234.126.43:3500/dtmeter/logs/events/userevents";
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(userEventPost);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("ESD successfully deleted.", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);


                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


            catch 
            {
                MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion menu item

        #region datagridDT  buttons

        private async  void btnViewMeter_Click(object sender, RoutedEventArgs e)
        {

            DT selectedDt = datagridDT.SelectedItem as DT;

            if (selectedDt != null)
            {
                ViewMeterWindow viewMeterWindow = new ViewMeterWindow(selectedDt, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);

                viewMeterWindow.ShowDialog();
            }

            await ReadDatabase();
        }

        private async void btnViewMri_Click(object sender, RoutedEventArgs e)
        {
            DT selectedDt = datagridDT.SelectedItem as DT;

            if (selectedDt != null)
            {
                ViewMriWindow viewMriWindow = new ViewMriWindow(selectedDt, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);

                viewMriWindow.ShowDialog();

            }
            await ReadDatabase();
        }

        private async void btnDeleteDt_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUserRole1.Equals("Operator"))
            {
                MessageBox.Show("Only Administrator can Delete DT Information", "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            else
            {
                DT selectedDt = datagridDT.SelectedItem as DT;

                if (selectedDt != null)
                {
                    DeleteDtWindow deleteDtWindow = new DeleteDtWindow(selectedDt, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);


                    deleteDtWindow.ShowDialog();


                }
            }

            await ReadDatabase();
        }

        private async void btnEditDt_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUserRole1.Equals("Operator"))
            {
                MessageBox.Show("Only Administrator can Edit DT Information", "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            else
            {
                DT selectedDt = datagridDT.SelectedItem as DT;


                if (selectedDt != null)
                {
                    UpdateDtWindow updateDtWindow = new UpdateDtWindow(selectedDt, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);

                    updateDtWindow.ShowDialog();
                }
            }

            await ReadDatabase();
        }

        #endregion datagridDT buttons

        #region datagridDtMeter buttons

        private async  void btnMeterEdit_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUserRole1.Equals("Operator"))
            {
                MessageBox.Show("Only Administrator can Edit Meter Information", "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Warning);

            }

            else
            {

                if (datagridDtMeter.SelectedItem is DtMeter selectedDtMeter)
                {
                    UpdateMeterWindow updateMeterWindow = new UpdateMeterWindow(selectedDtMeter, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);

                    updateMeterWindow.ShowDialog();

                }

            }

            await ReadDatabase();
        }

        private async  void btnMeterDelete_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentUserRole1.Equals("Operator"))
            {
                MessageBox.Show("Only Administrator can Delete Meter Information", "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Warning);

            }

            else
            {

                if (datagridDtMeter.SelectedItem is DtMeter selectedDtMeter)
                {
                    DeleteMeterWindow deleteMeterWindow = new DeleteMeterWindow(selectedDtMeter, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);


                    _ = deleteMeterWindow.ShowDialog();

                }
            }

            await ReadDatabase();
        }

        #endregion  datagridDtMeter buttons

        #region datagridMri buttons

        private async void btnMriEdit_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentUserRole1.Equals("Operator"))
            {
                MessageBox.Show("Only Administrator can Edit MRI Information", "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Warning);

            }

            else
            {

                if (datagridMri.SelectedItem is Mri selectedDtMri)
                {
                    UpdateMriWindow updateMriWindow = new UpdateMriWindow(selectedDtMri, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);


                    updateMriWindow.ShowDialog();

                }
            }
            await ReadDatabase();
        }

        private async  void btnMriDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUserRole1.Equals("Operator"))
            {
                MessageBox.Show("Only Administrator can Delete MRI Information", "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Warning);

            }


            else
            {

                if (datagridMri.SelectedItem is Mri selectedDtMri)
                {
                    DeleteMriWindow deleteMriWindow = new DeleteMriWindow(selectedDtMri, CurrentUserId1, CurrentUserRole1, CurrentUserPassword1, CurrentUserName1, CurrentUserEmployeeId1);


                    _ = deleteMriWindow.ShowDialog();

                }
            }

            await ReadDatabase();
        }

        #endregion datagridMri buttons

        #region Event Logs


       

      


        #endregion Event Logs

        private void datagridDT_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            {
                var visualContainer = datagridDT.GetVisualContainer();
                var rowColumnIndex = visualContainer.PointToCellRowColumnIndex(e.GetPosition(visualContainer));


                if (rowColumnIndex != null)
                {
                    if (rowColumnIndex.RowIndex ==0)
                    {
                        MessageBox.Show("Header clicked! Please click on the data within the DataGrid below, not on the header. The header is used for sorting and clicking it won't trigger the expected action.", "User Guidance", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    else  
                    {
                        var dataObject = datagridDT.View.Records[rowColumnIndex.RowIndex - 1].Data;

                        DTId1 = (string)datagridDT.View.GetPropertyAccessProvider().GetValue(dataObject, "dt_id");

                        



                        ContextMenu contextMenu = new ContextMenu();
                        contextMenu.Background = (Brush)new BrushConverter().ConvertFrom("#202020");


                        MenuItem itemTopic = new MenuItem();
                        itemTopic.Header = "DT ID: " + DTId1;
                        itemTopic.Foreground = Brushes.White;
                        itemTopic.FontSize = 12.0;
                        itemTopic.FontWeight = FontWeights.Bold;
                        itemTopic.HorizontalContentAlignment = HorizontalAlignment.Center;

                        itemTopic.IsEnabled = false;
                        contextMenu.Items.Add(itemTopic);

                        MenuItem itemLine = new MenuItem();
                        itemLine.Foreground = Brushes.White;
                        itemLine.Margin = new Thickness(20, -5, 20, 5);
                        itemLine.IsEnabled = false;
                        itemLine.Height = 1;
                        itemLine.Background = Brushes.Red;
                        contextMenu.Items.Add(itemLine);


                        MenuItem itemInstantaneous = new MenuItem();
                        itemInstantaneous.Header = "Meter Data";
                        itemInstantaneous.Foreground = Brushes.White;
                        itemInstantaneous.FontSize = 12.0;
                        itemInstantaneous.Margin = new Thickness(20, 5, 20, 5);

                        itemInstantaneous.Icon = new PackIcon
                        {
                            Kind = PackIconKind.Table,
                            Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3")
                        };


                        itemInstantaneous.Click += (s, ea) =>
                        {
                            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

                            // Use the existing ViewModel instance from MainWindow
                            ViewModel1 viewModel = (ViewModel1)mainWindow.DataContext;

                            // Update properties
                            viewModel.VmDtId = DTId1;
                            viewModel.VmUserRole = CurrentUserRole1;


                            // Raise property changed events
                            viewModel.OnMeterNoPropertyChanged(nameof(ViewModel1.VmDtId));
                            viewModel.OnMeterNoPropertyChanged(nameof(ViewModel1.VmUserRole));

                            // Navigate to MeterData
                            mainWindow.NavigateToMeterData();
                        };

                        contextMenu.Items.Add(itemInstantaneous);


                        

                        datagridDT.ContextMenu = contextMenu;
                    }
                }
            }
        }


        #region Grid Resize
        private bool isResizingBottom = false;

        private void BottomRegion_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizingBottom)
            {
                var rect = (Rectangle)sender;
                var grid = FindParent<Grid>(rect, "grdEvents");

                if (grid != null)
                {
                    double newHeight = grid.ActualHeight - e.GetPosition(grid).Y;

                    if (newHeight > 0)
                    {
                        grid.Height = newHeight;
                        rect.Margin = new Thickness(0, -e.GetPosition(grid).Y, 0, 0);
                    }
                }
            }
        }

        private void BottomRegion_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isResizingBottom = true;
            Mouse.Capture((IInputElement)sender);
            Mouse.OverrideCursor = Cursors.SizeNS; // Set the cursor to SizeNS when moving up and down
        }

        private void BottomRegion_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isResizingBottom = false;
            Mouse.Capture(null);
            Mouse.OverrideCursor = null; // Reset the cursor
        }

        private void BottomRegion_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeWE; // Set the cursor to SizeWE when hovering over recGridResize
        }

        private void BottomRegion_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!isResizingBottom)
            {
                Mouse.OverrideCursor = null; // Reset the cursor when not resizing
            }
        }

        // Helper method to find parent element by name
        private T FindParent<T>(DependencyObject child, string parentName) where T : DependencyObject
        {
            DependencyObject current = child;
            while (current != null && current.GetType() != typeof(T))
            {
                var frameworkElement = current as FrameworkElement;
                if (frameworkElement != null && frameworkElement.Name == parentName)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            return (T)current;
        }

        private void recGridResize_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeNS;
        }

        private void recGridResize_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null; // Reset the cursor when not resizing

        }


        #endregion Grid Resize

        #region Event
        private async Task ReadEvent()
        {



            await Dispatcher.InvokeAsync(() =>
            {
                // Show the progress bar
                progressLogin.Visibility = Visibility.Visible;
            });

            try
            {
                using (WebClient web = new WebClient())
                {
                    string url = "http://103.234.126.43:3080/dtmeter/logs/events/userevents";
                    string json = await web.DownloadStringTaskAsync(url);

                    List<UserEvent> allData = JsonConvert.DeserializeObject<List<UserEvent>>(json);

                    dtAddEvent = allData.Where(a => a.event_log == "Added DT").ToList();
                    dtEditEvent = allData.Where(a => a.event_log == "Edited DT").ToList();
                    dtDeleteEvent = allData.Where(a => a.event_log == "Deleted DT").ToList();
                    dtMeterEditEvent = allData.Where(a => a.event_log == "Edited Meter").ToList();

                    dtMeterDeleteEvent = allData.Where(a => a.event_log == "Deleted Meter").ToList();
                    dtMriEditEvent = allData.Where(a => a.event_log == "Edited Mri").ToList();
                    dtMriDeleteEvent = allData.Where(a => a.event_log == "Deleted Mri").ToList();
                    EsdAddEvent = allData.Where(a => a.event_log == "Added ESD").ToList();
                    EsdDeleteEvent = allData.Where(a => a.event_log == "Deleted ESD").ToList();
              
                    await Dispatcher.InvokeAsync(() =>
                    {
                        lblDtAdd.Content = dtAddEvent.Count;
                        lblDtEdit.Content = dtEditEvent.Count;
                        lblDtDelete.Content = dtDeleteEvent.Count;

                        lblDtEditMeter.Content = dtMeterEditEvent.Count;
                        lblDeleteMeter.Content = dtMeterDeleteEvent.Count;
                        lblDtEditMri.Content = dtMriEditEvent.Count;

                        lblDtDeleteMri.Content = dtMriDeleteEvent.Count;
                        lblEsdAdd.Content = EsdAddEvent.Count;
                        lblEsdDelete.Content = EsdDeleteEvent.Count;

                        //int totalUserEvents = dtAddEvent.Count + dtEditEvent.Count + dtDeleteEvent.Count + dtMeterEditEvent.Count + dtMeterDeleteEvent.Count + dtMriEditEvent.Count + dtMriDeleteEvent.Count + EsdAddEvent.Count + EsdDeleteEvent.Count;

                    });
                }
            }
            catch
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    // Hide the progress bar
                    progressLogin.Visibility = Visibility.Collapsed;
                });
            }
        }

        private void btnDtAdd_Click(object sender, RoutedEventArgs e)
        {
            datagridUserEvent.ItemsSource = dtAddEvent;
            ResetButtonBackground();
            Button clickedButton = (Button)sender;
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a5e3"));
        }

        private void btnDtEdit_Click(object sender, RoutedEventArgs e)
        {
            datagridUserEvent.ItemsSource = dtEditEvent;
            ResetButtonBackground();
            Button clickedButton = (Button)sender;
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a5e3"));
        }

        private void btnDtDelete_Click(object sender, RoutedEventArgs e)
        {
            datagridUserEvent.ItemsSource = dtDeleteEvent;
            ResetButtonBackground();
            Button clickedButton = (Button)sender;
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a5e3"));
        }

        private void btnDtEditMeter_Click(object sender, RoutedEventArgs e)
        {
            datagridUserEvent.ItemsSource = dtMeterEditEvent;
            ResetButtonBackground();
            Button clickedButton = (Button)sender;
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a5e3"));
        }

        private void btnDtMeterDelete_Click(object sender, RoutedEventArgs e)
        {
            datagridUserEvent.ItemsSource = dtMeterDeleteEvent;
            ResetButtonBackground();
            Button clickedButton = (Button)sender;
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a5e3"));
        }

        private void btnDtEditMri_Click(object sender, RoutedEventArgs e)
        {
            datagridUserEvent.ItemsSource = dtMriEditEvent;
            ResetButtonBackground();
            Button clickedButton = (Button)sender;
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a5e3"));
        }

        private void btnDtDeleteMri_Click(object sender, RoutedEventArgs e)
        {
            datagridUserEvent.ItemsSource = dtMriDeleteEvent;
            ResetButtonBackground();
            Button clickedButton = (Button)sender;
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a5e3"));
        }

        private void btnEsdAdd_Click(object sender, RoutedEventArgs e)
        {
            datagridUserEvent.ItemsSource = EsdAddEvent;
            ResetButtonBackground();
            Button clickedButton = (Button)sender;
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a5e3"));
        }

        private void btnEsdDelete_Click(object sender, RoutedEventArgs e)
        {
            datagridUserEvent.ItemsSource = EsdDeleteEvent;
            ResetButtonBackground();
            Button clickedButton = (Button)sender;
            clickedButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a5e3"));
        }

        private void ResetButtonBackground()
        {
            btnDtAdd.Background = btnDtEdit.Background = btnDtDelete.Background = btnDtEditMeter.Background = btnDtMeterDelete.Background = btnDtEditMri.Background = btnDtDeleteMri.Background = btnEsdAdd.Background = btnEsdDelete.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2d2d30"));
        }

        #endregion Event



      
    }
}
