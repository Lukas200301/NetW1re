using Avalonia.Data.Converters;
using System;
using System.Globalization;
using System.Linq;

namespace NetW1reAvalonia.Core.Converters
{
	public class StringEqualsConverter : IValueConverter
	{
		private string[] operators = new string[] { "!=", "=", "==" };
		
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value == null || parameter == null) return false;

			var valueStr = value as string;

			try
			{
				System.Diagnostics.Debug.WriteLine($"StringEqualsConverter: value='{valueStr}', parameter='{parameter}'");
				
				if (parameter.ToString()!.Split('|') is [var op, var str] &&
					operators.Contains(op))
				{
					var result = op switch
					{
						"!=" => valueStr != str,
						"=" or "==" => valueStr == str,
						_ => false
					};
					
					System.Diagnostics.Debug.WriteLine($"StringEqualsConverter: '{valueStr}' {op} '{str}' = {result}");
					return result;
				}
			}
			catch { }

			return false;
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
