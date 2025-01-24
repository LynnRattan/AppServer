﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppServer.Models;

[Index("Mail", Name = "UQ__Users__2724B2D1070B2DD3", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(100)]
    public string? Mail { get; set; }

    [StringLength(100)]
    public string? Username { get; set; }

    [StringLength(100)]
    public string? Password { get; set; }

    [StringLength(100)]
    public string? ProfileName { get; set; }

    public int? UserTypeId { get; set; }

    public byte[]? ProfileImage { get; set; }

    [InverseProperty("BakerNavigation")]
    public virtual Baker? Baker { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("UserTypeId")]
    [InverseProperty("Users")]
    public virtual UserType? UserType { get; set; }
}
