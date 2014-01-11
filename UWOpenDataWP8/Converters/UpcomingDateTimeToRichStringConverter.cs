using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace UWOpenDataWP8.Converters
{
    public class UpcomingDateTimeToRichStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is String)
            {
                DateTime inputDateTime;

                if (DateTime.TryParse(value as String, culture, DateTimeStyles.AdjustToUniversal, out inputDateTime))
                {
                    return GetRichString(inputDateTime);
                }
                else
                {
                    return value;
                }
            }
            else if (value is DateTime)
            {
                return GetRichString((DateTime)value);
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack: " + this.GetType().Name);
        }

        public String GetRichString(DateTime upcomingDate)
        {
            if (upcomingDate.Date.ToUniversalTime() >= DateTime.UtcNow.Date.AddDays(14))
            {
                return upcomingDate.ToLongDateString();
            }
            if (upcomingDate.Date.ToUniversalTime() >= DateTime.UtcNow.Date.AddDays(7))
            {
                return String.Format("{0} {1}", "next", upcomingDate.DayOfWeek);
            }
            if (upcomingDate.Date.ToUniversalTime() >= DateTime.UtcNow.Date.AddDays(1))
            {
                return String.Format("{0} {1}", "this", upcomingDate.DayOfWeek);
            }
            return String.Format("{0} {1}", "in", upcomingDate.ToShortTimeString());
        }
    }
}
