using dotNIES.API.Core.Repositories.Internal;
using dotNIES.Data.Dto.Common;
using dotNIES.Data.Logging.Services;

namespace dotNIES.API.Core.Repositories;

public class CategoryRepository(IBaseRepository baseRepository, ILoggerService loggerService)
{
    private readonly IBaseRepository _dataService = baseRepository;
    private readonly ILoggerService _loggerService = loggerService;

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        try
        {
            var result = await _dataService.GetAllAsync<CategoryDto>("Category");
            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from Category", e);
            return [];
        }
    }

    public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
    {
        try
        {
            var sql = "SELECT * FROM Category WHERE IsActive = 1 AND IsDeleted = 0";
            var result = await _dataService.GetDataAsync<CategoryDto>(sql);

            return result;
        }
        catch (Exception e)
        {
            _loggerService.SendError("An exception occurred while getting all records from Category", e);
            return Enumerable.Empty<CategoryDto>();
        }
    }

    [Obsolete("Update method is not implemented yet.", true)]
    public async Task<int> Insert(CategoryDto categoryDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Insert method is not implemented yet.");
    }

    [Obsolete("Update method is not implemented yet.", true)]
    public async Task<bool> Update(CategoryDto categoryDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Update method is not implemented yet.");
    }

    [Obsolete("Update method is not implemented yet.", true)]
    public async Task<bool> Delete(CategoryDto categoryDto)
    {
        await Task.Yield();
        throw new NotImplementedException("Delete method is not implemented yet.");
    }
}
