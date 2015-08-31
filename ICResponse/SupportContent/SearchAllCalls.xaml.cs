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
using FirstFloor.ModernUI.Windows.Controls;

namespace ICResponse.Pages
{
    /// <summary>
    /// Interaction logic for SearchCalls.xaml
    /// </summary>
    public partial class SearchCalls : UserControl
    {
        MySqlConnection dbConn = null;
        MySqlDataAdapter dbDataAdapter = null;
        DataTable dataFromDB = null;
        string selectedcategory = string.Empty;
        string mySearchKey = string.Empty;
        string selectedorigin = string.Empty;
        string selectedbrand = string.Empty;
        string begindateforpicker = string.Empty;
        string enddateforpicker = string.Empty;
        string dateformat = "yyyy-MM-dd";
        string generalquery = string.Empty;
        string daterangequery = string.Empty;


        public SearchCalls()
        {
            InitializeComponent();            
            Keyboard.Focus(SearchKey);
        }

        private void CategorySearchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedcategory = null;
            var cb = sender as ComboBox;
            ComboBoxItem Item = (ComboBoxItem)CategorySearchComboBox.SelectedItem;
            selectedcategory = Item.Tag.ToString();
            if (selectedcategory.Equals("ticketID"))
            {
                KeywordStackPanel.Visibility = System.Windows.Visibility.Visible;
                OriginCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DatePickerStackPanel.Visibility = System.Windows.Visibility.Hidden;
                BrandCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DescriptionLabel.Text = "ICResponse Ticket ID Number:";
                mySearchKey = removeAllButDigits(this.SearchKey.Text);
            }
            if (selectedcategory.Equals("tech"))
            {
                KeywordStackPanel.Visibility = System.Windows.Visibility.Visible;
                OriginCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                BrandCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DatePickerStackPanel.Visibility = System.Windows.Visibility.Hidden;
                DescriptionLabel.Text = "ICR Technician Name (or email username):";
                mySearchKey = removeAllButDigits(this.SearchKey.Text);
            }
            if (selectedcategory.Equals("origin"))
            {
                KeywordStackPanel.Visibility = System.Windows.Visibility.Hidden;
                OriginCategoryComboBox.Visibility = System.Windows.Visibility.Visible;
                BrandCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DatePickerStackPanel.Visibility = System.Windows.Visibility.Hidden;
                mySearchKey = selectedorigin;
            }
            if (selectedcategory.Equals("brand"))
            {
                KeywordStackPanel.Visibility = System.Windows.Visibility.Hidden;
                BrandCategoryComboBox.Visibility = System.Windows.Visibility.Visible;
                OriginCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DatePickerStackPanel.Visibility = System.Windows.Visibility.Hidden;
                mySearchKey = selectedbrand;
            }
            if (selectedcategory.Equals("company"))
            {
                KeywordStackPanel.Visibility = System.Windows.Visibility.Visible;
                OriginCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DatePickerStackPanel.Visibility = System.Windows.Visibility.Hidden;
                BrandCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DescriptionLabel.Text = "Company Name:";
                mySearchKey = removeSpecials(this.SearchKey.Text);
            }
            if (selectedcategory.Equals("contact"))
            {
                KeywordStackPanel.Visibility = System.Windows.Visibility.Visible;
                OriginCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DatePickerStackPanel.Visibility = System.Windows.Visibility.Hidden;
                BrandCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DescriptionLabel.Text = "Contact Name:";
                mySearchKey = removeSpecials(this.SearchKey.Text);
            }
            if (selectedcategory.Equals("phone"))
            {
                KeywordStackPanel.Visibility = System.Windows.Visibility.Visible;
                OriginCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DatePickerStackPanel.Visibility = System.Windows.Visibility.Hidden;
                BrandCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DescriptionLabel.Text = "Phone Number:";
                mySearchKey = removeAllButDigits(this.SearchKey.Text);
            }
            if (selectedcategory.Equals("date"))
            {
                KeywordStackPanel.Visibility = System.Windows.Visibility.Hidden;
                OriginCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                BrandCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DatePickerStackPanel.Visibility = System.Windows.Visibility.Visible;
            }
            if (selectedcategory.Equals("issue"))
            {
                KeywordStackPanel.Visibility = System.Windows.Visibility.Visible;
                OriginCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                BrandCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DatePickerStackPanel.Visibility = System.Windows.Visibility.Hidden;
                DescriptionLabel.Text = "Keyword To Search:";
                mySearchKey = removeSpecials(this.SearchKey.Text);
            }
            if (selectedcategory.Equals("resolution"))
            {
                KeywordStackPanel.Visibility = System.Windows.Visibility.Visible;
                OriginCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                BrandCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DatePickerStackPanel.Visibility = System.Windows.Visibility.Hidden;
                DescriptionLabel.Text = "Keyword To Search:";
                mySearchKey = removeSpecials(this.SearchKey.Text);
            }
            if (selectedcategory.Equals("casenumber"))
            {
                KeywordStackPanel.Visibility = System.Windows.Visibility.Visible;
                OriginCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DatePickerStackPanel.Visibility = System.Windows.Visibility.Hidden;
                BrandCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DescriptionLabel.Text = "NetSuite Case Number:";
                mySearchKey = removeSpecials(this.SearchKey.Text);
            }
            if (selectedcategory.Equals("allnetsuite"))
            {
                OriginCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                DatePickerStackPanel.Visibility = System.Windows.Visibility.Hidden;
                BrandCategoryComboBox.Visibility = System.Windows.Visibility.Hidden;
                KeywordStackPanel.Visibility = System.Windows.Visibility.Hidden;
            }
            
        }

