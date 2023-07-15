namespace AmerFamilyPlayoffs.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public class PlayoffRoundPointValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PlayoffRoundId { get; set; }
        public int PointValueId { get; set; }
        public virtual PointValue PointValue { get; set; }
    }
}
