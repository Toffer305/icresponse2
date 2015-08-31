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
using System.IO;
using FirstFloor.ModernUI.Windows.Controls;

namespace ICResponse.Pages
{
    /// <summary>
    /// Interaction logic for MyCalls.xaml
    /// </summary>
    public partial class MyCalls : UserControl
    {
        MySqlConnection dbConn = null;
        MySqlDataAdapter dbDataAdapter = null;
        DataTable dataFromDB = null;
        string begindateforpicker = string.Empty;
        string enddateforpicker = string.Empty;
        string dateformat = "yyyy-MM-dd";

        public MyCalls()
        {
            InitializeComponent();            
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (ICResponse.Properties.Settings.Default.SelectedThemeDisplayName.Equals("dark"))
            {
                MyCallsDataGrid.Background = Brushes.Gray;
            }
            if (ICResponse.Properties.Settings.Default.SelectedThemeDisplayName.Equals("light"))
            {
                MyCallsDataGrid.Background = Brushes.White;
            }            
            getCallsFromDB();            
        }

        private async void getCallsFromDB()
        {
            dataFromDB = new DataTable();
            string preparedquery = "SELECT * FROM calls WHERE tech='" + ICResponse.Properties.Settings.Default.AgentFirstName + " " + ICResponse.Properties.Settings.Default.AgentLastName + "' OR tech='" + ICResponse.Properties.Settings.Default.LoginUsername + "' ORDER BY ticketID DESC LIMIT 100";

            using (dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                            ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                            ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                            ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";"))
            {
                await dbConn.OpenAsync();
            }

            using (dbDataAdapter = new MySqlDataAdapter(preparedquery, dbConn))
            {
                await dbDataAdapter.FillAsync(dataFromDB);
            }

            MyCallsDataGrid.ItemsSource = dataFromDB.DefaultView;            
            dbConn.Close();
        }

