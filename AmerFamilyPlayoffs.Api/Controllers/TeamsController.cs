// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
    public class TeamsController : ControllerBase
    {
        private AmerFamilyPlayoffContext context;

        public TeamsController(AmerFamilyPlayoffContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: <TeamsController>
        [HttpGet]
        public Task<List<TeamModel>> Get([FromQuery] TeamQuery teamQuery)
        {
            var dbConference = context.Conferences.FirstOrDefault(c => c.Name == teamQuery.Conference);

            if (dbConference == null)
            {
                return context.GetTeamsByYear(teamQuery.Season).ToListAsync();
            }
            else
            {
                return context.GetTeamsByYearAndConference(teamQuery.Season, dbConference.Id).ToListAsync();
            }
        }

        // GET <TeamsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST <TeamsController>
        [HttpPost]
        public void Post([FromBody] List<TeamModel> teams)
        {
            foreach (var team in teams)
            {
                var playoff = context.GetPlayoff(team.Year);
                var seasonTeam = context.GetSeasonTeam(team.Id, team.Year);

                context.SavePlayoffTeam(seasonTeam, playoff, team.Seed.Value);
            }
        }

        // PUT <TeamsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE <TeamsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
