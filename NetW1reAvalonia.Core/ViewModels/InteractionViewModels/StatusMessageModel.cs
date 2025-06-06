using NetW1reAvalonia.Core.Models;

namespace NetW1reAvalonia.Core.ViewModels.InteractionViewModels;

public class StatusMessageModel
{
    public MessageType MessageType { get; set; }
    public string Message { get; set; }

    public StatusMessageModel(MessageType messageType, string message)
    {
        MessageType = messageType;
        Message = message;
    }
}
