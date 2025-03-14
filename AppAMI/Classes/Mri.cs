using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI.Classes
{
    public class Mri
    {
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
        public string mri_serial_no { get; set; }
        public string mri_version { get; set; }
       
        public string mri_firmware_version { get; set; }
        public string mri_manufacturing_year { get; set; }
        public string mri_installation_date { get; set; }


        


        //public string DtId { get; set; }
        public string MriSerialNo { get; set; }
        public string MriVersion { get; set; }

        public string MriFirmwareVersion { get; set; }
        public string MriManufacturingYear { get; set; }
        public string MriInstallationDate { get; set; }

    }

}

