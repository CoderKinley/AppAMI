using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI.Classes
{
    public class UserEventPost
    {


        public string UserID { get; set; }
        public string EmployeeId { get; set; }
        public string UserName { get; set; }

        public string EventLogs { get; set; }
        public string Time { get; set; }
        public string Date { get; set; }

        public string StatusAdmin1 { get; set; }
        public string StatusAdmin2 { get; set; }
        public string StatusAdmin3 { get; set; }

        public string Remarks { get; set; }

        
    }
}
