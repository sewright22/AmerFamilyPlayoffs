namespace AmerFamilyPlayoffs.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public class PlayoffRound
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PlayoffId { get; set; }
        public int RoundId { get; set; }
        public int PointValue { get; set; }
        public virtual Playoff Playoff { get; set; }
        public virtual Round Round { get; set; }
        public virtual List<RoundWinner> RoundWinners { get; set; }
    }
}
