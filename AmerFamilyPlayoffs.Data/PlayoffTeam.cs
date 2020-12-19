namespace AmerFamilyPlayoffs.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public class PlayoffTeam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PlayoffId { get; set; }

        public int Seed { get; set; }
        public int SeasonTeamId { get; set; }

        public virtual SeasonTeam SeasonTeam { get; set; }
        public virtual Playoff Playoff { get; set; }
    }
}
