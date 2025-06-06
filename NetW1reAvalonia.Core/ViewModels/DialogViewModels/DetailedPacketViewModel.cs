using NetW1reAvalonia.Core.Models;
using NetW1reAvalonia.Core.Services;
using ReactiveUI;
using System;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Avalonia.Platform.Storage;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;

namespace NetW1reAvalonia.Core.ViewModels.DialogViewModels
{
    public class DetailedPacketViewModel : ViewModelBase
    {
        private readonly IHostnameResolverService _hostnameResolver;
        private PacketInfo _packetInfo;
        private string _windowTitle;
        private bool _isResolvingHostnames;

        public DetailedPacketViewModel()
        {
            _packetInfo = new PacketInfo
            {
                Protocol = "TCP",
                SourceIP = System.Net.IPAddress.Parse("192.168.1.100"),
                DestinationIP = System.Net.IPAddress.Parse("8.8.8.8"),
                SourcePort = 12345,
                DestinationPort = 443,
                Length = 1500,
                Info = "HTTPS Traffic"
            };
            _windowTitle = "Packet Details - TCP";
            _hostnameResolver = new Services.Implementations.HostnameResolverService();
            InitializeCommands();
        }

        public DetailedPacketViewModel(PacketInfo packetInfo, IHostnameResolverService hostnameResolver)
        {
            _packetInfo = packetInfo ?? throw new ArgumentNullException(nameof(packetInfo));
            _hostnameResolver = hostnameResolver ?? throw new ArgumentNullException(nameof(hostnameResolver));
            _windowTitle = $"Packet Details - {packetInfo.Protocol}";
            
            InitializeCommands();
            _ = Task.Run(ResolveHostnamesAsync);
        }

        public PacketInfo PacketInfo
        {
            get => _packetInfo;
            set => this.RaiseAndSetIfChanged(ref _packetInfo, value);
        }

        public string WindowTitle
        {
            get => _windowTitle;
            set => this.RaiseAndSetIfChanged(ref _windowTitle, value);
        }

        public bool IsResolvingHostnames
        {
            get => _isResolvingHostnames;
            set => this.RaiseAndSetIfChanged(ref _isResolvingHostnames, value);
        }

        public bool HasEthernetInfo => !string.IsNullOrEmpty(PacketInfo.SourceMAC) && !string.IsNullOrEmpty(PacketInfo.DestinationMAC);
        public bool HasEthernetType => !string.IsNullOrEmpty(PacketInfo.EthernetType);
        public bool HasIpInfo => PacketInfo.SourceIP != null && PacketInfo.DestinationIP != null;
        public bool HasIpChecksum => !string.IsNullOrEmpty(PacketInfo.IpChecksum);
        public bool HasHostnames => !string.IsNullOrEmpty(PacketInfo.SourceHostname) || !string.IsNullOrEmpty(PacketInfo.DestinationHostname);
        
        public bool IsTcp => string.Equals(PacketInfo.Protocol, "TCP", StringComparison.OrdinalIgnoreCase);
        public bool IsUdp => string.Equals(PacketInfo.Protocol, "UDP", StringComparison.OrdinalIgnoreCase);
        public bool IsIcmp => string.Equals(PacketInfo.Protocol, "ICMP", StringComparison.OrdinalIgnoreCase);
        public bool IsArp => string.Equals(PacketInfo.Protocol, "ARP", StringComparison.OrdinalIgnoreCase);
        
        public bool HasTcpFlags => !string.IsNullOrEmpty(PacketInfo.TcpFlags);
        public bool HasTcpChecksum => !string.IsNullOrEmpty(PacketInfo.TcpChecksum);
        public bool HasUdpChecksum => !string.IsNullOrEmpty(PacketInfo.UdpChecksum);
        public bool HasIcmpType => !string.IsNullOrEmpty(PacketInfo.IcmpType);
        public bool HasIcmpCode => !string.IsNullOrEmpty(PacketInfo.IcmpCode);
        
