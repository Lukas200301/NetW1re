using NetW1reAvalonia.Core.Models;
using NetW1reAvalonia.Core.Services;
using NetW1reAvalonia.Core.ViewModels.DialogViewModels;
using NetW1reAvalonia.Core.Views.DialogViews;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace NetW1reAvalonia.Core.ViewModels.RoutedViewModels
{    public class SnifferViewModel : ViewModelBase, IRoutableViewModel
    {
        private readonly IPacketSnifferService _packetSnifferService;
        private readonly IHostnameResolverService _hostnameResolverService;
        private string[] _networkDevices = Array.Empty<string>();
        private string? _selectedDevice;        private string _statusMessage = "Ready to capture packets";
        private int _packetCount;
        private PacketInfo? _selectedPacket;
        private PacketDetailsViewModel? _packetDetailsViewModel;

        public string? UrlPathSegment { get; } = "Packet Sniffer";
        public IScreen HostScreen { get; }

        public ObservableCollection<PacketInfo> CapturedPackets => _packetSnifferService.CapturedPackets;

        public string[] NetworkDevices
        {
            get => _networkDevices;
            set => this.RaiseAndSetIfChanged(ref _networkDevices, value);
        }        public string? SelectedDevice
        {
            get => _selectedDevice;
            set => this.RaiseAndSetIfChanged(ref _selectedDevice, value);
        }

        public PacketInfo? SelectedPacket
        {
            get => _selectedPacket;
            set 
            { 
                this.RaiseAndSetIfChanged(ref _selectedPacket, value);
                UpdatePacketDetailsViewModel();
            }
        }

        public PacketDetailsViewModel? PacketDetailsViewModel
        {
            get => _packetDetailsViewModel;
            set => this.RaiseAndSetIfChanged(ref _packetDetailsViewModel, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }        public int PacketCount
        {
            get => _packetCount;
            set => this.RaiseAndSetIfChanged(ref _packetCount, value);
        }

        public bool IsCapturing => _packetSnifferService.IsCapturing;public ReactiveCommand<Unit, Unit> StartCaptureCommand { get; }
        public ReactiveCommand<Unit, Unit> StopCaptureCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearPacketsCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshDevicesCommand { get; }
        public ReactiveCommand<Unit, Unit> ShowPacketDetailsCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenDetailedPacketWindowCommand { get; }

        public event EventHandler<PacketInfo>? ShowPacketDetailsRequested;

        #region Constructors

#if DEBUG
        public SnifferViewModel()
        {
            _packetSnifferService = new Services.Implementations.PacketSnifferService();
            _hostnameResolverService = new Services.Implementations.HostnameResolverService();
            HostScreen = null!;
            
            StartCaptureCommand = ReactiveCommand.CreateFromTask(StartCapture,
                this.WhenAnyValue(x => x.SelectedDevice, x => x.IsCapturing,
                    (device, capturing) => !string.IsNullOrEmpty(device) && !capturing));

            StopCaptureCommand = ReactiveCommand.CreateFromTask(StopCapture,
                this.WhenAnyValue(x => x.IsCapturing));

            ClearPacketsCommand = ReactiveCommand.Create(ClearPackets);

            RefreshDevicesCommand = ReactiveCommand.CreateFromTask(LoadNetworkDevices);            ShowPacketDetailsCommand = ReactiveCommand.Create(ShowPacketDetails,
                this.WhenAnyValue(x => x.SelectedPacket).Select(packet => packet != null));

            OpenDetailedPacketWindowCommand = ReactiveCommand.Create(() => { });

            _ = Task.Run(LoadNetworkDevices);}
#endif

        [Splat.DependencyInjectionConstructor]
        public SnifferViewModel(IRouter screen, IPacketSnifferService packetSnifferService, IHostnameResolverService hostnameResolverService)
        {
            this.HostScreen = screen;
            _packetSnifferService = packetSnifferService;
            _hostnameResolverService = hostnameResolverService;

            StartCaptureCommand = ReactiveCommand.CreateFromTask(StartCapture,
                this.WhenAnyValue(x => x.SelectedDevice, x => x.IsCapturing,
                    (device, capturing) => !string.IsNullOrEmpty(device) && !capturing));

            StopCaptureCommand = ReactiveCommand.CreateFromTask(StopCapture,
                this.WhenAnyValue(x => x.IsCapturing));

            ClearPacketsCommand = ReactiveCommand.Create(ClearPackets);

            RefreshDevicesCommand = ReactiveCommand.CreateFromTask(LoadNetworkDevices);            ShowPacketDetailsCommand = ReactiveCommand.Create(ShowPacketDetails,
                this.WhenAnyValue(x => x.SelectedPacket).Select(packet => packet != null));

            OpenDetailedPacketWindowCommand = ReactiveCommand.CreateFromTask(OpenDetailedPacketWindow,
                this.WhenAnyValue(x => x.SelectedPacket).Select(packet => packet != null));

            _packetSnifferService.PacketCaptured += OnPacketCaptured;
            _ = Task.Run(LoadNetworkDevices);
        }

        #endregion

        private async Task LoadNetworkDevices()
        {
            try
            {
                StatusMessage = "Loading network devices...";
                var devices = await _packetSnifferService.GetNetworkDevicesAsync();
                NetworkDevices = devices;
                
                if (devices.Length > 0 && string.IsNullOrEmpty(SelectedDevice))
                {
                    SelectedDevice = devices.First();
                }

                StatusMessage = devices.Length > 0 ? "Ready to capture packets" : "No network devices found";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading devices: {ex.Message}";
            }
        }

        private async Task StartCapture()
        {
            if (string.IsNullOrEmpty(SelectedDevice))
                return;

            try
            {
                StatusMessage = "Starting packet capture...";
                await _packetSnifferService.StartCaptureAsync(SelectedDevice);
                StatusMessage = $"Capturing packets on {SelectedDevice}";
                this.RaisePropertyChanged(nameof(IsCapturing));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error starting capture: {ex.Message}";
            }
        }

        private async Task StopCapture()
        {
            try
            {
                StatusMessage = "Stopping packet capture...";
                await _packetSnifferService.StopCaptureAsync();
                StatusMessage = "Packet capture stopped";
                this.RaisePropertyChanged(nameof(IsCapturing));
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error stopping capture: {ex.Message}";
            }
        }

        private void ClearPackets()
        {
            _packetSnifferService.ClearPackets();
            PacketCount = 0;
            StatusMessage = "Packets cleared";
        }        private void OnPacketCaptured(object? sender, PacketInfo packet)
        {
            PacketCount = CapturedPackets.Count;
            
            _ = Task.Run(async () => await ResolvePacketHostnames(packet));
        }

        private async Task ResolvePacketHostnames(PacketInfo packet)
        {
            try
            {
                if (!string.IsNullOrEmpty(packet.Source) && packet.Source != packet.SourceHostname)
                {
                    var sourceHostname = await _hostnameResolverService.ResolveHostnameAsync(packet.Source);
                    if (!string.IsNullOrEmpty(sourceHostname) && sourceHostname != packet.Source)
                    {
                        packet.SourceHostname = sourceHostname;
                    }
                }

                if (!string.IsNullOrEmpty(packet.Destination) && packet.Destination != packet.DestinationHostname)
                {
                    var destinationHostname = await _hostnameResolverService.ResolveHostnameAsync(packet.Destination);
                    if (!string.IsNullOrEmpty(destinationHostname) && destinationHostname != packet.Destination)
                    {
                        packet.DestinationHostname = destinationHostname;
                    }
                }

                packet.HostnamesResolved = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hostname resolution failed: {ex.Message}");
            }
        }

        private async Task OpenDetailedPacketWindow()
        {
            if (SelectedPacket == null) return;

            try
            {
                if (!SelectedPacket.HostnamesResolved)
                {
                    await ResolvePacketHostnames(SelectedPacket);
                }

                var detailedViewModel = new DetailedPacketViewModel(SelectedPacket, _hostnameResolverService);                var window = new DetailedPacketWindow
                {
                    DataContext = detailedViewModel
                };               
                if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && 
                    desktop.MainWindow != null)
                {
                    await window.ShowDialog(desktop.MainWindow);
                }
                else
                {
                    window.Show();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error opening detailed packet window: {ex.Message}";
            }
        }

        private void ShowPacketDetails()
        {
            if (SelectedPacket != null)
            {
                ShowPacketDetailsRequested?.Invoke(this, SelectedPacket);
            }
        }

        private void UpdatePacketDetailsViewModel()
        {
            if (SelectedPacket != null)
            {
                PacketDetailsViewModel = new PacketDetailsViewModel(SelectedPacket);
            }
            else
            {
                PacketDetailsViewModel = null;
            }
        }
    }
}
