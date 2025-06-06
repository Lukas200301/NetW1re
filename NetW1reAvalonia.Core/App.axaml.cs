using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using NetW1reAvalonia.Core.Helpers;
using NetW1reAvalonia.Core.Views;
using Serilog;
using System;

namespace NetW1reAvalonia.Core
{
	public partial class App : Application
	{		public override void Initialize()
		{
			try
			{
				Console.WriteLine("Initializing Avalonia XAML...");
				AvaloniaXamlLoader.Load(this);
				Console.WriteLine("Avalonia XAML loaded successfully.");
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error in Initialize: {e.Message}");
				Console.WriteLine($"Stack trace: {e.StackTrace}");
				throw;
			}
		}
		public override void OnFrameworkInitializationCompleted()
		{
			try
			{
				Console.WriteLine("Framework initialization started...");
				if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
				{
					Console.WriteLine("Configuring app start...");
					LifeTimeHelpers.ConfigureAppStart(this);
				}

				Console.WriteLine("Calling base.OnFrameworkInitializationCompleted...");
				base.OnFrameworkInitializationCompleted();
				Console.WriteLine("Framework initialization completed successfully.");
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error in OnFrameworkInitializationCompleted: {e.Message}");
				Console.WriteLine($"Stack trace: {e.StackTrace}");
				Log.Error(LogMessageTemplates.ExceptionTemplate,
					e.GetType(), this.GetType(), e.Message);

				throw;
			}
		}
	}
}
