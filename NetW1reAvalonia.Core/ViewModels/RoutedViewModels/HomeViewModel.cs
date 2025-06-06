using DynamicData;
using DynamicData.Binding;
using NetW1reAvalonia.Core.Core.Views.Components;
using NetW1reAvalonia.Core.Models;
using NetW1reAvalonia.Core.Services;
using NetW1reAvalonia.Core.ViewModels.InteractionViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace NetW1reAvalonia.Core.ViewModels.RoutedViewModels
{
	public class HomeViewModel : ViewModelBase, IRoutableViewModel, IDisposable
	{
		#region Members

		private bool _blockAllHandlerAttached;
		private bool _redirectAllHandlerAttached;

		#endregion

		#region Services

		private readonly IErrorHandler _errorHandler;
		private readonly IStatusMessageService _statusMessageService;
		private readonly IDeviceScanner _deviceScanner;
		private readonly IBlockerRedirector _blockerRedirector;
		private readonly IDeviceNameResolver _deviceNameResolver;

		#endregion

		#region Subscriptions

		private IDisposable? _blockAllFutureHandlerSubscription;
		private IDisposable? _redirectAllFutureHandlerSubscription;

		#endregion

		#region Routing

		public string? UrlPathSegment { get; } = "Device List";
		public IScreen? HostScreen { get; }

		#endregion

		#region Commands

		public ReactiveCommand<Unit, Unit>? Scan { get; }
		public ReactiveCommand<Unit, Unit>? Refresh { get; }
		public ReactiveCommand<bool, Unit>? BlockAll { get; }
		public ReactiveCommand<bool, Unit>? RedirectAll { get; }

		#endregion

		#region Context Menu Commands

		public ReactiveCommand<PhysicalAddress?, Unit> BlockUnblock { get; }
		public ReactiveCommand<PhysicalAddress?, Unit> RedirectUnRedirect { get; }
		public ReactiveCommand<PhysicalAddress?, Unit> Limit { get; }
		public ReactiveCommand<PhysicalAddress?, Unit> SetFriendlyName { get; }
		public ReactiveCommand<PhysicalAddress?, Unit> ClearFriendlyName { get; }

		#endregion

		#region UI Properties

		private Device? _selectedDevice;
		public Device? SelectedDevice
		{
			get => _selectedDevice;
			set => this.RaiseAndSetIfChanged(ref _selectedDevice, value);
		}

		private bool _allBlocked;
		public bool AllBlocked
		{
			get => _allBlocked;
			set => this.RaiseAndSetIfChanged(ref _allBlocked, value);
		}

		private bool _allRedirected;
		public bool AllRedirected
		{
			get => _allRedirected;
			set => this.RaiseAndSetIfChanged(ref _allRedirected, value);
		}

		private bool _scanEnabled;
		public bool ScanEnabled
		{
			get => _scanEnabled;
			set => this.RaiseAndSetIfChanged(ref _scanEnabled, value);
		}


		#endregion

		#region Devices List

		// Accessor to expose the UI device list
		public ReadOnlyObservableCollection<Device>? Devices => _blockerRedirector?.Devices;

		// Configure the device list view
		public DeviceListViewSettings DeviceListViewSettings { get; set; } = new();

		#endregion

		#region Interactions

		public Interaction<DeviceLimitsModel?, DeviceLimitsModel?> ShowLimitDialogInteraction { get; set; }
		public Interaction<string?, string?> SetFriendlyNameInteraction { get; set; }

		#endregion

#if DEBUG

		public HomeViewModel()
		{

		}

#endif

		[Splat.DependencyInjectionConstructor]
		public HomeViewModel(
			IErrorHandler errorHandler,
			IStatusMessageService statusMessageService,
			IDeviceScanner deviceScanner,
			IBlockerRedirector blockerRedirector,
			IDeviceNameResolver deviceNameResolver)
		{
			_errorHandler = errorHandler;
			_statusMessageService = statusMessageService;
			_deviceScanner = deviceScanner;
			_blockerRedirector = blockerRedirector;
			_deviceNameResolver = deviceNameResolver;

			ScanEnabled = true;

			Scan = ReactiveCommand.Create(ScanImpl);
			Refresh = ReactiveCommand.Create(RefreshImpl);
			BlockAll = ReactiveCommand.CreateFromTask<bool>(BlockAllImpl);
			RedirectAll = ReactiveCommand.CreateFromTask<bool>(RedirectAllImpl);

			BlockUnblock = ReactiveCommand.CreateFromTask<PhysicalAddress?>(BlockUnblockImpl);
			RedirectUnRedirect = ReactiveCommand.CreateFromTask<PhysicalAddress?>(RedirectUnRedirectImpl);

			ShowLimitDialogInteraction = new Interaction<DeviceLimitsModel?, DeviceLimitsModel?>();
			Limit = ReactiveCommand.CreateFromTask<PhysicalAddress?>(LimitImpl);

			SetFriendlyNameInteraction = new Interaction<string?, string?>();
			SetFriendlyName = ReactiveCommand.CreateFromTask<PhysicalAddress?>(SetFriendlyNameImpl);
			ClearFriendlyName = ReactiveCommand.CreateFromTask<PhysicalAddress?>(ClearFriendlyNameImpl);

			Scan.ThrownExceptions.Subscribe(x =>
				_errorHandler.HandleError(new StatusMessageModel(MessageType.Error, x.Message)));
			Refresh.ThrownExceptions.Subscribe(x =>
				_errorHandler.HandleError(new StatusMessageModel(MessageType.Error, x.Message)));
			BlockUnblock.ThrownExceptions.Subscribe(x =>
				_errorHandler.HandleError(new StatusMessageModel(MessageType.Error, x.Message)));
			RedirectUnRedirect.ThrownExceptions.Subscribe(x =>
				_errorHandler.HandleError(new StatusMessageModel(MessageType.Error, x.Message)));
			BlockAll.ThrownExceptions.Subscribe(x =>
				_errorHandler.HandleError(new StatusMessageModel(MessageType.Error, x.Message)));
			RedirectAll.ThrownExceptions.Subscribe(x =>
				_errorHandler.HandleError(new StatusMessageModel(MessageType.Error, x.Message)));
		}

		#region Command Handlers

		private void ScanImpl()
		{
			_deviceScanner?.Scan();

			ScanEnabled = false;
		}

		private void RefreshImpl()
		{
			Task.Run(() => _deviceScanner?.Refresh());
		}

		private async Task BlockUnblockImpl(PhysicalAddress? mac)
		{
			(bool isValid, Device device) = await CheckIfMacAddressIsValidAsync(mac);

			if (isValid == false)
				return;

			if (device.Blocked == false)
			{
				_blockerRedirector?
					.Block(device);
			}
			else
			{
				_blockerRedirector?
					.UnBlock(device);
			}
		}

		private async Task RedirectUnRedirectImpl(PhysicalAddress? mac)
		{
			(bool isValid, Device device) = await CheckIfMacAddressIsValidAsync(mac);

			if (isValid == false)
				return;

			if (device.Redirected == false)
			{
				_blockerRedirector?.Redirect(device);
			}
			else
			{
				_blockerRedirector?.UnRedirect(device);
			}
		}

		private async Task LimitImpl(PhysicalAddress? mac)
		{
			(bool isValid, Device device) = await CheckIfMacAddressIsValidAsync(mac);

			if (isValid == false)
				return;

			var result =
				await ShowLimitDialogInteraction!.Handle(new DeviceLimitsModel(device.DownloadCap / 1024,
					device.UploadCap / 1024));

			if (result != null)
			{
				_blockerRedirector?.Limit(device!, result.Download, result.Upload);
			}
		}

		private async Task BlockAllImpl(bool active)
		{
			if (AllRedirected && active)
			{
				AllBlocked = false;

				await _statusMessageService.ShowMessage(
						new StatusMessageModel(MessageType.Error,
							"You have to uncheck the Redirect All toggle first!"));
				return;
			}

			if (active)
			{
				var devices = Devices!
					.Where(d => d.IsGateway() == false && d.IsLocalDevice() == false & d.Blocked == false)
					.ToList();

				foreach (var device in devices)
				{
					_blockerRedirector?.Block(device);
				}

				if (_blockAllHandlerAttached == false)
				{
					_blockAllHandlerAttached = true;
					AttachBlockAllFutureDetectionsHandler();
				}
			}
			else
			{
				var devices = Devices!
					.Where(d => d.IsGateway() == false && d.IsLocalDevice() == false & d.Blocked == true)
					.ToList();

				foreach (var device in devices)
				{
					_blockerRedirector?.UnBlock(device);
				}

				if (_blockAllHandlerAttached == true)
				{
					_blockAllHandlerAttached = false;
					RemoveBlockAllFutureDetectionsHandler();
				}
			}

			AllBlocked = active;
		}

		private async Task RedirectAllImpl(bool active)
		{
			if (AllBlocked && active)
			{
				AllRedirected = false;

				await _statusMessageService.ShowMessage(
						new StatusMessageModel(MessageType.Error,
							"You have to uncheck the Block All toggle first!"));
				return;
			}

			if (active)
			{
				var devices = Devices!
					.Where(d => d.IsGateway() == false && d.IsLocalDevice() == false && d.Redirected == false)
					.ToList();

				foreach (var device in devices)
				{
					_blockerRedirector?.Redirect(device);
				}

				if (_redirectAllHandlerAttached == false)
				{
					_redirectAllHandlerAttached = true;
					AttachRedirectAllFutureDetectionsHandler();
				}
			}
			else
			{
				var devices = Devices!
					.Where(d => d.IsGateway() == false && d.IsLocalDevice() == false && d.Redirected == true)
					.ToList();

				foreach (var device in devices)
				{
					_blockerRedirector?.UnRedirect(device);
				}

				if (_redirectAllHandlerAttached == true)
				{
					_redirectAllHandlerAttached = false;
					RemoveRedirectAllFutureDetectionsHandler();
				}
			}

			AllRedirected = active;
		}

		private async Task SetFriendlyNameImpl(PhysicalAddress? mac)
		{
			(bool isValid, Device device) = await CheckIfMacAddressIsValidAsync(mac, true);

			if (isValid == false)
				return;

			var result =
				await SetFriendlyNameInteraction!.Handle(device.Name);

			if (string.IsNullOrWhiteSpace(result) == false)
			{
				device.SetFriendlyName(result);
				_deviceNameResolver?.SaveDeviceNamesAsync(Devices!.ToList());
			}
		}

		private async Task ClearFriendlyNameImpl(PhysicalAddress? mac)
		{
			(bool isValid, Device device) = await CheckIfMacAddressIsValidAsync(mac, true);

			if (isValid == false)
				return;

			device.SetFriendlyName(null!);

			_deviceNameResolver?.SaveDeviceNamesAsync(Devices!.ToList());
		}


		#endregion

		#region Tools

		private async Task<(bool isValid, Device device)> CheckIfMacAddressIsValidAsync(PhysicalAddress? mac,
			bool canBeAppliedToGatewayAndLocal = false)
		{
			var device = Devices.FirstOrDefault(x => x.Mac!.Equals(mac));

			if (device == null)
			{
				await _statusMessageService.ShowMessage(new StatusMessageModel(MessageType.Error,
				"No device is selected!"));

				return (false, null!);
			}
			else if (device!.IsGateway() && canBeAppliedToGatewayAndLocal == false)
			{
				await _statusMessageService.ShowMessage(new StatusMessageModel(MessageType.Error,
				"Gateway can't be targeted!"));

				return (false, null!);
			}
			else if (device!.IsLocalDevice() && canBeAppliedToGatewayAndLocal == false)
			{
				await _statusMessageService.ShowMessage(new StatusMessageModel(MessageType.Error,
					"You can't target your own device!"));

				return (false, null!);
			}

			return (true, device);
		}

		public IEnumerable<Device> GetUiDeviceCollection() => Devices;

		private void AttachBlockAllFutureDetectionsHandler()
		{
			_blockAllFutureHandlerSubscription =
				Devices
					.ToObservableChangeSet()
					.Where(change => change.Adds > 0)
					.ToCollection()
					.Select(x => BlockAllImpl(true))
					.Subscribe();
		}

		private void RemoveBlockAllFutureDetectionsHandler()
		{
			_blockAllFutureHandlerSubscription?.Dispose();
		}

		private void AttachRedirectAllFutureDetectionsHandler()
		{
			_redirectAllFutureHandlerSubscription =
				Devices
					.ToObservableChangeSet()
					.Where(change => change.Adds > 0)
					.ToCollection()
					.Select(x => RedirectAllImpl(true))
					.Subscribe();
		}

		private void RemoveRedirectAllFutureDetectionsHandler()
		{
			_redirectAllFutureHandlerSubscription?.Dispose();
		}

		#endregion

		public void Dispose()
		{
			RemoveBlockAllFutureDetectionsHandler();
			RemoveRedirectAllFutureDetectionsHandler();
		}
	}
}
