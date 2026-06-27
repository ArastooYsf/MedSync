using System;
using System.Globalization;

namespace MedSync.Helpers;

public static class PersianCalendarHelper
{
    private static readonly PersianCalendar _pc = new();

    public static (int Year, int Month, int Day) ToPersian(DateTime date)
    {
        return (
            _pc.GetYear(date),
            _pc.GetMonth(date),
            _pc.GetDayOfMonth(date)
        );
    }

    public static DateTime ToGregorian(int pYear, int pMonth, int pDay)
    {
        return _pc.ToDateTime(pYear, pMonth, pDay, 0, 0, 0, 0);
    }

    public static string FormatPersian(DateTime date)
    {
        return FormatPersian(date, "yyyy/MM/dd");
    }

    public static string FormatPersian(DateTime date, string format)
    {
        var (y, m, d) = ToPersian(date);
        var hour = date.Hour;
        var minute = date.Minute;
        var second = date.Second;

        return format.Replace("yyyy", y.ToString("D4"))
                     .Replace("yy", (y % 100).ToString("D2"))
                     .Replace("MM", m.ToString("D2"))
                     .Replace("dd", d.ToString("D2"))
                     .Replace("HH", hour.ToString("D2"))
                     .Replace("mm", minute.ToString("D2"))
                     .Replace("ss", second.ToString("D2"));
    }

    public static string FormatPersianLong(DateTime date)
    {
        var (y, m, d) = ToPersian(date);
        return $"{d} {GetPersianMonthName(m)} {y}";
    }

    public static string GetPersianMonthName(int month) => month switch
    {
        1 => "فروردین",
        2 => "اردیبهشت",
        3 => "خرداد",
        4 => "تیر",
        5 => "مرداد",
        6 => "شهریور",
        7 => "مهر",
        8 => "آبان",
        9 => "آذر",
        10 => "دی",
        11 => "بهمن",
        12 => "اسفند",
        _ => ""
    };

    public static int GetPersianDaysInMonth(int year, int month)
    {
        return _pc.GetDaysInMonth(year, month);
    }

    public static int GetPersianYear(DateTime date) => _pc.GetYear(date);
    public static int GetPersianMonth(DateTime date) => _pc.GetMonth(date);
    public static int GetPersianDay(DateTime date) => _pc.GetDayOfMonth(date);
}
