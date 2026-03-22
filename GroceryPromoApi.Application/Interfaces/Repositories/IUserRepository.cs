using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}
