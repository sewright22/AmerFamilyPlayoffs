namespace AmerFamilyPlayoffs.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class TeamModel
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public bool IsInPlayoffs { get; set; }
        public int? Seed { get; set; }
    }
}
