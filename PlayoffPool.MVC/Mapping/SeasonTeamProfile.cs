using AmerFamilyPlayoffs.Data;
using AutoMapper;
using PlayoffPool.MVC.Models;
using PlayoffPool.MVC.Models.Home;

namespace PlayoffPool.MVC.Mapping
{
    public class SeasonTeamProfile : Profile
    {
        public SeasonTeamProfile()
        {
            this.CreateMap<SeasonTeam, PlayoffTeamViewModel>()
                .IncludeMembers(x => x.Team)
                .ForMember(x => x.Id, opt => opt.Ignore());

            this.CreateMap<SeasonTeam, BracketSummaryModel>()
                .IncludeMembers(x => x.Team);
        }
    }
}
