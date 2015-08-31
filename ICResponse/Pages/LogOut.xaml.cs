using System;
using System.Collections.Generic;
using FirstFloor.ModernUI.Presentation;
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

namespace ICResponse.Pages
{
    /// <summary>
    /// Interaction logic for LogOut.xaml
    /// </summary>
    public partial class LogOut : UserControl
    {

        
        public LogOut()
        {
            InitializeComponent();
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            //Reset Agent Information
            ICResponse.Properties.Settings.Default.LoginUsername = "";
            ICResponse.Properties.Settings.Default.LoginPassword = "";
            ICResponse.Properties.Settings.Default.LoginNick = "";
            ICResponse.Properties.Settings.Default.LoginLevel = "logout";
            ICResponse.Properties.Settings.Default.AgentID = 0;
            ICResponse.Properties.Settings.Default.Save();

            // Garbage Cleanup
            GC.Collect();

            //Change display content
            var mainWindow = new MainWindow();
            var logoutWindow = Window.GetWindow(this);
            mainWindow.Show();
            if (logoutWindow != null) logoutWindow.Close();                      
        }

        private void btnCancelLogOut_Click(object sender, RoutedEventArgs e)
        {
            //NavigationCommands.BrowseBack.Execute();
        }
        
    }
}
