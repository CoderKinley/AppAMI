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
    /// Interaction logic for InstantaneousSelector.xaml
    /// </summary>
    public partial class InstantaneousSelector : UserControl
    {

        UserControl usc = null;


        string CurrentUserId2;
        string CurrentUserRole2;
        string CurrentUserPassword2;
        string CurrentUserName2;
        string CurrentUserEmployeeId2;

        string DTId2;

        public InstantaneousSelector(string CurrentUserId1, string CurrentUserRole1, string CurrentUserPassword1, string CurrentUserName1, string CurrentUserEmployeeId1, string DTId1)
        {
            InitializeComponent();

            CurrentUserId2 = CurrentUserId1;
            CurrentUserRole2 = CurrentUserRole1;
            CurrentUserPassword2 = CurrentUserPassword1;
            CurrentUserName2 = CurrentUserName1;
            CurrentUserEmployeeId2 = CurrentUserEmployeeId1;
            DTId2 = DTId1;

            GridCursor.Visibility = Visibility.Collapsed;

            GetTypicalUI();
        }

        private void GetTypicalUI()
        {
            GridCursor.Visibility = Visibility.Visible;

            GridInstantParameter.Children.Clear();
            usc = new InstantaneousTypical(CurrentUserId2, CurrentUserRole2, CurrentUserPassword2, CurrentUserName2, CurrentUserEmployeeId2, DTId2);
            GridInstantParameter.Children.Add(usc);
    
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GridCursor.Visibility = Visibility.Visible;

            int index = int.Parse(((Button)e.Source).Uid);

            GridCursor.Margin = new Thickness(0 + (120 * index), 0, 0, 0);

            switch (index)
            {
                case 0:

                    GridInstantParameter.Children.Clear();
                    usc = new InstantaneousTypical( CurrentUserId2,  CurrentUserRole2,  CurrentUserPassword2,  CurrentUserName2,  CurrentUserEmployeeId2,  DTId2);
                    GridInstantParameter.Children.Add(usc);
                    break;


                case 1:
                    GridInstantParameter.Children.Clear();
                    usc = new InstantaneousCustom(CurrentUserId2, CurrentUserRole2, CurrentUserPassword2, CurrentUserName2, CurrentUserEmployeeId2, DTId2);
                    GridInstantParameter.Children.Add(usc);
                    break;

            }

        }
    }
}
