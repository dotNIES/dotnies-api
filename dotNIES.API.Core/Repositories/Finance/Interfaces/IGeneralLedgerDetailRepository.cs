using dotNIES.Data.Dto.Finance;

namespace dotNIES.API.Core.Repositories.Finance;
public interface IGeneralLedgerDetailRepository
{
    Task<bool> Delete(GeneralLedgerDetailDto generalLedgerDetailDto);
    Task<IEnumerable<GeneralLedgerDetailDto>> GetAllAsync();
    Task<IEnumerable<GeneralLedgerDetailDto>> GetDeletedGeneralLedgersAsync();
    Task<IEnumerable<GeneralLedgerDetailDto>> GetNotDeletedGeneralLedgersAsync();
    Task<int> Insert(GeneralLedgerDetailDto generalLedgerDetail);
    Task<bool> Update(GeneralLedgerDetailDto generalLedgerDetail);
}