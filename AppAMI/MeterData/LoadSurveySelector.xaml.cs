using System.Windows;
using System.Windows.Controls;

namespace AppAMI.MeterData
{
    /// <summary>
    /// Interaction logic for LoadSurveySelector.xaml
    /// </summary>
    public partial class LoadSurveySelector : UserControl
    {
        UserControl usc = null;
        public LoadSurveySelector()
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
                    usc = new LoadSurveyGraphical();
                    GridLoadSurveyView.Children.Add(usc);
                    break;


                case 1:
                    GridLoadSurveyView.Children.Clear();
                    usc = new LoadSurveyTabular();
                    GridLoadSurveyView.Children.Add(usc);
                    break;


            }

        }
    }
}
