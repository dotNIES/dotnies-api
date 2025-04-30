using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNIES.Data.Dto.Internal;

/// <summary>
/// This class contains 2 models one for a INT primary key and one for a GUID primary key
/// </summary>

public class BaseDto
{
    [Required]
    [Key]
    public int Id { get; set; }

    [Required]
    public string CreatedBy { get; set; } = string.Empty;

    [Required] 
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    [Required] 
    public string LastModifiedBy { get; set; } = string.Empty;

    [Required] 
    public DateTime LastModifiedOn { get; set; } = DateTime.Now;

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
}

public class BaseGuidDto
{
    [Required]
    [Key]
    public Guid Id { get; set; }

    [Required] 
    public string CreatedBy { get; set; } = string.Empty;

    [Required] 
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    [Required] 
    public string LastModifiedBy { get; set; } = string.Empty;

    [Required]  
    public DateTime LastModifiedOn { get; set; } = DateTime.Now;

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
}