namespace GroceryPromoApi.Application.Interfaces.Services;

public interface IProductService
{
    Task SearchAsync();
    Task GetByIdAsync();
}
