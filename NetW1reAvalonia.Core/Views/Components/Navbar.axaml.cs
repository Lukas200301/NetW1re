using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NetW1reAvalonia.Core.Views.Components
{
    public partial class Navbar : UserControl
    {
        public bool Collapsed
        {
            get
            {
                return (bool)GetValue(CollapsedProperty);
            }
            set
            {
                SetValue(CollapsedProperty, value);

                ToggleNavbar(value);
            }
        }

        public static readonly AttachedProperty<bool> CollapsedProperty =
            AttachedProperty<bool>.RegisterAttached<Navbar, bool>(nameof(Collapsed), typeof(Navbar));

        public Navbar()
        {
            InitializeComponent();
        }

        private void ToggleNavbar(bool state)
        {
            navbar.IsVisible = state;
        }
    }
}
