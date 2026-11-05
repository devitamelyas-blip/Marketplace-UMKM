using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using UmkmJatim.Application.Auth.Commands;
using UmkmJatim.Application.Common.Interfaces;
using UmkmJatim.Domain.Entities;
using UmkmJatim.Domain.Exceptions;
using Xunit;

namespace UmkmJatim.UnitTests.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        var storeMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            storeMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _tokenServiceMock = new Mock<ITokenService>();
        _contextMock = new Mock<IApplicationDbContext>();

        _handler = new LoginCommandHandler(
            _userManagerMock.Object,
            _tokenServiceMock.Object,
            _contextMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnTokensAndUserInfo()
    {
        // Arrange
        var command = new LoginCommand("user@example.com", "Password123!", "127.0.0.1");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            FullName = "User Test",
            Role = "Customer",
            IsActive = true
        };

        var refreshToken = new RefreshToken
        {
            Token = "sample-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, command.Password)).ReturnsAsync(true);
        _tokenServiceMock.Setup(m => m.GenerateAccessToken(user)).Returns("sample-access-token");
        _tokenServiceMock.Setup(m => m.GenerateRefreshToken(command.IpAddress)).Returns(refreshToken);
        _contextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("sample-access-token");
        result.RefreshToken.Should().Be("sample-refresh-token");
        result.User.Email.Should().Be(user.Email);
        result.User.FullName.Should().Be(user.FullName);
    }

    [Fact]
    public async Task Handle_InvalidEmail_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var command = new LoginCommand("notfound@example.com", "Password123!");
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync((ApplicationUser?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task Handle_WrongPassword_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var command = new LoginCommand("user@example.com", "WrongPassword!");
        var user = new ApplicationUser { Email = command.Email, IsActive = true };

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, command.Password)).ReturnsAsync(false);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task Handle_InactiveUser_ShouldThrowForbiddenException()
    {
        // Arrange
        var command = new LoginCommand("inactive@example.com", "Password123!");
        var user = new ApplicationUser { Email = command.Email, IsActive = false };

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, command.Password)).ReturnsAsync(true);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("*dinonaktifkan*");
    }
}
