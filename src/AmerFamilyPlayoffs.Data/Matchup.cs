namespace AmerFamilyPlayoffs.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public class Matchup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? HomeTeamId { get; set; }
        public int? AwayTeamId { get; set; }
        public int? WinningTeamId { get; set; }

        public virtual PlayoffTeam HomeTeam { get; set; }
        public virtual PlayoffTeam AwayTeam { get; set; }
        public virtual PlayoffTeam WinningTeam { get; set; }
    }
}
