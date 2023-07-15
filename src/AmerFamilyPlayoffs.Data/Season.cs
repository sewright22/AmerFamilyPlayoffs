namespace AmerFamilyPlayoffs.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;
    using Microsoft.EntityFrameworkCore;

    [Index(nameof(Year), IsUnique =true)]
    public class Season
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Year { get; set; }
        public string Description { get; set; }
        public virtual Playoff Playoff { get; set; }
    }
}
