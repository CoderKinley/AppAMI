using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string DtId { get; set; }
        public string MeterNo { get; set; }
        public string MeterPhase { get; set; }

        public string UserId1 { get; set; }
        public string EmployeeId1 { get; set; }
        public string UserName1 { get; set; }
        public string UserRole1 { get; set; }

        public void OnMeterNoPropertyChanged(string propertyMeterNo) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyMeterNo));
    }
}
