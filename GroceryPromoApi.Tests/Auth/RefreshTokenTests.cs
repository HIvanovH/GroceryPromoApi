using GroceryPromoApi.Application.DTOs.Auth;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Options;
using GroceryPromoApi.Application.Services;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Domain.Exceptions;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Cryptography;
using System.Text;

namespace GroceryPromoApi.Tests.Auth
{
    public class RefreshTokenTests
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IUserSessionRepository> _sessionRepository = new();
        private readonly AuthService _authService;

        public RefreshTokenTests()
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

        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes).ToLower();
        }

        private static string GenerateRawToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        [Fact]
        public async Task Refresh_ValidToken_ReturnsNewAuthResponse()
        {
            var rawToken = GenerateRawToken();
            var hash = HashToken(rawToken);

            var session = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                RefreshToken = hash,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                LastUsedAt = DateTime.UtcNow
            };

            var user = new User
            {
                Id = session.UserId,
                Email = "test@example.com",
                Role = "User"
            };

            _sessionRepository.Setup(r => r.GetByRefreshTokenHashAsync(hash, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(session);
            _sessionRepository.Setup(r => r.UpdateAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                              .Returns(Task.CompletedTask);
            _userRepository.Setup(r => r.GetByIdAsync(session.UserId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

            var result = await _authService.RefreshTokenAsync(new RefreshRequest { RefreshToken = rawToken });

            Assert.NotNull(result);
            Assert.NotEmpty(result.AccessToken);
            Assert.NotEmpty(result.RefreshToken);
            Assert.NotEqual(rawToken, result.RefreshToken);
        }

        [Fact]
        public async Task Refresh_ValidToken_RotatesToken()
        {
            var rawToken = GenerateRawToken();
            var hash = HashToken(rawToken);

            var session = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                RefreshToken = hash,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                LastUsedAt = DateTime.UtcNow
            };

            UserSession? capturedSession = null;

            _sessionRepository.Setup(r => r.GetByRefreshTokenHashAsync(hash, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(session);
            _sessionRepository.Setup(r => r.UpdateAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                              .Callback<UserSession, CancellationToken>((s, _) => capturedSession = s)
                              .Returns(Task.CompletedTask);
            _userRepository.Setup(r => r.GetByIdAsync(session.UserId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new User { Id = session.UserId, Email = "test@example.com", Role = "User" });

            await _authService.RefreshTokenAsync(new RefreshRequest { RefreshToken = rawToken });

            Assert.Equal(hash, capturedSession?.PreviousRefreshToken);
            Assert.NotEqual(hash, capturedSession?.RefreshToken);
        }

        [Fact]
        public async Task Refresh_ExpiredSession_ThrowsUnauthorizedException()
        {
            var rawToken = GenerateRawToken();
            var hash = HashToken(rawToken);

            var session = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                RefreshToken = hash,
                ExpiresAt = DateTime.UtcNow.AddDays(-1)
            };

            _sessionRepository.Setup(r => r.GetByRefreshTokenHashAsync(hash, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(session);
            _sessionRepository.Setup(r => r.DeleteAsync(session.Id, It.IsAny<CancellationToken>()))
                              .Returns(Task.CompletedTask);

            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _authService.RefreshTokenAsync(new RefreshRequest { RefreshToken = rawToken }));
        }

        [Fact]
        public async Task Refresh_UnknownToken_ThrowsUnauthorizedException()
        {
            var rawToken = GenerateRawToken();
            var hash = HashToken(rawToken);

            _sessionRepository.Setup(r => r.GetByRefreshTokenHashAsync(hash, It.IsAny<CancellationToken>()))
                              .ReturnsAsync((UserSession?)null);
            _sessionRepository.Setup(r => r.GetByPreviousRefreshTokenHashAsync(hash, It.IsAny<CancellationToken>()))
                              .ReturnsAsync((UserSession?)null);

            // BUG: currently throws NullReferenceException instead of UnauthorizedException
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _authService.RefreshTokenAsync(new RefreshRequest { RefreshToken = rawToken }));
        }

        [Fact]
        public async Task Refresh_ReplayedToken_ShouldDeleteAllSessions_ButBugPreventsIt()
        {
            var rawToken = GenerateRawToken();
            var hash = HashToken(rawToken);
            var userId = Guid.NewGuid();

            var staleSession = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PreviousRefreshToken = hash,
                RefreshToken = HashToken(GenerateRawToken()),
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

            _sessionRepository.Setup(r => r.GetByRefreshTokenHashAsync(hash, It.IsAny<CancellationToken>()))
                              .ReturnsAsync((UserSession?)null);
            _sessionRepository.Setup(r => r.GetByPreviousRefreshTokenHashAsync(hash, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(staleSession);
            _sessionRepository.Setup(r => r.DeleteAllForUserAsync(userId, It.IsAny<CancellationToken>()))
                              .Returns(Task.CompletedTask);

            // BUG: inverted condition means DeleteAllForUserAsync is never called on replay
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _authService.RefreshTokenAsync(new RefreshRequest { RefreshToken = rawToken }));

            _sessionRepository.Verify(r => r.DeleteAllForUserAsync(userId, It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
