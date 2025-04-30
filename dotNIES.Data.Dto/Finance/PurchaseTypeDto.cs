using dotNIES.Data.Dto.Internal;
using System.ComponentModel.DataAnnotations;

namespace dotNIES.Data.Dto.Finance;

public class PurchaseTypeDto : BaseDto
{
    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;
}

