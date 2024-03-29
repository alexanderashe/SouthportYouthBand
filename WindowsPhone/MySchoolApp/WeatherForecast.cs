﻿// 
//    Copyright (c) 2010 Microsoft Corporation.  All rights reserved.
//    Use of this sample source code is subject to the terms of the Microsoft license 
//    agreement under which you licensed this sample source code and is provided AS-IS.
//    If you did not accept the terms of the license agreement, you are not authorized 
//    to use this sample source code.  For the terms of the license, please see the 
//    license agreement between you and Microsoft.
//    
//

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


using System.Linq;
using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;


using System.Xml.Linq;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;


/// <summary>
/// Class for holding the forecast information
/// </summary>
public class WeatherForecast : INotifyPropertyChanged
{

	#region "member variables"

	// name of city forecast is for
	private string _cityName;
	// elevation of city
	private int _height;
	// source of information

	private string _credit;
	public event PropertyChangedEventHandler PropertyChanged;

	#endregion


	#region "Accessors"

	// collection of forecasts for each time period
	public ObservableCollection<ForecastPeriod> ForecastList { get; set; }


	public string CityName {
		get { return _cityName; }
		set {
			if (value != _cityName) {
				_cityName = value;
				NotifyPropertyChanged("CityName");
			}
		}
	}


	public int Height {
		get { return _height; }
		set {
			if (value != _height) {
				_height = value;
				NotifyPropertyChanged("Height");
			}
		}
	}


	public string Credit {
		get { return _credit; }
		set {
			if (value != _credit) {
				_credit = value;
				NotifyPropertyChanged("Credit");
			}
		}
	}

	#endregion


	#region "constructors"

	public WeatherForecast()
	{
		ForecastList = new ObservableCollection<ForecastPeriod>();

	}

	#endregion


	#region "private Helpers"

	/// <summary>
	/// Raise the PropertyChanged event and pass along the property that changed
	/// </summary>
	private void NotifyPropertyChanged(string property)
	{
		if (PropertyChanged != null) {
			PropertyChanged(this, new PropertyChangedEventArgs(property));
		}
	}

	#endregion

	/// <summary>
	/// Get a forecast for the given latitude and longitude
	/// </summary>
	public void GetForecast(string latitude, string longitude)
	{
		// form the URI

		UriBuilder fullUri = new UriBuilder("http://forecast.weather.gov/MapClick.php");
		fullUri.Query = "lat=" + latitude + "&lon=" + longitude + "&FcstType=dwml";

		// initialize a new WebRequest
		HttpWebRequest forecastRequest = (HttpWebRequest)WebRequest.Create(fullUri.Uri);

		// set up the state object for the async request
		ForecastUpdateState forecastState = new ForecastUpdateState();
		forecastState.AsyncRequest = forecastRequest;

		// start the asynchronous request
		forecastRequest.BeginGetResponse(new AsyncCallback(HandlerForecastResponse), forecastState);
	}


