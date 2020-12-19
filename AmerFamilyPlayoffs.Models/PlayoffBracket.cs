namespace AmerFamilyPlayoffs.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class PlayoffBracket
    {
        public int Id { get; set; }
        public RoundModel WildCardRound { get; set; }
        public RoundModel DivisionalRound { get; set; }
        public RoundModel ChampionshipRound { get; set; }
        public GameModel SuperBowl { get; set; }
    }
}
