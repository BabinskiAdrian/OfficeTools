using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeTools.Shared
{
    public static class DateHelper
    {
        public static DateTime GetNextBusinessDay(DateTime date)
        {
            do
            {
                date = date.AddDays(1);
            }
            while (IsWeekend(date));

            return date;
        }

        public static bool IsWeekend(DateTime date) 
            => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }
}