	/// <summary>
	/// Handle the information returned from the async request
	/// </summary>
	/// <param name="asyncResult"></param>
	private void HandlerForecastResponse(IAsyncResult asyncResult)
	{
		// get the state information
		ForecastUpdateState forecastState = (ForecastUpdateState)asyncResult.AsyncState;
		HttpWebRequest forecastRequest = (HttpWebRequest)forecastState.AsyncRequest;

		// end the async request
		forecastState.AsyncResponse = (HttpWebResponse)forecastRequest.EndGetResponse(asyncResult);

		Stream streamResult = null;

		string newCredit = "";
		string newCityName = "";
		int newHeight = 0;

		// create a temp collection for the new forecast information for each 
		// time period
		ObservableCollection<ForecastPeriod> newForecastList = new ObservableCollection<ForecastPeriod>();

		try {
			// get the stream containing the response from the async call
			streamResult = forecastState.AsyncResponse.GetResponseStream();

			// load the XML
			XElement xmlWeather = default(XElement);
			xmlWeather = XElement.Load(streamResult);

			// start parsing the XML.  You can see what the XML looks like by 
			// browsing to: 
			// http://forecast.weather.gov/MapClick.php?lat=44.52160&lon=-87.98980&FcstType=dwml

			// find the source element and retrieve the credit information
			XElement xmlCurrent = default(XElement);
			xmlCurrent = xmlWeather.Descendants("source").First();

			newCredit = Convert.ToString(xmlCurrent.Element("credit").Value);

			// find the source element and retrieve the city and elevation information
			xmlCurrent = xmlWeather.Descendants("location").First();
			newCityName = Convert.ToString(xmlCurrent.Element("city").Value );
			newHeight = Convert.ToInt32(xmlCurrent.Element("height").Value);

			// find the first time layout element
			xmlCurrent = xmlWeather.Descendants("time-layout").First();

			int timeIndex = 1;

			// search through the time layout elements until you find a node 
			// contains at least 12 time periods of information. Other nodes can be ignored
			while (xmlCurrent.Elements("start-valid-time").Count() < 12) {
				xmlCurrent = xmlWeather.Descendants("time-layout").ElementAt(timeIndex);
				timeIndex += 1;
			}

			ForecastPeriod newPeriod = default(ForecastPeriod);

			// For each time period element, create a new forecast object and store
			// the time period name.
			// Time periods vary depending on the time of day the data is fetched.  
			// You may get "Today", "Tonight", "Monday", "Monday Night", etc
			// or you may get "Tonight", "Monday", "Monday Night", etc
			// or you may get "This Afternoon", "Tonight", "Monday", "Monday Night", etc
			foreach (XElement curElement in xmlCurrent.Elements("start-valid-time")) {
				try {
					newPeriod = new ForecastPeriod();
					newPeriod.TimeName = Convert.ToString(curElement.Attribute("period-name").Value);

					newForecastList.Add(newPeriod);

				} catch (FormatException e1) {
				}
			}

			// now read in the weather data for each time period
			GetMinMaxTemperatures(xmlWeather, newForecastList);
			GetChancePrecipitation(xmlWeather, newForecastList);
			GetCurrentConditions(xmlWeather, newForecastList);
			GetWeatherIcon(xmlWeather, newForecastList);
			GetTextForecast(xmlWeather, newForecastList);



			// copy the data over
			// copy forecast object over
			// copy the list of forecast time periods over
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				Credit = newCredit;
				Height = newHeight;
				CityName = newCityName;
				ForecastList.Clear();
				foreach (ForecastPeriod forecastPeriod in newForecastList) {
					ForecastList.Add(forecastPeriod);
				}
			});


		} catch (FormatException e2) {
			// there was some kind of error processing the response from the web
			// additional error handling would normally be added here
			return;
		}
	}


	/// <summary>
	/// Get the minimum and maximum temperatures for all the time periods
	/// </summary>
	private void GetMinMaxTemperatures(XElement xmlWeather, ObservableCollection<ForecastPeriod> newForecastList)
	{
		XElement xmlCurrent = default(XElement);

		// Find the temperature parameters.   if first time period is "Tonight",
		// then the Daily Minimum Temperatures are listed first.
		// Otherwise the Daily Maximum Temperatures are listed first
		xmlCurrent = xmlWeather.Descendants("parameters").First();

		int minTemperatureIndex = 1;
		int maxTemperatureIndex = 0;

		// If "Tonight" is the first time period, then store Daily Minimum 
		// Temperatures first, then Daily Maximum Temperatuers next
		if (newForecastList.ElementAt(0).TimeName == "Tonight") {
			minTemperatureIndex = 0;
			maxTemperatureIndex = 1;

			// get the Daily Minimum Temperatures
			foreach (XElement curElement in xmlCurrent.Elements("temperature").ElementAt(0).Elements("value")) {
				newForecastList.ElementAt(minTemperatureIndex).Temperature = int.Parse(curElement.Value);

				minTemperatureIndex += 2;
			}

			// then get the Daily Maximum Temperatures
			foreach (XElement curElement in xmlCurrent.Elements("temperature").ElementAt(1).Elements("value")) {
				newForecastList.ElementAt(maxTemperatureIndex).Temperature = int.Parse(curElement.Value);

				maxTemperatureIndex += 2;
			}

		// otherwise we have a daytime time period first
		} else {
			// get the Daily Maximum Temperatures
			foreach (XElement curElement in xmlCurrent.Elements("temperature").ElementAt(0).Elements("value")) {
				newForecastList.ElementAt(maxTemperatureIndex).Temperature = int.Parse(curElement.Value);

				maxTemperatureIndex += 2;
			}

			// then get the Daily Minimum Temperatures
			foreach (XElement curElement in xmlCurrent.Elements("temperature").ElementAt(1).Elements("value")) {
				newForecastList.ElementAt(minTemperatureIndex).Temperature = int.Parse(curElement.Value);

				minTemperatureIndex += 2;
			}
		}
	}


	/// <summary>
	/// Get the chance of precipitation for all the time periods
	/// </summary>
	private void GetChancePrecipitation(XElement xmlWeather, ObservableCollection<ForecastPeriod> newForecastList)
	{
		XElement xmlCurrent = default(XElement);

		// now find the probablity of precipitation for each time period
		xmlCurrent = xmlWeather.Descendants("probability-of-precipitation").First();

		int elementIndex = 0;

		foreach (XElement curElement in xmlCurrent.Elements("value")) {
			try {
				newForecastList.ElementAt(elementIndex).ChancePrecipitation = int.Parse(curElement.Value);
			// some values are nil
			} catch (FormatException e1) {
				newForecastList.ElementAt(elementIndex).ChancePrecipitation = 0;
			}

			elementIndex += 1;
		}
	}


	/// <summary>
	/// Get the current conditions for all the time periods
	/// </summary>
	private void GetCurrentConditions(XElement xmlWeather, ObservableCollection<ForecastPeriod> newForecastList)
	{
		XElement xmlCurrent = default(XElement);
		int elementIndex = 0;

		// now get the current weather conditions for each time period
		xmlCurrent = xmlWeather.Descendants("weather").First();

		foreach (XElement curElement in xmlCurrent.Elements("weather-conditions")) {
			try {
				newForecastList.ElementAt(elementIndex).WeatherType = Convert.ToString(curElement.Attribute("weather-summary").Value);
			} catch (FormatException e1) {
				newForecastList.ElementAt(elementIndex).WeatherType = "";
			}

			elementIndex += 1;
		}
	}


	/// <summary>
	/// Get the link to the weather icon for all the time periods
	/// </summary>
	/// <param name="xmlWeather"></param>
	/// <param name="newForecastList"></param>
	private void GetWeatherIcon(XElement xmlWeather, ObservableCollection<ForecastPeriod> newForecastList)
	{
		XElement xmlCurrent = default(XElement);
		int elementIndex = 0;

		// get a link to the weather icon for each time period
		xmlCurrent = xmlWeather.Descendants("conditions-icon").First();

		foreach (XElement curElement in xmlCurrent.Elements("icon-link")) {
			try {
				newForecastList.ElementAt(elementIndex).ConditionIcon = Convert.ToString(curElement.Value);
			} catch (FormatException e1) {
				newForecastList.ElementAt(elementIndex).ConditionIcon = "";
			}

			elementIndex += 1;
		}
	}


	/// <summary>
	/// Get the long text forecast for all the time periods
	/// </summary>
	private void GetTextForecast(XElement xmlWeather, ObservableCollection<ForecastPeriod> newForecastList)
	{
		XElement xmlCurrent = default(XElement);
		int elementIndex = 0;

		// get a text forecast for each time period
		xmlCurrent = xmlWeather.Descendants("wordedForecast").First();

		foreach (XElement curElement in xmlCurrent.Elements("text")) {
			try {
				newForecastList.ElementAt(elementIndex).TextForecast = Convert.ToString(curElement.Value);
			} catch (FormatException e1) {
				newForecastList.ElementAt(elementIndex).TextForecast = "";
			}

			elementIndex += 1;
		}

	}
}

/// <summary>
/// State information for our BeginGetResponse async call
/// </summary>
public class ForecastUpdateState
{
	public HttpWebRequest AsyncRequest { get; set; }
	public HttpWebResponse AsyncResponse { get; set; }
}
