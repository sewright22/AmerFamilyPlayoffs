namespace AmerFamilyPlayoffs.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public class SeasonTeam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SeasonId { get; set; }
        public int TeamId { get; set; }
        public int ConferenceId { get; set; }
        public virtual Season Season { get; set; }
        public virtual Team Team { get; set; }
        public virtual Conference Conference { get; set; }
        public virtual PlayoffTeam PlayoffTeam { get; set; }
    }
}
