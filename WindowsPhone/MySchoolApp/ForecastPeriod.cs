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


using System;
using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;
using System.ComponentModel;

//    Copyright (c) 2010 Microsoft Corporation.  All rights reserved.
//    Use of this sample source code is subject to the terms of the Microsoft license 
//    agreement under which you licensed this sample source code and is provided AS-IS.
//    If you did not accept the terms of the license agreement, you are not authorized 
//    to use this sample source code.  For the terms of the license, please see the 
//    license agreement between you and Microsoft.
//    
//


/// <summary>
/// Class for holding the forecast for a particular time period
/// </summary>
public class ForecastPeriod : INotifyPropertyChanged
{
	#region "member variables"
	private string _timeName;
	private int _temperature;
	private int _chancePrecipitation;
	private string _weatherType;
	private string _textForecast;
	private string _conditionIcon;
	//  Private _itemColor As SolidColorBrush

	public event PropertyChangedEventHandler PropertyChanged;

	#endregion

	public ForecastPeriod()
	{
	}


	public string TimeName {
		get { return _timeName; }
		set {
			if (value != _timeName) {
				this._timeName = value;
				NotifyPropertyChanged("TimeName");
			}
		}
	}


	public int Temperature {
		get { return _temperature; }
		set {
			if (value != _temperature) {
				this._temperature = value;
				NotifyPropertyChanged("Temperature");
			}
		}
	}


	public int ChancePrecipitation {
		get { return _chancePrecipitation; }
		set {
			if (value != _chancePrecipitation) {
				this._chancePrecipitation = value;
				NotifyPropertyChanged("ChancePrecipitation");
			}
		}
	}


	public string WeatherType {
		get { return _weatherType; }
		set {
			if (value != _weatherType) {
				this._weatherType = value;
				NotifyPropertyChanged("WeatherType");
			}
		}
	}


	public string TextForecast {
		get { return _textForecast; }
		set {
			if (value != _textForecast) {
				this._textForecast = value;
				NotifyPropertyChanged("TextForecast");
			}
		}
	}


	public string ConditionIcon {
		get { return _conditionIcon; }
		set {
			if (value != _conditionIcon) {
				this._conditionIcon = value;
				NotifyPropertyChanged("ConditionIcon");
			}
		}
	}

	//Public Property ItemColor() As SolidColorBrush
	//    Get
	//        Return _itemColor
	//    End Get
	//    Set(ByVal value As SolidColorBrush)
	//        If value.Color <> _itemColor.Color Then
	//            Me._itemColor = value
	//            NotifyPropertyChanged("ItemColor")
	//        End If
	//    End Set
	//End Property


	private void NotifyPropertyChanged(string property)
	{
		if (PropertyChanged != null) {
			PropertyChanged(this, new PropertyChangedEventArgs(property));
		}
	}
}