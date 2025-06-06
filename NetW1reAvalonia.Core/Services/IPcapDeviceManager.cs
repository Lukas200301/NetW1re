using SharpPcap;
using SharpPcap.LibPcap;
using System;

namespace NetW1reAvalonia.Core.Services
{
	public interface IPcapDeviceManager
	{
		/// <summary>
		/// The created device is automatically opened
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="packetArrivalHandler"></param>
		/// <returns></returns>
		public IPcapLiveDevice CreateDevice(
			string filter,
			PacketArrivalEventHandler? packetArrivalHandler,
			int readTimeout,
			string? adapterName = null);
	}
}
