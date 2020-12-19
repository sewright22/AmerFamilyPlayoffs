using System.Collections.Generic;

namespace AmerFamilyPlayoffs.Models
{
    public class RoundModel
    {
        public int PointValue { get; set; }
        public List<GameModel> AFCGames { get; set; }
        public List<GameModel> NFCGames { get; set; }
    }
}