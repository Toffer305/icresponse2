using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using FirstFloor.ModernUI.Windows.Controls;


namespace ICResponse.Pages
{
    /// <summary>
    /// Interaction logic for AgentStats.xaml
    /// </summary>
    public partial class OverviewStats : UserControl
    {        
        public static LiveCallDoughnut IdleSector = new LiveCallDoughnut() { Status = "Idle", Count = 0 };
        public static LiveCallDoughnut InCallSector = new LiveCallDoughnut() { Status = "In Call", Count = 0 };
        
        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();
               
        int updateinterval = 20; // Seconds
        int idlecallcount = new Int32();
        int incallcount = new Int32();

        public class LiveCallDoughnutViewModel 
        {
            private readonly ObservableCollection<LiveCallDoughnut> _livecalls = new ObservableCollection<LiveCallDoughnut>();
            public ObservableCollection<LiveCallDoughnut> LiveCallStatus
            {
                get
                {
                    return _livecalls;
                }
            }
 
            public LiveCallDoughnutViewModel()
            {                
                _livecalls.Add(InCallSector);
                _livecalls.Add(IdleSector);                
            }
        }        

        public OverviewStats()
        {
            InitializeComponent();
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, updateinterval, 0);            
        }

        private void dt_Tick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                DoEverything();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            DoEverything();
            stopWatch.Start();
            dt.Start();
            this.Cursor = Cursors.Arrow;
        }

        private void DoEverything()
        {
            try
            {
                IdleSector.Count = 0;
                InCallSector.Count = 0;
                idlecallcount = 0;
                incallcount = 0;                
                
                GetInCalls();
                GetIdleCalls();
                LiveCallChart.DataContext = new LiveCallDoughnutViewModel();
                GetOverallDuration();
                GetTotalDailyCalls();
                GetOverViewCalls();
                //GetActiveCalls();
            }
            catch (MySqlException me)
            {
                System.Diagnostics.Debug.WriteLine(me);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;                
            }
            
        }

        private async void GetTotalDailyCalls()
        {
            using (MySqlConnection dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                                        ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                                        ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                                        ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";"))
            {
                dbConn.Open();
                using (MySqlCommand cmd = new MySqlCommand(ICResponse.Properties.Settings.Default.GetTotalDailyCallCount, dbConn))
                {
                    int dailycallcount = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    TDC.Content = Convert.ToString(dailycallcount);
                }
                dbConn.Close();
            } 
        }

        private async void GetIdleCalls()
        {
            using (MySqlConnection dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                                        ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                                        ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                                        ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";"))
            {
                dbConn.Open();
                using (MySqlCommand cmd = new MySqlCommand(ICResponse.Properties.Settings.Default.GetIdleCallCounts, dbConn))
                {
                    idlecallcount = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    IdleSector.Count = idlecallcount;
                }
                dbConn.Close();
            }                
        }

        private async void GetInCalls()
        {
            using (MySqlConnection dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                                        ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                                        ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                                        ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";"))
            {
                dbConn.Open();
                using (MySqlCommand cmd = new MySqlCommand(ICResponse.Properties.Settings.Default.GetInCallCounts, dbConn))
                {
                    incallcount = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    InCallSector.Count = incallcount;
                }
                dbConn.Close();
            }
        }

        private async void GetOverallDuration()
        {
            int calculatedaverage = new Int32();
            MySqlDataReader dbReader = null;

            using (MySqlConnection dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                                ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                                ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                                ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";"))
            {
                dbConn.Open();
                using (MySqlCommand cmd = new MySqlCommand(ICResponse.Properties.Settings.Default.GetOverallAverageTime, dbConn))
                {
                    dbReader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                    while (dbReader.Read())
                    {
                        int receivedaverage = Convert.ToInt32(dbReader["OverallAverageDuration"]);
                        calculatedaverage = (receivedaverage / 60);
                    }
                }
                dbConn.Close();
            }           
            
            ATT.Content = Convert.ToString(calculatedaverage);
            if (calculatedaverage >= 20)
            {
                ATT.Foreground = Brushes.Red;
            }
            if (calculatedaverage < 20 && calculatedaverage > 10)
            {
                ATT.Foreground = Brushes.Yellow;
            }
            if (calculatedaverage <= 10)
            {
                ATT.Foreground = Brushes.Chartreuse;
            }
        }

        private async void GetOverViewCalls()
        {
            try
            {
                DataTable dataFromDB = new DataTable();
                using (MySqlConnection dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                                                    ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                                                    ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                                                    ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass +
                                                                    ";Convert Zero Datetime=True;"))
                {
                    using (MySqlDataAdapter dbDataAdapter = new MySqlDataAdapter(ICResponse.Properties.Settings.Default.GetOverview, dbConn))
                    {
                        dbConn.Open();
                        await dbDataAdapter.FillAsync(dataFromDB);
                        OverViewCallsGrid.ItemsSource = dataFromDB.DefaultView;
                        dbDataAdapter.Dispose();                        
                    }
                    dbConn.Close();
                }                              
            }
            catch (MySqlException me)
            {
                System.Diagnostics.Debug.WriteLine(me);
                throw;                
            }
        }

        //private async void GetActiveCalls()
        //{
        //    try
        //    {
        //        DataTable dataFromDB = new DataTable();
        //        using (MySqlConnection dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
        //                                                            ";Database=" + ICResponse.Properties.Settings.Default.DBName +
        //                                                            ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
        //                                                            ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass +
        //                                                            ";Convert Zero Datetime=True;"))
        //        {
        //            using (MySqlDataAdapter dbDataAdapter = new MySqlDataAdapter(ICResponse.Properties.Settings.Default.GetActiveCalls, dbConn))
        //            {
        //                dbConn.Open();
        //                await dbDataAdapter.FillAsync(dataFromDB);
        //                ActiveCallsGrid.ItemsSource = dataFromDB.DefaultView;
        //                dbDataAdapter.Dispose();                        
        //            }
        //            dbConn.Close();
        //        }                
        //    }
        //    catch (MySqlException me)
        //    {
        //        System.Diagnostics.Debug.WriteLine(me);
        //        throw;
        //    }
        //}                         
        
        
    }   

}

