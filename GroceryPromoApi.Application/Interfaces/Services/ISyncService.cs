namespace GroceryPromoApi.Application.Interfaces.Services;

public interface ISyncService
{
    Task SyncAsync(CancellationToken cancellationToken = default);
}
