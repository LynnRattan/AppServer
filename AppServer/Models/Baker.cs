using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

public partial class Baker
{
    [Key]
    public int BakerId { get; set; }

    [StringLength(100)]
    public string ConfectioneryName { get; set; } = null!;

    public double HighestPrice { get; set; }

    public int ConfectioneryTypeId { get; set; }

    public int StatusCode { get; set; }

    public double? Profits { get; set; }

    [ForeignKey("BakerId")]
    [InverseProperty("Baker")]
    public virtual User BakerNavigation { get; set; } = null!;

    [ForeignKey("ConfectioneryTypeId")]
    [InverseProperty("Bakers")]
    public virtual ConfectioneryType ConfectioneryType { get; set; } = null!;

    [InverseProperty("Baker")]
    public virtual ICollection<Dessert> Desserts { get; set; } = new List<Dessert>();

    [InverseProperty("Baker")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("StatusCode")]
    [InverseProperty("Bakers")]
    public virtual Status StatusCodeNavigation { get; set; } = null!;
}
