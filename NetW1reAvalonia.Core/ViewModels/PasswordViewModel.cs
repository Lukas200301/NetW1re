using Avalonia.Controls;
using NetW1reAvalonia.Core.Helpers;
using NetW1reAvalonia.Core.Helpers;
using NetW1reAvalonia.Core.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace NetW1reAvalonia.Core.ViewModels
{
	public class PasswordViewModel : ViewModelBase
	{
		private readonly IAppLockService _appLockService;

#if DEBUG
		public PasswordViewModel()
		{

		}
#endif

		[Splat.DependencyInjectionConstructor]
		public PasswordViewModel(IAppLockService appLockService)
		{
			this._appLockService = appLockService;

			Submit = ReactiveCommand.Create(() =>
			{
				var result = _appLockService!.Unlock(Password!);

				if (result)
				{
					_ = this.CloseWindow?.Execute().Subscribe();
				}
				else
				{
					ErrorMessage = "Invalid password";
					ShowError = true;
				}
			});

			Exit = ReactiveCommand.Create(LifeTimeHelpers.ExitApp);
		}

		public ReactiveCommand<Unit, Unit> Submit { get; set; }
		public ReactiveCommand<Unit, Unit> Exit { get; set; }
		public ReactiveCommand<Unit, Unit> CloseWindow { get; set; }

		private string? _password;

		public string? Password
		{
			get => _password;
			set => this.RaiseAndSetIfChanged(ref _password, value);
		}

		private bool _showError;

		public bool ShowError
		{
			get => _showError;
			set => this.RaiseAndSetIfChanged(ref _showError, value);
		}

		private string? _errorMessage;

		public string? ErrorMessage
		{
			get => _errorMessage;
			set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
		}
	}
}
