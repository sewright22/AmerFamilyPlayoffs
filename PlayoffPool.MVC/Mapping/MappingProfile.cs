using AmerFamilyPlayoffs.Data;
using AutoMapper;
using PlayoffPool.MVC.Models;
using PlayoffPool.MVC.Models.Bracket;
using PlayoffPool.MVC.Models.Home;

namespace PlayoffPool.MVC.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<RegisterViewModel, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));

            this.CreateMap<LoginViewModel, User>()
                .ForMember(u => u.Email, opt => opt.MapFrom(x => x.Email));

            this.CreateMap<Team, TeamViewModel>()
                .ForMember(vm => vm.Name, opt => opt.MapFrom(t => $"{t.Location} {t.Name}"))
                .ForPath(x => x.Id, opt => opt.Ignore());

            this.CreateMap<PlayoffTeam, TeamViewModel>()
                .IncludeMembers(x => x.SeasonTeam)
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id));

            this.CreateMap<SeasonTeam, TeamViewModel>()
                .IncludeMembers(x => x.Team)
                .ForMember(x => x.Id, opt => opt.Ignore());

            this.CreateMap<BracketPrediction, BracketViewModel>();
            this.CreateMap<BracketPrediction, BracketSummaryModel>()
                .IncludeMembers(x => x.SuperBowl);

            this.CreateMap<MatchupPrediction, BracketSummaryModel>()
                .IncludeMembers(x => x.PredictedWinner);

            this.CreateMap<PlayoffTeam, BracketSummaryModel>()
                .IncludeMembers(x => x.SeasonTeam);

            this.CreateMap<SeasonTeam, BracketSummaryModel>()
                .IncludeMembers(x => x.Team);

            this.CreateMap<Team, BracketSummaryModel>()
                .ForMember(x => x.PredictedWinner, opt => opt.MapFrom(t => $"{t.Location} {t.Name}"));

            this.CreateMap<PlayoffRound, RoundViewModel>()
                .IncludeMembers(x => x.Round);

            this.CreateMap<Round, RoundViewModel>();

            this.CreateMap<BracketViewModel, BracketPrediction>();
            this.CreateMap<MatchupViewModel, MatchupPrediction>();
            this.CreateMap<BracketSummaryModel, BracketPrediction>();
        }
    }
}
