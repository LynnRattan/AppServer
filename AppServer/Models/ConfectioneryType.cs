using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

public partial class ConfectioneryType
{
    [Key]
    public int ConfectioneryTypeId { get; set; }

    [StringLength(100)]
    public string ConfectioneryTypeName { get; set; } = null!;

    [InverseProperty("ConfectioneryType")]
    public virtual ICollection<Baker> Bakers { get; set; } = new List<Baker>();
}
