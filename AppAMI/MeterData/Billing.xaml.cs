using System;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Net;
using System.Windows.Media;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using AppAMI.Classes;
using System.Threading.Tasks;

namespace AppAMI.MeterData
{
    /// <summary>
    /// Interaction logic for Billing.xaml
    /// </summary>
    public partial class Billing : UserControl
    {
      

        public Billing()
        {
            InitializeComponent();
            dtStartDate.DisplayDateEnd = DateTime.Today;

            //dtPickerEnd.DisplayDateEnd = DateTime.Today;
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

        private void dtStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy"; //for the second type
            Thread.CurrentThread.CurrentCulture = ci;
        }


        private async  void btnStartPolling_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (WebClient web = new WebClient())
                {
                    string url = string.Format("http://119.2.119.202:3500/dtmeter/billing/data/{0}", txtDtId.Text);
                    string json = await web.DownloadStringTaskAsync(url);

                    List<BillingClass> billingsData = JsonConvert.DeserializeObject<List<BillingClass>>(json);

                    // Get the selected date from the DatePicker
                    DateTime selectedDate = dtStartDate.SelectedDate ?? DateTime.MinValue;
                    string formattedDate = selectedDate.ToString("dd-MM-yyyy");

                    // Filter the billingsData based on the selected date
                    List<BillingClass> filteredBillingsData = billingsData
                        .Where(billing => billing.date == formattedDate)
                        .ToList();

                    // Display the import_active_energy from the first filtered Billing object
                    if (filteredBillingsData.Count > 0)
                    {
                        BillingClass filteredBilling = filteredBillingsData[0];

                        lblBillingDate.Content = filteredBilling.date;


                        lblImportActiveEnergy.Content = filteredBilling.import_active_energy;
                        lblImportActiveEnergyHis1.Content = filteredBilling.import_active_energy_history_1;
                        lblImportActiveEnergyHis2.Content = filteredBilling.import_active_energy_history_2;

                        lblImportReactiveEnergy.Content = filteredBilling.import_reactive_energy;
                        lblImportReactiveEnergyHis1.Content = filteredBilling.import_reactive_energy_history_1;
                        lblImportReactiveEnergyHis2.Content = filteredBilling.import_reactive_energy_history_2;

                        lblTotActiveEnergy.Content = filteredBilling.total_active_energy;
                        lblTotActiveEnergyHis1.Content = filteredBilling.total_active_energy_history_1;
                        lblTotActiveEnergyHis2.Content = filteredBilling.total_active_energy_history_2;

                        lblTotReactiveEnergy.Content = filteredBilling.total_reactive_energy;
                        lblTotReactiveEnergyHis1.Content = filteredBilling.total_reactive_energy_history_1;
                        lblTotReactiveEnergyHis2.Content = filteredBilling.total_reactive_energy_history_2;

                        lblTotalApparentEnergy.Content = filteredBilling.total_apparent_energy;
                        lblTotalApparentEnergyHis1.Content = filteredBilling.total_apparent_energy_history_1;
                        lblTotalApparentEnergyHis2.Content = filteredBilling.total_apparent_energy_history_2;

                        lblPhaseAImportActiveEnergy.Content = filteredBilling.phase_a_import_active_energy;
                        lblPhaseAImportActiveEnergyHis1.Content = filteredBilling.phase_a_import_active_energy_history_1;
                        lblPhaseAImportActiveEnergyHis2.Content = filteredBilling.phase_a_import_active_energy_history_2;

                        lblPhaseBImportActiveEnergy.Content = filteredBilling.phase_b_import_active_energy;
                        lblPhaseBImportActiveEnergyHis1.Content = filteredBilling.phase_b_import_active_energy_history_1;
                        lblPhaseBImportActiveEnergyHis2.Content = filteredBilling.phase_b_import_active_energy_history_2;


                        lblPhaseCImportActiveEnergy.Content = filteredBilling.phase_c_import_active_energy;
                        lblPhaseCImportActiveEnergyHis1.Content = filteredBilling.phase_c_import_active_energy_history_1;
                        lblPhaseCImportActiveEnergyHis2.Content = filteredBilling.phase_c_import_active_energy_history_2;

                        lblImportActiveMaxDemand.Content = filteredBilling.import_active_maximum_demand;
                        lblImportActiveMaxDemandHis1.Content = filteredBilling.import_active_maximum_demand_history_1;
                        lblImportActiveMaxDemandHis2.Content = filteredBilling.import_active_maximum_demand_history_2;
                    }
                    else
                    {
                        lblImportActiveEnergy.Content = "No data available.";
                    }
                }
            }
            catch
            {
                MessageBox.Show("Select Billing Date");
            }
        }

       
       


        #region scroll Viewer Synchronization

        private void scrollVSelectrdPara_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            // Reduce the speed of the scrolling by a factor of 2

            if (e.Handled)
            {
                return;
            }

            e.Handled = true;

            ScrollViewer scrollViewer = sender as ScrollViewer;

            if (scrollViewer != null)
            {
                // Reduce the speed of the scrolling by a factor of 2
                double delta = e.Delta / 2.0;

                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - delta);

                ScrollViewer scrollVData = FindVisualChild<ScrollViewer>(this, "scrollVData");

                if (scrollVData != null)
                {
                    scrollVData.ScrollToVerticalOffset(scrollViewer.VerticalOffset - delta);
                }
            }


            //if (e.Handled)
            //    return;

            //e.Handled = true;

            //var scrollViewer = sender as ScrollViewer;

            //if (scrollViewer != null)
            //{
            //    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);

            //    // Find the other ScrollViewer named "scrollVData"
            //    var scrollVData = FindVisualChild<ScrollViewer>(this, "scrollVData");

            //    if (scrollVData != null)
            //    {
            //        // Scroll the other ScrollViewer at the same time
            //        scrollVData.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            //    }
            //}
        }


        public static T FindVisualChild<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            int count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T && (child as FrameworkElement)?.Name == name)
                {
                    return child as T;
                }

                T result = FindVisualChild<T>(child, name);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        #endregion scroll Viewer Synchronization


        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Grid).Children.Remove(this);
        }



        #region Checkbox

        #region Parent CheckBox
        private void checkParentCheckBoxMethod()
        {
            cbParent.IsChecked = null;
            if ((cbBillingDate.IsChecked == true) && (cbImportActiveEnergy.IsChecked == true) && (cbImportActiveEnergyHis1.IsChecked == true) 
                && (cbImportActiveEnergyHis2.IsChecked == true) && (cbImportActiveEnergyHis3.IsChecked == true)
                && (cbImportActiveEnergyRate1.IsChecked == true) && (cbImportActiveEnergyRate1His1.IsChecked == true) 
                && (cbImportActiveEnergyRate1His2.IsChecked == true) && (cbImportActiveEnergyRate1His3.IsChecked == true)
                && (cbImportReactiveEnergy.IsChecked == true) && (cbImportReactiveEnergyHis1.IsChecked == true) 
                && (cbImportReactiveEnergyHis2.IsChecked == true) && (cbImportReactiveEnergyHis3.IsChecked == true)
                && (cbImportReactiveEnergyRate.IsChecked == true) && (cbImportReactiveEnergyRateHis1.IsChecked == true) 
                && (cbImportReactiveEnergyRateHis2.IsChecked == true) && (cbImportReactiveEnergyRateHis3.IsChecked == true)
                && (cbTotActiveEnergy.IsChecked == true) && (cbTotActiveEnergyHis1.IsChecked == true) 
                && (cbTotActiveEnergyHis2.IsChecked == true) && (cbTotActiveEnergyHis3.IsChecked == true)
                && (cbTotActiveEnergyRate1.IsChecked == true) && (cbTotActiveEnergyRate1His1.IsChecked == true)
                && (cbTotActiveEnergyRate1His2.IsChecked == true) && (cbTotActiveEnergyRate1His3.IsChecked == true)
                && (cbTotReactiveEnergy.IsChecked == true) && (cbTotReactiveEnergyHis1.IsChecked == true) 
                && (cbTotReactiveEnergyHis2.IsChecked == true) 
                && (cbTotReactiveEnergyHis3.IsChecked == true) && (cbTotReactiveEnergyRate1.IsChecked == true)
                && (cbTotReactiveEnergyRate1His1.IsChecked == true) && (cbTotReactiveEnergyRate1His2.IsChecked == true) 
                && (cbTotReactiveEnergyRate1His3.IsChecked == true) && (cbTotalApprentEnergy.IsChecked == true)
                && (cbTotalApprentEnergyHis1.IsChecked == true) && (cbTotalApprentEnergyHis2.IsChecked == true) 
                && (cbTotalApprentEnergyHis3.IsChecked == true) && (cbTotalApprentEnergyRate1.IsChecked == true) 
                && (cbTotalApprentEnergyRate1His1.IsChecked == true)
                 && (cbTotalApprentEnergyRate1His2.IsChecked == true) && (cbTotalApprentEnergyRate1His3.IsChecked == true) 
                 && (cbPhaseAImportActiveEnergy.IsChecked == true) && (cbPhaseAImportActiveEnergyHis1.IsChecked == true)
                && (cbPhaseAImportActiveEnergyHis2.IsChecked == true) && (cbPhaseAImportActiveEnergyHis3.IsChecked == true) 
                && (cbPhaseAImportActiveEnergyRate1.IsChecked == true) && (cbPhaseAImportActiveEnergyRate1His1.IsChecked == true)
                && (cbPhaseAImportActiveEnergyRate1His2.IsChecked == true) && (cbPhaseAImportActiveEnergyRate1His3.IsChecked == true)
                && (cbPhaseBImportActiveEnergy.IsChecked == true) && (cbPhaseBImportActiveEnergyHis1.IsChecked == true)
                && (cbPhaseBImportActiveEnergyHis2.IsChecked == true)
                && (cbPhaseBImportActiveEnergyHis3.IsChecked == true) && (cbPhaseBImportActiveEnergyRate1.IsChecked == true) 
                && (cbPhaseBImportActiveEnergyRate1His1.IsChecked == true) && (cbPhaseBImportActiveEnergyRate1His2.IsChecked == true)
                && (cbPhaseBImportActiveEnergyRate1His3.IsChecked == true) && (cbPhaseCImportActiveEnergy.IsChecked == true) 
                && (cbPhaseCImportActiveEnergyHis1.IsChecked == true) && (cbPhaseCImportActiveEnergyHis3.IsChecked == true)
                && (cbPhaseCImportActiveEnergyRate1.IsChecked == true)
                 && (cbPhaseCImportActiveEnergyRate1His1.IsChecked == true) && (cbPhaseCImportActiveEnergyRate1His2.IsChecked == true) 
                 && (cbPhaseCImportActiveEnergyRate1His3.IsChecked == true) && (cbImportActiveMaxDemand.IsChecked == true)
                && (cbImportActiveMaxDemandHis1.IsChecked == true) && (cbImportActiveMaxDemandHis2.IsChecked == true) 
                && (cbImportActiveMaxDemandHis3.IsChecked == true) && (cbImportActiveMaxDemandRate1.IsChecked == true)
                && (cbImportActiveMaxDemandRate1His1.IsChecked == true) && (cbImportActiveMaxDemandRate1His2.IsChecked == true)
                && (cbImportActiveMaxDemandRate1His3.IsChecked == true))
            {
                cbParent.IsChecked = true;
            }


            
        }

        private void uncheckParentCheckBoxMethod()
        {
            cbParent.IsChecked = null;
            if ((cbBillingDate.IsChecked == false) && (cbImportActiveEnergy.IsChecked == false) && (cbImportActiveEnergyHis1.IsChecked == false)
                && (cbImportActiveEnergyHis2.IsChecked == false) && (cbImportActiveEnergyHis3.IsChecked == false)
                && (cbImportActiveEnergyRate1.IsChecked == false) && (cbImportActiveEnergyRate1His1.IsChecked == false)
                && (cbImportActiveEnergyRate1His2.IsChecked == false) && (cbImportActiveEnergyRate1His3.IsChecked == false)
                && (cbImportReactiveEnergy.IsChecked == false) && (cbImportReactiveEnergyHis1.IsChecked == false)
                && (cbImportReactiveEnergyHis2.IsChecked == false) && (cbImportReactiveEnergyHis3.IsChecked == false)
                && (cbImportReactiveEnergyRate.IsChecked == false) && (cbImportReactiveEnergyRateHis1.IsChecked == false)
                && (cbImportReactiveEnergyRateHis2.IsChecked == false) && (cbImportReactiveEnergyRateHis3.IsChecked == false)
                && (cbTotActiveEnergy.IsChecked == false) && (cbTotActiveEnergyHis1.IsChecked == false)
                && (cbTotActiveEnergyHis2.IsChecked == false) && (cbTotActiveEnergyHis3.IsChecked == false)
                && (cbTotActiveEnergyRate1.IsChecked == false) && (cbTotActiveEnergyRate1His1.IsChecked == false)
                && (cbTotActiveEnergyRate1His2.IsChecked == false) && (cbTotActiveEnergyRate1His3.IsChecked == false)
                && (cbTotReactiveEnergy.IsChecked == false) && (cbTotReactiveEnergyHis1.IsChecked == false)
                && (cbTotReactiveEnergyHis2.IsChecked == false)
                && (cbTotReactiveEnergyHis3.IsChecked == false) && (cbTotReactiveEnergyRate1.IsChecked == false)
                && (cbTotReactiveEnergyRate1His1.IsChecked == false) && (cbTotReactiveEnergyRate1His2.IsChecked == false)
                && (cbTotReactiveEnergyRate1His3.IsChecked == false) && (cbTotalApprentEnergy.IsChecked == false)
                && (cbTotalApprentEnergyHis1.IsChecked == false) && (cbTotalApprentEnergyHis2.IsChecked == false)
                && (cbTotalApprentEnergyHis3.IsChecked == false) && (cbTotalApprentEnergyRate1.IsChecked == false)
                && (cbTotalApprentEnergyRate1His1.IsChecked == false)
                 && (cbTotalApprentEnergyRate1His2.IsChecked == false) && (cbTotalApprentEnergyRate1His3.IsChecked == false)
                 && (cbPhaseAImportActiveEnergy.IsChecked == false) && (cbPhaseAImportActiveEnergyHis1.IsChecked == false)
                && (cbPhaseAImportActiveEnergyHis2.IsChecked == false) && (cbPhaseAImportActiveEnergyHis3.IsChecked == false)
                && (cbPhaseAImportActiveEnergyRate1.IsChecked == false) && (cbPhaseAImportActiveEnergyRate1His1.IsChecked == false)
                && (cbPhaseAImportActiveEnergyRate1His2.IsChecked == false) && (cbPhaseAImportActiveEnergyRate1His3.IsChecked == false)
                && (cbPhaseBImportActiveEnergy.IsChecked == false) && (cbPhaseBImportActiveEnergyHis1.IsChecked == false)
                && (cbPhaseBImportActiveEnergyHis2.IsChecked == false)
                && (cbPhaseBImportActiveEnergyHis3.IsChecked == false) && (cbPhaseBImportActiveEnergyRate1.IsChecked == false)
                && (cbPhaseBImportActiveEnergyRate1His1.IsChecked == false) && (cbPhaseBImportActiveEnergyRate1His2.IsChecked == false)
                && (cbPhaseBImportActiveEnergyRate1His3.IsChecked == false) && (cbPhaseCImportActiveEnergy.IsChecked == false)
                && (cbPhaseCImportActiveEnergyHis1.IsChecked == false) && (cbPhaseCImportActiveEnergyHis3.IsChecked == false)
                && (cbPhaseCImportActiveEnergyRate1.IsChecked == false)
                 && (cbPhaseCImportActiveEnergyRate1His1.IsChecked == false) && (cbPhaseCImportActiveEnergyRate1His2.IsChecked == false)
                 && (cbPhaseCImportActiveEnergyRate1His3.IsChecked == false) && (cbImportActiveMaxDemand.IsChecked == false)
                && (cbImportActiveMaxDemandHis1.IsChecked == false) && (cbImportActiveMaxDemandHis2.IsChecked == false)
                && (cbImportActiveMaxDemandHis3.IsChecked == false) && (cbImportActiveMaxDemandRate1.IsChecked == false)
                && (cbImportActiveMaxDemandRate1His1.IsChecked == false) && (cbImportActiveMaxDemandRate1His2.IsChecked == false)
                && (cbImportActiveMaxDemandRate1His3.IsChecked == false))
            {
                cbParent.IsChecked = false;
            }


        }
   
        
        private void cbParent_Checked(object sender, RoutedEventArgs e)
        {
            bool newVal = cbParent.IsChecked == true;


            cbBillingDate.IsChecked = newVal;
            cbImportActiveEnergy.IsChecked = newVal;
            cbImportActiveEnergyHis1.IsChecked = newVal;
            cbImportActiveEnergyHis2.IsChecked = newVal;
            cbImportActiveEnergyHis3.IsChecked = newVal;
            cbImportActiveEnergyRate1.IsChecked = newVal;
            cbImportActiveEnergyRate1His1.IsChecked = newVal;
            cbImportActiveEnergyRate1His2.IsChecked = newVal;
            cbImportActiveEnergyRate1His3.IsChecked = newVal;
            cbImportReactiveEnergy.IsChecked = newVal;
            cbImportReactiveEnergyHis1.IsChecked = newVal;
            cbImportReactiveEnergyHis2.IsChecked = newVal;
            cbImportReactiveEnergyHis3.IsChecked = newVal;
            cbImportReactiveEnergyRate.IsChecked = newVal;
            cbImportReactiveEnergyRateHis1.IsChecked = newVal;
            cbImportReactiveEnergyRateHis2.IsChecked = newVal;
            cbImportReactiveEnergyRateHis3.IsChecked = newVal;
            cbTotActiveEnergy.IsChecked = newVal;
            cbTotActiveEnergyHis1.IsChecked = newVal;
            cbTotActiveEnergyHis2.IsChecked = newVal;
            cbTotActiveEnergyHis3.IsChecked = newVal;
            cbTotActiveEnergyRate1.IsChecked = newVal;
            cbTotActiveEnergyRate1His1.IsChecked = newVal;
            cbTotActiveEnergyRate1His2.IsChecked = newVal;
            cbTotActiveEnergyRate1His3.IsChecked = newVal;
            cbTotReactiveEnergy.IsChecked = newVal;
            cbTotReactiveEnergyHis1.IsChecked = newVal;
            cbTotReactiveEnergyHis2.IsChecked = newVal;
            cbTotReactiveEnergyHis3.IsChecked = newVal;
            cbTotReactiveEnergyRate1.IsChecked = newVal;
            cbTotReactiveEnergyRate1His1.IsChecked = newVal;
            cbTotReactiveEnergyRate1His2.IsChecked = newVal;
            cbTotReactiveEnergyRate1His3.IsChecked = newVal;
            cbTotalApprentEnergy.IsChecked = newVal;
            cbTotalApprentEnergyHis1.IsChecked = newVal;
            cbTotalApprentEnergyHis2.IsChecked = newVal;
            cbTotalApprentEnergyHis3.IsChecked = newVal;
            cbTotalApprentEnergyRate1.IsChecked = newVal;
            cbTotalApprentEnergyRate1His1.IsChecked = newVal;
            cbTotalApprentEnergyRate1His2.IsChecked = newVal;
            cbTotalApprentEnergyRate1His3.IsChecked = newVal;
            cbPhaseAImportActiveEnergy.IsChecked = newVal;
            cbPhaseAImportActiveEnergyHis1.IsChecked = newVal;
            cbPhaseAImportActiveEnergyHis2.IsChecked = newVal;
            cbPhaseAImportActiveEnergyHis3.IsChecked = newVal;
            cbPhaseAImportActiveEnergyRate1.IsChecked = newVal;
            cbPhaseAImportActiveEnergyRate1His1.IsChecked = newVal;
            cbPhaseAImportActiveEnergyRate1His2.IsChecked = newVal;
            cbPhaseAImportActiveEnergyRate1His3.IsChecked = newVal;
            cbPhaseBImportActiveEnergy.IsChecked = newVal;
            cbPhaseBImportActiveEnergyHis1.IsChecked = newVal;
            cbPhaseBImportActiveEnergyHis2.IsChecked = newVal;
            cbPhaseBImportActiveEnergyHis3.IsChecked = newVal;
            cbPhaseBImportActiveEnergyRate1.IsChecked = newVal;
            cbPhaseBImportActiveEnergyRate1His1.IsChecked = newVal;
            cbPhaseBImportActiveEnergyRate1His2.IsChecked = newVal;
            cbPhaseBImportActiveEnergyRate1His3.IsChecked = newVal;
            cbPhaseCImportActiveEnergy.IsChecked = newVal;
            cbPhaseCImportActiveEnergyHis1.IsChecked = newVal;
            cbPhaseCImportActiveEnergyHis3.IsChecked = newVal;
            cbPhaseCImportActiveEnergyRate1.IsChecked = newVal;
            cbPhaseCImportActiveEnergyRate1His1.IsChecked = newVal;
            cbPhaseCImportActiveEnergyRate1His2.IsChecked = newVal;
            cbPhaseCImportActiveEnergyRate1His3.IsChecked = newVal;
            cbImportActiveMaxDemand.IsChecked = newVal;
            cbImportActiveMaxDemandHis1.IsChecked = newVal;
            cbImportActiveMaxDemandHis2.IsChecked = newVal;
            cbImportActiveMaxDemandHis3.IsChecked = newVal;
            cbImportActiveMaxDemandRate1.IsChecked = newVal;
            cbImportActiveMaxDemandRate1His1.IsChecked = newVal;
            cbImportActiveMaxDemandRate1His2.IsChecked = newVal;
            cbImportActiveMaxDemandRate1His3.IsChecked = newVal;

        }
    
        private void cbParent_Unchecked(object sender, RoutedEventArgs e)
        {
            bool newVal = cbParent.IsChecked == true;

            cbBillingDate.IsChecked = newVal;
            cbImportActiveEnergy.IsChecked = newVal;
            cbImportActiveEnergyHis1.IsChecked = newVal;
            cbImportActiveEnergyHis2.IsChecked = newVal;
            cbImportActiveEnergyHis3.IsChecked = newVal;
            cbImportActiveEnergyRate1.IsChecked = newVal;
            cbImportActiveEnergyRate1His1.IsChecked = newVal;
            cbImportActiveEnergyRate1His2.IsChecked = newVal;
            cbImportActiveEnergyRate1His3.IsChecked = newVal;
            cbImportReactiveEnergy.IsChecked = newVal;
            cbImportReactiveEnergyHis1.IsChecked = newVal;
            cbImportReactiveEnergyHis2.IsChecked = newVal;
            cbImportReactiveEnergyHis3.IsChecked = newVal;
            cbImportReactiveEnergyRate.IsChecked = newVal;
            cbImportReactiveEnergyRateHis1.IsChecked = newVal;
            cbImportReactiveEnergyRateHis2.IsChecked = newVal;
            cbImportReactiveEnergyRateHis3.IsChecked = newVal;
            cbTotActiveEnergy.IsChecked = newVal;
            cbTotActiveEnergyHis1.IsChecked = newVal;
            cbTotActiveEnergyHis2.IsChecked = newVal;
            cbTotActiveEnergyHis3.IsChecked = newVal;
            cbTotActiveEnergyRate1.IsChecked = newVal;
            cbTotActiveEnergyRate1His1.IsChecked = newVal;
            cbTotActiveEnergyRate1His2.IsChecked = newVal;
            cbTotActiveEnergyRate1His3.IsChecked = newVal;
            cbTotReactiveEnergy.IsChecked = newVal;
            cbTotReactiveEnergyHis1.IsChecked = newVal;
            cbTotReactiveEnergyHis2.IsChecked = newVal;
            cbTotReactiveEnergyHis3.IsChecked = newVal;
            cbTotReactiveEnergyRate1.IsChecked = newVal;
            cbTotReactiveEnergyRate1His1.IsChecked = newVal;
            cbTotReactiveEnergyRate1His2.IsChecked = newVal;
            cbTotReactiveEnergyRate1His3.IsChecked = newVal;
            cbTotalApprentEnergy.IsChecked = newVal;
            cbTotalApprentEnergyHis1.IsChecked = newVal;
            cbTotalApprentEnergyHis2.IsChecked = newVal;
            cbTotalApprentEnergyHis3.IsChecked = newVal;
            cbTotalApprentEnergyRate1.IsChecked = newVal;
            cbTotalApprentEnergyRate1His1.IsChecked = newVal;
            cbTotalApprentEnergyRate1His2.IsChecked = newVal;
            cbTotalApprentEnergyRate1His3.IsChecked = newVal;
            cbPhaseAImportActiveEnergy.IsChecked = newVal;
            cbPhaseAImportActiveEnergyHis1.IsChecked = newVal;
            cbPhaseAImportActiveEnergyHis2.IsChecked = newVal;
            cbPhaseAImportActiveEnergyHis3.IsChecked = newVal;
            cbPhaseAImportActiveEnergyRate1.IsChecked = newVal;
            cbPhaseAImportActiveEnergyRate1His1.IsChecked = newVal;
            cbPhaseAImportActiveEnergyRate1His2.IsChecked = newVal;
            cbPhaseAImportActiveEnergyRate1His3.IsChecked = newVal;
            cbPhaseBImportActiveEnergy.IsChecked = newVal;
            cbPhaseBImportActiveEnergyHis1.IsChecked = newVal;
            cbPhaseBImportActiveEnergyHis2.IsChecked = newVal;
            cbPhaseBImportActiveEnergyHis3.IsChecked = newVal;
            cbPhaseBImportActiveEnergyRate1.IsChecked = newVal;
            cbPhaseBImportActiveEnergyRate1His1.IsChecked = newVal;
            cbPhaseBImportActiveEnergyRate1His2.IsChecked = newVal;
            cbPhaseBImportActiveEnergyRate1His3.IsChecked = newVal;
            cbPhaseCImportActiveEnergy.IsChecked = newVal;
            cbPhaseCImportActiveEnergyHis1.IsChecked = newVal;
            cbPhaseCImportActiveEnergyHis3.IsChecked = newVal;
            cbPhaseCImportActiveEnergyRate1.IsChecked = newVal;
            cbPhaseCImportActiveEnergyRate1His1.IsChecked = newVal;
            cbPhaseCImportActiveEnergyRate1His2.IsChecked = newVal;
            cbPhaseCImportActiveEnergyRate1His3.IsChecked = newVal;
            cbImportActiveMaxDemand.IsChecked = newVal;
            cbImportActiveMaxDemandHis1.IsChecked = newVal;
            cbImportActiveMaxDemandHis2.IsChecked = newVal;
            cbImportActiveMaxDemandHis3.IsChecked = newVal;
            cbImportActiveMaxDemandRate1.IsChecked = newVal;
            cbImportActiveMaxDemandRate1His1.IsChecked = newVal;
            cbImportActiveMaxDemandRate1His2.IsChecked = newVal;
            cbImportActiveMaxDemandRate1His3.IsChecked = newVal;

        }

        #endregion Parent CheckBox

        private void cbBillingDate_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbBillingDate.Content);
            lblBillingDate.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbBillingDate_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbBillingDate.Content);
            lblBillingDate.Visibility = Visibility.Collapsed ;
            uncheckParentCheckBoxMethod();
        }


        #region Checkbox Import Active Energy
        private void cbImportActiveEnergy_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveEnergy.Content);
            lblImportActiveEnergy.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveEnergy_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveEnergy.Content);
            lblImportActiveEnergy.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportActiveEnergyHis1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveEnergyHis1.Content);
            lblImportActiveEnergyHis1 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveEnergyHis1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveEnergyHis1.Content);
            lblImportActiveEnergyHis1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportActiveEnergyHis2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveEnergyHis2.Content);
            lblImportActiveEnergyHis2 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveEnergyHis2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveEnergyHis2.Content);
            lblImportActiveEnergyHis2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportActiveEnergyHis3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveEnergyHis3.Content);
            lblImportActiveEnergyHis3 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveEnergyHis3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveEnergyHis3.Content);
            lblImportActiveEnergyHis3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportActiveEnergyRate1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveEnergyRate1.Content);
            lblImportActiveEnergyRate1 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveEnergyRate1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveEnergyRate1.Content);
            lblImportActiveEnergyRate1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportActiveEnergyRate1His1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveEnergyRate1His1.Content);
            lblImportActiveEnergyRate1His1 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveEnergyRate1His1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveEnergyRate1His1.Content);
            lblImportActiveEnergyRate1His1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportActiveEnergyRate1His2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveEnergyRate1His2.Content);
            lblImportActiveEnergyRate1His2 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveEnergyRate1His2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveEnergyRate1His2.Content);
            lblImportActiveEnergyRate1His2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportActiveEnergyRate1His3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveEnergyRate1His3.Content);
            lblImportActiveEnergyRate1His3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveEnergyRate1His3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveEnergyRate1His3.Content);
            lblImportActiveEnergyRate1His3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        #endregion Checkbox Import Active Energy

        #region Checkbox Import Reactive Energy
        private void cbImportReactiveEnergy_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportReactiveEnergy.Content);
            lblImportReactiveEnergy.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergy_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportReactiveEnergy.Content);
            lblImportReactiveEnergy.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergyHis1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportReactiveEnergyHis1.Content);
            lblImportReactiveEnergyHis1 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergyHis1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportReactiveEnergyHis1.Content);
            lblImportReactiveEnergyHis1 .Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergyHis2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportReactiveEnergyHis2.Content);
            lblImportReactiveEnergyHis2 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergyHis2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportReactiveEnergyHis2.Content);
            lblImportReactiveEnergyHis2 .Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergyHis3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportReactiveEnergyHis3.Content);
            lblImportReactiveEnergyHis3 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergyHis3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportReactiveEnergyHis3.Content);
            lblImportReactiveEnergyHis3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergyRate_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportReactiveEnergyRate.Content);
            lblImportReactiveEnergyRate .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergyRate_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportReactiveEnergyRate.Content);
            lblImportReactiveEnergyRate.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergyRateHis1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportReactiveEnergyRateHis1.Content);
            lblImportReactiveEnergyRateHis1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergyRateHis1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportReactiveEnergyRateHis1.Content);
            lblImportReactiveEnergyRateHis1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergyRateHis2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportReactiveEnergyRateHis2.Content);
            lblImportReactiveEnergyRateHis2 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergyRateHis2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportReactiveEnergyRateHis2.Content);
            lblImportReactiveEnergyRateHis2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergyRateHis3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportReactiveEnergyRateHis3.Content);
            lblImportReactiveEnergyRateHis3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportReactiveEnergyRateHis3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportReactiveEnergyRateHis3.Content);
            lblImportReactiveEnergyRateHis3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }




        #endregion Checkbox Import Reactive Energy

        #region Total Active Energy
        private void cbTotActiveEnergy_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotActiveEnergy.Content);
            lblTotActiveEnergy.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotActiveEnergy_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotActiveEnergy.Content);
            lblTotActiveEnergy.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotActiveEnergyHis1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotActiveEnergyHis1.Content);
            lblTotActiveEnergyHis1 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotActiveEnergyHis1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotActiveEnergyHis1.Content);
            lblTotActiveEnergyHis1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotActiveEnergyHis2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotActiveEnergyHis2.Content);
            lblTotActiveEnergyHis2.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotActiveEnergyHis2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotActiveEnergyHis2.Content);
            lblTotActiveEnergyHis2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotActiveEnergyHis3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotActiveEnergyHis3.Content);
            lblTotActiveEnergyHis3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotActiveEnergyHis3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotActiveEnergyHis3.Content);
            lblTotActiveEnergyHis3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotActiveEnergyRate1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotActiveEnergyRate1.Content);
            lblTotActiveEnergyRate1 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotActiveEnergyRate1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotActiveEnergyRate1.Content);
            lblTotActiveEnergyRate1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }


        private void cbTotActiveEnergyRate1His1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotActiveEnergyRate1His1.Content);
            lblTotActiveEnergyRate1His1 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotActiveEnergyRate1His1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotActiveEnergyRate1His1.Content);
            lblTotActiveEnergyRate1His1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotActiveEnergyRate1His2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotActiveEnergyRate1His2.Content);
            lblTotActiveEnergyRate1His2.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotActiveEnergyRate1His2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotActiveEnergyRate1His2.Content);
            lblTotActiveEnergyRate1His2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotActiveEnergyRate1His3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotActiveEnergyRate1His3.Content);
            lblTotActiveEnergyRate1His3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotActiveEnergyRate1His3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotActiveEnergyRate1His3.Content);
            lblTotActiveEnergyRate1His3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        #endregion Total Active Energy

        #region Total Reactive Energy
        private void cbTotReactiveEnergy_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotReactiveEnergy.Content);
            lblTotReactiveEnergy.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergy_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotReactiveEnergy.Content);
            lblTotReactiveEnergy.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergyHis1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotReactiveEnergyHis1.Content);
            lblTotReactiveEnergyHis1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergyHis1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotReactiveEnergyHis1.Content);
            lblTotReactiveEnergyHis1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergyHis2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotReactiveEnergyHis2.Content);
            lblTotReactiveEnergyHis2.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergyHis2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotReactiveEnergyHis2.Content);
            lblTotReactiveEnergyHis2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergyHis3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotReactiveEnergyHis3.Content);
            lblTotReactiveEnergyHis3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergyHis3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotReactiveEnergyHis3.Content);
            lblTotReactiveEnergyHis3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergyRate1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotReactiveEnergyRate1.Content);
            lblTotReactiveEnergyRate1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergyRate1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotReactiveEnergyRate1.Content);
            lblTotReactiveEnergyRate1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergyRate1His1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotReactiveEnergyRate1His1.Content);
            lblTotReactiveEnergyRate1His1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergyRate1His1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotReactiveEnergyRate1His1.Content);
            lblTotReactiveEnergyRate1His1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergyRate1His2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotReactiveEnergyRate1His2.Content);
            lblTotReactiveEnergyRate1His2.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergyRate1His2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotReactiveEnergyRate1His2.Content);
            lblTotReactiveEnergyRate1His2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergyRate1His3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotReactiveEnergyRate1His3.Content);
            lblTotReactiveEnergyRate1His3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotReactiveEnergyRate1His3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotReactiveEnergyRate1His3.Content);
            lblTotReactiveEnergyRate1His3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }




        #endregion Total Reactive Energy

        #region Total Aparent Energy
        private void cbTotalApprentEnergy_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotalApprentEnergy.Content);
            lblTotalApparentEnergy.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergy_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotalApprentEnergy.Content);
            lblTotalApparentEnergy.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergyHis1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotalApprentEnergyHis1.Content);
            lblTotalApparentEnergyHis1 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergyHis1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotalApprentEnergyHis1.Content);
            lblTotalApparentEnergyHis1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergyHis2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotalApprentEnergyHis2.Content);
            lblTotalApparentEnergyHis2 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergyHis2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotalApprentEnergyHis2.Content);
            lblTotalApparentEnergyHis2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergyHis3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotalApprentEnergyHis3.Content);
            lblTotalApparentEnergyHis3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergyHis3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotalApprentEnergyHis3.Content);
            lblTotalApparentEnergyHis3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergyRate1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotalApprentEnergyRate1.Content);
            lblTotalApparentEnergyRate1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergyRate1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotalApprentEnergyRate1.Content);
            lblTotalApparentEnergyRate1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergyRate1His1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotalApprentEnergyRate1His1.Content);
            lblTotalApparentEnergyRate1His1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergyRate1His1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotalApprentEnergyRate1His1.Content);
            lblTotalApparentEnergyRate1His1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergyRate1His2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotalApprentEnergyRate1His2.Content);
            lblTotalApparentEnergyRate1His2 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergyRate1His2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotalApprentEnergyRate1His2.Content);
            lblTotalApparentEnergyRate1His2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergyRate1His3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbTotalApprentEnergyRate1His3.Content);
            lblTotalApparentEnergyRate1His3 .Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbTotalApprentEnergyRate1His3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbTotalApprentEnergyRate1His3.Content);
            lblTotalApparentEnergyRate1His3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        #endregion Total Aparent Energy

        #region Phase A Import Active Energy
        private void cbPhaseAImportActiveEnergy_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAImportActiveEnergy.Content);
            lblPhaseAImportActiveEnergy.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergy_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAImportActiveEnergy.Content);
            lblPhaseAImportActiveEnergy.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergyHis1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAImportActiveEnergyHis1.Content);
            lblPhaseAImportActiveEnergyHis1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergyHis1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAImportActiveEnergyHis1.Content);
            lblPhaseAImportActiveEnergyHis1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergyHis2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAImportActiveEnergyHis2.Content);
            lblPhaseAImportActiveEnergyHis2.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergyHis2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAImportActiveEnergyHis2.Content);
            lblPhaseAImportActiveEnergyHis2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergyHis3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAImportActiveEnergyHis3.Content);
            lblPhaseAImportActiveEnergyHis3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergyHis3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAImportActiveEnergyHis3.Content);
            lblPhaseAImportActiveEnergyHis3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergyRate1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAImportActiveEnergyRate1.Content);
            lblPhaseAImportActiveEnergyRate1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergyRate1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAImportActiveEnergyRate1.Content);
            lblPhaseAImportActiveEnergyRate1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergyRate1His1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAImportActiveEnergyRate1His1.Content);
            lblPhaseAImportActiveEnergyRate1His1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergyRate1His1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAImportActiveEnergyRate1His1.Content);
            lblPhaseAImportActiveEnergyRate1His1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergyRate1His2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAImportActiveEnergyRate1His2.Content);
            lblPhaseAImportActiveEnergyRate1His2.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergyRate1His2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAImportActiveEnergyRate1His2.Content);
            lblPhaseAImportActiveEnergyRate1His2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergyRate1His3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseAImportActiveEnergyRate1His3.Content);
            lblPhaseAImportActiveEnergyRate1His3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseAImportActiveEnergyRate1His3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseAImportActiveEnergyRate1His3.Content);
            lblPhaseAImportActiveEnergyRate1His3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }



        #endregion Phase A Import Active Energy

        #region Phase B Import Active Energy
        private void cbPhaseBImportActiveEnergy_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBImportActiveEnergy.Content);
            lblPhaseBImportActiveEnergy.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergy_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBImportActiveEnergy.Content);
            lblPhaseBImportActiveEnergy.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergyHis1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBImportActiveEnergyHis1.Content);
            lblPhaseBImportActiveEnergyHis1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergyHis1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBImportActiveEnergyHis1.Content);
            lblPhaseBImportActiveEnergyHis1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergyHis2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBImportActiveEnergyHis2.Content);
            lblPhaseBImportActiveEnergyHis2.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergyHis2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBImportActiveEnergyHis2.Content);
            lblPhaseBImportActiveEnergyHis2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergyHis3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBImportActiveEnergyHis3.Content);
            lblPhaseBImportActiveEnergyHis3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergyHis3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBImportActiveEnergyHis3.Content);
            lblPhaseBImportActiveEnergyHis3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergyRate1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBImportActiveEnergyRate1.Content);
            lblPhaseBImportActiveEnergyRate1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergyRate1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBImportActiveEnergyRate1.Content);
            lblPhaseBImportActiveEnergyRate1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergyRate1His1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBImportActiveEnergyRate1His1.Content);
            lblPhaseBImportActiveEnergyRate1His1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergyRate1His1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBImportActiveEnergyRate1His1.Content);
            lblPhaseBImportActiveEnergyRate1His1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergyRate1His2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBImportActiveEnergyRate1His2.Content);
            lblPhaseBImportActiveEnergyRate1His2.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergyRate1His2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBImportActiveEnergyRate1His2.Content);
            lblPhaseBImportActiveEnergyRate1His2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergyRate1His3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseBImportActiveEnergyRate1His3.Content);
            lblPhaseBImportActiveEnergyRate1His3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseBImportActiveEnergyRate1His3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseBImportActiveEnergyRate1His3.Content);
            lblPhaseBImportActiveEnergyRate1His3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }


        #endregion Phase B Import Active Energy

        #region Phase C Import Active Energy
        private void cbPhaseCImportActiveEnergy_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCImportActiveEnergy.Content);
            lblPhaseCImportActiveEnergy.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergy_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCImportActiveEnergy.Content);
            lblPhaseCImportActiveEnergy.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergyHis1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCImportActiveEnergyHis1.Content);
            lblPhaseCImportActiveEnergyHis1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergyHis1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCImportActiveEnergyHis1.Content);
            lblPhaseCImportActiveEnergyHis1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergyHis2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCImportActiveEnergyHis2.Content);
            lblPhaseCImportActiveEnergyHis2.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergyHis2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCImportActiveEnergyHis2.Content);
            lblPhaseCImportActiveEnergyHis2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergyHis3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCImportActiveEnergyHis3.Content);
            lblPhaseCImportActiveEnergyHis3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergyHis3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCImportActiveEnergyHis3.Content);
            lblPhaseCImportActiveEnergyHis3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergyRate1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCImportActiveEnergyRate1.Content);
            lblPhaseCImportActiveEnergyRate1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergyRate1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCImportActiveEnergyRate1.Content);
            lblPhaseCImportActiveEnergyRate1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergyRate1His1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCImportActiveEnergyRate1His1.Content);
            lblPhaseCImportActiveEnergyRate1His1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergyRate1His1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCImportActiveEnergyRate1His1.Content);
            lblPhaseCImportActiveEnergyRate1His1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergyRate1His2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCImportActiveEnergyRate1His2.Content);
            lblPhaseCImportActiveEnergyRate1His2.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergyRate1His2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCImportActiveEnergyRate1His2.Content);
            lblPhaseCImportActiveEnergyRate1His2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergyRate1His3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbPhaseCImportActiveEnergyRate1His3.Content);
            lblPhaseCImportActiveEnergyRate1His3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbPhaseCImportActiveEnergyRate1His3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbPhaseCImportActiveEnergyRate1His3.Content);
            lblPhaseCImportActiveEnergyRate1His3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        #endregion Phase C Import Active Energy

        #region Import Active Max Demand
        private void cbImportActiveMaxDemand_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveMaxDemand.Content);
            lblImportActiveMaxDemand.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemand_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveMaxDemand.Content);
            lblImportActiveMaxDemand.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemandHis1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveMaxDemandHis1.Content);
            lblImportActiveMaxDemandHis1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemandHis1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveMaxDemandHis1.Content);
            lblImportActiveMaxDemandHis1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemandHis2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveMaxDemandHis2.Content);
            lblImportActiveMaxDemandHis2.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemandHis2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveMaxDemandHis2.Content);
            lblImportActiveMaxDemandHis2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemandHis3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveMaxDemandHis3.Content);
            lblImportActiveMaxDemandHis3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemandHis3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveMaxDemandHis3.Content);
            lblImportActiveMaxDemandHis3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemandRate1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveMaxDemandRate1.Content);
            lblImportActiveMaxDemandRate1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemandRate1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveMaxDemandRate1.Content);
            lblImportActiveMaxDemandRate1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemandRate1His1_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveMaxDemandRate1His1.Content);
            lblImportActiveMaxDemandRate1His1.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemandRate1His1_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveMaxDemandRate1His1.Content);
            lblImportActiveMaxDemandRate1His1.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemandRate1His2_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveMaxDemandRate1His2.Content);
            lblImportActiveMaxDemandRate1His2.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemandRate1His2_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveMaxDemandRate1His2.Content);
            lblImportActiveMaxDemandRate1His2.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemandRate1His3_Checked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Add(cbImportActiveMaxDemandRate1His3.Content);
            lblImportActiveMaxDemandRate1His3.Visibility = Visibility.Visible;
            checkParentCheckBoxMethod();
        }

        private void cbImportActiveMaxDemandRate1His3_Unchecked(object sender, RoutedEventArgs e)
        {
            lvSelectedPara.Items.Remove(cbImportActiveMaxDemandRate1His3.Content);
            lblImportActiveMaxDemandRate1His3.Visibility = Visibility.Collapsed;
            uncheckParentCheckBoxMethod();
        }


        #endregion Import Active Max Demand

        #endregion Checkbox

       
    }
}
