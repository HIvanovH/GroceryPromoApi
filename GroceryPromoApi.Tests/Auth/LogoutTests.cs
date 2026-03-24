using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Options;
using GroceryPromoApi.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace GroceryPromoApi.Tests.Auth
{
    public class LogoutTests
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IUserSessionRepository> _sessionRepository = new();
        private readonly AuthService _authService;

        public LogoutTests()
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
        public async Task Logout_ValidSession_DeletesSession()
        {
            var sessionId = Guid.NewGuid();

            _sessionRepository.Setup(r => r.DeleteAsync(sessionId, It.IsAny<CancellationToken>()))
                              .Returns(Task.CompletedTask);

            await _authService.LogoutAsync(sessionId);

            _sessionRepository.Verify(r => r.DeleteAsync(sessionId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
