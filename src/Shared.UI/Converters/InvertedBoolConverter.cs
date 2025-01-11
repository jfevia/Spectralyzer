// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;

namespace Spectralyzer.Shared.UI.Converters;

[ValueConversion(typeof(bool), typeof(bool))]
public sealed class InvertedBoolConverter : IValueConverter
{
    public static InvertedBoolConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not bool boolValue ? throw new InvalidCastException() : !boolValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not bool boolValue ? throw new InvalidCastException() : !boolValue;
    }
}