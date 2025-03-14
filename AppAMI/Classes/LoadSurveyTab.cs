using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI.Classes
{
    public class LoadSurveyTab
    {
        public string district_code { get; set; }
        public string esd_name { get; set; }
        public string esd_code { get; set; }
        public string feeder_id { get; set; }

        public string dt_id { get; set; }
        public string dt_meter_serial_no { get; set; }

       
        public string date { get; set; }
        public string time { get; set; }


        //Load profile 0

        public string import_active_energy_kwh { get; set; }
        public string export_active_energy_kwh { get; set; }

        public string import_reactive_energy_kvarh { get; set; }
        public string export_reactive_energy_kvarh { get; set; }

        public string import_apparent_energy_kvah { get; set; }
        public string export_apparent_energy_kvah { get; set; }

        public string total_active_energy_kwh { get; set; }


        public string import_active_maximum_demand_kw { get; set; }
        public string export_active_maximum_demand_kw { get; set; }

        public string import_reactive_maximum_demand_kvar { get; set; }
        public string export_reactive_maximum_demand_kvar { get; set; }

        public string import_apparent_maximum_demand_kva { get; set; }
        public string export_apparent_maximum_demand_kva { get; set; }


        //Load profile 1


        public string phase_a_instantaneous_current_a { get; set; }
        public string phase_b_instantaneous_current_a { get; set; }
        public string phase_c_instantaneous_current_a { get; set; }

        public string phase_a_instantaneous_voltage_v { get; set; }
        public string phase_b_instantaneous_voltage_v { get; set; }
        public string phase_c_instantaneous_voltage_v { get; set; }

        public string instantaneous_power_factor { get; set; }





        public string record_date { get; set; }
        public string record_time { get; set; }

        public string import_active_energy { get; set; }
        public string export_active_energy { get; set; }
        public string import_reactive_energy { get; set; }
        public string export_reactive_energy { get; set; }
        public string import_apparent_energy { get; set; }
        public string export_apparent_energy { get; set; }









    }
}