        public bool HasHttpData => !string.IsNullOrEmpty(PacketInfo.HttpMethod) || !string.IsNullOrEmpty(PacketInfo.HttpHost) || !string.IsNullOrEmpty(PacketInfo.HttpUserAgent);
        public bool HasDnsData => !string.IsNullOrEmpty(PacketInfo.DnsQuery) || !string.IsNullOrEmpty(PacketInfo.DnsResponse);
        public bool HasTlsData => !string.IsNullOrEmpty(PacketInfo.TlsSni) || !string.IsNullOrEmpty(PacketInfo.TlsVersion);
        public bool HasApplicationData => !string.IsNullOrEmpty(PacketInfo.ApplicationData);
        public bool HasPayloadText => !string.IsNullOrEmpty(PacketInfo.PayloadText);
        public bool HasPayloadPreview => !string.IsNullOrEmpty(PacketInfo.PayloadPreview);
        public bool HasArpData => !string.IsNullOrEmpty(PacketInfo.ArpOperation);

        public string PayloadSizeText => $"Payload Size: {PacketInfo.PayloadData.Length} bytes";
        public string RawDataSizeText => $"Total Size: {PacketInfo.RawData.Length} bytes";
        public string SourceDisplay => string.IsNullOrEmpty(PacketInfo.SourceHostname) ? 
            PacketInfo.SourceIP?.ToString() ?? "Unknown" : 
            $"{PacketInfo.SourceHostname} ({PacketInfo.SourceIP})";
        public string DestinationDisplay => string.IsNullOrEmpty(PacketInfo.DestinationHostname) ? 
            PacketInfo.DestinationIP?.ToString() ?? "Unknown" : 
            $"{PacketInfo.DestinationHostname} ({PacketInfo.DestinationIP})";

        public ReactiveCommand<Unit, Unit> CloseCommand { get; private set; } = null!;
        public ReactiveCommand<Unit, Unit> CopyAllDetailsCommand { get; private set; } = null!;
        public ReactiveCommand<Unit, Unit> CopyPayloadCommand { get; private set; } = null!;
        public ReactiveCommand<Unit, Unit> CopyHexDumpCommand { get; private set; } = null!;
        public ReactiveCommand<Unit, Unit> SaveRawDataCommand { get; private set; } = null!;
        public ReactiveCommand<Unit, Unit> RefreshHostnamesCommand { get; private set; } = null!;

        public event EventHandler? CloseRequested;

        private void InitializeCommands()
        {
            CloseCommand = ReactiveCommand.Create(() => CloseRequested?.Invoke(this, EventArgs.Empty));
            CopyAllDetailsCommand = ReactiveCommand.CreateFromTask(CopyAllDetailsToClipboard);
            CopyPayloadCommand = ReactiveCommand.CreateFromTask(CopyPayloadToClipboard);
            CopyHexDumpCommand = ReactiveCommand.CreateFromTask(CopyHexDumpToClipboard);
            SaveRawDataCommand = ReactiveCommand.CreateFromTask(SaveRawDataToFile);
            RefreshHostnamesCommand = ReactiveCommand.CreateFromTask(ResolveHostnamesAsync);
        }

        private async Task ResolveHostnamesAsync()
        {
            if (PacketInfo.HostnamesResolved)
                return;

            IsResolvingHostnames = true;

            try
            {
                var tasks = new Task[2];
                
                if (PacketInfo.SourceIP != null)
                {
                    tasks[0] = _hostnameResolver.ResolveHostnameAsync(PacketInfo.SourceIP)
                        .ContinueWith(t => PacketInfo.SourceHostname = t.Result);
                }
                else
                {
                    tasks[0] = Task.CompletedTask;
                }

                if (PacketInfo.DestinationIP != null)
                {
                    tasks[1] = _hostnameResolver.ResolveHostnameAsync(PacketInfo.DestinationIP)
                        .ContinueWith(t => PacketInfo.DestinationHostname = t.Result);
                }
                else
                {
                    tasks[1] = Task.CompletedTask;
                }

                await Task.WhenAll(tasks);
                PacketInfo.HostnamesResolved = true;

                this.RaisePropertyChanged(nameof(HasHostnames));
                this.RaisePropertyChanged(nameof(SourceDisplay));
                this.RaisePropertyChanged(nameof(DestinationDisplay));
            }
            finally
            {
                IsResolvingHostnames = false;
            }
        }

