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
using FirstFloor.ModernUI.Windows.Controls;

namespace ICResponse.Pages
{
    /// <summary>
    /// Interaction logic for ConnectionSettings.xaml
    /// </summary>
    public partial class ConnectionSettings : UserControl
    {
        MySqlConnection dbConn = null;
        public ConnectionSettings()
        {
            InitializeComponent();

            settingsPassword.Password = ICResponse.Properties.Settings.Default.DBPass;
        }

        private void setttingsSubmit_Click(object sender, RoutedEventArgs e)
        {
            
            saveChanges();
            
        }

        private void saveChanges()
        {
            string address = this.settingsAddress.Text;
            string name = this.settingsName.Text;
            string user = this.settingsUser.Text;
            string password = this.settingsPassword.Password;

            ICResponse.Properties.Settings.Default.DBServer = address;
            ICResponse.Properties.Settings.Default.DBName = name;
            ICResponse.Properties.Settings.Default.DBUser = user;
            ICResponse.Properties.Settings.Default.DBPass = password;
            ICResponse.Properties.Settings.Default.Save();
        }

        private void goBackToLogin()
        {
            LogIn loginPage = new LogIn();
            this.Content = loginPage;
        }

        private void settingsTest_Click(object sender, RoutedEventArgs e)
        {
            if (attemptLogin())
            {
                ModernDialog msgbox = new ModernDialog();
                msgbox.Title = "Success!";
                msgbox.Content = "Test Connection Successful.";
                msgbox.Buttons = new[] { msgbox.OkButton };
                msgbox.ShowDialog();
                
                goBackToLogin();
            }
            else
            {
                
                ModernDialog msgbox = new ModernDialog();
                msgbox.Title = "FAILED";
                msgbox.Content = "Check Settings.";
                msgbox.Buttons = new[] { msgbox.OkButton };
                msgbox.ShowDialog();
            }
        }

        private bool attemptLogin()
        {
            try
            {
                dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer
                                             + ";Database=" + ICResponse.Properties.Settings.Default.DBName
                                             + ";Uid=" + ICResponse.Properties.Settings.Default.DBUser
                                             + ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";");
                dbConn.Open();                
                return true;
            }
            catch (MySqlException err)
            {
                System.Diagnostics.Debug.WriteLine(err);
                return false;
            }
            finally
            {
                if (dbConn != null)
                {
                    dbConn.Close(); //close the connection                    
                }
            }
        }

        private void settingsCancel_Click(object sender, RoutedEventArgs e)
        {
            LogIn loginPage = new LogIn();
            this.Content = loginPage;
        }

        
    }
}
