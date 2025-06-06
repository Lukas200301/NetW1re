using Avalonia;
using Avalonia.ReactiveUI;
using NetW1reAvalonia.Core;
using NetW1reAvalonia.Core.Configuration;
using NetW1reAvalonia.Core.Helpers;
using Serilog;
using System;

namespace NetW1reAvalonia.Windows
{
	public class Program
	{
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args)
	{
		Console.WriteLine("NetW1re application starting...");
		Console.WriteLine($"Args: {string.Join(" ", args)}");
		
		try
		{
			Console.WriteLine("Starting NetW1re application...");
			BuildAvaloniaApp()
				.StartWithClassicDesktopLifetime(args);
		}
		catch (Exception e)
		{
			Console.WriteLine($"Error in Main: {e.Message}");
			Console.WriteLine($"Stack trace: {e.StackTrace}");
			Log.Error(LogMessageTemplates.ExceptionTemplate,
				e.GetType(), nameof(Main), e.Message);
		}
		finally
		{
			Console.WriteLine("Application ended.");
			Log.CloseAndFlush();
		}
	}

		// Avalonia configuration, don't remove; also used by visual designer.
		public static AppBuilder BuildAvaloniaApp()
		{
			try
			{
				Console.WriteLine("Reading app configuration...");
				// Read app config
				Config.ReadConfiguration();

				Console.WriteLine("Registering dependencies...");
				DependencyInjectionHelpers.RegisterAppDependencies();

				// Register platform specific deps
				Helpers.DependencyInjectionHelpers.RegisterAppDependencies();

				Console.WriteLine("Building Avalonia app...");
				return AppBuilder.Configure<App>()
					.UsePlatformDetect()
					.LogToTrace()
					.UseReactiveUI();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error in BuildAvaloniaApp: {e.Message}");
				Console.WriteLine($"Stack trace: {e.StackTrace}");
				throw;
			}
		}
	}
}
