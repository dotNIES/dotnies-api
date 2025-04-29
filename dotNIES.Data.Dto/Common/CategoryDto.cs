using dotNIES.Data.Dto.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotNIES.Data.Dto.Common;

[Table("Category", Schema = "common")]
public class CategoryDto : BaseDto
{
    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;
}

