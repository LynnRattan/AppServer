using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

public partial class DessertType
{
    [Key]
    public int DessertTypeId { get; set; }

    [StringLength(100)]
    public string DessertTypeName { get; set; } = null!;

    [InverseProperty("DessertType")]
    public virtual ICollection<Dessert> Desserts { get; set; } = new List<Dessert>();
}
