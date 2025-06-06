using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NetW1reAvalonia.Core.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace NetW1reAvalonia.Core.Views;

public partial class AdapterSelectView : ReactiveWindow<AdapterSelectViewModel>
{
	public AdapterSelectView()
	{
		this.WhenActivated(disposables =>
		{
			if (ViewModel!.IsAppLocked)
			{
				var passwordWindow = new PasswordView
				{
					DataContext = this.ViewModel._passwordViewModel
				};

				passwordWindow.ShowDialog(this).DisposeWith(disposables);
			}

		});

		InitializeComponent();
	}
}
