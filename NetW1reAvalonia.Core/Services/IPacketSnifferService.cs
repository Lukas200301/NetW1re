using NetW1reAvalonia.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NetW1reAvalonia.Core.Services
{    public interface IPacketSnifferService
    {
        ObservableCollection<PacketInfo> CapturedPackets { get; }
        bool IsCapturing { get; }
        event EventHandler<PacketInfo>? PacketCaptured;
        
        Task StartCaptureAsync(string deviceName);
        Task StopCaptureAsync();
        void ClearPackets();
        Task<string[]> GetNetworkDevicesAsync();
    }
}
