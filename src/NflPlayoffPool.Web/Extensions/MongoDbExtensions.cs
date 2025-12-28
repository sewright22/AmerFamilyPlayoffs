// <copyright file="MongoDbExtensions.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Extensions
{
    using System.Security.Cryptography;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using Microsoft.EntityFrameworkCore;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using NflPlayoffPool.Data;
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Models;

    public static class MongoDbExtensions
    {
        public static IServiceCollection AddMongoDbContext(this IServiceCollection services, string connectionString, string databaseName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("MongoDB connection string is missing.");
            }

            var client = new MongoClient(connectionString);
            var db = MflixDbContext.Create(client.GetDatabase(databaseName));
            services.AddSingleton<MflixDbContext>(sp => db);
            return services;
        }

        public static IServiceCollection AddPlayoffPoolContext(this IServiceCollection services, string connectionString, string databaseName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("MongoDB connection string is missing.");
            }

            var client = new MongoClient(connectionString);
            var db = PlayoffPoolContext.Create(client.GetDatabase(databaseName));
            services.AddSingleton<PlayoffPoolContext>(sp => db);
            return services;
        }

        public static User? CreateUser(this PlayoffPoolContext dbContext, RegisterViewModel registerViewModel)
        {
            // Check if the user already exists
            if (dbContext.Users.Any(u => u.Email == registerViewModel.Email))
            {
                return null; // User already exists, return null
            }

            // Hash the password
            var passwordHash = HashPassword(registerViewModel.Password);

            // Create a new User object
            var newUser = new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                Email = registerViewModel.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                Roles = new List<Role> { Role.Player } // Default role is 'User'
            };

            // Add and save the user to the database
            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();

            return newUser; // Return the newly created user
        }

        public static User? ValidateUser(this PlayoffPoolContext dbContext, LoginViewModel loginViewModel)
        {
            // Find user by email
            var user = dbContext.Users.FirstOrDefault(u => u.Email == loginViewModel.Email);

            // Return null if user doesn't exist
            if (user == null)
            {
                return null;
            }

            // Verify password
            if (VerifyPassword(loginViewModel.Password, user.PasswordHash))
            {
                return user; // Return user if password matches
            }

            return null; // Return null if password is incorrect
        }

        private static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));

            return $"{Convert.ToBase64String(salt)}:{hashed}";
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));

            return hash == parts[1];
        }
    }
}
