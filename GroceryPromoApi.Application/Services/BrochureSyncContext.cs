using GroceryPromoApi.Domain.Entities;

namespace GroceryPromoApi.Application.Services;

internal record BrochureSyncContext(
    Dictionary<string, Brochure> ActiveBrochuresByCode,
    int StartPage,
    HashSet<int> SeenExternalIds
);
