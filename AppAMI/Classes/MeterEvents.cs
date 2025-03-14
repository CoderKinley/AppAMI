using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI.Classes
{
    public class MeterEvents
    {
        public string district_code { get; set; }
        public string esd_name { get; set; }
        public string esd_code { get; set; }
        public string feeder_id { get; set; }

        public string dt_id { get; set; }
        public string dt_meter_serial_no { get; set; }

        public string capture_date { get; set; }
        public string capture_time { get; set; }




        //public string error_code { get; set; }

        //public string import_active_energy { get; set; }
        public string export_active_energy { get; set; }
        public string import_reactive_energy { get; set; }
        public string export_reactive_energy { get; set; }

        public string phase_a_instantaneous_voltage { get; set; }
        public string phase_b_instantaneous_voltage { get; set; }
        public string phase_c_instantaneous_voltage { get; set; }

        public string phase_a_instantaneous_current { get; set; }
        public string phase_b_instantaneous_current { get; set; }
        public string phase_c_instantaneous_current { get; set; }

        public string instantaneous_power_factor { get; set; }
        public string phase_a_instantaneous_power_factor { get; set; }
        public string phase_b_instantaneous_power_factor { get; set; }
        public string phase_c_instantaneous_power_factor { get; set; }




        public string record_date { get; set; }
        public string record_time { get; set; }
        public string error_code { get; set; }
        public string import_active_energy { get; set; }
        public string phase_a_current { get; set; }
        public string phase_b_current { get; set; }
        public string phase_c_current { get; set; }
        public string phase_a_voltage { get; set; }
        public string phase_b_voltage { get; set; }
        public string phase_c_voltage { get; set; }






    }
}


