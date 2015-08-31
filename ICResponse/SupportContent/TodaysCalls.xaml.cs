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
using MySql.Data.MySqlClient;

namespace ICResponse.SupportContent
{
    /// <summary>
    /// Interaction logic for TodaysCalls.xaml
    /// </summary>
    public partial class TodaysCalls : UserControl
    {
        MySqlConnection dbConn = null;
        MySqlDataAdapter dbDataAdapter = null;
        DataTable dataFromDB = null;

        public TodaysCalls()
        {
            InitializeComponent();
            this.Cursor = Cursors.Wait;
            GetTodaysCalls();
            this.Cursor = Cursors.Arrow;
        }

        private void OnAutoGeneratingColumn(object send, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as System.Windows.Controls.DataGridTextColumn).Binding.StringFormat = "MM-dd-yyyy";
            }
        }

        private void GetTodaysCalls()
        {
            dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                            ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                            ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                            ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";");
            dbConn.Open();
            dbDataAdapter = new MySqlDataAdapter(ICResponse.Properties.Settings.Default.TodayCallSelect, dbConn);
            dataFromDB = new DataTable();
            dbDataAdapter.Fill(dataFromDB);
            TodaysCallsDataGrid.ItemsSource = dataFromDB.DefaultView;
            dbConn.Close();
        }
    }
}
