using dotNIES.Data.Dto.Finance;

namespace dotNIES.API.Core.Repositories;
public interface IVendorRepository
{
    Task<bool> Delete(VendorDto VendorDto);
    Task<IEnumerable<VendorDto>> GetActiveVendorsAsync();
    Task<IEnumerable<VendorDto>> GetAllAsync();
    Task<int> Insert(VendorDto VendorDto);
    Task<bool> Update(VendorDto VendorDto);
}