namespace AmerFamilyPlayoffs.Models
{
    public class GameModel
    {
        public int Id { get; set; }
        public TeamModel HomeTeam { get; set; }
        public TeamModel AwayTeam { get; set; }
        public TeamModel Winner { get; set; }
    }
}