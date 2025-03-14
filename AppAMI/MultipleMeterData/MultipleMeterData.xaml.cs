using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using AppAMI.Classes;

namespace AppAMI.MultipleMeterData
{
    /// <summary>
    /// Interaction logic for MultipleMeterData.xaml
    /// </summary>
    public partial class MultipleMeterData : UserControl
    {

        string InstantCurrentIR;
        string InstantCurrentIY;
        string InstantCurrentIB;

        string InstantVoltageVRY;

        public MultipleMeterData()
        {
            InitializeComponent();

            ReadDatabase();

            dgvInstantPara.Visibility = Visibility.Collapsed;
            dgvBilling.Visibility = Visibility.Collapsed;
            dgvLoadSurvey.Visibility = Visibility.Collapsed;

            dgvInstantParaSum.Visibility = Visibility.Collapsed;


            itemlistsInstantPara = new ObservableCollection<MmDataSum>();
        }


        void ReadDatabase()
        {
            using (WebClient web = new WebClient())
            {
                string url = string.Format("http://119.2.119.202:3500/dtmeter/meterconfig/meterinfo/all");
                string json = web.DownloadString(url);

                List<MmData> all_data = JsonConvert.DeserializeObject<List<MmData>>(json);
                meterdgv.ItemsSource = all_data;

            }
        }

        #region CheckBoxes

        private void cbInstantPara_Checked(object sender, RoutedEventArgs e)
        {



            cbBilling.IsChecked = false;
            cbLoadSurvey.IsChecked = false;

            dgvInstantPara.Visibility = Visibility.Visible;
            dgvBilling.Visibility = Visibility.Collapsed;
            dgvLoadSurvey.Visibility = Visibility.Collapsed;



            lblSelectedMePara.Content = "Instantaneous Parameter";

            dgvInstantParaSum.Visibility = Visibility.Visible;
        }

        private void cbInstantPara_Unchecked(object sender, RoutedEventArgs e)
        {
            dgvInstantPara.Visibility = Visibility.Collapsed;
        }

        private void cbBilling_Checked(object sender, RoutedEventArgs e)
        {
            cbInstantPara.IsChecked = false;
            cbLoadSurvey.IsChecked = false;

            dgvBilling.Visibility = Visibility.Visible;
            dgvInstantPara.Visibility = Visibility.Collapsed;
            dgvLoadSurvey.Visibility = Visibility.Collapsed;

            lblSelectedMePara.Content = "Billing";

            dgvInstantParaSum.Visibility = Visibility.Collapsed;
        }

        private void cbBilling_Unchecked(object sender, RoutedEventArgs e)
        {
            dgvBilling.Visibility = Visibility.Collapsed;
        }

        private void cbLoadSurvey_Checked(object sender, RoutedEventArgs e)
        {
            cbBilling.IsChecked = false;
            cbInstantPara.IsChecked = false;

            dgvLoadSurvey.Visibility = Visibility.Visible;
            dgvInstantPara.Visibility = Visibility.Collapsed;
            dgvBilling.Visibility = Visibility.Collapsed;


            lblSelectedMePara.Content = "Load Survey";

            dgvInstantParaSum.Visibility = Visibility.Collapsed;


        }

        private void cbLoadSurvey_Unchecked(object sender, RoutedEventArgs e)
        {
            dgvLoadSurvey.Visibility = Visibility.Collapsed;
        }



        #endregion CheckBoxes

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {


            for (int i = 0; i < meterdgv.SelectedItems.Count; i++)
            {
                foreach (var obj in meterdgv.SelectedItems)
                {
                    dgvInstantPara.ItemsSource = meterdgv.SelectedItems;
                    dgvBilling.ItemsSource = meterdgv.SelectedItems;
                    dgvLoadSurvey.ItemsSource = meterdgv.SelectedItems;

                }
            }

        }

        #region Filter Meters
        private void txtSearchLoc_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(meterdgv.ItemsSource);
            view.Filter = UserFilter;
            CollectionViewSource.GetDefaultView(meterdgv.ItemsSource).Refresh();
        }


        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txtSearchLoc.Text))
                return true;
            else
                return ((item as MmData).address.StartsWith(txtSearchLoc.Text.ToUpper()));
        }

        #endregion Filter Meters


        #region Parameter Sum
        private void dgvInstantPara_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            decimal sumIR = 0;
            decimal sumIY = 0;
            decimal sumIB = 0;

            decimal sumVRY = 0;

            for (int i = 0; i < dgvInstantPara.SelectedItems.Count; i++)
            {
                TextBlock tbIR = dgvInstantPara.Columns[3].GetCellContent(dgvInstantPara.SelectedItems[i]) as TextBlock;

                if (tbIR != null)
                {
                    sumIR += Convert.ToDecimal(tbIR.Text);
                }


                TextBlock tbIY = dgvInstantPara.Columns[4].GetCellContent(dgvInstantPara.SelectedItems[i]) as TextBlock;

                if (tbIY != null)
                {
                    sumIY += Convert.ToDecimal(tbIY.Text);
                }

                TextBlock tbIB = dgvInstantPara.Columns[4].GetCellContent(dgvInstantPara.SelectedItems[i]) as TextBlock;

                if (tbIB != null)
                {
                    sumIB += Convert.ToDecimal(tbIB.Text);
                }

                TextBlock tbVRY = dgvInstantPara.Columns[4].GetCellContent(dgvInstantPara.SelectedItems[i]) as TextBlock;

                if (tbVRY != null)
                {
                    sumVRY += Convert.ToDecimal(tbVRY.Text);
                }
            }

            InstantCurrentIR = sumIR.ToString();
            InstantCurrentIY = sumIY.ToString();
            InstantCurrentIB = sumIB.ToString();
            InstantVoltageVRY = sumVRY.ToString();

            GetSumDataGrid();

        }

        private ObservableCollection<MmDataSum> _itemlistsInstantPara;
        public ObservableCollection<MmDataSum> itemlistsInstantPara
        {
            get { return _itemlistsInstantPara; }
            set { _itemlistsInstantPara = value; }
        }

        private void GetSumDataGrid()
        {
            itemlistsInstantPara.Add(new MmDataSum()
            {
                vtratioSum = InstantCurrentIR,
                longitudeSum = InstantCurrentIY,
                latitudeSum = InstantCurrentIB,
                ctratioSum = InstantVoltageVRY,
            });

            dgvInstantParaSum.ItemsSource = itemlistsInstantPara;


            if (itemlistsInstantPara.Count > 1)
            {
                itemlistsInstantPara.RemoveAt(0);

            }


        }


        #endregion Parameter Sum
    }
}
