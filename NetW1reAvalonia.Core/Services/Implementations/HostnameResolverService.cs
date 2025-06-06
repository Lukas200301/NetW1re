using NetW1reAvalonia.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace NetW1reAvalonia.Core.Services.Implementations
{
    public class HostnameResolverService : IHostnameResolverService
    {
        private readonly ConcurrentDictionary<string, string> _hostnameCache = new();
        private readonly TimeSpan _cacheTimeout = TimeSpan.FromMinutes(30);
        private readonly ConcurrentDictionary<string, DateTime> _cacheTimestamps = new();

        public async Task<string> ResolveHostnameAsync(IPAddress ipAddress)
        {
            if (ipAddress == null)
                return string.Empty;

            return await ResolveHostnameAsync(ipAddress.ToString());
        }

        public async Task<string> ResolveHostnameAsync(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return string.Empty;

            // Check cache first
            if (_hostnameCache.TryGetValue(ipAddress, out var cachedHostname) &&
                _cacheTimestamps.TryGetValue(ipAddress, out var timestamp) &&
                DateTime.Now - timestamp < _cacheTimeout)
            {
                return cachedHostname;
            }

            try
            {
                // Perform reverse DNS lookup
                var hostEntry = await Dns.GetHostEntryAsync(ipAddress);
                var hostname = hostEntry.HostName;

                // Update cache
                _hostnameCache[ipAddress] = hostname;
                _cacheTimestamps[ipAddress] = DateTime.Now;

                return hostname;
            }
            catch
            {
                // If resolution fails, cache the IP address itself
                _hostnameCache[ipAddress] = ipAddress;
                _cacheTimestamps[ipAddress] = DateTime.Now;
                return ipAddress;
            }
        }

        public void ClearCache()
        {
            _hostnameCache.Clear();
            _cacheTimestamps.Clear();
        }
    }
}
