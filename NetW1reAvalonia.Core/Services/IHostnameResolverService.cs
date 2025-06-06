using System;
using System.Net;
using System.Threading.Tasks;

namespace NetW1reAvalonia.Core.Services
{
    public interface IHostnameResolverService
    {
        Task<string> ResolveHostnameAsync(IPAddress ipAddress);
        Task<string> ResolveHostnameAsync(string ipAddress);
        void ClearCache();
    }
}
