using AppAMI.Classes;
using Microsoft.Win32;
using Newtonsoft.Json;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AppAMI.MeterConfig
{
    /// <summary>
    /// Interaction logic for AddDtFromExcelWindow.xaml
    /// </summary>
    public partial class AddDtFromExcelWindow : Window
    {
        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;

        public AddDtFromExcelWindow(string esdCode, string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1)
        {


            InitializeComponent();
            txtEsdCode.Text = esdCode;

            CurrentUserId2 = CurrentUserId1;
            CurrentUserRole2 = CurrentUserRole1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;
        }

        private void btnImportFromExcel_Click(object sender, RoutedEventArgs e)
        {
            

            try
            {
                // Open file dialog to select Excel file
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                if (openFileDialog.ShowDialog() == true)
                {
                    // Load Excel file into a DataTable
                    ExcelEngine excelEngine = new ExcelEngine();
                    IWorkbook workbook = excelEngine.Excel.Workbooks.Open(openFileDialog.FileName);
                    IWorksheet worksheet = workbook.Worksheets[0];
                    DataTable dataTable = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);

                    // Insert esdCode into the first column with Header "ESD Code"
                    DataColumn esdCodeColumn = new DataColumn("ESD Code", typeof(string));
                    esdCodeColumn.DefaultValue = txtEsdCode.Text;
                    dataTable.Columns.Add(esdCodeColumn);

                    // Move "ESD Code" column to the first position in the DataTable
                    dataTable.Columns["ESD Code"].SetOrdinal(0);

                    // Bind DataTable to DataGrid
                    datagridNewEsd.ItemsSource = dataTable.AsDataView();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                // Create list of DT objects from DataGrid data
                List<DT> dtList = new List<DT>();
                foreach (var item in datagridNewEsd.View.Records)
                {
                    DataRowView row = item.Data as DataRowView;
                    DT dt = new DT();
                    dt.EsdCode = row["ESD Code"].ToString();

                    dt.FeederName = row["Feeder Name"].ToString();
                    dt.FeederId = row["Feeder ID"].ToString();
                    dt.PortionId = row["Portion ID"].ToString();
                    dt.RootId = row["Root ID"].ToString();

                    dt.DtName = row["DT Name"].ToString();
                    dt.DtId = row["DT ID"].ToString();
                    dt.TransformerSerialNo = row["Transformer Serial No."].ToString();

                    dt.DtCapacityKva = row["DT Capacity (kVA)"].ToString();
                    dt.RatedVoltagePrimary = row["Primary Rated Voltage"].ToString();
                    dt.RatedVoltageSecondary = row["Secondary Rated Voltage"].ToString();
                    dt.CTPrimary = row["CT Primary"].ToString();
                    dt.CTSecondary = row["CT Secondary"].ToString();
                    dt.VTPrimary = row["VT Primary"].ToString();
                    dt.VTSecondary = row["VT Secondary"].ToString();

                    dt.Rphase = row["R - Phase"].ToString();
                    dt.Yphase = row["Y - Phase"].ToString();
                    dt.Bphase = row["B - Phase"].ToString();
                    dt.TotalCustomerCount = row["Tot Consumer"].ToString();


                    dt.Latitude = row["Location"].ToString();
                    dt.Longitude = row["Latitude"].ToString();
                    dt.Elevation = row["Longitude"].ToString();
                    dt.Location = row["Elevation"].ToString();

                    dtList.Add(dt);
                }

                // Serialize DT list to JSON
                string json = JsonConvert.SerializeObject(dtList);

                // Send HTTP POST request with JSON payload
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.PostAsync("http://103.234.126.43:3080/dtmeter/district/post/DT", new StringContent(json, Encoding.UTF8, "application/json"));

                // Handle the response from the server
                if (response.IsSuccessStatusCode)
                {

                    //addNewUserEvent();
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Exception ex)
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

                    EventLogs = "Added DT",

                    Date = UserEventDate,
                    Time = UserEventTime,

                    StatusAdmin1 = "Not Acknowledged",
                    StatusAdmin2 = "Not Acknowledged",
                    StatusAdmin3 = "Not Acknowledged",

                    Remarks = "Added from Excel ",
                };

                string url = "http://103.234.126.43:3080/dtmeter/logs/events/userevents";
                HttpClient client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(userEventPost);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("DT successfully added.", "Success Notification", MessageBoxButton.OK, MessageBoxImage.Information);

                    Close();
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

       
    }
}
