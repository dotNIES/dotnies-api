using dotNIES.API.Core.Repositories.Internal;
using dotNIES.Data.Dto.Finance;
using dotNIES.Data.Logging.Services;

namespace dotNIES.API.Core.Repositories.Finance;
public class PaymentTypeRepository(IBaseRepository dataRepository, ILoggerService loggerService) : IPaymentTypeRepository
{
    private readonly IBaseRepository _dataRepository = dataRepository;
    private readonly ILoggerService _loggerService = loggerService;

    public async Task<IEnumerable<PaymentTypeDto>> GetAllAsync()
    {
        try
        {
            var result = await _dataRepository.GetAllAsync<PaymentTypeDto>("PaymentType", "fin");
            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from PaymentType", e);
            return [];
        }
    }

    public async Task<IEnumerable<PaymentTypeDto>> GetActivePaymentTypesAsync()
    {
        try
        {
            var sql = "SELECT * FROM fin.PaymentType WHERE IsActive = 1";
            var result = await _dataRepository.GetDataAsync<PaymentTypeDto>(sql);

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from PaymentType", e);
            return Enumerable.Empty<PaymentTypeDto>();
        }
    }

    [Obsolete("Update method is not implemented yet.", true)]
    public async Task<int> Insert(PaymentTypeDto PaymentTypeDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Insert method is not implemented yet.");
    }

    [Obsolete("Update method is not implemented yet.", true)]
    public async Task<bool> Update(PaymentTypeDto PaymentTypeDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Update method is not implemented yet.");
    }

    [Obsolete("Update method is not implemented yet.", true)]
    public async Task<bool> Delete(PaymentTypeDto PaymentTypeDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Delete method is not implemented yet.");
    }
}