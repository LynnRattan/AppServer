using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

public partial class OrderedDessert
{
    [Key]
    public int OrderedDessertId { get; set; }

    public int? OrderId { get; set; }

    public int? DessertId { get; set; }

    public int? StatusCode { get; set; }

    public int? UserId { get; set; }

    public int? BakerId { get; set; }

    public int Quantity { get; set; }

    public double? Price { get; set; }

    [StringLength(100)]
    public string OrderedDessertImage { get; set; } = null!;

    [ForeignKey("BakerId")]
    [InverseProperty("OrderedDesserts")]
    public virtual Baker? Baker { get; set; }

    [ForeignKey("DessertId")]
    [InverseProperty("OrderedDesserts")]
    public virtual Dessert? Dessert { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderedDesserts")]
    public virtual Order? Order { get; set; }

    [ForeignKey("StatusCode")]
    [InverseProperty("OrderedDesserts")]
    public virtual Status? StatusCodeNavigation { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("OrderedDesserts")]
    public virtual User? User { get; set; }
}
