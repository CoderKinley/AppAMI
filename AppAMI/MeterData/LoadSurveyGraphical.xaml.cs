using AppAMI.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AppAMI.MeterData
{
    /// <summary>
    /// Interaction logic for LoadSurveyGraphical.xaml
    /// </summary>
    public partial class LoadSurveyGraphical : UserControl
    {
        UserControl usc = null;

        

        DispatcherTimer timer1 = new DispatcherTimer();

        public LoadSurveyGraphical()
        {
            InitializeComponent();
            HideMethod();

            dtPickerStart.DisplayDateEnd = DateTime.Today;
            dtPickerEnd.DisplayDateEnd = DateTime.Today;

            
        }

        private void HideMethod()
        {
            btnGridACompress.Visibility = Visibility.Hidden;
            btnGridBCompress.Visibility = Visibility.Hidden;
            btnGridCCompress.Visibility = Visibility.Hidden;
            btnGridDCompress.Visibility = Visibility.Hidden;
            btnGridECompress.Visibility = Visibility.Hidden;

            GridA.Visibility = Visibility.Hidden;
            GridB.Visibility = Visibility.Hidden;
            GridC.Visibility = Visibility.Hidden;
            GridD.Visibility = Visibility.Hidden;
            GridE.Visibility = Visibility.Hidden;
        }
        #region current     
        private void AllCurrentShowMethod()
        {
            GridA.SetValue(Grid.RowProperty, 0);
            GridA.SetValue(Grid.RowSpanProperty, 3);
            GridA.SetValue(Grid.ColumnProperty, 0);
            GridA.SetValue(Grid.ColumnSpanProperty, 2);

            GridB.SetValue(Grid.RowProperty, 0);
            GridB.SetValue(Grid.RowSpanProperty, 3);
            GridB.SetValue(Grid.ColumnProperty, 2);
            GridB.SetValue(Grid.ColumnSpanProperty, 2);

            GridC.SetValue(Grid.RowProperty, 3);
            GridC.SetValue(Grid.RowSpanProperty, 3);
            GridC.SetValue(Grid.ColumnProperty, 1);
            GridC.SetValue(Grid.ColumnSpanProperty, 2);

            GridA.Visibility = Visibility.Visible;
            GridB.Visibility = Visibility.Visible;
            GridC.Visibility = Visibility.Visible;

            btnGridAExpand.Visibility = Visibility.Visible;
            btnGridBExpand.Visibility = Visibility.Visible;
            btnGridCExpand.Visibility = Visibility.Visible;
            btnGridDExpand.Visibility = Visibility.Visible;
            btnGridEExpand.Visibility = Visibility.Visible;

        }

        private void AllCurrentHideMethod()
        {
            GridA.Visibility = Visibility.Hidden;
            GridB.Visibility = Visibility.Hidden;
            GridC.Visibility = Visibility.Hidden;

        }

        private void CallCurrentMethod()
        {
            if (cbCurrentIR.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 6);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Hidden;

                ChartA.Header = "Current IR";
                btnGridAExpand.Visibility = Visibility.Hidden;

                ChartAA.XBindingPath = "time";
                ChartAA.YBindingPath = "ir_current";

            }

            if (cbCurrentIY.IsChecked == true)
            {
                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 6);
                GridB.SetValue(Grid.ColumnProperty, 0);
                GridB.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Hidden;




                ChartB.Header = "Current IY";
                btnGridBExpand.Visibility = Visibility.Hidden;

                ChartBB.XBindingPath = "time";
                ChartBB.YBindingPath = "iy_current";

            }

            if (cbCurrentIB.IsChecked == true)
            {
                GridC.SetValue(Grid.RowProperty, 0);
                GridC.SetValue(Grid.RowSpanProperty, 6);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Visible;


                ChartC.Header = "Current IB";
                btnGridCExpand.Visibility = Visibility.Hidden;


                ChartCC.XBindingPath = "time";
                ChartCC.YBindingPath = "ib_current";
            }

            if (cbCurrentIR.IsChecked == true && cbCurrentIY.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 4);

                GridB.SetValue(Grid.RowProperty, 3);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 0);
                GridB.SetValue(Grid.ColumnSpanProperty, 4);



                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Hidden;
            }

            if (cbCurrentIR.IsChecked == true && cbCurrentIB.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 4);

                GridC.SetValue(Grid.RowProperty, 3);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 4);



                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Visible;
            }

            if (cbCurrentIY.IsChecked == true && cbCurrentIB.IsChecked == true)
            {
                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 0);
                GridB.SetValue(Grid.ColumnSpanProperty, 4);

                GridC.SetValue(Grid.RowProperty, 3);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 4);



                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Visible;
            }

            if (cbCurrentIY.IsChecked == true && cbCurrentIR.IsChecked == true && cbCurrentIB.IsChecked == true)
            {
                cbCurrentParent.IsChecked = null;
                cbCurrentParent.IsChecked = true;
                cbVoltageParent.IsChecked = false;
                cbEnergyParent.IsChecked = false;

                AllCurrentShowMethod();
            }

            if (cbCurrentIR.IsChecked == false && cbCurrentIY.IsChecked == false && cbCurrentIB.IsChecked == false)
            {
                cbCurrentParent.IsChecked = null;
                cbCurrentParent.IsChecked = false;

                AllCurrentHideMethod();
            }
        }

        private void cbCurrentParent_Checked(object sender, RoutedEventArgs e)
        {
            bool newVal = cbCurrentParent.IsChecked == true;
            cbCurrentIR.IsChecked = newVal;
            cbCurrentIY.IsChecked = newVal;
            cbCurrentIB.IsChecked = newVal;

            ChartA.Header = "Current IR";
            ChartB.Header = "Current IY";
            ChartC.Header = "Current IB";
        }

        private void cbCurrentParent_Unchecked(object sender, RoutedEventArgs e)
        {
            bool newVal = cbCurrentParent.IsChecked == true;
            cbCurrentIR.IsChecked = newVal;
            cbCurrentIY.IsChecked = newVal;
            cbCurrentIB.IsChecked = newVal;
        }

        private void cbCurrentIR_Checked(object sender, RoutedEventArgs e)
        {
            CallCurrentMethod();
        }

        private void cbCurrentIR_Unchecked(object sender, RoutedEventArgs e)
        {
            CallCurrentMethod();
        }

        private void cbCurrentIY_Checked(object sender, RoutedEventArgs e)
        {
            CallCurrentMethod();

        }

        private void cbCurrentIY_Unchecked(object sender, RoutedEventArgs e)
        {
            CallCurrentMethod();
        }

        private void cbCurrentIB_Checked(object sender, RoutedEventArgs e)
        {
            CallCurrentMethod();
        }

        private void cbCurrentIB_Unchecked(object sender, RoutedEventArgs e)
        {
            CallCurrentMethod();
        }

        #endregion current

        #region voltage 

        private void AllVoltageShowMethod()
        {
            GridA.SetValue(Grid.RowProperty, 0);
            GridA.SetValue(Grid.RowSpanProperty, 2);
            GridA.SetValue(Grid.ColumnProperty, 0);
            GridA.SetValue(Grid.ColumnSpanProperty, 2);

            GridB.SetValue(Grid.RowProperty, 0);
            GridB.SetValue(Grid.RowSpanProperty, 2);
            GridB.SetValue(Grid.ColumnProperty, 2);
            GridB.SetValue(Grid.ColumnSpanProperty, 2);

            GridC.SetValue(Grid.RowProperty, 2);
            GridC.SetValue(Grid.RowSpanProperty, 2);
            GridC.SetValue(Grid.ColumnProperty, 0);
            GridC.SetValue(Grid.ColumnSpanProperty, 2);

            GridD.SetValue(Grid.RowProperty, 2);
            GridD.SetValue(Grid.RowSpanProperty, 2);
            GridD.SetValue(Grid.ColumnProperty, 2);
            GridD.SetValue(Grid.ColumnSpanProperty, 2);

            GridE.SetValue(Grid.RowProperty, 4);
            GridE.SetValue(Grid.RowSpanProperty, 2);
            GridE.SetValue(Grid.ColumnProperty, 1);
            GridE.SetValue(Grid.ColumnSpanProperty, 2);

            GridA.Visibility = Visibility.Visible;
            GridB.Visibility = Visibility.Visible;
            GridC.Visibility = Visibility.Visible;
            GridD.Visibility = Visibility.Visible;
            GridE.Visibility = Visibility.Visible;

            btnGridAExpand.Visibility = Visibility.Visible;
            btnGridBExpand.Visibility = Visibility.Visible;
            btnGridCExpand.Visibility = Visibility.Visible;
            btnGridDExpand.Visibility = Visibility.Visible;
            btnGridEExpand.Visibility = Visibility.Visible;

        }
        private void AllVoltageHideMethod()
        {
            GridA.Visibility = Visibility.Hidden;
            GridB.Visibility = Visibility.Hidden;
            GridC.Visibility = Visibility.Hidden;
            GridD.Visibility = Visibility.Hidden;
            GridE.Visibility = Visibility.Hidden;
        }
        private void CallVoltageMethod()
        {
            #region  1 Voltage
            if (cbVoltageVRN.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 6);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Hidden;



                ChartA.Header = "Voltage VRN";
                btnGridAExpand.Visibility = Visibility.Hidden;

                ChartAA.XBindingPath = "time";
                ChartAA.YBindingPath = "ir_voltage";
            }

            if (cbVoltageVYN.IsChecked == true)
            {
                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 6);
                GridB.SetValue(Grid.ColumnProperty, 0);
                GridB.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Hidden;


                ChartB.Header = "Voltage VYN";
                btnGridBExpand.Visibility = Visibility.Hidden;

                ChartBB.XBindingPath = "time";
                ChartBB.YBindingPath = "iy_voltage";

            }

            if (cbVoltageVBN.IsChecked == true)
            {
                GridC.SetValue(Grid.RowProperty, 0);
                GridC.SetValue(Grid.RowSpanProperty, 6);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Hidden;



                ChartC.Header = "Voltage VBN";
                btnGridCExpand.Visibility = Visibility.Hidden;

                ChartCC.XBindingPath = "time";
                ChartCC.YBindingPath = "ib_voltage";
            }

            if (cbVoltageVRY.IsChecked == true)
            {
                GridD.SetValue(Grid.RowProperty, 0);
                GridD.SetValue(Grid.RowSpanProperty, 6);
                GridD.SetValue(Grid.ColumnProperty, 0);
                GridD.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Visible;
                GridE.Visibility = Visibility.Hidden;



                ChartD.Header = "Voltage VRY";
                btnGridDExpand.Visibility = Visibility.Hidden;

                ChartDD.XBindingPath = "drptwo_time";
                ChartDD.YBindingPath = "drptwo_soil_temp";

            }

            if (cbVolatgeVBY.IsChecked == true)
            {
                GridE.SetValue(Grid.RowProperty, 0);
                GridE.SetValue(Grid.RowSpanProperty, 6);
                GridE.SetValue(Grid.ColumnProperty, 0);
                GridE.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Visible;



                ChartE.Header = "Voltage VBY";
                btnGridEExpand.Visibility = Visibility.Hidden;

                ChartEE.XBindingPath = "drptwo_time";
                ChartEE.YBindingPath = "drptwo_tank_level";
            }
            #endregion 1 Voltage

            #region 2 Voltage
            if (cbVoltageVRN.IsChecked == true && cbVoltageVYN.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 4);

                GridB.SetValue(Grid.RowProperty, 3);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 0);
                GridB.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Hidden;
            }

            if (cbVoltageVRN.IsChecked == true && cbVoltageVBN.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 4);

                GridC.SetValue(Grid.RowProperty, 3);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Hidden;
            }

            if (cbVoltageVRN.IsChecked == true && cbVoltageVRY.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 4);

                GridD.SetValue(Grid.RowProperty, 3);
                GridD.SetValue(Grid.RowSpanProperty, 3);
                GridD.SetValue(Grid.ColumnProperty, 0);
                GridD.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Visible;
                GridE.Visibility = Visibility.Hidden;
            }

            if (cbVoltageVRN.IsChecked == true && cbVolatgeVBY.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 4);

                GridE.SetValue(Grid.RowProperty, 3);
                GridE.SetValue(Grid.RowSpanProperty, 3);
                GridE.SetValue(Grid.ColumnProperty, 0);
                GridE.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Visible;
            }

            if (cbVoltageVYN.IsChecked == true && cbVoltageVBN.IsChecked == true)
            {
                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 0);
                GridB.SetValue(Grid.ColumnSpanProperty, 4);

                GridC.SetValue(Grid.RowProperty, 3);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Hidden;
            }

            if (cbVoltageVYN.IsChecked == true && cbVoltageVRY.IsChecked == true)
            {
                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 0);
                GridB.SetValue(Grid.ColumnSpanProperty, 4);

                GridD.SetValue(Grid.RowProperty, 3);
                GridD.SetValue(Grid.RowSpanProperty, 3);
                GridD.SetValue(Grid.ColumnProperty, 0);
                GridD.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Visible;
                GridE.Visibility = Visibility.Hidden;
            }

            if (cbVoltageVYN.IsChecked == true && cbVolatgeVBY.IsChecked == true)
            {
                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 0);
                GridB.SetValue(Grid.ColumnSpanProperty, 4);

                GridE.SetValue(Grid.RowProperty, 3);
                GridE.SetValue(Grid.RowSpanProperty, 3);
                GridE.SetValue(Grid.ColumnProperty, 0);
                GridE.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Visible;
            }

            if (cbVoltageVBN.IsChecked == true && cbVoltageVRY.IsChecked == true)
            {
                GridC.SetValue(Grid.RowProperty, 0);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 4);

                GridD.SetValue(Grid.RowProperty, 3);
                GridD.SetValue(Grid.RowSpanProperty, 3);
                GridD.SetValue(Grid.ColumnProperty, 0);
                GridD.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Visible;
                GridE.Visibility = Visibility.Hidden;
            }

            if (cbVoltageVBN.IsChecked == true && cbVolatgeVBY.IsChecked == true)
            {
                GridC.SetValue(Grid.RowProperty, 0);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 4);

                GridE.SetValue(Grid.RowProperty, 3);
                GridE.SetValue(Grid.RowSpanProperty, 3);
                GridE.SetValue(Grid.ColumnProperty, 0);
                GridE.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Visible;
            }

            if (cbVoltageVRY.IsChecked == true && cbVolatgeVBY.IsChecked == true)
            {
                GridD.SetValue(Grid.RowProperty, 0);
                GridD.SetValue(Grid.RowSpanProperty, 3);
                GridD.SetValue(Grid.ColumnProperty, 0);
                GridD.SetValue(Grid.ColumnSpanProperty, 4);

                GridE.SetValue(Grid.RowProperty, 3);
                GridE.SetValue(Grid.RowSpanProperty, 3);
                GridE.SetValue(Grid.ColumnProperty, 0);
                GridE.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Visible;
                GridE.Visibility = Visibility.Visible;
            }

            #endregion 2 Voltage

            #region 3 Voltage
            if (cbVoltageVRN.IsChecked == true && cbVoltageVYN.IsChecked == true && cbVoltageVBN.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 2);

                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 2);
                GridB.SetValue(Grid.ColumnSpanProperty, 2);

                GridC.SetValue(Grid.RowProperty, 3);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 1);
                GridC.SetValue(Grid.ColumnSpanProperty, 2);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Hidden;
            }

            if (cbVoltageVRN.IsChecked == true && cbVoltageVYN.IsChecked == true && cbVoltageVRY.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 2);

                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 2);
                GridB.SetValue(Grid.ColumnSpanProperty, 2);

                GridD.SetValue(Grid.RowProperty, 3);
                GridD.SetValue(Grid.RowSpanProperty, 3);
                GridD.SetValue(Grid.ColumnProperty, 1);
                GridD.SetValue(Grid.ColumnSpanProperty, 2);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Visible;
                GridE.Visibility = Visibility.Hidden;
            }

            if (cbVoltageVRN.IsChecked == true && cbVoltageVYN.IsChecked == true && cbVolatgeVBY.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 2);

                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 2);
                GridB.SetValue(Grid.ColumnSpanProperty, 2);

                GridE.SetValue(Grid.RowProperty, 3);
                GridE.SetValue(Grid.RowSpanProperty, 3);
                GridE.SetValue(Grid.ColumnProperty, 1);
                GridE.SetValue(Grid.ColumnSpanProperty, 2);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Visible;
            }

            if (cbVoltageVYN.IsChecked == true && cbVoltageVBN.IsChecked == true && cbVoltageVRY.IsChecked == true)
            {
                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 2);
                GridB.SetValue(Grid.ColumnSpanProperty, 2);

                GridC.SetValue(Grid.RowProperty, 0);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 2);

                GridD.SetValue(Grid.RowProperty, 3);
                GridD.SetValue(Grid.RowSpanProperty, 3);
                GridD.SetValue(Grid.ColumnProperty, 1);
                GridD.SetValue(Grid.ColumnSpanProperty, 2);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Visible;
                GridE.Visibility = Visibility.Hidden;
            }

            if (cbVoltageVYN.IsChecked == true && cbVoltageVBN.IsChecked == true && cbVolatgeVBY.IsChecked == true)
            {
                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 2);
                GridB.SetValue(Grid.ColumnSpanProperty, 2);

                GridC.SetValue(Grid.RowProperty, 0);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 2);

                GridE.SetValue(Grid.RowProperty, 3);
                GridE.SetValue(Grid.RowSpanProperty, 3);
                GridE.SetValue(Grid.ColumnProperty, 1);
                GridE.SetValue(Grid.ColumnSpanProperty, 2);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Visible;
            }

            if (cbVoltageVBN.IsChecked == true && cbVoltageVRY.IsChecked == true && cbVolatgeVBY.IsChecked == true)
            {

                GridC.SetValue(Grid.RowProperty, 0);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 2);

                GridD.SetValue(Grid.RowProperty, 0);
                GridD.SetValue(Grid.RowSpanProperty, 3);
                GridD.SetValue(Grid.ColumnProperty, 2);
                GridD.SetValue(Grid.ColumnSpanProperty, 2);

                GridE.SetValue(Grid.RowProperty, 3);
                GridE.SetValue(Grid.RowSpanProperty, 3);
                GridE.SetValue(Grid.ColumnProperty, 1);
                GridE.SetValue(Grid.ColumnSpanProperty, 2);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Visible;
                GridE.Visibility = Visibility.Visible;
            }

            #endregion 3 Voltage

            #region 4 Voltage
            if (cbVoltageVRN.IsChecked == true && cbVoltageVYN.IsChecked == true && cbVoltageVBN.IsChecked == true && cbVoltageVRY.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 2);

                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 2);
                GridB.SetValue(Grid.ColumnSpanProperty, 2);

                GridC.SetValue(Grid.RowProperty, 3);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 2);

                GridD.SetValue(Grid.RowProperty, 3);
                GridD.SetValue(Grid.RowSpanProperty, 3);
                GridD.SetValue(Grid.ColumnProperty, 2);
                GridD.SetValue(Grid.ColumnSpanProperty, 2);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Visible;
                GridE.Visibility = Visibility.Hidden;
            }

            if (cbVoltageVRN.IsChecked == true && cbVoltageVYN.IsChecked == true && cbVoltageVBN.IsChecked == true && cbVolatgeVBY.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 2);

                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 2);
                GridB.SetValue(Grid.ColumnSpanProperty, 2);

                GridC.SetValue(Grid.RowProperty, 3);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 2);

                GridE.SetValue(Grid.RowProperty, 3);
                GridE.SetValue(Grid.RowSpanProperty, 3);
                GridE.SetValue(Grid.ColumnProperty, 2);
                GridE.SetValue(Grid.ColumnSpanProperty, 2);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Visible;
            }

            if (cbVoltageVYN.IsChecked == true && cbVoltageVBN.IsChecked == true && cbVoltageVRY.IsChecked == true && cbVolatgeVBY.IsChecked == true)
            {
                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 2);
                GridB.SetValue(Grid.ColumnSpanProperty, 2);

                GridC.SetValue(Grid.RowProperty, 3);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 2);

                GridD.SetValue(Grid.RowProperty, 0);
                GridD.SetValue(Grid.RowSpanProperty, 3);
                GridD.SetValue(Grid.ColumnProperty, 0);
                GridD.SetValue(Grid.ColumnSpanProperty, 2);

                GridE.SetValue(Grid.RowProperty, 3);
                GridE.SetValue(Grid.RowSpanProperty, 3);
                GridE.SetValue(Grid.ColumnProperty, 2);
                GridE.SetValue(Grid.ColumnSpanProperty, 2);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Visible;
                GridE.Visibility = Visibility.Visible;
            }

            #endregion 4 Voltage

            #region All Voltage
            if (cbVoltageVRN.IsChecked == true && cbVoltageVYN.IsChecked == true && cbVoltageVBN.IsChecked == true && cbVoltageVRY.IsChecked == true && cbVolatgeVBY.IsChecked == true)
            {
                cbVoltageParent.IsChecked = null;
                cbVoltageParent.IsChecked = true;
                cbCurrentParent.IsChecked = false;
                cbEnergyParent.IsChecked = false;

                AllVoltageShowMethod();
            }

            if (cbVoltageVRN.IsChecked == false && cbVoltageVYN.IsChecked == false && cbVoltageVBN.IsChecked == false && cbVoltageVRY.IsChecked == false && cbVolatgeVBY.IsChecked == false)
            {
                cbVoltageParent.IsChecked = null;
                cbVoltageParent.IsChecked = false;

                AllVoltageHideMethod();
            }

            #endregion All Voltage

        }


        private void cbVoltageParent_Checked(object sender, RoutedEventArgs e)
        {
            bool newVal = cbVoltageParent.IsChecked == true;
            cbVoltageVRN.IsChecked = newVal;
            cbVoltageVYN.IsChecked = newVal;
            cbVoltageVBN.IsChecked = newVal;
            cbVoltageVRY.IsChecked = newVal;
            cbVolatgeVBY.IsChecked = newVal;

            ChartA.Header = "Voltage VRN";
            ChartB.Header = "Voltage VYN";
            ChartC.Header = "Voltage VBN";
            ChartD.Header = "Voltage VRY";
            ChartE.Header = "Voltage VBY";
        }
        private void cbVoltageParent_Unchecked(object sender, RoutedEventArgs e)
        {
            bool newVal = cbVoltageParent.IsChecked == true;

            cbVoltageVRN.IsChecked = newVal;
            cbVoltageVYN.IsChecked = newVal;
            cbVoltageVBN.IsChecked = newVal;
            cbVoltageVRY.IsChecked = newVal;
            cbVolatgeVBY.IsChecked = newVal;
        }
        private void cbVoltageVRN_Checked(object sender, RoutedEventArgs e)
        {
            CallVoltageMethod();
        }
        private void cbVoltageVRN_Unchecked(object sender, RoutedEventArgs e)
        {
            CallVoltageMethod();
        }
        private void cbVoltageVYN_Checked(object sender, RoutedEventArgs e)
        {
            CallVoltageMethod();
        }
        private void cbVoltageVYN_Unchecked(object sender, RoutedEventArgs e)
        {
            CallVoltageMethod();
        }
        private void cbVoltageVBN_Checked(object sender, RoutedEventArgs e)
        {
            CallVoltageMethod();
        }
        private void cbVoltageVBN_Unchecked(object sender, RoutedEventArgs e)
        {
            CallVoltageMethod();
        }
        private void cbVoltageVRY_Checked(object sender, RoutedEventArgs e)
        {
            CallVoltageMethod();
        }
        private void cbVoltageVRY_Unchecked(object sender, RoutedEventArgs e)
        {
            CallVoltageMethod();
        }
        private void cbVolatgeVBY_Checked(object sender, RoutedEventArgs e)
        {
            CallVoltageMethod();
        }
        private void cbVolatgeVBY_Unchecked(object sender, RoutedEventArgs e)
        {
            CallVoltageMethod();
        }

        #endregion voltage 


        #region Energy
        private void AllEnergyShowMethod()
        {
            GridA.SetValue(Grid.RowProperty, 0);
            GridA.SetValue(Grid.RowSpanProperty, 3);
            GridA.SetValue(Grid.ColumnProperty, 0);
            GridA.SetValue(Grid.ColumnSpanProperty, 2);

            GridB.SetValue(Grid.RowProperty, 0);
            GridB.SetValue(Grid.RowSpanProperty, 3);
            GridB.SetValue(Grid.ColumnProperty, 2);
            GridB.SetValue(Grid.ColumnSpanProperty, 2);

            GridC.SetValue(Grid.RowProperty, 3);
            GridC.SetValue(Grid.RowSpanProperty, 3);
            GridC.SetValue(Grid.ColumnProperty, 0);
            GridC.SetValue(Grid.ColumnSpanProperty, 2);

            GridD.SetValue(Grid.RowProperty, 3);
            GridD.SetValue(Grid.RowSpanProperty, 3);
            GridD.SetValue(Grid.ColumnProperty, 2);
            GridD.SetValue(Grid.ColumnSpanProperty, 2);

            GridA.Visibility = Visibility.Visible;
            GridB.Visibility = Visibility.Visible;
            GridC.Visibility = Visibility.Visible;
            GridD.Visibility = Visibility.Visible;

            btnGridAExpand.Visibility = Visibility.Visible;
            btnGridBExpand.Visibility = Visibility.Visible;
            btnGridCExpand.Visibility = Visibility.Visible;
            btnGridDExpand.Visibility = Visibility.Visible;
            btnGridEExpand.Visibility = Visibility.Visible;

        }
        private void AllEnergyHideMethod()
        {
            GridA.Visibility = Visibility.Hidden;
            GridB.Visibility = Visibility.Hidden;
            GridC.Visibility = Visibility.Hidden;
            GridD.Visibility = Visibility.Hidden;
        }
        private void CallEnergyMethod()
        {
            #region 1 Energy
            if (cbBlocknEnergykWh.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 6);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Hidden;



                ChartA.Header = "Block energy, in kWh";
                btnGridAExpand.Visibility = Visibility.Hidden;

                ChartAA.XBindingPath = "time";
                ChartAA.YBindingPath = "energy";

            }

            if (cbBlockEnergykvarhLag.IsChecked == true)
            {
                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 6);
                GridB.SetValue(Grid.ColumnProperty, 0);
                GridB.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Hidden;



                ChartB.Header = "Block energy, in kvarh (lag)";
                btnGridBExpand.Visibility = Visibility.Hidden;

                ChartBB.XBindingPath = "drptwo_time";
                ChartBB.YBindingPath = "drptwo_soil_moist";

            }

            if (cbBlockEnergykvarhLead.IsChecked == true)
            {
                GridC.SetValue(Grid.RowProperty, 0);
                GridC.SetValue(Grid.RowSpanProperty, 6);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Hidden;



                ChartC.Header = "Block energy, in kvarh (lead)";
                btnGridCExpand.Visibility = Visibility.Hidden;

                ChartCC.XBindingPath = "drptwo_time";
                ChartCC.YBindingPath = "drptwo_ec";
            }

            if (cbBlockEnergykVAh.IsChecked == true)
            {
                GridD.SetValue(Grid.RowProperty, 0);
                GridD.SetValue(Grid.RowSpanProperty, 6);
                GridD.SetValue(Grid.ColumnProperty, 0);
                GridD.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Visible;



                ChartD.Header = "Block energy, in kVAh";
                btnGridDExpand.Visibility = Visibility.Hidden;

                ChartDD.XBindingPath = "drptwo_time";
                ChartDD.YBindingPath = "drptwo_soil_temp";
            }

            #endregion 1 Energy

            #region 2 Energy
            if (cbBlocknEnergykWh.IsChecked == true && cbBlockEnergykvarhLag.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 4);

                GridB.SetValue(Grid.RowProperty, 3);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 0);
                GridB.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Hidden;
                GridE.Visibility = Visibility.Hidden;
            }

            if (cbBlocknEnergykWh.IsChecked == true && cbBlockEnergykvarhLead.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 4);

                GridC.SetValue(Grid.RowProperty, 3);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Hidden;
            }

            if (cbBlocknEnergykWh.IsChecked == true && cbBlockEnergykVAh.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 4);

                GridD.SetValue(Grid.RowProperty, 3);
                GridD.SetValue(Grid.RowSpanProperty, 3);
                GridD.SetValue(Grid.ColumnProperty, 0);
                GridD.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Visible;

            }

            if (cbBlockEnergykvarhLag.IsChecked == true && cbBlockEnergykvarhLead.IsChecked == true)
            {
                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 0);
                GridB.SetValue(Grid.ColumnSpanProperty, 4);

                GridC.SetValue(Grid.RowProperty, 3);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Hidden;

            }

            if (cbBlockEnergykvarhLag.IsChecked == true && cbBlockEnergykVAh.IsChecked == true)
            {
                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 0);
                GridB.SetValue(Grid.ColumnSpanProperty, 4);

                GridD.SetValue(Grid.RowProperty, 3);
                GridD.SetValue(Grid.RowSpanProperty, 3);
                GridD.SetValue(Grid.ColumnProperty, 0);
                GridD.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Visible;

            }

            if (cbBlockEnergykvarhLead.IsChecked == true && cbBlockEnergykVAh.IsChecked == true)
            {
                GridC.SetValue(Grid.RowProperty, 0);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 4);

                GridD.SetValue(Grid.RowProperty, 3);
                GridD.SetValue(Grid.RowSpanProperty, 3);
                GridD.SetValue(Grid.ColumnProperty, 0);
                GridD.SetValue(Grid.ColumnSpanProperty, 4);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Hidden;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Visible;

            }

            #endregion 2 Energy

            #region 3 Energy
            if (cbBlocknEnergykWh.IsChecked == true && cbBlockEnergykvarhLag.IsChecked == true && cbBlockEnergykvarhLead.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 2);

                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 2);
                GridB.SetValue(Grid.ColumnSpanProperty, 2);

                GridC.SetValue(Grid.RowProperty, 3);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 1);
                GridC.SetValue(Grid.ColumnSpanProperty, 2);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Hidden;

            }

            if (cbBlocknEnergykWh.IsChecked == true && cbBlockEnergykvarhLag.IsChecked == true && cbBlockEnergykVAh.IsChecked == true)
            {
                GridA.SetValue(Grid.RowProperty, 0);
                GridA.SetValue(Grid.RowSpanProperty, 3);
                GridA.SetValue(Grid.ColumnProperty, 0);
                GridA.SetValue(Grid.ColumnSpanProperty, 2);

                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 2);
                GridB.SetValue(Grid.ColumnSpanProperty, 2);

                GridD.SetValue(Grid.RowProperty, 3);
                GridD.SetValue(Grid.RowSpanProperty, 3);
                GridD.SetValue(Grid.ColumnProperty, 1);
                GridD.SetValue(Grid.ColumnSpanProperty, 2);

                GridA.Visibility = Visibility.Visible;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Hidden;
                GridD.Visibility = Visibility.Visible;

            }

            if (cbBlockEnergykvarhLag.IsChecked == true && cbBlockEnergykvarhLead.IsChecked == true && cbBlockEnergykVAh.IsChecked == true)
            {

                GridB.SetValue(Grid.RowProperty, 0);
                GridB.SetValue(Grid.RowSpanProperty, 3);
                GridB.SetValue(Grid.ColumnProperty, 2);
                GridB.SetValue(Grid.ColumnSpanProperty, 2);

                GridC.SetValue(Grid.RowProperty, 0);
                GridC.SetValue(Grid.RowSpanProperty, 3);
                GridC.SetValue(Grid.ColumnProperty, 0);
                GridC.SetValue(Grid.ColumnSpanProperty, 2);

                GridD.SetValue(Grid.RowProperty, 3);
                GridD.SetValue(Grid.RowSpanProperty, 3);
                GridD.SetValue(Grid.ColumnProperty, 1);
                GridD.SetValue(Grid.ColumnSpanProperty, 2);

                GridA.Visibility = Visibility.Hidden;
                GridB.Visibility = Visibility.Visible;
                GridC.Visibility = Visibility.Visible;
                GridD.Visibility = Visibility.Visible;

            }


            #endregion 3 Energy

            #region All Energy

            if (cbBlocknEnergykWh.IsChecked == true && cbBlockEnergykvarhLag.IsChecked == true && cbBlockEnergykvarhLead.IsChecked == true && cbBlockEnergykVAh.IsChecked == true)
            {
                cbEnergyParent.IsChecked = null;
                cbEnergyParent.IsChecked = true;
                cbCurrentParent.IsChecked = false;
                cbVoltageParent.IsChecked = false;

                AllEnergyShowMethod();
            }


            if (cbBlocknEnergykWh.IsChecked == false && cbBlockEnergykvarhLag.IsChecked == false && cbBlockEnergykvarhLead.IsChecked == false && cbBlockEnergykVAh.IsChecked == false)
            {
                cbEnergyParent.IsChecked = null;
                cbEnergyParent.IsChecked = false;

                AllEnergyHideMethod();
            }


            #endregion All Energy

        }


        private void cbEnergyParent_Checked(object sender, RoutedEventArgs e)
        {
            bool newVal = cbEnergyParent.IsChecked == true;
            cbBlocknEnergykWh.IsChecked = newVal;
            cbBlockEnergykvarhLag.IsChecked = newVal;
            cbBlockEnergykvarhLead.IsChecked = newVal;
            cbBlockEnergykVAh.IsChecked = newVal;

            ChartA.Header = "Block energy, in kWh";
            ChartB.Header = "Block energy, in kvarh (lag)";
            ChartC.Header = "Block energy, in kvarh (lead)";
            ChartD.Header = "Block energy, in kVAh";
        }
        private void cbEnergyParent_Unchecked(object sender, RoutedEventArgs e)
        {
            bool newVal = cbEnergyParent.IsChecked == true;
            cbBlocknEnergykWh.IsChecked = newVal;
            cbBlockEnergykvarhLag.IsChecked = newVal;
            cbBlockEnergykvarhLead.IsChecked = newVal;
            cbBlockEnergykVAh.IsChecked = newVal;
        }
        private void cbBlocknEnergykWh_Checked(object sender, RoutedEventArgs e)
        {
            CallEnergyMethod();
        }
        private void cbBlocknEnergykWh_Unchecked(object sender, RoutedEventArgs e)
        {
            CallEnergyMethod();
        }
        private void cbBlockEnergykvarhLag_Checked(object sender, RoutedEventArgs e)
        {
            CallEnergyMethod();
        }
        private void cbBlockEnergykvarhLag_Unchecked(object sender, RoutedEventArgs e)
        {
            CallEnergyMethod();
        }
        private void cbBlockEnergykvarhLead_Checked(object sender, RoutedEventArgs e)
        {
            CallEnergyMethod();
        }
        private void cbBlockEnergykvarhLead_Unchecked(object sender, RoutedEventArgs e)
        {
            CallEnergyMethod();
        }
        private void cbBlockEnergykVAh_Checked(object sender, RoutedEventArgs e)
        {
            CallEnergyMethod();
        }
        private void cbBlockEnergykVAh_Unchecked(object sender, RoutedEventArgs e)
        {
            CallEnergyMethod();
        }


        #endregion Energy


        #region WebClient


        private void dtPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {


            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy"; //for the second type
            Thread.CurrentThread.CurrentCulture = ci;

        }

        private void dtPickerEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy"; //for the second type
            Thread.CurrentThread.CurrentCulture = ci;

            WebClientCurrent();
            WebClientVoltage();
            WebClientEnergykWh();
        }

        private void WebClientCurrent()
        {

            try
            {
                using (WebClient web = new WebClient())
                {
                    string url = string.Format("http://119.2.119.202:3500/dtmeter/instantenous/data");
                    string json = web.DownloadString(url);

                    List<LoadSurveyTab> all_data = JsonConvert.DeserializeObject<List<LoadSurveyTab>>(json);

                    List<LoadSurveyTab> filteredDtId = all_data.Where(a => a.dt_id == txtDtId.Text).ToList();

                    // Filter data based on date range
                    List<LoadSurveyTab> filtered_data = filteredDtId
                        .Where(x => DateTime.ParseExact(x.date, "dd-MM-yyyy", CultureInfo.InvariantCulture) >= dtPickerStart.SelectedDate.Value
                            && DateTime.ParseExact(x.date, "dd-MM-yyyy", CultureInfo.InvariantCulture) <= dtPickerEnd.SelectedDate.Value)
                        .ToList();

                    // Bind filtered data to DataGrid
                    ChartAA.ItemsSource = filtered_data;
                    ChartBB.ItemsSource = filtered_data;
                    ChartCC.ItemsSource = filtered_data;

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Unable to Connect to the Server" + ex.Message);

            }


           

        }

        private void WebClientVoltage()
        {
            using (WebClient web = new WebClient())
            {
                string url = string.Format("http://119.2.119.202:3500/dtmeter/instantenous/data");
                string json = web.DownloadString(url);

                List<LoadSurveyTab> all_data = JsonConvert.DeserializeObject<List<LoadSurveyTab>>(json);

                List<LoadSurveyTab> filteredDtId = all_data.Where(a => a.dt_id == txtDtId.Text).ToList();

                // Filter data based on date range
                List<LoadSurveyTab> filtered_data = filteredDtId
                    .Where(x => DateTime.ParseExact(x.date, "dd-MM-yyyy", CultureInfo.InvariantCulture) >= dtPickerStart.SelectedDate.Value
                        && DateTime.ParseExact(x.date, "dd-MM-yyyy", CultureInfo.InvariantCulture) <= dtPickerEnd.SelectedDate.Value)
                    .ToList();

                // Bind filtered data to DataGrid


                ChartAA.ItemsSource = filtered_data;
                ChartBB.ItemsSource = filtered_data;
                ChartCC.ItemsSource = filtered_data;
                ChartDD.ItemsSource = filtered_data;
                ChartEE.ItemsSource = filtered_data;
            }


            


        }

        private void WebClientEnergykWh()
        {
            using (WebClient web = new WebClient())
            {
                string url = string.Format("http://119.2.119.202:3500/dtmeter/instantenous/data");
                string json = web.DownloadString(url);

                List<LoadSurveyTab> all_data = JsonConvert.DeserializeObject<List<LoadSurveyTab>>(json);

                List<LoadSurveyTab> filteredDtId = all_data.Where(a => a.dt_id == txtDtId.Text).ToList();

                // Filter data based on date range
                List<LoadSurveyTab> filtered_data = filteredDtId
                    .Where(x => DateTime.ParseExact(x.date, "dd-MM-yyyy", CultureInfo.InvariantCulture) >= dtPickerStart.SelectedDate.Value
                        && DateTime.ParseExact(x.date, "dd-MM-yyyy", CultureInfo.InvariantCulture) <= dtPickerEnd.SelectedDate.Value)
                    .ToList();

                // Bind filtered data to DataGrid



                ChartAA.ItemsSource = filtered_data;
                ChartBB.ItemsSource = filtered_data;
                ChartCC.ItemsSource = filtered_data;
                ChartDD.ItemsSource = filtered_data;

            }







        }

        #endregion WebClient


        #region Button Control for Grid
        private void btnGridAExpand_Click(object sender, RoutedEventArgs e)
        {
            btnGridAExpand.Visibility = Visibility.Hidden;
            btnGridACompress.Visibility = Visibility.Visible;

            GridA.SetValue(Grid.RowProperty, 0);
            GridA.SetValue(Grid.RowSpanProperty, 6);
            GridA.SetValue(Grid.ColumnProperty, 0);
            GridA.SetValue(Grid.ColumnSpanProperty, 4);

            GridA.Visibility = Visibility.Visible;
            GridB.Visibility = Visibility.Hidden;
            GridC.Visibility = Visibility.Hidden;
            GridD.Visibility = Visibility.Hidden;
            GridE.Visibility = Visibility.Hidden;

        }

        private void btnGridACompress_Click(object sender, RoutedEventArgs e)
        {
            btnGridACompress.Visibility = Visibility.Hidden;
            btnGridAExpand.Visibility = Visibility.Visible;



            if (cbVoltageParent.IsChecked == true)
            {
                CallVoltageMethod();
            }

            if (cbCurrentParent.IsChecked == true)
            {
                CallCurrentMethod();
            }

            if (cbEnergyParent.IsChecked == true)
            {
                CallEnergyMethod();
            }

        }

        private void btnGridBExpand_Click(object sender, RoutedEventArgs e)
        {
            btnGridBExpand.Visibility = Visibility.Hidden;
            btnGridBCompress.Visibility = Visibility.Visible;

            GridB.SetValue(Grid.RowProperty, 0);
            GridB.SetValue(Grid.RowSpanProperty, 6);
            GridB.SetValue(Grid.ColumnProperty, 0);
            GridB.SetValue(Grid.ColumnSpanProperty, 4);

            GridA.Visibility = Visibility.Hidden;
            GridB.Visibility = Visibility.Visible;
            GridC.Visibility = Visibility.Hidden;
            GridD.Visibility = Visibility.Hidden;
            GridE.Visibility = Visibility.Hidden;
        }

        private void btnGridBCompress_Click(object sender, RoutedEventArgs e)
        {
            btnGridBCompress.Visibility = Visibility.Hidden;
            btnGridBExpand.Visibility = Visibility.Visible;

            if (cbVoltageParent.IsChecked == true)
            {
                CallVoltageMethod();
            }

            if (cbCurrentParent.IsChecked == true)
            {
                CallCurrentMethod();
            }

            if (cbEnergyParent.IsChecked == true)
            {
                CallEnergyMethod();
            }
        }

        private void btnGridCExpand_Click(object sender, RoutedEventArgs e)
        {
            btnGridCExpand.Visibility = Visibility.Hidden;
            btnGridCCompress.Visibility = Visibility.Visible;

            GridC.SetValue(Grid.RowProperty, 0);
            GridC.SetValue(Grid.RowSpanProperty, 6);
            GridC.SetValue(Grid.ColumnProperty, 0);
            GridC.SetValue(Grid.ColumnSpanProperty, 4);

            GridA.Visibility = Visibility.Hidden;
            GridB.Visibility = Visibility.Hidden;
            GridC.Visibility = Visibility.Visible;
            GridD.Visibility = Visibility.Hidden;
            GridE.Visibility = Visibility.Hidden;
        }

        private void btnGridCCompress_Click(object sender, RoutedEventArgs e)
        {
            btnGridCCompress.Visibility = Visibility.Hidden;
            btnGridCExpand.Visibility = Visibility.Visible;

            if (cbVoltageParent.IsChecked == true)
            {
                CallVoltageMethod();
            }

            if (cbCurrentParent.IsChecked == true)
            {
                CallCurrentMethod();
            }

            if (cbEnergyParent.IsChecked == true)
            {
                CallEnergyMethod();
            }
        }

        private void btnGridDExpand_Click(object sender, RoutedEventArgs e)
        {
            btnGridDExpand.Visibility = Visibility.Hidden;
            btnGridDCompress.Visibility = Visibility.Visible;

            GridD.SetValue(Grid.RowProperty, 0);
            GridD.SetValue(Grid.RowSpanProperty, 6);
            GridD.SetValue(Grid.ColumnProperty, 0);
            GridD.SetValue(Grid.ColumnSpanProperty, 4);

            GridA.Visibility = Visibility.Hidden;
            GridB.Visibility = Visibility.Hidden;
            GridC.Visibility = Visibility.Hidden;
            GridD.Visibility = Visibility.Visible;
            GridE.Visibility = Visibility.Hidden;
        }

        private void btnGridDCompress_Click(object sender, RoutedEventArgs e)
        {
            btnGridDCompress.Visibility = Visibility.Hidden;
            btnGridDExpand.Visibility = Visibility.Visible;

            if (cbVoltageParent.IsChecked == true)
            {
                CallVoltageMethod();
            }

            if (cbCurrentParent.IsChecked == true)
            {
                CallCurrentMethod();
            }

            if (cbEnergyParent.IsChecked == true)
            {
                CallEnergyMethod();
            }
        }

        private void btnGridEExpand_Click(object sender, RoutedEventArgs e)
        {
            btnGridEExpand.Visibility = Visibility.Hidden;
            btnGridECompress.Visibility = Visibility.Visible;

            GridE.SetValue(Grid.RowProperty, 0);
            GridE.SetValue(Grid.RowSpanProperty, 6);
            GridE.SetValue(Grid.ColumnProperty, 0);
            GridE.SetValue(Grid.ColumnSpanProperty, 4);

            GridA.Visibility = Visibility.Hidden;
            GridB.Visibility = Visibility.Hidden;
            GridC.Visibility = Visibility.Hidden;
            GridD.Visibility = Visibility.Hidden;
            GridE.Visibility = Visibility.Visible;

        }

        private void btnGridECompress_Click(object sender, RoutedEventArgs e)
        {
            btnGridECompress.Visibility = Visibility.Hidden;
            btnGridEExpand.Visibility = Visibility.Visible;
            if (cbVoltageParent.IsChecked == true)
            {
                CallVoltageMethod();
            }

            if (cbCurrentParent.IsChecked == true)
            {
                CallCurrentMethod();
            }

            if (cbEnergyParent.IsChecked == true)
            {
                CallEnergyMethod();
            }
        }

        #endregion Button Control for Grid

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Grid).Children.Remove(this);
        }
    }
}
