using System;
using System.Collections.Generic;
using System.Data;
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
using System.IO.Ports;
using System.Windows.Threading;
using System.IO;
using System.Deployment.Application;
using System.Threading;
using System.Net.NetworkInformation;
using System.Windows.Markup;

namespace MRB_Review_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    /// First Release By Javier Marquez
    /// 1.0.0.0
    /// MRB Review System
    /// 
    /// 
    /// 17 Jul 2022
    /// Release to PRD
    /// 1.0.0.5
    /// 
    /// 
    /// 20 Jul 2022
    /// Correct Report related of Missing process
    /// 1.0.0.6
    ///
    /// 
    /// 21 Jul 2022
    /// Fix Reported Missing Process units
    /// 1.0.0.7

    public partial class MainWindow : Window
    {
        SerialPort _Keyence = new SerialPort();
        DispatcherTimer _TimerTrigger = new DispatcherTimer();

        //string Result = new MES_TIS.MES_TIS().OKToTest("BOSTON", "BOSTON", "BXG22220226", "BX92316388-12-01",
        //                                                "MXCHIBOSTEFVT01", "FVT / FVT");

        public MainWindow()
        {
            InitializeComponent();
            InitializeDgv();
            GetVersion();        
        }

        void GetVersion() 
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                Version myVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                lblVersion.Content = "PRD v" + myVersion;
            }
            else
            {
                lblVersion.Content = "UAT v" + "1.0.0.0";
            }


            DockMenu.Width = 0;
            btnGetData.Visibility = Visibility.Hidden;
            btnGetDataAll.Visibility = Visibility.Hidden;
            btnCelanExcel.Visibility = Visibility.Hidden;
            btnFromFile.Visibility = Visibility.Hidden;

            try
            {
                if(Environment.MachineName == "MXCHILAP971") _Keyence.PortName = ConfigFiles.reader("MRB Review System", "UAT_SERIAL_PORT", Globals.CONFIG_FILE);
                else _Keyence.PortName = ConfigFiles.reader("MRB Review System", "SERIAL_PORT", Globals.CONFIG_FILE);
                _Keyence.BaudRate = 9600;
                _Keyence.DataBits = 8;
                _Keyence.Parity = Parity.None;
                _Keyence.Handshake = Handshake.None;
                _Keyence.DataReceived += _Keyence_DataReceived;
                _Keyence.Open();

                _TimerTrigger.Interval = new TimeSpan(0, 0, Globals.TRIGGER);
                _TimerTrigger.Tick += _TimerTrigger_Tick;
                _TimerTrigger.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("App Error: " + ex.Message + "\n" + "\n" + "Escaner Keyence no habilitado", "MRB Review System",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                //txtBoxScan.IsEnabled = false;
            }
        }

        private void _TimerTrigger_Tick(object sender, EventArgs e)
        {
            txtBoxScan.Focus();
            txtBoxScan.Clear();
            _Keyence.Write("LON\r");
        }

        public struct MyData
        {
            public DateTime DATE { set; get; }
            public string SERIAL_NUMBER { set; get; }
            public string PROCESS { set; get; }
            public string CRD { set; get; }
            public string DEFECT { set; get; }
            public string STATUS { set; get; }
        }


        public delegate void myDelegate();

        void InitializeDgv() 
        {
            DataGridTextColumn DATE = new DataGridTextColumn();
            DATE.Header = "DATE";
            DATE.Binding = new Binding("DATE");
            DATE.Width = 150;
            DATE.IsReadOnly = true;

            DataGridTextColumn SERIAL_NUMBER = new DataGridTextColumn();
            SERIAL_NUMBER.Header = "SERIAL NUMBER";
            SERIAL_NUMBER.Binding = new Binding("SERIAL_NUMBER");
            SERIAL_NUMBER.Width = 130;
            SERIAL_NUMBER.IsReadOnly = true;

            DataGridTextColumn PROCESS = new DataGridTextColumn();
            PROCESS.Header = "PROCESS";
            PROCESS.Binding = new Binding("PROCESS");
            PROCESS.Width = 300;
            PROCESS.IsReadOnly = true;

            DataGridTextColumn DEFECT = new DataGridTextColumn();
            DEFECT.Header = "DEFECT";
            DEFECT.Binding = new Binding("DEFECT");
            DEFECT.Width = 200;
            DEFECT.IsReadOnly = true;

            DataGridTextColumn CRD = new DataGridTextColumn();
            CRD.Header = "CRD";
            CRD.Binding = new Binding("CRD");
            CRD.Width = 100;
            CRD.IsReadOnly = true;

            DataGridTextColumn STATUS = new DataGridTextColumn();
            STATUS.Header = "STATUS";
            STATUS.Binding = new Binding("STATUS");
            STATUS.Width = 100;
            STATUS.IsReadOnly = true;


            dgvData.Columns.Add(DATE);
            dgvData.Columns.Add(SERIAL_NUMBER);
            dgvData.Columns.Add(PROCESS);
            dgvData.Columns.Add(DEFECT);
            dgvData.Columns.Add(CRD);
            dgvData.Columns.Add(STATUS);
        }

        private async void txtBoxScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Globals.SERIAL_NUMBER = txtBoxScan.Text;
                
                if (string.IsNullOrEmpty(Globals.SERIAL_NUMBER)) return;

                if (Globals.SERIAL_NUMBER.Length == 16) 
                {
                    Task<string> _taskMainFunction = new Task<string>(MainFunction3);
                    _taskMainFunction.Start();
                    string result = await _taskMainFunction;

                    txtBoxScan.Focus();
                    txtBoxScan.Clear();
                }
            }
        }

        private void _Keyence_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string Result = _Keyence.ReadLine();

            if (Result.Length == 17)
            {
                _TimerTrigger.Stop();
                Globals.SERIAL_NUMBER = Result.TrimEnd('\r');
                Globals.PREVIOUS_SN = Globals.SERIAL_NUMBER;
                this.Dispatcher.BeginInvoke(new Action(() => {txtBoxScan.Text = Globals.SERIAL_NUMBER; }));
                Task.Factory.StartNew(() => MainFunction());    
                this.Dispatcher.BeginInvoke(new Action(() => { lblUnitsScanned.Content = "UNIT SCANNED: " + Globals.QTY_UNITS_SCANNED; }));
            }
        }

        string MainFunction()
        {
            Globals.BUSY = true;

            if(StaticFunctions.IsAlreadyRegister()) 
            {
                Thread.Sleep(3000);
                WriteDgv(DateTime.Now, Globals.SERIAL_NUMBER, "UNIDAD YA REGISTRADA", "", "", ""); 
                StaticFunctions.NotifyError();
                CleanUP();
                return "OK";
            }

            DataSet _dsQuery = new MES.Service().SelectBySerialNumber(Globals.SERIAL_NUMBER);

            if (_dsQuery.Tables[0].Rows.Count > 1) goto Skip;

            Globals.CUSTOMER_ID = Convert.ToInt32(_dsQuery.Tables[0].Rows[0][2]);
            Globals.WIP_ID = Convert.ToInt32(_dsQuery.Tables[0].Rows[0][0]);

            _dsQuery = new MES.Service().BoardHistoryReport(Globals.SERIAL_NUMBER, Globals.CUSTOMER_ID);

            foreach (DataRow _dr in _dsQuery.Tables[0].Rows)
            {
                string _TempProces = _dr[9].ToString();              
                string _TempStatus = _dr[10].ToString();

                if (_TempStatus == "Fail")
                {
                    Globals.PROCESS = _TempProces;
                    Globals.STATUS = _TempStatus;

                    foreach(DataRow __dr in _dsQuery.Tables[0].Rows) 
                    {
                        string __TempStatus = __dr[10].ToString();

                        if(__TempStatus == "Unconfirmed" || __TempStatus == "Confirmed") 
                        {
                            Globals.CRD = __dr[11].ToString();
                            Globals.DEFECT = __dr[19].ToString();                         
                            break;
                        }
                    }

                    Globals.FLAG_FAIL = true;
                    StaticFunctions.RegisterUnit();

                    break;
                }                            
            }


            if (!Globals.FLAG_FAIL) 
            {
                foreach(string _StepMatrix in Globals.DATA_MATRIX) 
                {
                    string StepToCheck = ConfigFiles.reader("DATA_MATRIX", _StepMatrix, Globals.CONFIG_FILE);

                    DataTable EventsByStepMatrix = new DataTable();

                    try 
                    {
                        EventsByStepMatrix = _dsQuery.Tables[0].AsEnumerable()
                        .Where(r => r.Field<string>("Test_Process") == StepToCheck)
                        .CopyToDataTable();
                    }
                    catch (Exception) { }

                
                    if(EventsByStepMatrix.Rows.Count >= 1) 
                    {
                        Globals.COUNT_MATRIX++;   
                    }
                    else 
                    {
                        Globals.STEPS_MISSING += StepToCheck + ", ";
                    }
                }

                if (!string.IsNullOrEmpty(Globals.STEPS_MISSING)) 
                {
                    Globals.PROCESS = "PROCESOS FALTANTES";
                    Globals.DEFECT = Globals.STEPS_MISSING;
                    WriteDgv(DateTime.Now, Globals.SERIAL_NUMBER, Globals.PROCESS, Globals.CRD, Globals.DEFECT, Globals.STATUS);                 
                    StaticFunctions.RegisterUnit();
                    StaticFunctions.NotifyOK();
                    CleanUP();
                    this.Dispatcher.BeginInvoke(new Action(() => { lblUnitsScanned.Content = "Units Scanned: " + Globals.QTY_UNITS_SCANNED; }));
                    return "OK";
                }
                if(string.IsNullOrEmpty(Globals.STEPS_MISSING) && Globals.COUNT_MATRIX == 16) 
                {
                    Globals.PROCESS = "TODOS LOS PROCESOS OK";
                    WriteDgv(DateTime.Now, Globals.SERIAL_NUMBER, Globals.PROCESS, Globals.CRD, Globals.DEFECT, Globals.STATUS);                  
                    StaticFunctions.RegisterUnit();
                    StaticFunctions.NotifyOK();
                    CleanUP();
                    this.Dispatcher.BeginInvoke(new Action(() => { lblUnitsScanned.Content = "Units Scanned: " + Globals.QTY_UNITS_SCANNED; }));
                    return "OK";
                }
            }

            WriteDgv(DateTime.Now, Globals.SERIAL_NUMBER, Globals.PROCESS, Globals.CRD, Globals.DEFECT, Globals.STATUS);
            StaticFunctions.NotifyOK();

            this.Dispatcher.BeginInvoke(new Action(() => { lblUnitsScanned.Content = "Units Scanned: " + Globals.QTY_UNITS_SCANNED; }));

        Skip: { }

            CleanUP();

            return "OK";
        }





        string MainFunction2()
        {
            Globals.BUSY = true;

            if (StaticFunctions.IsAlreadyRegister())
            {
                Thread.Sleep(3000);
                WriteDgv(DateTime.Now, Globals.SERIAL_NUMBER, "UNIDAD YA REGISTRADA", "", "", "");
                //StaticFunctions.NotifyError();
                CleanUP();
                return "OK";
            }

            DataSet _dsQuery = new MES.Service().SelectBySerialNumber(Globals.SERIAL_NUMBER);

            //if (_dsQuery.Tables[0].Rows.Count > 1) goto Skip;

            Globals.CUSTOMER_ID = Convert.ToInt32(_dsQuery.Tables[0].Rows[0][2]);
            Globals.WIP_ID = Convert.ToInt32(_dsQuery.Tables[0].Rows[0][0]);

            _dsQuery = new MES.Service().BoardHistoryReport(Globals.SERIAL_NUMBER, Globals.CUSTOMER_ID);

            DataTable EventsByStepMatrix1 = new DataTable();

            foreach (string _Key in Globals.DATA_MATRIX)
            {
                string StepToCheck = ConfigFiles.reader("DATA_MATRIX", _Key, Globals.CONFIG_FILE);

                try
                {
                    EventsByStepMatrix1 = _dsQuery.Tables[0].AsEnumerable()
                                                              .Where(r => r.Field<string>("TestType") == "TEST" &&
                                                                          r.Field<string>("Test_Process") == StepToCheck &&
                                                                          r.Field<string>("TestStatus") == "Pass")
                                                              .CopyToDataTable();

                    goto NEXT_PROCESS;
                }
                catch (Exception ex) { }


                try
                {
                    if(StepToCheck.Contains("QC") || StepToCheck.Contains("LINK-TXSN")) 
                    {
                        EventsByStepMatrix1 = _dsQuery.Tables[0].AsEnumerable()
                                               .Where(r => r.Field<string>("TestType") == "Movement Both" &&
                                                           r.Field<string>("Test_Process") == StepToCheck &&
                                                           r.Field<string>("TestStatus") == "Pass")
                                               .CopyToDataTable();
                        goto NEXT_PROCESS;
                    }              
                }

                catch (Exception ex) 
                {
                    Globals.PROCESS = StepToCheck;
                    Globals.STATUS = "MISSING PROCESS";
                    Globals.DEFECT = "MISSING PROCESS";
                    Globals.CRD = "MISSING PROCESS";

                    StaticFunctions.RegisterUnit();
                    goto End;
                }           

                try
                {

                    if (StepToCheck == "AOI / AOIB") StepToCheck = "AOI / AOIINSPB";

                    EventsByStepMatrix1 = _dsQuery.Tables[0].AsEnumerable()
                                                              .Where(r => r.Field<string>("TestType") == "TEST" &&
                                                                          r.Field<string>("Test_Process") == StepToCheck &&
                                                                          r.Field<string>("TestStatus") == "Fail")
                                                              .CopyToDataTable();


                    EventsByStepMatrix1 = _dsQuery.Tables[0].AsEnumerable()
                                                        .Where(r => r.Field<string>("TestType") == "Analysis" &&
                                                                    r.Field<string>("Test_Process") == StepToCheck &&
                                                                    r.Field<string>("TestStatus") == "Confirmed" || r.Field<string>("TestStatus") == "Unconfirmed")
                                                        .CopyToDataTable();

                    Globals.PROCESS = EventsByStepMatrix1.Rows[0][9].ToString();
                    Globals.STATUS = EventsByStepMatrix1.Rows[0][10].ToString();
                    Globals.DEFECT = EventsByStepMatrix1.Rows[0][19].ToString();
                    Globals.CRD = EventsByStepMatrix1.Rows[0][11].ToString();

                    Globals.FLAG_FAIL = true;
                    StaticFunctions.RegisterUnit();
                    goto End;
                }

                catch (Exception) 
                {
                    Globals.PROCESS = StepToCheck;
                    Globals.STATUS = "MISSING PROCESS";
                    Globals.DEFECT = "MISSING PROCESS";
                    Globals.CRD = "MISSING PROCESS";
                  
                    StaticFunctions.RegisterUnit();
                    goto End;
                }

            NEXT_PROCESS: { }

            }

            if (Globals.FLAG_FAIL) goto End;

            Globals.PROCESS = "TODOS LOS PROCESOS OK";
            Globals.STATUS = "TODOS LOS PROCESOS OK";
            Globals.DEFECT = "TODOS LOS PROCESOS OK";
            Globals.CRD = "TODOS LOS PROCESOS OK";
            StaticFunctions.RegisterUnit();
            StaticFunctions.NotifyOK();

        End: { }
            WriteDgv(DateTime.Now, Globals.SERIAL_NUMBER, Globals.PROCESS, Globals.CRD, Globals.DEFECT, Globals.STATUS);
            CleanUP();
            return "OK";
        }



        string MainFunction3() 
        {
            Globals.BUSY = true;

            if (StaticFunctions.IsAlreadyRegister())
            {
                Thread.Sleep(3000);
                WriteDgv(DateTime.Now, Globals.SERIAL_NUMBER, "UNIDAD YA REGISTRADA", "", "", "");
                CleanUP();
                return "OK";
            }

            DataSet _dsQuery = new MES.Service().SelectBySerialNumber(Globals.SERIAL_NUMBER);
            Globals.CUSTOMER_ID = Convert.ToInt32(_dsQuery.Tables[0].Rows[0][2]);
            Globals.WIP_ID = Convert.ToInt32(_dsQuery.Tables[0].Rows[0][0]);

            _dsQuery = new MES.Service().BoardHistoryReport(Globals.SERIAL_NUMBER, Globals.CUSTOMER_ID);

            DataTable EventsByStepMatrix1 = new DataTable();

            foreach (string _Key in Globals.DATA_MATRIX)
            {
                bool result = false;
                string StepToCheck = ConfigFiles.reader("DATA_MATRIX", _Key, Globals.CONFIG_FILE);
                string TestType = "TEST";
                string TestStatus = "Pass";

                if (StepToCheck == "QC / BSI1" || StepToCheck == "QC / SMT FINAL INSP") TestType = "Movement Both";

             
                result = StaticMESFunctions.CheckPoint(_dsQuery, StepToCheck, TestType, TestStatus);
                if (result) goto NEXT_PROCESS;


                if(StepToCheck == "AOI / AOIB") 
                {
                    StepToCheck = "AOI / AOIINSPB";
                    result = StaticMESFunctions.CheckPoint(_dsQuery, StepToCheck, TestType, TestStatus);
                    if (result) goto NEXT_PROCESS;
                }
                
                TestType = "Analysis";
                TestStatus = "Confirmed";
                var items = StaticMESFunctions.GetFailure(_dsQuery, StepToCheck, TestType, TestStatus);

                if (items.Item1)
                {
                    Globals.PROCESS = items.Item2;
                    Globals.STATUS = items.Item3;
                    Globals.DEFECT = items.Item4;
                    Globals.CRD = items.Item5;
                    Globals.FLAG_FAIL = true;
                    break;
                }

                TestType = "Analysis";
                TestStatus = "Unconfirmed";
                items = StaticMESFunctions.GetFailure(_dsQuery, StepToCheck, TestType, TestStatus);

                if (items.Item1)
                {
                    Globals.PROCESS = items.Item2;
                    Globals.STATUS = items.Item3;
                    Globals.DEFECT = items.Item4;
                    Globals.CRD = items.Item5;
                    Globals.FLAG_FAIL = true;
                    break;
                }

                TestType = "TEST";
                TestStatus = "Fail";
                items = StaticMESFunctions.GetFailure(_dsQuery, StepToCheck, TestType, TestStatus);

                if (items.Item1)
                {
                    Globals.PROCESS = items.Item2;
                    Globals.STATUS = items.Item3;
                    Globals.DEFECT = items.Item5;
                    Globals.CRD = items.Item4;
                    Globals.FLAG_FAIL = true;
                    break;
                }

                if (!items.Item1) 
                {
                    //MISSING PROCESS
                    Globals.PROCESS = StepToCheck;
                    Globals.STATUS = "MISSING PROCESS";
                    Globals.DEFECT = "MISSING PROCESS";
                    Globals.CRD = "MISSING PROCESS";
                    Globals.FLAG_FAIL = true;
                    break;
                }
  

            NEXT_PROCESS: { }
            }

            if (!Globals.FLAG_FAIL) 
            {
                Globals.PROCESS = "TODOS LOS PROCESOS OK";
                Globals.STATUS = "TODOS LOS PROCESOS OK";
                Globals.DEFECT = "TODOS LOS PROCESOS OK";
                Globals.CRD = "TODOS LOS PROCESOS OK";
            }

            StaticFunctions.RegisterUnit();
            WriteDgv(DateTime.Now, Globals.SERIAL_NUMBER, Globals.PROCESS, Globals.CRD, Globals.DEFECT, Globals.STATUS);
            CleanUP();
            return "OK";
        }

        void CleanUP() 
        {
            Globals.SERIAL_NUMBER = string.Empty;
            Globals.PROCESS = string.Empty;
            Globals.CRD = string.Empty;
            Globals.DEFECT = string.Empty;
            Globals.STATUS = string.Empty;
            Globals.FLAG_FAIL = false;
            Globals.COUNT_MATRIX = 0;
            Globals.STEPS_MISSING = string.Empty;
            Globals.BUSY = false;
            _TimerTrigger.Start();
        }



        private void WaitNMilliSeconds(int MilliSeconds)
        {
            if (MilliSeconds < 100) return;
            DateTime _desired = DateTime.Now.AddMilliseconds(MilliSeconds);

            while (DateTime.Now < _desired)
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                      new Action(delegate { }));
            }
        }

        private void dgvData_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var row = e.Row;
            MyData _myData = new MyData();
            _myData = (MyData)row.DataContext;

            if (_myData.STATUS == "Confirmed" || _myData.STATUS == "Unconfirmed" || _myData.STATUS == "Fail")
            {
                row.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B71C1C")); //DEEP RED 900
                row.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
                return;
            }

            if (_myData.STATUS == "MISSING PROCESS")
            {
                row.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b28900")); //DEEP ORANGE 900
                row.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
                return;
            }

            if (_myData.PROCESS == "TODOS LOS PROCESOS OK") 
            {
                row.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1B5E20")); //DEEP GREEN 900
                row.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
                return;
            }

            if (_myData.PROCESS == "UNIDAD YA REGISTRADA")
            {
                row.Background = new SolidColorBrush(Colors.GhostWhite);
                row.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#212121")); //DEEP GREEN 900 //GRAY 900
                return;
            }

        }

        void WriteDgv(DateTime _DATE, string _SERIAL_NUMBER, string _PROCESS, string _CRD, string _DEFECT, string _STATUS)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                dgvData.Items.Add(new MyData { DATE = _DATE, SERIAL_NUMBER = _SERIAL_NUMBER, PROCESS = _PROCESS, CRD = _CRD, DEFECT = _DEFECT, STATUS = _STATUS });

                if (dgvData.Items.Count > 0)
                {
                    var border = VisualTreeHelper.GetChild(dgvData, 0) as Decorator;
                    if (border != null)
                    {
                        var scroll = border.Child as ScrollViewer;
                        if (scroll != null) scroll.ScrollToEnd();
                    }
                }

                return;
            }));              
        }

        void WriteDgvExample()
        {
            dgvData.Items.Add(new MyData { DATE = DateTime.Now, SERIAL_NUMBER = "123456789", PROCESS = "AOI EPOXY / AOI EPOXY", CRD = "U3", DEFECT = "CONTAMINATED", STATUS = "FAIL" });

            if (dgvData.Items.Count > 0)
            {
                var border = VisualTreeHelper.GetChild(dgvData, 0) as Decorator;
                if (border != null)
                {
                    var scroll = border.Child as ScrollViewer;
                    if (scroll != null) scroll.ScrollToEnd();
                }
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }

        private void btnTrigger_Click(object sender, RoutedEventArgs e)
        {
            txtBoxScan.Clear();
            _Keyence.Write("LON\r");
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
                   
            if (Globals.DOCK_MENU) 
            {
                //Thickness _temp = dgvData.Margin;
                //_temp.Left = 160f;
                //dgvData.Margin = _temp;
                btnGetData.Visibility = Visibility.Visible;
                btnGetDataAll.Visibility = Visibility.Visible;
                btnCelanExcel.Visibility = Visibility.Visible;
                btnFromFile.Visibility = Visibility.Visible;

                Globals.DOCK_MENU = false;
                DockMenu.Width = 180;
                return;
            }

            if (!Globals.DOCK_MENU) 
            {
                //Thickness _temp = dgvData.Margin;
                //_temp.Left = 0f;
                //dgvData.Margin = _temp;
                btnGetData.Visibility = Visibility.Hidden;
                btnGetDataAll.Visibility = Visibility.Hidden;
                btnCelanExcel.Visibility = Visibility.Hidden;
                btnFromFile.Visibility = Visibility.Hidden;

                Globals.DOCK_MENU = true;
                DockMenu.Width = 0;
                return;
            }
        }

        private void btnGetDataAll_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "Archive (*.xlsx)|*.xlsx";
            saveFileDialog.FileName = "MRBAll_report_" + System.DateTime.Now.Day.ToString() + "_"
                                                    + System.DateTime.Now.Month.ToString() + "_"
                                                    + System.DateTime.Now.Year.ToString() + "_"
                                                    + System.DateTime.Now.Millisecond.ToString();

            System.Windows.Forms.DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
            tryReadAgainAll:
                DataTable dtQuery = ExcelFunctions.Excel_To_DataTable(Globals.PATH_FILE_ALL, 0);
                if (dtQuery == null) goto tryReadAgainAll;

                ExcelFunctions.DataTable_To_Excel(dtQuery, saveFileDialog.FileName);
                StaticFunctions.MakeToast(saveFileDialog.FileName);
                btnMenu_Click(sender, e);
            }          
        }

        private void btnGetData_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "Archive (*.xlsx)|*.xlsx";
            saveFileDialog.FileName = "MRBTemp_report_" + System.DateTime.Now.Day.ToString() + "_"
                                                    + System.DateTime.Now.Month.ToString() + "_"
                                                    + System.DateTime.Now.Year.ToString() + "_"
                                                    + System.DateTime.Now.Millisecond.ToString();

            System.Windows.Forms.DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
            tryReadAgain:
                DataTable dtQuery = ExcelFunctions.Excel_To_DataTable(Globals.PATH_FILE, 0);
                if (dtQuery == null) goto tryReadAgain;

                ExcelFunctions.DataTable_To_Excel(dtQuery, saveFileDialog.FileName);
                StaticFunctions.MakeToast(saveFileDialog.FileName);
                btnMenu_Click(sender, e);
            }

            
        }

        private void lblHistory_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WriteDgvExample();
        }

        void FunctionTemporal()
        {

            string[] _SerialNumbers = File.ReadAllLines(@"C:\AppData\ToReview.txt");


            foreach (string Serial in _SerialNumbers)
            {
                Globals.SERIAL_NUMBER = Serial;

                DataSet _dsQuery = new MES.Service().SelectBySerialNumber(Globals.SERIAL_NUMBER);

                Globals.CUSTOMER_ID = Convert.ToInt32(_dsQuery.Tables[0].Rows[0][2]);

                Globals.WIP_ID = Convert.ToInt32(_dsQuery.Tables[0].Rows[0][0]);

                _dsQuery = new MES.Service().BoardHistoryReport(Globals.SERIAL_NUMBER, Globals.CUSTOMER_ID);

                foreach (DataRow _dr in _dsQuery.Tables[0].Rows)
                {
                    Globals.PROCESS = _dr[9].ToString();
                    Globals.CRD = _dr[11].ToString();
                    Globals.STATUS = _dr[10].ToString();


                    if (Globals.STATUS == "Fail")
                    {
                        StaticFunctions.RegisterUnit();
                    }
                }

                WriteDgv(DateTime.Now, Globals.SERIAL_NUMBER, Globals.PROCESS, Globals.CRD, Globals.DEFECT, Globals.STATUS);
            }
        }

        private void btnCelanExcel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Se iniciaria proceso para limpieza en la base de datos \n \n" + 
                                                      "Una vez limpia la base de datos no se podra recuperar \n \n" + 
                                                      "Presiona OK para continuar \n" +
                                                      "Presiona Cancelar para cancelar" , "MRB Review System", MessageBoxButton.OKCancel, MessageBoxImage.Question);        
            if (result == MessageBoxResult.OK)
            {
                StaticFunctions.CleanExcelFile();
                Globals.QTY_UNITS_SCANNED = 0;
                lblUnitsScanned.Content = "Units Scanned: " + Globals.QTY_UNITS_SCANNED;
                MessageBox.Show("La base de datos se ha limpiado exitosamente", "MRB Review System", MessageBoxButton.OK, MessageBoxImage.Information);
                btnMenu_Click(sender, e);
            }        
        }







        int vari_1 = 0;
        int vari_2 = 0;

        void ExampleAsyncFunc_1() 
        {
            while(vari_1 <= 300) 
            {
                vari_1++;
                Thread.Sleep(100);
            }        
        }


        void ExampleAsyncFunc_2()
        {
            while (vari_2 < 300)
            {
                vari_2++;
                Thread.Sleep(100);
            }           
        }

        private void lblTitle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            vari_1 = 0;
            vari_2 = 0;

            ThreadStart ts1 = new ThreadStart(ExampleAsyncFunc_1);
            ThreadStart ts2 = new ThreadStart(ExampleAsyncFunc_2);

            Thread t1 = new Thread(ts1);
            Thread t2 = new Thread(ts2);

            t1.Start();
            t2.Start();
        }

        private void btnFromFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Archive (*.xlsx)|*.xlsx";
            openFileDialog.InitialDirectory = @"C:\";
            openFileDialog.Title = "IMPORT FILE";
            //openFileDialog.DefaultExt= "Archive(*.xlsx) | *.xlsx";

            System.Windows.Forms.DialogResult dialogResult = openFileDialog.ShowDialog();


            if(dialogResult == System.Windows.Forms.DialogResult.OK) 
            {
                string PathFile = openFileDialog.FileName;
                KeyEventArgs _key = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Enter);

            tryReadAgainAll:
                DataTable dtQuery = ExcelFunctions.Excel_To_DataTable(PathFile, 0);
                if (dtQuery == null) goto tryReadAgainAll;

                ProBarTask.Visibility= Visibility.Visible;
                ProBarTask.Value = 0;
                ProBarTask.Maximum = dtQuery.Rows.Count;

                btnMenu_Click(sender, e);

                foreach(DataRow dr in dtQuery.Rows) 
                {
                    txtBoxScan.Text = dr[0].ToString();

                    this.Dispatcher.Invoke(() =>
                    {
                        ProBarTask.Value++;
                    });



                TryAgain:
                    if (Globals.BUSY) goto TryAgain;


                    try { txtBoxScan_KeyDown(sender, _key); }
                    catch(Exception ex) { goto TryAgain; }
                   
                    WaitNSeconds(1);
                }
            }
        }

        private void WaitNSeconds(int segundos)
        {
            if (segundos < 1) return;
            DateTime _desired = DateTime.Now.AddSeconds(segundos);
            while (DateTime.Now < _desired)
            {        
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                      new Action(delegate { }));
            }
        }
    }



}
