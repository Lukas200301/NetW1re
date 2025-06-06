using NetW1reAvalonia.Core.Theme;
using ReactiveUI;

namespace NetW1reAvalonia.Core.ViewModels
{
	public class ViewModelBase : ReactiveObject
	{
		public AppTheme Theme => AppTheme.Instance;
	}
}
