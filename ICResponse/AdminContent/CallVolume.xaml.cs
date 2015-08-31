using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Threading;
using MySql.Data.MySqlClient;

namespace ICResponse.AdminContent
{
    /// <summary>
    /// Interaction logic for CallVolume.xaml
    /// </summary>
    public partial class CallVolume : UserControl
    {
        public static ObservableCollection<CallVolumeChart> _volume = new ObservableCollection<CallVolumeChart>();
        List<DateTime> volumebyHour = new List<DateTime>();
        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();
        int updateinterval = 30; // Seconds

        public CallVolume()
        {
            InitializeComponent();
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, updateinterval, 0);
        }

        private void dt_Tick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                RunAll();
            }
        }

        private void RunAll()
        {
            GetCallVolumes();
            CallVolumeChart.DataContext = new CallVolumeViewModel();
        }

        private async void GetCallVolumes()
        {
            try
            {
                MySqlConnection dbConn = null;
                MySqlDataReader dbReader = null;
                string receivedDT = string.Empty;
                DateTime receivedtime = DateTime.Now;
                int nine2ten = 0;
                int ten2eleven = 0;
                int eleven2twelve = 0;
                int twelve2thirteen = 0;
                int thirteen2fourteen = 0;
                int fourteen2fifteen = 0;
                int fifteen2sixteen = 0;
                int sixteen2seventeen = 0;
                int seventeen2eighteen = 0;
                int eighteen2nineteen = 0;
                int nineteen2twenty = 0;
                int twenty2twentyone = 0;
                _volume.Clear();
                volumebyHour.Clear();

                using (dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                                ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                                ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                                ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";"))
                {
                    dbConn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(ICResponse.Properties.Settings.Default.GetCallVolume, dbConn))
                    {
                        dbReader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                        while (dbReader.Read())
                        {
                            receivedDT = Convert.ToString(dbReader["startTime"]);
                            receivedtime = DateTime.Parse(receivedDT);
                            volumebyHour.Add(receivedtime);
                        }
                    }                    
                    dbReader.Close();
                    dbConn.Close();

                    for (int i = 0; i < volumebyHour.Count; i++)
                    {
                        int hourcomponent = Int32.Parse(volumebyHour[i].Hour.ToString()); 
                        switch (hourcomponent)
                        {
                            case 9:
                                nine2ten++;
                                break;
                            case 10:
                                ten2eleven++;
                                break;
                            case 11:
                                eleven2twelve++;
                                break;
                            case 12:
                                twelve2thirteen++;
                                break;
                            case 13:
                                thirteen2fourteen++;
                                break;
                            case 14:
                                fourteen2fifteen++;
                                break;
                            case 15:
                                fifteen2sixteen++;
                                break;
                            case 16:
                                sixteen2seventeen++;
                                break;
                            case 17:
                                seventeen2eighteen++;
                                break;
                            case 18:
                                eighteen2nineteen++;
                                break;
                            case 19:
                                nineteen2twenty++;
                                break;
                            case 20:
                                twenty2twentyone++;
                                break;
                            default:
                                break;
                        }
                    }
                }
                
                _volume.Add(new CallVolumeChart() { Time = "Nine", Count = nine2ten });
                _volume.Add(new CallVolumeChart() { Time = "Ten", Count = ten2eleven });
                _volume.Add(new CallVolumeChart() { Time = "Eleven", Count = eleven2twelve });
                _volume.Add(new CallVolumeChart() { Time = "Twelve", Count = twelve2thirteen });
                _volume.Add(new CallVolumeChart() { Time = "One", Count = thirteen2fourteen });
                _volume.Add(new CallVolumeChart() { Time = "Two", Count = fourteen2fifteen });
                _volume.Add(new CallVolumeChart() { Time = "Three", Count = fifteen2sixteen });
                _volume.Add(new CallVolumeChart() { Time = "Four", Count = sixteen2seventeen });
                _volume.Add(new CallVolumeChart() { Time = "Five", Count = seventeen2eighteen });
                _volume.Add(new CallVolumeChart() { Time = "Six", Count = eighteen2nineteen });
                _volume.Add(new CallVolumeChart() { Time = "Seven", Count = nineteen2twenty });
                _volume.Add(new CallVolumeChart() { Time = "Eight", Count = twenty2twentyone });

            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err);
            }
        }

        public class CallVolumeViewModel
        {
            private readonly ObservableCollection<CallVolumeChart> _volumecounts = new ObservableCollection<CallVolumeChart>();

            public ObservableCollection<CallVolumeChart> DailyCallVolume
            {
                get
                {
                    return _volumecounts;
                }
            }

            public CallVolumeViewModel()
            {
                _volumecounts = _volume;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RunAll();
            stopWatch.Start();
            dt.Start();
        }


    }
}
