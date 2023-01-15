using AmerFamilyPlayoffs.Data;
using AutoMapper;
using PlayoffPool.MVC.Models;
using PlayoffPool.MVC.Models.Home;

namespace PlayoffPool.MVC.Mapping
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            this.CreateMap<Team, PlayoffTeamViewModel>()
                .ForMember(vm => vm.Name, opt => opt.MapFrom(t => $"{t.Location} {t.Name}"))
                .ForPath(x => x.Id, opt => opt.Ignore());

            this.CreateMap<Team, BracketSummaryModel>()
                .ForMember(x => x.PredictedWinner, opt => opt.MapFrom(t => $"{t.Location} {t.Name}"));
        }
    }
}
