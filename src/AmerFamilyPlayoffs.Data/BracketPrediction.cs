namespace AmerFamilyPlayoffs.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection.Metadata.Ecma335;
    using System.Text;

    public class BracketPrediction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PlayoffId { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }
        public virtual Playoff Playoff { get; set; }
        public virtual List<MatchupPrediction> MatchupPredictions { get; set; }
        public virtual User User { get; set; }

        [NotMapped]
        public virtual MatchupPrediction? SuperBowl
        {
            get => this.MatchupPredictions?.FirstOrDefault(x => x.PlayoffRound?.Round?.Number == 4);
        }
    }
}
