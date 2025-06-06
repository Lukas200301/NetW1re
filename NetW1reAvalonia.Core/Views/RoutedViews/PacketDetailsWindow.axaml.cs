using Avalonia.Controls;
using NetW1reAvalonia.Core.ViewModels.RoutedViewModels;

namespace NetW1reAvalonia.Core.Views.RoutedViews
{
    public partial class PacketDetailsWindow : Window
    {
        public PacketDetailsWindow()
        {
            InitializeComponent();
        }

        public PacketDetailsWindow(PacketDetailsViewModel viewModel) : this()
        {
            DataContext = viewModel;
            
            viewModel.CloseRequested += (s, e) => Close();
        }
    }
}
