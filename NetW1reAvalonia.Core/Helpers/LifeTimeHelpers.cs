using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using NetW1reAvalonia.Core.Configuration;
using NetW1reAvalonia.Core.Services;
using NetW1reAvalonia.Core.ViewModels;
using NetW1reAvalonia.Core.ViewModels.RoutedViewModels;
using NetW1reAvalonia.Core.Views;
using System;

namespace NetW1reAvalonia.Core.Helpers
{	public class LifeTimeHelpers
	{
		public static void ConfigureAppStart(Application application)
		{
			try
			{
				Console.WriteLine("ConfigureAppStart started...");
				var desktop = application.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

				Console.WriteLine("Creating AdapterSelectView...");
				var adapterSelectWindow = new AdapterSelectView
				{
					DataContext = DependencyInjectionHelpers.ResolveIfNull<AdapterSelectViewModel>(null!)
				};

				Console.WriteLine("Setting MainWindow...");
				desktop!.MainWindow = adapterSelectWindow;

				Console.WriteLine("Setting up Accept subscription...");
				adapterSelectWindow.ViewModel!.Accept.Subscribe(x =>
				{
				var mainViewModel = DependencyInjectionHelpers.ResolveIfNull<MainViewModel>(null!);
				application.DataContext = mainViewModel;

				desktop.ShutdownRequested += (sender, args) =>
				{
					var homeViewModel = DependencyInjectionHelpers.ResolveIfNull<HomeViewModel>(null!);

					var deviceNameResolver = DependencyInjectionHelpers.ResolveIfNull<IDeviceNameResolver>(null!);
					deviceNameResolver.SaveDeviceNamesAsync(homeViewModel.GetUiDeviceCollection()); 

					Config.AppSettings?.SaveChanges();

					var rulesService = DependencyInjectionHelpers.ResolveIfNull<IRuleService>(null!);
					rulesService.SaveRules();
				};


				desktop.MainWindow = new MainView
				{
					DataContext = mainViewModel,
					ViewModel = mainViewModel
				};

				StaticData.MainWindow = desktop.MainWindow;

				desktop.MainWindow.Show();
				adapterSelectWindow.Close();				mainViewModel.InitTrayIcon();
			});
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error in ConfigureAppStart: {e.Message}");
				Console.WriteLine($"Stack trace: {e.StackTrace}");
				throw;
			}
		}

		public static void ShowApp()
		{

			StaticData.MainWindow!.WindowState = Avalonia.Controls.WindowState.Normal;
			StaticData.MainWindow!.WindowState = Avalonia.Controls.WindowState.Minimized;
			StaticData.MainWindow!.WindowState = Avalonia.Controls.WindowState.Normal;
		}

		public static void ExitApp()
		{
			var app = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
			app?.Shutdown();
		}
	}
}
