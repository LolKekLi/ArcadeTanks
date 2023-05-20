using System;
using System.Globalization;

namespace Project
{
    public static class DateTimeExtensions
    {
        private const string SerializationDateFormat = "yyyy-MM-dd HH:mm:ss";
        
        public static string Serialize(this DateTime dateTime)
        {
            return dateTime.ToString(SerializationDateFormat);
        }
        
        public static bool TryDeserializeDateTime(this string data, out DateTime result)
        {
            return DateTime.TryParseExact(data, SerializationDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }

        public static TimeSpan ToTimeSpan(this DateTime date)
        {
            return TimeSpan.FromTicks(date.Ticks);
        } 
    }
}