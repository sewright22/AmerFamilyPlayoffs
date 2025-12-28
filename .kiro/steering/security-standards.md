---
inclusion: always
---

# Security Standards and Guidelines

This document establishes security standards for the NFL Playoff Pool project to ensure secure development practices.

## Password Security

### ❌ NEVER Hardcode Passwords
- **NO hardcoded passwords** in source code, configuration files, or documentation
- **NO default passwords** that are not secure
- **NO passwords in version control** (use .env files with .gitignore)

### ✅ Secure Password Management
- **Environment Variables**: All passwords MUST be provided via environment variables
- **Password Strength**: Enforce minimum security requirements:
  - Minimum 12 characters
  - Must contain uppercase letters
  - Must contain lowercase letters  
  - Must contain numbers
  - Must contain symbols/special characters
- **Validation**: Validate password strength before accepting
- **Documentation**: Provide clear examples of secure password formats

### Implementation Pattern
```csharp
// ✅ GOOD: Require secure password via environment variable
var password = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
if (string.IsNullOrEmpty(password))
{
    Console.WriteLine("❌ Password environment variable is required");
    return; // Fail securely
}

if (!IsPasswordSecure(password))
{
    Console.WriteLine("❌ Password does not meet security requirements");
    return; // Reject insecure passwords
}

// ❌ BAD: Hardcoded or weak default password
var password = configuration["Password"] ?? "Admin123!"; // NEVER DO THIS
```

## Environment Variable Security

### Required Environment Variables
All sensitive configuration MUST use environment variables:
- `MONGODB_ROOT_PASSWORD` - Database root password
- `ADMIN_PASSWORD` - Default admin account password
- Any API keys, connection strings, or secrets

### Environment Variable Validation
```csharp
// Validate required environment variables at startup
private static void ValidateSecurityConfiguration()
{
    var requiredSecureVars = new[] { "MONGODB_ROOT_PASSWORD", "ADMIN_PASSWORD" };
    
    foreach (var varName in requiredSecureVars)
    {
        var value = Environment.GetEnvironmentVariable(varName);
        if (string.IsNullOrEmpty(value) || value.Contains("CHANGE_ME"))
        {
            Console.WriteLine($"❌ Required secure variable '{varName}' not set");
            Environment.Exit(1); // Fail fast for security
        }
    }
}
```

## Configuration File Security

### .env File Management
- **Template Only**: Only commit `.env.template` with placeholder values
- **Local .env**: Never commit actual `.env` files (use .gitignore)
- **Clear Documentation**: Provide examples of secure values
- **Validation**: Application should validate environment variables at startup

### .env.template Format
```bash
# ✅ GOOD: Clear security guidance
# SECURITY: Set a secure password for the default admin account
# Password must be at least 12 characters with uppercase, lowercase, numbers, and symbols
# Example: MyS3cur3P@ssw0rd!2024
ADMIN_PASSWORD=CHANGE_ME_SECURE_PASSWORD

# ❌ BAD: Weak or hardcoded default
ADMIN_PASSWORD=admin123
```

## Database Security

### Connection Security
- Use authentication for all database connections
- Store credentials in environment variables only
- Use connection string validation
- Implement proper error handling without exposing credentials

### User Account Security
- Hash all passwords using strong algorithms (PBKDF2, bcrypt, Argon2)
- Use proper salt generation
- Implement secure password verification
- Never store plaintext passwords

## Development Practices

### Code Review Checklist
- [ ] No hardcoded passwords or secrets
- [ ] All sensitive data uses environment variables
- [ ] Password validation implemented
- [ ] Secure defaults (fail closed, not open)
- [ ] Error messages don't expose sensitive information

### Testing Security
- Use test-specific credentials, never production
- Mock external services in tests
- Don't commit test credentials to version control
- Use in-memory databases for unit tests when possible

## Deployment Security

### Environment Separation
- Different credentials for each environment (dev, staging, production)
- Secure credential management in deployment pipelines
- Regular credential rotation
- Monitoring for credential exposure

### Production Considerations
- Use secure credential storage (Azure Key Vault, AWS Secrets Manager, etc.)
- Implement proper logging without exposing secrets
- Regular security audits
- Incident response procedures

## Security Validation

### Startup Validation
Applications MUST validate security configuration at startup:
1. Check all required environment variables are set
2. Validate password strength requirements
3. Verify secure defaults are not being used
4. Fail fast if security requirements not met

### Runtime Security
- Log security events (failed logins, etc.)
- Implement rate limiting
- Use HTTPS in production
- Validate all user inputs

---

**Remember**: Security is not optional. When in doubt, fail securely and require explicit secure configuration rather than using insecure defaults.

*Last Updated: 2025-12-20*