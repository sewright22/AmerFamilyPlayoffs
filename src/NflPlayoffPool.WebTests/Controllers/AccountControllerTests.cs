using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NflPlayoffPool.Data;
using NflPlayoffPool.Data.Models;
using NflPlayoffPool.TestCommon.Builders;
using NflPlayoffPool.Web.Controllers;
using NflPlayoffPool.Web.Extensions;
using NflPlayoffPool.Web.Models;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace NflPlayoffPool.WebTests.Controllers;

/// <summary>
/// Test authentication handler for unit testing authentication scenarios
/// </summary>
public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.NameIdentifier, "123")
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

/// <summary>
/// Tests for AccountController authentication and user management logic
/// These methods contain security-critical business logic for user authentication
/// </summary>
[TestClass]
public class AccountControllerTests
{
    private PlayoffPoolContext _context = null!;
    private AccountController _controller = null!;
    private ILogger<AccountController> _logger = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PlayoffPoolContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PlayoffPoolContext(options);
        _logger = new LoggerFactory().CreateLogger<AccountController>();
        _controller = new AccountController(_logger, _context);

        // Setup HTTP context with authentication and MVC services
        var services = new ServiceCollection();
        services.AddAuthentication("Test")
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { })
            .AddCookie("Cookies"); // Add cookie authentication for the controller
        services.AddMvc();
        services.AddLogging();
        services.AddRouting(); // Add routing services for URL generation
        
        var serviceProvider = services.BuildServiceProvider();
        
        var httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };
        
        // Setup ActionContext for URL routing
        var actionDescriptor = new ControllerActionDescriptor
        {
            ActionName = "TestAction",
            ControllerName = "TestController"
        };
        var actionContext = new ActionContext(httpContext, new RouteData(), actionDescriptor);
        
        _controller.ControllerContext = new ControllerContext(actionContext);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void Login_Get_ReturnsViewWithModel()
    {
        // Arrange
        var returnUrl = "/test-return-url";

        // Act
        var result = _controller.Login(returnUrl) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().BeOfType<LoginViewModel>();
        
        var viewModel = result.Model as LoginViewModel;
        viewModel!.ReturnUrl.Should().Be(returnUrl);
    }

    [TestMethod]
    public void Login_Get_WithDefaultReturnUrl_SetsDefaultValue()
    {
        // Act
        var result = _controller.Login() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        var viewModel = result!.Model as LoginViewModel;
        viewModel!.ReturnUrl.Should().Be("/");
    }

    [TestMethod]
    public async Task Login_Post_WithValidCredentials_RedirectsToReturnUrl()
    {
        // Arrange
        var password = "TestPassword123!";
        var hashedPassword = DatabaseSeedingExtensions.HashPassword(password);
        
        var user = new UserBuilder()
            .WithEmail("test@example.com")
            .WithFirstName("Test")
            .WithLastName("User")
            .WithPasswordHash(hashedPassword)
            .Build();
        
        _context.Users.Add(user);
        _context.SaveChanges();

        var loginModel = new LoginViewModel
        {
            Email = "test@example.com",
            Password = password,
            ReturnUrl = "/dashboard"
        };

        // Act
        var result = await _controller.Login(loginModel);

        // Assert
        result.Should().BeOfType<LocalRedirectResult>();
        var redirectResult = result as LocalRedirectResult;
        redirectResult!.Url.Should().Be("/dashboard");
    }

    [TestMethod]
    public async Task Login_Post_WithInvalidCredentials_ReturnsViewWithError()
    {
        // Arrange
        var loginModel = new LoginViewModel
        {
            Email = "invalid@example.com",
            Password = "WrongPassword",
            ReturnUrl = "/dashboard"
        };

        // Act
        var result = await _controller.Login(loginModel);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().Be(loginModel);
        
        _controller.ModelState.Should().ContainKey(string.Empty);
        _controller.ModelState[string.Empty]!.Errors.Should().Contain(e => e.ErrorMessage == "Invalid login attempt.");
    }

    [TestMethod]
    public async Task Login_Post_WithNullUser_ReturnsViewWithError()
    {
        // Arrange - No user in database
        var loginModel = new LoginViewModel
        {
            Email = "nonexistent@example.com",
            Password = "SomePassword123!",
            ReturnUrl = "/dashboard"
        };

        // Act
        var result = await _controller.Login(loginModel);

        // Assert
        result.Should().BeOfType<ViewResult>();
        _controller.ModelState.Should().ContainKey(string.Empty);
        _controller.ModelState[string.Empty]!.Errors.Should().Contain(e => e.ErrorMessage == "Invalid login attempt.");
    }

    [TestMethod]
    public void Register_Get_ReturnsView()
    {
        // Act
        var result = _controller.Register();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [TestMethod]
    public async Task Logout_RedirectsToHome()
    {
        // Act
        var result = await _controller.Logout();

        // Assert
        result.Should().BeOfType<RedirectToActionResult>();
        var redirectResult = result as RedirectToActionResult;
        redirectResult!.ActionName.Should().Be("Index");
        redirectResult.ControllerName.Should().Be("Home");
    }

    // Helper methods
    private User CreateTestUser(string email, string name)
    {
        return new UserBuilder()
            .WithEmail(email)
            .WithFirstName(name.Split(' ')[0])
            .WithLastName(name.Split(' ').Length > 1 ? name.Split(' ')[1] : "")
            .WithPasswordHash("hashed_password_here") // In real app, this would be properly hashed
            .WithRole(Role.Player)
            .Build();
    }
}