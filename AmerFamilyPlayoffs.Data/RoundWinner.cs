using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmerFamilyPlayoffs.Data
{
    public class RoundWinner
    {
        public int Id { get; set; }
        public int PlayoffRoundId { get; set; }
        public int PlayoffTeamId { get; set; }
        public virtual PlayoffRound PlayoffRound { get; set; }
        public virtual PlayoffTeam PlayoffTeam { get; set; }
    }
}
