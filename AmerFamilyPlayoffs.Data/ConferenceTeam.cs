namespace AmerFamilyPlayoffs.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public class ConferenceTeam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ConferenceId { get; set; }
        public int SeasonTeamId { get; set; }
        public virtual Conference Conference { get; set; }
        public virtual SeasonTeam SeasonTeam { get; set; }
    }
}
