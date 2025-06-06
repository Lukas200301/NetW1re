using Avalonia.ReactiveUI;
using NetW1reAvalonia.Core.Helpers;
using NetW1reAvalonia.Core.ViewModels;
using NetW1reAvalonia.Core.ViewModels.InteractionViewModels;
using NetW1reAvalonia.Core.ViewModels.RoutedViewModels;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace NetW1reAvalonia.Core.Views.RoutedViews
{
	public partial class HomeView : ReactiveUserControl<HomeViewModel>
	{
		public HomeView()
		{
			this.WhenActivated(disposables =>
			{
				ViewModel!
					.SetFriendlyNameInteraction!
					.RegisterHandler(DoShowSetFriendlyDeviceNameDialogAsync)
					.DisposeWith(disposables);

				ViewModel!
					.ShowLimitDialogInteraction!
					.RegisterHandler(DoShowLimitDialogAsync)
					.DisposeWith(disposables);

			});

			InitializeComponent();
		}

		private async Task DoShowSetFriendlyDeviceNameDialogAsync(InteractionContext<string?, string?> interaction)
		{
			var setNameDialogWindow = new SetNameView
			{
				DataContext = new SetNameViewModel() { Name = interaction.Input }
			};

			var result = await setNameDialogWindow.ShowDialog<string?>(StaticData.MainWindow);
			interaction.SetOutput(result);
		}

		private async Task DoShowLimitDialogAsync(
			InteractionContext<DeviceLimitsModel?, DeviceLimitsModel?> interaction)
		{
			var dialog = new LimitView
			{
				DataContext = new LimitViewModel() { DeviceLimits = interaction.Input }
			};

			var result = await dialog.ShowDialog<DeviceLimitsModel>(StaticData.MainWindow);
			interaction.SetOutput(result);
		}
	}
}
