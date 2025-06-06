using System;
using System.Threading.Tasks;
using NetW1reAvalonia.Core.Models;

namespace NetW1reAvalonia.Core.Services
{
    public interface IDeviceTypeIdentifier : IDisposable
    {
        void IdentifyDevice(Device device);
    }
}
