// <copyright file="DatabaseSeedingExtensions.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Extensions
{
    using System.Security.Cryptography;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using MongoDB.Bson;
    using NflPlayoffPool.Data;
    using NflPlayoffPool.Data.Models;

    /// <summary>
    /// Database-agnostic seeding extensions for PlayoffPoolContext.
    /// Works with any EF Core provider (MongoDB, SQL Server, PostgreSQL, etc.).
    /// </summary>
    public static class DatabaseSeedingExtensions
    {
        /// <summary>
        /// Seeds the database with a default admin account if no admin users exist.
        /// Uses EF Core abstractions - works with any database provider.
        /// SECURITY: Requires ADMIN_PASSWORD environment variable to be set.
        /// </summary>
        /// <param name="context">The PlayoffPoolContext.</param>
        /// <param name="configuration">The configuration for admin account details.</param>
        public static void SeedAdminUser(this PlayoffPoolContext context, IConfiguration configuration)
        {
            Console.WriteLine("🌱 Starting admin user seeding process...");
            
            try
            {
                // Check if any admin users already exist using EF Core abstractions
                Console.WriteLine("🔍 Checking for existing admin users...");
                if (context.Users.Any(u => u.Roles.Contains(Role.Admin)))
                {
                    Console.WriteLine("✅ Admin user already exists, skipping seeding");
                    return;
                }

                Console.WriteLine("📋 No admin users found, proceeding with seeding...");

                // SECURITY: Admin password MUST be provided via environment variable
                var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
                if (string.IsNullOrEmpty(adminPassword))
                {
                    Console.WriteLine("❌ ADMIN_PASSWORD environment variable is required for admin user seeding");
                    Console.WriteLine("💡 Set ADMIN_PASSWORD in your .env file with a secure password");
                    Console.WriteLine("⚠️  Admin user seeding skipped - no admin account created");
                    return;
                }

                Console.WriteLine("🔐 Admin password found, validating security requirements...");

                // Validate password strength
                if (!IsPasswordSecure(adminPassword))
                {
                    Console.WriteLine("❌ ADMIN_PASSWORD does not meet security requirements");
                    Console.WriteLine("💡 Password must be at least 12 characters with uppercase, lowercase, numbers, and symbols");
                    Console.WriteLine("⚠️  Admin user seeding skipped - insecure password rejected");
                    return;
                }

                Console.WriteLine("✅ Password meets security requirements");

                // Get other admin account details from environment variables or configuration
                var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? 
                               configuration["AdminAccount:Email"] ?? 
                               "admin@nflplayoffpool.local";
                
                var adminFirstName = Environment.GetEnvironmentVariable("ADMIN_FIRST_NAME") ?? 
                                   configuration["AdminAccount:FirstName"] ?? 
                                   "Admin";
                
                var adminLastName = Environment.GetEnvironmentVariable("ADMIN_LAST_NAME") ?? 
                                  configuration["AdminAccount:LastName"] ?? 
                                  "User";

                Console.WriteLine($"👤 Creating admin user: {adminEmail}");

                // Hash the password using provider-agnostic method
                var passwordHash = HashPassword(adminPassword);

                // Create the admin user using EF Core entities
                var adminUser = new User
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    FirstName = adminFirstName,
                    LastName = adminLastName,
                    Email = adminEmail,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow,
                    Roles = new List<Role> { Role.Admin, Role.Player } // Admin can also play
                };

                // Add and save using EF Core abstractions
                Console.WriteLine("💾 Saving admin user to database...");
                context.Users.Add(adminUser);
                context.SaveChanges();

                Console.WriteLine($"✅ Default admin account created: {adminEmail}");
                Console.WriteLine("⚠️  Please change the default password after first login");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during admin user seeding: {ex.Message}");
                Console.WriteLine($"🔍 Stack trace: {ex.StackTrace}");
                throw; // Re-throw to ensure the application doesn't start with a broken state
            }
        }

        /// <summary>
        /// Validates that a password meets security requirements.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <returns>True if the password is secure, false otherwise.</returns>
        private static bool IsPasswordSecure(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 12)
            {
                return false;
            }

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSymbol = password.Any(c => !char.IsLetterOrDigit(c));

            return hasUpper && hasLower && hasDigit && hasSymbol;
        }

        /// <summary>
        /// Hashes a password using PBKDF2 with HMACSHA256.
        /// This is database-provider agnostic.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password in format "salt:hash".</returns>
        public static string HashPassword(string password)
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

        /// <summary>
        /// Verifies a password against a stored hash.
        /// This is database-provider agnostic.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="storedHash">The stored hash in format "salt:hash".</param>
        /// <returns>True if the password matches, false otherwise.</returns>
        public static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }

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