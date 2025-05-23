﻿using dotNIES.API.Core.Repositories.Internal;
using dotNIES.Data.Dto.Finance;
using dotNIES.Data.Dto.Internal;
using dotNIES.Data.Logging.Services;
using System.Data;

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

    public async Task<GeneralLedgerDto?> GetGeneralLedger(int id)
    {
        try
        {
            var sql = $"SELECT TOP(1) * FROM fin.GeneralLedger WHERE Id = {id}";
            var result = await _dataRepository.GetRecordAsync<GeneralLedgerDto>(sql);

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from GeneralLedger", e);
            return null;
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

            if (generalLedgerDto.Id > 0)
            {
                _loggerService.SendError("GeneralLedger Id is not 0, inserting valid Id not allowed");
                throw new ArgumentOutOfRangeException(nameof(generalLedgerDto), "GeneralLedger Id is not 0, inserting valid Id not allowed");
            }

            var result = await _dataRepository.InsertAsync(generalLedgerDto); // db call

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

    public async Task<bool> Update(GeneralLedgerDto generalLedger)
    {
        try
        {
            _loggerService.SendDebugInfo("START updating general ledger");

            if (generalLedger == null)
            {
                _loggerService.SendError("GeneralLedgerDto is null");
                throw new ArgumentNullException(nameof(generalLedger), "GeneralLedger object cannot be null");
            }

            if (generalLedger.Id < 1)
            {
                _loggerService.SendError("GeneralLedgerDto.Id cannot be 0");
                throw new ArgumentOutOfRangeException(nameof(generalLedger), "GeneralLedger Id cannot be zero");
            }

            var result = await _dataRepository.UpdateRecordAsync(generalLedger);

            if (!result)
            {
                _loggerService.SendError("An error occurred while updating the GeneralLedger");
                _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedger)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed

                return false;
            }

            _loggerService.SendDebugInfo("END updating general ledger");

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while updating the GeneralLedger", e);
            _loggerService.SendError($"Record: {System.Text.Json.JsonSerializer.Serialize(generalLedger)}"); // we do not care if the option 'log entire record is set' we want to see the record that failed

            return false;
        }
    }

    public async Task<bool> Delete(int generalLedgerId)
    {
        IDbTransaction transaction;
        // start the transaction
        transaction = await _dataRepository.StartTransactionAsync();

        try
        {
            _loggerService.SendDebugInfo("START delete general ledger");

            if (generalLedgerId < 1)
            {
                _loggerService.SendError("GeneralLedgerDto.Id cannot be 0");
                throw new ArgumentOutOfRangeException(nameof(generalLedgerId), "GeneralLedger Id cannot be zero");
            }

            // start the transaction
            transaction = await _dataRepository.StartTransactionAsync();

            var query = "SELECT * FROM fin.GeneralLedger WHERE Id = @Id";
            var generalLedger = await _dataRepository.QueryFirstOrDefaultAsync<GeneralLedgerDetailDto>(query, new { Id = generalLedgerId }, transaction);

            if (generalLedger == null)
            {
                _loggerService.SendError($"GeneralLedger with Id {generalLedgerId} not found");
                await _dataRepository.RollbackTransactionAsync(transaction);
                return false;
            }

            // get the details, delete them first then delete this record and close the transaction
            query = "SELECT * FROM fin.GeneralLedgerDetail WHERE GeneralLedgerId = @GeneralLedgerId";
            var generalLedgerDetails = await _dataRepository.QueryAsync<GeneralLedgerDetailDto>(query, new { GeneralLedgerId = generalLedgerId }, transaction);

            if (generalLedgerDetails != null)
            {
                foreach (var detail in generalLedgerDetails)
                {
                    var resultDetails = await _dataRepository.SoftDeleteRecordAsync(detail, transaction);
                    if (!resultDetails)
                    {
                        _loggerService.SendError($"An error occurred while deleting the GeneralLedgerDetail with Id {detail.Id}");
                        // rollback the transaction
                        await _dataRepository.RollbackTransactionAsync(transaction);
                        return false;
                    }
                }
            }

            var result = await _dataRepository.SoftDeleteRecordAsync(generalLedger, transaction);

            if (result == false)
            {
                _loggerService.SendError($"An error occurred while deleting the GeneralLedger with Id {generalLedgerId}");
                // rollback the transaction
                await _dataRepository.RollbackTransactionAsync(transaction);
                return false;
            }

            // commit the transaction
            await _dataRepository.CommitTransactionAsync(transaction);

            _loggerService.SendDebugInfo("END deleting general ledger");

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while deleting the GeneralLedger", e);

            // rollback the transaction
            await _dataRepository.RollbackTransactionAsync(transaction);

            return false;
        }
    }
}

