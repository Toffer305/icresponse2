using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICResponse.Pages;
using ICResponse.Content;

namespace ICResponse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {        

        public MainWindow()
        {
            var linkTechGroups = new LinkGroupCollection();
            var linkAdminGroups = new LinkGroupCollection();
            var linkLogOutGroups = new LinkGroupCollection();

            string techdisplayname = ICResponse.Properties.Settings.Default.LoginNick;
            string admindisplayname = ICResponse.Properties.Settings.Default.LoginNick;
            string logoutdisplayname = "Welcome";
            
            InitializeComponent();
            
            
            AppearanceViewModel settings = new AppearanceViewModel();
            settings.SetThemeAndColor(ICResponse.Properties.Settings.Default.SelectedThemeDisplayName,
                  ICResponse.Properties.Settings.Default.SelectedThemeSource,
                  ICResponse.Properties.Settings.Default.SelectedAccentColor,
                  ICResponse.Properties.Settings.Default.SelectedFontSize);

            if (ICResponse.Properties.Settings.Default.LoginLevel.Equals("logout"))
            {
                #region * Logout Group *
                var logoutlinks = new LinkGroup()
                {
                    DisplayName = logoutdisplayname
                };
                logoutlinks.Links.Add(new Link()
                {
                    DisplayName = "Login",
                    Source = new Uri(@"/Pages/Login.xaml", UriKind.RelativeOrAbsolute),
                });
                logoutlinks.Links.Add(new Link()
                {
                    DisplayName = "Register",
                    Source = new Uri(@"/Pages/Register.xaml", UriKind.RelativeOrAbsolute),
                });
                logoutlinks.Links.Add(new Link()
                {
                    DisplayName = "Connection Settings",
                    Source = new Uri(@"/Pages/ConnectionSettings.xaml", UriKind.RelativeOrAbsolute),
                });
                #endregion
                LogOutLink.DisplayName = "";
                linkLogOutGroups.Add(logoutlinks);
                MenuLinkGroups = linkLogOutGroups;

                this.ContentSource = new Uri(@"/Pages/Login.xaml", UriKind.RelativeOrAbsolute);
            }

            if (ICResponse.Properties.Settings.Default.LoginLevel.Equals("user"))
            {
                #region * Technician Group *
                var technicianlinks = new LinkGroup()
                {
                    DisplayName = techdisplayname
                };
                technicianlinks.Links.Add(new Link()
                {
                    DisplayName = "Support",
                    Source = new Uri(@"/Pages/Support.xaml", UriKind.RelativeOrAbsolute),
                });
                #endregion
                LogOutLink.DisplayName = "log out";
                linkTechGroups.Add(technicianlinks);
                MenuLinkGroups = linkTechGroups;

                this.ContentSource = new Uri(@"/Pages/Support.xaml", UriKind.RelativeOrAbsolute);
            }


            if (ICResponse.Properties.Settings.Default.LoginLevel.Equals("admin"))
            {
                #region * Admin Group *
                var adminlinks = new LinkGroup()
                {
                    DisplayName = admindisplayname
                };
                adminlinks.Links.Add(new Link()
                {
                    DisplayName = "Support",
                    Source = new Uri(@"/Pages/Support.xaml", UriKind.RelativeOrAbsolute),
                });
                adminlinks.Links.Add(new Link()
                {
                    DisplayName = "Administration",
                    Source = new Uri(@"/Pages/Administration.xaml", UriKind.RelativeOrAbsolute),
                });
                #endregion
                LogOutLink.DisplayName = "log out";
                linkAdminGroups.Add(adminlinks);
                MenuLinkGroups = linkAdminGroups;

                this.ContentSource = new Uri(@"/Pages/Support.xaml", UriKind.RelativeOrAbsolute);

            }            
            
        }
        
    }
}
