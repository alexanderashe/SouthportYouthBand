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
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Windows.Resources;
using System.Net.NetworkInformation;

namespace MySchoolApp
{

    public partial class RSSFeed : PhoneApplicationPage
    {
        public MySchoolInfo schoolInfo;
        public RSSFeed()
        {
            InitializeComponent();
            schoolInfo = new MySchoolInfo();
            ApplicationBar.Opacity = 0.5;

            Rectangle verticalFillRectangle = new Rectangle();

            verticalFillRectangle.Width = 200;
            verticalFillRectangle.Height = 100;

            //'Create a vertical linear gradient  

            LinearGradientBrush myVerticalGradient = new LinearGradientBrush();
            myVerticalGradient.StartPoint = new Point(0.5, 0);
            myVerticalGradient.EndPoint = new Point(0.5, 1);

            GradientStop stop1 = new GradientStop();
            stop1.Color = schoolInfo.Color1.Color;
            stop1.Offset = 0.33;

            GradientStop stop2 = new GradientStop();
            stop2.Color = schoolInfo.Color2.Color;
            stop2.Offset = 0.66;

            myVerticalGradient.GradientStops.Add(stop1);
            myVerticalGradient.GradientStops.Add(stop2);

            listboxRSSFeedItems.Background = myVerticalGradient;



        }

        private void ReadRss(Uri rssUri)
	{
		//The WebClient class allows async downloading
		WebClient wclient = new WebClient();


		//The statement lambda is a replacement for AddressOf
		wclient.OpenReadCompleted += (object sender, OpenReadCompletedEventArgs e) =>
		{
			if (e.Error != null) {
				MessageBox.Show(e.Error.Message);
				return;
			}

			System.IO.Stream str = e.Result;
			XDocument xdoc = XDocument.Load(str);

			// take the top ten results
			var rssFeedItems = (from Item in xdoc.Descendants("item")
          
                select new RSSFeedItem {
                    Link = new Uri(Item.Element("link").Value, UriKind.Absolute),
                    pubDate = Convert.ToDateTime(Item.Element("pubDate").Value).ToLocalTime().ToString(),
                    Title = Item.Element("title").Value,
                    Description = (Item.Element("description") != null ? Item.Element("description").Value: "...")
			}).ToList().Take(10);

			// close
			str.Close();
			// add results to listbox
			listboxRSSFeedItems.Items.Clear();
			foreach (RSSFeedItem item in rssFeedItems) {
				listboxRSSFeedItems.Items.Add(item);
			}

		};
		wclient.OpenReadAsync(rssUri);
	}



        private void MoreButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            Button senderButton = (Button)sender;
            string pageUrl = null;
            pageUrl = senderButton.Tag.ToString();
            NavigationService.Navigate(new Uri("/Browser.xaml?url=" + pageUrl + "&title=News", UriKind.Relative));
        }

        private void Page_Loaded(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            string pgUrl = "";
            string pgTitle = "";

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

                pgUrl = pgUrl.Replace("!", "&");
                // handle the case where the URL part has more than one query string
                ReadRss(new Uri(pgUrl, UriKind.Absolute));

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
    public class RSSFeedItem
    {

        //Content's title
        public string Title { get; set; }
        //Publish date
        public string pubDate { get; set; }
        //Content's physical address
        public Uri Link { get; set; }
        public string Description { get; set; }
    }
}