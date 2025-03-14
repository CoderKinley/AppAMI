using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI.Classes
{
    public  class ReliabilityIndexClass
    {
        public string district_name { get; set; }

        public string district_code { get; set; }
        public string esd_name { get; set; }
        public string esd_code { get; set; }
        public string feeder_id { get; set; }

        public string dt_id { get; set; }
        public string dt_meter_serial_no { get; set; }


        public string trip_date { get; set; }

        public string total_customer_count { get; set; }
        public string sustain_interruption_times { get; set; }

        public string sustain_interruption_duration_hrs { get; set; }
        public string momentary_interruption_times { get; set; }

        public string saifi { get; set; }
        public string saidi { get; set; }
        public string maifi { get; set; }


        public string total_customer_served { get; set; }
        public string sum_of_sustain_customer_interruption_times { get; set; }
        public string sum_of_sustained_customer_interruption_duration_hrs { get; set; }
        public string sum_of_momentary_customer_interruption_times { get; set; }

        public string sum_of_sustained_customer_interruption_times { get; set; }

        

        public string total_sustain_interruption_times { get; set; }
        public string total_sustain_interruption_duration_hrs { get; set; }
        public string total_momentary_interruption_times { get; set; }

        public string month { get; set; }

    }
}
