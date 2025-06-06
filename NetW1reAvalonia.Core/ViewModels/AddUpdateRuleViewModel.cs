using NetW1reAvalonia.Core.Rules;
using NetW1reAvalonia.Core.ViewModels.InteractionViewModels;
using ReactiveUI;
using System.Collections.Generic;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;

namespace NetW1reAvalonia.Core.ViewModels
{
	public class AddUpdateRuleViewModel : ViewModelBase
	{
		private IEnumerable<string>? ruleActions;
		public IEnumerable<string>? RuleActions
		{
			get => ruleActions;
			set => this.RaiseAndSetIfChanged(ref ruleActions, value);
		}

		private IEnumerable<string>? ruleSourceValues;
		public IEnumerable<string>? RuleSourceValues
		{
			get => ruleSourceValues;
			set => this.RaiseAndSetIfChanged(ref ruleSourceValues, value);
		}

		private readonly ObservableAsPropertyHelper<bool> isLimitRule;
		public bool IsLimitRule => isLimitRule.Value;

		private AddUpdateRuleModel? _addUpdateRuleModel;
		public AddUpdateRuleModel? AddUpdateRuleModel
		{
			get => _addUpdateRuleModel;
			set => this.RaiseAndSetIfChanged(ref _addUpdateRuleModel, value);
		}

		private bool isUpdate;
		public bool IsUpdate
		{
			get => isUpdate;
			set => this.RaiseAndSetIfChanged(ref isUpdate, value);
		}

		private ObservableAsPropertyHelper<string> windowTitle;
		public string WindowTitle => windowTitle.Value;

		public ReactiveCommand<Unit, AddUpdateRuleModel?> Accept { get; set; }


#if DEBUG

		public AddUpdateRuleViewModel()
		{

		}

#endif

		public AddUpdateRuleViewModel(bool isUpdate = false)
		{
			AddUpdateRuleModel = new AddUpdateRuleModel();

			windowTitle = this.WhenAnyValue(x => x.IsUpdate)
				.Select(x => x == false ? "Add Rule" : "Update Rule")
				.ToProperty(this, x => x.WindowTitle);

			var canAcceptRule = this.WhenAnyValue(
				x => x.AddUpdateRuleModel!.Action,
				x => x.AddUpdateRuleModel!.SourceValue,
				x => x.AddUpdateRuleModel!.Target,
				x => x.AddUpdateRuleModel!.Upload,
				x => x.AddUpdateRuleModel!.Download,
				(action, source, target, upload, download) =>
				action != null && source != null && !string.IsNullOrWhiteSpace(target) && upload >= 0 && download >= 0);

			Accept = ReactiveCommand.Create(AcceptImpl, canAcceptRule);

			isLimitRule = this.WhenAnyValue(x => x.AddUpdateRuleModel!.Action)
					  .Select(x => x == RuleAction.Limit)
					  .ToProperty(this, x => x.IsLimitRule);

			RuleActions = Enum.GetNames(typeof(RuleAction));
			RuleSourceValues = Enum.GetNames(typeof(RuleSourceValue));
			IsUpdate = isUpdate;
		}

		public AddUpdateRuleModel? AcceptImpl()
		{
			return AddUpdateRuleModel;
		}
	}
}
