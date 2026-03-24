namespace GroceryPromoApi.Application.Interfaces.Services;

public interface ISyncService
{
    Task SyncAsync();

    Task BackfillAsync();
}
