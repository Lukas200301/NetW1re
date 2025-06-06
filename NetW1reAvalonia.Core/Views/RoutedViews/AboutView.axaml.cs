using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using NetW1reAvalonia.Core.Helpers;
using NetW1reAvalonia.Core.ViewModels.RoutedViewModels;
using ReactiveUI;
using System.Diagnostics;
using System.Linq;

namespace NetW1reAvalonia.Core.Views.RoutedViews
{
	public partial class AboutView : ReactiveUserControl<AboutViewModel>
	{
		public AboutView()
		{
			this.WhenActivated(disposables =>
			{
			});

			InitializeComponent();
		}
	}
}
