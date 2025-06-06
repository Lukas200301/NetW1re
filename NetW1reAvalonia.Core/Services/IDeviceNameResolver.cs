using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetW1reAvalonia.Core.Models;
using NetW1reAvalonia.Core.ViewModels.InteractionViewModels;

namespace NetW1reAvalonia.Core.Services
{
    public interface IDeviceNameResolver
	{
		public List<DeviceNameModel> DevicesNames { get; }
		Task ResolveDeviceNameAsync(Device device);
		void SaveDeviceNamesAsync(IEnumerable<Device> devices);
		void ClearDeviceNames();
	}
}
