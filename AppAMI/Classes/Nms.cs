using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI.Classes
{
    public class Nms
    {
        //retrieving data

        public string district_code { get; set; }
        public string esd_name { get; set; }
        public string esd_code { get; set; }
        public string feeder_id { get; set; }

        //public string dt_id { get; set; }
        public string dt_meter_serial_no { get; set; }      
        public string mri_serial_no { get; set; }

        public string date { get; set; }
        public string time { get; set; }

        public string meter_mri_con { get; set; }
        public string sd_card_storage { get; set; }
        public string used_storage { get; set; }

        public string sd_card_percentage { get; set; }
        public string sd_card_read_no { get; set; }
        public string sd_card_write_no { get; set; }

        //public string purge_date { get; set; }
        //public string purge_time { get; set; }
        //public string net_strength { get; set; }


        public string dt_id { get; set; }
        public string net_strength { get; set; }
        public string total_memory_str { get; set; }
        public string used_memory { get; set; }
        public string used_memory_percent { get; set; }
        public string read_write_count { get; set; }
        public string purge_date { get; set; }
        public string purge_time { get; set; }
        public string record_date { get; set; }
        public string record_time { get; set; }




    }
}
