using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

[PrimaryKey("OrderId", "DessertId")]
public partial class OrderedDessert
{
    [Key]
    public int OrderId { get; set; }

    [Key]
    public int DessertId { get; set; }

    public int? StatusCode { get; set; }

    public int Quantity { get; set; }

    public double? Price { get; set; }

    [ForeignKey("DessertId")]
    [InverseProperty("OrderedDesserts")]
    public virtual Dessert Dessert { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("OrderedDesserts")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("StatusCode")]
    [InverseProperty("OrderedDesserts")]
    public virtual Status? StatusCodeNavigation { get; set; }
}
