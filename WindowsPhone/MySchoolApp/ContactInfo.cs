using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MySchoolApp
{
    public class ContactInfo
    {

        //Content's title
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public Uri ContactPhoto { get; set; }
        public string ContactPhone { get; set; }
        public string ContactDescription { get; set; }
        public string ContactLink { get; set; }

        public SolidColorBrush ItemColor { get; set; }


        public ContactInfo(string strContactName, string strContactEmail, string strContactPhoto, string strContactPhone, string strContactDescription, string strContactLink)
        {
            ContactName = strContactName;
            ContactEmail = strContactEmail;
            ContactPhoto = new Uri(strContactPhoto, UriKind.Absolute);
            ContactDescription = strContactDescription;
            ContactPhone = strContactPhone;
            ContactLink = strContactLink;

        }

        public ContactInfo()
        {
            ContactName = "";
            ContactEmail = "";
            ContactPhoto = null;
            ContactDescription = "";
            ContactPhone = "";
            ContactLink = "";
        }




    }

}
