using NetW1reAvalonia.Core.ViewModels.InteractionViewModels;

namespace NetW1reAvalonia.Core.Services
{
	public interface IErrorHandler
	{
		void HandleError(StatusMessageModel statusMessageModel);
	}
}
