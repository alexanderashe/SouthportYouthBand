using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Windows.Resources;
using System.Net.NetworkInformation;

namespace MySchoolApp
{
    public partial class Browser : PhoneApplicationPage
    {
        public Browser()
        {
            InitializeComponent();
        }



        private void Page_Loaded(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            string pgUrl = "";
            string pgTitle = "";

            MySchoolInfo schoolInfo = default(MySchoolInfo);
            schoolInfo = new MySchoolInfo();
            ApplicationBar.Opacity = 0.5;


            ApplicationTitle.Text = schoolInfo.SchoolName;


            if ((!NetworkInterface.GetIsNetworkAvailable()))
            {
                //' game's over, an internet connection is required
                NavigationService.Navigate(new Uri("/PanoramaPage1.xaml", UriKind.Relative));
            }


            try
            {
                //Parses a query string

                NavigationContext.QueryString.TryGetValue("url", out pgUrl);
                NavigationContext.QueryString.TryGetValue("title", out pgTitle);

                PageTitle.Text = pgTitle;


                WebBrowser1.Navigate(new Uri(pgUrl, UriKind.Absolute));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region "App Bar Navigation"

        private void Home_Click(System.Object sender, System.EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PanoramaPage1.xaml", UriKind.Relative));



        }

        private void Back_Click(System.Object sender, System.EventArgs e)
        {
            NavigationService.GoBack();

        }
        #endregion
    }
}