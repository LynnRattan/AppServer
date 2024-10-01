using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

public partial class Status
{
    [Key]
    public int StatusCode { get; set; }

    [StringLength(100)]
    public string? StatusName { get; set; }

    [InverseProperty("StatusCodeNavigation")]
    public virtual ICollection<Baker> Bakers { get; set; } = new List<Baker>();

    [InverseProperty("StatusCodeNavigation")]
    public virtual ICollection<Dessert> Desserts { get; set; } = new List<Dessert>();

    [InverseProperty("StatusCodeNavigation")]
    public virtual ICollection<OrderedDessert> OrderedDesserts { get; set; } = new List<OrderedDessert>();

    [InverseProperty("StatusCodeNavigation")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
