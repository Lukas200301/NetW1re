using System;

namespace NetW1reAvalonia.Core.Services;

public interface IDeviceScanner : IService, IDisposable
{
    void Scan();
    void Refresh();
    void Stop();
}
