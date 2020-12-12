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
            return context.SeasonTeams.Include(st => st.Season)
                                      .Include(st => st.Team).ThenInclude(t => t.PlayoffTeam)
                                      .Where(st => st.Season.Year == teamQuery.Season)
                                      .Select(st => new TeamModel
                                      {
                                          Id = st.Team.Id,
                                          Abbreviation = st.Team.Abbreviation,
                                          Location = st.Team.Location,
                                          Name = st.Team.Name,
                                          Year = st.Season.Year,
                                          IsInPlayoffs = st.Team.PlayoffTeam != null && st.Team.PlayoffTeam.Playoff.Season.Year == teamQuery.Season,
                                          Seed = st.Team.PlayoffTeam == null ? null as int? : st.Team.PlayoffTeam.Seed,
                                      })
                                      .ToListAsync();
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
                var dbTeam = context.Teams.Include(t => t.PlayoffTeam).FirstOrDefault(x => x.Id == team.Id);

                var playoff = context.GetPlayoff(team.Year);

                context.SavePlayoffTeam(dbTeam, playoff, team.Seed.Value);
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
