// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AmerFamilyPlayoffs.Api.Controllers
{
    using AmerFamilyPlayoffs.Api.Queries;
    using AmerFamilyPlayoffs.Data;
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
        public Task<List<Team>> Get([FromQuery] TeamQuery teamQuery)
        {
            return context.SeasonTeams.Include(st => st.Season)
                                      .Include(st => st.Team)
                                      .Where(st => st.Season.Year == teamQuery.Season)
                                      .Select(st => st.Team)
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
        public void Post([FromBody] string value)
        {
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
