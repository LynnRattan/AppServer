using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

public partial class Dessert
{
    [Key]
    public int DessertId { get; set; }

    [StringLength(100)]
    public string? DessertName { get; set; }

    public int? BakerId { get; set; }

    public int? DessertTypeId { get; set; }

    public int? StatusCode { get; set; }

    public double? Price { get; set; }

    public byte[]? DessertImage { get; set; }

    [ForeignKey("BakerId")]
    [InverseProperty("Desserts")]
    public virtual Baker? Baker { get; set; }

    [ForeignKey("DessertTypeId")]
    [InverseProperty("Desserts")]
    public virtual DessertType? DessertType { get; set; }

    [InverseProperty("Dessert")]
    public virtual ICollection<OrderedDessert> OrderedDesserts { get; set; } = new List<OrderedDessert>();

    [ForeignKey("StatusCode")]
    [InverseProperty("Desserts")]
    public virtual Status? StatusCodeNavigation { get; set; }
}
