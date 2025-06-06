using NetW1reAvalonia.Core.Services;
using NetW1reAvalonia.Windows.Services.Implementations;
using Splat;

namespace NetW1reAvalonia.Windows.Helpers
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
			//SplatRegistrations.RegisterLazySingleton<IAppLockService, AppLockManagerWindows>();
			Locator.CurrentMutable.RegisterLazySingleton(() => new AppLockManagerWindows(), typeof(IAppLockService));
		}
	}
}
