using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;
using GroceryPromoApi.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class UserSessionRepository : IUserSessionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserSessionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserSessions.FindAsync([sessionId], cancellationToken);
    }

    public async Task<UserSession?> GetByRefreshTokenHashAsync(string hash, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserSessions.FirstOrDefaultAsync(s => s.RefreshToken == hash, cancellationToken);
    }

    public async Task<UserSession?> GetByPreviousRefreshTokenHashAsync(string hash, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserSessions.FirstOrDefaultAsync(s => s.PreviousRefreshToken == hash, cancellationToken);
    }

    public async Task AddAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserSessions.AddAsync(session, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        _dbContext.UserSessions.Update(session);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.UserSessions.FindAsync([sessionId], cancellationToken);
        if (session is not null)
        {
            _dbContext.UserSessions.Remove(session);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.UserSessions
            .Where(s => s.UserId == userId)
            .ToListAsync(cancellationToken);

        _dbContext.UserSessions.RemoveRange(sessions);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
