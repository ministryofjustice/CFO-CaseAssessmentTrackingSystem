using Cfo.Cats.Domain.Entities.ManagementInformation;
using System.Globalization;

namespace Cfo.Cats.Infrastructure.Persistence.Seeding;

public static class DateDimensionSeeder
{
    public static IEnumerable<DateDimension> GenerateDateDimensions(DateTime startDate, DateTime endDate)
    {
        var dateDimensions = new List<DateDimension>();

        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);
            var lastDayOfNextMonth = firstDayOfNextMonth.AddMonths(1).AddDays(-1);
            var quarter = (date.Month - 1) / 3 + 1;
            var firstDayOfQuarter = new DateTime(date.Year, (quarter - 1) * 3 + 1, 1);
            var lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);
            var firstDayOfYear = new DateTime(date.Year, 1, 1);
            var lastDayOfYear = new DateTime(date.Year, 12, 31);

            dateDimensions.Add(new DateDimension
            {
                TheDate = date,
                TheDay = date.Day,
                TheDaySuffix = GetDaySuffix(date.Day),
                TheDayName = date.ToString("dddd"),
                TheDayOfWeek = (int)date.DayOfWeek + 1,
                TheDayOfWeekInMonth = (date.Day + 6) / 7,
                TheDayOfYear = date.DayOfYear,
                IsWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday,
                TheWeek = ISOWeek.GetWeekOfYear(date),
                TheISOweek = ISOWeek.GetWeekOfYear(date),
                TheFirstOfWeek = date.AddDays(-(int)date.DayOfWeek),
                TheLastOfWeek = date.AddDays(6 - (int)date.DayOfWeek),
                TheWeekOfMonth = (date.Day - 1) / 7 + 1,
                TheMonth = date.Month,
                TheMonthName = date.ToString("MMMM"),
                TheFirstOfMonth = firstDayOfMonth,
                TheLastOfMonth = lastDayOfMonth,
                TheFirstOfNextMonth = firstDayOfNextMonth,
                TheLastOfNextMonth = lastDayOfNextMonth,
                TheQuarter = quarter,
                TheFirstOfQuarter = firstDayOfQuarter,
                TheLastOfQuarter = lastDayOfQuarter,
                TheYear = date.Year,
                TheISOYear = ISOWeek.GetYear(date),
                TheFirstOfYear = firstDayOfYear,
                TheLastOfYear = lastDayOfYear,
                IsLeapYear = DateTime.IsLeapYear(date.Year),
                Has53Weeks = GetHas53Weeks(date.Year),
                Has53ISOWeeks = ISOWeek.GetWeeksInYear(date.Year) == 53,
                MMYYYY = date.ToString("MM/yyyy"),
                Style101 = date.ToString("MM/dd/yyyy"),
                Style103 = date.ToString("dd/MM/yyyy"),
                Style112 = date.ToString("yyyyMMdd"),
                Style120 = date.ToString("yyyy-MM-dd")
            });
        }

        return dateDimensions;
    }

    private static string GetDaySuffix(int day)
    {
        return day switch
        {
            1 or 21 or 31 => "st",
            2 or 22 => "nd",
            3 or 23 => "rd",
            _ => "th"
        };
    }

    private static bool GetHas53Weeks(int year)
    {
        var dec31 = new DateTime(year, 12, 31);
        return dec31.DayOfWeek == DayOfWeek.Thursday || (dec31.DayOfWeek == DayOfWeek.Wednesday && DateTime.IsLeapYear(year));
    }
}