using Avalonia.ReactiveUI;
using NetW1reAvalonia.Core.Services.Implementations.StatusMessages;
using NetW1reAvalonia.Core.ViewModels;
using NetW1reAvalonia.Core.ViewModels.InteractionViewModels;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace NetW1reAvalonia.Core.Views
{
	public partial class MainView : ReactiveWindow<MainViewModel>
	{
		public MainView()
		{
			this.WhenActivated(disposables =>
			{
				StatusMessageService
					.MessageInteraction
					.RegisterHandler(DoShowMessageDialogAsync)
					.DisposeWith(disposables);
	
				ViewModel!
					.ShowAppLogInteraction!
					.RegisterHandler(DoShowAppLogDialog)
					.DisposeWith(disposables);
			});

			InitializeComponent();
		}

		private void DoShowAppLogDialog(
			InteractionContext<Unit, Unit> interaction)
		{
			var dialog = new AppLogView
			{
				DataContext = ViewModel!._appLogViewModel
			};

			dialog.Show(this);
			interaction.SetOutput(Unit.Default);
		}

		private async Task DoShowMessageDialogAsync(InteractionContext<StatusMessageModel, Unit> interaction)
		{
			var statusMessageDialog = new StatusMessageView();
			statusMessageDialog.DataContext = new StatusMessageViewModel() { StatusMessage = interaction.Input };

			var result = await statusMessageDialog.ShowDialog<Unit>(this);
			interaction.SetOutput(result);
		}
	}
}
