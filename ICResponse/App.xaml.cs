using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using FirstFloor.ModernUI.Windows.Controls;

namespace ICResponse
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        CookieContainer cookies = new CookieContainer();
        const string acd_logout_url = "https://acd2.ststelecom.com/QueueManager/Logout.php";

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Perform tasks at application exit
            ICResponse.Properties.Settings.Default.LoginLevel = "logout";
            ICResponse.Properties.Settings.Default.Save();
        }
    
    }
}
