using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NetW1reAvalonia.Core.ViewModels.RoutedViewModels;
using ReactiveUI;

namespace NetW1reAvalonia.Core.Views.RoutedViews
{
    public partial class SnifferView : ReactiveUserControl<SnifferViewModel>
    {
        public SnifferView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            
            var dataGrid = this.FindControl<DataGrid>("PacketDataGrid");
            if (dataGrid != null)
            {
                dataGrid.DoubleTapped += OnDataGridDoubleClick;
            }
        }        private void OnDataGridDoubleClick(object? sender, TappedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.OpenDetailedPacketWindowCommand.Execute(System.Reactive.Unit.Default);
            }
        }
    }
}
