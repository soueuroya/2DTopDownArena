using System;

public class DateHelpers
{
    public static DateTime parseDateString(string dateString, DateTime defaultValue = default(DateTime))
    {
        // TODO: if this isn't reliable, we might have to use ParseExact and specify the format string
        return string.IsNullOrEmpty(dateString) ? defaultValue : DateTime.Parse(dateString);
    }

    public static DateTime dateFromSeconds(long timestamp)
    {
        System.DateTime retVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        retVal = retVal.AddSeconds(timestamp);
        return retVal;
    }

    public static double secondsFromDate(DateTime date)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (date.ToUniversalTime() - epoch).TotalSeconds;
    }

    public static string GetDaySuffix(DateTime date)
    {
        switch (date.Day) {
            case 1:
            case 21:
            case 31:
                return "st";
            case 2:
            case 22:
                return "nd";
            case 3:
            case 23:
                return "rd";
            default:
                return "th";
        }
    }

    public static double secondsSinceDate(DateTime date)
    {
        if (date == null) {
            return 0;
        }
        TimeSpan difference = DateTime.Now - date;
        return difference.TotalSeconds;
    }
    

    static readonly string[] kAllMonths = {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};
    public static string QuarterRange(int year, int quarter)
    {
        
        return kAllMonths[(quarter-1)*3] + " - " + kAllMonths[(quarter)*3-1] + " " + year;
    }

    public static string FormatTimeSpan(TimeSpan time, bool shortVersion = false)
    {
        int years = time.Days / 365;
        int weeks = time.Days / 7;
        string output = "";

        string amount = "";
        string unit = "";

        if (years > 0) {
            amount = years.ToString();
            unit = (shortVersion || years == 1) ? "year" : "years";

        } else if (weeks > 0) {
            amount = weeks.ToString();
            unit = (shortVersion || weeks == 1) ? "week" : "weeks";

        } else if (time.Days > 0) {
            amount = time.Days.ToString();
            unit = (shortVersion || time.Days == 1) ? "day" : "days";

        } else if (time.Hours > 0) {
            amount = time.Hours.ToString();
            unit = (shortVersion || time.Hours == 1) ? "hour" : "hours";

        } else if (time.Minutes > 0) {
            amount = time.Minutes.ToString();
            unit = "min";
        }

        string suffix = shortVersion ? "" : " ago";
        object[] amountArg = { amount };
        output = string.IsNullOrEmpty(unit) ? "just now" : $"{amount} {unit}{suffix}";

        return output;
    }
}