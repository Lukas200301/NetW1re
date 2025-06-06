using System;
using System.Globalization;
using Avalonia.Data.Converters;
using NetW1reAvalonia.Core.Models;

namespace NetW1reAvalonia.Core.Converters;

public class DeviceStatusToIntConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var blockedVal = System.Convert.ToBoolean(value);

        if (blockedVal)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
