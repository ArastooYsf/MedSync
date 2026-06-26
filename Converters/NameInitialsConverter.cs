using System;
using System.Globalization;
using Avalonia.Data.Converters;
using MedSync.Models;

namespace MedSync.Converters;

public class NameInitialsConverter : IValueConverter
{
    public static readonly NameInitialsConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Patient patient) return "؟";

        var first = string.IsNullOrWhiteSpace(patient.FirstName) ? "؟" : patient.FirstName[0].ToString();
        var last = string.IsNullOrWhiteSpace(patient.LastName) ? "؟" : patient.LastName[0].ToString();

        return $"{first}.{last}";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}