        private void OnAutoGeneratingColumn(object send, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as System.Windows.Controls.DataGridTextColumn).Binding.StringFormat = "MM-dd-yyyy";
            }
        }  

        private async void btnExportCalls_Click_1(object sender, RoutedEventArgs e)
        {
            if (begindateforpicker.Equals("") || enddateforpicker.Equals(""))
            {
                ModernDialog msgbox = new ModernDialog();
                msgbox.Title = "Pick Date Range";
                msgbox.Content = "Please select a range of dates.";
                msgbox.Buttons = new[] { msgbox.OkButton };
                msgbox.ShowDialog();
            }
            else
            {                
                string reportpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                StringBuilder reportbuilder = new StringBuilder();
                StringBuilder vmbuilder = new StringBuilder();
                int i = 0;
                int contactedcounter = 0;
                //int voicemailcounter = 0;
                string datecolumnvalue = string.Empty;
                string monthformat = string.Empty;
                string finalmonthformat = string.Empty;
                string preparedquery = "SELECT * FROM `calls` WHERE tech='" + ICResponse.Properties.Settings.Default.AgentFirstName 
                                        + " " 
                                        + ICResponse.Properties.Settings.Default.AgentLastName 
                                        + "' OR tech='" 
                                        + ICResponse.Properties.Settings.Default.LoginUsername 
                                        + "'AND date BETWEEN '" 
                                        + begindateforpicker 
                                        + "' AND '" 
                                        + enddateforpicker + "'";
                dataFromDB.Clear();
                dataFromDB = new DataTable();

                using (dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                           ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                           ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                           ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";"))
                {
                    await dbConn.OpenAsync();
                }

                using (dbDataAdapter = new MySqlDataAdapter(preparedquery, dbConn))
                {
                    await dbDataAdapter.FillAsync(dataFromDB);
                }                
                
                MyCallsDataGrid.ItemsSource = dataFromDB.DefaultView;                

                foreach (DataRow dRow in dataFromDB.Rows)
                {
                    i++;
                    string BBcompany = String.Empty;
                    string BBcontact = String.Empty;
                    string BBduration = String.Empty;

                    //if (dRow["voicemail"].ToString().Equals("Voicemail"))
                    //{
                    //    BBcompany = dRow["company"].ToString();
                    //    BBcontact = dRow["contact"].ToString();
                    //    BBduration = dRow["duration"].ToString();

                    //    vmbuilder.AppendLine(dRow["ticketID"].ToString());
                    //    vmbuilder.AppendLine(dRow["origin"].ToString());
                    //    vmbuilder.AppendLine("[b]" + BBcompany + "[/b]"); // [b]
                    //    vmbuilder.AppendLine("[i]" + BBcontact + "[/i]"); // [i]
                    //    vmbuilder.AppendLine(dRow["phone"].ToString());

                    //    datecolumnvalue = dRow["date"].ToString();
                    //    monthformat = datecolumnvalue.Replace("12:00:00 AM", "");
                    //    vmbuilder.AppendLine(monthformat);

                    //    vmbuilder.AppendLine(dRow["startTime"].ToString());
                    //    vmbuilder.AppendLine(dRow["stopTime"].ToString());
                    //    vmbuilder.AppendLine("[u]" + BBduration + "[/u]"); // [u]
                    //    vmbuilder.AppendLine(dRow["voicemail"].ToString());
                    //    vmbuilder.AppendLine();
                    //    vmbuilder.AppendLine();
                    //    voicemailcounter++;
                    //}
                    //else
                    //{
                        BBcompany = dRow["company"].ToString();
                        BBcontact = dRow["contact"].ToString();
                        BBduration = dRow["duration"].ToString();

                        reportbuilder.AppendLine(dRow["ticketID"].ToString());
                        //reportbuilder.AppendLine(dRow["origin"].ToString());
                        reportbuilder.AppendLine("[b]" + BBcompany + "[/b]"); // [b]
                        reportbuilder.AppendLine("[i]" + BBcontact + "[/i]"); // [i]
                        reportbuilder.AppendLine(dRow["phone"].ToString());

                        datecolumnvalue = dRow["date"].ToString();
                        monthformat = datecolumnvalue.Replace("12:00:00 AM", "");
                        reportbuilder.AppendLine(monthformat);

                        reportbuilder.AppendLine(dRow["startTime"].ToString());
                        reportbuilder.AppendLine(dRow["stopTime"].ToString());
                        reportbuilder.AppendLine("[u]" + BBduration + "[/u]"); // [u]
                        //reportbuilder.AppendLine(dRow["voicemail"].ToString());
                        reportbuilder.AppendLine(dRow["issue"].ToString());
                        reportbuilder.AppendLine(dRow["resolution"].ToString());
                        reportbuilder.AppendLine();
                        reportbuilder.AppendLine();
                        contactedcounter++;
                    //}

                }
                dbConn.Close();
                vmbuilder.AppendLine("Total Contacted: " + contactedcounter.ToString());
                //vmbuilder.AppendLine("Total Voicemails: " + voicemailcounter.ToString());
                vmbuilder.AppendLine("Total Results: " + dataFromDB.Rows.Count.ToString());

                string datetimepatt = @"MM_dd_yyyy_hh_mm_ss_tt";
                DateTime savenow = System.DateTime.Now;
                string currdatetime = savenow.ToString(datetimepatt);
                string partialpath = @"\ICResponse_Report_" + currdatetime + ".txt";
                string fullpath = reportpath + partialpath;

                using (StreamWriter outfile = new StreamWriter(fullpath, true))
                {
                    await outfile.WriteAsync(reportbuilder.ToString());
                    await outfile.WriteAsync(vmbuilder.ToString());
                    outfile.Close();
                }

                ModernDialog msgbox = new ModernDialog();
                msgbox.Title = "Export Completed";
                msgbox.Content = fullpath + " , Results:" + dataFromDB.Rows.Count.ToString();
                msgbox.Buttons = new[] { msgbox.OkButton };
                msgbox.ShowDialog();
                this.Cursor = Cursors.Arrow;               
            }
        }

        private void DatePickerBegin_SelectedDateChanged_1(object sender, SelectionChangedEventArgs e)
        {
            begindateforpicker = string.Empty;
            var picker = sender as DatePicker;
            DateTime? date = picker.SelectedDate;
            if (date == null)
            {
                picker.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                begindateforpicker = date.Value.ToString(dateformat);
            }
        }

        private void DatePickerEnd_SelectedDateChanged_1(object sender, SelectionChangedEventArgs e)
        {
            enddateforpicker = string.Empty;
            var picker = sender as DatePicker;
            DateTime? date = picker.SelectedDate;
            if (date == null)
            {
                picker.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                enddateforpicker = date.Value.ToString(dateformat);
            }
        }       

    }
}
