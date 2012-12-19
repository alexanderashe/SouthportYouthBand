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


using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class LinkAndTitle
{

    //Content's title


    //Content's physical address
    public string Link { get; set; }
    public string Title { get; set; }

    public SolidColorBrush ItemColor { get; set; }
    //' used for display only 

    public LinkAndTitle(string strLink, string strTitle)
    {
        Link = strLink;
        Title = strTitle;


    }
    public LinkAndTitle()
    {
        Link = "";
        Title = "";
    }


}