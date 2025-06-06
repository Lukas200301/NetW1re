using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NetW1reAvalonia.Core.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace NetW1reAvalonia.Core.Views;

public partial class SetNameView : ReactiveWindow<SetNameViewModel>
{
	public SetNameView()
	{
		this.WhenActivated(disposables => { ViewModel!.Accept.Subscribe(Close).DisposeWith(disposables); });

		InitializeComponent();
	}
}
