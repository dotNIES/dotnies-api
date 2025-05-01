using dotNIES.Data.Dto.Finance;

namespace dotNIES.API.Core.Repositories.Finance;
public interface IPurchaseTypeRepository
{
    Task<bool> Delete(PurchaseTypeDto PurchaseTypeDto);
    Task<IEnumerable<PurchaseTypeDto>> GetActivePurchaseTypesAsync();
    Task<IEnumerable<PurchaseTypeDto>> GetAllAsync();
    Task<PurchaseTypeDto?> GetPurchaseTypeByIdAsync(int id);
    Task<int> Insert(PurchaseTypeDto PurchaseTypeDto);
    Task<bool> Update(PurchaseTypeDto PurchaseTypeDto);
}