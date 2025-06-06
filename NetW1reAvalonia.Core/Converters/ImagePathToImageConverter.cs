using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Splat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetW1reAvalonia.Core.Converters
{
	public class ImagePathToImageConverter : IValueConverter
	{
		public readonly static ImagePathToImageConverter Instance = new();

		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value == null)
				return null;

			if (value is string rawUri && targetType.IsAssignableFrom(typeof(Bitmap)))
			{
				Uri uri;

				// Allow for assembly overrides
				if (rawUri.StartsWith("avares://"))
				{
					uri = new Uri(rawUri);
				}
				else
				{
					string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
					uri = new Uri($"avares://{assemblyName}/{rawUri}");
				}

				var asset = AssetLoader.Open(uri);

				return new Bitmap(asset);
			}

			throw new NotSupportedException();
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
