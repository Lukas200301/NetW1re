using NetW1reAvalonia.Core.Services;
using NetW1reAvalonia.Linux.Services.Implementations;
using Splat;
using System.IO.Abstractions;

namespace NetW1reAvalonia.Linux.Helpers
{
	public class DependencyInjectionHelpers
	{
		public static void RegisterAppDependencies()
		{
			RegisterRequiredServices();
		}

		private static void RegisterRequiredServices()
		{
			// Source generation not working from this assembly, we'll register via the locator for now
			//SplatRegistrations.RegisterLazySingleton<IAppLockService, AppLockManagerLinux>();
			Locator.CurrentMutable.RegisterLazySingleton(() => new AppLockManagerLinux(Locator.Current.GetService<IFileSystem>()), typeof(IAppLockService));
		}
	}
}
