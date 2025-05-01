using dotNIES.Data.Dto.Finance;

namespace dotNIES.API.Core.Repositories.Finance;
public interface IGeneralLedgerRepository
{
    /// <summary>
    /// Deletes the general ledger and all its details
    /// </summary>
    /// <remarks>
    /// This will not delete the general ledger and details from the database, 
    /// but will mark it as deleted. Call the 'Remove' method to remove it from the database.
    /// </remarks>
    /// <param name="GeneralLedgerId"></param>
    /// <returns></returns>
    Task<bool> Delete(int GeneralLedgerId);

    /// <summary>
    /// Gets all general ledgers
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<GeneralLedgerDto>> GetAllAsync();

    /// <summary>
    /// Gets all general ledgers that are marked as deleted
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<GeneralLedgerDto>> GetDeletedGeneralLedgersAsync();

    /// <summary>
    /// Gets one specific general ledger
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<GeneralLedgerDto?> GetGeneralLedger(int id);

    /// <summary>
    /// Gets all general ledgers that are not marked as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IEnumerable<GeneralLedgerDto>> GetNotDeletedGeneralLedgersAsync();

    /// <summary>
    /// Inserts a new general ledger
    /// </summary>
    /// <param name="generalLedgerDto"></param>
    /// <returns></returns>
    Task<int> Insert(GeneralLedgerDto generalLedgerDto);

    /// <summary>
    /// Updates the general ledger
    /// </summary>
    /// <param name="GeneralLedgerDto"></param>
    /// <returns></returns>
    Task<bool> Update(GeneralLedgerDto GeneralLedgerDto);
}