using Avalonia.Controls;
using Avalonia.ReactiveUI;
using NetW1reAvalonia.Core.ViewModels;

namespace NetW1reAvalonia.Core.Views
{
	public partial class AppLogView : ReactiveWindow<AppLogViewModel>
	{
		public AppLogView()
		{
			InitializeComponent();
		}
	}
}
