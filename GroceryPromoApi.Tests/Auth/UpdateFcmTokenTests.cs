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
    public class UpdateFcmTokenTests
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IUserSessionRepository> _sessionRepository = new();
        private readonly AuthService _authService;

        public UpdateFcmTokenTests()
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
        public async Task UpdateFcmToken_ValidSession_UpdatesToken()
        {
            var sessionId = Guid.NewGuid();
            var session = new UserSession { Id = sessionId, UserId = Guid.NewGuid() };
            UserSession? capturedSession = null;

            _sessionRepository.Setup(r => r.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(session);
            _sessionRepository.Setup(r => r.UpdateAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                              .Callback<UserSession, CancellationToken>((s, _) => capturedSession = s)
                              .Returns(Task.CompletedTask);

            await _authService.UpdateFcmTokenAsync(sessionId, "new-fcm-token");

            Assert.Equal("new-fcm-token", capturedSession?.FcmToken);
        }

        [Fact]
        public async Task UpdateFcmToken_SessionNotFound_ThrowsNotFoundException()
        {
            var sessionId = Guid.NewGuid();

            _sessionRepository.Setup(r => r.GetByIdAsync(sessionId, It.IsAny<CancellationToken>()))
                              .ReturnsAsync((UserSession?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _authService.UpdateFcmTokenAsync(sessionId, "some-fcm-token"));
        }
    }
}
