using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
namespace _24ayar.Web.Utility
{
    public class CalendarModel
    {
        private CalendarModel()
        {
            this.CalendarItems = new List<CalendarItem>();
            this.Headers = new List<string>();
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //public Func<int, int, string> GeneratePreviousMonth { get; set; }
        //public Func<int, int, string> GenerateNextMonth { get; set; }
        //public Func<int, int, string> GenerateWeekUrl { get; set; }
        //public Func<DateTime, string> GenerateDayUrl { get; set; }
        public List<string> Headers { get; set; }
        public string NextMonthUrl { get; set; }
        public string PreviousMonthUrl { get; set; }
        public string Title { get; set; }
        public string NextMonthName { get; set; }
        public string PreviousMonthName { get; set; }
        public List<CalendarItem> CalendarItems { get; set; }
        public static CalendarModel CreateCalender(
            int Year, 
            int Month, 
            Func<int, int, string> GeneratePreviousMonthUrlCallBack,
            Func<int, int, string> GenerateNextMonthUrlCallback,
            Func<int, int, string> GenerateWeekUrlCallback,
            Func<DateTime, string> GenerateDayUrlCallback,
            Action<DateTime,DateTime> CalendarInit,
            Action<DateTime,CalendarItem> RenderDay)
            
        {

            CalendarModel model = new CalendarModel();
            model.Headers.Add("Haftalar");
            for (int j = 0; j < 7; j++)
            {
                int currentDay = j + (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                if (currentDay > 6)
                    currentDay = currentDay - 7;
                model.Headers.Add(CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[currentDay]);
            }
            DateTime startDate = new DateTime(Year, Month, 1);
            int nextMonth = Month == 12 ? 1 : Month + 1;
            int nextYear = (Month == 12 ? Year + 1 : Year);
            int previousYear = (Month == 1 ? Year - 1 : Year);
            int previousMonth = Month == 1 ? 12 : Month - 1;
            model.NextMonthUrl = GenerateNextMonthUrlCallback != null ? GenerateNextMonthUrlCallback(nextYear, nextMonth) : "";
            model.PreviousMonthUrl = GeneratePreviousMonthUrlCallBack != null ? GeneratePreviousMonthUrlCallBack(previousYear, previousMonth) : "";
            int startWeek = startDate.GetWeekNumber();
            int endWeek = startWeek + 6;
            model.NextMonthName = new DateTime(nextYear,nextMonth,1).ToString("MMMM");
            model.PreviousMonthName = new DateTime(previousYear, previousMonth, 1).ToString("MMMM");
            model.Title = startDate.ToString("MMMM yyyy");
            DateTime calendarStartDate = startDate.GetFirstDateOfWeek();
            if (calendarStartDate == startDate)
            {
                calendarStartDate = startDate.AddDays(-1).GetFirstDateOfWeek(); // ayın birinden başlamasın diye bir önceki aydan da gün göstersin diye.
                startWeek = calendarStartDate.GetWeekNumber();
                endWeek = startWeek + 6;
            }
            DateTime calendarEndDate = calendarStartDate.AddDays(5*7).GetLastDateOfWeek();
            model.StartDate = calendarStartDate;
            model.EndDate = calendarEndDate;
            CalendarInit?.Invoke(model.StartDate, model.EndDate);
            int i = 0;
            while (calendarStartDate <= calendarEndDate)
            {
                var calendarItem = new CalendarItem();

                calendarItem.Date = calendarStartDate;
                calendarItem.Title = calendarStartDate.Day.ToString();
                calendarItem.IsOtherMonthDay = !(Month == calendarStartDate.Month);
                //calendarItem.Url = GenerateDayUrlCallback != null ? GenerateDayUrlCallback(calendarStartDate): "";
                //if (GenerateDayUrlCallback != null)
                //    calendarItem.Url = GenerateDayUrlCallback(calendarStartDate) ;
                calendarItem.Url = GenerateDayUrlCallback?.Invoke(calendarStartDate);
                RenderDay?.Invoke(calendarStartDate,calendarItem);

                if (i % 7 == 0)
                {
                    var calendarWeek = new CalendarItem();
                    calendarWeek.Date = calendarStartDate;
                    calendarWeek.Title = calendarStartDate.GetWeekNumber().ToString();
                    calendarWeek.WeekNumber = calendarStartDate.GetWeekNumber();
                    calendarWeek.IsWeek = true;
                    calendarWeek.Url = GenerateWeekUrlCallback != null ? GenerateWeekUrlCallback(calendarStartDate.Year, calendarWeek.WeekNumber) : "";
                    model.CalendarItems.Add(calendarWeek);
                }
                model.CalendarItems.Add(calendarItem);
                calendarStartDate =calendarStartDate.AddDays(1);
                i++;
            }
            return model;
        }
    }
    public class CalendarItem: EventDayModel
    {
        
        public string Title { get; set; }
        public bool IsWeek { get; set; }
        public int WeekNumber { get; set; }
        
        public bool IsOtherMonthDay { get; set; }
        public string Url { get; internal set; }
    }
    public class EventDayModel
    {
        public DateTime Date { get; set; }
        public List<object> Events { get; set; }
    }
}

