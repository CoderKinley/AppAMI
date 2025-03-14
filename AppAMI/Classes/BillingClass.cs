using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI.Classes
{
    public class BillingClass
    {
        

        public string dt_meter_serial_no { get; set; }

        public string date { get; set; }
        public string time { get; set; }


        public string import_active_energy_kwh { get; set; }
        public string import_active_energy_history_1_kwh { get; set; }
        public string export_active_energy_kwh { get; set; }
        public string export_active_energy_history_1_kwh { get; set; }


        public string import_reactive_energy_kvarh { get; set; }
        public string import_reactive_energy_history_1_kvarh { get; set; }
        public string export_reactive_energy_kvarh { get; set; }
        public string export_reactive_energy_history_1_kvarh { get; set; }

        public string import_apparent_energy_kvah { get; set; }
        public string import_apparent_energy_history_1_kvah { get; set; }
        public string export_apparent_energy_kvah { get; set; }
        public string export_apparent_energy_history_1_kvah { get; set; }

        public string total_active_energy_kwh { get; set; }
        public string total_active_energy_history_1_kwh { get; set; }


        public string import_active_maximum_demand_kw { get; set; }
        public string import_active_maximum_demand_history_1_kw { get; set; }
        public string export_active_maximum_demand_kw { get; set; }
        public string export_active_maximum_demand_history_1_kw { get; set; }

        public string import_reactive_maximum_demand_kvar { get; set; }
        public string import_reactive_maximum_demand_history_1_kvar { get; set; }
        public string export_reactive_maximum_demand_kvar { get; set; }
        public string export_reactive_maximum_demand_history_1_kvar { get; set; }


        public string import_apparent_maximum_demand_kva { get; set; }
        public string import_apparent_maximum_demand_history_1_kva { get; set; }
        public string export_apparent_maximum_demand_kva { get; set; }
        public string export_apparent_maximum_demand_history_1_kva { get; set; }




        //InstantAll Historical....................
        public string district_code { get; set; }
        public string esd_name { get; set; }
        public string esd_code { get; set; }
        public string feeder_id { get; set; }
        public string dt_id { get; set; }

        public string record_date { get; set; }
        public string record_time { get; set; }

        public string meter_address { get; set; }
        public string mri_serial_no { get; set; }

        public string current_pa { get; set; }
        public string current_pb { get; set; }
        public string current_pc { get; set; }

        public string voltage_pa { get; set; }
        public string voltage_pb { get; set; }
        public string voltage_pc { get; set; }

        public string power_factor { get; set; }

        public string import_active_power { get; set; }
        public string export_active_power { get; set; }
        public string import_reactive_power { get; set; }
        public string export_reactive_power { get; set; }
        public string import_apparent_power { get; set; }
        public string export_apparent_power { get; set; }

        public string total_active_power { get; set; }
        public string total_reactive_power { get; set; }
        public string total_apparent_power { get; set; }
        public string net_active_power { get; set; }

        public string phase_a_import_active_power { get; set; }
        public string phase_a_export_active_power { get; set; }
        public string phase_a_import_reactive_power { get; set; }
        public string phase_a_export_reactive_power { get; set; }
        public string phase_a_import_apparent_power { get; set; }
        public string phase_a_export_apparent_power { get; set; }

        public string phase_b_import_active_power { get; set; }
        public string phase_b_export_active_power { get; set; }
        public string phase_b_import_reactive_power { get; set; }
        public string phase_b_export_reactive_power { get; set; }
        public string phase_b_import_apparent_power { get; set; }
        public string phase_b_export_apparent_power { get; set; }

        public string phase_c_import_active_power { get; set; }
        public string phase_c_export_active_power { get; set; }
        public string phase_c_import_reactive_power { get; set; }
        public string phase_c_export_reactive_power { get; set; }
        public string phase_c_import_apparent_power { get; set; }
        public string phase_c_export_apparent_power { get; set; }

        public string active_energy { get; set; }
        public string active_energy_history_1 { get; set; }
        public string active_energy_history_2 { get; set; }
       


    }
}
