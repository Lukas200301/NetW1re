using NetW1reAvalonia.Core.Models;
using NetW1reAvalonia.Core.Rules;
using NetW1reAvalonia.Core.Rules.Implementations;
using NetW1reAvalonia.Core.Services.Implementations.RulesService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetW1reAvalonia.Core.Services
{
	public interface IRuleService : IService
	{
		/// <summary>
		/// Apply the rules that match the provided device.
		/// </summary>
		/// <remarks>
		/// If multiple rules match the same device, the last one in terms of order is applied.
		/// </remarks>
		/// <param name="device"></param>
		public void ApplyIfMatch(Device device);
		public bool TryAddBlockingRule(BlockRule blockRule);
		public bool TryAddRedirectingRule(RedirectRule redirectRule);
		public bool TryAddLimitingRule(LimitRule limitRule);
		public bool TryUpdateRule(RuleBase rule);
		public bool TryRemoveRule(RuleBase rule);
		public void MoveRuleUp(RuleBase rule);
		public void MoveRuleDown(RuleBase rule);
		public void SaveRules();
		public ReadOnlyObservableCollection<RuleBase> Rules { get; }
	}
}
