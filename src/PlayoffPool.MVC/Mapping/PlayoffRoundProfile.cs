using AmerFamilyPlayoffs.Data;
using AutoMapper;
using PlayoffPool.MVC.Models.Admin;

namespace PlayoffPool.MVC.Mapping
{
    public class PlayoffRoundProfile : Profile
    {
        public PlayoffRoundProfile()
        {
            this.CreateMap<PlayoffRound, AdminRoundViewModel>();
        }
    }
}
