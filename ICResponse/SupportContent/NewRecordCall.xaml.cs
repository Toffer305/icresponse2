using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using MySql.Data.MySqlClient;
using FirstFloor.ModernUI.Windows.Controls;
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace ICResponse.SupportContent
{
    /// <summary>
    /// Interaction logic for NewRecordCall.xaml
    /// </summary>
    public partial class NewRecordCall : UserControl
    {
        MySqlConnection dbConn = null;
        MySqlCommand cmd = null;
        string selectedbrand = string.Empty;
        bool pointofnoreturn = false;
        private const string DisableScriptError = @"function noError() { return true;} window.onerror = noError;";
        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();
        string currentTime = string.Empty;
        string useThisUID = string.Empty;
        CookieContainer ACDcookies = new CookieContainer();

        public NewRecordCall()
        {
            InitializeComponent();
            NetSuiteFrame.Visibility = System.Windows.Visibility.Hidden;
            pointofnoreturn = false;
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 500);
            stopWatch.Start();
            dt.Start();
        }

        private void btnSubmitRecord_Click(object sender, RoutedEventArgs e)
        {
            if (this.recCompany.Text.Equals("") || this.recTechUser.Text.Equals("")
                || this.recPhone.Text.Equals("") || this.recIssue.Text.Equals("")
                || this.recResolution.Text.Equals(""))
            {
                ModernDialog msgbox = new ModernDialog();
                msgbox.Title = "All fields required.";
                msgbox.Content = "Please complete any empty fields.";
                msgbox.Buttons = new[] { msgbox.OkButton };
                msgbox.ShowDialog();
            }
            else
            {
                TimeSpan elapsedTime = new TimeSpan(0);
                if (isCase.IsChecked.Value && caseNumber.Text.Equals(""))
                {
                    ModernDialog msgbox = new ModernDialog();
                    msgbox.Title = "Missing NetSuite Case Number.";
                    msgbox.Content = "Please enter a NetSuite case number.";
                    msgbox.Buttons = new[] { msgbox.OkButton };
                    msgbox.ShowDialog();
                }
                else
                {
                    this.Cursor = Cursors.Wait;
                    if (stopWatch.IsRunning)
                    {
                        dt.Stop();
                        stopWatch.Stop();
                        elapsedTime = stopWatch.Elapsed;
                    }
                    try
                    {
                        cmd = null;
                        string preparedStatement = string.Empty;
                        string enteredCompany = removeSpecials(this.recCompany.Text);
                        string enteredContact = removeContactSpecials(this.recTechUser.Text);
                        string enteredPhone = removeAllButDigits(this.recPhone.Text);
                        string enteredEmail = removeSpecials(this.recEmail.Text);
                        string enteredIssue = removeSpecials(this.recIssue.Text);
                        string enteredResolution = removeSpecials(this.recResolution.Text);
                        int icheckflag = 0;
                        bool NScheckboxflag = isCase.IsChecked.Value;
                        if (NScheckboxflag)
                        {
                            icheckflag = 1;
                        }
                        dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer
                                                    + ";Database=" + ICResponse.Properties.Settings.Default.DBName
                                                    + ";Uid=" + ICResponse.Properties.Settings.Default.DBUser
                                                    + ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";");
                        dbConn.Open();
                        string stringTicketID = getCurrentCallTicketID();

                        //No UID Found, Create New Record
                        if (stringTicketID.Equals(""))
                        {
                            //Create new record                            
                            cmd = new MySqlCommand(ICResponse.Properties.Settings.Default.InsertCallRecord, dbConn);
                            cmd.Parameters.AddWithValue("?tech", ICResponse.Properties.Settings.Default.AgentFirstName
                                                                 + " " + ICResponse.Properties.Settings.Default.AgentLastName);
                            cmd.Parameters.AddWithValue("?status", "Not ACD");
                            cmd.Parameters.AddWithValue("?company", enteredCompany);
                            cmd.Parameters.AddWithValue("?contact", enteredContact);
                            cmd.Parameters.AddWithValue("?phone", enteredPhone);
                            cmd.Parameters.AddWithValue("?email", enteredEmail);
                            cmd.Parameters.AddWithValue("?date", System.DateTime.Today);
                            cmd.Parameters.AddWithValue("?startTime", System.DateTime.Now - elapsedTime);
                            cmd.Parameters.AddWithValue("?stopTime", System.DateTime.Now);
                            cmd.Parameters.AddWithValue("?holdtime", 0);
                            cmd.Parameters.AddWithValue("?duration", Math.Ceiling(elapsedTime.TotalMinutes));
                            cmd.Parameters.AddWithValue("?brand", selectedbrand);
                            cmd.Parameters.AddWithValue("?issue", enteredIssue);
                            cmd.Parameters.AddWithValue("?resolution", enteredResolution);
                            cmd.Parameters.AddWithValue("?iscase", icheckflag);
                            cmd.Parameters.AddWithValue("?casenumber", caseNumber.Text);
                        }
                        // UID Found update that record
                        else
                        {
                            if (uidEntryIsEmptyandMostRecent(stringTicketID))
                            {
                                cmd = new MySqlCommand(ICResponse.Properties.Settings.Default.UpdateCallRecord, dbConn);
                                cmd.Parameters.AddWithValue("?company", enteredCompany);
                                cmd.Parameters.AddWithValue("?contact", enteredContact);
                                cmd.Parameters.AddWithValue("?phone", enteredPhone);
                                cmd.Parameters.AddWithValue("?email", enteredEmail);
                                cmd.Parameters.AddWithValue("?brand", selectedbrand);
                                cmd.Parameters.AddWithValue("?issue", enteredIssue);
                                cmd.Parameters.AddWithValue("?resolution", enteredResolution);
                                cmd.Parameters.AddWithValue("?iscase", icheckflag);
                                cmd.Parameters.AddWithValue("?casenumber", caseNumber.Text);
                                cmd.Parameters.AddWithValue("?uid", stringTicketID);
                            }
                            else
                            {
                                //Create new record                            
                                cmd = new MySqlCommand(ICResponse.Properties.Settings.Default.InsertCallRecord, dbConn);
                                cmd.Parameters.AddWithValue("?tech", ICResponse.Properties.Settings.Default.AgentFirstName
                                                                     + " " + ICResponse.Properties.Settings.Default.AgentLastName);
                                cmd.Parameters.AddWithValue("?status", "Not ACD");
                                cmd.Parameters.AddWithValue("?company", enteredCompany);
                                cmd.Parameters.AddWithValue("?contact", enteredContact);
                                cmd.Parameters.AddWithValue("?phone", enteredPhone);
                                cmd.Parameters.AddWithValue("?email", enteredEmail);
                                cmd.Parameters.AddWithValue("?date", System.DateTime.Today);
                                cmd.Parameters.AddWithValue("?startTime", System.DateTime.Now - elapsedTime);
                                cmd.Parameters.AddWithValue("?stopTime", System.DateTime.Now);
                                cmd.Parameters.AddWithValue("?holdtime", 0);
                                cmd.Parameters.AddWithValue("?duration", Math.Ceiling(elapsedTime.TotalMinutes));
                                cmd.Parameters.AddWithValue("?brand", selectedbrand);
                                cmd.Parameters.AddWithValue("?issue", enteredIssue);
                                cmd.Parameters.AddWithValue("?resolution", enteredResolution);
                                cmd.Parameters.AddWithValue("?iscase", icheckflag);
                                cmd.Parameters.AddWithValue("?casenumber", caseNumber.Text);
                            }
                        }
                        dbConn.Open();
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        ModernDialog msgbox = new ModernDialog();
                        msgbox.Title = "Call Submitted";
                        msgbox.Content = "Duration: " + Math.Ceiling(elapsedTime.TotalMinutes) + " minute(s)";
                        msgbox.Buttons = new[] { msgbox.OkButton };
                        msgbox.ShowDialog();
                        //dbConn.Close();
                        SupportContent.AddCallConfirmation returntobase = new SupportContent.AddCallConfirmation();
                        this.Content = returntobase;
                    }
                    catch (MySqlException err)
                    {
                        ModernDialog msgbox = new ModernDialog();
                        msgbox.Title = "MySQL Exception";
                        msgbox.Buttons = new[] { msgbox.OkButton };
                        msgbox.ShowDialog();
                        System.Diagnostics.Debug.WriteLine(err);
                    }
                    finally
                    {
                        if (dbConn != null)
                        {
                            dbConn.Close(); //close the connection
                        }
                    }
                    this.Cursor = Cursors.Arrow;
                }

            }

        }

        private bool uidEntryIsEmptyandMostRecent(string UID)
        {
            //dbConn.Open();
            System.Diagnostics.Debug.WriteLine(dbConn.State);
            cmd = new MySqlCommand(ICResponse.Properties.Settings.Default.getTicketIDFromUID + "'" + UID + "'", dbConn);
            string UIDTicketID = cmd.ExecuteScalar().ToString();
            cmd = new MySqlCommand(ICResponse.Properties.Settings.Default.getMaxTicketForTech + "'"
                                   + ICResponse.Properties.Settings.Default.AgentFirstName + " "
                                   + ICResponse.Properties.Settings.Default.AgentLastName
                                   + "' OR tech='" + ICResponse.Properties.Settings.Default.LoginUsername + "'", dbConn);
            System.Diagnostics.Debug.WriteLine(UIDTicketID);
            string maxTicketID = cmd.ExecuteScalar().ToString();
            if (UIDTicketID == maxTicketID)
            {
                cmd = new MySqlCommand(ICResponse.Properties.Settings.Default.getCompanyFromUID + "'" + UID + "'", dbConn);
                if (cmd.ExecuteScalar().ToString().Equals(""))
                {
                    dbConn.Close();
                    return true;
                }
            }
            dbConn.Close();
            return false;
        }

        private string getCurrentCallTicketID()
        {
            Debug.Write("getTicketID()");
            string receivedTicketID = null;
            string preparedquery = ICResponse.Properties.Settings.Default.CallCurrentIDSelect + "'"
                                   + ICResponse.Properties.Settings.Default.AgentFirstName + " "
                                   + ICResponse.Properties.Settings.Default.AgentLastName
                                   + "' OR tech='" + ICResponse.Properties.Settings.Default.LoginUsername + "'";
            cmd = new MySqlCommand(preparedquery, dbConn);

            receivedTicketID = cmd.ExecuteScalar().ToString();

            return receivedTicketID;
        }

        private void dt_Tick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}",
                    ts.Hours, ts.Minutes, ts.Seconds);
                ClockTextBlock.Text = currentTime;
                if (ts.TotalSeconds > 59)
                {
                    pointofnoreturn = true;
                    btnCancelCall.Visibility = System.Windows.Visibility.Hidden;
                }
                if (ts.TotalMinutes > 10 && ts.TotalMinutes <= 14)
                {
                    ClockTextBlock.Foreground = new SolidColorBrush(Colors.Yellow);
                }
                if (ts.TotalMinutes > 14 && ts.TotalMinutes <= 17)
                {
                    ClockTextBlock.Foreground = new SolidColorBrush(Colors.Orange);
                }
                if (ts.TotalMinutes > 17)
                {
                    ClockTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void btnCancelCall_Click(object sender, RoutedEventArgs e)
        {
            if (pointofnoreturn)
            {
                //Do Nothing
            }
            else
            {
                CallCancel();
            }
        }

        private void CallCancel()
        {
            try
            {
                SupportContent.AddCallConfirmation returnfromcall = new SupportContent.AddCallConfirmation();
                this.Content = returnfromcall;
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

        private string removeSpecials(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (Char.IsLetterOrDigit(c) || c == '.' || c == '@' || c == ' ' || c == '_' || c == '/')
                { sb.Append(c); }
            }
            return sb.ToString();
        }

        private string removeContactSpecials(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (Char.IsLetter(c) || c == ' ')
                { sb.Append(c); }
            }
            return sb.ToString();
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

        private void brandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedbrand = null;
            var cb = sender as ComboBox;
            ComboBoxItem Item = (ComboBoxItem)brandComboBox.SelectedItem;
            selectedbrand = Item.Tag.ToString();
        }

        private void isCase_Checked(object sender, RoutedEventArgs e)
        {
            caseNumber.Visibility = System.Windows.Visibility.Visible;
            NetSuiteFrame.Visibility = System.Windows.Visibility.Visible;
            NetSuiteFrame.Navigate(new Uri("https://system.netsuite.com/pages/customerlogin.jsp", UriKind.Absolute));
        }

        private void isCase_Unchecked(object sender, RoutedEventArgs e)
        {
            caseNumber.Visibility = System.Windows.Visibility.Hidden;
            NetSuiteFrame.Visibility = System.Windows.Visibility.Hidden;
        }

        private void NetSuiteFrame_Loaded(object sender, RoutedEventArgs e)
        {
            NetSuiteFrame.Navigated += new NavigatedEventHandler(NetSuiteFrame_Navigated);
        }

        private void NetSuiteFrame_Navigated(object sender, NavigationEventArgs e)
        {
            HideJsScriptErrors((WebBrowser)sender);

        }

        public void HideJsScriptErrors(WebBrowser wb)
        {
            // IWebBrowser2 interface
            // Exposes methods that are implemented by the WebBrowser control  
            // Searches for the specified field, using the specified binding constraints.
            FieldInfo fld = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fld == null)
                return;
            object obj = fld.GetValue(wb);
            if (obj == null)
                return;
            // Silent: Sets or gets a value that indicates whether the object can display dialog boxes.
            // HRESULT IWebBrowser2::get_Silent(VARIANT_BOOL *pbSilent);HRESULT IWebBrowser2::put_Silent(VARIANT_BOOL bSilent);
            obj.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, obj, new object[] { true });
        }

        private void ModernWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (ICResponse.Properties.Settings.Default.SelectedThemeDisplayName.Equals("dark"))
            {
                btnCancelCall.Background = Brushes.Gray;
                btnSubmitRecord.Background = Brushes.Gray;
            }
            if (ICResponse.Properties.Settings.Default.SelectedThemeDisplayName.Equals("light"))
            {
                btnCancelCall.Background = Brushes.White;
                btnSubmitRecord.Background = Brushes.White;
            }
        }


    }
}
