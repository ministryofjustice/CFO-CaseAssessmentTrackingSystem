using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class DateDimension
{
    public DateTime TheDate { get; set; } // Primary key

    public int TheDay { get; set; }
    public string TheDaySuffix { get; set; }
    public string TheDayName { get; set; }
    public int TheDayOfWeek { get; set; }
    public int TheDayOfWeekInMonth { get; set; }
    public int TheDayOfYear { get; set; }
    public bool IsWeekend { get; set; }
    public int TheWeek { get; set; }
    public int TheISOweek { get; set; }
    public DateTime TheFirstOfWeek { get; set; }
    public DateTime TheLastOfWeek { get; set; }
    public int TheWeekOfMonth { get; set; }
    public int TheMonth { get; set; }
    public string TheMonthName { get; set; }
    public DateTime TheFirstOfMonth { get; set; }
    public DateTime TheLastOfMonth { get; set; }
    public DateTime TheFirstOfNextMonth { get; set; }
    public DateTime TheLastOfNextMonth { get; set; }
    public int TheQuarter { get; set; }
    public DateTime TheFirstOfQuarter { get; set; }
    public DateTime TheLastOfQuarter { get; set; }
    public int TheYear { get; set; }
    public int TheISOYear { get; set; }
    public DateTime TheFirstOfYear { get; set; }
    public DateTime TheLastOfYear { get; set; }
    public bool IsLeapYear { get; set; }
    public bool Has53Weeks { get; set; }
    public bool Has53ISOWeeks { get; set; }
    public string MMYYYY { get; set; }
    public string Style101 { get; set; } // MM/DD/YYYY
    public string Style103 { get; set; } // DD/MM/YYYY
    public string Style112 { get; set; } // YYYYMMDD
    public string Style120 { get; set; } // YYYY-MM-DD
}