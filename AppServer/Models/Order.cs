using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    public int? StatusCode { get; set; }

    public int? UserId { get; set; }

    public int? BakerId { get; set; }

    public DateOnly? OrderDate { get; set; }

    public DateOnly? DispatchDate { get; set; }

    [StringLength(100)]
    public string Adress { get; set; } = null!;

    public double TotalPrice { get; set; }

    [ForeignKey("BakerId")]
    [InverseProperty("Orders")]
    public virtual Baker? Baker { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderedDessert> OrderedDesserts { get; set; } = new List<OrderedDessert>();

    [ForeignKey("StatusCode")]
    [InverseProperty("Orders")]
    public virtual Status? StatusCodeNavigation { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual User? User { get; set; }
}
