using AutoMapper;
using NetW1reAvalonia.Core.Rules;
using NetW1reAvalonia.Core.Rules.Implementations;
using NetW1reAvalonia.Core.ViewModels.InteractionViewModels;

namespace NetW1reAvalonia.Core.Helpers
{
	public class AutoMapperHelpers
	{
		public static Mapper BuildAutoMapper()
		{
			return new Mapper(new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<RuleBase, AddUpdateRuleModel>().ReverseMap();

				cfg.CreateMap<BlockRule, AddUpdateRuleModel>().ReverseMap();

				cfg.CreateMap<RedirectRule, AddUpdateRuleModel>().ReverseMap();

				cfg.CreateMap<LimitRule, AddUpdateRuleModel>().ReverseMap();

				cfg.CreateMap<BlockRule, RuleBase>().ReverseMap();

				cfg.CreateMap<RedirectRule, RuleBase>().ReverseMap();

				cfg.CreateMap<LimitRule, RuleBase>().ReverseMap();

				cfg.CreateMap<LimitRule, LimitRule>().ReverseMap();
			}));
		}
	}
}
