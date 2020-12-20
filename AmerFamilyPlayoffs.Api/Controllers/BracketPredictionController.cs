namespace AmerFamilyPlayoffs.Api.Controllers
{
    using AmerFamilyPlayoffs.Api.Extensions;
    using AmerFamilyPlayoffs.Data;
    using AmerFamilyPlayoffs.Models;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    [Route("[controller]")]
    [ApiController]
    public class BracketPredictionController : ControllerBase
    {
        private AmerFamilyPlayoffContext context;

        public BracketPredictionController(AmerFamilyPlayoffContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayoffBracketPrediction>>> GetBrackets()
        {
            return Ok(await this.context.GetBrackets());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayoffBracketPrediction>> GetBracket(int id)
        {
            return Ok(await this.context.GetBracketPrediction(id));
        }

        [HttpPost]
        public async Task<ActionResult<PlayoffBracketPrediction>> CreateBracketPrediction(PlayoffBracketPrediction playoffBracketPrediction)
        {
            var created = await this.context.CreateBracketPrediction(playoffBracketPrediction.Name);
            return CreatedAtAction(nameof(CreateBracketPrediction), new { Id = created.Id }, created);
        }
    }
}
