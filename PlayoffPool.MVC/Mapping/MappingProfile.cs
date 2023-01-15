using AmerFamilyPlayoffs.Data;
using AutoMapper;
using PlayoffPool.MVC.Models;
using PlayoffPool.MVC.Models.Admin;
using PlayoffPool.MVC.Models.Bracket;
using PlayoffPool.MVC.Models.Home;

namespace PlayoffPool.MVC.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            this.CreateMap<BracketPrediction, BracketViewModel>();

            this.CreateMap<BracketPrediction, BracketSummaryModel>()
                .IncludeMembers(x => x.SuperBowl);

            this.CreateMap<MatchupPrediction, BracketSummaryModel>()
                .IncludeMembers(x => x.PredictedWinner);

            this.CreateMap<PlayoffRound, RoundViewModel>()
                .IncludeMembers(x => x.Round);

            this.CreateMap<Round, RoundViewModel>();

            this.CreateMap<BracketViewModel, BracketPrediction>();
            this.CreateMap<MatchupViewModel, MatchupPrediction>();
            this.CreateMap<MatchupPrediction, MatchupViewModel>();
            this.CreateMap<BracketSummaryModel, BracketPrediction>();
        }
    }
}
