using AmerFamilyPlayoffs.Data;

namespace PlayoffPool.MVC.Extensions
{
    public static class EfExtensions
    {
        public static IQueryable<PlayoffTeam> FilterConference(this IQueryable<PlayoffTeam> playoffTeams, string conference)
        {
            return playoffTeams.Where(x=>x.SeasonTeam.Conference.Name== conference);
        }

        public static PlayoffTeam GetTeamFromSeed(this IQueryable<PlayoffTeam> conferenceTeams, int seed)
        {
            var playoffTeam = conferenceTeams.FirstOrDefault(x => x.Seed == seed);

            if (playoffTeam == null)
            {
                throw new KeyNotFoundException(nameof(seed));
            }

            return playoffTeam;
        }
    }
}
