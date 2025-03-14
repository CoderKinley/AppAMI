using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI.Classes
{
    public class ReportDt
    {

        public string region_name { get; set; }
        public string region_code { get; set; }
        public string district_name { get; set; }
        public string district_code { get; set; }
        public string esd_name { get; set; }

        public string esd_code { get; set; }
        public string feeder_id { get; set; }
        public string portion_id { get; set; }
        public string root_id { get; set; }
        public string dt_id { get; set; }


        public string transformer_serial_number { get; set; }
        public string dt_meter_serial_no { get; set; }
        public string mri_serial_no { get; set; }
        public string feeder_name { get; set; }
        public string tripping_id { get; set; }

        public string substation { get; set; }
        public string tripping_point { get; set; }
        public string substation_relay_indication { get; set; }
        public string trip_date { get; set; }
        public string trip_time { get; set; }

        public string restore_date { get; set; }
        public string restore_time { get; set; }
        public string duration { get; set; }
        public string number_of_customers_affected { get; set; }
        public string type_of_tripping { get; set; }

        public string how_was_the_incident_identified { get; set; }
        public string weather_condition { get; set; }
        public string start_time_from_station { get; set; }
        public string end_journey_at_fault_location { get; set; }
        public string mode_of_transport { get; set; }

        public string road_condition { get; set; }
        public string maintenance_type { get; set; }
        public string work_carried_out { get; set; }
        public string name_of_equipment { get; set; }
        public string causes_of_outage { get; set; }

        public string impact_reason { get; set; }
        public string elaborate_the_cause_of_outages { get; set; }
        public string responsible_person { get; set; }
        public string target_completion_date { get; set; }
        public string status { get; set; }

        public string repair_cost { get; set; }
        public string customer_compensation { get; set; }
        public string area_affected { get; set; }


        //Reliability Indices

        public string total_customer_served_ { get; set; }
        public string sum_of_sustain_customer_interruption_times { get; set; }
        public string sum_of_sustained_customer_interruption_duration_hrs { get; set; }
        public string sum_of_momentary_customer_interruption_times { get; set; }
        public string saifi { get; set; }
        public string saidi { get; set; }
        public string maifi { get; set; }



    }
}
