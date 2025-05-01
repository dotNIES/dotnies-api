using dotNIES.Data.Dto.Internal;
using System.ComponentModel.DataAnnotations;

namespace dotNIES.Data.Dto.Finance;

public class GeneralLedgerDto : BaseDto
{
    [Required] 
    public int VendorId { get; set; }

    [Required] 
    public int PaymentTypeId { get; set; }
    
    [Required] 
    public DateTime EntryDate { get; set; } = DateTime.Now;

    [Required] 
    public decimal Amount { get; set; }

    [Required] 
    public decimal DiscountAmt { get; set; }

    [Required]
    public bool Debit { get; set; }

    [MaxLength(100)]
    public string? Description { get; set; }
}