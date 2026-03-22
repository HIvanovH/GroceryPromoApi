using GroceryPromoApi.Application.Interfaces.Repositories;
using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Infrastructure.Database.Repositories;

public class NotificationRepository : INotificationRepository
{
    public Task AddAsync(Notification notification) => throw new NotImplementedException();
    public Task UpdateAsync(Notification notification) => throw new NotImplementedException();
    public Task DeleteAsync(Notification notification) => throw new NotImplementedException();
}
