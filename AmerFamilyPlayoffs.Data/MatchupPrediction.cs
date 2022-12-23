namespace AmerFamilyPlayoffs.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MatchupPrediction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BracketPredictionId { get; set; }
        public int PlayoffRoundId { get; set; }
        public virtual PlayoffTeam PredictedWinner { get; set; }
        public virtual PlayoffRound PlayoffRound { get; set; }
    }
}