        private async Task CopyAllDetailsToClipboard()
        {
            try
            {
                var details = GenerateDetailedPacketInfo();
                var clipboard = await GetClipboardAsync();
                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(details);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to copy to clipboard: {ex.Message}");
            }
        }

        private async Task CopyPayloadToClipboard()
        {
            try
            {
                if (PacketInfo.PayloadData.Length == 0)
                    return;

                var text = Encoding.UTF8.GetString(PacketInfo.PayloadData.Where(b => b >= 32 && b <= 126).ToArray());
                var clipboard = await GetClipboardAsync();
                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(text);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to copy payload: {ex.Message}");
            }
        }

        private async Task CopyHexDumpToClipboard()
        {
            try
            {
                var hexDump = PacketInfo.GetHexDump();
                var clipboard = await GetClipboardAsync();
                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(hexDump);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to copy hex dump: {ex.Message}");
            }
        }

        private async Task SaveRawDataToFile()
        {
            try
            {
                if (PacketInfo.RawData.Length == 0)
                    return;

                var storageProvider = GetStorageProvider();
                if (storageProvider == null)
                    return;

                var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save Raw Packet Data",
                    DefaultExtension = "bin",
                    SuggestedFileName = $"packet_{PacketInfo.Timestamp:yyyyMMdd_HHmmss}.bin"
                });

                if (file != null)
                {
                    using var stream = await file.OpenWriteAsync();
                    await stream.WriteAsync(PacketInfo.RawData);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save raw data: {ex.Message}");
            }
        }

        private string GenerateDetailedPacketInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine($"                    PACKET DETAILS - {PacketInfo.Protocol}");
            sb.AppendLine("═══════════════════════════════════════════════════════════════");
            sb.AppendLine();

            sb.AppendLine("📋 BASIC INFORMATION");
            sb.AppendLine("───────────────────────────────────────────────────────────────");
            sb.AppendLine($"Timestamp:      {PacketInfo.Timestamp:yyyy-MM-dd HH:mm:ss.fff}");
            sb.AppendLine($"Protocol:       {PacketInfo.Protocol}");
            sb.AppendLine($"Total Length:   {PacketInfo.Length} bytes");
            sb.AppendLine($"Direction:      {PacketInfo.Direction}");
            if (!string.IsNullOrEmpty(PacketInfo.Info))
                sb.AppendLine($"Info:           {PacketInfo.Info}");
            sb.AppendLine();

            if (HasIpInfo)
            {
                sb.AppendLine("🌐 NETWORK LAYER (IP)");
                sb.AppendLine("───────────────────────────────────────────────────────────────");
                sb.AppendLine($"Source IP:      {PacketInfo.SourceIP}");
                if (!string.IsNullOrEmpty(PacketInfo.SourceHostname))
                    sb.AppendLine($"Source Host:    {PacketInfo.SourceHostname}");
                sb.AppendLine($"Dest IP:        {PacketInfo.DestinationIP}");
                if (!string.IsNullOrEmpty(PacketInfo.DestinationHostname))
                    sb.AppendLine($"Dest Host:      {PacketInfo.DestinationHostname}");
                
                if (PacketInfo.IpVersion.HasValue)
                    sb.AppendLine($"IP Version:     {PacketInfo.IpVersion}");
                if (PacketInfo.Ttl.HasValue)
                    sb.AppendLine($"TTL:            {PacketInfo.Ttl}");
                if (!string.IsNullOrEmpty(PacketInfo.IpChecksum))
                    sb.AppendLine($"Checksum:       {PacketInfo.IpChecksum}");
                sb.AppendLine();
            }

