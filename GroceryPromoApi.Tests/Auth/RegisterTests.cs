using GroceryPromoApi.Application.DTOs.Auth;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Options;
using GroceryPromoApi.Application.Services;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Domain.Exceptions;
using Microsoft.Extensions.Options;
using Moq;

namespace GroceryPromoApi.Tests.Auth
{
    public class RegisterTests
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IUserSessionRepository> _sessionRepository = new();
        private readonly AuthService _authService;

        public RegisterTests()
        {
            var jwtOptions = Options.Create(new JwtOptions
            {
                SecretKey = "super-secret-key-that-is-long-enough-32chars",
                Issuer = "GroceryPromoApi",
                Audience = "GroceryPromoApp",
                ExpirationMinutes = 30,
                RefreshTokenExpirationDays = 30
            });

            _authService = new AuthService(_userRepository.Object, _sessionRepository.Object, jwtOptions);
        }

        [Fact]
        public async Task Register_NewEmail_ReturnsAuthResponse()
        {
            _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((User?)null);

            _userRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            _sessionRepository.Setup(r => r.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "StrongPass1!",
                ConfirmPassword = "StrongPass1!"
            };

            var result = await _authService.RegisterAsync(request);

            Assert.NotNull(result);
            Assert.NotEmpty(result.AccessToken);
            Assert.NotEmpty(result.RefreshToken);
            Assert.True(result.ExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public async Task Register_ExistingEmail_ThrowsConflictException()
        {
            _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new User { Email = "test@example.com" });

            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "StrongPass1!",
                ConfirmPassword = "StrongPass1!"
            };

            await Assert.ThrowsAsync<ConflictException>(() => _authService.RegisterAsync(request));
        }

        [Fact]
        public async Task Register_EmailIsTrimmedAndLowercased()
        {
            User? capturedUser = null;

            _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((User?)null);

            _userRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                     .Callback<User, CancellationToken>((u, _) => capturedUser = u)
                     .Returns(Task.CompletedTask);

            _sessionRepository.Setup(r => r.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

            var request = new RegisterRequest
            {
                Email = "  TEST@Example.COM  ",
                Password = "StrongPass1!",
                ConfirmPassword = "StrongPass1!"
            };

            await _authService.RegisterAsync(request);

            Assert.Equal("test@example.com", capturedUser?.Email);
        }

        [Fact]
        public async Task Register_PasswordIsHashed()
        {
            User? capturedUser = null;

            _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((User?)null);

            _userRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                     .Callback<User, CancellationToken>((u, _) => capturedUser = u)
                     .Returns(Task.CompletedTask);

            _sessionRepository.Setup(r => r.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "StrongPass1!",
                ConfirmPassword = "StrongPass1!"
            };

            await _authService.RegisterAsync(request);

            Assert.NotNull(capturedUser?.PasswordHash);
            Assert.NotEqual("StrongPass1!", capturedUser!.PasswordHash);
            Assert.True(BCrypt.Net.BCrypt.Verify("StrongPass1!", capturedUser.PasswordHash));
        }
    }
}
