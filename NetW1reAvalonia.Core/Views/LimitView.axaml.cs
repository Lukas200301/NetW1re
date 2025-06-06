using Avalonia.ReactiveUI;
using NetW1reAvalonia.Core.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace NetW1reAvalonia.Core.Views;

public partial class LimitView : ReactiveWindow<LimitViewModel>
{
	public LimitView()
	{
		this.WhenActivated(disposables =>
		{
			ViewModel!.Apply.Subscribe(Close).DisposeWith(disposables);
		});

		InitializeComponent();
	}
}
