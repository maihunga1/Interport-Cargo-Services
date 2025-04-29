using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DbContext;
using Model;
using UserControllers;
using Services;

public class UserControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IPasswordService> _mockPasswordService;

    public UserControllerTests()
    {
        _mockPasswordService = new Mock<IPasswordService>();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new ApplicationDbContext(options);

    }

    [Fact]
    public async Task Register_Post_ValidModel_RedirectsToLogin()
    {
        // Arrange
        var user = new User
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "password",
            FirstName = "Test",
            FamilyName = "User",
            PhoneNumber = "1234567890",
            Address = "123 Test St",
            CompanyName = "Test Company",
        };

        _mockPasswordService.Setup(p => p.HashPassword(It.IsAny<string>())).Returns("hashedPassword");

        UserController _controller = new(_context, _mockPasswordService.Object);

        // Act
        var result = await _controller.Register(user);

        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        Assert.NotNull(savedUser);
        Assert.Equal("hashedPassword", savedUser.Password);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirectResult.ActionName);
    }

    [Fact]
    public async Task Register_With_Invalid_Model_Should_Return_View()
    {
        // Arrange
        UserController _controller = new UserController(_context, _mockPasswordService.Object);
        _controller.ModelState.AddModelError("Email", "Required");

        var user = new User
        {
            UserName = "testuser",
            FirstName = "Jane",
            FamilyName = "Doe",
            PhoneNumber = "9876543210",
            Address = "456 Another St",
            Email = "test@example.com",
            CompanyName = "Example Inc",
            Password = "anotherpassword"
        };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "LoginDb")
            .Options;

        using var context = new ApplicationDbContext(options);

        // Act
        var result = await _controller.Register(user);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedModel = Assert.IsType<User>(viewResult.Model);
        Assert.Equal(user.UserName, returnedModel.UserName);

        // User should NOT be saved
        var savedUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "invaliduser");
        Assert.Null(savedUser);
    }

    [Fact]
    public async Task Login_Post_ValidCredentials_RedirectsToHomeIndex()
    {
        // Arrange
        var user = new User
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "hashedPassword",
            FirstName = "Test",
            FamilyName = "User",
            PhoneNumber = "1234567890",
            Address = "123 Test St",
            CompanyName = "Test Company",
        };

        _mockPasswordService
        .Setup(p => p.VerifyPassword(user.Password, "password123"))
        .Returns(true);

        var controller = new UserController(_context, _mockPasswordService.Object);

        // Act
        var result = await controller.Login("test@example.com", "password123");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task Login_Post_InvalidCredentials_ReturnsViewWithError()
    {
        // Arrange

        var user = new User
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "hashedPassword",
            FirstName = "Test",
            FamilyName = "User",
            PhoneNumber = "1234567890",
            Address = "123 Test St",
            CompanyName = "Test Company",
        };

        _mockPasswordService
        .Setup(p => p.VerifyPassword(user.Password, "wrongPassword"))
        .Returns(false);

        var controller = new UserController(_context, _mockPasswordService.Object);

        // Act
        var result = await controller.Login("test@example.com", "wrongPassword");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }
}