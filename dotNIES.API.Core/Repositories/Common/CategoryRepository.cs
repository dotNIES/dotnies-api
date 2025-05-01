using dotNIES.API.Core.Repositories.Internal;
using dotNIES.Data.Dto.Common;
using dotNIES.Data.Logging.Services;

namespace dotNIES.API.Core.Repositories.Common;

public class CategoryRepository(IBaseRepository dataRepository, ILoggerService loggerService) : ICategoryRepository
{
    private readonly IBaseRepository _dataRepository = dataRepository;
    private readonly ILoggerService _loggerService = loggerService;

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        try
        {
            var result = await _dataRepository.GetAllAsync<CategoryDto>("Category", "common");
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
            var sql = "SELECT " +
                "   * " +
                "FROM " +
                "   common.Category " +
                "WHERE " +
                "   IsActive = 1 " +
                "   AND IsDeleted = 0";

            var result = await _dataRepository.GetDataAsync<CategoryDto>(sql);

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