        private void  btnStartSearch_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            PerformSearch();
            this.Cursor = Cursors.Arrow;
        }

        private void PerformSearch()
        {
            dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer + ";Database=" + ICResponse.Properties.Settings.Default.DBName + ";Uid=" + ICResponse.Properties.Settings.Default.DBUser + ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";");
            dbConn.Open();

            if (selectedcategory.Equals("ticketID"))
            {
                mySearchKey = removeAllButDigits(this.SearchKey.Text);
                generalquery = "SELECT ticketID, tech, brand, company,contact,phone,email, date, startTime, stopTime, holdtime, duration, issue, resolution, iscase, casenumber, queuename FROM calls WHERE " + selectedcategory + " LIKE'%" + mySearchKey + "%' ORDER BY ticketID DESC";
                dbDataAdapter = new MySqlDataAdapter(generalquery, dbConn);
            }
            if (selectedcategory.Equals("tech"))
            {
                mySearchKey = this.SearchKey.Text;
                generalquery = "SELECT ticketID, tech, brand, company,contact,phone,email, date, startTime, stopTime, holdtime, duration, issue, resolution, iscase, casenumber, queuename FROM calls WHERE " + selectedcategory + " LIKE'%" + mySearchKey + "%' ORDER BY ticketID DESC";
                dbDataAdapter = new MySqlDataAdapter(generalquery, dbConn);
            }
            
            if (selectedcategory.Equals("brand"))
            {
                mySearchKey = selectedbrand;
                generalquery = "SELECT ticketID, tech, brand, company,contact,phone,email, date, startTime, stopTime, holdtime, duration, issue, resolution, iscase, casenumber, queuename FROM calls WHERE " + selectedcategory + " LIKE'%" + mySearchKey + "%' ORDER BY ticketID DESC";
                dbDataAdapter = new MySqlDataAdapter(generalquery, dbConn);
            }
            if (selectedcategory.Equals("company"))
            {
                mySearchKey = removeSpecials(this.SearchKey.Text);
                generalquery = "SELECT ticketID, tech, brand, company,contact,phone,email, date, startTime, stopTime, holdtime, duration, issue, resolution, iscase, casenumber, queuename FROM calls WHERE " + selectedcategory + " LIKE'%" + mySearchKey + "%' ORDER BY ticketID DESC";
                dbDataAdapter = new MySqlDataAdapter(generalquery, dbConn);
            }
            if (selectedcategory.Equals("contact"))
            {
                mySearchKey = removeSpecials(this.SearchKey.Text);
                generalquery = "SELECT ticketID, tech, brand, company,contact,phone,email, date, startTime, stopTime, holdtime, duration, issue, resolution, iscase, casenumber, queuename FROM calls WHERE " + selectedcategory + " LIKE'%" + mySearchKey + "%' ORDER BY ticketID DESC";
                dbDataAdapter = new MySqlDataAdapter(generalquery, dbConn);
            }
            if (selectedcategory.Equals("phone"))
            {
                mySearchKey = removeAllButDigits(this.SearchKey.Text);
                generalquery = "SELECT ticketID, tech, brand, company,contact,phone,email, date, startTime, stopTime, holdtime, duration, issue, resolution, iscase, casenumber, queuename FROM calls WHERE " + selectedcategory + " LIKE'%" + mySearchKey + "%' ORDER BY ticketID DESC";
                dbDataAdapter = new MySqlDataAdapter(generalquery, dbConn);
            }
            if (selectedcategory.Equals("date"))
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
                    daterangequery = "SELECT ticketID, tech, brand, company,contact,phone,email, date, startTime, stopTime, holdtime, duration, issue, resolution, iscase, casenumber, queuename FROM `calls` WHERE date BETWEEN '" + begindateforpicker + "' AND '" + enddateforpicker + "' ORDER BY ticketID DESC";
                    dbDataAdapter = new MySqlDataAdapter(daterangequery, dbConn);
                }

            }
            if (selectedcategory.Equals("issue"))
            {
                mySearchKey = removeSpecials(this.SearchKey.Text);
                generalquery = "SELECT ticketID, tech, brand, company,contact,phone,email, date, startTime, stopTime, holdtime, duration, issue, resolution, iscase, casenumber, queuename FROM calls WHERE " + selectedcategory + " LIKE'%" + mySearchKey + "%' ORDER BY ticketID DESC";
                dbDataAdapter = new MySqlDataAdapter(generalquery, dbConn);
            }
            if (selectedcategory.Equals("resolution"))
            {
                mySearchKey = removeSpecials(this.SearchKey.Text);
                generalquery = "SELECT ticketID, tech, brand, company,contact,phone,email, date, startTime, stopTime, holdtime, duration, issue, resolution, iscase, casenumber, queuename FROM calls WHERE " + selectedcategory + " LIKE'%" + mySearchKey + "%' ORDER BY ticketID DESC";
                dbDataAdapter = new MySqlDataAdapter(generalquery, dbConn);
            }
            if (selectedcategory.Equals("casenumber"))
            {
                mySearchKey = removeSpecials(this.SearchKey.Text);
                generalquery = "SELECT ticketID, tech, brand, company,contact,phone,email, date, startTime, stopTime, holdtime, duration, issue, resolution, iscase, casenumber, queuename FROM calls WHERE " + selectedcategory + " LIKE'%" + mySearchKey + "%' ORDER BY ticketID DESC";
                dbDataAdapter = new MySqlDataAdapter(generalquery, dbConn);
            }
            if (selectedcategory.Equals("allnetsuite"))
            {
                generalquery = "SELECT ticketID, tech, brand, company,contact,phone,email, date, startTime, stopTime, holdtime, duration, issue, resolution, iscase, casenumber, queuename FROM calls WHERE iscase='1' ORDER BY ticketID DESC";
                dbDataAdapter = new MySqlDataAdapter(generalquery, dbConn);
            }

            if (mySearchKey.Equals("", StringComparison.OrdinalIgnoreCase) && !selectedcategory.Equals("date") && !selectedcategory.Equals("allnetsuite"))
            {
                // Inform user to input a key
                ModernDialog msgbox = new ModernDialog();
                msgbox.Title = "Required";
                msgbox.Content = "Please input a searchable term.";
                msgbox.Buttons = new[] { msgbox.OkButton };
                msgbox.ShowDialog();
            }
            else
            {
                dataFromDB = new DataTable();
                dbDataAdapter.Fill(dataFromDB);
                SearchCallsDataGrid.ItemsSource = dataFromDB.DefaultView;                
                dbConn.Close();
            }
            
        }

        //private void OnAutoGeneratingColumn(object send, DataGridAutoGeneratingColumnEventArgs e)
        //{
        //    if (e.PropertyType == typeof(DateTime))
        //    {
        //       (e.Column as System.Windows.Controls.DataGridTextColumn).Binding.StringFormat = "MM-dd-yyyy";
        //    }
        //}

        private void btnExportResults_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            string reportpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string datetimepatt = @"MM_dd_yyyy_hh_mm_ss_tt";
            DateTime savenow = System.DateTime.Now;
            string currdatetime = savenow.ToString(datetimepatt);
            string partialpath = @"\ICResponse_Search_" + currdatetime + ".csv";
            string fullpath = reportpath + partialpath;

            SearchCallsDataGrid.SelectAllCells();
            SearchCallsDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
            ApplicationCommands.Copy.Execute(null, SearchCallsDataGrid);
            SearchCallsDataGrid.UnselectAllCells();
            String results = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            Clipboard.Clear();
            System.IO.StreamWriter file = new System.IO.StreamWriter(fullpath);
            file.WriteLine(results);
            file.Close();
            this.Cursor = Cursors.Arrow;
            ModernDialog msgbox = new ModernDialog();
            msgbox.Title = "Export Completed";
            msgbox.Content = fullpath + ", Results:" + dataFromDB.Rows.Count.ToString();
            msgbox.Buttons = new[] { msgbox.OkButton };
            msgbox.ShowDialog();
        }

        private string removeAllButDigits(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (Char.IsDigit(c))
                { sb.Append(c); }
            }
            return sb.ToString();
        }

        private string removeSpecials(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (Char.IsLetterOrDigit(c) || c == '.' || c == '@' || c == ' ' || c == '_')
                { sb.Append(c); }
            }
            return sb.ToString();
        }

        private void OriginCategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedorigin = null;
            var cb = sender as ComboBox;
            ComboBoxItem Item = (ComboBoxItem)OriginCategoryComboBox.SelectedItem;
            selectedorigin = Item.Tag.ToString();
        }

        private void datepickersearchbegin_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
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

        private void datepickersearchend_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
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

        private void getAllCalls()
        {
            try
            {               
                makeCallConnection();
                getCallsFromDB();                
            }
            catch (MySqlException err)
            {
                System.Diagnostics.Debug.WriteLine(err);
            }
            finally
            {
                if (dbConn != null)
                {
                    dbConn.Close(); //close the connection
                }
            }
        }

        private void makeCallConnection()
        {
            dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                            ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                            ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                            ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";");
            dbConn.Open();
        }

        private void getCallsFromDB()
        {
            dbDataAdapter = new MySqlDataAdapter(ICResponse.Properties.Settings.Default.GetAllCalls, dbConn);
            dataFromDB = new DataTable();
            dbDataAdapter.Fill(dataFromDB);
            SearchCallsDataGrid.ItemsSource = dataFromDB.DefaultView;
            dbConn.Close();            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ICResponse.Properties.Settings.Default.SelectedThemeDisplayName.Equals("dark"))
            {
                SearchCallsDataGrid.Background = Brushes.Gray;
                btnStartSearch.Background = Brushes.Gray;
                btnExportResults.Background = Brushes.Gray;
            }
            if (ICResponse.Properties.Settings.Default.SelectedThemeDisplayName.Equals("light"))
            {
                SearchCallsDataGrid.Background = Brushes.White;
                btnStartSearch.Background = Brushes.White;
                btnExportResults.Background = Brushes.White;
            }
            
            getAllCalls();
            
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                
                PerformSearch();
                
            }
        }

        private void BrandCategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedbrand = null;
            var cb = sender as ComboBox;
            ComboBoxItem Item = (ComboBoxItem)BrandCategoryComboBox.SelectedItem;
            selectedbrand = Item.Tag.ToString();
        }
        
    }
}
