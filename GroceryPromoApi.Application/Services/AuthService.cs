using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GroceryPromoApi.Application.DTOs.Auth;
using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Application.Interfaces.Services;
using GroceryPromoApi.Application.Options;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Domain.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GroceryPromoApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserSessionRepository _sessionRepository;
    private readonly JwtOptions _jwt;

    public AuthService(IUserRepository userRepository, IUserSessionRepository sessionRepo, IOptions<JwtOptions> jwt)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepo;
        _jwt = jwt.Value;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLower();

        var existing = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
            throw new ConflictException("An account with this email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);

        return await CreateSessionAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLower();

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (user.LockoutUntil.HasValue && user.LockoutUntil > DateTime.UtcNow)
            throw new UnauthorizedException($"Account is locked until {user.LockoutUntil.Value:HH:mm} UTC.");

        if (user.PasswordHash == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutUntil = DateTime.UtcNow.AddMinutes(15);
                user.FailedLoginAttempts = 0;
            }

            await _userRepository.UpdateAsync(user, cancellationToken);
            throw new UnauthorizedException("Invalid email or password.");
        }

        user.FailedLoginAttempts = 0;
        user.LockoutUntil = null;
        await _userRepository.UpdateAsync(user, cancellationToken);

        return await CreateSessionAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshRequest request, CancellationToken cancellationToken = default)
    {
        var incomingHash = HashToken(request.RefreshToken);

        var session = await _sessionRepository.GetByRefreshTokenHashAsync(incomingHash, cancellationToken);

        if (session == null)
        {
            var staleSession = await _sessionRepository.GetByPreviousRefreshTokenHashAsync(incomingHash, cancellationToken);
            if (staleSession == null)
                await _sessionRepository.DeleteAllForUserAsync(staleSession.UserId, cancellationToken);

            throw new UnauthorizedException("Invalid or expired refresh token. Please log in again.");
        }

        if (session.ExpiresAt < DateTime.UtcNow)
        {
            await _sessionRepository.DeleteAsync(session.Id, cancellationToken);
            throw new UnauthorizedException("Refresh token has expired. Please log in again.");
        }

        var newRawToken = GenerateRawRefreshToken();
        var newHash = HashToken(newRawToken);

        session.PreviousRefreshToken = session.RefreshToken;
        session.RefreshToken = newHash;
        session.LastUsedAt = DateTime.UtcNow;
        session.ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpirationDays);

        await _sessionRepository.UpdateAsync(session, cancellationToken);

        var user = await _userRepository.GetByIdAsync(session.UserId, cancellationToken)
            ?? throw new UnauthorizedException("User not found.");

        return new AuthResponse
        {
            AccessToken = GenerateJwt(user, session.Id),
            RefreshToken = newRawToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes)
        };
    }

    public async Task LogoutAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        await _sessionRepository.DeleteAsync(sessionId, cancellationToken);
    }

    public async Task UpdateFcmTokenAsync(Guid sessionId, string fcmToken, CancellationToken ct = default)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, ct)
            ?? throw new NotFoundException("Session not found.");

        session.FcmToken = fcmToken;
        await _sessionRepository.UpdateAsync(session, ct);
    }

    private async Task<AuthResponse> CreateSessionAsync(User user, CancellationToken cancellationToken)
    {
        var rawToken = GenerateRawRefreshToken();
        var sessionId = Guid.NewGuid();

        var session = new UserSession
        {
            Id = sessionId,
            UserId = user.Id,
            RefreshToken = HashToken(rawToken),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpirationDays),
            LastUsedAt = DateTime.UtcNow
        };

        await _sessionRepository.AddAsync(session, cancellationToken);

        return new AuthResponse
        {
            AccessToken = GenerateJwt(user, sessionId),
            RefreshToken = rawToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes)
        };
    }

    private string GenerateJwt(User user, Guid sessionId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("sessionId", sessionId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRawRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLower();
    }
}
