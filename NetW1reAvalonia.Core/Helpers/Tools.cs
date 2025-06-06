using NetW1reAvalonia.Core.Configuration;
using NetW1reAvalonia.Core.ViewModels.InteractionViewModels;
using ReactiveUI;
using System.Diagnostics;

namespace NetW1reAvalonia.Core.Helpers;

public class Tools
{
	public static void OpenLink(string link)
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = link,
			UseShellExecute = true
		});
	}
}
