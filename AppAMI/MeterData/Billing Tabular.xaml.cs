using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AppAMI.Classes;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;

namespace AppAMI.MeterData
{
    /// <summary>
    /// Interaction logic for Billing_Tabular.xaml
    /// </summary>
    public partial class Billing_Tabular : UserControl
    {
        public Billing_Tabular()
        {
            InitializeComponent();
            dtPickerStart.DisplayDateEnd = DateTime.Today;
            dtPickerEnd.DisplayDateEnd = DateTime.Today;

            CheckDtId();
        }

        private async void CheckDtId()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            if (string.IsNullOrEmpty(txtDtId.Text))
            {
                MessageBox.Show("Enter DT Id");


            }
            else
            {


            }

        }


        #region Read Database

        private void dtPickerStart_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy"; //for the second type
            Thread.CurrentThread.CurrentCulture = ci;
        }

        private void dtPickerEnd_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy"; //for the second type
            Thread.CurrentThread.CurrentCulture = ci;


        }

        void ReadDatabase()
        {

            if (string.IsNullOrEmpty(txtDtId.Text))
            {
                MessageBox.Show("Enter DT Id");


            }
            else
            {
                try
                {
                    using (WebClient web = new WebClient())
                    {
                        string url = string.Format("http://119.2.119.202:3500/dtmeter/instantenous/data");
                        string json = web.DownloadString(url);

                        List<LoadSurveyTab> all_data = JsonConvert.DeserializeObject<List<LoadSurveyTab>>(json);

                        List<LoadSurveyTab> filteredDtId = all_data.Where(a => a.dt_id == txtDtId.Text).ToList();

                        // Filter data based on date range
                        List<LoadSurveyTab> filtered_data = filteredDtId
                            .Where(x => DateTime.ParseExact(x.date, "dd-MM-yyyy", CultureInfo.InvariantCulture) >= dtPickerStart.SelectedDate.Value
                                && DateTime.ParseExact(x.date, "dd-MM-yyyy", CultureInfo.InvariantCulture) <= dtPickerEnd.SelectedDate.Value)
                            .ToList();

                        // Bind filtered data to DataGrid
                        loadSurveyDgv.ItemsSource = filtered_data;


                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Unable to Connect to the Server" + ex.Message);

                }

            }



        }



        #endregion  Read Database


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

            MenuItem menExportPdf = new MenuItem { Header = "Export as .pdf", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.FilePdfBox, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
            menExportPdf.Click += menExportPdf_Click;
            contextMenu.Items.Add(menExportPdf);

            MenuItem menExportCsv = new MenuItem { Header = "Export as .csv", Style = menuItemStyle, Icon = new PackIcon { Kind = PackIconKind.FileCsv, Foreground = (Brush)new BrushConverter().ConvertFrom("#00a5e3") } };
            menExportCsv.Click += menExportCsv_Click;
            contextMenu.Items.Add(menExportCsv);

            btnExport.ContextMenu = contextMenu;
            btnExport.Focus();
        }

        private void menExportCsv_Click(object sender, RoutedEventArgs e)
        {
        }

        private void menExportPdf_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MenExportXls_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = loadSurveyDgv.ExportToExcel(loadSurveyDgv.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                FilterIndex = 2,
                Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx",
                FileName = "Load Survey_" + txtDtId.Text + "_From_" + dtPickerStart.Text + "To_" + dtPickerEnd.Text + ".xlsx"

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

        #endregion Export

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Grid).Children.Remove(this);

        }
    }
}
