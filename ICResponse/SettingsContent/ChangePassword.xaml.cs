using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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

namespace ICResponse.SettingsContent
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class ChangePassword : UserControl
    {
        StringBuilder myStringBuilder = new StringBuilder();
        DateTime todaydate = new DateTime();
        public ChangePassword()
        {
            InitializeComponent();
        }

        private void submitPassword_Click(object sender, RoutedEventArgs e)
        {
            attemptPasswordChange();
        }

        private void forgotPassword_Click(object sender, RoutedEventArgs e)
        {
            enterlabeL.Visibility = System.Windows.Visibility.Visible;
            emailtextboX.Visibility = System.Windows.Visibility.Visible;
            emaillabeL.Visibility = System.Windows.Visibility.Visible;
            cancelbuttoN.Visibility = System.Windows.Visibility.Visible;
            sendbuttoN.Visibility = System.Windows.Visibility.Visible;

            old_password_box.IsEnabled = false;
            new_password_box.IsEnabled = false;
            confirm_new_box.IsEnabled = false;
            old_password_label.IsEnabled = false;
            new_password_label.IsEnabled = false;
            confirm_new_label.IsEnabled = false;
            savepwbuttoN.IsEnabled = false;
        }

        private void cancelbuttoN_Click(object sender, RoutedEventArgs e)
        {
            enterlabeL.Visibility = System.Windows.Visibility.Hidden;
            emailtextboX.Visibility = System.Windows.Visibility.Hidden;
            emaillabeL.Visibility = System.Windows.Visibility.Hidden;
            cancelbuttoN.Visibility = System.Windows.Visibility.Hidden;
            sendbuttoN.Visibility = System.Windows.Visibility.Hidden;

            old_password_box.IsEnabled = true;
            new_password_box.IsEnabled = true;
            confirm_new_box.IsEnabled = true;
            old_password_label.IsEnabled = true;
            new_password_label.IsEnabled = true;
            confirm_new_label.IsEnabled = true;
            savepwbuttoN.IsEnabled = true;
        }

        private void sendbuttoN_Click(object sender, RoutedEventArgs e)
        {
            myStringBuilder.Clear();
            todaydate = System.DateTime.Today;
            int smtpport = ICResponse.Properties.Settings.Default.smtpport;
            string receipient = this.emailtextboX.Text + "@icrealtime.com";
            string smtphost = ICResponse.Properties.Settings.Default.smtphost;
            string smtpusername = ICResponse.Properties.Settings.Default.smtpusername;
            string smtppassword = ICResponse.Properties.Settings.Default.smtppassword;
            string smtpsubjectline = ICResponse.Properties.Settings.Default.smtpsubjectline;

            string userpassword = retrievePassword(receipient);

            myStringBuilder.AppendLine(this.emailtextboX.Text + ",");
            myStringBuilder.AppendLine(" ");
            myStringBuilder.AppendLine("Your current password is: " + userpassword);

            string smtpmessagebody = myStringBuilder.ToString();
            // SEND EMAIL
            MailMessage mail = new MailMessage(smtpusername, receipient, smtpsubjectline, smtpmessagebody);
            SmtpClient client = new SmtpClient(smtphost);
            client.Port = smtpport;
            client.Credentials = new System.Net.NetworkCredential(smtpusername, smtppassword);
            client.EnableSsl = true;
            client.Send(mail);

            enterlabeL.Visibility = System.Windows.Visibility.Hidden;
            emailtextboX.Visibility = System.Windows.Visibility.Hidden;
            emaillabeL.Visibility = System.Windows.Visibility.Hidden;
            cancelbuttoN.Visibility = System.Windows.Visibility.Hidden;
            sendbuttoN.Visibility = System.Windows.Visibility.Hidden;

            old_password_box.IsEnabled = true;
            new_password_box.IsEnabled = true;
            confirm_new_box.IsEnabled = true;
            old_password_label.IsEnabled = true;
            new_password_label.IsEnabled = true;
            confirm_new_label.IsEnabled = true;
            savepwbuttoN.IsEnabled = true;
        }

        private void attemptPasswordChange()
        {
            try
            {
                if (ICResponse.Properties.Settings.Default.LoginLevel.Equals("logout"))
                {
                    showEmptyDialog();
                }
                else
                {
                    if (hasSpecials(this.new_password_box.Password) || hasSpecials(this.confirm_new_box.Password))
                    {
                        showSpecialsDialog();
                    }
                    else
                    {
                        string oldPassword = this.old_password_box.Password;
                        string enteredPassword = removeSpecials(this.new_password_box.Password);
                        string enteredConfirmPassword = removeSpecials(this.confirm_new_box.Password);

                        if (CheckOldPassword(oldPassword))
                        {
                            if (enteredPassword == enteredConfirmPassword)
                            {
                                PasswordChange(enteredPassword);
                                showSuccessDialog();
                                ICResponse.Properties.Settings.Default.LoginLevel = "logout";
                                ICResponse.Properties.Settings.Default.Save();

                                var mainWindow = new MainWindow();
                                var thisWindow = Window.GetWindow(this);
                                mainWindow.Show();
                                if (thisWindow != null) thisWindow.Close();
                            }
                            else
                            {
                                showMisMatchDialog();
                            }
                        }
                        else
                        {
                            showOldPWDialog();
                        }                       
                    }
                }
                
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err);
            }
        }

        private async void PasswordChange(string newPW)
        {
            string agentid = Convert.ToString(ICResponse.Properties.Settings.Default.AgentID);
            string prequerystring = "UPDATE agents SET password='" +newPW + "' WHERE agentID='" + agentid + "'";

            using (MySqlConnection dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer +
                                                        ";Database=" + ICResponse.Properties.Settings.Default.DBName +
                                                        ";Uid=" + ICResponse.Properties.Settings.Default.DBUser +
                                                        ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";"))
            {
                dbConn.Open();
                using (MySqlCommand cmd = new MySqlCommand(prequerystring, dbConn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                dbConn.Close();
            }

        }

        private bool CheckOldPassword(string oldpw)
        {
            bool iscorrectpw = false;
            if (oldpw.Equals(ICResponse.Properties.Settings.Default.LoginPassword))
            {
                iscorrectpw = true;
            }
            return iscorrectpw;
        }

        private string retrievePassword(string enteredEmail)
        {
            string preparedQuery = ICResponse.Properties.Settings.Default.AgentSelectByEmail + "'" + enteredEmail + "'";
            string myPassword = string.Empty;
            // DB Connection
            using (MySqlConnection dbConn = new MySqlConnection("Server=" + ICResponse.Properties.Settings.Default.DBServer
                                                        + ";Database=" + ICResponse.Properties.Settings.Default.DBName
                                                        + ";Uid=" + ICResponse.Properties.Settings.Default.DBUser
                                                        + ";Pwd=" + ICResponse.Properties.Settings.Default.DBPass + ";"))
            {
                dbConn.Open();
                using (MySqlCommand cmd = new MySqlCommand(preparedQuery, dbConn))
                {                    
                    // DB Reader Execution
                    MySqlDataReader dbReader = cmd.ExecuteReader();
                    // Loop through results
                    while (dbReader.Read())
                    {
                        myPassword = Convert.ToString(dbReader["password"]);
                    }
                    dbReader.Close();
                }
                dbConn.Close();
            }
            return myPassword;
        }

        private void showMisMatchDialog()
        {
            ModernDialog msgbox = new ModernDialog();
            msgbox.Title = "Error";
            msgbox.Content = "Passwords do not match.";
            msgbox.Buttons = new[] { msgbox.OkButton };

            msgbox.ShowDialog();
        }

        private void showOldPWDialog()
        {
            ModernDialog msgbox = new ModernDialog();
            msgbox.Title = "Incorrect";
            msgbox.Content = "Old Password not correct.";
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
            msgbox.Content = "You password has changed. Please login again.";
            msgbox.Buttons = new[] { msgbox.OkButton };
            msgbox.ShowDialog();
        }
        private void showEmptyDialog()
        {
            ModernDialog msgbox = new ModernDialog();
            msgbox.Title = "Login";
            msgbox.Content = "Please login to ICResponse first.";
            msgbox.Buttons = new[] { msgbox.OkButton };
            msgbox.ShowDialog();
        }

        private string removeSpecials(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (Char.IsLetterOrDigit(c))
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
                { }
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


    }
}
