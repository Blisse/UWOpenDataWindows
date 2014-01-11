using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using UWOpenDataLib.JsonModels.Events;
using UWOpenDataLib.JsonModels.FoodServices;
using UWOpenDataLib.JsonModels.Weather;
using UWOpenDataLib.Services;
using UWOpenDataWindowsBase.Utilities;

namespace UWOpenDataWindowsBase.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        #region Properties

        /// <summary>
        /// The <see cref="EventsHolidaysList" /> property's name.
        /// </summary>
        public const string EventsHolidaysListPropertyName = "EventsHolidaysListPropertyName";

        private ObservableCollection<HolidaysData> _eventsHolidaysList = new ObservableCollection<HolidaysData>();

        /// <summary>
        /// Sets and gets the EventsHolidaysList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<HolidaysData> EventsHolidaysList
        {
            get
            {
                return _eventsHolidaysList;
            }

            set
            {
                if (_eventsHolidaysList == value)
                {
                    return;
                }

                RaisePropertyChanging(EventsHolidaysListPropertyName);
                _eventsHolidaysList = value;
                RaisePropertyChanged(EventsHolidaysListPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="WeatherData" /> property's name.
        /// </summary>
        public const string WeatherDataPropertyName = "WeatherData";

        private WeatherData _weatherData;

        /// <summary>
        /// Sets and gets the WeatherData property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public WeatherData WeatherData
        {
            get
            {
                return _weatherData;
            }

            set
            {
                if (_weatherData == value)
                {
                    return;
                }

                RaisePropertyChanging(WeatherDataPropertyName);
                _weatherData = value;
                RaisePropertyChanged(WeatherDataPropertyName);
            }
        }

        #endregion



        public async Task GetHomePageData(CancellationToken cancellationToken)
        {
            if (IsDataLoading)
            {
                return;
            }
            IsDataLoading = true;


            await Task.WhenAll(
                GetEventsHolidaysListData(cancellationToken),
                GetWeatherCurrentData(cancellationToken)
            );

            IsDataLoading = false;
        }

        private async Task GetEventsHolidaysListData(CancellationToken cancellationToken)
        {
            var eventsListData = await GetData(EventsHolidaysListPropertyName, UwOpenDataApi.GetEventsHolidayData,
                cancellationToken);
            
            foreach (var holidayData in eventsListData.data)
            {
                DateTime holidayDateTime;
                
                if (DateTime.TryParse(holidayData.date, out holidayDateTime))
                {
                    if (holidayDateTime.ToUniversalTime().Date >= DateTime.UtcNow.Date)
                    {
                        EventsHolidaysList.Add(holidayData);
                        Debug.WriteLine(holidayDateTime);
                    }
                }
            }
        }

        private async Task GetWeatherCurrentData(CancellationToken cancellationToken)
        {
            var weatherData = await GetData(WeatherDataPropertyName, UwOpenDataApi.GetWeatherData,
                cancellationToken);

            if (weatherData != null)
            {
                WeatherData = weatherData.data;
            }
        }
    }
}
