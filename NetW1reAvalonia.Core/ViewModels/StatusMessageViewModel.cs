using System.Reactive;
using NetW1reAvalonia.Core.ViewModels.InteractionViewModels;
using ReactiveUI;

namespace NetW1reAvalonia.Core.ViewModels;

public class StatusMessageViewModel : ViewModelBase
{
    public StatusMessageModel? StatusMessage { get; set; }
    public ReactiveCommand<Unit, Unit> Close { get; set; }

    public StatusMessageViewModel()
    {
        Close = ReactiveCommand.Create(() => Unit.Default);
    }
}
