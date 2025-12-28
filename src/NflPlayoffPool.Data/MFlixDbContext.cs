// <copyright file="MFlixDbContext.cs" company="stevencodeswright">
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

    public class MflixDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; init; }
        public static MflixDbContext Create(IMongoDatabase database) =>
            new(new DbContextOptionsBuilder<MflixDbContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);
        public MflixDbContext(DbContextOptions options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Movie>().ToCollection("movies");
        }
    }
}
