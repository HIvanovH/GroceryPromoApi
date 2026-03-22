using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class UserRepository : IUserRepository
{
    public Task AddAsync(User user) => throw new NotImplementedException();
    public Task UpdateAsync(User user) => throw new NotImplementedException();
    public Task DeleteAsync(User user) => throw new NotImplementedException();
}
