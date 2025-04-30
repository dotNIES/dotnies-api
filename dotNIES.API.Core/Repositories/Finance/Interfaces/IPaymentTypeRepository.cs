using dotNIES.Data.Dto.Finance;

namespace dotNIES.API.Core.Repositories.Finance;
public interface IPaymentTypeRepository
{
    Task<bool> Delete(PaymentTypeDto PaymentTypeDto);
    Task<IEnumerable<PaymentTypeDto>> GetActivePaymentTypesAsync();
    Task<IEnumerable<PaymentTypeDto>> GetAllAsync();
    Task<int> Insert(PaymentTypeDto PaymentTypeDto);
    Task<bool> Update(PaymentTypeDto PaymentTypeDto);
}