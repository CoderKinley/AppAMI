using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppAMI.MeterData
{
    /// <summary>
    /// Interaction logic for EventLogs.xaml
    /// </summary>
    public partial class EventLogs : UserControl
    {
        UserControl usc = null;
        public EventLogs()
        {
            InitializeComponent();
            GridCursor.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GridCursor.Visibility = Visibility.Visible;

            int index = int.Parse(((Button)e.Source).Uid);

            GridCursor.Margin = new Thickness(0 + (120 * index), 0, 0, 0);

            switch (index)
            {
                case 0:

                    GridLoadSurveyView.Children.Clear();
                    usc = new EventLogsEvents ();
                    GridLoadSurveyView.Children.Add(usc);
                    break;


                case 1:
                    GridLoadSurveyView.Children.Clear();
                    usc = new EventLogsAlarms ();
                    GridLoadSurveyView.Children.Add(usc);
                    break;


            }

        }
    }
}
