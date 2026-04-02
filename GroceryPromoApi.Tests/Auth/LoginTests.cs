using GroceryPromoApi.Application.DTOs.Auth;
using GroceryPromoApi.Application.Requests.Auth;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Options;
using GroceryPromoApi.Application.Services;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace GroceryPromoApi.Tests.Auth
{
    public class LoginTests
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IUserSessionRepository> _sessionRepository = new();
        private readonly AuthService _authService;

        public LoginTests()
        {
            var jwtOptions = Options.Create(new JwtOptions
            {
                SecretKey = "super-secret-key-that-is-long-enough-32chars",
                Issuer = "GroceryPromoApi",
                Audience = "GroceryPromoApp",
                ExpirationMinutes = 30,
                RefreshTokenExpirationDays = 30
            });

            _authService = new AuthService(_userRepository.Object, _sessionRepository.Object, jwtOptions, Mock.Of<ILogger<AuthService>>());
        }

        [Fact]
        public async Task Login_With_ValidCredencials()
        {
            _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new User
                     {
                         Id = Guid.NewGuid(),
                         Email = "test@example.com",
                         PasswordHash = BCrypt.Net.BCrypt.HashPassword("StrongPass1!"),
                         Role = "User"
                     });

            _userRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            _sessionRepository.Setup(r => r.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "StrongPass1!",
            };

            var result = await _authService.LoginAsync(request);

            Assert.NotNull(result);
            Assert.NotEmpty(result.AccessToken);
            Assert.NotEmpty(result.RefreshToken);
            Assert.True(result.ExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public async Task Login_With_UnknownEmail_ThrowsUnauthorizedException()
        {
            var request = new LoginRequest
            {
                Email = "unknown@example.com",
                Password = "StrongPass1!",
            };

            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));
        }

        [Fact]
        public async Task Login_With_WrongPassword_ThrowsUnauthorizedException()
        {
            _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new User
                     {
                         Id = Guid.NewGuid(),
                         Email = "test@example.com",
                         PasswordHash = BCrypt.Net.BCrypt.HashPassword("StrongPass1!"),
                         Role = "User"
                     });

            _sessionRepository.Setup(r => r.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPass123",
            };

            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));
        }

        [Fact]
        public async Task Lockout_Then_Succesfful_Login()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("StrongPass1!"),
                Role = "User"
            };

            _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

            var attempts = 0;
            _userRepository.Setup(r => r.IncrementFailedAttemptsAsync(user.Id, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(() => ++attempts);

            User? capturedUser = null;
            _userRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                     .Callback<User, CancellationToken>((u, _) => capturedUser = u)
                     .Returns(Task.CompletedTask);

            _userRepository.Setup(r => r.ResetFailedAttemptsAsync(user.Id, It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            _sessionRepository.Setup(r => r.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPass123",
            };

            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));//1
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));//2
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));//3
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));//4
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));//5

            user.LockoutUntil = DateTime.UtcNow.AddMinutes(-1);

            request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "StrongPass1!",
            };

            await _authService.LoginAsync(request);

            Assert.Equal(0, capturedUser?.FailedLoginAttempts);
            Assert.Null(capturedUser?.LockoutUntil);
        }

        [Fact]
        public async Task Lockout_ThrowsUnauthorizedException()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("StrongPass1!"),
                Role = "User",
                LockoutUntil = DateTime.UtcNow.AddMinutes(1)
            };

            _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(user);

            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(new LoginRequest
            {
                Email = "test@example.com",
                Password = "StrongPass1!"
            }));
        }
    }
}
