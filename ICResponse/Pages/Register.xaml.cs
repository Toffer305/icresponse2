using FirstFloor.ModernUI.Windows.Controls;
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


namespace ICResponse.Pages
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Registration : UserControl
    {       

        MySqlConnection dbConn = null;

        public Registration()
        {
            InitializeComponent();
        }


        private void btnSubmitReg_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            attemptRegistration();
            this.Cursor = Cursors.Arrow;
        }

        private void attemptRegistration(){
            try
            {
                if (hasSpecials(this.regPassword.Password) || hasSpecials(this.regConfirmPassword.Password))
                {
                    showSpecialsDialog();
                }
                else
                {
                    string enteredFirstName = removeSpecials(this.regFirstName.Text);
                    string enteredLastName = removeSpecials(this.regLastName.Text);
                    string enteredEmail = removeSpecials(this.regEmail.Text + "@icrealtime.com");
                    string enteredPassword = removeSpecials(this.regPassword.Password);
                    string enteredConfirmPassword = removeSpecials(this.regConfirmPassword.Password);

                    if (enteredPassword == enteredConfirmPassword)
                    {
                        registrationCommit(enteredFirstName, enteredLastName, enteredEmail, enteredPassword);
                    }
                    else
                    {
                        showMisMatchDialog();
                    }
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

        private void registrationCommit(string enteredFirstName, string enteredLastName, string enteredEmail, string enteredPassword)
        {
            using (dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer + ";Database=" + ICResponse.Properties.Settings.Default.DBName + ";Uid=" + ICResponse.Properties.Settings.Default.DBUser + ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";"))
            {
                dbConn.Open();

                using (MySqlCommand cmd = new MySqlCommand(ICResponse.Properties.Settings.Default.AgentInsert, dbConn))
                {
                    cmd.Parameters.AddWithValue("?first", enteredFirstName);
                    cmd.Parameters.AddWithValue("?last", enteredLastName);
                    cmd.Parameters.AddWithValue("?email", enteredEmail);
                    cmd.Parameters.AddWithValue("?password", enteredPassword);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    showSuccessDialog();
                    returnToLoginPage();
                }

            }

        }

        private void returnToLoginPage()
        {
            LogIn loginPage = new LogIn();
            this.Content = loginPage;
        }

        private void showMisMatchDialog()
        {
            ModernDialog msgbox = new ModernDialog();
            msgbox.Title = "Error";
            msgbox.Content = "Passwords do not match.";
            msgbox.Buttons = new[] { msgbox.OkButton };
            msgbox.ShowDialog();
        }

        private void showSpecialsDialog()
        {
            ModernDialog msgbox = new ModernDialog();
            msgbox.Title = "Error";
            msgbox.Content = "Passwords must be letters and digits only.";
            msgbox.Buttons = new[] { msgbox.OkButton };
            msgbox.ShowDialog();
        }

        private void showSuccessDialog()
        {
            ModernDialog msgbox = new ModernDialog();
            msgbox.Title = "Success";
            msgbox.Content = "You are now registered!";
            msgbox.Buttons = new[] { msgbox.OkButton };
            msgbox.ShowDialog();
        }

        private string removeSpecials(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (Char.IsLetterOrDigit(c) || c == '.' || c == '@' || c == '_')
                { sb.Append(c); }
            }
            return sb.ToString();
        }

        private bool hasSpecials(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (Char.IsLetterOrDigit(c))
                {  }
                else
                {
                  sb.Append(c); 
                }
            }
            if (sb.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private void btnRegCancel_Click(object sender, RoutedEventArgs e)
        {
            LogIn loginPage = new LogIn();
            this.Content = loginPage;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ICResponse.Properties.Settings.Default.SelectedThemeDisplayName.Equals("dark"))
            {

                btnSubmitReg.Background = Brushes.Gray;
                btnRegCancel.Background = Brushes.Gray;
            }
            if (ICResponse.Properties.Settings.Default.SelectedThemeDisplayName.Equals("light"))
            {

                btnSubmitReg.Background = Brushes.White;
                btnRegCancel.Background = Brushes.White;
            }
        }
    
    }
}
