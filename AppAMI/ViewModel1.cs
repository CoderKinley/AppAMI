using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppAMI
{
    public class ViewModel1 : INotifyPropertyChanged
    {
    
        public event PropertyChangedEventHandler PropertyChanged;

        public string VmDtId { get; set; }
        public string VmUserRole { get; set; }

        public void OnMeterNoPropertyChanged(string propertyDtInfo) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyDtInfo));
    }
}
