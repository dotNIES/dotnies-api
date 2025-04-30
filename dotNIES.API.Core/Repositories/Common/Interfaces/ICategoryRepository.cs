using dotNIES.Data.Dto.Common;

namespace dotNIES.API.Core.Repositories;
public interface ICategoryRepository
{
    Task<bool> Delete(CategoryDto categoryDto);
    Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<int> Insert(CategoryDto categoryDto);
    Task<bool> Update(CategoryDto categoryDto);
}