using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;


namespace ICResponse.Pages
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LogIn : UserControl
    {

        MySqlConnection dbConn = null;
        MySqlDataReader dbReader = null;
        MySqlCommand cmd = null;

        public LogIn()
        {
            InitializeComponent();
            
        }

        private void Button_Click_Login(object sender, RoutedEventArgs e)
        {
            attemptLogin();                       
        }

        private async void attemptLogin()
        {
            try
            {
                string enteredEmail = this.Login_Email.Text + "@icrealtime.com";
                string enteredPassword = this.Login_Password.Password;
                string receivedPassword = string.Empty;
                string receivedLevel = string.Empty;

                // DB Connection
                dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer + ";Database=" + ICResponse.Properties.Settings.Default.DBName + ";Uid=" + ICResponse.Properties.Settings.Default.DBUser + ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";");
                await dbConn.OpenAsync();
                // DB Command Preparation
                cmd = new MySqlCommand(ICResponse.Properties.Settings.Default.SelectAgentByEmail, dbConn);
                cmd.Parameters.AddWithValue("@email", enteredEmail);
                cmd.Prepare();
                // DB Reader Execution
                dbReader = (MySqlDataReader) await cmd.ExecuteReaderAsync();
                // Loop through results
                while (await dbReader.ReadAsync())
                {
                    receivedPassword = Convert.ToString(dbReader["password"]);
                    receivedLevel = Convert.ToString(dbReader["level"]);
                }

                if (enteredPassword != receivedPassword)
                {
                    ModernDialog msgbox = new ModernDialog();
                    msgbox.Title = "Not found";
                    msgbox.Content = "Incorrect Username or Password.";
                    msgbox.Buttons = new[] { msgbox.OkButton };
                    msgbox.ShowDialog();
                    dbReader.Close();
                }
                else
                {
                    ICResponse.Properties.Settings.Default.LoginUsername = Convert.ToString(dbReader["email"]);
                    ICResponse.Properties.Settings.Default.AgentID = Convert.ToInt32(dbReader["agentID"]);
                    ICResponse.Properties.Settings.Default.AgentFirstName = Convert.ToString(dbReader["first"]);
                    ICResponse.Properties.Settings.Default.AgentLastName = Convert.ToString(dbReader["last"]);
                    ICResponse.Properties.Settings.Default.LoginPassword = receivedPassword;
                    ICResponse.Properties.Settings.Default.LoginLevel = receivedLevel;
                    ICResponse.Properties.Settings.Default.LoginNick = ICResponse.Properties.Settings.Default.LoginUsername.Replace("@icrealtime.com", "");
                    ICResponse.Properties.Settings.Default.Save();
                    dbReader.Close();

                    var mainWindow = new MainWindow();
                    var loginWindow = Window.GetWindow(this);
                    mainWindow.Show();
                    if (loginWindow != null) loginWindow.Close();
                }

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

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
               attemptLogin();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Sets keyboard focus on the first Button in the sample.
            Keyboard.Focus(Login_Password);
            lgnLogin.CaptureMouse();
            lgnLogin.ReleaseMouseCapture();
            Login_Password.Password = ICResponse.Properties.Settings.Default.LoginPassword;

            if (ICResponse.Properties.Settings.Default.SelectedThemeDisplayName.Equals("dark"))
            {
                lgnLogin.Background = Brushes.Gray;
            }
            if (ICResponse.Properties.Settings.Default.SelectedThemeDisplayName.Equals("light"))
            {
                lgnLogin.Background = Brushes.White;
            }
        }

        
    }

}
