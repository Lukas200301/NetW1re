using Avalonia.ReactiveUI;
using NetW1reAvalonia.Core.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace NetW1reAvalonia.Core.Views;

public partial class StatusMessageView : ReactiveWindow<StatusMessageViewModel>
{
	public StatusMessageView()
	{
		this.WhenActivated(disposables => { ViewModel!.Close.Subscribe(x => Close()).DisposeWith(disposables); });

		InitializeComponent();
	}
}
