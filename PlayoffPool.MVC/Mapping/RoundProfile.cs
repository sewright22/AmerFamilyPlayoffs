using AmerFamilyPlayoffs.Data;
using AutoMapper;
using PlayoffPool.MVC.Models.Admin;

namespace PlayoffPool.MVC.Mapping
{
    public class RoundProfile : Profile
    {
        public RoundProfile()
        {
            this.CreateMap<Round, AdminRoundViewModel>();
        }
    }
}
