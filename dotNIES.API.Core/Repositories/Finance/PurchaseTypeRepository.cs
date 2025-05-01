using dotNIES.API.Core.Repositories.Internal;
using dotNIES.Data.Dto.Finance;
using dotNIES.Data.Logging.Services;

namespace dotNIES.API.Core.Repositories.Finance;
public class PurchaseTypeRepository(IBaseRepository dataRepository, ILoggerService loggerService) : IPurchaseTypeRepository
{
    private readonly IBaseRepository _dataRepository = dataRepository;
    private readonly ILoggerService _loggerService = loggerService;

    public async Task<IEnumerable<PurchaseTypeDto>> GetAllAsync()
    {
        try
        {
            var result = await _dataRepository.GetAllAsync<PurchaseTypeDto>("PurchaseType", "fin");
            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from PurchaseType", e);
            return [];
        }
    }

    public async Task<IEnumerable<PurchaseTypeDto>> GetActivePurchaseTypesAsync()
    {
        try
        {
            var sql = "SELECT * FROM fin.PurchaseType WHERE IsActive = 1";
            var result = await _dataRepository.GetDataAsync<PurchaseTypeDto>(sql);

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from PurchaseType", e);
            return Enumerable.Empty<PurchaseTypeDto>();
        }
    }

    public async Task<PurchaseTypeDto?> GetPurchaseTypeByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                _loggerService.SendError($"Invalid PurchaseType Id: {id}");
                return null;
            }

            var sql = $"SELECT * FROM fin.PurchaseType WHERE Id = {id}";
            var result = await _dataRepository.GetRecordAsync<PurchaseTypeDto>(sql);

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError($"An exception occurred while getting PurchaseType with Id {id}", e);
            return null;
        }
    }

    [Obsolete("Update method is not implemented yet.", true)]
    public async Task<int> Insert(PurchaseTypeDto PurchaseTypeDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Insert method is not implemented yet.");
    }

    [Obsolete("Update method is not implemented yet.", true)]
    public async Task<bool> Update(PurchaseTypeDto PurchaseTypeDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Update method is not implemented yet.");
    }

    [Obsolete("Update method is not implemented yet.", true)]
    public async Task<bool> Delete(PurchaseTypeDto PurchaseTypeDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Delete method is not implemented yet.");
    }
}

