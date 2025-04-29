using CommunityToolkit.Mvvm.Messaging;
using dotNIES.API.Core.Helpers;
using dotNIES.Data.Dto.Common;
using dotNIES.Data.Logging.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNIES.API.Core.Services;

public class CategoryService(IBaseDataService baseDataService) : ICategoryService
{
    private readonly IBaseDataService _baseDataService = baseDataService;

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        try
        {
            var result = await _baseDataService.GetAllAsync<CategoryDto>("Category");
            return result;
        }
        catch (Exception e)
        {
            var logMessage = new LogMessageModel
            {
                Message = $"An exception occurred while getting all records from Category",
                LogLevel = LogLevel.Information,
            };
            WeakReferenceMessenger.Default.Send(logMessage);
            return Enumerable.Empty<CategoryDto>();
        }
    }

    public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
    {
        try
        {
            var sql = "SELECT * FROM Category WHERE IsActive = 1 AND IsDeleted = 0";
            var result = await _baseDataService.GetDataAsync<CategoryDto>(sql);

            return result;
        }
        catch (Exception e)
        {
            var logMessage = new LogMessageModel
            {
                Message = $"An exception occurred while getting the active records from Category",
                LogLevel = LogLevel.Information,
                Exception = e
            };
            WeakReferenceMessenger.Default.Send(logMessage);
            return Enumerable.Empty<CategoryDto>();
        }
    }

    public async Task<int> Insert(CategoryDto categoryDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Insert method is not implemented yet.");
    }

    public async Task<bool> Update(CategoryDto categoryDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Update method is not implemented yet.");
    }

    public async Task<bool> Delete(CategoryDto categoryDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Delete method is not implemented yet.");
    }
}

