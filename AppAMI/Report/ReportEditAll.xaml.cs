using AppAMI.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

namespace AppAMI.Report
{
    /// <summary>
    /// Interaction logic for ReportEditAll.xaml
    /// </summary>
    public partial class ReportEditAll : Window
    {
        ReportDt selectedDt;

        string CurrentDistrictCode;
        string CurrentEsdCode;


        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;

        public ReportEditAll(ReportDt selectedDt, string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1)
        {
            InitializeComponent();

            CurrentUserId2 = CurrentUserId1;
            CurrentUserRole2 = CurrentUserRole1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;

            this.selectedDt = selectedDt;


            lblDtId.Content = this.selectedDt.dt_id;

            CurrentDistrictCode = this.selectedDt.district_code;
            CurrentEsdCode = this.selectedDt.esd_code;

            txtRegionName.Text = this.selectedDt.region_name;
            txtRegionCode.Text = this.selectedDt.region_code;
            txtDistrictName.Text = this.selectedDt.district_name;
            txtDistrictCode.Text = this.selectedDt.district_code;
            txtEsdName.Text = this.selectedDt.esd_name;

            txtEsdCode.Text = this.selectedDt.esd_code;
            txtFeederId.Text = this.selectedDt.feeder_id;
            txtPortionId.Text = this.selectedDt.portion_id;
            txtRootId.Text = this.selectedDt.root_id;
            txtDtId.Text = this.selectedDt.dt_id;

            txtTransformerSerialNumber.Text = this.selectedDt.transformer_serial_number;
            txtDtMeterSerialNo.Text = this.selectedDt.dt_meter_serial_no;
            txtMriSerialNo.Text = this.selectedDt.mri_serial_no;
            txtFeederName.Text = this.selectedDt.feeder_name;
            txtTripDate.Text = this.selectedDt.trip_date;

            txtTripTime.Text = this.selectedDt.trip_time;
            txtRestoreDate.Text = this.selectedDt.restore_date;
            txtRestoreTime.Text = this.selectedDt.restore_time;
            txtDuration.Text = this.selectedDt.duration;
            txtNoOfCustomersAffected.Text = this.selectedDt.number_of_customers_affected;




            txtTrippingId.Text = this.selectedDt.tripping_id;
            txtSubstation.Text = this.selectedDt.substation;
            txtTrippingPoint.Text = this.selectedDt.tripping_point;
            txtSubstationIndication.Text = this.selectedDt.substation_relay_indication;
            txtTypeofTripping.Text = this.selectedDt.type_of_tripping;

            txtHowEventIdentified.Text = this.selectedDt.how_was_the_incident_identified;
            txtWeatherCondition.Text = this.selectedDt.weather_condition;
            txtStartTimeStation.Text = this.selectedDt.start_time_from_station;
            txtEndJourneyAtFaultLocation.Text = this.selectedDt.end_journey_at_fault_location;
            txtModeofTransport.Text = this.selectedDt.mode_of_transport;

            txtRoadCondition.Text = this.selectedDt.road_condition;
            txtMaintenanceType.Text = this.selectedDt.maintenance_type;
            txtWorkCarriedOut.Text = this.selectedDt.work_carried_out;
            txtNameOfEquipment.Text = this.selectedDt.name_of_equipment;
            txtCausesOfOutage.Text = this.selectedDt.causes_of_outage;

            txtImpactReason.Text = this.selectedDt.impact_reason;
            txtElaborateCauseOfOutages.Text = this.selectedDt.elaborate_the_cause_of_outages;
            txtResponsiblePerson.Text = this.selectedDt.responsible_person;
            txtTargetCompletionDate.Text = this.selectedDt.target_completion_date;
            txtStatus.Text = this.selectedDt.status;

            txtRepairCost.Text = this.selectedDt.repair_cost;
            txtCustomerCompensation.Text = this.selectedDt.customer_compensation;
            txtAffectedArea.Text = this.selectedDt.area_affected;


        }


        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDtId.Text))
            {
                MessageBox.Show("Please fill in the fields marked with a star (*).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                progressLogin.Visibility = Visibility.Collapsed;
            }

            else
            {
                progressLogin.Visibility = Visibility.Visible;
                try
                {

                    ReportDt reportDt = new ReportDt()
                    {

                        region_name = txtRegionName.Text,
                        region_code = txtRegionCode.Text,
                        district_name = txtDistrictName.Text,
                        district_code = txtDistrictCode.Text,
                        esd_name = txtEsdName.Text,

                        esd_code = txtEsdCode.Text,
                        feeder_id = txtFeederId.Text,
                        portion_id = txtPortionId.Text,
                        root_id = txtRootId.Text,
                        dt_id = txtDtId.Text,


                        transformer_serial_number = txtTransformerSerialNumber.Text,
                        dt_meter_serial_no = txtDtMeterSerialNo.Text,
                        mri_serial_no = txtMriSerialNo.Text,
                        feeder_name = txtFeederName.Text,
                        tripping_id = txtFeederId.Text,

                        substation = txtSubstation.Text,
                        tripping_point = txtTrippingPoint.Text,
                        substation_relay_indication = txtSubstationIndication.Text,
                        trip_date = txtTripDate.Text,
                        trip_time = txtTripTime.Text,

                        restore_date = txtRestoreDate.Text,
                        restore_time = txtRestoreTime.Text,
                        duration = txtDuration.Text,
                        number_of_customers_affected = txtNoOfCustomersAffected.Text,
                        type_of_tripping = txtTypeofTripping.Text,

                        how_was_the_incident_identified = txtHowEventIdentified.Text,
                        weather_condition = txtWeatherCondition.Text,
                        start_time_from_station = txtStartTimeStation.Text,
                        end_journey_at_fault_location = txtEndJourneyAtFaultLocation.Text,
                        mode_of_transport = txtModeofTransport.Text,

                        road_condition = txtRoadCondition.Text,
                        maintenance_type = txtMaintenanceType.Text,
                        work_carried_out = txtWorkCarriedOut.Text,
                        name_of_equipment = txtNameOfEquipment.Text,
                        causes_of_outage = txtCausesOfOutage.Text,

                        impact_reason = txtImpactReason.Text,
                        elaborate_the_cause_of_outages = txtElaborateCauseOfOutages.Text,
                        responsible_person = txtResponsiblePerson.Text,
                        target_completion_date = txtTargetCompletionDate.Text,
                        status = txtStatus .Text,

                        repair_cost = txtRepairCost.Text,
                        customer_compensation = txtCustomerCompensation.Text,
                        area_affected = txtAffectedArea.Text

                    };

                  
                    string url = string.Format("http://103.234.126.43:3080/meter_data/report/{0}/{1}", txtDtId.Text, txtTrippingId.Text);

                    HttpClient client = new HttpClient();
                    string jsonData = JsonConvert.SerializeObject(reportDt);
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {

                        MessageBox.Show("POI successfully updated");
                        Close();
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show("Error updating DT configuration. Please check the details and try again.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                catch
                {
                    MessageBox.Show("Server connection is lost. Please check your network connection and try again.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                finally
                {

                    progressLogin.Visibility = Visibility.Collapsed;

                }
            }

        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
