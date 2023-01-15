using AmerFamilyPlayoffs.Data;
using AutoMapper;
using PlayoffPool.MVC.Models;
using PlayoffPool.MVC.Models.Home;

namespace PlayoffPool.MVC.Mapping
{
    public class PlayoffTeamProfile : Profile
    {
        public PlayoffTeamProfile()
        {
            this.CreateMap<PlayoffTeam, PlayoffTeamViewModel>()
                .IncludeMembers(x => x.SeasonTeam)
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id));

            this.CreateMap<PlayoffTeam, BracketSummaryModel>()
                .IncludeMembers(x => x.SeasonTeam);

            this.CreateMap<PlayoffTeamViewModel, PlayoffTeamViewModel>()
                .ForMember(x => x.ViewId, obj => obj.Ignore());
        }
    }
}
