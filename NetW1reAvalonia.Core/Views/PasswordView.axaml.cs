using System;
using Avalonia.Controls.Mixins;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using NetW1reAvalonia.Core.ViewModels;
using ReactiveUI;

namespace NetW1reAvalonia.Core.Views
{
    public partial class PasswordView : ReactiveWindow<PasswordViewModel>
    {
        public PasswordView()
        {
            this.WhenActivated(disposables =>
            {
                ViewModel!.CloseWindow = ReactiveCommand.Create(() =>
                {
                    this.Close();
                });
            });

            InitializeComponent();
        }
    }
}
