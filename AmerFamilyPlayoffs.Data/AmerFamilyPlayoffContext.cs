namespace AmerFamilyPlayoffs.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class AmerFamilyPlayoffContext : IdentityDbContext<User>
    {
        public AmerFamilyPlayoffContext() { }

        public AmerFamilyPlayoffContext(DbContextOptions<AmerFamilyPlayoffContext> options)
               : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Season> Seasons { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Playoff> Playoffs { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<PlayoffRound> PlayoffRounds { get; set; }
        public DbSet<Matchup> Matchups { get; set; }
        public DbSet<SeasonTeam> SeasonTeams { get; set; }
        public DbSet<PlayoffTeam> PlayoffTeams { get; set; }
        public DbSet<Conference> Conferences { get; set; }
        public DbSet<Bracket> Brackets { get; set; }
        public DbSet<BracketPrediction> BracketPredictions { get; set; }
        public DbSet<RoundWinner> RoundWinners { get; set; }
    }
}
