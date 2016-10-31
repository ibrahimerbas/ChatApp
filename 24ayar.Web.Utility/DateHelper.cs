using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace _24ayar.Web.Utility
{
    public static class DateHelper
    {
        public static int GetWeekNumber(this DateTime date)
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(/*date.AddDays(4 - (day == 0 ? 7 : day))*/date, CalendarWeekRule.FirstDay, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
        }
        public static DateTime GetFirstDateOfWeek(this DateTime date)
        {
            if (date == DateTime.MinValue)
                return date;

            int week = date.GetWeekNumber();
            while (week == date.GetWeekNumber())
                date = date.AddDays(-1);
            return date.AddDays(1);
        }
        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);

            int daysOffset = (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;

            DateTime firstMonday = jan1.AddDays(daysOffset);

            int firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(jan1, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);

            if (firstWeek <= 1)
            {
                weekOfYear -= 1;
            }

            return firstMonday.AddDays(weekOfYear * 7);
        }
        public static DateTime GetLastDateOfWeek(this DateTime date)
        {
            if (date == DateTime.MinValue)
                return date;

            int week = date.GetWeekNumber();
            while (week == date.GetWeekNumber())
                date = date.AddDays(+1);
            return date.AddDays(-1);
        }
    }
}
