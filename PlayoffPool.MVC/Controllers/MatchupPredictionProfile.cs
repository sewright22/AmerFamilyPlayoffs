using AmerFamilyPlayoffs.Data;
using AutoMapper;
using PlayoffPool.MVC.Models;

namespace PlayoffPool.MVC.Controllers
{
    public class MatchupPredictionProfile : Profile
    {
        public MatchupPredictionProfile()
        {
            this.CreateMap<MatchupPrediction, PlayoffTeamViewModel>()
                .ForMember(x => x.Name, obj => obj.MapFrom(x => x.PredictedWinner.SeasonTeam.Team.Name));
        }
    }
}
