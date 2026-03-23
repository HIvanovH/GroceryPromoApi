using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Interfaces.Repositories;

public interface IUserSessionRepository
{
    Task<UserSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);

    Task<UserSession?> GetByRefreshTokenHashAsync(string hash, CancellationToken cancellationToken = default);

    Task<UserSession?> GetByPreviousRefreshTokenHashAsync(string hash, CancellationToken cancellationToken = default);

    Task AddAsync(UserSession session, CancellationToken cancellationToken = default);

    Task UpdateAsync(UserSession session, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid sessionId, CancellationToken cancellationToken = default);

    Task DeleteAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
