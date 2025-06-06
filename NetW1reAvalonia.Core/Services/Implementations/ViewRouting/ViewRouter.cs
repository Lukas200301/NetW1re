using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetW1reAvalonia.Core.Services.Implementations.ViewRouting
{
	public class ViewRouter : IRouter
	{
		public RoutingState Router { get; set; }

		public ViewRouter()
		{
			Router = new RoutingState();
		}
	}
}
