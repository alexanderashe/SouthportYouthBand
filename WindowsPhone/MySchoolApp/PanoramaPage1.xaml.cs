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

using MySchoolApp.GeocodeService;

using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using System.ServiceModel;

namespace MySchoolApp
{



    public partial class PanoramaPage1 : PhoneApplicationPage
    {

        public MySchoolInfo schoolInfo;
        public PanoramaPage1()
        {
            InitializeComponent();
            schoolInfo = new MySchoolInfo();

            Panorama.Title = schoolInfo.SchoolName;


          

        }


        private void Page_Loaded(System.Object sender, System.Windows.RoutedEventArgs e)
        {
            if ((NetworkInterface.GetIsNetworkAvailable()))
            {
                //' we have connectivity

                panOnCampus.Visibility = System.Windows.Visibility.Visible;
                panHeader.Visibility = System.Windows.Visibility.Visible;
             
                SetUp_OnCampus();
                Setup_Links();
                



            }
            else
            {
                panOnCampus.Visibility = System.Windows.Visibility.Collapsed;
                panHeader.Visibility = System.Windows.Visibility.Collapsed;
            

                txtBlock_ConnectivityError.Visibility = System.Windows.Visibility.Visible;
                txtBlock_ConnectivityError.Text = "A network connection is required to run" + Environment.NewLine + "this application. Please try again" + Environment.NewLine + "when you have connectivity.";

            }


        }


        #region "onCampus Setup"




        private void SetUp_OnCampus()
	{
		//<Contacts>
		//<ContactInfo>
		//  <ContactName>Bentley Library</ContactName>
		//  <ContactEmail>library@bentley.edu</ContactEmail> 
		//  <ContactPhoto>http://undergraduate.bentley.edu/sites/undergraduate.bentley.edu/files/images/library-side2.jpg</ContactPhoto> 
		//  <ContactPhone>781.891.2168</ContactPhone>
		//  <ContactDescription>The Library Services Desk is located on the main floor of the Library.</ContactDescription>
		//  <ContactLink>http://library.bentley.edu</ContactLink>
		//</ContactInfo>
		// ...
		// </Contacts>

        StreamResourceInfo rs = default(StreamResourceInfo);
        rs = Application.GetResourceStream(new Uri("/MySchoolApp;component/Contacts.xml", UriKind.Relative));

        XDocument xdoc = XDocument.Load(rs.Stream);
        List<ContactInfo> contactList = new List<ContactInfo>();


        if (xdoc != null)
        {
            contactList = (from item in xdoc.Element("Contacts").Descendants("ContactInfo")
                           select new ContactInfo
                           {
                               ContactName = item.Element("ContactName").Value,
                               ContactEmail = item.Element("ContactEmail").Value,
                               ContactPhoto = new Uri(item.Element("ContactPhoto").Value, UriKind.Absolute),
                               ContactPhone = item.Element("ContactPhone").Value,
                               ContactDescription = item.Element("ContactDescription").Value,
                               ContactLink = item.Element("ContactLink").Value
                           }).ToList();


            ShowContacts(contactList);
        }
	}

        private void ShowContacts(List<ContactInfo> contactList)
        {
            // add results to listbox
            MySchoolInfo schoolinfo = new MySchoolInfo();

            listBoxContactList.Items.Clear();
            bool alternate = true;

            foreach (ContactInfo item in contactList)
            {
                if (alternate)
                {
                    item.ItemColor = schoolinfo.Color1;
                }
                else
                {
                    item.ItemColor = schoolinfo.Color2;
                }
                alternate = !alternate;
                listBoxContactList.Items.Add(item);
            }
        }
        private void LinkButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {

            Button senderButton = (Button)sender;
            string linkUrl = null;
            string title = null;
            linkUrl = senderButton.Tag.ToString();
            title = senderButton.Content.ToString();


            // if the URL contains rss, xml, or feed, assume it's an RSS Feed

            //' assume it's an RSS feed 
            if (linkUrl.ToLower().Contains("rss") | linkUrl.ToLower().Contains("xml") | linkUrl.ToLower().Contains("feed"))
            {
                NavigationService.Navigate(new Uri("/RSSFeed.xaml?url=" + linkUrl + "&title=" + title, UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/Browser.xaml?url=" + linkUrl + "&title=" + title, UriKind.Relative));
            }

        }
        #endregion



        

        

        #region "Links Setup"


        private void Setup_Links()
	{
		string newsfeedURL = null;

		//the bing news feed for your school

		newsfeedURL = "http://api.bing.com/rss.aspx" + "?Source=News&Market=en-US&Version=2.0&Query=%22" + schoolInfo.SchoolName.Replace(" ", "+") + "%22";
		newsfeedURL = newsfeedURL.Replace("&", "!");
		//turns & into ! so that RSSFeed.xaml can detect the title querystring below

		//<Links>
		//  <LinkAndTitle>
		//    <Link>http://www.bentley.edu</Link>
		//    <Title>Bentley Web Site</Title>
		//  </LinkAndTitle>
		//  <LinkAndTitle>
		//    <Link>http://www.facebook.com/bentleyuniversity</Link>
		//    <Title>Bentley on Facebook</Title>
		//  </LinkAndTitle>
		// ...
		//</Links>

		StreamResourceInfo rs = default(StreamResourceInfo);
		rs = Application.GetResourceStream(new Uri("/MySchoolApp;component/Links.xml", UriKind.Relative));

		XDocument xdoc = XDocument.Load(rs.Stream);
		List<LinkAndTitle> linkList = new List<LinkAndTitle>();

        if (xdoc != null)
        {

            //linkList = (from item in xdoc.Elements().Elements("LinkAndTitle")
            linkList = (from item in xdoc.Element("Links").Descendants("LinkAndTitle")
                        select new LinkAndTitle
                        {
                            Link = item.Element("Link").Value,
                            Title = item.Element("Title").Value
                        }).ToList();

            ShowLinks(linkList);
        }
	}
        private void ShowLinks(List<LinkAndTitle> linkList)
        {
            // add results to listbox


            listBoxLinksList.Items.Clear();
            bool alternate = true;

            foreach (LinkAndTitle item in linkList)
            {
                if (alternate)
                {
                    item.ItemColor = schoolInfo.Color1;
                }
                else
                {
                    item.ItemColor = schoolInfo.Color2;
                }
                alternate = !alternate;
                listBoxLinksList.Items.Add(item);
            }
        }


        private void Links_LinkButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
        {

            Button senderButton = (Button)sender;
            string linkUrl = null;
            string title = null;
            linkUrl = senderButton.Tag.ToString();
            title = senderButton.Content.ToString();


            // if the URL contains rss, xml, or feed, assume it's an RSS Feed

            //' assume it's an RSS feed 
            if (linkUrl.ToLower().Contains("rss") | linkUrl.ToLower().Contains("xml") | linkUrl.ToLower().Contains("feed"))
            {
                NavigationService.Navigate(new Uri("/RSSFeed.xaml?url=" + linkUrl + "&title=" + title, UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/Browser.xaml?url=" + linkUrl + "&title=" + title, UriKind.Relative));
            }


        }
        #endregion
        

        

    }
}