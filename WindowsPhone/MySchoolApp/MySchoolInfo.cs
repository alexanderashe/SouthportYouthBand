
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

//using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Windows.Resources;

public class MySchoolInfo
{

    private string strSchoolName;
    private string strSchoolUrl;
    private string strAddress;
    private string strCity;
    private string strState;
    private string strZip;
    private SolidColorBrush clrColor1;
    private SolidColorBrush clrColor2;

    private string strTeamName;


    public string SchoolName
    {
        get { return strSchoolName; }
        set { strSchoolName = value; }
    }
    public string SchoolURL
    {
        get { return strSchoolUrl; }
        set { strSchoolUrl = value; }
    }
    public string Address
    {
        get { return strAddress; }
        set { strAddress = value; }
    }
    public string City
    {
        get { return strCity; }
        set { strCity = value; }
    }
    public string State
    {
        get { return strState; }
        set { strState = value; }
    }
    public string Zip
    {
        get { return strZip; }
        set { strZip = value; }
    }
    public SolidColorBrush Color1
    {
        get { return clrColor1; }
        set { clrColor1 = value; }
    }
    public SolidColorBrush Color2
    {
        get { return clrColor2; }
        set { clrColor2 = value; }
    }
    public string TeamName
    {
        get { return strTeamName; }
        set { strTeamName = value; }
    }



    public MySchoolInfo()
    {
        // Sample XML

        // <MySchoolInfo>
        //    <SchoolName>Bentley University</SchoolName>
        //    <SchoolUrl>http://www.bentley.edu</SchoolUrl>
        //    <SchoolAddress>175 Forest Street</SchoolAddress>
        //    <City>Waltham</City>
        //    <State>MA</State>
        //    <Zip>02452</Zip>
        //    <SchoolColor1>336699</SchoolColor1>
        //    <SchoolColor2>FFCC33</SchoolColor2>
        //    <TeamName>Bentley Falcons</TeamName>
        // </MySchoolInfo>





        StreamResourceInfo rs = default(StreamResourceInfo);

        rs = Application.GetResourceStream(new Uri("/MySchoolApp;component/MySchoolInfo.xml", UriKind.Relative));
        
        XDocument xdoc =  XDocument.Load(rs.Stream);

        IEnumerable<XElement> SchoolInfo = xdoc.Elements();

        // there should be only one record in schoolinfo collection

        foreach (var si in SchoolInfo)
        {

            clrColor1 = GetColorFromRGB(si.Element("SchoolColor1").Value);

            clrColor2 = GetColorFromRGB(si.Element("SchoolColor2").Value);

            strSchoolName = si.Element("SchoolName").Value;
            strSchoolUrl = si.Element("SchoolUrl").Value;
            strAddress = si.Element("SchoolAddress").Value;
            strCity = si.Element("City").Value;
            strState = si.Element("State").Value;
            strZip = si.Element("Zip").Value;
            strTeamName = si.Element("TeamName").Value;

            Color1 = clrColor1;
            Color2 = clrColor2;
            SchoolName = strSchoolName;
            Address = strAddress;
            City = strCity;
            State = strState;
            Zip = strZip;
            SchoolURL = strSchoolUrl;
            TeamName = strTeamName;

        }


    }

    protected SolidColorBrush GetColorFromRGB(string hexColor)
    {
        //' assumes hexColor is rrggbb
        //' if you see bright red, it means your color wasn't valid (or your school's color is FF0000 !)\

        //' adapted from http://weblogs.asp.net/lduveau/archive/2009/04/22/silverlight-get-color-from-hex.aspx

        try
        {

            string argb = "FF" + hexColor;
            //' alpha component is opacity, set to 255 
            //                  
            return new SolidColorBrush(Color.FromArgb(System.Convert.ToByte(argb.Substring(0, 2), 16), System.Convert.ToByte(argb.Substring(2, 2), 16), System.Convert.ToByte(argb.Substring(4, 2), 16), System.Convert.ToByte(argb.Substring(6, 2), 16)));


        }
        catch (Exception ex)
        {
            // '' Colors must be valid Hex colors or you'll be seeing red.

            return new SolidColorBrush(Color.FromArgb(Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0)));


        }

    }


}