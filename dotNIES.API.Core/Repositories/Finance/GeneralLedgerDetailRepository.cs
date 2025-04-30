using dotNIES.API.Core.Repositories.Internal;
using dotNIES.Data.Dto.Finance;
using dotNIES.Data.Logging.Services;

namespace dotNIES.API.Core.Repositories.Finance;
public class GeneralLedgerDetailRepository(IBaseRepository dataRepository,
                                           ILoggerService loggerService) : IGeneralLedgerDetailRepository
{
    private readonly IBaseRepository _dataRepository = dataRepository;
    private readonly ILoggerService _loggerService = loggerService;

    public async Task<IEnumerable<GeneralLedgerDetailDto>> GetAllAsync()
    {
        try
        {
            var result = await _dataRepository.GetAllAsync<GeneralLedgerDetailDto>("GeneralLedgerDetail", "fin");
            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from GeneralLedgerDetail", e);
            return [];
        }
    }

    public async Task<IEnumerable<GeneralLedgerDetailDto>> GetNotDeletedGeneralLedgersAsync()
    {
        try
        {
            var sql = "SELECT * FROM fin.GeneralLedgerDetail WHERE IsDeleted = 0";
            var result = await _dataRepository.GetDataAsync<GeneralLedgerDetailDto>(sql);

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting undeleted records from GeneralLedgerDetail", e);
            return Enumerable.Empty<GeneralLedgerDetailDto>();
        }
    }

    public async Task<IEnumerable<GeneralLedgerDetailDto>> GetDeletedGeneralLedgersAsync()
    {
        try
        {
            var sql = "SELECT * FROM fin.GeneralLedgerDetail WHERE IsDeleted = 1";
            var result = await _dataRepository.GetDataAsync<GeneralLedgerDetailDto>(sql);

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting deleted records from GeneralLedgerDetail", e);
            return Enumerable.Empty<GeneralLedgerDetailDto>();
        }
    }

    public async Task<int> Insert(GeneralLedgerDetailDto generalLedgerDetail)
    {
        try
        {
            _loggerService.SendDebugInfo("START inserting general ledger detail");

            if (generalLedgerDetail == null)
            {
                _loggerService.SendError("GeneralLedgerDetail is null");
                throw new ArgumentNullException(nameof(generalLedgerDetail), "GeneralLedgerDetail object cannot be null");
            }

            if (generalLedgerDetail.GeneralLedgerId < 1)
            {
                _loggerService.SendError("GeneralLedgerId cannot be 0");
                throw new ArgumentOutOfRangeException(nameof(generalLedgerDetail), "GeneralLedgerId cannot be 0");
            }

            if (generalLedgerDetail.Id > 0) generalLedgerDetail.Id = 0; // reset the Id to 0

            var result = await _dataRepository.InsertAsync(generalLedgerDetail);

            if (result == 0)
            {
                _loggerService.SendError("An exception occurred while inserting the new GeneralLedgerDetail");
                _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedgerDetail)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed
                return -1;
            }

            _loggerService.SendInformation($"Inserting new GeneralLedgerDetail with Id {result}");
            _loggerService.SendDebugInfo("END inserting general ledger detail");

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while inserting the new GeneralLedgerDetail", e);
            _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedgerDetail)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed

            return -1;
        }
    }

    public async Task<bool> Update(GeneralLedgerDetailDto generalLedgerDetail)
    {
        try
        {
            _loggerService.SendDebugInfo("START inserting general ledger detail");

            if (generalLedgerDetail == null)
            {
                _loggerService.SendError("GeneralLedgerDetail is null");
                throw new ArgumentNullException(nameof(generalLedgerDetail), "generalLedgerDetail object cannot be null");
            }

            if (generalLedgerDetail.GeneralLedgerId < 1)
            {
                _loggerService.SendError("GeneralLedgerId cannot be 0");
                throw new ArgumentOutOfRangeException(nameof(generalLedgerDetail), "GeneralLedgerId cannot be 0");
            }

            if (generalLedgerDetail.Id < 1)
            {
                _loggerService.SendError("GeneralLedgerDetailId cannot be 0");
                throw new ArgumentOutOfRangeException(nameof(generalLedgerDetail), "GeneralLedgerDetailId cannot be 0");
            }

            var result = await _dataRepository.UpdateRecordAsync(generalLedgerDetail);

            if (!result)
            {
                _loggerService.SendError("An exception occurred while inserting the new GeneralLedgerDetail");
                _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedgerDetail)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed
                return false;
            }

            _loggerService.SendInformation($"Updating new GeneralLedgerDetail with Id {result}");
            _loggerService.SendDebugInfo("END updating general ledger detail");

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while inserting the new GeneralLedgerDetail", e);
            _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedgerDetail)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed

            return false;
        }
    }

    public async Task<bool> Delete(GeneralLedgerDetailDto generalLedgerDetailDto)
    {
        try
        {
            _loggerService.SendDebugInfo("START delete general ledger detail");

            if (generalLedgerDetailDto == null)
            {
                _loggerService.SendError("generalLedgerDetailDto is null");
                throw new ArgumentNullException(nameof(generalLedgerDetailDto), "generalLedgerDetailDto object cannot be null");
            }

            if (generalLedgerDetailDto.Id < 1)
            {
                _loggerService.SendError("GeneralLedgerDto.Id cannot be 0");
                throw new ArgumentOutOfRangeException(nameof(generalLedgerDetailDto), "generalLedgerDetailDto Id cannot be zero");
            }

            // Deleting a general ledger is not allowed. We issue a soft delete instead.
            // TODO: Implement a hard delete as well when checking the transaction against the bank.
            var result = await _dataRepository.SoftDeleteRecordAsync(generalLedgerDetailDto);

            if (result == false)
            {
                _loggerService.SendError("An error occurred while deleting the generalLedgerDetailDto");
                _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedgerDetailDto)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed

                return false;
            }

            _loggerService.SendDebugInfo("END deleting general ledger detail");

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while deleting the generalLedgerDetailDto", e);
            _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedgerDetailDto)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed

            return false;
        }
    }
}