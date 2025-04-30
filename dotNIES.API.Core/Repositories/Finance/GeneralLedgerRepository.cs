using dotNIES.API.Core.Repositories.Internal;
using dotNIES.Data.Dto.Finance;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Services;

namespace dotNIES.API.Core.Repositories.Finance;

public class GeneralLedgerRepository(IBaseRepository dataRepository,
                                     ILoggerService loggerService,
                                     IUserAppInfoDto userAppInfoDto) : IGeneralLedgerRepository
{
    private readonly IBaseRepository _dataRepository = dataRepository;
    private readonly ILoggerService _loggerService = loggerService;
    private readonly IUserAppInfoDto _userAppInfoDto = userAppInfoDto;

    public async Task<IEnumerable<GeneralLedgerDto>> GetAllAsync()
    {
        try
        {
            var result = await _dataRepository.GetAllAsync<GeneralLedgerDto>("GeneralLedger", "fin");
            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from GeneralLedger", e);
            return [];
        }
    }

    public async Task<IEnumerable<GeneralLedgerDto>> GetNotDeletedGeneralLedgersAsync()
    {
        try
        {
            var sql = "SELECT * FROM fin.GeneralLedger WHERE IsDeleted = 0";
            var result = await _dataRepository.GetDataAsync<GeneralLedgerDto>(sql);

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from GeneralLedger", e);
            return Enumerable.Empty<GeneralLedgerDto>();
        }
    }

    public async Task<IEnumerable<GeneralLedgerDto>> GetDeletedGeneralLedgersAsync()
    {
        try
        {
            var sql = "SELECT * FROM fin.GeneralLedger WHERE IsDeleted = 1";
            var result = await _dataRepository.GetDataAsync<GeneralLedgerDto>(sql);

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from GeneralLedger", e);
            return Enumerable.Empty<GeneralLedgerDto>();
        }
    }

    public async Task<int> Insert(GeneralLedgerDto generalLedgerDto)
    {
        try
        {
            _loggerService.SendDebugInfo("START inserting general ledger");

            if (generalLedgerDto == null)
            {
                _loggerService.SendError("GeneralLedgerDto is null");
                throw new ArgumentNullException(nameof(generalLedgerDto), "GeneralLedger object cannot be null");
            }

            if (generalLedgerDto.Id > 0) generalLedgerDto.Id = 0; // reset the Id to 0

            var result = await _dataRepository.InsertAsync(generalLedgerDto);

            if (result == 0)
            {
                _loggerService.SendError("An exception occurred while inserting the new GeneralLedger");
                _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedgerDto)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed
                return -1;
            }

            _loggerService.SendInformation($"Inserting new GeneralLedger with Id {result}");
            _loggerService.SendDebugInfo("END inserting general ledger");

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while inserting the new GeneralLedger", e);
            _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedgerDto)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed

            return -1;
        }
    }

    public async Task<bool> Update(GeneralLedgerDto generalLedgerDto)
    {
        try
        {
            _loggerService.SendDebugInfo("START updating general ledger");

            if (generalLedgerDto == null)
            {
                _loggerService.SendError("GeneralLedgerDto is null");
                throw new ArgumentNullException(nameof(generalLedgerDto), "GeneralLedger object cannot be null");
            }

            if (generalLedgerDto.Id > 0)
            {
                _loggerService.SendError("GeneralLedgerDto.Id cannot be 0");
                throw new ArgumentOutOfRangeException(nameof(generalLedgerDto), "GeneralLedger Id cannot be zero");
            }

            var result = await _dataRepository.UpdateRecordAsync(generalLedgerDto);

            if (!result)
            {
                _loggerService.SendError("An error occurred while updating the GeneralLedger");
                _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedgerDto)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed

                return false;
            }

            _loggerService.SendDebugInfo("END updating general ledger");

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while updating the GeneralLedger", e);
            _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedgerDto)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed

            return false;
        }
    }

    public async Task<bool> Delete(GeneralLedgerDto generalLedgerDto)
    {
        try
        {
            _loggerService.SendDebugInfo("START delete general ledger");

            if (generalLedgerDto == null)
            {
                _loggerService.SendError("GeneralLedgerDto is null");
                throw new ArgumentNullException(nameof(generalLedgerDto), "GeneralLedger object cannot be null");
            }

            if (generalLedgerDto.Id < 1)
            {
                _loggerService.SendError("GeneralLedgerDto.Id cannot be 0");
                throw new ArgumentOutOfRangeException(nameof(generalLedgerDto), "GeneralLedger Id cannot be zero");
            }

            // Deleting a general ledger is not allowed. We issue a soft delete instead.
            // TODO: Implement a hard delete as well when checking the transaction against the bank.
            var result = await _dataRepository.SoftDeleteRecordAsync(generalLedgerDto);

            if (result == false)
            {
                _loggerService.SendError("An error occurred while deleting the GeneralLedger");
                _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedgerDto)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed

                return false;
            }

            _loggerService.SendDebugInfo("END deleting general ledger");

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while deleting the GeneralLedger", e);
            _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedgerDto)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed

            return false;
        }
    }
}

