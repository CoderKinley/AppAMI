using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AppAMI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY3NTA5OUAzMjMxMmUzMTJlMzMzOGZBWjZOTm8rVlhKeHgxWUNFTW8vZG5vb0ZqQ0pScUwwMFdnMElQanRBT3c9");
        }
    }
}
