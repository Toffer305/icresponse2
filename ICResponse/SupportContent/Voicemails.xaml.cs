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
using MySql.Data.MySqlClient;
using System.Data;

namespace ICResponse.SupportContent
{
    /// <summary>
    /// Interaction logic for Voicemails.xaml
    /// </summary>
    public partial class Voicemails : UserControl
    {
        MySqlConnection dbConn = null;
        MySqlDataAdapter dbDataAdapter = null;
        DataTable dataFromDB = null;

        public Voicemails()
        {
            InitializeComponent();
            GetVoicemails();
        }

        private void GetVoicemails(){
            using (dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                            ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                            ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                            ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";"))
            {
                dbConn.Open();
                using (dbDataAdapter = new MySqlDataAdapter(ICResponse.Properties.Settings.Default.GetVoicemails, dbConn))
                {
                    dataFromDB = new DataTable();
                    dbDataAdapter.Fill(dataFromDB);
                    VoicemailsDataGrid.ItemsSource = dataFromDB.DefaultView;
                }
                dbConn.Close();
            }
        }
    }
}
