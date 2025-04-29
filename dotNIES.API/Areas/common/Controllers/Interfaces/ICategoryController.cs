using dotNIES.Data.Dto.Common;
using Microsoft.AspNetCore.Mvc;

namespace dotNIES.API.Areas.common.Controllers;
public interface ICategoryController
{
    /// <summary>
    /// Gets all active categories.
    /// </summary>
    /// <remarks>
    /// An active record is: IsActive = 1 and IsDeleted = 0
    /// </remarks>
    /// <returns></returns>
    Task<ActionResult<IEnumerable<CategoryDto>>> GetActiveCategories();

    /// <summary>
    /// Returns ALL records from the Category table.
    /// </summary>
    /// <returns></returns>
    Task<ActionResult<IEnumerable<CategoryDto>>> GetAll();
}