using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI.Classes
{
    public class NmsMriClass
    {
        public string district_code { get; set; }
        public string esd_name { get; set; }

        public string esd_code { get; set; }
        public string feeder_id { get; set; }

        public string dt_id { get; set; }

        public string instrumentation_last_received_time { get; set; }

        public string instrumentation_status { get; set; }

        public string load_profile_0_last_received_time { get; set; }

        public string load_profile_0_status { get; set; }

        public string load_profile_1_last_received_time { get; set; }

        public string load_profile_1_status { get; set; }

        public string events_last_received_time { get; set; }

        public string events_status { get; set; }

        public string nms_last_received_time { get; set; }

        public string nms_status { get; set; }

        public string mri_status { get; set; }

        public string last_communication_time { get; set; }


    }
}
