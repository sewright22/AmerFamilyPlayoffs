using NflPlayoffPool.Data.Models;

namespace NflPlayoffPool.TestCommon.Builders;

/// <summary>
/// Builder for creating User test data with fluent API
/// </summary>
public class UserBuilder
{
    private User _user;

    public UserBuilder()
    {
        _user = new User
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "default_hash",
            Roles = new List<Role> { Role.Player },
            CreatedAt = DateTime.UtcNow
        };
    }

    public UserBuilder WithId(Guid id)
    {
        _user.Id = id.ToString();
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _user.Email = email;
        return this;
    }

    public UserBuilder WithFirstName(string firstName)
    {
        _user.FirstName = firstName;
        return this;
    }

    public UserBuilder WithLastName(string lastName)
    {
        _user.LastName = lastName;
        return this;
    }

    public UserBuilder WithPasswordHash(string passwordHash)
    {
        _user.PasswordHash = passwordHash;
        return this;
    }

    public UserBuilder WithRole(Role role)
    {
        _user.Roles = new List<Role> { role };
        return this;
    }

    public UserBuilder WithRoles(params Role[] roles)
    {
        _user.Roles = roles.ToList();
        return this;
    }

    public UserBuilder WithCreatedAt(DateTime createdAt)
    {
        _user.CreatedAt = createdAt;
        return this;
    }

    public User Build() => _user;
}