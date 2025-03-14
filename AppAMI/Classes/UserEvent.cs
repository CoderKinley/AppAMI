using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI.Classes
{
    public class UserEvent
    {



        public string user_id { get; set; }
        public string employee_id { get; set; }
        public string user_name { get; set; }

        public string event_log { get; set; }
        public string date { get; set; }
        public string time { get; set; }

        public string status_admin1 { get; set; }
        public string status_admin2 { get; set; }
        public string status_admin3 { get; set; }

        public string remarks { get; set; }

    }
}
