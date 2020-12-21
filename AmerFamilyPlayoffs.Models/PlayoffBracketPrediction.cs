namespace AmerFamilyPlayoffs.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    public class PlayoffBracketPrediction : PlayoffBracket
    {
        [Required]
        [StringLength(1000, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }
    }
}
