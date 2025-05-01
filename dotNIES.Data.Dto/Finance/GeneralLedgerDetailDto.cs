using dotNIES.Data.Dto.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNIES.Data.Dto.Finance;

public class GeneralLedgerDetailDto : BaseDto
{
    [Required]
    public int GeneralLedgerId { get; set; }

    [Required]
    public int PurchaseTypeId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public DateTime EntryDate { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public decimal DiscountAmt { get; set; }

    [Required]
    public decimal TaxRate { get; set; }

    [Required]
    public decimal TaxAmt { get; set; }

    [Required]
    public decimal TotalAmt { get; set; }

    [Required]
    public bool Debit { get; set; }

    [MaxLength(100)]
    public string? Description { get; set; }
}

