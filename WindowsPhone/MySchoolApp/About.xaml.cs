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




namespace MySchoolApp
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            //MySchoolInfo schoolinfo;

            InitializeComponent();
        }

        private void Page_Load(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            MySchoolInfo schoolInfo = default(MySchoolInfo);
            schoolInfo = new MySchoolInfo();
            ApplicationBar.Opacity = 0.5;
            ApplicationTitle.Text = schoolInfo.SchoolName;

        }


        //' display a local HTML file in a browser control.
        //' code adapted from http://phone7.wordpress.com/2010/08/08/loading-a-local-html-file-in-the-webbrowser-control/#more-1287


        private void Browser_Loaded(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            StreamResourceInfo rs = default(StreamResourceInfo);
            StreamReader reader = null;
            string html = null;


            try
            {
                rs = Application.GetResourceStream(new Uri("about.html", UriKind.Relative));
                reader = new StreamReader(rs.Stream);
                html = reader.ReadToEnd();
            }
            catch
            {
                html = "<html><head></head><body>Error with File</body></html>";
            }

            WebBrowser1.NavigateToString(html);


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