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
using System.Collections.Specialized;
using MySql.Data.MySqlClient;
using De.TorstenMandelkow.MetroChart;

namespace ICResponse.AdminContent
{
    /// <summary>
    /// Interaction logic for agentstats.xaml
    /// </summary>
    public partial class AgentStats : UserControl
    {
        public static ObservableCollection<DailyAgent> _counts = new ObservableCollection<DailyAgent>();
        public static ObservableCollection<DailyAgent> _times = new ObservableCollection<DailyAgent>();
        public static ObservableCollection<DailyAgent> _missed = new ObservableCollection<DailyAgent>();

        StringCollection _currentagents = new StringCollection();

        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();
        int updateinterval = 30; // Seconds       

        public AgentStats()
        {
            InitializeComponent();

            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, updateinterval, 0);

            
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RunAll();
            stopWatch.Start();
            dt.Start();
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
            GetAgentStats();
            DailyAgentCountChart.DataContext = new DailyAgentViewModel();
            DailyAgentTimesChart.DataContext = new DailyAgentViewModel();
            DailyAgentMissedChart.DataContext = new DailyAgentViewModel();
        }       

        private async void GetAgentStats()
        {
            try
            {
                MySqlConnection dbConn = null;
                MySqlDataReader dbReader = null;
                string receivedemail = string.Empty;
                int receivedNumOfRows = 0;
                _counts.Clear();
                _times.Clear();
                _missed.Clear();
                using (dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                                ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                                ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                                ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";"))
                {
                    dbConn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(ICResponse.Properties.Settings.Default.GetGroupedCalls, dbConn))
                    {
                        dbReader = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                        while (dbReader.Read())
                        {
                            receivedemail = Convert.ToString(dbReader["tech"]);
                            string formalname = GetAgentACDname(receivedemail);
                            receivedNumOfRows = Convert.ToInt32(dbReader["NumOfRows"]);
                            int averageduration = Convert.ToInt32(dbReader["TechAverageDuration"]);
                            int convertedduration = (averageduration / 60);
                            int missedcounts = GetAgentMissedCalls(formalname);                            

                            _counts.Add(new DailyAgent() { Agent = formalname, Count = receivedNumOfRows });
                            _times.Add(new DailyAgent() { Agent = formalname, Count = convertedduration });
                            _missed.Add(new DailyAgent() { Agent = formalname, Count = missedcounts });                            
                        }
                    }
                    dbReader.Close();
                    dbConn.Close();
                }               
            }
            catch (MySqlException err)
            {
                System.Diagnostics.Debug.WriteLine(err);
            }
        }

        private int GetAgentMissedCalls(string agentname)
        {            
            int missedcalls = 0;
            try
            {
                MySqlConnection dbConn = null;
                MySqlDataReader dbReader = null;
                using (dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                                    ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                                    ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                                    ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";"))
                {
                    string preppedquery = "SELECT missed FROM acdoverview WHERE name LIKE '" + agentname + "'";
                    dbConn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(preppedquery, dbConn))
                    {
                        cmd.Prepare();
                        dbReader = cmd.ExecuteReader();
                        while (dbReader.Read())
                        {
                            missedcalls = Convert.ToInt32(dbReader["missed"]);
                        }
                    }
                    dbReader.Close();
                    dbConn.Close();
                }
                        
            }
            catch (MySqlException me)
            {
                System.Diagnostics.Debug.WriteLine(me);
                throw;
            }           
            return missedcalls;     
        }

        private string GetAgentACDname(string agentname)
        {
            string fullname = string.Empty;

            try
            {                
                MySqlConnection dbConn = null;
                MySqlDataReader dbReader = null;
                using (dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                                    ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                                    ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                                    ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";"))
                {
                    string preppedquery = "SELECT first,last FROM agents WHERE email ='" + agentname + "'";
                    dbConn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(preppedquery, dbConn))
                    {
                        cmd.Prepare();
                        dbReader = cmd.ExecuteReader();
                        while (dbReader.Read())
                        {
                            string firstname = Convert.ToString(dbReader["first"]);
                            string lastname = Convert.ToString(dbReader["last"]);
                            fullname = firstname + " " + lastname;
                        }
                    }
                    dbReader.Close();
                    dbConn.Close();
                }

            }
            catch (MySqlException me)
            {
                System.Diagnostics.Debug.WriteLine(me);
                throw;
            }
            return fullname;
        }

        public class DailyAgentViewModel
        {
            private readonly ObservableCollection<DailyAgent> _agentcounts = new ObservableCollection<DailyAgent>();
            private readonly ObservableCollection<DailyAgent> _agenttimes = new ObservableCollection<DailyAgent>();
            private readonly ObservableCollection<DailyAgent> _agentmissed = new ObservableCollection<DailyAgent>();

            public ObservableCollection<DailyAgent> AgentDailyCounts
            {
                get
                {
                    return _agentcounts;
                }
            }

            public ObservableCollection<DailyAgent> AgentDailyTimes
            {
                get
                {
                    return _agenttimes;
                }
            }

            public ObservableCollection<DailyAgent> AgentDailyMissed
            {
                get
                {
                    return _agentmissed;
                }
            }

            public DailyAgentViewModel()
            {
                _agentcounts = _counts;
                _agenttimes = _times;
                _agentmissed = _missed;
            }
        }        

    }
}
