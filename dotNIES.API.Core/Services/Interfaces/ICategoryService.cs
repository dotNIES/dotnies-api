using dotNIES.Data.Dto.Common;

namespace dotNIES.API.Core.Services;
public interface ICategoryService
{
    Task<bool> Delete(CategoryDto categoryDto);

    /// <summary>
    /// Return all records that are active and not (soft)deleted from the Category table.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();

    /// <summary>
    /// Get all records from the Category table.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<CategoryDto>> GetAllAsync();

    Task<int> Insert(CategoryDto categoryDto);

    Task<bool> Update(CategoryDto categoryDto);
}