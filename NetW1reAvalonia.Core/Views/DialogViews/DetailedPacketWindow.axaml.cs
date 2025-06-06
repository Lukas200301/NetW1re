using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NetW1reAvalonia.Core.ViewModels.DialogViewModels;

namespace NetW1reAvalonia.Core.Views.DialogViews
{
    public partial class DetailedPacketWindow : Window
    {
        public DetailedPacketWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            
            this.DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object? sender, EventArgs e)
        {
            if (DataContext is DetailedPacketViewModel viewModel)
            {
                viewModel.CloseRequested += (s, e) => Close();
            }
        }
    }
}
