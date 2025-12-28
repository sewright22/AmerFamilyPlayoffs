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
        /// <summary>
        /// Configures MongoDB connection and health checks for containerized deployment.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddContainerizedMongoDB(this IServiceCollection services, IConfiguration configuration)
        {
            // Validate required environment variables
            ValidateMongoDbConfiguration();

            // Build connection string from environment variables
            var connectionString = BuildMongoDbConnectionString(configuration);

            // Get database name
            var databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE") ?? "playoff_pool";

            // Add MongoDB context
            services.AddPlayoffPoolContext(connectionString, databaseName);

            // Add basic health checks
            services.AddHealthChecks();

            return services;
        }

        /// <summary>
        /// Validates that all required MongoDB environment variables are present.
        /// </summary>
        private static void ValidateMongoDbConfiguration()
        {
            var requiredVars = new[] { "MONGODB_ROOT_PASSWORD", "ADMIN_PASSWORD" };

            foreach (var varName in requiredVars)
            {
                var value = Environment.GetEnvironmentVariable(varName);
                if (string.IsNullOrEmpty(value) || value == "CHANGE_ME_SECURE_PASSWORD")
                {
                    Console.WriteLine($"❌ Required environment variable '{varName}' is missing or using default value");
                    Console.WriteLine("💡 Please update your .env file with proper secure values");
                    Console.WriteLine("🔒 See security-standards.md for password requirements");
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Builds MongoDB connection string from environment variables with fallback to configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The MongoDB connection string.</returns>
        private static string BuildMongoDbConnectionString(IConfiguration configuration)
        {
            // Check for explicit connection string first
            var explicitConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") ??
                                         configuration.GetConnectionString("MongoDb");

            if (!string.IsNullOrEmpty(explicitConnectionString))
            {
                return explicitConnectionString;
            }

            // Build connection string from individual components
            var mongoPassword = Environment.GetEnvironmentVariable("MONGODB_ROOT_PASSWORD");
            var mongoHost = Environment.GetEnvironmentVariable("MONGODB_HOST") ?? "mongodb";
            var mongoPort = Environment.GetEnvironmentVariable("MONGODB_PORT") ?? "27017";
            var mongoDatabase = Environment.GetEnvironmentVariable("MONGODB_DATABASE") ?? "playoff_pool";

            // URL encode the password to handle special characters
            var encodedPassword = Uri.EscapeDataString(mongoPassword ?? string.Empty);

            var connectionString = $"mongodb://admin:{encodedPassword}@{mongoHost}:{mongoPort}/{mongoDatabase}?authSource=admin";

            Console.WriteLine($"🔗 Using MongoDB connection: mongodb://admin:***@{mongoHost}:{mongoPort}/{mongoDatabase}");

            return connectionString;
        }

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
            // Check if the user already exists (case-insensitive email comparison)
            var existingUser = dbContext.Users
                .AsEnumerable() // Switch to client-side evaluation for case-insensitive comparison
                .FirstOrDefault(u => string.Equals(u.Email, registerViewModel.Email, StringComparison.OrdinalIgnoreCase));
            
            if (existingUser != null)
            {
                return null; // User already exists, return null
            }

            // Hash the password using database-agnostic method
            var passwordHash = DatabaseSeedingExtensions.HashPassword(registerViewModel.Password);

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
            // Find user by email (case-insensitive comparison)
            var user = dbContext.Users
                .AsEnumerable() // Switch to client-side evaluation for case-insensitive comparison
                .FirstOrDefault(u => string.Equals(u.Email, loginViewModel.Email, StringComparison.OrdinalIgnoreCase));

            // Return null if user doesn't exist
            if (user == null)
            {
                return null;
            }

            // Verify password using database-agnostic method
            if (DatabaseSeedingExtensions.VerifyPassword(loginViewModel.Password, user.PasswordHash))
            {
                return user; // Return user if password matches
            }

            return null; // Return null if password is incorrect
        }

    }
}
