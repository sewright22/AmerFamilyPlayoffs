using AmerFamilyPlayoffs.Data;
using AutoMapper;
using PlayoffPool.MVC.Models;

namespace PlayoffPool.MVC.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            this.CreateMap<RegisterViewModel, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));

            this.CreateMap<LoginViewModel, User>()
                .ForMember(u => u.Email, opt => opt.MapFrom(x => x.Email));
        }
    }
}
