using Avalonia.Data.Converters;
using System;
using System.Globalization;
using System.Net;

namespace NetW1reAvalonia.Core.Converters
{
    public class IpAddressToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not IPAddress iPAddress)
                return null;

            return iPAddress.ToString();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return null;

            return IPAddress.Parse(value.ToString());
        }
    }
}
