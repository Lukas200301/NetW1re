using NetW1reAvalonia.Core.ViewModels.InteractionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetW1reAvalonia.Core.Services
{
	public interface IStatusMessageService
	{
		Task ShowMessage(StatusMessageModel statusMessage);
	}
}
