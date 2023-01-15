using AmerFamilyPlayoffs.Data;
using AutoMapper;
using PlayoffPool.MVC.Models;
using PlayoffPool.MVC.Models.Bracket;
using PlayoffPool.MVC.Models.Home;

namespace PlayoffPool.MVC.Mapping
{
    public class BracketPredictionProfile : Profile
    {
        public BracketPredictionProfile()
        {
            this.CreateMap<BracketPrediction, BracketViewModel>();

            this.CreateMap<BracketPrediction, BracketSummaryModel>()
                .IncludeMembers(x => x.SuperBowl)
                .ForMember(x => x.PredictedWinner, obj => obj.MapFrom(x => x.MatchupPredictions.FirstOrDefault(mp => mp.PlayoffRound.Round.Number == 4)));

            this.CreateMap<BracketViewModel, BracketPrediction>();
            this.CreateMap<BracketSummaryModel, BracketPrediction>();
        }
    }
}
