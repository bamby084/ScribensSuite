using System;

namespace ScribensMSWord.ExtensionMethods
{
    public static class DateTimeExtensions
    {
        public static DateTime MinTimeOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }

        public static DateTime MaxTimeOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 29, 59);
        }
    }
}
