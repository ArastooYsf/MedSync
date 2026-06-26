using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MedSync.Converters;

public class TimerBackgroundConverter : IMultiValueConverter
{
    public static readonly TimerBackgroundConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType,
        object? parameter, CultureInfo culture)
    {
        if (values is not [bool isPast, bool isNow, bool isUpcoming])
            return new SolidColorBrush(Color.Parse("#1A3A5C"));

        if (isPast)
            return new SolidColorBrush(Color.Parse("#4A1E28")); // قرمز تیره
        if (isNow)
            return new SolidColorBrush(Color.Parse("#1A4A3A")); // سبز تیره
        return new SolidColorBrush(Color.Parse("#1A3A5C"));     // آبی تیره (پیش‌فرض)
    }
}