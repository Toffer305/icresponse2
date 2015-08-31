using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICResponse.Pages;

using FirstFloor.ModernUI.Windows.Controls;

namespace ICResponse.SupportContent
{
    /// <summary>
    /// Interaction logic for AddCallConfirmation.xaml
    /// </summary>
    public partial class AddCallConfirmation : UserControl
    {

        public AddCallConfirmation()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var newcallwindow = new NewRecordCall();            
            this.Content = newcallwindow;
        }        

    }
}
