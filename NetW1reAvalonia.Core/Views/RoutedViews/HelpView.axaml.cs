using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NetW1reAvalonia.Core.ViewModels.RoutedViewModels;
using ReactiveUI;

namespace NetW1reAvalonia.Core.Views.RoutedViews
{
    public partial class HelpView : ReactiveUserControl<HelpViewModel>
    {
        public HelpView()
        {
            this.WhenActivated(disposables => { });
            InitializeComponent();
        }
    }
}
