namespace Objectivity.Bot.Utils
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    public static class DateParser
    {
        public static string ParseDotsToDashes(string date)
        {
            var pattern = "(?<day>\\d{1,2}).\\b(?<month>\\d{1,2}).(?<year>\\d{2,4})\\b";
            var newPattern = @"${day}-${month}-${year}";
            var output = Regex.Replace(date, pattern, newPattern, RegexOptions.None);
            return output;
        }

        public static bool IsStringContainsDotsInDate(string date)
        {
            return Regex.IsMatch(date, @"\d\d.\d\d.\d\d\d\d");
        }

        private static bool TryParseDateWithPriorFormat(string date, out DateTime dateTime, string format = null)
        {
            DateTime tempDate = DateTime.MinValue;
                if (!DateTime.TryParseExact(
                    date,
                    format ?? "yyyy-dd-MM",
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.None,
                    out tempDate))
                {
                    DateTime.TryParse(date, out tempDate);
                }

            dateTime = tempDate;
            return tempDate > DateTime.MinValue;
        }

        public static DateTime ParseDate(string dateString, bool isParsable = true)
        {
            var today = DateTime.Now.Date;

            // "XXXX-30-09" - "september 30th",
            if (RegexHelper.GetRegex(EnumRegexTypes.MonthAndDayMatch).IsMatch(dateString))
            {
                return ParseDate(dateString.Replace("XXXX", today.Year.ToString()), isParsable);
            }

            // "2015-09-30"
            DateTime date;
            if (DateParser.TryParseDateWithPriorFormat(dateString, out date, isParsable ? null : "yyyy-MM-dd"))
            {
                return date;
            }

            // "2015-W34" - next week
            var weekNumberMatch = RegexHelper.GetRegex(EnumRegexTypes.WeekNumberMatch).Match(dateString);
            if (weekNumberMatch.Success)
            {
                int year;
                int weekNumber;
                int.TryParse(weekNumberMatch.Groups["year"].Value, out year);
                int.TryParse(weekNumberMatch.Groups["weekNumber"].Value, out weekNumber);
                return FirstDateOfWeek(year, weekNumber, new CultureInfo("pl-PL")); // first monday
            }

            // "XXXX-WXX-1" get monday
            var dayOfTheWeekMatch = RegexHelper.GetRegex(EnumRegexTypes.DayOfTheWeekMatch).Match(dateString);
            if (dayOfTheWeekMatch.Success)
            {
                int desiredDayOfWeek;
                int.TryParse(dayOfTheWeekMatch.Groups["dayOfWeek"].Value, out desiredDayOfWeek);
                var todayDayOfWeek = (int)today.DayOfWeek;
                return today.AddDays(desiredDayOfWeek - todayDayOfWeek);
            }

            // "XXXX-11" or "2017-11" get november
            var monthOfTheYearMatch = RegexHelper.GetRegex(EnumRegexTypes.MonthOfTheYearMatch).Match(dateString);
            if (monthOfTheYearMatch.Success)
            {
                int desiredMonthOfTheYear;
                int year;

                if (!int.TryParse(weekNumberMatch.Groups["year"].Value, out year))
                {
                    year = DateTime.Now.Year;
                }

                if (int.TryParse(monthOfTheYearMatch.Groups["monthOfTheYear"].Value, out desiredMonthOfTheYear))
                {
                    return desiredMonthOfTheYear > DateTime.Now.Month
                               ? new DateTime(year, desiredMonthOfTheYear, 1)
                               : DateTime.Now;
                }
            }

            return today;
        }

        private static DateTime FirstDateOfWeek(int year, int weekOfYear, CultureInfo ci)
        {
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            var firstWeekDay = jan1.AddDays(daysOffset);
            var firstWeek = ci.Calendar.GetWeekOfYear(
                jan1,
                ci.DateTimeFormat.CalendarWeekRule,
                ci.DateTimeFormat.FirstDayOfWeek);
            if ((firstWeek <= 1 || firstWeek >= 52) && daysOffset >= -3)
            {
                weekOfYear -= 1;
            }

            return firstWeekDay.AddDays(weekOfYear * 7);
        }
    }
}