using AmerFamilyPlayoffs.Data;
using AutoMapper;
using PlayoffPool.MVC.Models;
using PlayoffPool.MVC.Models.Bracket;

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
                .ForPath(x => x.Id, opt => opt.MapFrom(x => x.Id));

            this.CreateMap<PlayoffTeam, TeamViewModel>()
                .IncludeMembers(x => x.SeasonTeam)
                .ForMember(x => x.Id, opt => opt.Ignore());

            this.CreateMap<SeasonTeam, TeamViewModel>()
                .IncludeMembers(x => x.Team)
                .ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}
