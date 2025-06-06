using Avalonia.Controls;
using Avalonia.ReactiveUI;
using NetW1reAvalonia.Core.ViewModels;
using ReactiveUI;
using System;

namespace NetW1reAvalonia.Core.Views
{
	public partial class AddUpdateRuleView : ReactiveWindow<AddUpdateRuleViewModel>
	{
		public AddUpdateRuleView()
		{
			this.WhenActivated(d => d(ViewModel!.Accept.Subscribe(Close)));
			InitializeComponent();
		}
	}
}
