using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DbContext;
using Model;

public class UserControllerTests
{
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly UserController.UserController _controller;
    private readonly Mock<DbSet<User>> _mockSet;

    public UserControllerTests()
    {
        _mockSet = new Mock<DbSet<User>>();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        var context = new ApplicationDbContext(options);
        _mockContext = new Mock<ApplicationDbContext>(options);
        _controller = new UserController.UserController(context);
    }

    [Fact]
    public async Task Register_Post_ValidModel_RedirectsToLogin()
    {
        // Arrange
        var user = new User
        {
            UserName = "t",
            Email = "test@example.com",
            Password = "password",
            FirstName = "Test",
            FamilyName = "User",
            PhoneNumber = "1234567890",
            Address = "123 Test St",
            CompanyName = "Test Company",
        };

        // Act
        var result = await _controller.Register(user);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirectResult.ActionName);
    }

    [Fact]
    public async Task Register_With_Invalid_Model_Should_Return_View()
    {
        // Arrange
        _controller.ModelState.AddModelError("Email", "Required");

        var user = new User
        {
            UserName = "i",
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
            Password = "password",
            FirstName = "Test",
            FamilyName = "User",
            PhoneNumber = "1234567890",
            Address = "123 Test St",
            CompanyName = "Test Company",
        };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "LoginDb")
            .Options;

        using var context = new ApplicationDbContext(options);
        context.Users.Add(user);
        context.SaveChanges();

        var controller = new UserController.UserController(context);

        // Act
        var result = await controller.Login(user.Email, user.Password);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task Login_Post_InvalidCredentials_ReturnsViewWithError()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InvalidLoginDb")
            .Options;

        using var context = new ApplicationDbContext(options);

        var controller = new UserController.UserController(context);

        // Act
        var result = await controller.Login("wrong@example.com", "wrongpassword");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }
}