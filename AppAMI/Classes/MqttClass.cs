using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI.Classes
{
    public class MqttClass
    {
        public string district_code { get; set; }
        public string esd_name { get; set; }
        public string esd_code { get; set; }
        public string feeder_id { get; set; }
        public string location { get; set; }

        public string dt_id { get; set; }
        public string firmware_update { get; set; }

        public string dt_meter_serial_no { get; set; }
        public string mri_serial_no { get; set; }
        public string phase { get; set; }


        public string dt_meter_serial_no_mqtt { get; set; }


        public string instantaneous { get; set; }

        public string updateTopic { get; set; }
        public string firmware_status { get; set; }
        public string ping { get; set; }
        public string ping_report { get; set; }

        public string nms { get; set; }




        public string date { get; set; }
        public string time { get; set; }


        public string phase_a_instantaneous_current_a { get; set; }
        public string phase_b_instantaneous_current_a { get; set; }
        public string phase_c_instantaneous_current_a { get; set; }

        public string active_energy { get; set; }

        public string phase_a_instantaneous_voltage_v { get; set; }
        public string phase_b_instantaneous_voltage_v { get; set; }
        public string phase_c_instantaneous_voltage_v { get; set; }

        public string instantaneous_power_factor { get; set; }

        public string instantaneous_import_active_power_kw { get; set; }
        public string instantaneous_export_active_power_kw { get; set; }
        public string instantaneous_import_reactive_power_kvar { get; set; }
        public string instantaneous_export_reactive_power_kvar { get; set; }
        public string instantaneous_import_apparent_power_kva { get; set; }
        public string instantaneous_export_apparent_power_kva { get; set; }


        public string instantaneous_total_active_power_kw { get; set; }
        public string total_reactive_power_kvar { get; set; }
        public string total_apparent_power_kva { get; set; }
        public string instantaneous_net_active_power_kw { get; set; }


        public string phase_a_instantaneous_import_active_power_kw { get; set; }
        public string phase_a_instantaneous_export_active_power_kw { get; set; }
        public string phase_a_instantaneous_import_reactive_power_kvar { get; set; }
        public string phase_a_instantaneous_export_reactive_power_kvar { get; set; }
        public string phase_a_instantaneous_import_apparent_power_kva { get; set; }
        public string phase_a_instantaneous_export_apparent_power_kva { get; set; }

        public string phase_b_instantaneous_import_active_power_kw { get; set; }
        public string phase_b_instantaneous_export_active_power_kw { get; set; }
        public string phase_b_instantaneous_import_reactive_power_kvar { get; set; }
        public string phase_b_instantaneous_export_reactive_power_kvar { get; set; }
        public string phase_b_instantaneous_import_apparent_power_kva { get; set; }
        public string phase_b_instantaneous_export_apparent_power_kva { get; set; }

        public string phase_c_instantaneous_import_active_power_kw { get; set; }
        public string phase_c_instantaneous_export_active_power_kw { get; set; }
        public string phase_c_instantaneous_import_reactive_power_kvar { get; set; }
        public string phase_c_instantaneous_export_reactive_power_kvar { get; set; }
        public string phase_c_instantaneous_import_apparent_power_kva { get; set; }
        public string phase_c_instantaneous_export_apparent_power_kva { get; set; }

        //public string import_active_current_average_demand_kw { get; set; }
        //public string export_active_current_average_demand_kw { get; set; }
        //public string import_reactive_current_average_demand_kvar { get; set; }
        //public string export_reactive_current_average_demand_kvar { get; set; }
        //public string import_apparent_current_average_demand_kva { get; set; }
        //public string export_apparent_current_average_demand_kva { get; set; }


        public string activeEnergyH1 { get; set; }
        public string activeEnergyH2 { get; set; }
        public string firmware_version { get; set; }
    }
}
