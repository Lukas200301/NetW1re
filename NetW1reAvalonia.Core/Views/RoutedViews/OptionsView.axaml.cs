using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NetW1reAvalonia.Core.Configuration;
using NetW1reAvalonia.Core.Helpers;
using NetW1reAvalonia.Core.ViewModels.RoutedViewModels;
using ReactiveUI;

namespace NetW1reAvalonia.Core.Views.RoutedViews
{
    public partial class OptionsView : ReactiveUserControl<OptionsViewModel>
	{
		public OptionsView()
		{
			this.WhenActivated(disposables =>
			{
				ViewModel!.AppSettings = Config.AppSettings!;
			});

			InitializeComponent();
		}
	}
}
