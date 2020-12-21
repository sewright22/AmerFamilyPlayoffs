namespace AmerFamilyPlayoffs.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public class BracketPrediction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PlayoffId { get; set; }

        public string Name { get; set; }
        public virtual Playoff Playoff { get; set; }
        public virtual List<MatchupPrediction> MatchupPredictions { get; set; }
    }
}