            if (IsTcp || IsUdp)
            {
                sb.AppendLine($"🚀 TRANSPORT LAYER ({PacketInfo.Protocol})");
                sb.AppendLine("───────────────────────────────────────────────────────────────");
                if (PacketInfo.SourcePort.HasValue)
                    sb.AppendLine($"Source Port:    {PacketInfo.SourcePort}");
                if (PacketInfo.DestinationPort.HasValue)
                    sb.AppendLine($"Dest Port:      {PacketInfo.DestinationPort}");

                if (IsTcp)
                {
                    if (PacketInfo.TcpSequenceNumber.HasValue)
                        sb.AppendLine($"Seq Number:     {PacketInfo.TcpSequenceNumber}");
                    if (PacketInfo.TcpAcknowledgmentNumber.HasValue)
                        sb.AppendLine($"Ack Number:     {PacketInfo.TcpAcknowledgmentNumber}");
                    if (!string.IsNullOrEmpty(PacketInfo.TcpFlags))
                        sb.AppendLine($"TCP Flags:      {PacketInfo.TcpFlags}");
                    if (PacketInfo.TcpWindowSize.HasValue)
                        sb.AppendLine($"Window Size:    {PacketInfo.TcpWindowSize}");
                    if (!string.IsNullOrEmpty(PacketInfo.TcpChecksum))
                        sb.AppendLine($"Checksum:       {PacketInfo.TcpChecksum}");
                }
                else if (IsUdp)
                {
                    if (PacketInfo.UdpLength.HasValue)
                        sb.AppendLine($"UDP Length:     {PacketInfo.UdpLength}");
                    if (!string.IsNullOrEmpty(PacketInfo.UdpChecksum))
                        sb.AppendLine($"Checksum:       {PacketInfo.UdpChecksum}");
                }
                sb.AppendLine();
            }

            if (HasHttpData || HasDnsData || HasTlsData)
            {
                sb.AppendLine("🌍 APPLICATION LAYER");
                sb.AppendLine("───────────────────────────────────────────────────────────────");

                if (HasHttpData)
                {
                    if (!string.IsNullOrEmpty(PacketInfo.HttpMethod))
                        sb.AppendLine($"HTTP Method:    {PacketInfo.HttpMethod}");
                    if (!string.IsNullOrEmpty(PacketInfo.HttpHost))
                        sb.AppendLine($"HTTP Host:      {PacketInfo.HttpHost}");
                    if (!string.IsNullOrEmpty(PacketInfo.HttpUserAgent))
                        sb.AppendLine($"User Agent:     {PacketInfo.HttpUserAgent}");
                    if (!string.IsNullOrEmpty(PacketInfo.HttpContentType))
                        sb.AppendLine($"Content Type:   {PacketInfo.HttpContentType}");
                }

                if (HasDnsData)
                {
                    if (!string.IsNullOrEmpty(PacketInfo.DnsQuery))
                        sb.AppendLine($"DNS Query:      {PacketInfo.DnsQuery}");
                    if (!string.IsNullOrEmpty(PacketInfo.DnsQueryType))
                        sb.AppendLine($"Query Type:     {PacketInfo.DnsQueryType}");
                    if (!string.IsNullOrEmpty(PacketInfo.DnsResponse))
                        sb.AppendLine($"DNS Response:   {PacketInfo.DnsResponse}");
                }

                if (HasTlsData)
                {
                    if (!string.IsNullOrEmpty(PacketInfo.TlsVersion))
                        sb.AppendLine($"TLS Version:    {PacketInfo.TlsVersion}");
                    if (!string.IsNullOrEmpty(PacketInfo.TlsSni))
                        sb.AppendLine($"TLS SNI:        {PacketInfo.TlsSni}");
                }

                sb.AppendLine();
            }

            // Payload Information
            if (HasPayloadText || HasApplicationData)
            {
                sb.AppendLine("📦 PAYLOAD DATA");
                sb.AppendLine("───────────────────────────────────────────────────────────────");
                sb.AppendLine($"Payload Size:   {PacketInfo.PayloadData.Length} bytes");
                
                if (HasApplicationData)
                {
                    sb.AppendLine("Application Data:");
                    sb.AppendLine(PacketInfo.ApplicationData);
                }
                else if (HasPayloadText)
                {
                    sb.AppendLine("Payload Text:");
                    sb.AppendLine(PacketInfo.PayloadText);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private async Task<IClipboard?> GetClipboardAsync()
        {
            if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow?.Clipboard;
            }
            return null;
        }

        private IStorageProvider? GetStorageProvider()
        {
            if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow?.StorageProvider;
            }
            return null;
        }
    }
}
