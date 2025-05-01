using dotNIES.API.Core.Repositories.Internal;
using dotNIES.Data.Dto.Finance;
using dotNIES.Data.Logging.Services;

namespace dotNIES.API.Core.Repositories.Finance;

public class VendorRepository(IBaseRepository dataRepository, ILoggerService loggerService) : IVendorRepository
{
    private readonly IBaseRepository _dataRepository = dataRepository;
    private readonly ILoggerService _loggerService = loggerService;

    public async Task<IEnumerable<VendorDto>> GetAllAsync()
    {
        try
        {
            var result = await _dataRepository.GetAllAsync<VendorDto>("Vendor", "fin");
            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from Vendor", e);
            return [];
        }
    }

    public async Task<IEnumerable<VendorDto>> GetActiveVendorsAsync()
    {
        try
        {
            var sql = "SELECT * FROM fin.Vendor WHERE IsActive = 1 AND IsDeleted = 0";
            var result = await _dataRepository.GetDataAsync<VendorDto>(sql);

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from Vendor", e);
            return Enumerable.Empty<VendorDto>();
        }
    }

    [Obsolete("Update method is not implemented yet.", true)]
    public async Task<int> Insert(VendorDto VendorDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Insert method is not implemented yet.");
    }

    [Obsolete("Update method is not implemented yet.", true)]
    public async Task<bool> Update(VendorDto VendorDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Update method is not implemented yet.");
    }

    [Obsolete("Update method is not implemented yet.", true)]
    public async Task<bool> Delete(VendorDto VendorDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Delete method is not implemented yet.");
    }
}

