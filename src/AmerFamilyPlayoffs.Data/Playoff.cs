namespace AmerFamilyPlayoffs.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public class Playoff
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SeasonId { get; set; }
        public virtual Season Season { get; set; }
        public virtual List<PlayoffRound> PlayoffRounds { get; set; }
        public virtual List<PlayoffTeam> PlayoffTeams { get; set; }
    }
}
