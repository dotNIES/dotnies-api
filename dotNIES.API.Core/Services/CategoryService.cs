using dotNIES.Data.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNIES.API.Core.Services;
public class CategoryService(IBaseDataService baseDataService)
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
            throw;
        }
    }
}

