using dotNIES.Data.Dto.Finance;

namespace dotNIES.API.Core.Repositories.Finance;
public interface IGeneralLedgerRepository
{
    Task<bool> Delete(GeneralLedgerDto GeneralLedgerDto);
    Task<IEnumerable<GeneralLedgerDto>> GetAllAsync();
    Task<IEnumerable<GeneralLedgerDto>> GetDeletedGeneralLedgersAsync();
    Task<GeneralLedgerDto?> GetGeneralLedger(int id);
    Task<IEnumerable<GeneralLedgerDto>> GetNotDeletedGeneralLedgersAsync();
    Task<int> Insert(GeneralLedgerDto generalLedgerDto);
    Task<bool> Update(GeneralLedgerDto GeneralLedgerDto);
}