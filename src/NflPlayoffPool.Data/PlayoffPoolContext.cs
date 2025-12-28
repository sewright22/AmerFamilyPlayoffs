// <copyright file="PlayoffPoolContext.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using MongoDB.Driver;
    using MongoDB.EntityFrameworkCore.Extensions;
    using NflPlayoffPool.Data.Models;

    public class PlayoffPoolContext : DbContext
    {
        public static PlayoffPoolContext Create(IMongoDatabase database) =>
            new(new DbContextOptionsBuilder<PlayoffPoolContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);

        public PlayoffPoolContext(DbContextOptions<PlayoffPoolContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Bracket> Brackets { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<SuperGridRawImport> SuperGridRawImports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Season>().ToCollection("seasons");
            
            // Configure SuperGridRawImport with owned types to avoid shadow properties
            modelBuilder.Entity<SuperGridRawImport>(entity =>
            {
                entity.ToCollection("supergrid_raw_imports");
                
                // Configure Rows as owned type collection
                entity.OwnsMany(i => i.Rows, row =>
                {
                    // Configure Cells as owned type collection within Rows
                    row.OwnsMany(r => r.Cells);
                });
            });
        }
    }
}
