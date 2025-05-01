using dotNIES.Data.Dto.Finance;

namespace dotNIES.API.Core.Repositories.Finance;
public interface IGeneralLedgerDetailRepository
{
    Task<bool> Delete(int generalLedgerDetailId);
    Task<IEnumerable<GeneralLedgerDetailDto>> GetAllAsync();
    Task<IEnumerable<GeneralLedgerDetailDto>> GetDeletedGeneralLedgerDetailsAsync();
    Task<GeneralLedgerDetailDto?> GetGeneralLedgerDetail(int id);
    Task<IEnumerable<GeneralLedgerDetailDto>?> GetGeneralLedgerDetailsForGL(int generalLedgerId);
    Task<IEnumerable<GeneralLedgerDetailDto>> GetNotDeletedGeneralLedgerDetailsAsync();
    Task<int> Insert(GeneralLedgerDetailDto generalLedgerDetail);
    Task<bool> Update(GeneralLedgerDetailDto generalLedgerDetail);
}