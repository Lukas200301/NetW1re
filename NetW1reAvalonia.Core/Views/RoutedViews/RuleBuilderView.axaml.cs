using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NetW1reAvalonia.Core.Helpers;
using NetW1reAvalonia.Core.ViewModels;
using NetW1reAvalonia.Core.ViewModels.InteractionViewModels;
using NetW1reAvalonia.Core.ViewModels.RoutedViewModels;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace NetW1reAvalonia.Core.Views.RoutedViews
{
    public partial class RuleBuilderView : ReactiveUserControl<RuleBuilderViewModel>
	{
		public RuleBuilderView()
		{
			this.WhenActivated(d =>
			{
				ViewModel!.ShowAddRuleDialog.RegisterHandler(DoShowAddDialogAsync).DisposeWith(d);
				ViewModel!.ShowUpdateRuleDialog.RegisterHandler(DoShowUpdateDialogAsync).DisposeWith(d);

				var listBox = this.FindControl<ListBox>("RuleList");

				listBox.ContextRequested += (sender, args) =>
				{
					listBox.SelectedItem = null;
					args.Handled = true;
				};
			});

			InitializeComponent();
		}

		private async Task DoShowAddDialogAsync(InteractionContext<Unit, AddUpdateRuleModel?> interaction)
		{
			var dialog = new AddUpdateRuleView
			{
				DataContext = new AddUpdateRuleViewModel(isUpdate: false)
			};

			var result = await dialog.ShowDialog<AddUpdateRuleModel?>(StaticData.MainWindow);
			interaction.SetOutput(result);
		}

		private async Task DoShowUpdateDialogAsync(InteractionContext<AddUpdateRuleModel, AddUpdateRuleModel> interaction)
		{
			var dialog = new AddUpdateRuleView
			{
				DataContext = new AddUpdateRuleViewModel(isUpdate: true)
				{
					AddUpdateRuleModel = interaction.Input
				}
			};

			var result = await dialog.ShowDialog<AddUpdateRuleModel>(StaticData.MainWindow);
			interaction.SetOutput(result);
		}
	}
}
