using dotNIES.Data.Dto.Internal;
using System.ComponentModel.DataAnnotations;

namespace dotNIES.Data.Dto.Finance;

public class VendorDto : BaseDto
{
    [Required]
    public int VendorTypeId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(20)]
    public string? ZipCode { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? BankAccount { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }
}

