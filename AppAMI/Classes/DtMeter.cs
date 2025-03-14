using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI.Classes
{
    public class DtMeter
    {
        public string district_name { get; set; }
        public string district_code { get; set; }
        public string esd_name { get; set; }
        public string esd_code { get; set; }

        public string feeder_name { get; set; }
        public string feeder_id { get; set; }
        public string portion_id { get; set; }
        public string root_id { get; set; }


        public string dt_id { get; set; }
        public string dt_meter_serial_no { get; set; }
        public string meter_type { get; set; }
        public string meter_firmware_version { get; set; }
        public string meter_manufacturing_year { get; set; }
        public string meter_installation_date { get; set; }


        


        //this is for meter data header informations
        public string dt_name { get; set; }
        public string mri_serial_no { get; set; }


        public string DtMeterSerialNo { get; set; }
        public string MeterType { get; set; }
        public string MeterFirmwareVersion { get; set; }
        public string MeterManufacturingYear { get; set; }
        public string MeterInstallationDate { get; set; }
    }
}
