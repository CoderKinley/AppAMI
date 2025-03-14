using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI.Classes
{
    public class DT
    {
        //Get Dt Informations 
        public string district_name { get; set; }
        public string district_code { get; set; }
        public string esd_name { get; set; }
        public string esd_code { get; set; }

        public string feeder_name { get; set; }
        public string feeder_id { get; set; }
        public string portion_id { get; set; }
        public string root_id { get; set; }


        public string dt_name { get; set; }
        public string dt_id { get; set; }
        public string transformer_serial_number { get; set; }


        public string dt_capacity_kva { get; set; }
        public string rated_voltage_primary { get; set; }
        public string rated_voltage_secondary { get; set; }
        public string ct_primary { get; set; }
        public string ct_secondary { get; set; }
        public string vt_primary { get; set; }
        public string vt_secondary { get; set; }

       
        public string r_phase { get; set; }
        public string y_phase { get; set; }
        public string b_phase { get; set; }
        public string total_customer_count { get; set; }


        public string location { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string elevation { get; set; }


        public string dt_meter_serial_no { get; set; }
        public string mri_serial_no { get; set; }





        //Post Dt Informations 


        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public string EsdName { get; set; }
        public string EsdCode { get; set; }

        public string FeederName { get; set; }
        public string FeederId { get; set; }
        public string PortionId { get; set; }
        public string RootId { get; set; }

        public string DtName { get; set; }
        public string DtId { get; set; }
        public string TransformerSerialNo { get; set; }

        public string DtCapacityKva { get; set; }
        public string RatedVoltagePrimary { get; set; }
        public string RatedVoltageSecondary { get; set; }
        public string CTPrimary { get; set; }
        public string CTSecondary { get; set; }
        public string VTPrimary { get; set; }
        public string VTSecondary { get; set; }

        public string Rphase { get; set; }
        public string Yphase { get; set; }
        public string Bphase { get; set; }
        public string TotalCustomerCount { get; set; }

        public string Location { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Elevation { get; set; }







        //retrieving data for meters  view 


        public string meter_type { get; set; }
        public string meter_firmware_version { get; set; }
        public string meter_manufacturing_year { get; set; }
        public string meter_installation_date { get; set; }
        public string mqtt_topic { get; set; }



        //retrieving data for mri  view 
        public string mri_version { get; set; }
        public string mri_firmware_version { get; set; }
        public string mri_manufacturing_year { get; set; }
        public string mri_installation_date { get; set; }

        public string nms { get; set; }

        public string mri_status { get; set; }





    }
}
