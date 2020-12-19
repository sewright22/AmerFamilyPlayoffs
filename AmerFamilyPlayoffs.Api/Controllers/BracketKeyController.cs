namespace AmerFamilyPlayoffs.Api.Controllers
{
    using AmerFamilyPlayoffs.Api.Extensions;
    using AmerFamilyPlayoffs.Api.Queries;
    using AmerFamilyPlayoffs.Data;
    using AmerFamilyPlayoffs.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    [Route("[controller]")]
    [ApiController]
    public class BracketKeyController : ControllerBase
    {
        private AmerFamilyPlayoffContext context;

        public BracketKeyController(AmerFamilyPlayoffContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public Task<List<PlayoffBracket>> Get([FromQuery] int year)
        {
            PlayoffBracket dbConference = context.GetBracketByYear(year);
            throw new NotImplementedException();
            //if (dbConference == null)
            //{
            //    return context.GetTeamsByYear(teamQuery.Season).ToListAsync();
            //}
            //else
            //{
            //    return context.GetTeamsByYearAndConference(teamQuery.Season, dbConference.Id).ToListAsync();
            //}
        }
    }
}
