using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using UmkmJatim.Application.Auth.Commands;
using UmkmJatim.Domain.Entities;
using UmkmJatim.Domain.Exceptions;
using Xunit;

namespace UmkmJatim.UnitTests.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        var storeMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            storeMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _handler = new RegisterCommandHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateUserAndReturnResponse()
    {
        // Arrange
        var command = new RegisterCommand(
            FullName: "Budi Santoso",
            Email: "budi@example.com",
            Password: "Password123!",
            ConfirmPassword: "Password123!",
            PhoneNumber: "081234567890"
        );

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((ApplicationUser?)null);

        _userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), command.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Customer"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(command.Email);
        result.FullName.Should().Be(command.FullName);
        result.Role.Should().Be("Customer");

        _userManagerMock.Verify(m => m.CreateAsync(It.Is<ApplicationUser>(u => 
            u.Email == command.Email && u.FullName == command.FullName), command.Password), Times.Once);

        _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Customer"), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ShouldThrowConflictException()
    {
        // Arrange
        var command = new RegisterCommand(
            FullName: "Budi Santoso",
            Email: "budi@example.com",
            Password: "Password123!",
            ConfirmPassword: "Password123!",
            PhoneNumber: "081234567890"
        );

        _userManagerMock
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync(new ApplicationUser { Email = command.Email });

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*sudah terdaftar*");
    }

    [Theory]
    [InlineData("short", false)]
    [InlineData("nocapital123", false)]
    [InlineData("NO_LOWER_123", false)]
    [InlineData("ValidPassword123", true)]
    public void Validator_PasswordRules_ShouldValidateCorrectly(string password, bool shouldBeValid)
    {
        // Arrange
        var validator = new RegisterCommandValidator();
        var command = new RegisterCommand(
            FullName: "Test User",
            Email: "test@example.com",
            Password: password,
            ConfirmPassword: password,
            PhoneNumber: "081234567890"
        );

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().Be(shouldBeValid);
    }
}
