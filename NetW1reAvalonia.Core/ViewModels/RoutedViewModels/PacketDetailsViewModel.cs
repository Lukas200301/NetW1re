using NetW1reAvalonia.Core.Models;
using ReactiveUI;
using System.Reactive;
using System;
using System.Text;
using System.Linq;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using System.IO;
using Avalonia;
using Avalonia.Input.Platform;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;

namespace NetW1reAvalonia.Core.ViewModels.RoutedViewModels
{
    public class PacketDetailsViewModel : ViewModelBase
    {
        private PacketInfo _packetInfo;

        public PacketDetailsViewModel()
        {
            _packetInfo = new PacketInfo(); 
            InitializeCommands();
        }

        public PacketDetailsViewModel(PacketInfo packetInfo)
        {
            _packetInfo = packetInfo ?? throw new ArgumentNullException(nameof(packetInfo));
            InitializeCommands();
        }

        public PacketInfo PacketInfo
        {
            get => _packetInfo;
            set => this.RaiseAndSetIfChanged(ref _packetInfo, value);
        }

        public bool HasEthernetInfo => !string.IsNullOrEmpty(PacketInfo.SourceMAC) && !string.IsNullOrEmpty(PacketInfo.DestinationMAC);
        public bool HasEthernetType => !string.IsNullOrEmpty(PacketInfo.EthernetType);
        public bool HasIpInfo => PacketInfo.SourceIP != null && PacketInfo.DestinationIP != null;
        public bool HasIpChecksum => !string.IsNullOrEmpty(PacketInfo.IpChecksum);
        
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

        public string PayloadSizeText => $"Payload Size: {PacketInfo.PayloadData.Length} bytes";
        public string RawDataSizeText => $"Total Size: {PacketInfo.RawData.Length} bytes";
        
        public ReactiveCommand<Unit, Unit> CloseCommand { get; private set; } = null!;
        public ReactiveCommand<Unit, Unit> CopyPayloadCommand { get; private set; } = null!;
        public ReactiveCommand<Unit, Unit> CopyHexCommand { get; private set; } = null!;
        public ReactiveCommand<Unit, Unit> CopyHexDumpCommand { get; private set; } = null!;
        public ReactiveCommand<Unit, Unit> CopyPayloadTextCommand { get; private set; } = null!;
        public ReactiveCommand<Unit, Unit> CopyApplicationDataCommand { get; private set; } = null!;
        public ReactiveCommand<Unit, Unit> CopyPayloadPreviewCommand { get; private set; } = null!;
        public ReactiveCommand<Unit, Unit> SaveRawDataCommand { get; private set; } = null!;

        public event EventHandler? CloseRequested;        
        
        private void InitializeCommands()
        {
            CloseCommand = ReactiveCommand.Create(() => CloseRequested?.Invoke(this, EventArgs.Empty));
            CopyPayloadCommand = ReactiveCommand.CreateFromTask(CopyPayloadToClipboard);
            CopyHexCommand = ReactiveCommand.CreateFromTask(CopyHexToClipboard);
            CopyHexDumpCommand = ReactiveCommand.CreateFromTask(CopyHexToClipboard);
            CopyPayloadTextCommand = ReactiveCommand.CreateFromTask(CopyPayloadTextToClipboard);
            CopyApplicationDataCommand = ReactiveCommand.CreateFromTask(CopyApplicationDataToClipboard);
            CopyPayloadPreviewCommand = ReactiveCommand.CreateFromTask(CopyPayloadPreviewToClipboard);
            SaveRawDataCommand = ReactiveCommand.CreateFromTask(SaveRawDataToFile);
        }

        private async Task CopyPayloadToClipboard()
        {
            try
            {
                if (PacketInfo.PayloadData.Length == 0)
                    return;

                var text = Encoding.UTF8.GetString(PacketInfo.PayloadData.Where(b => b >= 32 && b <= 126).ToArray());
                
                if (string.IsNullOrEmpty(text))
                {
                    text = BitConverter.ToString(PacketInfo.PayloadData).Replace("-", " ");
                }

                var clipboard = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null)?.Clipboard;
                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying payload: {ex.Message}");
            }
        }

        private async Task CopyHexToClipboard()
        {
            try
            {
                var hexDump = PacketInfo.GetHexDump();
                var clipboard = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null)?.Clipboard;
                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(hexDump);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying hex data: {ex.Message}");
            }
        }

        private async Task CopyPayloadTextToClipboard()
        {
            try
            {
                if (string.IsNullOrEmpty(PacketInfo.PayloadText))
                    return;

                var clipboard = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null)?.Clipboard;
                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(PacketInfo.PayloadText);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying payload text: {ex.Message}");
            }
        }

        private async Task CopyApplicationDataToClipboard()
        {
            try
            {
                if (string.IsNullOrEmpty(PacketInfo.ApplicationData))
                    return;

                var clipboard = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null)?.Clipboard;                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(PacketInfo.ApplicationData);
                }
            }
            catch
            {
                Console.WriteLine("Error copying application data to clipboard.");
            }
        }

        private async Task CopyPayloadPreviewToClipboard()
        {
            try
            {
                if (string.IsNullOrEmpty(PacketInfo.PayloadPreview))
                    return;

                var clipboard = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null)?.Clipboard;                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(PacketInfo.PayloadPreview);
                }
            }
            catch
            {
            }
        }

        private async Task SaveRawDataToFile()
        {
            try
            {
                var topLevel = Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;

                if (topLevel?.StorageProvider != null)
                {
                    var fileName = $"packet_{PacketInfo.Timestamp:yyyyMMdd_HHmmss}_{PacketInfo.Protocol}.bin";
                    
                    var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                    {
                        Title = "Save Packet Data",
                        SuggestedFileName = fileName,
                        FileTypeChoices = new[]
                        {
                            new FilePickerFileType("Binary Files") { Patterns = new[] { "*.bin" } },
                            new FilePickerFileType("All Files") { Patterns = new[] { "*.*" } }
                        }
                    });

                    if (file != null)
                    {
                        await using var stream = await file.OpenWriteAsync();
                        await stream.WriteAsync(PacketInfo.RawData);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving raw data: {ex.Message}");
            }
        }
    }
}
